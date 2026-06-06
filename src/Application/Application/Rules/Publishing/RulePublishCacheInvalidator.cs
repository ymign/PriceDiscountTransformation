using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Pricing.Queries;
using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则发布链路缓存失效器。
/// </summary>
public sealed class RulePublishCacheInvalidator
{
    private readonly IMemoryCache _cache;
    private readonly IRuleRuntimeCacheInvalidator _runtimeCacheInvalidator;
    private readonly ILogger<RulePublishCacheInvalidator> _logger;

    /// <summary>
    /// 初始化规则发布缓存失效器。
    /// </summary>
    public RulePublishCacheInvalidator(
        IMemoryCache cache,
        IRuleRuntimeCacheInvalidator runtimeCacheInvalidator,
        ILogger<RulePublishCacheInvalidator> logger)
    {
        _cache = cache;
        _runtimeCacheInvalidator = runtimeCacheInvalidator;
        _logger = logger;
    }

    /// <summary>
    /// 清除发布、停用和回滚后必须失效的运行期缓存。
    /// </summary>
    public void ClearEffectiveCache()
    {
        var effectiveRuleRemoved = EffectiveRuleCacheKeys.Clear(_cache);
        var specialFlagRemoved = SpecialFlagCacheKeys.Clear(_cache);
        _runtimeCacheInvalidator.ClearRuntimeCache();
        _logger.LogDebug(
            "已清除生效规则缓存、特殊项目标识缓存和规则运行期缓存，生效规则缓存键 {EffectiveRuleCount} 个，特殊项目缓存键 {SpecialFlagCount} 个",
            effectiveRuleRemoved,
            specialFlagRemoved);
    }
}
