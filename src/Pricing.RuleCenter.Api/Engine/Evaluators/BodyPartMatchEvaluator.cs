using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

/// <summary>
/// 检查部位匹配条件评估器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】将规则限制在指定检查部位或执行部位。例如：某项目在"左乳"和"右乳"
/// 执行不同的换算规则，需要按部位匹配不同的规则条目。
/// </para>
/// <para>
/// 【空值处理】RightValue 为空时表示规则未配置部位限制，按"通过"处理。
/// 这是可选匹配维度的设计：未配置 = 不限制，避免可选条件缺失导致整条规则无法命中。
/// 这与业务规则"NULL ≠ 0"一致：NULL 表示"不校验"。
/// </para>
/// <para>
/// 【多部位场景】对于多肿物、多部位项目，请求方应通过 pricingParts 明细表达每个部位，
/// 计价引擎对每个部位独立匹配规则。不能将多部位压成单个数量粗算。
/// </para>
/// </remarks>
public sealed class BodyPartMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码，对应 PR_RULE_CONDITION.CONDITION_TYPE = "BODY_PART"。
    /// </summary>
    public string ConditionType => "BODY_PART";

    /// <summary>
    /// 判断请求上下文的部位编码是否满足规则条件。
    /// </summary>
    /// <param name="condition">
    /// 规则条件配置。RightValue 保存目标部位编码（来自 PR_RULE_CONDITION.RIGHT_VALUE 列）。
    /// </param>
    /// <param name="context">
    /// 计价上下文。BodyPartCode 来自本次收费请求，由渠道（HIS/自助机/微信）传入。
    /// </param>
    /// <returns>
    /// 满足部位条件时返回 <c>true</c>。
    /// RightValue 为空时返回 <c>true</c>（不限制部位）。
    /// </returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 部位是可选匹配维度。未配置目标值时视为不限制部位，保持规则可继续参与其他条件判断。
        // 典型场景：某折价公式适用于所有部位，无需配置部位条件。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // 部位编码按业务编码比较，不区分大小写（OrdinalIgnoreCase），
        // 兼容上游系统（HIS、自助机）大小写不一致的情况。
        return string.Equals(context.BodyPartCode, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
