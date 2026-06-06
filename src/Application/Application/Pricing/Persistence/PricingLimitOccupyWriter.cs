using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

internal sealed record NegativeLimitOccupyInput
{
    public PricingReverseRequest Request { get; init; } = null!;

    public IReadOnlyList<ChargeDiscountDetail> MatchedDetails { get; init; } =
        Array.Empty<ChargeDiscountDetail>();

    public long ReverseRequestId { get; init; }

    public string? TraceId { get; init; }

    public decimal ReverseQty { get; init; }

    public decimal ReverseAmt { get; init; }

    public DateTime ReverseTime { get; init; }
}

/// <summary>
/// 计价限额占用写入器，负责 confirm 正向占额和 reverse 负向释放。
/// </summary>
public sealed class PricingLimitOccupyWriter
{
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly PricingOptions _options;
    private readonly ILogger<PricingLimitOccupyWriter> _logger;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化计价限额占用写入器。
    /// </summary>
    /// <param name="limitRepository">限额占用仓储。</param>
    /// <param name="options">计价配置项。</param>
    /// <param name="logger">日志组件。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    public PricingLimitOccupyWriter(
        ILimitOccupyRepository limitRepository,
        IOptions<PricingOptions> options,
        ILogger<PricingLimitOccupyWriter> logger,
        IClock clock)
    {
        _limitRepository = limitRepository;
        _options = options.Value;
        _logger = logger;
        _clock = clock;
    }

    internal async Task SaveAsync(
        long requestId,
        string? traceId,
        PricingCalculateItemRequest item,
        PricingResult result)
    {
        var now = _clock.Now;
        var expireAt = now.AddMinutes(_options.ConfirmExpireMinutes);
        var resultGroupNo = PricingResultGroupNoGenerator.Resolve(requestId, item, result);
        foreach (var occupy in result.LimitOccupies)
        {
            occupy.RequestId = requestId;
            occupy.TraceId = traceId;
            occupy.ChargeDetailNo = NormalizeString(item.ChargeDetailNo);
            occupy.ResultGroupNo = resultGroupNo;
            occupy.Status = OccupyStatusCodes.Pending;
            occupy.ExpireAt = expireAt;
            occupy.OccupiedAt = now;
            await _limitRepository.InsertAsync(occupy);

            _logger.LogDebug(
                "限额占用 RequestId={RequestId}, LimitType={LimitType}, LimitDimensionCode={LimitDimensionCode}, OccupyQty={OccupyQty}, ItemCode={ItemCode}",
                requestId, occupy.LimitType, occupy.LimitDimensionCode, occupy.OccupyQty, occupy.ItemCode);
        }
    }

    internal async Task InsertNegativeAsync(NegativeLimitOccupyInput input)
    {
        var request = input.Request;
        var originalOccupies = await _limitRepository.GetByRequestIdAsync(request.OriginalRequestId);
        var matchedChargeDetailNos = input.MatchedDetails
            .Select(d => NormalizeString(d.ChargeDetailNo))
            .Where(chargeDetailNo => chargeDetailNo is not null)
            .Select(chargeDetailNo => chargeDetailNo!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var matchedResultGroupNos = input.MatchedDetails
            .Select(d => NormalizeString(d.ResultGroupNo))
            .Where(resultGroupNo => resultGroupNo is not null)
            .Select(resultGroupNo => resultGroupNo!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var matchedItemCodes = input.MatchedDetails
            .Select(d => NormalizeString(d.ItemCode))
            .Where(itemCode => itemCode is not null)
            .Select(itemCode => itemCode!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var candidates = originalOccupies
            .Where(o => o.Status == BusinessStatusCodes.Confirmed)
            .Where(o => o.OccupyType == "CHARGE")
            .Where(o => MatchReverseOccupyCandidate(
                o,
                matchedChargeDetailNos,
                matchedResultGroupNos,
                matchedItemCodes))
            .Where(o => !request.PartSeq.HasValue || o.PartSeq == request.PartSeq)
            .Where(o => IsSameBusinessDay(o.BusinessChargeTime, input.ReverseTime))
            .ToList();

        var totalOriginalQty = candidates.Sum(o => Math.Abs(o.OccupyQty));
        if (totalOriginalQty <= 0)
        {
            return;
        }

        foreach (var occupy in candidates)
        {
            var now = _clock.Now;
            var ratio = Math.Abs(occupy.OccupyQty) / totalOriginalQty;
            var releaseQty = input.ReverseQty * ratio;
            var releaseAmt = input.ReverseAmt * ratio;
            await _limitRepository.InsertAsync(new LimitOccupy
            {
                RequestId = input.ReverseRequestId,
                TraceId = input.TraceId ?? occupy.TraceId,
                PatientId = occupy.PatientId,
                ItemCode = occupy.ItemCode,
                ChargeDetailNo = occupy.ChargeDetailNo,
                RuleId = occupy.RuleId,
                RuleVersionNo = occupy.RuleVersionNo,
                ResultGroupNo = occupy.ResultGroupNo,
                LimitType = occupy.LimitType,
                LimitKey = occupy.LimitKey,
                LimitDimensionCode = occupy.LimitDimensionCode,
                OccupyQty = -releaseQty,
                OccupyAmt = -PricingAmountRounder.RoundFinal(releaseAmt),
                OccupyType = PricingCallTypeCodes.Reverse,
                OriginalOccupyId = occupy.OccupyId,
                BusinessChargeTime = occupy.BusinessChargeTime,
                PartSeq = occupy.PartSeq,
                Status = BusinessStatusCodes.Confirmed,
                OccupiedAt = now,
                ConfirmedAt = now
            });

            _logger.LogInformation(
                "限额释放 OriginalRequestId={OriginalRequestId}, LimitType={LimitType}, LimitDimensionCode={LimitDimensionCode}, ReleaseQty={ReleaseQty}, ReleaseAmt={ReleaseAmt}",
                request.OriginalRequestId, occupy.LimitType, occupy.LimitDimensionCode, releaseQty, releaseAmt);
        }
    }

    private static bool IsSameBusinessDay(DateTime? originalBusinessTime, DateTime reverseTime)
    {
        return originalBusinessTime.HasValue &&
               originalBusinessTime.Value.Date == reverseTime.Date;
    }

    private static bool MatchReverseOccupyCandidate(
        LimitOccupy occupy,
        IReadOnlySet<string> matchedChargeDetailNos,
        IReadOnlySet<string> matchedResultGroupNos,
        IReadOnlySet<string> matchedItemCodes)
    {
        var occupyChargeDetailNo = NormalizeString(occupy.ChargeDetailNo);
        var occupyResultGroupNo = NormalizeString(occupy.ResultGroupNo);
        var occupyItemCode = NormalizeString(occupy.ItemCode);

        if (matchedResultGroupNos.Count > 0)
        {
            return (occupyResultGroupNo is not null && matchedResultGroupNos.Contains(occupyResultGroupNo)) ||
                   (occupyChargeDetailNo is not null && matchedChargeDetailNos.Contains(occupyChargeDetailNo)) ||
                   (occupyResultGroupNo is null &&
                    occupyChargeDetailNo is null &&
                    occupyItemCode is not null &&
                    matchedItemCodes.Contains(occupyItemCode));
        }

        if (matchedChargeDetailNos.Count > 0)
        {
            return (occupyChargeDetailNo is not null && matchedChargeDetailNos.Contains(occupyChargeDetailNo)) ||
                   (occupyChargeDetailNo is null &&
                    occupyItemCode is not null &&
                    matchedItemCodes.Contains(occupyItemCode));
        }

        return occupyItemCode is not null && matchedItemCodes.Contains(occupyItemCode);
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
