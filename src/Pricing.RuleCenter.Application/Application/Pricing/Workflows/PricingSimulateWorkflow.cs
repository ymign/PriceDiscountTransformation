using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 试算计价工作流：不占用额度的模拟计价并写入可追溯日志。
/// </summary>
public sealed class PricingSimulateWorkflow
{
    private readonly IPricingEngine _engine;
    private readonly AuthorityPriceChecker _authorityPriceChecker;
    private readonly PricingSimulationPersistenceService _persistenceService;
    private readonly RuntimePackageTraceResolver _runtimePackageTraceResolver;
    private readonly IClock _clock;
    private readonly ILogger<PricingSimulateWorkflow> _logger;

    public PricingSimulateWorkflow(
        IPricingEngine engine,
        AuthorityPriceChecker authorityPriceChecker,
        PricingSimulationPersistenceService persistenceService,
        RuntimePackageTraceResolver runtimePackageTraceResolver,
        IClock clock,
        ILogger<PricingSimulateWorkflow> logger)
    {
        _engine = engine;
        _authorityPriceChecker = authorityPriceChecker;
        _persistenceService = persistenceService;
        _runtimePackageTraceResolver = runtimePackageTraceResolver;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行试算计价：校验 → 运行包捕获 → 逐条计价 → 持久化。
    /// </summary>
    public async Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        var items = await ValidateRequestAsync(request);
        var firstItem = items[0];
        _logger.LogInformation(
            "试算开始 来源系统={SourceSystem}, 患者ID={PatientId}, 项目编码={ItemCode}, 输入数量={InputQty}",
            request.SourceSystem, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        var runtimePackageContext = await _runtimePackageTraceResolver.CaptureContextAsync();
        using var runtimePackageScope = _runtimePackageTraceResolver.BeginScope(runtimePackageContext);

        // 逐条明细计价，共享请求内累计状态（同组互斥、同手术封顶等）。
        var sharedState = new RequestSharedPricingState();
        var calculations = new List<ItemPricingCalculation>(items.Count);
        foreach (var item in items)
        {
            var context = PricingContextFactory.Create(new PricingContextBuildInput
            {
                Request = request,
                Item = item,
                CallType = "SIMULATE",
                ShouldLockLimits = false,
                RequestSharedState = sharedState
            });
            var result = await _engine.CalculateAsync(context);
            sharedState.Accumulate(result, context);
            calculations.Add(new ItemPricingCalculation(item, result));
        }

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
        var items = PricingRequestGuard.GetRequiredItems(request);
        await _authorityPriceChecker.CheckAsync(request, items);
        return items;
    }
}
