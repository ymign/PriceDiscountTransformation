using Pricing.RuleCenter.Core.Aggregates.Runtime;

namespace Pricing.RuleCenter.Core.Interfaces.Runtime;

public interface IRuntimeRuleBuildRepository
{
    Task InsertPackagePoliciesAsync(IReadOnlyList<RuntimePackagePolicy> packagePolicies);

    Task InsertRulesAsync(IReadOnlyList<RuntimeRule> rules);

    Task InsertConditionsAsync(IReadOnlyList<RuntimeCondition> conditions);

    Task InsertActionsAsync(IReadOnlyList<RuntimeAction> actions);
}
