using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PricingWorkflowSupportTests
{
    [Fact]
    public async Task SpecialFlagResolver_UsesMostConservativeRollbackModeWithinEffectivePublishedRules()
    {
        var resolver = new PricingSpecialFlagResolver(
            new StubRuleHeaderRepository(new[]
            {
                new RuleAggregate
                {
                    RuleId = 1,
                    ItemCode = "ITEM001",
                    Status = RuleStatusCodes.Published,
                    IsEnabled = EnableFlag.Yes,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    RollbackMode = "LEGACY_EQUIVALENT"
                },
                new RuleAggregate
                {
                    RuleId = 2,
                    ItemCode = "ITEM001",
                    Status = RuleStatusCodes.Published,
                    IsEnabled = EnableFlag.Yes,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    RollbackMode = "STOP_CHARGE"
                },
                new RuleAggregate
                {
                    RuleId = 3,
                    ItemCode = "ITEM001",
                    Status = RuleStatusCodes.Published,
                    IsEnabled = EnableFlag.Yes,
                    EffectiveFrom = new DateTime(2027, 1, 1),
                    EffectiveTo = new DateTime(2027, 12, 31),
                    RollbackMode = "MANUAL_REVIEW"
                }
            }),
            new FixedClock(new DateTime(2026, 5, 10, 10, 0, 0)));

        var result = await resolver.ResolveAsync(" ITEM001 ");

        Assert.True(result.IsSpecial);
        Assert.Equal(2, result.RuleCount);
        Assert.Equal("STOP_CHARGE", result.RollbackMode);
    }

    private sealed class StubRuleHeaderRepository : IRuleHeaderRepository
    {
        private readonly IReadOnlyList<RuleAggregate> _items;

        public StubRuleHeaderRepository(IReadOnlyList<RuleAggregate> items)
        {
            _items = items;
        }

        public Task<RuleAggregate?> GetByIdAsync(long ruleId) => Task.FromResult<RuleAggregate?>(null);
        public Task<RuleAggregate?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult<RuleAggregate?>(null);
        public Task<RuleAggregate?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleAggregate?>(null);
        public Task<IReadOnlyList<RuleAggregate>> GetByItemCodeAsync(string itemCode) =>
            Task.FromResult((IReadOnlyList<RuleAggregate>)_items.Where(item => item.ItemCode == itemCode.Trim()).ToList());
        public Task<(IReadOnlyList<RuleAggregate> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) =>
            Task.FromResult(((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>(), 0));
        public Task<IReadOnlyList<RuleAggregate>> GetEffectiveAsync(DateTime businessTime) =>
            Task.FromResult((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>());
        public Task<long> InsertAsync(RuleAggregate entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleAggregate entity, string? expectedCurrentStatus = null) => Task.FromResult(true);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }
}
