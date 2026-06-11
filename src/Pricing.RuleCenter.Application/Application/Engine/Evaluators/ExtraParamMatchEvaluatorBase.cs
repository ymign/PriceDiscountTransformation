using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Evaluators;

/// <summary>
/// 基于 ExtraParams 的通用条件匹配基类。
/// </summary>
/// <remarks>
/// <para>
/// 该基类服务于诊断、设备类型、孕次、医保类型等“请求扩展参数”条件。
/// 这些维度在接口 DTO 中不是高频固定字段，但会影响部分规则匹配，因此通过 ExtraParams 进入上下文。
/// </para>
/// <para>
/// RightValue 为空表示“不限制”，返回 true；请求未传对应扩展参数时返回 false，
/// 因为规则明确要求该扩展参数时，缺失不能误判命中。
/// </para>
/// </remarks>
public abstract class ExtraParamMatchEvaluatorBase : IRuleConditionEvaluator
{
    /// <summary>
    /// 本评估器读取的 ExtraParams 键名。
    /// </summary>
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
    /// <param name="condition">规则条件配置，RightValue 支持逗号分隔多个允许值。</param>
    /// <param name="context">计价上下文，ExtraParams 来自请求级和明细级扩展参数合并结果。</param>
    /// <returns>扩展参数匹配时返回 true。</returns>
    public ValueTask<bool> EvaluateAsync(RuleCondition condition, PricingContext context)
    {
        return ValueTask.FromResult(Evaluate(condition, context));
    }

    /// <summary>
    /// 同步评估条件，供单元测试直接调用。
    /// </summary>
    /// <param name="condition">规则条件配置。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>扩展参数满足配置时返回 true。</returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        if (string.IsNullOrWhiteSpace(condition.RightValue))
        {
            // 未配置目标值表示不限制该扩展参数。
            return true;
        }

        if (context.ExtraParams is null ||
            !context.ExtraParams.TryGetValue(_extraParamKey, out var actual) ||
            string.IsNullOrWhiteSpace(actual))
        {
            // 规则明确配置了扩展参数条件，但请求未提供该参数，按不命中处理。
            return false;
        }

        // RightValue 和实际值都支持逗号分隔，任一实际值命中允许集合即通过。
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
