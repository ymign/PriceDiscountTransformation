using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Core.Services;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Application.Pricing.Validation;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 计价应用服务，统一编排试算、确认、提交、取消、冲正和特殊项目识别。
/// </summary>
/// <remarks>
/// 这个服务刻意不把资金安全逻辑下沉到控制器中。控制器只负责 HTTP 入口，
/// 这里集中处理权威单价校验、confirm 幂等、请求指纹、三段式状态机、
/// 限额占用、追溯日志和事务边界。这样做的目的，是让 HIS、自助机、公众号
/// 等不同渠道共享同一套资金口径，避免各端在失败重试、取消落账、过期释放
/// 上出现不一致。
/// </remarks>
/// <summary>
/// 计价用例共享基类，集中承载事务、幂等响应兜底和资金链路通用辅助逻辑。
/// </summary>
public abstract class PricingUseCaseBase
{
    /// <summary>
    /// 计价引擎依赖，负责规则匹配和动作链执行；应用服务只关心它的输入输出，
    /// 不直接关心具体规则动作如何计算。
    /// </summary>
    private readonly IPricingEngine _engine;
    /// <summary>
    /// 计价请求日志仓储，负责保存幂等键、请求指纹、请求快照、响应快照和业务状态。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;
    /// <summary>
    /// 折价结果明细仓储，负责保存最终折价、封顶、公式调整结果，并参与 commit/cancel/expire 状态同步。
    /// </summary>
    private readonly IChargeDiscountDetailRepository _discountRepository;
    /// <summary>
    /// 限额占用仓储，负责时间窗、单日等额度的累计查询、锁行加锁和占用状态推进。
    /// </summary>
    private readonly ILimitOccupyRepository _limitRepository;
    /// <summary>
    /// 冲正日志仓储，负责读取已有冲正流水并参与 reverse 幂等判断。
    /// </summary>
    private readonly IChargeReverseLogRepository _reverseLogRepository;
    private readonly AuthorityPriceChecker _authorityPriceChecker;
    private readonly PricingIdempotencyService _idempotencyService;
    private readonly PricingRequestLogWriter _requestLogWriter;
    private readonly PricingTraceStepWriter _traceStepWriter;
    private readonly PricingDiscountDetailWriter _discountDetailWriter;
    private readonly PricingLimitOccupyWriter _limitOccupyWriter;
    private readonly PricingReverseLogWriter _reverseLogWriter;
    /// <summary>
    /// _options 配置对象，集中承载超时、清理间隔、单价校验等运行参数。
    /// </summary>
    private readonly PricingOptions _options;
    /// <summary>
    /// _logger 日志依赖，用于记录关键状态流转、幂等命中和异常上下文。
    /// </summary>
    private readonly ILogger _logger;
    private readonly IClock _clock;
    private readonly PricingIdempotentResponseReader _idempotentResponseReader;
    private readonly PricingTransactionExecutor _transactionExecutor;
    private readonly PricingSpecialFlagResolver _specialFlagResolver;
    private readonly PricingReverseHistoryReader _reverseHistoryReader;
    private readonly PricingSimulateWorkflow _simulateWorkflow;
    private readonly PricingConfirmWorkflow _confirmWorkflow;
    private readonly PricingCommitWorkflow _commitWorkflow;
    private readonly PricingCancelWorkflow _cancelWorkflow;
    private readonly PricingReverseWorkflow _reverseWorkflow;

    /// <summary>
    /// 初始化计价接口应用服务。
    /// </summary>
    /// <param name="calculationDependencies">计价计算侧依赖，包括计价引擎、规则头仓储和权威物价仓储。</param>
    /// <param name="repositories">计价持久化侧仓储集合，覆盖请求、明细、步骤、限额和冲正日志。</param>
    /// <param name="authorityPriceChecker">权威物价校验器。</param>
    /// <param name="idempotencyService">confirm 幂等服务。</param>
    /// <param name="requestLogWriter">计价请求日志写入器。</param>
    /// <param name="traceStepWriter">计价步骤日志写入器。</param>
    /// <param name="discountDetailWriter">折价明细写入器。</param>
    /// <param name="limitOccupyWriter">限额占用写入器。</param>
    /// <param name="reverseLogWriter">冲正日志写入器。</param>
    /// <param name="unitOfWork">工作单元，用于创建事务边界。</param>
    /// <param name="options">计价配置项，包括过期时间和单价校验开关。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    /// <param name="logger">日志组件，用于记录幂等命中、状态推进和异常上下文。</param>
    protected PricingUseCaseBase(
        PricingAppCalculationDependencies calculationDependencies,
        PricingAppPersistenceRepositories repositories,
        AuthorityPriceChecker authorityPriceChecker,
        PricingIdempotencyService idempotencyService,
        PricingRequestLogWriter requestLogWriter,
        PricingTraceStepWriter traceStepWriter,
        PricingDiscountDetailWriter discountDetailWriter,
        PricingLimitOccupyWriter limitOccupyWriter,
        PricingReverseLogWriter reverseLogWriter,
        RuntimePackageTraceResolver runtimePackageTraceResolver,
        IUnitOfWork unitOfWork,
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger logger)
    {
        _engine = calculationDependencies.Engine;
        _requestLogRepository = repositories.RequestLogRepository;
        _discountRepository = repositories.DiscountRepository;
        _limitRepository = repositories.LimitRepository;
        _reverseLogRepository = repositories.ReverseLogRepository;
        _authorityPriceChecker = authorityPriceChecker;
        _idempotencyService = idempotencyService;
        _requestLogWriter = requestLogWriter;
        _traceStepWriter = traceStepWriter;
        _discountDetailWriter = discountDetailWriter;
        _limitOccupyWriter = limitOccupyWriter;
        _reverseLogWriter = reverseLogWriter;
        _options = options.Value;
        _clock = clock;
        _logger = logger;
        _idempotentResponseReader = new PricingIdempotentResponseReader(NullLogger<PricingIdempotentResponseReader>.Instance);
        _transactionExecutor = new PricingTransactionExecutor(unitOfWork, NullLogger<PricingTransactionExecutor>.Instance);
        _specialFlagResolver = new PricingSpecialFlagResolver(calculationDependencies.HeaderRepository, clock);
        _reverseHistoryReader = new PricingReverseHistoryReader(repositories.ReverseLogRepository);
        _simulateWorkflow = new PricingSimulateWorkflow(
            _engine,
            _authorityPriceChecker,
            _requestLogWriter,
            _traceStepWriter,
            runtimePackageTraceResolver,
            _clock,
            _logger);
        _confirmWorkflow = new PricingConfirmWorkflow(
            _engine,
            _requestLogRepository,
            _authorityPriceChecker,
            _idempotencyService,
            _requestLogWriter,
            _traceStepWriter,
            _discountDetailWriter,
            _limitOccupyWriter,
            runtimePackageTraceResolver,
            _transactionExecutor,
            _idempotentResponseReader,
            _limitRepository,
            _options,
            _clock,
            _logger);
        _commitWorkflow = new PricingCommitWorkflow(
            _requestLogRepository,
            _discountRepository,
            _limitRepository,
            _transactionExecutor,
            _options,
            _clock,
            _logger);
        _cancelWorkflow = new PricingCancelWorkflow(
            _requestLogRepository,
            _discountRepository,
            _limitRepository,
            _transactionExecutor,
            _clock,
            _logger);
        _reverseWorkflow = new PricingReverseWorkflow(
            _requestLogRepository,
            _discountRepository,
            _limitRepository,
            _reverseLogRepository,
            _reverseLogWriter,
            _limitOccupyWriter,
            _transactionExecutor,
            _reverseHistoryReader,
            _clock,
            _logger);
    }

    /// <summary>
    /// 执行试算计价。
    /// </summary>
    /// <param name="request">统一计价请求。试算允许重复调用，不要求稳定业务号。</param>
    /// <returns>返回本次试算的金额、数量、命中规则和追溯步骤。</returns>
    /// <remarks>
    /// 试算用于页面展示或影子验证，不写正式折价明细，也不占用限额额度。
    /// 但它仍然校验权威单价并保存请求日志和步骤日志，因为后续排查"页面为什么显示这个价格"
    /// 时需要看到当时的规则匹配过程。
    /// </remarks>
    protected async Task<PricingCalculateResponse> ExecuteSimulateAsync(PricingCalculateRequest request)
    {
        return await _simulateWorkflow.ExecuteAsync(request);
    }

    /// <summary>
    /// 执行正式确认计价，并把结果推进到待落账保护状态。
    /// </summary>
    /// <param name="request">统一计价请求。confirm 必须传入稳定的 <c>BusinessRequestNo</c>。</param>
    /// <returns>返回可供 HIS 落账使用的最终计价结果。</returns>
    /// <exception cref="ArgumentException">业务号为空时抛出，防止 confirm 超时重试重复占额。</exception>
    /// <exception cref="InvalidOperationException">同一业务号参数变化时抛出 IDEMPOTENT_CONFLICT。</exception>
    /// <remarks>
    /// confirm 是资金安全链路的核心入口。它不直接表示 HIS 已经落账成功，而是表示计价中心
    /// 已经计算出结果并暂时占用额度。只有 HIS 写入收费明细成功并调用 commit 后，状态才会进入
    /// CONFIRMED；如果 HIS 失败或用户取消，则必须 cancel；如果长时间没有回调，则由后台 expire。
    /// </remarks>
    protected async Task<PricingCalculateResponse> ExecuteConfirmAsync(PricingCalculateRequest request)
    {
        return await _confirmWorkflow.ExecuteAsync(request);
    }

    /// <summary>
    /// HIS 落账成功后提交计价结果，主子项目原子处理。
    /// </summary>
    /// <param name="request">commit 请求，必须携带 confirm 返回的请求 ID。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// <para>
    /// commit 只允许从 CONFIRM_PENDING 进入 CONFIRMED。它不能重复提交，也不能提交已取消或已过期记录。
    /// 这样能保证报表口径只统计真正落账成功的折价明细和限额占用。
    /// </para>
    /// <para>
    /// 主子项目原子性保证：
    /// 当请求涉及主子项目（通过 ResultGroupNo 关联）时，按 ResultGroupNo 分组处理。
    /// 同组内的主项目和子项在同一事务中一起 commit，如果同组内任何一项 commit 失败，整组回滚。
    /// 不同组之间独立处理，某组失败不影响其他组。
    /// </para>
    /// </remarks>
    protected async Task ExecuteCommitAsync(PricingCommitRequest request)
    {
        await _commitWorkflow.ExecuteAsync(request);
    }

    /// <summary>
    /// HIS 落账失败、支付失败或用户取消时释放 confirm 保护状态，主子项目原子释放。
    /// </summary>
    /// <param name="request">cancel 请求，必须携带 confirm 返回的请求 ID。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// <para>
    /// cancel 只允许处理 CONFIRM_PENDING。它的业务含义不是"退费"，而是"确认结果没有被 HIS 使用"，
    /// 因此应该释放待确认占额，并把折价明细标记为 CANCELLED，避免进入正式收费报表。
    /// </para>
    /// <para>
    /// 主子项目原子性保证：
    /// 同组内的限额占用和折价明细在同一事务中一起取消。
    /// 原子性保证：不会出现主项目取消但子项目残留的情况。
    /// </para>
    /// </remarks>
    protected async Task ExecuteCancelAsync(PricingCancelRequest request)
    {
        await _cancelWorkflow.ExecuteAsync(request);
    }

    /// <summary>
    /// 对已经落账确认的计价结果执行冲正，主子项目原子处理。
    /// </summary>
    /// <param name="request">冲正请求，支持按收费明细、项目和片段定位部分退费。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// <para>
    /// reverse 的语义不同于 cancel。cancel 处理"未落账的确认结果"，reverse 处理"已经落账的收费结果"。
    /// 部分退费必须校验“本次退费 + 历史已退 <c>&lt;=</c> 原有效收费数量”。当日退费通过负向占额释放额度；
    /// 隔日退费只记录冲正事实，不回写历史窗口，重收时按重收当天业务时间重新校验。
    /// </para>
    /// <para>
    /// 主子项目原子性保证：
    /// 退费时按 ResultGroupNo 分组原子处理。校验：本次退费 + 历史已退不超过原有效收费（按组校验）。
    /// 同组内的主项目和子项一起冲正，不会出现主项目冲正成功但子项目残留的情况。
    /// </para>
    /// </remarks>
    protected async Task ExecuteReverseAsync(PricingReverseRequest request)
    {
        await _reverseWorkflow.ExecuteAsync(request);
    }

    /// <summary>
    /// 查询项目是否属于必须调用统一计价中心的特殊项目。
    /// </summary>
    /// <param name="itemCode">HIS 项目编码。</param>
    /// <returns>返回特殊项目标识和已发布规则数量。</returns>
    /// <remarks>
    /// 渠道侧可以用这个接口决定是否弹出特殊计价流程。若服务不可用，渠道侧应按文档要求保守处理，
    /// 不能自行按普通单价收费。
    /// </remarks>
    protected async Task<SpecialFlagResponse> ExecuteGetSpecialFlagAsync(string itemCode)
    {
        return await _specialFlagResolver.ResolveAsync(itemCode);
    }

}
