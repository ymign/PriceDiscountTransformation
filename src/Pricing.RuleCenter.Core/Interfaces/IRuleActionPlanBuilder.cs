using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 规则动作执行计划构建器抽象。
/// </summary>
public interface IRuleActionPlanBuilder
{
    /// <summary>
    /// 构建当前命中规则的可执行动作链。
    /// </summary>
    /// <param name="actions">命中规则下的全部候选动作。</param>
    /// <param name="matchedRules">已命中的规则集合。</param>
    /// <returns>按全局口径排序后的动作集合。</returns>
    Task<IReadOnlyList<RuleAction>> BuildAsync(
        IReadOnlyList<RuleAction> actions,
        IReadOnlyList<RuleAggregate> matchedRules);

    /// <summary>
    /// 清理内部动作顺序缓存。
    /// </summary>
    void ClearCache();
}
