using Newtonsoft.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

internal sealed record ReverseRequestLogSaveInput
{
    public PricingReverseRequest Request { get; init; } = null!;

    public ChargeRequest OriginalLog { get; init; } = null!;

    public IReadOnlyList<ChargeDiscountDetail> MatchedDetails { get; init; } =
        Array.Empty<ChargeDiscountDetail>();

    public decimal ReverseQty { get; init; }

    public decimal ReverseAmt { get; init; }

    public DateTime ReverseTime { get; init; }
}

internal sealed record ReverseLogSaveInput
{
    public PricingReverseRequest Request { get; init; } = null!;

    public ChargeRequest OriginalLog { get; init; } = null!;

    public IReadOnlyList<ChargeDiscountDetail> MatchedDetails { get; init; } =
        Array.Empty<ChargeDiscountDetail>();

    public long ReverseRequestId { get; init; }

    public decimal ReverseQty { get; init; }

    public decimal ReverseAmt { get; init; }

    public DateTime ReverseTime { get; init; }
}

/// <summary>
/// 冲正日志写入器，负责 reverse 请求日志和冲正明细日志落库。
/// </summary>
public sealed class PricingReverseLogWriter
{
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IChargeReverseLogRepository _reverseLogRepository;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化冲正日志写入器。
    /// </summary>
    /// <param name="requestLogRepository">计价请求日志仓储。</param>
    /// <param name="reverseLogRepository">冲正日志仓储。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    public PricingReverseLogWriter(
        IChargeRequestLogRepository requestLogRepository,
        IChargeReverseLogRepository reverseLogRepository,
        IClock clock)
    {
        _requestLogRepository = requestLogRepository;
        _reverseLogRepository = reverseLogRepository;
        _clock = clock;
    }

    internal async Task<long> SaveRequestLogAsync(ReverseRequestLogSaveInput input)
    {
        var request = input.Request;
        var originalLog = input.OriginalLog;
        var matchedDetails = input.MatchedDetails;
        var firstDetail = matchedDetails.FirstOrDefault();
        var now = _clock.Now;
        var reverseRequestLog = new ChargeRequest
        {
            RequestNo = PricingLockKeyBuilder.BuildReverseRequestNo(request.OriginalRequestId, request.ReverseNo!),
            BusinessRequestNo = NormalizeString(request.ReverseNo),
            RequestFingerprint = PricingRequestFingerprintBuilder.BuildReverseFingerprint(request, originalLog, input.ReverseTime),
            TraceId = originalLog.TraceId,
            CallType = PricingCallTypeCodes.Reverse,
            BusinessStatus = BusinessStatusCodes.Reversed,
            SourceSystem = NormalizeString(originalLog.SourceSystem) ?? "UNKNOWN",
            SourceTerminal = originalLog.SourceTerminal,
            PatientId = originalLog.PatientId,
            VisitId = originalLog.VisitId,
            ChargeScene = originalLog.ChargeScene,
            ChargeNo = originalLog.ChargeNo,
            ChargeDetailNo = NormalizeString(request.ChargeDetailNo) ?? firstDetail?.ChargeDetailNo ?? originalLog.ChargeDetailNo,
            ResultGroupNo = firstDetail?.ResultGroupNo ?? originalLog.ResultGroupNo,
            ItemCode = NormalizeString(request.ItemCode) ?? firstDetail?.ItemCode ?? originalLog.ItemCode,
            ItemName = firstDetail?.ItemName ?? originalLog.ItemName,
            InputQty = input.ReverseQty,
            InputUnit = originalLog.InputUnit,
            BodyPartCode = originalLog.BodyPartCode,
            BusinessChargeTime = input.ReverseTime,
            PriceVersion = originalLog.PriceVersion,
            RequestJson = JsonConvert.SerializeObject(request),
            ResponseJson = JsonConvert.SerializeObject(new
            {
                request.OriginalRequestId,
                request.ReverseNo,
                ReverseQty = input.ReverseQty,
                ReverseAmt = input.ReverseAmt
            }),
            RequestAt = now,
            ResponseAt = now,
            IsSuccess = EnableFlag.Yes
        };

        return await _requestLogRepository.InsertAsync(reverseRequestLog);
    }

    internal Task SaveReverseLogAsync(ReverseLogSaveInput input)
    {
        return _reverseLogRepository.InsertAsync(new ChargeReverseLog
        {
            OriginalRequestId = input.Request.OriginalRequestId,
            ReverseRequestId = input.ReverseRequestId,
            TraceId = input.OriginalLog.TraceId,
            ChargeNo = input.OriginalLog.ChargeNo,
            ReverseNo = input.Request.ReverseNo,
            ChargeDetailNo = NormalizeString(input.Request.ChargeDetailNo),
            ItemCode = NormalizeString(input.Request.ItemCode) ??
                       input.MatchedDetails.FirstOrDefault()?.ItemCode ??
                       input.OriginalLog.ItemCode,
            PartSeq = input.Request.PartSeq,
            ReverseQty = input.ReverseQty,
            ReverseAmt = input.ReverseAmt,
            ReverseReason = input.Request.Reason,
            ReversedBy = input.Request.ReversedBy,
            ReversedAt = input.ReverseTime
        });
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
