using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Application.Dto;
using System.Globalization;

namespace Pricing.RuleCenter.Application.Pricing.Queries;

/// <summary>
/// 特殊项目标识缓存键注册表。
/// </summary>
public static class SpecialFlagCacheKeys
{
    /// <summary>
    /// 特殊项目缓存键前缀，避免与其他内存缓存项冲突。
    /// </summary>
    private const string Prefix = "pricing:special-flag:";
    /// <summary>
    /// 已登记的缓存键集合，用于规则发布、停用或回滚时批量清理。
    /// </summary>
    private static readonly HashSet<string> s_keys = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// 缓存键注册表的进程内锁，保护 HashSet 的并发读写。
    /// </summary>
    private static readonly object s_gate = new();

    /// <summary>
    /// 只按项目编码生成并登记缓存键。
    /// </summary>
    /// <param name="itemCode">项目编码。</param>
    /// <returns>标准化后的缓存键。</returns>
    public static string Register(string itemCode)
    {
        var key = Prefix + itemCode.Trim().ToUpperInvariant();
        return RegisterKey(key);
    }

    /// <summary>
    /// 按特殊项目查询完整维度生成并登记缓存键。
    /// </summary>
    /// <param name="request">特殊项目查询请求。</param>
    /// <returns>包含项目、场景、业务时间、就诊类型、部位和收费科室的缓存键。</returns>
    public static string Register(SpecialFlagRequest request)
    {
        var key = string.Join(
            "|",
            Prefix + Normalize(request.ItemCode),
            Normalize(request.ChargeScene),
            request.BusinessChargeTime?.ToString("O", CultureInfo.InvariantCulture) ?? "-",
            Normalize(request.VisitType),
            Normalize(request.BodyPartCode),
            Normalize(request.ChargeDeptCode));
        return RegisterKey(key);
    }

    private static string RegisterKey(string key)
    {
        // IMemoryCache 本身不提供按前缀批量清理能力，因此需要单独登记所有生成过的键。
        // 规则发布/停用后 RulePublishCacheInvalidator 会调用 Clear 清掉这些键。
        lock (s_gate)
        {
            s_keys.Add(key);
        }

        return key;
    }

    private static string Normalize(string? value)
    {
        // 空维度统一写成 "-"，避免 null、空串和空白字符串生成不同缓存键。
        return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim().ToUpperInvariant();
    }

    /// <summary>
    /// 清除全部已登记的特殊项目标识缓存。
    /// </summary>
    /// <param name="cache">进程内缓存。</param>
    /// <returns>实际尝试移除的缓存键数量。</returns>
    public static int Clear(IMemoryCache cache)
    {
        string[] keys;
        lock (s_gate)
        {
            keys = s_keys.ToArray();
            s_keys.Clear();
        }

        foreach (var key in keys)
        {
            cache.Remove(key);
        }

        return keys.Length;
    }
}
