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

    private sealed class CapturingRuleRuntimeCacheInvalidator : IRuleRuntimeCacheInvalidator
    {
        public int ClearCount { get; private set; }

        public void ClearRuntimeCache()
        {
            ClearCount++;
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


