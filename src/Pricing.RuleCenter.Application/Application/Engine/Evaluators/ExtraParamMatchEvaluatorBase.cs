using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Evaluators;

/// <summary>
/// 基于 ExtraParams 的通用条件匹配基类。
/// </summary>
public abstract class ExtraParamMatchEvaluatorBase : IRuleConditionEvaluator
{
    private readonly string _extraParamKey;

    /// <summary>
    /// 初始化基于 ExtraParams 的通用条件匹配器。
    /// </summary>
    /// <param name="extraParamKey">要读取的 ExtraParams 键名。</param>
    protected ExtraParamMatchEvaluatorBase(string extraParamKey)
    {
        _extraParamKey = extraParamKey;
    }

    /// <summary>
    /// 条件类型编码。
    /// </summary>
    public abstract string ConditionType { get; }

    /// <summary>
    /// 评估 ExtraParams 中的条件值是否与规则配置匹配。
    /// </summary>
    public ValueTask<bool> EvaluateAsync(RuleCondition condition, PricingContext context)
    {
        return ValueTask.FromResult(Evaluate(condition, context));
    }

    /// <summary>
    /// 同步评估条件，供单元测试直接调用。
    /// </summary>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        if (string.IsNullOrWhiteSpace(condition.RightValue))
        {
            return true;
        }

        if (context.ExtraParams is null ||
            !context.ExtraParams.TryGetValue(_extraParamKey, out var actual) ||
            string.IsNullOrWhiteSpace(actual))
        {
            return false;
        }

        var allowedValues = SplitValues(condition.RightValue);
        var actualValues = SplitValues(actual);
        return actualValues.Any(value => allowedValues.Contains(value));
    }

    private static HashSet<string> SplitValues(string value)
    {
        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Trim())
            .Where(item => item.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
