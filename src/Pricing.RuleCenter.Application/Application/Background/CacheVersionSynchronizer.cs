using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;

namespace Pricing.RuleCenter.Application.Background;

/// <summary>
/// 缓存版本同步器。
/// </summary>
/// <remarks>
/// 通过 Oracle 中共享的缓存版本号判断本机缓存是否过期。
/// 检测到版本变化时，负责清理本机规则缓存和运行期动作顺序缓存。
/// </remarks>
public sealed class CacheVersionSynchronizer : ICacheVersionSynchronizer
{
    public const string EffectiveRulesScope = "EFFECTIVE_RULES";
    public const string ActionTypeOrderScope = "ACTION_TYPE_ORDER";

    private static readonly ConcurrentDictionary<string, long> LocalVersions = new(StringComparer.OrdinalIgnoreCase);

    private readonly ICacheVersionRepository _cacheVersionRepository;
    private readonly IMemoryCache _cache;
    private readonly IRuleRuntimeCacheInvalidator _runtimeCacheInvalidator;
    private readonly ILogger<CacheVersionSynchronizer> _logger;

    public CacheVersionSynchronizer(
        ICacheVersionRepository cacheVersionRepository,
        IMemoryCache cache,
        IRuleRuntimeCacheInvalidator runtimeCacheInvalidator,
        ILogger<CacheVersionSynchronizer> logger)
    {
        _cacheVersionRepository = cacheVersionRepository;
        _cache = cache;
        _runtimeCacheInvalidator = runtimeCacheInvalidator;
        _logger = logger;
    }

    public async Task SyncAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await SyncScopeAsync(EffectiveRulesScope, invalidateRules: true, invalidateRuntime: false, cancellationToken);
        await SyncScopeAsync(ActionTypeOrderScope, invalidateRules: false, invalidateRuntime: true, cancellationToken);
    }

    public async Task<long> IncreaseVersionAsync(string cacheScope, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var version = await _cacheVersionRepository.IncreaseVersionAsync(cacheScope);
        LocalVersions[cacheScope] = version;
        return version;
    }

    private async Task SyncScopeAsync(
        string cacheScope,
        bool invalidateRules,
        bool invalidateRuntime,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var current = await _cacheVersionRepository.GetByScopeAsync(cacheScope);
        var remoteVersion = current?.VersionNo ?? 0L;
        var localVersion = LocalVersions.TryGetValue(cacheScope, out var version) ? version : 0L;
        if (remoteVersion <= localVersion)
        {
            return;
        }

        if (invalidateRules)
        {
            var removed = EffectiveRuleCacheKeys.Clear(_cache);
            _logger.LogInformation("检测到缓存版本变化，已清理生效规则缓存 Scope={Scope}, Removed={Removed}", cacheScope, removed);
        }

        if (invalidateRuntime)
        {
            _runtimeCacheInvalidator.ClearRuntimeCache();
            _logger.LogInformation("检测到缓存版本变化，已清理运行期规则缓存 Scope={Scope}", cacheScope);
        }

        LocalVersions[cacheScope] = remoteVersion;
    }
}


