using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Application.RuntimePackages;

namespace Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;

/// <summary>
/// 运行期生效规则快照缓存。
/// </summary>
/// <remarks>
/// 规则匹配是计价接口的高频路径。该缓存按项目编码和当前运行包版本缓存候选规则快照，
/// 既减少数据库读取，也避免同一请求在运行包切换时混读不同版本。
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
    /// 当前计价请求捕获的运行包上下文访问器。
    /// </summary>
    private readonly RuntimePackageTraceContextAccessor? _traceContextAccessor;

    /// <summary>
    /// 初始化运行期生效规则快照缓存。
    /// </summary>
    /// <param name="cache">进程内缓存。</param>
    /// <param name="loader">快照加载器。</param>
    /// <param name="traceContextAccessor">当前请求运行包上下文访问器。</param>
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
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>候选规则快照集合。</returns>
    public async Task<IReadOnlyList<EffectiveRuleSnapshot>> GetByItemCodeAsync(string itemCode)
    {
        // 缓存键包含当前运行包指针。confirm/simulate workflow 会先捕获运行包上下文，
        // 后续同一请求内多条明细都使用该上下文生成缓存键，避免一单多版本。
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

    private string BuildKeyForCurrentPackage(string itemCode)
    {
        var context = _traceContextAccessor?.Current;
        if (context is null)
        {
            // 没有运行包上下文时退化为仅按项目缓存，主要用于旧规则模型或单元测试。
            return BuildKey(itemCode);
        }

        // activePackageId/version 进入缓存键，确保发布新运行包后不会复用旧版本候选规则。
        return CacheKeyPrefix
            + "pkg:"
            + (context.ActivePackageId?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "none")
            + ":"
            + (context.ActivePackageVersion?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "none")
            + ":"
            + itemCode.Trim().ToUpperInvariant();
    }
}
