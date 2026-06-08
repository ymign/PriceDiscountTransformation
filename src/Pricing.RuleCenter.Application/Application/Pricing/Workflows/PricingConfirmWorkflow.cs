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
/// 确认计价 workflow。
/// </summary>
public sealed class PricingConfirmWorkflow
{
    private readonly IPricingEngine _engine;
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly AuthorityPriceChecker _authorityPriceChecker;
    private readonly PricingIdempotencyService _idempotencyService;
    private readonly PricingRequestLogWriter _requestLogWriter;
    private readonly PricingTraceStepWriter _traceStepWriter;
    private readonly PricingDiscountDetailWriter _discountDetailWriter;
    private readonly PricingLimitOccupyWriter _limitOccupyWriter;
    private readonly RuntimePackageTraceResolver _runtimePackageTraceResolver;
    private readonly PricingTransactionExecutor _transactionExecutor;
    private readonly PricingIdempotentResponseReader _idempotentResponseReader;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly PricingOptions _options;
    private readonly IClock _clock;
    private readonly ILogger<PricingConfirmWorkflow> _logger;

    /// <summary>
    /// 初始化确认计价 workflow。
    /// </summary>
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
    public async Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        var items = PricingRequestGuard.GetRequiredItems(request);
        var firstItem = items[0];
        _logger.LogInformation(
            "CONFIRM 开始 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, PatientId={PatientId}, ItemCode={ItemCode}, InputQty={InputQty}",
            request.SourceSystem, request.BusinessRequestNo, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            _logger.LogWarning(
                "CONFIRM 校验失败: BusinessRequestNo 为空, SourceSystem={SourceSystem}",
                request.SourceSystem);
            PricingRequestGuard.EnsureConfirmRequest(request);
        }

        await _authorityPriceChecker.CheckAsync(items);

        var idempotency = await _idempotencyService.CheckConfirmAsync(request, items, "CONFIRM");
        var fingerprint = idempotency.Fingerprint;
        if (idempotency.ExistingRequest is { } existing)
        {
            _idempotencyService.EnsureSameFingerprint(existing, fingerprint, request.BusinessRequestNo!);
            _logger.LogInformation(
                "幂等命中 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, RequestId={RequestId}, ItemCode={ItemCode}, OriginalStatus={Status}",
                request.SourceSystem, request.BusinessRequestNo, existing.RequestId, existing.ItemCode, existing.BusinessStatus);
            return await _idempotentResponseReader.ReadAsync(existing);
        }

        return await _transactionExecutor.ExecuteAsync(async () =>
        {
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
                    "CONFIRM 事务内幂等命中 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, RequestId={RequestId}",
                    request.SourceSystem, request.BusinessRequestNo, existingInTransaction.RequestId);
                return await _idempotentResponseReader.ReadAsync(existingInTransaction);
            }

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
                    CallType = "CONFIRM",
                    ShouldLockLimits = true,
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

            var response = PricingResponseBuilder.Build(
                requestLog.RequestId,
                requestLog.TraceId,
                calculations,
                _clock.Now,
                runtimeTrace,
                requestLog.RequestAt.AddMinutes(_options.ConfirmExpireMinutes));
            await _requestLogWriter.SaveResponseJsonAsync(requestLog, response);

            _logger.LogInformation(
                "CONFIRM 成功 RequestId={RequestId}, SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, ItemCode={ItemCode}, FinalQty={FinalQty}, FinalAmount={FinalAmount}, IsSpecialItem={IsSpecialItem}",
                requestLog.RequestId, request.SourceSystem, request.BusinessRequestNo,
                firstItem.ItemCode, response.FinalQty, response.FinalAmount, response.IsSpecialItem);

            return response;
        });
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
