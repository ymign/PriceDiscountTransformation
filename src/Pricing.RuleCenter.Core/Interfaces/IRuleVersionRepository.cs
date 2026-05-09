using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IRuleVersionRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IRuleVersionRepository
{
    Task<RuleVersion?> GetByIdAsync(long versionId);
    Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo);
    Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId);
    Task<long> InsertAsync(RuleVersion entity);
    Task<bool> UpdateStatusAsync(long versionId, string status);
}
