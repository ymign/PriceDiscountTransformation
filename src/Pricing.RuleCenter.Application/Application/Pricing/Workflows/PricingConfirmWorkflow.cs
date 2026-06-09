using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Models;
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
    /// <summary>
    /// 计价核心引擎，负责执行规则匹配和动作链。
    /// </summary>
    private readonly IPricingEngine _engine;

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
    /// 请求日志写入器，负责保存 CONFIRM_PENDING 请求和响应快照。
    /// </summary>
    private readonly PricingRequestLogWriter _requestLogWriter;

    /// <summary>
    /// 计算步骤写入器，负责保存本次 confirm 的规则执行过程。
    /// </summary>
    private readonly PricingTraceStepWriter _traceStepWriter;

    /// <summary>
    /// 折价明细写入器，负责记录每条费用的最终数量、金额和特殊计价结果。
    /// </summary>
    private readonly PricingDiscountDetailWriter _discountDetailWriter;

    /// <summary>
    /// 限额占用写入器，只对命中特殊项目的明细写入待提交占用。
    /// </summary>
    private readonly PricingLimitOccupyWriter _limitOccupyWriter;

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
    /// <param name="engine">计价核心引擎。</param>
    /// <param name="requestLogRepository">请求日志仓储。</param>
    /// <param name="authorityPriceChecker">权威价格诊断器。</param>
    /// <param name="idempotencyService">幂等服务。</param>
    /// <param name="requestLogWriter">请求日志写入器。</param>
    /// <param name="traceStepWriter">计算步骤写入器。</param>
    /// <param name="discountDetailWriter">折价明细写入器。</param>
    /// <param name="limitOccupyWriter">限额占用写入器。</param>
    /// <param name="runtimePackageTraceResolver">运行包追踪解析器。</param>
    /// <param name="transactionExecutor">计价事务执行器。</param>
    /// <param name="idempotentResponseReader">幂等响应读取器。</param>
    /// <param name="limitRepository">限额占用仓储。</param>
    /// <param name="options">计价配置。</param>
    /// <param name="clock">统一时钟。</param>
    /// <param name="logger">confirm 工作流日志对象。</param>
    public PricingConfirmWorkflow(
        IPricingEngine engine,
        IChargeRequestLogRepository requestLogRepository,
        AuthorityPriceChecker authorityPriceChecker,
        PricingIdempotencyService idempotencyService,
        PricingRequestLogWriter requestLogWriter,
        PricingTraceStepWriter traceStepWriter,
        PricingDiscountDetailWriter discountDetailWriter,
        PricingLimitOccupyWriter limitOccupyWriter,
        RuntimePackageTraceResolver runtimePackageTraceResolver,
        PricingTransactionExecutor transactionExecutor,
        PricingIdempotentResponseReader idempotentResponseReader,
        ILimitOccupyRepository limitRepository,
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger<PricingConfirmWorkflow> logger)
    {
        _engine = engine;
        _requestLogRepository = requestLogRepository;
        _authorityPriceChecker = authorityPriceChecker;
        _idempotencyService = idempotencyService;
        _requestLogWriter = requestLogWriter;
        _traceStepWriter = traceStepWriter;
        _discountDetailWriter = discountDetailWriter;
        _limitOccupyWriter = limitOccupyWriter;
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
        // ========== 第一阶段：请求校验和权威单价诊断 ==========
        // confirm 会产生资金影响，缺少业务请求号仍需拦截；否则 HIS 超时重试时无法判断是否同一次收费动作。
        // 单价差异当前只记录日志，不阻断流程；如果未来恢复强校验，应在 AuthorityPriceChecker 和测试中同步调整。
        var items = PricingRequestGuard.GetRequiredItems(request);
        var firstItem = items[0];
        _logger.LogInformation(
            "确认计价开始 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 患者ID={PatientId}, 项目编码={ItemCode}, 输入数量={InputQty}",
            request.SourceSystem, request.BusinessRequestNo, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            _logger.LogWarning(
                "确认计价校验失败：业务请求号为空，来源系统={SourceSystem}",
                request.SourceSystem);
            PricingRequestGuard.EnsureConfirmRequest(request);
        }

        await _authorityPriceChecker.CheckAsync(request, items);

        // ========== 第二阶段：事务外幂等快路径 ==========
        // 已存在的 confirm 可以直接复用首次响应，减少重复重试对数据库锁的压力。
        // 这里仍会比较请求指纹，防止 HIS 复用同一个 businessRequestNo 但改了部位、数量、扩展参数或多 part 明细。
        var idempotency = await _idempotencyService.CheckConfirmAsync(request, items, "CONFIRM");
        var fingerprint = idempotency.Fingerprint;
        if (idempotency.ExistingRequest is { } existing)
        {
            _idempotencyService.EnsureSameFingerprint(existing, fingerprint, request.BusinessRequestNo!);
            _logger.LogInformation(
                "确认计价幂等命中 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 请求ID={RequestId}, 项目编码={ItemCode}, 原状态={Status}",
                request.SourceSystem, request.BusinessRequestNo, existing.RequestId, existing.ItemCode, existing.BusinessStatus);
            return await _idempotentResponseReader.ReadAsync(existing);
        }

        return await _transactionExecutor.ExecuteAsync(async () =>
        {
            // ========== 第三阶段：抢占幂等锁并做事务内二次幂等检查 ==========
            // 并发请求可能同时通过事务外查询，因此必须在同一个业务键锁内再次读取请求日志。
            // 这一步保护的是“同一业务号只生成一份 confirm 结果”，与后续额度维度锁共同防止重复占额。
            await _limitRepository.EnsureAndLockAsync(new[]
            {
                PricingLockKeyBuilder.BuildIdempotencyLockKey(request.SourceSystem, request.BusinessRequestNo!, "CONFIRM")
            });

            var existingInTransaction = await _requestLogRepository.GetByBusinessKeyAsync(
                request.SourceSystem,
                request.BusinessRequestNo!,
                "CONFIRM");
            if (existingInTransaction is not null)
            {
                _idempotencyService.EnsureSameFingerprint(
                    existingInTransaction,
                    fingerprint,
                    request.BusinessRequestNo!);

                _logger.LogInformation(
                    "确认计价事务内幂等命中 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 请求ID={RequestId}",
                    request.SourceSystem, request.BusinessRequestNo, existingInTransaction.RequestId);
                return await _idempotentResponseReader.ReadAsync(existingInTransaction);
            }

            // ========== 第四阶段：固定运行包上下文 ==========
            // 同一 confirm 请求内所有明细必须使用同一个运行包，避免发布瞬间出现一单多版本。
            // 运行包信息会写入追溯上下文，后续退费、对账和问题复盘都能定位当时实际使用的规则版本。
            var runtimePackageContext = await _runtimePackageTraceResolver.CaptureContextAsync();
            using var runtimePackageScope = _runtimePackageTraceResolver.BeginScope(runtimePackageContext);

            // ========== 第五阶段：逐条明细计价并累计本请求内占用 ==========
            // ShouldLockLimits=true 表示规则执行器可以对数据库限额维度加锁，防止多渠道并发突破。
            // 内存中的 inRequestLimitOccupies 解决“一次 confirm 多条明细”内部互相影响的问题；
            // 数据库锁解决“不同渠道或不同请求并发 confirm”之间互相影响的问题。
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

            // ========== 第六阶段：写请求、步骤、明细和限额占用 ==========
            // 所有持久化操作必须和 confirm 事务绑定，任一写入失败都回滚，避免“有占额无请求”或“有请求无明细”。
            // 折价明细保存本次 confirm 的全部可落账结果；限额占用只针对命中特殊规则且需要保护额度的结果。
            var runtimeTrace = await _runtimePackageTraceResolver.ResolveAsync(calculations);
            var requestLog = await _requestLogWriter.SaveAsync(new RequestLogSaveInput
            {
                Request = request,
                Items = items,
                Calculations = calculations,
                CallType = "CONFIRM",
                BusinessStatus = BusinessStatusCodes.ConfirmPending,
                Fingerprint = fingerprint,
                RuntimeTrace = runtimeTrace
            });
            await _traceStepWriter.SaveAsync(requestLog.RequestId, requestLog.TraceId, calculations, runtimeTrace);

            foreach (var calculation in calculations)
            {
                await _discountDetailWriter.SaveAsync(new DiscountDetailSaveInput
                {
                    RequestId = requestLog.RequestId,
                    TraceId = requestLog.TraceId,
                    Request = request,
                    Item = calculation.Item,
                    Result = calculation.Result,
                    Status = OccupyStatusCodes.Pending,
                    RuntimeTrace = runtimeTrace
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

            // ========== 第七阶段：保存响应快照 ==========
            // 响应 JSON 是后续幂等重试的事实来源，必须在事务内和请求日志一起落库。
            // 后续重复 confirm 不重新计算，直接读取该快照，避免规则发布、历史占用变化导致同一业务号返回不同金额。
            var response = PricingResponseBuilder.Build(
                requestLog.RequestId,
                requestLog.TraceId,
                calculations,
                _clock.Now,
                runtimeTrace,
                requestLog.RequestAt.AddMinutes(_options.ConfirmExpireMinutes));
            await _requestLogWriter.SaveResponseJsonAsync(requestLog, response);

            _logger.LogInformation(
                "确认计价成功 请求ID={RequestId}, 来源系统={SourceSystem}, 业务请求号={BusinessRequestNo}, 项目编码={ItemCode}, 最终数量={FinalQty}, 最终金额={FinalAmount}, 是否特殊项目={IsSpecialItem}",
                requestLog.RequestId, request.SourceSystem, request.BusinessRequestNo,
                firstItem.ItemCode, response.FinalQty, response.FinalAmount, response.IsSpecialItem);

            return response;
        });
    }

    /// <summary>
    /// 将当前明细产生的正式占用候选累加到本次 confirm 的请求内上下文。
    /// </summary>
    /// <param name="inRequestOccupiedQtyByLimitDimension">
    /// 按限额类型和限额维度汇总的本请求内占用数量，用于后续费用明细判断同批累计。
    /// </param>
    /// <param name="inRequestLimitOccupies">
    /// 本请求内已经产生的占用候选明细，供后续执行器读取完整占用信息。
    /// </param>
    /// <param name="result">当前费用明细的计价结果。</param>
    /// <remarks>
    /// confirm 的请求内累计和 simulate 的请求内累计语义相同，但 confirm 还会在持久化阶段写入正式
    /// <c>PR_LIMIT_OCCUPY</c>。这里先在内存中累计，是为了让同一请求后续明细在正式落库前就能看到前置明细影响。
    /// </remarks>
    private static void AccumulateInRequestLimits(
        Dictionary<string, decimal> inRequestOccupiedQtyByLimitDimension,
        List<LimitOccupy> inRequestLimitOccupies,
        PricingResult result)
    {
        // 批量 confirm 中同一请求的前置明细占用要影响后续明细，避免一单内多条费用绕过窗口或同组限制。
        foreach (var occupy in result.LimitOccupies.Where(o =>
                     !string.IsNullOrWhiteSpace(o.LimitType) &&
                     !string.IsNullOrWhiteSpace(o.LimitDimensionCode)))
        {
            var key = $"{occupy.LimitType.Trim().ToUpperInvariant()}:{occupy.LimitDimensionCode?.Trim().ToUpperInvariant()}";
            inRequestOccupiedQtyByLimitDimension.TryGetValue(key, out var existingQty);
            inRequestOccupiedQtyByLimitDimension[key] = existingQty + occupy.OccupyQty;
            inRequestLimitOccupies.Add(occupy);
        }
    }
}
