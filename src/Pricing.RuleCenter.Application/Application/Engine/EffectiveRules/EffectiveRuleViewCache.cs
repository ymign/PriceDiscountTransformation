using Microsoft.Extensions.Caching.Memory;

namespace Pricing.RuleCenter.Application.Engine.EffectiveRules;

/// <summary>
/// 当前生效规则视图缓存。
/// </summary>
/// <remarks>
/// 规则匹配是计价接口的高频路径。该缓存按项目编码缓存候选规则视图，
/// 通过发布、停用和回滚后的统一失效机制保证不会长期保留旧规则结果。
/// </remarks>
public sealed class EffectiveRuleViewCache : IEffectiveRuleViewCache
{
    /// <summary>
    /// 缓存键前缀。
    /// </summary>
    private const string CacheKeyPrefix = "runtime:effective-rule-view:";

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
    /// 当前规则读取器。
    /// </summary>
    private readonly EffectiveRuleReader _reader;

    /// <summary>
    /// 初始化当前生效规则视图缓存。
    /// </summary>
    /// <param name="cache">进程内缓存。</param>
    /// <param name="reader">当前规则读取器。</param>
    public EffectiveRuleViewCache(
        IMemoryCache cache,
        EffectiveRuleReader reader)
    {
        _cache = cache;
        _reader = reader;
    }

    /// <summary>
    /// 获取项目运行期候选规则视图。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>候选规则视图集合。</returns>
    public async Task<IReadOnlyList<EffectiveRuleView>> GetByItemCodeAsync(string itemCode)
    {
        var key = BuildKey(itemCode);
        if (_cache.TryGetValue(key, out IReadOnlyList<EffectiveRuleView>? rules) &&
            rules is not null)
        {
            return rules;
        }

        rules = await _reader.LoadByItemCodeAsync(itemCode);
        _cache.Set(key, rules, TimeSpan.FromMinutes(10));
        lock (KeysLock)
        {
            Keys.Add(key);
        }

        return rules;
    }

    /// <summary>
    /// 清除全部当前规则视图缓存。
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
