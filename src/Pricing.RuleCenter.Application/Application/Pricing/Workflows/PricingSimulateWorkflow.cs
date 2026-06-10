using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Interfaces;

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
/// <para>
/// 单条试算和批量试算都进入本 workflow。差异不在 HTTP 控制器，而在计价运行器是否让多条费用
/// 共享同一个 <see cref="RequestSharedPricingState"/>：这样后续明细才能看到前序明细已经形成的
/// 同组互斥、同手术封顶、同项目多行累计以及请求内虚拟占用。
/// </para>
/// <para>
/// 本 workflow 的边界：可以落请求日志和步骤日志；不写 <c>PR_CHARGE_DISCOUNT_DETAIL</c> 的正式结果，
/// 不写 <c>PR_LIMIT_OCCUPY</c> 的正式占用，也不参与 commit/cancel/reverse 状态机。
/// </para>
/// </remarks>
public sealed class PricingSimulateWorkflow
{
    private readonly PricingItemCalculationRunner _calculationRunner;

    /// <summary>
    /// 权威价格诊断器，用于在开关允许时记录明细单价与 HIS 物价主数据的差异。
    /// </summary>
    private readonly AuthorityPriceChecker _authorityPriceChecker;

    /// <summary>
    /// 试算结果持久化服务，负责写请求日志、步骤日志和响应快照。
    /// </summary>
    private readonly PricingSimulationPersistenceService _persistenceService;

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
        PricingItemCalculationRunner calculationRunner,
        AuthorityPriceChecker authorityPriceChecker,
        PricingSimulationPersistenceService persistenceService,
        RuntimePackageTraceResolver runtimePackageTraceResolver,
        IClock clock,
        ILogger<PricingSimulateWorkflow> logger)
    {
        _calculationRunner = calculationRunner;
        _authorityPriceChecker = authorityPriceChecker;
        _persistenceService = persistenceService;
        _runtimePackageTraceResolver = runtimePackageTraceResolver;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行试算计价。
    /// </summary>
    /// <param name="request">试算请求，支持单条或多条费用明细。</param>
    /// <returns>试算结果，包含金额、数量、命中规则和追踪步骤。</returns>
    /// <remarks>
    /// 该方法用于 <c>/api/pricing/calculate/simulate</c> 和
    /// <c>/api/pricing/calculate/batch-simulate</c> 两个入口。它不会校验 confirm 幂等键，
    /// 因为试算允许用户在收费界面反复调整数量、部位和扩展参数。
    /// </remarks>
    public async Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        var items = await ValidateRequestAsync(request);
        var firstItem = items[0];
        _logger.LogInformation(
            "试算开始 来源系统={SourceSystem}, 患者ID={PatientId}, 项目编码={ItemCode}, 输入数量={InputQty}",
            request.SourceSystem, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        var runtimePackageContext = await _runtimePackageTraceResolver.CaptureContextAsync();
        using var runtimePackageScope = _runtimePackageTraceResolver.BeginScope(runtimePackageContext);

        var calculations = await _calculationRunner.RunAsync(request, items, "SIMULATE", shouldLockLimits: false);
        var runtimeTrace = await _runtimePackageTraceResolver.ResolveAsync(calculations);

        return await _persistenceService.PersistAsync(new PricingSimulationPersistenceInput
        {
            Request = request,
            Items = items,
            Calculations = calculations,
            RuntimeTrace = runtimeTrace
        });
    }

    private async Task<IReadOnlyList<PricingCalculateItemRequest>> ValidateRequestAsync(
        PricingCalculateRequest request)
    {
        // 试算不占额，但仍必须校验最基本的请求结构，否则后续追溯日志会缺少患者、项目或数量等关键维度。
        // 当前权威单价只做诊断日志：目的是在联调期发现渠道传价差异，但不让展示型试算直接阻断收费入口。
        var items = PricingRequestGuard.GetRequiredItems(request);
        await _authorityPriceChecker.CheckAsync(request, items);
        return items;
    }
}
