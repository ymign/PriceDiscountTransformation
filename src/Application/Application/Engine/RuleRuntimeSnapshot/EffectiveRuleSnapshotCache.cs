using Microsoft.Extensions.Caching.Memory;

namespace Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;

/// <summary>
/// 运行期生效规则快照缓存。
/// </summary>
public sealed class EffectiveRuleSnapshotCache
{
    private const string CacheKeyPrefix = "runtime:effective-rule-snapshot:";
    private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object KeysLock = new();

    private readonly IMemoryCache _cache;
    private readonly EffectiveRuleSnapshotLoader _loader;

    /// <summary>
    /// 初始化运行期生效规则快照缓存。
    /// </summary>
    public EffectiveRuleSnapshotCache(
        IMemoryCache cache,
        EffectiveRuleSnapshotLoader loader)
    {
        _cache = cache;
        _loader = loader;
    }

    /// <summary>
    /// 获取项目运行期候选规则快照。
    /// </summary>
    public async Task<IReadOnlyList<EffectiveRuleSnapshot>> GetByItemCodeAsync(string itemCode)
    {
        var key = BuildKey(itemCode);
        if (_cache.TryGetValue(key, out IReadOnlyList<EffectiveRuleSnapshot>? snapshots) &&
            snapshots is not null)
        {
            return snapshots;
        }

        snapshots = await _loader.LoadByItemCodeAsync(itemCode);
        _cache.Set(key, snapshots, TimeSpan.FromMinutes(10));
        lock (KeysLock)
        {
            Keys.Add(key);
        }

        return snapshots;
    }

    /// <summary>
    /// 清除全部运行期规则快照缓存。
    /// </summary>
    public int Clear()
    {
        List<string> keys;
        lock (KeysLock)
        {
            keys = Keys.ToList();
            Keys.Clear();
        }

        foreach (var key in keys)
        {
            _cache.Remove(key);
        }

        return keys.Count;
    }

    private static string BuildKey(string itemCode)
    {
        return CacheKeyPrefix + itemCode.Trim().ToUpperInvariant();
    }
}
