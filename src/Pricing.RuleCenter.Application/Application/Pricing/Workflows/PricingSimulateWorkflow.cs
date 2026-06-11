using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 试算计价工作流：不占用额度的模拟计价并写入可追溯日志。
/// </summary>
/// <remarks>
/// 试算不写正式折价明细和限额占用，可重复调用。仍然写请求日志和步骤日志用于追溯。
/// 单条和批量试算都进入本 workflow，差异在计价运行器是否共享 RequestSharedPricingState。
/// </remarks>
public sealed class PricingSimulateWorkflow
{
    private readonly PricingItemCalculationRunner _calculationRunner;
    private readonly AuthorityPriceChecker _authorityPriceChecker;
    private readonly PricingSimulationPersistenceService _persistenceService;
    private readonly RuntimePackageTraceResolver _runtimePackageTraceResolver;
    private readonly IClock _clock;
    private readonly ILogger<PricingSimulateWorkflow> _logger;

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
    /// 执行试算计价：校验 → 运行包捕获 → 计价 → 持久化。
    /// </summary>
    public async Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        var items = await ValidateRequestAsync(request);
        var firstItem = items[0];
        _logger.LogInformation(
            "试算开始 来源系统={SourceSystem}, 患者ID={PatientId}, 项目编码={ItemCode}, 输入数量={InputQty}",
            request.SourceSystem, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        // 锁定当前激活的运行包版本，保证本请求内所有明细使用同一版本规则。
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
        var items = PricingRequestGuard.GetRequiredItems(request);
        // 试算保持 async 以保留 AuthorityPriceChecker 内部的异步 DB 查询能力。
        await _authorityPriceChecker.CheckAsync(request, items);
        return items;
    }
}
