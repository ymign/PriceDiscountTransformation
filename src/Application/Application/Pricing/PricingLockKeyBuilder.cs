using System.Security.Cryptography;
using System.Text;

namespace Pricing.RuleCenter.Application.Pricing;

internal static class PricingLockKeyBuilder
{
    public static string BuildIdempotencyLockKey(string sourceSystem, string businessRequestNo, string callType)
    {
        return $"IDEMP:{sourceSystem.Trim()}:{businessRequestNo.Trim()}:{callType.Trim()}"
            .ToUpperInvariant();
    }

    public static string BuildRequestLockKey(long requestId)
    {
        return $"REQ:{requestId}".ToUpperInvariant();
    }

    public static string BuildReverseLockKey(long originalRequestId, string reverseNo)
    {
        return $"REV:{originalRequestId}:{reverseNo.Trim()}".ToUpperInvariant();
    }

    public static string BuildReverseRequestNo(long originalRequestId, string reverseNo)
    {
        var raw = $"{originalRequestId}:{reverseNo.Trim()}";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)))[..16];
        return $"REV-{originalRequestId}-{hash}";
    }
}


