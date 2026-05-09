using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

/// <summary>
/// 收费场景条件评估器。
/// </summary>
/// <remarks>
/// 收费场景通常用于区分门诊、住院、急诊或其他业务来源。空 RightValue 表示规则不限制场景，
/// 因此返回通过。
/// </remarks>
public sealed class ChargeSceneMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码。
    /// </summary>
    public string ConditionType => "CHARGE_SCENE";

    /// <summary>
    /// 判断请求上下文的收费场景是否满足规则条件。
    /// </summary>
    /// <param name="condition">规则条件，RightValue 保存目标收费场景编码。</param>
    /// <param name="context">计价上下文，ChargeScene 来自本次收费请求。</param>
    /// <returns>满足场景条件时返回 <c>true</c>。</returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 场景条件为空时视为通配，便于配置跨场景通用规则。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // 场景编码按业务编码比较，不区分大小写，减少上游编码大小写差异带来的误不命中。
        return string.Equals(context.ChargeScene, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
