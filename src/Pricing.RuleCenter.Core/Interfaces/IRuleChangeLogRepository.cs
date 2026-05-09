using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IRuleChangeLogRepository
{
    Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId);
    Task<long> InsertAsync(RuleChangeLog entity);
}
