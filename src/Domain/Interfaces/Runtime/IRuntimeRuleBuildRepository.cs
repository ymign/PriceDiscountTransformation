using Pricing.RuleCenter.Core.Aggregates.Runtime;

namespace Pricing.RuleCenter.Core.Interfaces.Runtime;

public interface IRuntimeRuleBuildRepository
{
    Task<IReadOnlyList<long>> ReservePackagePolicyIdsAsync(int count);

    Task<IReadOnlyList<long>> ReserveRuleIdsAsync(int count);

    Task<IReadOnlyList<long>> ReserveConditionIdsAsync(int count);

    Task<IReadOnlyList<long>> ReserveActionIdsAsync(int count);

    Task InsertPackagePoliciesAsync(IReadOnlyList<RuntimePackagePolicy> packagePolicies);

    Task InsertRulesAsync(IReadOnlyList<RuntimeRule> rules);

    Task InsertConditionsAsync(IReadOnlyList<RuntimeCondition> conditions);

    Task InsertActionsAsync(IReadOnlyList<RuntimeAction> actions);
}
