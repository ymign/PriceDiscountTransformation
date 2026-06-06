using Newtonsoft.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

internal sealed record RequestLogSaveInput
{
    public PricingCalculateRequest Request { get; init; } = null!;

    public IReadOnlyList<PricingCalculateItemRequest> Items { get; init; } =
        Array.Empty<PricingCalculateItemRequest>();

    public IReadOnlyList<ItemPricingCalculation> Calculations { get; init; } =
        Array.Empty<ItemPricingCalculation>();

    public string CallType { get; init; } = string.Empty;

    public string BusinessStatus { get; init; } = string.Empty;

    public string? Fingerprint { get; init; }
}

/// <summary>
/// 计价请求日志写入器，负责保存请求主表和响应快照。
/// </summary>
public sealed class PricingRequestLogWriter
{
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化计价请求日志写入器。
    /// </summary>
    /// <param name="requestLogRepository">请求日志仓储。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    public PricingRequestLogWriter(
        IChargeRequestLogRepository requestLogRepository,
        IClock clock)
    {
        _requestLogRepository = requestLogRepository;
        _clock = clock;
    }

    internal async Task<ChargeRequest> SaveAsync(RequestLogSaveInput input)
    {
        var request = input.Request;
        var items = input.Items;
        var calculations = input.Calculations;
        var now = _clock.Now;
        var log = new ChargeRequest
        {
            RequestNo = NormalizeString(request.RequestNo) ?? $"REQ-{now:yyyyMMddHHmmssfff}",
            BusinessRequestNo = NormalizeString(request.BusinessRequestNo),
            RequestFingerprint = input.Fingerprint,
            TraceId = PricingTraceIdGenerator.Build(input.CallType, request.RequestNo, request.BusinessRequestNo, now),
            CallType = input.CallType,
            BusinessStatus = input.BusinessStatus,
            SourceSystem = request.SourceSystem.Trim(),
            SourceTerminal = NormalizeString(request.SourceTerminal),
            PatientId = request.PatientId.Trim(),
            VisitId = NormalizeString(request.VisitId),
            ChargeScene = NormalizeString(request.ChargeScene),
            ChargeNo = NormalizeString(request.ChargeNo),
            ChargeDetailNo = items.Count == 1 ? NormalizeString(items[0].ChargeDetailNo) : null,
            ItemCode = items.Count == 1 ? items[0].ItemCode.Trim() : null,
            ItemName = items.Count == 1 ? NormalizeString(items[0].ItemName) : null,
            InputQty = items.Count == 1 ? items[0].InputQty : null,
            InputUnit = items.Count == 1 ? NormalizeString(items[0].Unit) : null,
            BodyPartCode = items.Count == 1 ? NormalizeString(items[0].BodyPartCode) : null,
            BusinessChargeTime = items.Count == 1
                ? items[0].BusinessChargeTime ?? request.BusinessChargeTime
                : request.BusinessChargeTime,
            RequestJson = JsonConvert.SerializeObject(request),
            ResponseJson = JsonConvert.SerializeObject(calculations.Select(c => c.Result).ToList()),
            RequestAt = now,
            ResponseAt = now,
            IsSuccess = EnableFlag.Yes
        };

        await _requestLogRepository.InsertAsync(log);
        return log;
    }

    internal async Task SaveResponseJsonAsync(ChargeRequest log, PricingCalculateResponse response)
    {
        log.ResponseJson = JsonConvert.SerializeObject(response);
        log.ResponseAt = _clock.Now;
        await _requestLogRepository.UpdateAsync(log);
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
