using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IRuleConditionRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IRuleConditionRepository
{
    Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo);
    Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities);
    Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo);
}
