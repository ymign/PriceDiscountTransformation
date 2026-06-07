using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 规则条件组匹配器。
/// </summary>
public sealed class RuleConditionGroupMatcher
{
    private readonly ConditionEvaluatorFactory _evaluatorFactory;
    private readonly ILogger _logger;

    /// <summary>
    /// 初始化规则条件组匹配器。
    /// </summary>
    public RuleConditionGroupMatcher(
        ConditionEvaluatorFactory evaluatorFactory,
        ILogger logger)
    {
        _evaluatorFactory = evaluatorFactory;
        _logger = logger;
    }

    /// <summary>
    /// 按条件组判断当前上下文是否满足规则条件集合。
    /// </summary>
    public async Task<bool> EvaluateAsync(IReadOnlyList<RuleCondition> conditions, PricingContext context)
    {
        if (conditions.Count == 0)
        {
            return true;
        }

        var enabled = conditions.Where(c => c.IsEnabled == "Y").ToList();
        if (enabled.Count == 0)
        {
            return true;
        }

        var groups = enabled.GroupBy(c => c.ConditionGroup);
        foreach (var group in groups)
        {
            var allMatch = true;

            foreach (var condition in group)
            {
                var evaluator = _evaluatorFactory.GetEvaluator(condition.ConditionType);
                if (evaluator is null)
                {
                    _logger.LogWarning("未找到条件评估器: {ConditionType}，RuleId={RuleId}",
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
                return true;
            }
        }

        return false;
    }
}
