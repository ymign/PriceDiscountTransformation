using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
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
        ChargeRequest requestLog;
        try
        {
            requestLog = await SaveRequestLogAsync(input);
        }
        catch (Exception ex) when (IsBusinessRequestNoUniqueConstraintViolation(ex))
        {
            throw BuildDuplicateBusinessRequestNoException(input.Request);
        }

        await _traceStepWriter.SaveAsync(
            requestLog.RequestId,
            requestLog.TraceId,
            input.Calculations);

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
            LifecycleKind = RequestLogLifecycleKind.Simulated
        });
    }

    private static BizException BuildDuplicateBusinessRequestNoException(PricingCalculateRequest request)
    {
        var sourceSystem = request.SourceSystem.Trim();
        var businessRequestNo = NormalizeString(request.BusinessRequestNo) ?? "(空)";
        return new BizException(
            BizErrorCode.BusinessRequestNoDuplicated,
            409,
            $"业务请求号重复：source_system={sourceSystem}, business_request_no={businessRequestNo}, call_type=SIMULATE。请更换业务请求号后重新试算。");
    }

    private static bool IsBusinessRequestNoUniqueConstraintViolation(Exception ex)
    {
        return ex.Message.Contains("ORA-00001", StringComparison.OrdinalIgnoreCase) &&
               ex.Message.Contains("UK_PR_CRL_BIZ", StringComparison.OrdinalIgnoreCase);
    }

    private PricingCalculateResponse BuildResponse(
        ChargeRequest requestLog,
        PricingSimulationPersistenceInput input)
    {
        return PricingResponseBuilder.Build(
            requestLog.RequestId,
            requestLog.TraceId,
            input.Calculations,
            _clock.Now);
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
