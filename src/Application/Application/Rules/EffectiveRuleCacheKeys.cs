using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace Pricing.RuleCenter.Application.Rules;

/// <summary>
/// 生效规则缓存键登记器。
/// </summary>
internal static class EffectiveRuleCacheKeys
{
    /// <summary>
    /// 已写入 <see cref="IMemoryCache"/> 的生效规则缓存键集合。
    /// </summary>
    /// <remarks>
    /// IMemoryCache 没有按前缀批量清理能力，因此发布、停用、回滚规则时必须记住曾经写入过哪些 key，
    /// 再逐个删除，避免计价入口继续使用旧规则快照。
    /// </remarks>
    private static readonly ConcurrentDictionary<string, byte> Keys = new(StringComparer.Ordinal);

    /// <summary>
    /// 登记一个已写入内存缓存的生效规则缓存键。
    /// </summary>
    /// <param name="cacheKey">生效规则缓存键。调用方应传入实际写入 <see cref="IMemoryCache"/> 的完整 key。</param>
    public static void Track(string cacheKey)
    {
        Keys[cacheKey] = 0;
    }

    /// <summary>
    /// 清除当前登记过的所有生效规则缓存键。
    /// </summary>
    /// <param name="cache">应用级内存缓存实例。</param>
    /// <returns>本次成功从登记集合中移除的缓存键数量。</returns>
    /// <remarks>
    /// 删除缓存和移除登记集合分开执行。即使某个 key 已经过期或不存在，调用 <see cref="IMemoryCache.Remove"/>
    /// 仍然是安全的；只有从登记集合删除成功时才计数，便于发布日志反映真实清理规模。
    /// </remarks>
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




