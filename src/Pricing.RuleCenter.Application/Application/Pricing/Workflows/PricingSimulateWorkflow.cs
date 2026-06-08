using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 试算计价工作流，负责执行不占用额度的模拟计价并写入可追溯日志。
/// </summary>
/// <remarks>
/// <para>
/// 试算用于收费录入界面预览折价效果。它会执行完整规则匹配和金额计算，但不会写入正式折价明细，
/// 也不会持久化限额占用；因此试算可以重复调用，不影响后续 confirm 的额度判断。
/// </para>
/// <para>
/// 仍然写请求日志和步骤日志，是为了让前端、运维和收费人员能够解释“当时为什么算出这个价格”。
/// </para>
/// </remarks>
public sealed class PricingSimulateWorkflow
{
    /// <summary>
    /// 计价核心引擎，负责规则匹配、动作执行、金额计算和步骤生成。
    /// </summary>
    private readonly IPricingEngine _engine;

    /// <summary>
    /// 权威价格诊断器，用于在开关允许时记录明细单价与 HIS 物价主数据的差异。
    /// </summary>
    private readonly AuthorityPriceChecker _authorityPriceChecker;

    /// <summary>
    /// 请求日志写入器，负责保存试算请求和最终响应 JSON。
    /// </summary>
    private readonly PricingRequestLogWriter _requestLogWriter;

    /// <summary>
    /// 计算步骤写入器，负责保存每条规则匹配和动作执行过程。
    /// </summary>
    private readonly PricingTraceStepWriter _traceStepWriter;

    /// <summary>
    /// 运行包追踪解析器，用于把本次试算关联到当前激活运行包和运行时规则。
    /// </summary>
    private readonly RuntimePackageTraceResolver _runtimePackageTraceResolver;

    /// <summary>
    /// 统一时钟，保证响应时间和日志时间来源一致。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// 试算工作流日志对象。
    /// </summary>
    private readonly ILogger<PricingSimulateWorkflow> _logger;

    /// <summary>
    /// 初始化试算计价工作流。
    /// </summary>
    /// <param name="engine">计价核心引擎。</param>
    /// <param name="authorityPriceChecker">权威价格诊断器。</param>
    /// <param name="requestLogWriter">请求日志写入器。</param>
    /// <param name="traceStepWriter">计算步骤写入器。</param>
    /// <param name="runtimePackageTraceResolver">运行包追踪解析器。</param>
    /// <param name="clock">统一时钟。</param>
    /// <param name="logger">试算工作流日志对象。</param>
    public PricingSimulateWorkflow(
        IPricingEngine engine,
        AuthorityPriceChecker authorityPriceChecker,
        PricingRequestLogWriter requestLogWriter,
        PricingTraceStepWriter traceStepWriter,
        RuntimePackageTraceResolver runtimePackageTraceResolver,
        IClock clock,
        ILogger<PricingSimulateWorkflow> logger)
    {
        _engine = engine;
        _authorityPriceChecker = authorityPriceChecker;
        _requestLogWriter = requestLogWriter;
        _traceStepWriter = traceStepWriter;
        _runtimePackageTraceResolver = runtimePackageTraceResolver;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行试算计价。
    /// </summary>
    /// <param name="request">试算请求，支持单条或多条费用明细。</param>
    /// <returns>试算结果，包含金额、数量、命中规则和追踪步骤。</returns>
    public async Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        // ========== 第一阶段：基础校验和价格诊断 ==========
        // 试算不占额；请求结构错误仍拦截，单价差异只记录日志供联调和对账排查。
        var items = PricingRequestGuard.GetRequiredItems(request);

        var firstItem = items[0];
        _logger.LogInformation(
            "试算开始 来源系统={SourceSystem}, 患者ID={PatientId}, 项目编码={ItemCode}, 输入数量={InputQty}",
            request.SourceSystem, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        await _authorityPriceChecker.CheckAsync(request, items);

        // ========== 第二阶段：捕获运行包上下文 ==========
        // 同一次请求内的多条费用必须使用同一个激活运行包，避免规则发布瞬间造成同单明细版本不一致。
        var runtimePackageContext = await _runtimePackageTraceResolver.CaptureContextAsync();
        using var runtimePackageScope = _runtimePackageTraceResolver.BeginScope(runtimePackageContext);

        // ========== 第三阶段：逐条明细计价 ==========
        // 试算不锁数据库额度，但仍维护“本请求内已占数量”，保证批量明细之间的同组互斥/窗口限制口径一致。
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

        // ========== 第四阶段：保存追踪日志并构建响应 ==========
        // 试算日志状态使用 SIMULATED，后续不会进入 commit/cancel 状态流转。
        var runtimeTrace = await _runtimePackageTraceResolver.ResolveAsync(calculations);
        var requestLog = await _requestLogWriter.SaveAsync(new RequestLogSaveInput
        {
            Request = request,
            Items = items,
            Calculations = calculations,
            CallType = "SIMULATE",
            BusinessStatus = BusinessStatusCodes.Simulated,
            RuntimeTrace = runtimeTrace
        });
        await _traceStepWriter.SaveAsync(requestLog.RequestId, requestLog.TraceId, calculations, runtimeTrace);

        var response = PricingResponseBuilder.Build(
            requestLog.RequestId,
            requestLog.TraceId,
            calculations,
            _clock.Now,
            runtimeTrace);
        await _requestLogWriter.SaveResponseJsonAsync(requestLog, response);
        return response;
    }

    private static void AccumulateInRequestLimits(
        Dictionary<string, decimal> inRequestOccupiedQtyByLimitDimension,
        List<LimitOccupy> inRequestLimitOccupies,
        PricingResult result)
    {
        // 批量试算时，前一条明细的虚拟占用要参与后一条明细的规则判断。
        // 这里不写数据库，只在当前请求内累计，防止试算污染正式额度。
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
