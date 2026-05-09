using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IRuleActionRepository
{
    Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo);
    Task InsertBatchAsync(IReadOnlyList<RuleAction> entities);
    Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo);
}
