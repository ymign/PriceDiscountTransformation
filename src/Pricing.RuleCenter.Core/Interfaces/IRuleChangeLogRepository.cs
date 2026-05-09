using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IRuleChangeLogRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IRuleChangeLogRepository
{
    Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId);
    Task<long> InsertAsync(RuleChangeLog entity);
}
