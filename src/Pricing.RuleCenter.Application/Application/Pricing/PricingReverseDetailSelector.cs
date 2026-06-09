using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// reverse 阶段原折价明细筛选和退费幂等参数比对工具。
/// </summary>
/// <remarks>
/// 退费请求可能只传原请求 ID，也可能进一步指定 chargeDetailNo、itemCode 或 partSeq。
/// 本类负责把这些筛选条件应用到原 confirm/commit 保存的折价明细上，并在命中 resultGroupNo 时自动带出同组主子项目。
/// </remarks>
internal static class PricingReverseDetailSelector
{
    /// <summary>
    /// 根据退费请求筛选本次可退费的原折价明细。
    /// </summary>
    /// <param name="details">原请求下的全部折价明细。</param>
    /// <param name="request">退费请求。</param>
    /// <returns>本次退费应覆盖的折价明细集合。</returns>
    public static IReadOnlyList<ChargeDiscountDetail> FilterReverseDetails(
        IReadOnlyList<ChargeDiscountDetail> details,
        PricingReverseRequest request)
    {
        // 只有已落账状态的明细可以退费。PENDING/CANCELLED/EXPIRED 不属于 HIS 已收费事实。
        var confirmedDetails = details
            .Where(d => d.Status == BusinessStatusCodes.Confirmed || d.Status == BusinessStatusCodes.Committed)
            .ToList();
        var query = confirmedDetails.AsEnumerable();

        var chargeDetailNo = NormalizeString(request.ChargeDetailNo);
        if (!string.IsNullOrWhiteSpace(chargeDetailNo))
        {
            query = query.Where(d => string.Equals(
                d.ChargeDetailNo, chargeDetailNo, StringComparison.OrdinalIgnoreCase));
        }

        var itemCode = NormalizeString(request.ItemCode);
        if (!string.IsNullOrWhiteSpace(itemCode))
        {
            query = query.Where(d => string.Equals(
                d.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase));
        }

        if (request.PartSeq.HasValue)
        {
            query = query.Where(d => d.PartSeq == request.PartSeq);
        }

        var matched = query.ToList();
        // 命中任意 resultGroupNo 时，要把同组明细全部带出。
        // 主项目和替换/加收子项必须按组退费，避免只退主项导致子项残留。
        var matchedGroupNos = matched
            .Select(d => NormalizeString(d.ResultGroupNo))
            .Where(groupNo => groupNo is not null)
            .Select(groupNo => groupNo!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (matchedGroupNos.Count == 0)
        {
            return matched;
        }

        return confirmedDetails
            .Where(d =>
            {
                var groupNo = NormalizeString(d.ResultGroupNo);
                return groupNo is not null && matchedGroupNos.Contains(groupNo);
            })
            .ToList();
    }

    /// <summary>
    /// 判断已有冲正流水是否与本次 reverse 请求表示同一笔退费。
    /// </summary>
    /// <param name="existing">已保存的冲正日志。</param>
    /// <param name="request">本次退费请求。</param>
    /// <returns>关键参数一致时返回 true。</returns>
    /// <remarks>
    /// 相同 ReverseNo 只允许作为同一笔退费重试。若数量、金额、明细、片段、退费时间或原因发生变化，
    /// 说明调用方复用了退费流水号但业务事实不同，应由 workflow 返回幂等冲突。
    /// </remarks>
    public static bool IsSameReverseRequest(
        ChargeReverseLog existing,
        PricingReverseRequest request)
    {
        var requestChargeDetailNo = NormalizeString(request.ChargeDetailNo);
        if (requestChargeDetailNo is not null &&
            !string.Equals(
                NormalizeString(existing.ChargeDetailNo),
                requestChargeDetailNo,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var requestItemCode = NormalizeString(request.ItemCode);
        if (requestItemCode is not null &&
            !string.Equals(
                NormalizeString(existing.ItemCode),
                requestItemCode,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (existing.PartSeq != request.PartSeq)
        {
            return false;
        }

        if (request.ReverseQty.HasValue &&
            Math.Round(existing.ReverseQty ?? 0m, 4) != Math.Round(request.ReverseQty.Value, 4))
        {
            return false;
        }

        if (request.ReverseAmt.HasValue &&
            PricingAmountRounder.RoundFinal(existing.ReverseAmt ?? 0m) !=
            PricingAmountRounder.RoundFinal(request.ReverseAmt.Value))
        {
            return false;
        }

        if (request.ReverseTime.HasValue &&
            NormalizeToSecond(existing.ReversedAt) != NormalizeToSecond(request.ReverseTime.Value))
        {
            return false;
        }

        var requestReversedBy = NormalizeString(request.ReversedBy);
        if (requestReversedBy is not null &&
            !string.Equals(
                NormalizeString(existing.ReversedBy),
                requestReversedBy,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var requestReason = NormalizeString(request.Reason);
        if (requestReason is not null &&
            !string.Equals(
                NormalizeString(existing.ReverseReason),
                requestReason,
                StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    private static DateTime NormalizeToSecond(DateTime value)
    {
        // HIS 与规则中心的时间精度可能不同，幂等比对按秒对齐，避免毫秒差导致同一退费重试被误判冲突。
        return new DateTime(
            value.Year,
            value.Month,
            value.Day,
            value.Hour,
            value.Minute,
            value.Second,
            value.Kind);
    }

    private static string? NormalizeString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
