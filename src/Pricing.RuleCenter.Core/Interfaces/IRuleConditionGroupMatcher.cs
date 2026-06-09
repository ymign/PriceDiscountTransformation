using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则条件组匹配器抽象。
/// </summary>
public interface IRuleConditionGroupMatcher
{
    /// <summary>
    /// 按条件组口径评估规则是否命中当前计价上下文。
    /// </summary>
    /// <param name="conditions">规则条件集合。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>命中返回 <c>true</c>。</returns>
    Task<bool> EvaluateAsync(IReadOnlyList<RuleCondition> conditions, PricingContext context);
}
