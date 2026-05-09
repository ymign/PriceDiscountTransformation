using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IRuleConditionEvaluator 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IRuleConditionEvaluator
{
    string ConditionType { get; }
    bool Evaluate(RuleCondition condition, PricingContext context);
}
