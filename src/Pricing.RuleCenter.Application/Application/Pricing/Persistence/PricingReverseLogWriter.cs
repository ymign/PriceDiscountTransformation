using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

/// <summary>
/// 保存 reverse 请求日志所需输入。
/// </summary>
/// <remarks>
/// reverse 也会写入 PR_CHARGE_REQUEST_LOG，形成独立请求事实，便于按退费流水追溯。
/// </remarks>
internal sealed record ReverseRequestLogSaveInput
{
    /// <summary>退费请求。</summary>
    public PricingReverseRequest Request { get; init; } = null!;

    public ChargeRequest OriginalLog { get; init; } = null!;

    public IReadOnlyList<ChargeDiscountDetail> MatchedDetails { get; init; } =
        Array.Empty<ChargeDiscountDetail>();

    public decimal ReverseQty { get; init; }

    public decimal ReverseAmt { get; init; }

    public DateTime ReverseTime { get; init; }
}

/// <summary>
/// 保存冲正明细日志所需输入。
/// </summary>
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

    public bool IsFullReverse { get; init; }
}

/// <summary>
/// 冲正日志写入器，负责 reverse 请求日志和冲正明细日志落库。
/// </summary>
public sealed class PricingReverseLogWriter
{
    /// <summary>
    /// 请求日志仓储，用于保存 reverse 请求主记录。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;
    /// <summary>
    /// 冲正日志仓储，用于保存退费明细事实。
    /// </summary>
    private readonly IChargeReverseLogRepository _reverseLogRepository;
    /// <summary>
    /// 统一时钟。
    /// </summary>
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
        // reverse 请求日志复用原 TraceId，表示它属于原收费计价链路的一次冲正分支。
        var request = input.Request;
        var originalLog = input.OriginalLog;
        var matchedDetails = input.MatchedDetails;
        var firstDetail = matchedDetails.FirstOrDefault();
        var now = _clock.Now;
        var reverseRequestLog = new ChargeRequest
        {
            RequestNo = PricingLockKeyBuilder.BuildReverseRequestNo(request.OriginalRequestId, request.ReverseNo!),
            BusinessRequestNo = NormalizeString(request.ReverseNo),
            // reverse 指纹覆盖退费范围、数量、金额、时间和原因，用于同 ReverseNo 重试一致性判断。
            RequestFingerprint = PricingRequestFingerprintBuilder.BuildReverseFingerprint(request, originalLog, input.ReverseTime),
            TraceId = originalLog.TraceId,
            CallType = PricingCallTypeCodes.Reverse,
            SourceSystem = NormalizeString(request.SourceSystem) ?? NormalizeString(originalLog.SourceSystem) ?? "UNKNOWN",
            SourceTerminal = NormalizeString(request.SourceTerminal) ?? originalLog.SourceTerminal,
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
            RequestJson = RuleCenterJsonSerializer.Serialize(request),
            ResponseJson = RuleCenterJsonSerializer.Serialize(new
            {
                request.OriginalRequestId,
                request.ReverseNo,
                request.SourceSystem,
                request.SourceTerminal,
                ReverseQty = input.ReverseQty,
                ReverseAmt = input.ReverseAmt
            }),
            RequestAt = now,
        };
        RequestLogLifecycleInitializer.Apply(reverseRequestLog, RequestLogLifecycleKind.Reversed, now);

        return await _requestLogRepository.InsertAsync(reverseRequestLog);
    }

    internal Task SaveReverseLogAsync(ReverseLogSaveInput input)
    {
        // 冲正日志是历史已退数量/金额累计和退费幂等判断的事实来源。
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
            ReverseType = input.IsFullReverse ? "FULL" : "PARTIAL",
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
