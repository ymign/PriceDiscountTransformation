using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IRuleConditionRepository
{
    Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo);
    Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities);
    Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo);
}
