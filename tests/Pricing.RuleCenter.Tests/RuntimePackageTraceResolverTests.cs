using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuntimePackageTraceResolverTests
{
    [Fact]
    public async Task ResolveAsync_ReturnsActivePackageAndMatchedRuntimeRules()
    {
        var resolver = new RuntimePackageTraceResolver(
            new FixedRuntimePackageStateRepository(new RuntimePackageState
            {
                StateCode = RuntimePackageStateCodes.Active,
                ActivePackageId = 77,
                ActivePackageVersion = 5
            }),
            new FixedRuntimeRuleReadRepository(new RuntimeRule
            {
                RuntimeRuleId = 501,
                PackageId = 77,
                SourcePolicyVersionId = 9001,
                SourceTemplateVersionId = 8001
            }));

        var resolution = await resolver.ResolveAsync(new[] { 501L });

        Assert.Equal(77, resolution.RuntimePackageId);
        Assert.Equal(5, resolution.RuntimePackageVersion);
        var rule = Assert.Single(resolution.RuntimeRulesById.Values);
        Assert.Equal(9001, rule.SourcePolicyVersionId);
        Assert.Equal(8001, rule.SourceTemplateVersionId);
    }

    private sealed class FixedRuntimePackageStateRepository : IRuntimePackageStateRepository
    {
        private readonly RuntimePackageState _state;

        public FixedRuntimePackageStateRepository(RuntimePackageState state)
        {
            _state = state;
        }

        public Task<RuntimePackageState?> GetActiveAsync() => Task.FromResult<RuntimePackageState?>(_state);
        public Task<RuntimePackageState?> GetActiveForUpdateAsync() => Task.FromResult<RuntimePackageState?>(_state);
        public Task UpsertAsync(RuntimePackageState entity) => Task.CompletedTask;
    }

    private sealed class FixedRuntimeRuleReadRepository : IRuntimeRuleReadRepository
    {
        private readonly RuntimeRule _rule;

        public FixedRuntimeRuleReadRepository(RuntimeRule rule)
        {
            _rule = rule;
        }

        public Task<IReadOnlyList<RuntimeRule>> GetRulesByItemCodeAsync(long packageId, string itemCode) =>
            Task.FromResult((IReadOnlyList<RuntimeRule>)new[] { _rule });

        public Task<IReadOnlyList<RuntimeRule>> GetRulesByIdsAsync(IReadOnlyCollection<long> runtimeRuleIds) =>
            Task.FromResult((IReadOnlyList<RuntimeRule>)(runtimeRuleIds.Contains(_rule.RuntimeRuleId)
                ? new[] { _rule }
                : Array.Empty<RuntimeRule>()));

        public Task<IReadOnlyDictionary<long, IReadOnlyList<RuntimeCondition>>> GetConditionsByRuleIdsAsync(IReadOnlyCollection<long> runtimeRuleIds) =>
            Task.FromResult((IReadOnlyDictionary<long, IReadOnlyList<RuntimeCondition>>)new Dictionary<long, IReadOnlyList<RuntimeCondition>>());

        public Task<IReadOnlyDictionary<long, IReadOnlyList<RuntimeAction>>> GetActionsByRuleIdsAsync(IReadOnlyCollection<long> runtimeRuleIds) =>
            Task.FromResult((IReadOnlyDictionary<long, IReadOnlyList<RuntimeAction>>)new Dictionary<long, IReadOnlyList<RuntimeAction>>());
    }
}
