using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IRuleVersionRepository
{
    Task<RuleVersion?> GetByIdAsync(long versionId);
    Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo);
    Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId);
    Task<long> InsertAsync(RuleVersion entity);
    Task<bool> UpdateStatusAsync(long versionId, string status);
}
