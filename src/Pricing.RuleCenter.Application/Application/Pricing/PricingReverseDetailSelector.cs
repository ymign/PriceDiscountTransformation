using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing;

internal static class PricingReverseDetailSelector
{
    public static IReadOnlyList<ChargeDiscountDetail> FilterReverseDetails(
        IReadOnlyList<ChargeDiscountDetail> details,
        PricingReverseRequest request)
    {
        var confirmedDetails = details
            .Where(d => d.Status == "CONFIRMED" || d.Status == "COMMITTED")
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


