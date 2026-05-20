using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Api.Application.Rules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuleHeaderServiceTests
{
    [Fact]
    public async Task GetEffectiveAsync_UsesRequestedChargeTimeAndCachesByItemAndTime()
    {
        var repository = new CapturingRuleHeaderRepository();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            repository,
            new EmptyRuleChangeLogRepository(),
            cache,
            NullLogger<RuleHeaderService>.Instance);

        var firstTime = new DateTime(2026, 5, 10, 9, 0, 0);
        var secondTime = new DateTime(2026, 5, 11, 9, 0, 0);

        await service.GetEffectiveAsync("ITEM001", firstTime);
        await service.GetEffectiveAsync("ITEM001", secondTime);

        Assert.Equal(new[] { firstTime, secondTime }, repository.BusinessTimes);
    }

    [Fact]
    public async Task ClearEffectiveCache_RemovesItemSpecificCachedRules()
    {
        var repository = new CapturingRuleHeaderRepository();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            repository,
            new EmptyRuleChangeLogRepository(),
            cache,
            NullLogger<RuleHeaderService>.Instance);

        await service.GetEffectiveAsync("ITEM001", new DateTime(2026, 5, 10, 9, 0, 0));
        service.ClearEffectiveCache();
        await service.GetEffectiveAsync("ITEM001", new DateTime(2026, 5, 10, 9, 0, 0));

        Assert.Equal(2, repository.BusinessTimes.Count);
    }

    private sealed class CapturingRuleHeaderRepository : IRuleHeaderRepository
    {
        public List<DateTime> BusinessTimes { get; } = new();

        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(null);
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));

        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime)
        {
            BusinessTimes.Add(businessTime);
            return Task.FromResult((IReadOnlyList<RuleHeader>)new[]
            {
                new RuleHeader
                {
                    RuleId = businessTime.Day,
                    RuleCode = $"RULE-{businessTime:yyyyMMdd}",
                    RuleName = "测试规则",
                    RuleCategory = "MIXED",
                    RuleScope = "ITEM",
                    ItemCode = "ITEM001",
                    Priority = 10,
                    CurrentVersion = 1,
                    Status = "PUBLISHED",
                    IsEnabled = "Y",
                    CreatedAt = businessTime,
                    UpdatedAt = businessTime
                }
            });
        }

        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleHeader entity) => Task.FromResult(false);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }

    private sealed class EmptyRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());
        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
    }
}
