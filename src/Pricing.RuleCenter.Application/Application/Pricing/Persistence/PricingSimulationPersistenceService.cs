using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

/// <summary>
/// 试算结果持久化服务。
/// </summary>
/// <remarks>
/// 该服务把试算 workflow 中“写请求日志、写步骤、构建响应、保存响应快照”的持久化流程收拢到一起，
/// 让 workflow 只保留计价编排，不直接拼装多个 writer。
/// </remarks>
internal sealed record PricingSimulationPersistenceInput
{
    public PricingCalculateRequest Request { get; init; } = null!;

    public IReadOnlyList<PricingCalculateItemRequest> Items { get; init; } =
        Array.Empty<PricingCalculateItemRequest>();

    public IReadOnlyList<ItemPricingCalculation> Calculations { get; init; } =
        Array.Empty<ItemPricingCalculation>();

    public RuntimePackageTraceResolution? RuntimeTrace { get; init; }
}

/// <summary>
/// 试算持久化编排服务。
/// </summary>
public sealed class PricingSimulationPersistenceService
{
    private readonly PricingRequestLogWriter _requestLogWriter;
    private readonly PricingTraceStepWriter _traceStepWriter;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="PricingSimulationPersistenceService"/> class.
    /// </summary>
    /// <param name="requestLogWriter">A request log writer.</param>
    /// <param name="traceStepWriter">A trace step writer.</param>
    /// <param name="clock">A system clock.</param>
    public PricingSimulationPersistenceService(
        PricingRequestLogWriter requestLogWriter,
        PricingTraceStepWriter traceStepWriter,
        IClock clock)
    {
        _requestLogWriter = requestLogWriter;
        _traceStepWriter = traceStepWriter;
        _clock = clock;
    }

    internal async Task<PricingCalculateResponse> PersistAsync(PricingSimulationPersistenceInput input)
    {
        var requestLog = await SaveRequestLogAsync(input);
        await _traceStepWriter.SaveAsync(
            requestLog.RequestId,
            requestLog.TraceId,
            input.Calculations,
            input.RuntimeTrace);

        var response = BuildResponse(requestLog, input);
        await _requestLogWriter.SaveResponseJsonAsync(requestLog, response);
        return response;
    }

    private Task<ChargeRequest> SaveRequestLogAsync(PricingSimulationPersistenceInput input)
    {
        return _requestLogWriter.SaveAsync(new RequestLogSaveInput
        {
            Request = input.Request,
            Items = input.Items,
            Calculations = input.Calculations,
            CallType = PricingCallTypeCodes.Simulate,
            LifecycleKind = RequestLogLifecycleKind.Simulated,
            RuntimeTrace = input.RuntimeTrace
        });
    }

    private PricingCalculateResponse BuildResponse(
        ChargeRequest requestLog,
        PricingSimulationPersistenceInput input)
    {
        return PricingResponseBuilder.Build(
            requestLog.RequestId,
            requestLog.TraceId,
            input.Calculations,
            _clock.Now,
            input.RuntimeTrace);
    }
}
