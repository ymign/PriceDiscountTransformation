using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IRulePublishRepository
{
    Task<IReadOnlyList<RulePublish>> GetByRuleIdAsync(long ruleId);
    Task<long> InsertAsync(RulePublish entity);
}
