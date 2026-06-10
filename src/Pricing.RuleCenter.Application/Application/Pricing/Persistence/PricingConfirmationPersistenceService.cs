using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

/// <summary>
/// 确认计价结果持久化服务。
/// </summary>
/// <remarks>
/// confirm 的持久化需要同时写请求日志、步骤日志、折价明细、限额占用和响应快照。
/// 该服务专门负责这段事务内落库流程，让 workflow 只保留幂等、锁和计价编排。
/// </remarks>
internal sealed record PricingConfirmationPersistenceInput
{
    public PricingCalculateRequest Request { get; init; } = null!;

    public IReadOnlyList<PricingCalculateItemRequest> Items { get; init; } =
        Array.Empty<PricingCalculateItemRequest>();

    public IReadOnlyList<ItemPricingCalculation> Calculations { get; init; } =
        Array.Empty<ItemPricingCalculation>();

    public string Fingerprint { get; init; } = string.Empty;

    public RuntimePackageTraceResolution? RuntimeTrace { get; init; }
}

/// <summary>
/// confirm 持久化编排服务。
/// </summary>
public sealed class PricingConfirmationPersistenceService
{
    private readonly PricingRequestLogWriter _requestLogWriter;
    private readonly PricingTraceStepWriter _traceStepWriter;
    private readonly PricingDiscountDetailWriter _discountDetailWriter;
    private readonly PricingLimitOccupyWriter _limitOccupyWriter;
    private readonly PricingOptions _options;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="PricingConfirmationPersistenceService"/> class.
    /// </summary>
    /// <param name="requestLogWriter">A request log writer.</param>
    /// <param name="traceStepWriter">A trace step writer.</param>
    /// <param name="discountDetailWriter">A discount detail writer.</param>
    /// <param name="limitOccupyWriter">A limit occupy writer.</param>
    /// <param name="options">A pricing options accessor.</param>
    /// <param name="clock">A system clock.</param>
    public PricingConfirmationPersistenceService(
        PricingRequestLogWriter requestLogWriter,
        PricingTraceStepWriter traceStepWriter,
        PricingDiscountDetailWriter discountDetailWriter,
        PricingLimitOccupyWriter limitOccupyWriter,
        IOptions<PricingOptions> options,
        IClock clock)
    {
        _requestLogWriter = requestLogWriter;
        _traceStepWriter = traceStepWriter;
        _discountDetailWriter = discountDetailWriter;
        _limitOccupyWriter = limitOccupyWriter;
        _options = options.Value;
        _clock = clock;
    }

    internal async Task<PricingCalculateResponse> PersistAsync(PricingConfirmationPersistenceInput input)
    {
        var requestLog = await SaveRequestLogAsync(input);
        await _traceStepWriter.SaveAsync(
            requestLog.RequestId,
            requestLog.TraceId,
            input.Calculations,
            input.RuntimeTrace);

        await SaveCalculationArtifactsAsync(requestLog, input);

        var response = BuildResponse(requestLog, input);
        await _requestLogWriter.SaveResponseJsonAsync(requestLog, response);
        return response;
    }

    private Task<ChargeRequest> SaveRequestLogAsync(PricingConfirmationPersistenceInput input)
    {
        return _requestLogWriter.SaveAsync(new RequestLogSaveInput
        {
            Request = input.Request,
            Items = input.Items,
            Calculations = input.Calculations,
            CallType = PricingCallTypeCodes.Confirm,
            BusinessStatus = BusinessStatusCodes.ConfirmPending,
            Fingerprint = input.Fingerprint,
            RuntimeTrace = input.RuntimeTrace
        });
    }

    private async Task SaveCalculationArtifactsAsync(
        ChargeRequest requestLog,
        PricingConfirmationPersistenceInput input)
    {
        foreach (var calculation in input.Calculations)
        {
            await SaveCalculationArtifactsAsync(requestLog, calculation, input);
        }
    }

    private async Task SaveCalculationArtifactsAsync(
        ChargeRequest requestLog,
        ItemPricingCalculation calculation,
        PricingConfirmationPersistenceInput input)
    {
        await _discountDetailWriter.SaveAsync(new DiscountDetailSaveInput
        {
            RequestId = requestLog.RequestId,
            TraceId = requestLog.TraceId,
            Request = input.Request,
            Item = calculation.Item,
            Result = calculation.Result,
            Status = OccupyStatusCodes.Pending,
            RuntimeTrace = input.RuntimeTrace
        });

        if (!calculation.Result.IsSpecialItem)
        {
            return;
        }

        await _limitOccupyWriter.SaveAsync(
            requestLog.RequestId,
            requestLog.TraceId,
            calculation.Item,
            calculation.Result);
    }

    private PricingCalculateResponse BuildResponse(
        ChargeRequest requestLog,
        PricingConfirmationPersistenceInput input)
    {
        return PricingResponseBuilder.Build(
            requestLog.RequestId,
            requestLog.TraceId,
            input.Calculations,
            _clock.Now,
            input.RuntimeTrace,
            requestLog.RequestAt.AddMinutes(_options.ConfirmExpireMinutes));
    }
}
