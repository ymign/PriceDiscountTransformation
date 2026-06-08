using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Application.RuntimePackages;

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
    private readonly RuntimePackageTraceContextAccessor? _traceContextAccessor;

    /// <summary>
    /// 初始化运行期生效规则快照缓存。
    /// </summary>
    public EffectiveRuleSnapshotCache(
        IMemoryCache cache,
        EffectiveRuleSnapshotLoader loader,
        RuntimePackageTraceContextAccessor? traceContextAccessor = null)
    {
        _cache = cache;
        _loader = loader;
        _traceContextAccessor = traceContextAccessor;
    }

    /// <summary>
    /// 获取项目运行期候选规则快照。
    /// </summary>
    public async Task<IReadOnlyList<EffectiveRuleSnapshot>> GetByItemCodeAsync(string itemCode)
    {
        var key = BuildKeyForCurrentPackage(itemCode);
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

    private string BuildKeyForCurrentPackage(string itemCode)
    {
        var context = _traceContextAccessor?.Current;
        if (context is null)
        {
            return BuildKey(itemCode);
        }

        return CacheKeyPrefix
            + "pkg:"
            + (context.ActivePackageId?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "none")
            + ":"
            + (context.ActivePackageVersion?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "none")
            + ":"
            + itemCode.Trim().ToUpperInvariant();
    }
}
