using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IRulePublishRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IRulePublishRepository
{
    Task<IReadOnlyList<RulePublish>> GetByRuleIdAsync(long ruleId);
    Task<long> InsertAsync(RulePublish entity);
}
