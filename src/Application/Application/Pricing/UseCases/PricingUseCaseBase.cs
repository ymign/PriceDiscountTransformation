using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
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
    /// 规则头仓储，用于 special-flag 查询，判断某个项目当前是否存在已发布特殊规则。
    /// </summary>
    private readonly IRuleHeaderRepository _headerRepository;
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
    /// 冲正日志仓储，负责记录退费、作废、撤销时为什么释放或作废原占用。
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
    /// 工作单元，用于把请求日志、折价明细、限额占用和冲正日志放入同一事务。
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;
    /// <summary>
    /// _options 配置对象，集中承载超时、清理间隔、单价校验等运行参数。
    /// </summary>
    private readonly PricingOptions _options;
    /// <summary>
    /// _logger 日志依赖，用于记录关键状态流转、幂等命中和异常上下文。
    /// </summary>
    private readonly ILogger _logger;
    private readonly IClock _clock;

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
        IUnitOfWork unitOfWork,
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger logger)
    {
        _engine = calculationDependencies.Engine;
        _headerRepository = calculationDependencies.HeaderRepository;
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
        _unitOfWork = unitOfWork;
        _options = options.Value;
        _clock = clock;
        _logger = logger;
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
        var items = PricingRequestGuard.GetRequiredItems(request);

        var firstItem = items[0];
        _logger.LogInformation(
            "SIMULATE 开始 SourceSystem={SourceSystem}, PatientId={PatientId}, ItemCode={ItemCode}, InputQty={InputQty}",
            request.SourceSystem, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        // 即使是试算，也不允许在启用权威单价校验时用错误单价继续计算。
        // 这样可以提前暴露 HIS、自助机或公众号传参错误，避免试算展示和最终确认口径不一致。
        await _authorityPriceChecker.CheckAsync(items);

        // shouldLockLimits=false 表示执行器只按历史 PENDING/CONFIRMED 数据试算，
        // 不创建 PR_LIMIT_LOCK 锁，也不写 PR_LIMIT_OCCUPY 占用。
        // 批量场景下创建 BatchPricingContext，确保同批内多个项目的限额累计和互斥判断正确。
        var inRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        var inRequestLimitOccupies = new List<LimitOccupy>();
        var batchContext = items.Count > 1 ? new BatchPricingContext() : null;
        var calculations = new List<ItemPricingCalculation>(items.Count);
        foreach (var item in items)
        {
            var context = PricingContextFactory.Create(new PricingContextBuildInput
            {
                Request = request,
                Item = item,
                CallType = "SIMULATE",
                ShouldLockLimits = false,
                InRequestOccupiedQtyByLimitDimension = inRequestOccupiedQtyByLimitDimension,
                InRequestLimitOccupies = inRequestLimitOccupies
            });
            var result = await _engine.CalculateAsync(context, batchContext);
            AccumulateInRequestLimits(inRequestOccupiedQtyByLimitDimension, inRequestLimitOccupies, result);
            calculations.Add(new ItemPricingCalculation(item, result));
        }

        // 试算不会进入资金状态机，但保留请求和步骤日志可以支持后续页面解释、问题复盘和影子对账。
        var requestLog = await _requestLogWriter.SaveAsync(new RequestLogSaveInput
        {
            Request = request,
            Items = items,
            Calculations = calculations,
            CallType = "SIMULATE",
            BusinessStatus = BusinessStatusCodes.Simulated
        });
        await _traceStepWriter.SaveAsync(requestLog.RequestId, requestLog.TraceId, calculations);

        // 响应快照不是幂等必需，但可以让追溯查询直接展示当时返回给渠道的结果。
        var response = PricingResponseBuilder.Build(requestLog.RequestId, calculations, _clock.Now);
        await _requestLogWriter.SaveResponseJsonAsync(requestLog, response);
        return response;
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
        var items = PricingRequestGuard.GetRequiredItems(request);

        // 无论后续成功或失败，先记录请求到达信息，便于排查"渠道说调了但计价中心没收到"的问题。
        // 只记录首项的 itemCode，多项目场景通过 RequestId 在追溯链路中查看完整列表。
        var firstItem = items[0];
        _logger.LogInformation(
            "CONFIRM 开始 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, PatientId={PatientId}, ItemCode={ItemCode}, InputQty={InputQty}",
            request.SourceSystem, request.BusinessRequestNo, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        // requestNo 是一次 HTTP 调用流水，超时重试时可能变化；BusinessRequestNo 才代表一次收费确认动作。
        // 如果这里允许空业务号，服务端无法区分"同一动作重试"和"新收费动作"，会导致重复占额。
        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            _logger.LogWarning(
                "CONFIRM 校验失败: BusinessRequestNo 为空, SourceSystem={SourceSystem}",
                request.SourceSystem);
            PricingRequestGuard.EnsureConfirmRequest(request);
        }

        // 单价错误属于资金风险，不应该进入规则引擎后再修正。这里直接失败，让渠道重新取价或修正参数。
        await _authorityPriceChecker.CheckAsync(items);

        // 幂等键只定位"是否同一次业务动作"；请求指纹负责证明"这次业务动作的参数没有悄悄变化"。
        // 两者不能互相替代，否则用户改了部位、数量或 extraParams 后可能继续复用旧结果。
        var idempotency = await _idempotencyService.CheckConfirmAsync(request, items, "CONFIRM");
        var fingerprint = idempotency.Fingerprint;
        if (idempotency.ExistingRequest is { } existing)
        {
            // 同一业务号下参数不一致时必须阻断。这里不能覆盖旧记录，也不能重新计算，
            // 否则调用方会以为重试成功，实际额度和折价明细已经对应另一组参数。
            _idempotencyService.EnsureSameFingerprint(existing, fingerprint, request.BusinessRequestNo!);

            // 参数一致时直接返回首次响应快照，不重新执行规则，也不再次写占额。
            // 这是 confirm 超时重试时避免重复占额的关键路径。
            _logger.LogInformation(
                "幂等命中 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, RequestId={RequestId}, ItemCode={ItemCode}, OriginalStatus={Status}",
                request.SourceSystem, request.BusinessRequestNo, existing.RequestId, existing.ItemCode, existing.BusinessStatus);
            return await BuildIdempotentResponse(existing);
        }

        // confirm 产生的三类记录必须一起成功或一起回滚：
        // 1. PR_CHARGE_REQUEST_LOG：请求状态和响应快照；
        // 2. PR_CHARGE_DISCOUNT_DETAIL：待确认折价结果；
        // 3. PR_LIMIT_OCCUPY：待确认额度占用。
        // 如果其中任一张表失败，不能留下半条资金链路。
        return await ExecuteInTransactionAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[]
            {
                BuildIdempotencyLockKey(request.SourceSystem, request.BusinessRequestNo!, "CONFIRM")
            });

            var existingInTransaction = await _requestLogRepository.GetByBusinessKeyAsync(
                request.SourceSystem, request.BusinessRequestNo!, "CONFIRM");
            if (existingInTransaction is not null)
            {
                _idempotencyService.EnsureSameFingerprint(
                    existingInTransaction,
                    fingerprint,
                    request.BusinessRequestNo!);

                _logger.LogInformation(
                    "CONFIRM 事务内幂等命中 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, RequestId={RequestId}",
                    request.SourceSystem, request.BusinessRequestNo, existingInTransaction.RequestId);
                return await BuildIdempotentResponse(existingInTransaction);
            }

            // shouldLockLimits=true 表示限额执行器会在计算窗口累计前锁定 PR_LIMIT_LOCK，
            // 以防两个渠道同时确认同一患者同一项目时一起看到"还剩额度"。
            // 批量场景下创建 BatchPricingContext，确保同批内限额累计和互斥判断正确。
            var inRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var inRequestLimitOccupies = new List<LimitOccupy>();
            var batchContext = items.Count > 1 ? new BatchPricingContext() : null;
            var calculations = new List<ItemPricingCalculation>(items.Count);
            foreach (var item in items)
            {
                var context = PricingContextFactory.Create(new PricingContextBuildInput
                {
                    Request = request,
                    Item = item,
                    CallType = "CONFIRM",
                    ShouldLockLimits = true,
                    InRequestOccupiedQtyByLimitDimension = inRequestOccupiedQtyByLimitDimension,
                    InRequestLimitOccupies = inRequestLimitOccupies
                });
                var result = await _engine.CalculateAsync(context, batchContext);
                AccumulateInRequestLimits(inRequestOccupiedQtyByLimitDimension, inRequestLimitOccupies, result);
                calculations.Add(new ItemPricingCalculation(item, result));
            }

            // 请求日志先落库并拿到 RequestId，后续步骤、折价明细和占额都用它串联。
            var requestLog = await _requestLogWriter.SaveAsync(new RequestLogSaveInput
            {
                Request = request,
                Items = items,
                Calculations = calculations,
                CallType = "CONFIRM",
                BusinessStatus = BusinessStatusCodes.ConfirmPending,
                Fingerprint = fingerprint
            });
            await _traceStepWriter.SaveAsync(requestLog.RequestId, requestLog.TraceId, calculations);

            // confirm 返回后，HIS 会按本次响应落账整批收费明细；commit 阶段也必须能用
            // confirm 快照逐项核对 HIS 的真实落账结果。因此这里保存所有费用明细的计价结果。
            // 只有特殊规则产生的限额草稿需要写保护占用，普通项目只作为 commit 对账基准保存。
            foreach (var calculation in calculations)
            {
                await _discountDetailWriter.SaveAsync(new DiscountDetailSaveInput
                {
                    RequestId = requestLog.RequestId,
                    TraceId = requestLog.TraceId,
                    Request = request,
                    Item = calculation.Item,
                    Result = calculation.Result,
                    Status = OccupyStatusCodes.Pending
                });
                if (calculation.Result.IsSpecialItem)
                {
                    await _limitOccupyWriter.SaveAsync(
                        requestLog.RequestId,
                        requestLog.TraceId,
                        calculation.Item,
                        calculation.Result);
                }
            }

            // 最后保存响应快照。幂等重试优先读取这个快照，保证返回内容和首次 confirm 完全一致。
            var response = PricingResponseBuilder.Build(
                requestLog.RequestId,
                calculations,
                _clock.Now,
                requestLog.RequestAt.AddMinutes(_options.ConfirmExpireMinutes));
            await _requestLogWriter.SaveResponseJsonAsync(requestLog, response);

            // 记录最终结果摘要，便于对账和异常排查。
            // 包含 RequestId 以便后续通过追溯接口查看完整计算过程。
            _logger.LogInformation(
                "CONFIRM 成功 RequestId={RequestId}, SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, ItemCode={ItemCode}, FinalQty={FinalQty}, FinalAmount={FinalAmount}, IsSpecialItem={IsSpecialItem}",
                requestLog.RequestId, request.SourceSystem, request.BusinessRequestNo,
                firstItem.ItemCode, response.FinalQty, response.FinalAmount, response.IsSpecialItem);

            return response;
        });
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
        PricingRequestGuard.EnsureCommitRequest(request);

        _logger.LogInformation(
            "COMMIT 开始 RequestId={RequestId}, ChargeNo={ChargeNo}",
            request.RequestId, request.ChargeNo);

        // 请求日志、折价明细、限额占用三张表必须在同一事务内推进到 CONFIRMED。
        // 如果任何一步失败，整体回滚，避免报表看到"请求已确认但占额仍待确认"的断裂状态。
        await ExecuteInTransactionAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[] { BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new BizException(
                    BizErrorCode.RequestNotFound,
                    404,
                    $"请求不存在: {request.RequestId}");

            if (log.BusinessStatus == BusinessStatusCodes.Confirmed || log.BusinessStatus == BusinessStatusCodes.Committed)
            {
                if ((request.ActualItems?.Count ?? 0) > 0 || request.ActualTotalAmount.HasValue)
                {
                    var confirmedDetails = await _discountRepository.GetByRequestIdAsync(request.RequestId);
                    PricingCommitActualValidator.Validate(request, confirmedDetails, requireActualItems: false);
                }

                _logger.LogInformation(
                    "COMMIT 幂等命中 RequestId={RequestId}, 当前状态={Status}",
                    request.RequestId, log.BusinessStatus);
                return;
            }

            // 只允许待确认保护状态 commit。已经取消、过期或确认的记录再 commit，
            // 通常代表渠道回调乱序或重复回调，必须暴露给调用方处理。
            if (log.BusinessStatus != BusinessStatusCodes.ConfirmPending)
            {
                _logger.LogWarning(
                    "COMMIT 状态校验失败 RequestId={RequestId}, 当前状态={Status}, 期望=CONFIRM_PENDING",
                    request.RequestId, log.BusinessStatus);
                throw new BizException(
                    BizErrorCode.RequestStatusNotAllowed,
                    409,
                    $"只有CONFIRM_PENDING状态可以COMMIT, 当前: {log.BusinessStatus}");
            }

            // confirm 结果有有效期。过期后额度可能已经释放给其他收费动作，
            // 因此不能再接受迟到 commit，调用方必须重新 confirm。
            if (_clock.Now > log.RequestAt.AddMinutes(_options.ConfirmExpireMinutes))
            {
                _logger.LogWarning(
                    "COMMIT 已过期 RequestId={RequestId}, RequestAt={RequestAt}, 过期分钟数={ExpireMinutes}",
                    request.RequestId, log.RequestAt, _options.ConfirmExpireMinutes);
                throw new BizException(
                    BizErrorCode.RequestStatusNotAllowed,
                    409,
                    "确认计价结果已过期，请重新 confirm");
            }

            // commit 是 HIS 真正落账后的资金状态推进。推进前必须把 HIS 实际落账明细
            // 与 confirm 保存的折价明细逐项比对，避免 HIS 少收、多收或数量不一致后仍进入 CONFIRMED。
            var details = await _discountRepository.GetByRequestIdAsync(request.RequestId);
            PricingCommitActualValidator.Validate(request, details, requireActualItems: true);

            // 请求日志进入 CONFIRMED，表示 HIS 已经完成本地落账。
            log.BusinessStatus = BusinessStatusCodes.Confirmed;
            log.ChargeNo = request.ChargeNo ?? log.ChargeNo;
            log.ResponseAt = _clock.Now;
            await _requestLogRepository.UpdateAsync(log);

            // 折价明细和限额占用同步确认。后续窗口累计会把 CONFIRMED 记录计入净占用。
            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Confirmed);
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Confirmed);

            _logger.LogInformation(
                "COMMIT 成功 RequestId={RequestId}, SourceSystem={SourceSystem}, ItemCode={ItemCode}, ChargeNo={ChargeNo}",
                request.RequestId, log.SourceSystem, log.ItemCode, log.ChargeNo);
        });
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
        PricingRequestGuard.EnsureCancelRequest(request);

        _logger.LogInformation(
            "CANCEL 开始 RequestId={RequestId}",
            request.RequestId);

        // 与 commit 一样，cancel 也必须同步更新请求日志、折价明细和限额占用。
        // 否则会出现额度释放了但明细还在 PENDING，或明细取消了但额度仍占着的资金口径断裂。
        await ExecuteInTransactionAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[] { BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new BizException(
                    BizErrorCode.RequestNotFound,
                    404,
                    $"请求不存在: {request.RequestId}");

            if (log.BusinessStatus == BusinessStatusCodes.Cancelled || log.BusinessStatus == BusinessStatusCodes.Expired)
            {
                _logger.LogInformation(
                    "CANCEL 幂等命中 RequestId={RequestId}, 当前状态={Status}",
                    request.RequestId, log.BusinessStatus);
                return;
            }

            // 只有待落账的 confirm 可以取消。已 CONFIRMED 的记录代表 HIS 已经收费，
            // 应走 reverse，而不是 cancel。
            if (log.BusinessStatus != BusinessStatusCodes.ConfirmPending)
            {
                _logger.LogWarning(
                    "CANCEL 状态校验失败 RequestId={RequestId}, 当前状态={Status}, 期望=CONFIRM_PENDING",
                    request.RequestId, log.BusinessStatus);
                throw new BizException(
                    BizErrorCode.RequestStatusNotAllowed,
                    409,
                    $"只有CONFIRM_PENDING状态可以CANCEL, 当前: {log.BusinessStatus}");
            }

            // 请求日志标记为取消后，对账和追溯可以明确知道这次 confirm 没有形成正式收费。
            log.BusinessStatus = BusinessStatusCodes.Cancelled;
            log.ResponseAt = _clock.Now;
            await _requestLogRepository.UpdateAsync(log);

            // CANCELLED 占额不参与后续限额累计，相当于释放 confirm 阶段的待确认保护额度。
            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Cancelled);
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Cancelled);

            _logger.LogInformation(
                "CANCEL 成功 RequestId={RequestId}, SourceSystem={SourceSystem}, ItemCode={ItemCode}, 限额已释放",
                request.RequestId, log.SourceSystem, log.ItemCode);
        });
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
        PricingRequestGuard.EnsureReverseRequest(request);
        var reverseNo = request.ReverseNo!;

        _logger.LogInformation(
            "REVERSE 开始 OriginalRequestId={OriginalRequestId}, ItemCode={ItemCode}, ReverseQty={ReverseQty}",
            request.OriginalRequestId, request.ItemCode, request.ReverseQty);

        // 冲正会同时影响请求状态、折价明细、限额占用和冲正日志。任何一环失败都不能部分提交。
        await ExecuteInTransactionAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[]
            {
                BuildRequestLockKey(request.OriginalRequestId),
                BuildReverseLockKey(request.OriginalRequestId, reverseNo)
            });

            var log = await _requestLogRepository.GetByIdAsync(request.OriginalRequestId)
                ?? throw new BizException(
                    BizErrorCode.RequestNotFound,
                    404,
                    $"原请求不存在: {request.OriginalRequestId}");

            var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
            var sameReverseNo = reverseLogs.FirstOrDefault(r =>
                string.Equals(r.ReverseNo, request.ReverseNo, StringComparison.OrdinalIgnoreCase));
            if (sameReverseNo is not null)
            {
                if (!PricingReverseDetailSelector.IsSameReverseRequest(sameReverseNo, request))
                {
                    throw new BizException(
                        BizErrorCode.IdempotencyConflict,
                        409,
                        $"ReverseNo={request.ReverseNo} 已存在，但本次冲正参数与首次请求不一致");
                }

                _logger.LogInformation(
                    "REVERSE 幂等命中 OriginalRequestId={OriginalRequestId}, ReverseNo={ReverseNo}",
                    request.OriginalRequestId, request.ReverseNo);
                return;
            }

            // 只有已落账确认的记录才能 reverse。CONFIRM_PENDING 应该 cancel；
            // CANCELLED/EXPIRED 没有形成正式收费，不应该再冲正。
            if (!IsCommittedBusinessStatus(log.BusinessStatus))
            {
                _logger.LogWarning(
                    "REVERSE 状态校验失败 OriginalRequestId={OriginalRequestId}, 当前状态={Status}, 期望=CONFIRMED/COMMITTED",
                    request.OriginalRequestId, log.BusinessStatus);
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    $"只有CONFIRMED或COMMITTED状态可以REVERSE, 当前: {log.BusinessStatus}");
            }

            var details = await _discountRepository.GetByRequestIdAsync(request.OriginalRequestId);
            var matchedDetails = PricingReverseDetailSelector.FilterReverseDetails(details, request);
            if (matchedDetails.Count == 0)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    "未找到可退费的原收费明细");
            }

            var allOriginalQty = details
                .Where(d => d.Status == BusinessStatusCodes.Confirmed || d.Status == BusinessStatusCodes.Committed)
                .Sum(d => d.FinalQty ?? 0);
            var originalQty = matchedDetails.Sum(d => d.FinalQty ?? 0);
            var originalAmt = matchedDetails.Sum(d => d.FinalAmt ?? 0);
            var reverseQty = request.ReverseQty ?? originalQty;
            if (reverseQty <= 0)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    "退费数量必须大于0");
            }

            var historicalReversedQty = await GetHistoricalReversedQtyAsync(request);
            var allHistoricalReversedQty = await GetHistoricalReversedQtyAsync(request.OriginalRequestId);
            if (historicalReversedQty + reverseQty > originalQty)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    $"原有效数量={originalQty}, 历史已退={historicalReversedQty}, 本次退费={reverseQty}");
            }

            var reverseAmt = request.ReverseAmt ??
                (originalQty == 0 ? 0 : originalAmt * reverseQty / originalQty);
            reverseAmt = PricingAmountRounder.RoundFinal(reverseAmt);
            var historicalReversedAmt = await GetHistoricalReversedAmtAsync(request);
            if (historicalReversedAmt + reverseAmt > originalAmt)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    $"原有效金额={originalAmt}, 历史已退={historicalReversedAmt}, 本次退费={reverseAmt}");
            }

            var isFullReverse =
                allHistoricalReversedQty + reverseQty == allOriginalQty &&
                historicalReversedAmt + reverseAmt == originalAmt;

            // 主子项目原子性要求：当原请求包含主子项目（通过 ResultGroupNo 关联）时，
            // 退费校验必须按组进行，确保同组内的退费数量不超过该组的原有效收费数量。
            // 这样可以防止"主项目退了但子项目没退"或"子项目退超了"的不一致情况。
            var groupedDetails = matchedDetails
                .Where(d => !string.IsNullOrWhiteSpace(d.ResultGroupNo))
                .GroupBy(d => d.ResultGroupNo)
                .ToList();
            foreach (var group in groupedDetails)
            {
                var groupOriginalQty = group.Sum(d => d.FinalQty ?? 0);
                var groupOriginalAmt = group.Sum(d => d.FinalAmt ?? 0);

                var groupHistoricalQty = reverseLogs
                    .Where(r => group.Any(d =>
                        string.Equals(d.ItemCode, r.ItemCode, StringComparison.OrdinalIgnoreCase) &&
                        d.ChargeDetailNo == r.ChargeDetailNo))
                    .Sum(r => r.ReverseQty ?? 0);
                var groupHistoricalAmt = reverseLogs
                    .Where(r => group.Any(d =>
                        string.Equals(d.ItemCode, r.ItemCode, StringComparison.OrdinalIgnoreCase) &&
                        d.ChargeDetailNo == r.ChargeDetailNo))
                    .Sum(r => r.ReverseAmt ?? 0);

                // 按组分配本次退费数量（按原始数量比例分摊，最后一组用扣减法保证总量一致）
                decimal groupReverseQty, groupReverseAmt;
                if (group.Key == groupedDetails.Last().Key)
                {
                    var allocatedQty = groupedDetails.Where(g => g.Key != group.Key)
                        .Sum(g => originalQty == 0 ? 0 : reverseQty * g.Sum(d => d.FinalQty ?? 0) / originalQty);
                    var allocatedAmt = groupedDetails.Where(g => g.Key != group.Key)
                        .Sum(g => originalAmt == 0 ? 0 : reverseAmt * g.Sum(d => d.FinalAmt ?? 0) / originalAmt);
                    groupReverseQty = reverseQty - allocatedQty;
                    groupReverseAmt = reverseAmt - allocatedAmt;
                }
                else
                {
                    var groupRatio = originalQty == 0 ? 0 : groupOriginalQty / originalQty;
                    groupReverseQty = reverseQty * groupRatio;
                    groupReverseAmt = originalAmt == 0 ? 0 : reverseAmt * groupOriginalAmt / originalAmt;
                }

                if (groupHistoricalQty + groupReverseQty > groupOriginalQty)
                {
                    throw new BizException(
                        BizErrorCode.ReverseNotAllowed,
                        409,
                        $"ResultGroupNo={group.Key}, 组原有效数量={groupOriginalQty}, 组历史已退={groupHistoricalQty}, 组本次退费={groupReverseQty}");
                }

                if (groupHistoricalAmt + groupReverseAmt > groupOriginalAmt)
                {
                    throw new BizException(
                        BizErrorCode.ReverseNotAllowed,
                        409,
                        $"ResultGroupNo={group.Key}, 组原有效金额={groupOriginalAmt}, 组历史已退={groupHistoricalAmt}, 组本次退费={groupReverseAmt}");
                }
            }

            // 全退时原请求整体进入 REVERSED，旧占额不再参与累计；部分退费则保留原 CONFIRMED，
            // 并用负向占额扣减当前累计，避免把未退数量也释放掉。
            if (isFullReverse)
            {
                log.BusinessStatus = BusinessStatusCodes.Reversed;
                log.ResponseAt = _clock.Now;
                await _requestLogRepository.UpdateAsync(log);

                await _discountRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, OccupyStatusCodes.Reversed);
                await _limitRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, OccupyStatusCodes.Reversed);
            }

            var reverseTime = request.ReverseTime ?? _clock.Now;
            var reverseRequestId = await _reverseLogWriter.SaveRequestLogAsync(new ReverseRequestLogSaveInput
            {
                Request = request,
                OriginalLog = log,
                MatchedDetails = matchedDetails,
                ReverseQty = reverseQty,
                ReverseAmt = reverseAmt,
                ReverseTime = reverseTime
            });
            if (!isFullReverse)
            {
                await _limitOccupyWriter.InsertNegativeAsync(new NegativeLimitOccupyInput
                {
                    Request = request,
                    MatchedDetails = matchedDetails,
                    ReverseRequestId = reverseRequestId,
                    TraceId = log.TraceId,
                    ReverseQty = reverseQty,
                    ReverseAmt = reverseAmt,
                    ReverseTime = reverseTime
                });
            }

            // 这张表回答"为什么原来的收费结果被冲掉"，也是后续财务追查和退费口径复盘的入口。
            await _reverseLogWriter.SaveReverseLogAsync(new ReverseLogSaveInput
            {
                Request = request,
                OriginalLog = log,
                MatchedDetails = matchedDetails,
                ReverseRequestId = reverseRequestId,
                ReverseQty = reverseQty,
                ReverseAmt = reverseAmt,
                ReverseTime = reverseTime
            });

            _logger.LogInformation(
                "REVERSE 成功 OriginalRequestId={OriginalRequestId}, ItemCode={ItemCode}, ReverseQty={ReverseQty}, ReverseAmt={ReverseAmt}, 全退={IsFullReverse}",
                request.OriginalRequestId, matchedDetails.FirstOrDefault()?.ItemCode,
                reverseQty, reverseAmt, isFullReverse);
        });
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
        var normalizedItemCode = NormalizeString(itemCode)
            ?? throw new ArgumentException("项目编码不能为空", nameof(itemCode));

        // special-flag 是渠道是否进入特殊计价流程的入口，必须只统计当前有效规则。
        // 未来生效或已经过期的规则不能提前改变渠道行为。
        var rules = await _headerRepository.GetByItemCodeAsync(normalizedItemCode);
        var now = _clock.Now;
        var published = rules
            .Where(r => r.Status == RuleStatusCodes.Published && r.IsEnabled == EnableFlag.Yes)
            .Where(r => r.IsEffectiveAt(now))
            .ToList();

        return new SpecialFlagResponse
        {
            ItemCode = normalizedItemCode,
            IsSpecial = published.Count > 0,
            RuleCount = published.Count,
            RollbackMode = ResolveRollbackMode(published)
        };
    }

    private static string ResolveRollbackMode(IReadOnlyList<RuleAggregate> rules)
    {
        if (rules.Count == 0)
        {
            return "STOP_CHARGE";
        }

        // 多条有效规则同时命中同一项目时，渠道降级必须按最保守策略执行。
        // STOP_CHARGE 优先级最高，其次人工复核，只有所有规则均允许时才返回 LEGACY_EQUIVALENT。
        var modes = rules
            .Select(r => NormalizeString(r.RollbackMode) ?? "STOP_CHARGE")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (modes.Contains("STOP_CHARGE"))
        {
            return "STOP_CHARGE";
        }

        if (modes.Contains("MANUAL_REVIEW") || modes.Contains("NEW_SERVICE_ONLY"))
        {
            return modes.Contains("NEW_SERVICE_ONLY") ? "NEW_SERVICE_ONLY" : "MANUAL_REVIEW";
        }

        if (modes.Contains("LEGACY_EQUIVALENT"))
        {
            return "LEGACY_EQUIVALENT";
        }

        return "STOP_CHARGE";
    }

    private async Task<decimal> GetHistoricalReversedQtyAsync(PricingReverseRequest request)
    {
        var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
        var chargeDetailNo = NormalizeString(request.ChargeDetailNo);
        var itemCode = NormalizeString(request.ItemCode);

        return reverseLogs
            .Where(r => string.IsNullOrWhiteSpace(chargeDetailNo) ||
                        string.Equals(r.ChargeDetailNo, chargeDetailNo, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.IsNullOrWhiteSpace(itemCode) ||
                        string.Equals(r.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase))
            .Where(r => !request.PartSeq.HasValue || r.PartSeq == request.PartSeq)
            .Sum(r => r.ReverseQty ?? 0);
    }

    private async Task<decimal> GetHistoricalReversedQtyAsync(long originalRequestId)
    {
        var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(originalRequestId);
        return reverseLogs.Sum(r => r.ReverseQty ?? 0);
    }

    private async Task<decimal> GetHistoricalReversedAmtAsync(PricingReverseRequest request)
    {
        var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
        var chargeDetailNo = NormalizeString(request.ChargeDetailNo);
        var itemCode = NormalizeString(request.ItemCode);

        return reverseLogs
            .Where(r => string.IsNullOrWhiteSpace(chargeDetailNo) ||
                        string.Equals(r.ChargeDetailNo, chargeDetailNo, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.IsNullOrWhiteSpace(itemCode) ||
                        string.Equals(r.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase))
            .Where(r => !request.PartSeq.HasValue || r.PartSeq == request.PartSeq)
            .Sum(r => r.ReverseAmt ?? 0);
    }

    private static bool IsCommittedBusinessStatus(string? businessStatus)
    {
        return string.Equals(businessStatus, BusinessStatusCodes.Confirmed, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(businessStatus, BusinessStatusCodes.Committed, StringComparison.OrdinalIgnoreCase);
    }

    private static void AccumulateInRequestLimits(
        Dictionary<string, decimal> inRequestOccupiedQtyByLimitDimension,
        List<LimitOccupy> inRequestLimitOccupies,
        PricingResult result)
    {
        foreach (var occupy in result.LimitOccupies.Where(o =>
                     !string.IsNullOrWhiteSpace(o.LimitType) &&
                     !string.IsNullOrWhiteSpace(o.LimitDimensionCode)))
        {
            var key = BuildInRequestLimitKey(occupy.LimitType, occupy.LimitDimensionCode);
            inRequestOccupiedQtyByLimitDimension.TryGetValue(key, out var existingQty);
            inRequestOccupiedQtyByLimitDimension[key] = existingQty + occupy.OccupyQty;
            inRequestLimitOccupies.Add(occupy);
        }
    }

    private static string BuildInRequestLimitKey(string limitType, string? limitDimensionCode)
    {
        return $"{limitType.Trim().ToUpperInvariant()}:{limitDimensionCode?.Trim().ToUpperInvariant()}";
    }

    private static string BuildIdempotencyLockKey(
        string sourceSystem,
        string businessRequestNo,
        string callType)
    {
        return PricingLockKeyBuilder.BuildIdempotencyLockKey(sourceSystem, businessRequestNo, callType);
    }

    internal static string BuildRequestLockKey(long requestId)
    {
        return PricingLockKeyBuilder.BuildRequestLockKey(requestId);
    }

    private static string BuildReverseLockKey(long originalRequestId, string reverseNo)
    {
        return PricingLockKeyBuilder.BuildReverseLockKey(originalRequestId, reverseNo);
    }

    private async Task<PricingCalculateResponse> BuildIdempotentResponse(ChargeRequest log)
    {
        // 这是最严格的幂等语义。即使后来规则配置发生变化，重试同一业务号也必须得到首次 confirm 的结果。
        if (string.IsNullOrWhiteSpace(log.ResponseJson))
        {
            throw new BizException(
                BizErrorCode.IdempotencyResponseSnapshotInvalid,
                409,
                $"RequestId={log.RequestId} 的幂等响应快照缺失");
        }

        try
        {
            var response = JsonConvert.DeserializeObject<PricingCalculateResponse>(log.ResponseJson);
            if (response is not null)
            {
                return response;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "幂等响应快照解析失败 RequestId={RequestId}", log.RequestId);
        }

        throw new BizException(
            BizErrorCode.IdempotencyResponseSnapshotInvalid,
            409,
            $"RequestId={log.RequestId} 的幂等响应快照不可解析");
    }

    private async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        // 本服务的大部分资金操作会同时更新多张表。使用工作单元统一事务可以保证
        // 请求日志、折价明细、限额占用、冲正日志之间不会出现半提交。
        try
        {
            await _unitOfWork.BeginAsync();
            var result = await action();
            // 只有所有仓储操作都成功，才允许对外暴露本次状态推进。
            await _unitOfWork.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            // 资金链路宁可失败返回给渠道重试，也不能留下部分写入的请求或占额。
            _logger.LogError(ex, "事务执行异常，已回滚");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            await action();
            return true;
        });
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
