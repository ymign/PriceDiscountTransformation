using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

/// <summary>
/// 检查部位条件评估器。
/// </summary>
/// <remarks>
/// 该评估器用于把规则限制在指定检查部位或执行部位。空 RightValue 表示规则未配置部位限制，
/// 因此按“通过”处理，避免可选条件缺失导致整条规则无法命中。
/// </remarks>
public sealed class BodyPartMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码。
    /// </summary>
    public string ConditionType => "BODY_PART";

    /// <summary>
    /// 判断请求上下文的部位编码是否满足规则条件。
    /// </summary>
    /// <param name="condition">规则条件，RightValue 保存目标部位编码。</param>
    /// <param name="context">计价上下文，BodyPartCode 来自本次收费请求。</param>
    /// <returns>满足部位条件时返回 <c>true</c>。</returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 部位是可选匹配维度。未配置目标值时视为不限制部位，保持规则可继续参与其他条件判断。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // 部位编码按业务编码比较，不区分大小写，兼容上游系统大小写不一致。
        return string.Equals(context.BodyPartCode, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
