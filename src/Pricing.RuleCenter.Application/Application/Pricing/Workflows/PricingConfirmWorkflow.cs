using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Options;
using Microsoft.Extensions.Options;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 确认计价工作流，负责正式计价、幂等保护、额度占用和折价明细落库。
/// </summary>
/// <remarks>
/// <para>
/// confirm 是收费链路中唯一会占用限额的入口。它必须在事务内完成规则计算、请求日志、追踪步骤、
/// 折价明细和限额占用写入，避免 HIS 重试或并发收费造成额度突破。
/// </para>
/// <para>
/// 幂等键为 <c>sourceSystem + businessRequestNo + callType</c>。同一业务号重复 confirm 必须返回首次结果，
/// 不允许重复写占用；同一业务号但请求指纹不同则按幂等冲突处理。
/// </para>
/// <para>
/// confirm 不代表 HIS 已经收费成功。它的业务状态是 <c>CONFIRM_PENDING</c>：规则中心先把可收费结果、
/// 追溯步骤和保护占用固化下来，等待 HIS 写库成功后再由 commit 推进到 <c>CONFIRMED</c>。
/// 如果 HIS 写库失败、用户取消或支付失败，必须调用 cancel 释放这次保护占用。
/// </para>
/// <para>
/// 本 workflow 同时承担两个一致性要求：一是同一业务号重试不能重复占额；二是同一次请求内多条明细
/// 必须共享运行包和请求内累计，避免批量费用绕过同组互斥、时间窗限制或同手术封顶。
/// </para>
/// </remarks>
public sealed class PricingConfirmWorkflow
{
    private readonly PricingItemCalculationRunner _calculationRunner;

    /// <summary>
    /// 请求日志仓储，用于事务内二次查询幂等记录。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;

    /// <summary>
    /// 权威价格诊断器，用于记录渠道传价与 HIS 物价主数据的差异。
    /// </summary>
    private readonly AuthorityPriceChecker _authorityPriceChecker;

    /// <summary>
    /// 幂等服务，负责业务键查询、请求指纹构建和冲突判定。
    /// </summary>
    private readonly PricingIdempotencyService _idempotencyService;

    /// <summary>
    /// confirm 结果持久化服务，负责写请求、步骤、明细、占额和响应快照。
    /// </summary>
    private readonly PricingConfirmationPersistenceService _persistenceService;

    /// <summary>
    /// 运行包追踪解析器，保证同一 confirm 请求内使用同一个激活运行包快照。
    /// </summary>
    private readonly RuntimePackageTraceResolver _runtimePackageTraceResolver;

    /// <summary>
    /// 计价事务执行器，统一处理 Oracle 事务提交和回滚。
    /// </summary>
    private readonly PricingTransactionExecutor _transactionExecutor;

    /// <summary>
    /// 幂等响应读取器，用于重复 confirm 时复用首次响应。
    /// </summary>
    private readonly PricingIdempotentResponseReader _idempotentResponseReader;

    /// <summary>
    /// 限额占用仓储，用于抢占幂等锁和额度锁。
    /// </summary>
    private readonly ILimitOccupyRepository _limitRepository;

    /// <summary>
    /// 计价配置，主要使用 confirm 保护占用的过期时间。
    /// </summary>
    private readonly PricingOptions _options;

    /// <summary>
    /// 统一时钟，用于响应时间、过期时间和日志时间。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// confirm 工作流日志对象。
    /// </summary>
    private readonly ILogger<PricingConfirmWorkflow> _logger;

    /// <summary>
    /// 初始化确认计价工作流。
    /// </summary>
    /// <param name="calculationRunner">费用明细计价运行器。</param>
    /// <param name="requestLogRepository">请求日志仓储。</param>
    /// <param name="authorityPriceChecker">权威价格诊断器。</param>
    /// <param name="idempotencyService">幂等服务。</param>
    /// <param name="persistenceService">confirm 结果持久化服务。</param>
    /// <param name="runtimePackageTraceResolver">运行包追踪解析器。</param>
    /// <param name="transactionExecutor">计价事务执行器。</param>
    /// <param name="idempotentResponseReader">幂等响应读取器。</param>
    /// <param name="limitRepository">限额占用仓储。</param>
    /// <param name="options">计价配置。</param>
    /// <param name="clock">统一时钟。</param>
    /// <param name="logger">confirm 工作流日志对象。</param>
    public PricingConfirmWorkflow(
        PricingItemCalculationRunner calculationRunner,
        IChargeRequestLogRepository requestLogRepository,
        AuthorityPriceChecker authorityPriceChecker,
        PricingIdempotencyService idempotencyService,
        PricingConfirmationPersistenceService persistenceService,
        RuntimePackageTraceResolver runtimePackageTraceResolver,
        PricingTransactionExecutor transactionExecutor,
        PricingIdempotentResponseReader idempotentResponseReader,
        ILimitOccupyRepository limitRepository,
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger<PricingConfirmWorkflow> logger)
    {
        _calculationRunner = calculationRunner;
        _requestLogRepository = requestLogRepository;
        _authorityPriceChecker = authorityPriceChecker;
        _idempotencyService = idempotencyService;
        _persistenceService = persistenceService;
        _runtimePackageTraceResolver = runtimePackageTraceResolver;
        _transactionExecutor = transactionExecutor;
        _idempotentResponseReader = idempotentResponseReader;
        _limitRepository = limitRepository;
        _options = options.Value;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行确认计价。
    /// </summary>
    /// <param name="request">确认计价请求，必须包含稳定的业务请求号。</param>
    /// <returns>确认计价结果，包含 requestId、过期时间和每条明细的计价结果。</returns>
    /// <remarks>
    /// 该方法对应 <c>/api/pricing/calculate/confirm</c>。它会产生资金相关状态，调用方必须把返回的
    /// <c>RequestId</c> 保存下来，并在 HIS 落账成功后 commit，落账失败后 cancel。
    /// </remarks>
    public async Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        var items = await ValidateRequestAsync(request);
        var firstItem = items[0];
        _logger.LogInformation(
            "确认计价开始 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 患者ID={PatientId}, 项目编码={ItemCode}, 输入数量={InputQty}",
            request.SourceSystem, request.BusinessRequestNo, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        var idempotency = await _idempotencyService.CheckConfirmAsync(request, items, "CONFIRM");
        var fingerprint = idempotency.Fingerprint;
        var existingResponse = await TryReadExistingResponseAsync(request, fingerprint, firstItem, idempotency.ExistingRequest);
        if (existingResponse is not null)
        {
            return existingResponse;
        }

        return await _transactionExecutor.ExecuteAsync(async () =>
        {
            await LockIdempotencyAsync(request);

            var existingInTransaction = await _requestLogRepository.GetByBusinessKeyAsync(request.SourceSystem, request.BusinessRequestNo!, "CONFIRM");
            var transactionResponse = await TryReadExistingResponseAsync(request, fingerprint, firstItem, existingInTransaction);
            if (transactionResponse is not null)
            {
                return transactionResponse;
            }

            var runtimePackageContext = await _runtimePackageTraceResolver.CaptureContextAsync();
            using var runtimePackageScope = _runtimePackageTraceResolver.BeginScope(runtimePackageContext);

            var calculations = await _calculationRunner.RunAsync(request, items, "CONFIRM", shouldLockLimits: true);
            var runtimeTrace = await _runtimePackageTraceResolver.ResolveAsync(calculations);
            var response = await _persistenceService.PersistAsync(new PricingConfirmationPersistenceInput
            {
                Request = request,
                Items = items,
                Calculations = calculations,
                Fingerprint = fingerprint,
                RuntimeTrace = runtimeTrace
            });

            _logger.LogInformation(
                "确认计价成功 请求ID={RequestId}, 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 项目编码={ItemCode}, 最终数量={FinalQty}, 最终金额={FinalAmount}, 是否特殊项目={IsSpecialItem}",
                response.RequestId, request.SourceSystem, request.BusinessRequestNo,
                firstItem.ItemCode, response.FinalQty, response.FinalAmount, response.IsSpecialItem);

            return response;
        });
    }

    private async Task<IReadOnlyList<PricingCalculateItemRequest>> ValidateRequestAsync(
        PricingCalculateRequest request)
    {
        // confirm 会产生资金影响，缺少业务请求号仍需拦截；否则 HIS 超时重试时无法判断是否同一次收费动作。
        // 单价差异当前只记录日志，不阻断流程；如果未来恢复强校验，应在 AuthorityPriceChecker 和测试中同步调整。
        var items = PricingRequestGuard.GetRequiredItems(request);
        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            _logger.LogWarning(
                "确认计价校验失败：业务请求号为空，来源系统={SourceSystem}",
                request.SourceSystem);
            PricingRequestGuard.EnsureConfirmRequest(request);
        }

        await _authorityPriceChecker.CheckAsync(request, items);
        return items;
    }

    private async Task<PricingCalculateResponse?> TryReadExistingResponseAsync(
        PricingCalculateRequest request,
        string fingerprint,
        PricingCalculateItemRequest firstItem,
        ChargeRequest? existingRequest)
    {
        if (existingRequest is null)
        {
            return null;
        }

        _idempotencyService.EnsureSameFingerprint(existingRequest, fingerprint, request.BusinessRequestNo!);
        _logger.LogInformation(
            "确认计价幂等命中 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 请求ID={RequestId}, 项目编码={ItemCode}, 原状态={Status}",
            request.SourceSystem, request.BusinessRequestNo, existingRequest.RequestId, firstItem.ItemCode, existingRequest.BusinessStatus);
        return await _idempotentResponseReader.ReadAsync(existingRequest);
    }

    private Task LockIdempotencyAsync(PricingCalculateRequest request)
    {
        // 并发请求可能同时通过事务外查询，因此必须在同一个业务键锁内再次读取请求日志。
        // 这一步保护的是“同一业务号只生成一份 confirm 结果”，与后续额度维度锁共同防止重复占额。
        return _limitRepository.EnsureAndLockAsync(new[]
        {
            PricingLockKeyBuilder.BuildIdempotencyLockKey(request.SourceSystem, request.BusinessRequestNo!, "CONFIRM")
        });
    }
}
