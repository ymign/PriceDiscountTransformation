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
/// 试算计价 workflow。
/// </summary>
public sealed class PricingSimulateWorkflow
{
    private readonly IPricingEngine _engine;
    private readonly AuthorityPriceChecker _authorityPriceChecker;
    private readonly PricingRequestLogWriter _requestLogWriter;
    private readonly PricingTraceStepWriter _traceStepWriter;
    private readonly RuntimePackageTraceResolver _runtimePackageTraceResolver;
    private readonly IClock _clock;
    private readonly ILogger _logger;

    /// <summary>
    /// 初始化试算计价 workflow。
    /// </summary>
    public PricingSimulateWorkflow(
        IPricingEngine engine,
        AuthorityPriceChecker authorityPriceChecker,
        PricingRequestLogWriter requestLogWriter,
        PricingTraceStepWriter traceStepWriter,
        RuntimePackageTraceResolver runtimePackageTraceResolver,
        IClock clock,
        ILogger logger)
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
    public async Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        var items = PricingRequestGuard.GetRequiredItems(request);

        var firstItem = items[0];
        _logger.LogInformation(
            "SIMULATE 开始 SourceSystem={SourceSystem}, PatientId={PatientId}, ItemCode={ItemCode}, InputQty={InputQty}",
            request.SourceSystem, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        await _authorityPriceChecker.CheckAsync(items);

        var runtimePackageContext = await _runtimePackageTraceResolver.CaptureContextAsync();
        using var runtimePackageScope = _runtimePackageTraceResolver.BeginScope(runtimePackageContext);

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
