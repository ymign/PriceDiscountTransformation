using System.Security.Cryptography;
using System.Text;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Pricing.Builders;

internal static class PricingResultGroupNoGenerator
{
    public static string Build(long requestId, PricingCalculateItemRequest item, string groupType)
    {
        var chargeDetailNo = NormalizeString(item.ChargeDetailNo) ?? "NO_DETAIL";
        var itemRequestNo = NormalizeString(item.ItemRequestNo) ?? "NO_ITEM_REQUEST";
        var rawKey = $"{item.ItemCode.Trim()}:{chargeDetailNo}:{itemRequestNo}".ToUpperInvariant();
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)))[..12];
        return $"{groupType}:{requestId}:{hash}";
    }

    public static string? Resolve(long requestId, PricingCalculateItemRequest item, PricingResult result)
    {
        var hasChildItems = result.ChildPricingResults.Count > 0;
        return result.ReplaceChildResult is null && !hasChildItems
            ? null
            : Build(requestId, item, result.ReplaceChildResult is not null ? "REPLACE" : "CHILD");
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
