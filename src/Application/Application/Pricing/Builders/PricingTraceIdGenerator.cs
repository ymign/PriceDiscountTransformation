using System.Security.Cryptography;
using System.Text;

namespace Pricing.RuleCenter.Application.Pricing.Builders;

internal static class PricingTraceIdGenerator
{
    public static string Build(
        string callType,
        string? requestNo,
        string? businessRequestNo,
        DateTime now)
    {
        var seed = $"{callType}:{NormalizeString(requestNo)}:{NormalizeString(businessRequestNo)}:{Guid.NewGuid():N}";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(seed)))[..24];
        return $"TRACE-{now:yyyyMMddHHmmssfff}-{hash}";
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
