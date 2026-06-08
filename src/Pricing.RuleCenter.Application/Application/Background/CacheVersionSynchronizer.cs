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
    /// <summary>
    /// 生效规则缓存版本作用域。
    /// </summary>
    public const string EffectiveRulesScope = "EFFECTIVE_RULES";

    /// <summary>
    /// 动作类型执行顺序缓存版本作用域。
    /// </summary>
    public const string ActionTypeOrderScope = "ACTION_TYPE_ORDER";

    private readonly ICacheVersionRepository _cacheVersionRepository;
    private readonly IMemoryCache _cache;
    private readonly IRuleRuntimeCacheInvalidator _runtimeCacheInvalidator;
    private readonly CacheVersionLocalState _localState;
    private readonly ILogger<CacheVersionSynchronizer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheVersionSynchronizer"/> class.
    /// </summary>
    /// <param name="cacheVersionRepository">缓存版本仓储。</param>
    /// <param name="cache">本机内存缓存。</param>
    /// <param name="runtimeCacheInvalidator">运行期规则缓存失效器。</param>
    /// <param name="localState">当前实例的本地版本状态。</param>
    /// <param name="logger">日志对象。</param>
    public CacheVersionSynchronizer(
        ICacheVersionRepository cacheVersionRepository,
        IMemoryCache cache,
        IRuleRuntimeCacheInvalidator runtimeCacheInvalidator,
        CacheVersionLocalState localState,
        ILogger<CacheVersionSynchronizer> logger)
    {
        _cacheVersionRepository = cacheVersionRepository;
        _cache = cache;
        _runtimeCacheInvalidator = runtimeCacheInvalidator;
        _localState = localState;
        _logger = logger;
    }

    /// <summary>
    /// 同步所有已知缓存作用域的远端版本，并在版本变化时清理本机缓存。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步同步操作的任务。</returns>
    public async Task SyncAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await SyncScopeAsync(EffectiveRulesScope, invalidateRules: true, invalidateRuntime: false, cancellationToken);
        await SyncScopeAsync(ActionTypeOrderScope, invalidateRules: false, invalidateRuntime: true, cancellationToken);
    }

    /// <summary>
    /// 递增指定缓存作用域的共享版本号。
    /// </summary>
    /// <param name="cacheScope">缓存作用域编码。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>递增后的版本号。</returns>
    public async Task<long> IncreaseVersionAsync(string cacheScope, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var version = await _cacheVersionRepository.IncreaseVersionAsync(cacheScope);
        _localState.SetVersion(cacheScope, version);
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
        var localVersion = _localState.GetVersion(cacheScope);
        if (remoteVersion <= localVersion)
        {
            return;
        }

        if (invalidateRules)
        {
            var removed = EffectiveRuleCacheKeys.Clear(_cache);
            _logger.LogInformation("检测到缓存版本变化，已清理生效规则缓存 范围={Scope}, 清理数量={Removed}", cacheScope, removed);
        }

        if (invalidateRuntime)
        {
            _runtimeCacheInvalidator.ClearRuntimeCache();
            _logger.LogInformation("检测到缓存版本变化，已清理运行期规则缓存 范围={Scope}", cacheScope);
        }

        _localState.SetVersion(cacheScope, remoteVersion);
    }
}


