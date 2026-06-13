using Microsoft.Extensions.Caching.Memory;
namespace Pricing.RuleCenter.Application.Engine.RuleRuntimeSnapshot;

/// <summary>
/// 运行期生效规则快照缓存。
/// </summary>
/// <remarks>
/// 规则匹配是计价接口的高频路径。该缓存按项目编码缓存候选规则快照，
/// 通过发布/激活后的统一失效机制保证不会长期保留旧包数据。
/// </remarks>
public sealed class EffectiveRuleSnapshotCache : IEffectiveRuleSnapshotCache
{
    /// <summary>
    /// 缓存键前缀。
    /// </summary>
    private const string CacheKeyPrefix = "runtime:effective-rule-snapshot:";
    /// <summary>
    /// 已登记缓存键集合，用于发布/停用/回滚后批量清理。
    /// </summary>
    private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// 缓存键集合锁。
    /// </summary>
    private static readonly object KeysLock = new();

    /// <summary>
    /// 进程内缓存。
    /// </summary>
    private readonly IMemoryCache _cache;
    /// <summary>
    /// 快照加载器。
    /// </summary>
    private readonly EffectiveRuleSnapshotLoader _loader;
    /// <summary>
    /// 初始化运行期生效规则快照缓存。
    /// </summary>
    /// <param name="cache">进程内缓存。</param>
    /// <param name="loader">快照加载器。</param>
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
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>候选规则快照集合。</returns>
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
    /// <returns>实际尝试清除的缓存键数量。</returns>
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
