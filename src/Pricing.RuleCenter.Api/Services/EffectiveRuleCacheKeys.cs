using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace Pricing.RuleCenter.Api.Services;

/// <summary>
/// 生效规则缓存键登记器。
/// </summary>
internal static class EffectiveRuleCacheKeys
{
    private static readonly ConcurrentDictionary<string, byte> Keys = new(StringComparer.Ordinal);

    public static void Track(string cacheKey)
    {
        Keys[cacheKey] = 0;
    }

    public static int Clear(IMemoryCache cache)
    {
        var removed = 0;
        foreach (var cacheKey in Keys.Keys)
        {
            cache.Remove(cacheKey);
            if (Keys.TryRemove(cacheKey, out _))
            {
                removed++;
            }
        }

        return removed;
    }
}
