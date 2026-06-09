using System.Security.Cryptography;
using System.Text;

namespace Pricing.RuleCenter.Application.Pricing.Builders;

/// <summary>
/// 计价追溯流水号生成器。
/// </summary>
/// <remarks>
/// TraceId 不是业务幂等键，而是追溯链路标识。它用于把请求日志、步骤日志、折价明细和冲正日志串起来，
/// 方便收费处或运维按一次计价过程排查问题。
/// </remarks>
internal static class PricingTraceIdGenerator
{
    /// <summary>
    /// 生成一次计价请求的追溯流水号。
    /// </summary>
    /// <param name="callType">调用类型，例如 SIMULATE、CONFIRM、REVERSE。</param>
    /// <param name="requestNo">调用方技术请求号。</param>
    /// <param name="businessRequestNo">调用方稳定业务请求号。</param>
    /// <param name="now">当前技术时间。</param>
    /// <returns>格式为 TRACE-时间戳-哈希 的追溯流水号。</returns>
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
        // TraceId 的种子只需要稳定去空白；最终仍加入 Guid，避免相同业务号多次试算产生相同 TraceId。
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
