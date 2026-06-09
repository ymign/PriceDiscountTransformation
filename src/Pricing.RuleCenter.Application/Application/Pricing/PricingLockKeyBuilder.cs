using System.Security.Cryptography;
using System.Text;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 计价状态机使用的数据库锁键构建器。
/// </summary>
/// <remarks>
/// 锁键最终会写入或锁定 <c>PR_LIMIT_LOCK</c>，通过 Oracle 行锁实现跨请求串行化。
/// 不同业务边界使用不同前缀：IDEMP 保护 confirm 幂等，REQ 保护请求状态流转，REV 保护退费流水幂等。
/// </remarks>
internal static class PricingLockKeyBuilder
{
    /// <summary>
    /// 构建 confirm 幂等锁键。
    /// </summary>
    /// <param name="sourceSystem">来源系统。</param>
    /// <param name="businessRequestNo">稳定业务请求号。</param>
    /// <param name="callType">调用类型。</param>
    /// <returns>幂等锁键。</returns>
    public static string BuildIdempotencyLockKey(string sourceSystem, string businessRequestNo, string callType)
    {
        return $"IDEMP:{sourceSystem.Trim()}:{businessRequestNo.Trim()}:{callType.Trim()}"
            .ToUpperInvariant();
    }

    /// <summary>
    /// 构建请求状态流转锁键。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <returns>请求维度锁键。</returns>
    public static string BuildRequestLockKey(long requestId)
    {
        return $"REQ:{requestId}".ToUpperInvariant();
    }

    /// <summary>
    /// 构建退费流水幂等锁键。
    /// </summary>
    /// <param name="originalRequestId">原始已落账请求 ID。</param>
    /// <param name="reverseNo">HIS 退费流水号。</param>
    /// <returns>退费幂等锁键。</returns>
    public static string BuildReverseLockKey(long originalRequestId, string reverseNo)
    {
        return $"REV:{originalRequestId}:{reverseNo.Trim()}".ToUpperInvariant();
    }

    /// <summary>
    /// 构建 reverse 请求日志使用的技术请求号。
    /// </summary>
    /// <param name="originalRequestId">原始已落账请求 ID。</param>
    /// <param name="reverseNo">HIS 退费流水号。</param>
    /// <returns>稳定且长度可控的退费请求号。</returns>
    /// <remarks>
    /// reverseNo 可能很长或包含不适合直接落库展示的字符，因此用原请求 ID + 短哈希生成技术请求号。
    /// 幂等判断仍以 ReverseNo 和指纹为准，不依赖该技术号。
    /// </remarks>
    public static string BuildReverseRequestNo(long originalRequestId, string reverseNo)
    {
        var raw = $"{originalRequestId}:{reverseNo.Trim()}";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)))[..16];
        return $"REV-{originalRequestId}-{hash}";
    }
}


