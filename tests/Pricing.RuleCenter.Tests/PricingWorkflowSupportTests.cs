using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
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
    public void PricingContextFactory_CreatesSpecialFlagContextFromSharedMapping()
    {
        var businessTime = new DateTime(2026, 5, 10, 9, 30, 0);

        var context = PricingContextFactory.Create(new SpecialFlagPricingContextBuildInput
        {
            ItemCode = " ITEM001 ",
            BusinessTime = businessTime,
            Request = new SpecialFlagRequest
            {
                ItemCode = "ITEM001",
                ItemGroupCode = " GROUP_A ",
                InputQty = 2m,
                Unit = " PART ",
                UnitPrice = 100m,
                ChargeScene = " OUTPATIENT ",
                BusinessChargeTime = businessTime,
                VisitType = " OUTPATIENT ",
                BodyPartCode = " HEAD ",
                ChargeDeptCode = " 998 ",
                ExtraParams = new Dictionary<string, object?>
                {
                    ["diagnosisCodes"] = "A01,B02"
                },
                PricingParts = new[]
                {
                    new PricingPartItemRequest
                    {
                        PartSeq = 1,
                        PartCode = " P1 ",
                        PartName = " 部位1 ",
                        BodyPartCode = " HEAD ",
                        Qty = 2m,
                        Area = 3m,
                        MeasureType = " AREA ",
                        MeasureValue = 3m,
                        MeasureUnit = " CM2 ",
                        LesionCount = 1
                    }
                }
            }
        });

        Assert.Equal("SPECIAL_FLAG", context.CallType);
        Assert.False(context.ShouldLockLimits);
        Assert.Equal("ITEM001", context.ItemCode);
        Assert.Equal("GROUP_A", context.ItemGroupCode);
        Assert.Equal(2m, context.InputQty);
        Assert.Equal(2m, context.ConvertedQty);
        Assert.Equal(2m, context.FinalQty);
        Assert.Equal("PART", context.Unit);
        Assert.Equal(100m, context.UnitPrice);
        Assert.Equal(200m, context.FinalAmount);
        Assert.Equal("OUTPATIENT", context.ChargeScene);
        Assert.Equal("OUTPATIENT", context.VisitType);
        Assert.Equal("HEAD", context.BodyPartCode);
        Assert.Equal("998", context.ChargeDeptCode);
        Assert.Equal(businessTime, context.BusinessChargeTime);
        Assert.Equal("A01,B02", Assert.Contains("diagnosisCodes", context.ExtraParams!));

        var part = Assert.Single(context.PricingParts!);
        Assert.Equal("P1", part.PartCode);
        Assert.Equal("部位1", part.PartName);
        Assert.Equal("HEAD", part.BodyPartCode);
        Assert.Equal(2m, part.Qty);
        Assert.Equal(3m, part.Area);
        Assert.Equal("AREA", part.MeasureType);
        Assert.Equal(3m, part.MeasureValue);
        Assert.Equal("CM2", part.MeasureUnit);
        Assert.Equal(1, part.LesionCount);
    }

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
        public Task<IReadOnlyDictionary<string, IReadOnlyList<RuleAggregate>>> GetByItemCodesAsync(
            IReadOnlyCollection<string> itemCodes)
        {
            var result = itemCodes.ToDictionary(
                itemCode => itemCode,
                itemCode => (IReadOnlyList<RuleAggregate>)_items
                    .Where(item => string.Equals(item.ItemCode, itemCode.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList(),
                StringComparer.OrdinalIgnoreCase);
            return Task.FromResult((IReadOnlyDictionary<string, IReadOnlyList<RuleAggregate>>)result);
        }
        public Task<(IReadOnlyList<RuleAggregate> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) =>
            Task.FromResult(((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>(), 0));
        public Task<IReadOnlyList<RuleAggregate>> GetEffectiveAsync(DateTime businessTime) =>
            Task.FromResult((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>());
        public Task<long> InsertAsync(RuleAggregate entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleAggregate entity, string? expectedCurrentStatus = null) => Task.FromResult(true);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }
}
