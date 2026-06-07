using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PolicyPriorityKeyFactoryTests
{
    private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

    [Fact]
    public void Build_ItemBindingRanksHigherThanGroupBinding()
    {
        var factory = new PolicyPriorityKeyFactory();
        var version = new PolicyVersion
        {
            PolicyVersionId = 1,
            VersionNo = 1,
            ScopeLevel = "HOSPITAL",
            PriorityWeight = 100
        };

        var itemKey = factory.Build(
            version,
            new PolicyBinding { ItemCode = "ITEM001" },
            Array.Empty<PolicyScope>());
        var groupKey = factory.Build(
            version,
            new PolicyBinding { GroupCode = "GROUP001" },
            Array.Empty<PolicyScope>());

        Assert.True(Comparer.Compare(itemKey, groupKey) < 0);
    }

    [Fact]
    public void Build_MoreSpecificScopesRankHigher()
    {
        var factory = new PolicyPriorityKeyFactory();
        var version = new PolicyVersion
        {
            PolicyVersionId = 2,
            VersionNo = 1,
            ScopeLevel = "SCENE",
            PriorityWeight = 100
        };
        var binding = new PolicyBinding { ItemCode = "ITEM001" };

        var specificKey = factory.Build(
            version,
            binding,
            new[]
            {
                new PolicyScope { ScopeDimension = "SCENE", ScopeValueText = "OUTPATIENT" },
                new PolicyScope { ScopeDimension = "BODY_PART", ScopeValueText = "SKIN" }
            });
        var broadKey = factory.Build(
            version,
            binding,
            new[]
            {
                new PolicyScope { ScopeDimension = "SCENE", ScopeValueText = "OUTPATIENT" }
            });

        Assert.True(Comparer.Compare(specificKey, broadKey) < 0);
    }

    [Fact]
    public void Build_LowerManualWeightRanksHigher()
    {
        var factory = new PolicyPriorityKeyFactory();
        var binding = new PolicyBinding { ItemCode = "ITEM001" };
        var scopes = new[] { new PolicyScope { ScopeDimension = "SCENE", ScopeValueText = "OUTPATIENT" } };

        var lowerWeight = factory.Build(
            new PolicyVersion { PolicyVersionId = 3, VersionNo = 1, ScopeLevel = "SCENE", PriorityWeight = 10 },
            binding,
            scopes);
        var higherWeight = factory.Build(
            new PolicyVersion { PolicyVersionId = 4, VersionNo = 1, ScopeLevel = "SCENE", PriorityWeight = 50 },
            binding,
            scopes);

        Assert.True(Comparer.Compare(lowerWeight, higherWeight) < 0);
    }
}
