using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Pricing.RuleCenter.Application.Pricing.Queries;

/// <summary>
/// 特殊项目标识缓存键注册表。
/// </summary>
public static class SpecialFlagCacheKeys
{
    private const string Prefix = "pricing:special-flag:";
    private static readonly HashSet<string> s_keys = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object s_gate = new();

    /// <summary>生成并登记缓存键。</summary>
    public static string Register(string itemCode)
    {
        var key = Prefix + itemCode.Trim().ToUpperInvariant();
        return RegisterKey(key);
    }

    /// <summary>生成并登记缓存键。</summary>
    public static string Register(GetSpecialFlagQuery query)
    {
        var key = string.Join(
            "|",
            Prefix + Normalize(query.ItemCode),
            Normalize(query.ChargeScene),
            query.BusinessChargeTime?.ToString("O", CultureInfo.InvariantCulture) ?? "-",
            Normalize(query.VisitType),
            Normalize(query.BodyPartCode),
            Normalize(query.ChargeDeptCode));
        return RegisterKey(key);
    }

    private static string RegisterKey(string key)
    {
        lock (s_gate)
        {
            s_keys.Add(key);
        }

        return key;
    }

    private static string Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim().ToUpperInvariant();
    }

    /// <summary>清除全部已登记的特殊项目标识缓存。</summary>
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
