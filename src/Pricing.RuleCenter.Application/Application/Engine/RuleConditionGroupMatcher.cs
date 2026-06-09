using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 规则条件组匹配器。
/// </summary>
/// <remarks>
/// <para>
/// 条件组语义为“组内 AND，组间 OR”：同一个 ConditionGroup 下的条件必须全部满足；
/// 只要任意一个条件组全部满足，整条规则即视为命中。
/// </para>
/// <para>
/// 未配置任何启用条件时返回 true，表示该规则只受规则主档项目、生效期和状态控制。
/// 未找到条件评估器时按不命中处理，避免未知条件类型导致规则误命中。
/// </para>
/// </remarks>
public sealed class RuleConditionGroupMatcher : IRuleConditionGroupMatcher
{
    /// <summary>
    /// 条件评估器工厂，用于按 ConditionType 找到具体评估器。
    /// </summary>
    private readonly ConditionEvaluatorFactory _evaluatorFactory;
    /// <summary>
    /// 运行期诊断日志。
    /// </summary>
    private readonly ILogger<RuleConditionGroupMatcher> _logger;

    /// <summary>
    /// 初始化规则条件组匹配器。
    /// </summary>
    /// <param name="evaluatorFactory">条件评估器工厂。</param>
    /// <param name="logger">日志组件。</param>
    public RuleConditionGroupMatcher(
        ConditionEvaluatorFactory evaluatorFactory,
        ILogger<RuleConditionGroupMatcher> logger)
    {
        _evaluatorFactory = evaluatorFactory;
        _logger = logger;
    }

    /// <summary>
    /// 按条件组判断当前上下文是否满足规则条件集合。
    /// </summary>
    /// <param name="conditions">规则条件集合。</param>
    /// <param name="context">当前计价上下文。</param>
    /// <returns>任一条件组全部满足时返回 true。</returns>
    public async Task<bool> EvaluateAsync(IReadOnlyList<RuleCondition> conditions, PricingContext context)
    {
        if (conditions.Count == 0)
        {
            // 无条件规则视为通配，适合“只要项目命中就执行”的基础规则。
            return true;
        }

        var enabled = conditions.Where(c => c.IsEnabled == "Y").ToList();
        if (enabled.Count == 0)
        {
            // 条件都被禁用时等价于没有条件，规则是否命中只由主档控制。
            return true;
        }

        var groups = enabled.GroupBy(c => c.ConditionGroup);
        foreach (var group in groups)
        {
            // 组内 AND：任意条件不满足，当前组失败，继续尝试下一个 OR 组。
            var allMatch = true;

            foreach (var condition in group)
            {
                var evaluator = _evaluatorFactory.GetEvaluator(condition.ConditionType);
                if (evaluator is null)
                {
                    // 未知条件类型不能按通过处理，否则配置错误会扩大规则命中范围。
                    _logger.LogWarning("未找到条件评估器：条件类型={ConditionType}，规则ID={RuleId}",
                        condition.ConditionType, condition.RuleId);
                    allMatch = false;
                    break;
                }

                if (!await evaluator.EvaluateAsync(condition, context))
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                // 组间 OR：任意一个组全部满足即整条规则命中。
                return true;
            }
        }

        return false;
    }
}
