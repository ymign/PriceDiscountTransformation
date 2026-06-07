using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class CacheVersionSynchronizerTests
{
    [Fact]
    public async Task IncreaseVersionAsync_ShouldPersistAndRememberLatestVersion()
    {
        var repository = new InMemoryCacheVersionRepository();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var runtimeCache = new CapturingRuleRuntimeCacheInvalidator();
        var synchronizer = new CacheVersionSynchronizer(
            repository,
            cache,
            runtimeCache,
            new CacheVersionLocalState(),
            NullLogger<CacheVersionSynchronizer>.Instance);

        var version = await synchronizer.IncreaseVersionAsync(CacheVersionSynchronizer.ActionTypeOrderScope);

        Assert.Equal(1L, version);
        Assert.Equal(1L, repository.Items.Single().VersionNo);
        Assert.Equal(CacheVersionSynchronizer.ActionTypeOrderScope, repository.Items.Single().CacheScope);
    }

    [Fact]
    public async Task SyncAsync_ShouldClearRuleAndRuntimeCacheWhenDatabaseVersionChanges()
    {
        var repository = new InMemoryCacheVersionRepository();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var runtimeCache = new CapturingRuleRuntimeCacheInvalidator();
        var synchronizer = new CacheVersionSynchronizer(
            repository,
            cache,
            runtimeCache,
            new CacheVersionLocalState(),
            NullLogger<CacheVersionSynchronizer>.Instance);

        repository.Items.Add(new CacheVersion
        {
            CacheScope = CacheVersionSynchronizer.EffectiveRulesScope,
            VersionNo = 1,
            UpdatedAt = DateTime.Now
        });
        repository.Items.Add(new CacheVersion
        {
            CacheScope = CacheVersionSynchronizer.ActionTypeOrderScope,
            VersionNo = 1,
            UpdatedAt = DateTime.Now
        });

        await synchronizer.SyncAsync();

        cache.Set("rules:effective:all:current", Array.Empty<object>());
        repository.Items.Single(i => i.CacheScope == CacheVersionSynchronizer.EffectiveRulesScope).VersionNo = 2;
        repository.Items.Single(i => i.CacheScope == CacheVersionSynchronizer.ActionTypeOrderScope).VersionNo = 2;

        await synchronizer.SyncAsync();

        Assert.Equal(2, runtimeCache.ClearCount);
        Assert.Equal(2L, repository.Items.Single(i => i.CacheScope == CacheVersionSynchronizer.EffectiveRulesScope).VersionNo);
        Assert.Equal(2L, repository.Items.Single(i => i.CacheScope == CacheVersionSynchronizer.ActionTypeOrderScope).VersionNo);
    }

    [Fact]
    public async Task SyncAsync_ShouldInvalidateOtherInstanceWhenSharedVersionChanges()
    {
        var repository = new InMemoryCacheVersionRepository();
        repository.Items.Add(new CacheVersion
        {
            CacheScope = CacheVersionSynchronizer.EffectiveRulesScope,
            VersionNo = 1,
            UpdatedAt = DateTime.Now
        });
        repository.Items.Add(new CacheVersion
        {
            CacheScope = CacheVersionSynchronizer.ActionTypeOrderScope,
            VersionNo = 1,
            UpdatedAt = DateTime.Now
        });

        using var cacheA = new MemoryCache(new MemoryCacheOptions());
        using var cacheB = new MemoryCache(new MemoryCacheOptions());
        var runtimeA = new CapturingRuleRuntimeCacheInvalidator();
        var runtimeB = new CapturingRuleRuntimeCacheInvalidator();
        var instanceA = new CacheVersionSynchronizer(
            repository,
            cacheA,
            runtimeA,
            new CacheVersionLocalState(),
            NullLogger<CacheVersionSynchronizer>.Instance);
        var instanceB = new CacheVersionSynchronizer(
            repository,
            cacheB,
            runtimeB,
            new CacheVersionLocalState(),
            NullLogger<CacheVersionSynchronizer>.Instance);

        await instanceA.SyncAsync();
        await instanceB.SyncAsync();
        runtimeA.Reset();
        runtimeB.Reset();

        var trackedKey = "rules:effective:item:ITEM001";
        TrackEffectiveRuleCacheKey(trackedKey);
        cacheB.Set(trackedKey, new object());

        await instanceA.IncreaseVersionAsync(CacheVersionSynchronizer.EffectiveRulesScope);
        await instanceA.IncreaseVersionAsync(CacheVersionSynchronizer.ActionTypeOrderScope);

        await instanceB.SyncAsync();

        Assert.False(cacheB.TryGetValue(trackedKey, out _));
        Assert.Equal(1, runtimeB.ClearCount);
        Assert.Equal(0, runtimeA.ClearCount);
    }

    private static void TrackEffectiveRuleCacheKey(string cacheKey)
    {
        var assembly = typeof(CacheVersionSynchronizer).Assembly;
        var type = assembly.GetType("Pricing.RuleCenter.Application.Rules.EffectiveRuleCacheKeys", throwOnError: true)!;
        var method = type.GetMethod("Track", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)!;
        method.Invoke(null, new object[] { cacheKey });
    }

    private sealed class CapturingRuleRuntimeCacheInvalidator : IRuleRuntimeCacheInvalidator
    {
        public int ClearCount { get; private set; }

        public void ClearRuntimeCache()
        {
            ClearCount++;
        }

        public void Reset()
        {
            ClearCount = 0;
        }
    }

    private sealed class InMemoryCacheVersionRepository : ICacheVersionRepository
    {
        public List<CacheVersion> Items { get; } = new();

        public Task<CacheVersion?> GetByScopeAsync(string cacheScope) =>
            Task.FromResult(Items.SingleOrDefault(i =>
                string.Equals(i.CacheScope, cacheScope, StringComparison.OrdinalIgnoreCase)));

        public Task<long> IncreaseVersionAsync(string cacheScope)
        {
            var normalizedScope = cacheScope.Trim().ToUpperInvariant();
            var item = Items.SingleOrDefault(i => i.CacheScope == normalizedScope);
            if (item is null)
            {
                item = new CacheVersion
                {
                    CacheScope = normalizedScope,
                    VersionNo = 1,
                    UpdatedAt = DateTime.Now
                };
                Items.Add(item);
            }
            else
            {
                item.VersionNo++;
                item.UpdatedAt = DateTime.Now;
            }

            return Task.FromResult(item.VersionNo);
        }
    }
}


