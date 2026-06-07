using Pricing.RuleCenter.Core.Aggregates.Runtime;

namespace Pricing.RuleCenter.Core.Interfaces.Runtime;

public interface IRuntimeRuleReadRepository
{
    Task<IReadOnlyList<RuntimeRule>> GetRulesByItemCodeAsync(long packageId, string itemCode);

    Task<IReadOnlyDictionary<long, IReadOnlyList<RuntimeCondition>>> GetConditionsByRuleIdsAsync(IReadOnlyCollection<long> runtimeRuleIds);

    Task<IReadOnlyDictionary<long, IReadOnlyList<RuntimeAction>>> GetActionsByRuleIdsAsync(IReadOnlyCollection<long> runtimeRuleIds);
}
