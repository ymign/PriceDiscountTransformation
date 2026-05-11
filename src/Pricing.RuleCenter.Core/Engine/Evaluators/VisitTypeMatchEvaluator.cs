using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Evaluators;

/// <summary>
/// 就诊类型匹配条件评估器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】判断当前就诊类型是否匹配规则条件。不同就诊类型（门诊、住院、急诊等）
/// 可能适用不同的折价规则。例如：住院项目不参与门诊折价活动，急诊加收规则只对急诊就诊生效。
/// </para>
/// <para>
/// 【与 ChargeSceneMatchEvaluator 的区别】
/// <list type="bullet">
///   <item><description>ChargeSceneMatchEvaluator：匹配收费场景（门诊收费、手术划价、终端确认等收费途径）</description></item>
///   <item><description>VisitTypeMatchEvaluator：匹配就诊类型（门诊、住院、急诊等患者就诊性质）</description></item>
/// </list>
/// 同一次门诊就诊中，可能涉及多种收费场景（如门诊收费 + 医技划价），
/// 但就诊类型始终是"门诊"。两者互补，不互斥。
/// </para>
/// <para>
/// 【空值处理策略】
/// <list type="bullet">
///   <item><description>condition.RightValue 为空 → 通过（规则未配置就诊类型限制）</description></item>
///   <item><description>context.VisitType 为空 → 通过（渠道未传入就诊类型，不校验）</description></item>
/// </list>
/// 这与业务规则"NULL ≠ 0"一致：NULL 表示"不校验"。
/// </para>
/// <para>
/// 【多值匹配】RightValue 支持逗号分隔的多个就诊类型编码（如 "OUTPATIENT,EMERGENCY"），
/// 任一匹配即视为满足条件。这是 IN 操作符的文本化实现。
/// </para>
/// <para>
/// 【保守策略】评估器异常时默认不通过（安全优先），防止因异常导致错误放行。
/// </para>
/// </remarks>
public sealed class VisitTypeMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码，对应 PR_RULE_CONDITION.CONDITION_TYPE = "VISIT_TYPE_MATCH"。
    /// </summary>
    public string ConditionType => "VISIT_TYPE_MATCH";

    /// <summary>
    /// 判断当前就诊类型是否满足规则条件。
    /// </summary>
    /// <param name="condition">
    /// 规则条件配置。RightValue 保存目标就诊类型编码，支持逗号分隔多值
    /// （来自 PR_RULE_CONDITION.RIGHT_VALUE 列）。
    /// </param>
    /// <param name="context">
    /// 计价上下文。VisitType 来自本次收费请求，由渠道（HIS/自助机/微信）传入。
    /// </param>
    /// <returns>
    /// 满足就诊类型条件时返回 <c>true</c>。
    /// RightValue 为空时返回 <c>true</c>（不限制就诊类型）。
    /// VisitType 为空时返回 <c>true</c>（渠道未传入，不校验）。
    /// </returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 就诊类型条件为空时视为不限制，避免可选条件缺失导致规则整体失效。
        // 典型场景：某折价规则对门诊和住院都适用，无需维护两条规则。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // 就诊类型为空时按"不校验"处理。原因：
        // 1. 部分渠道可能不传入就诊类型，不应因此阻断规则匹配
        // 2. 与"NULL ≠ 0"业务规则一致：NULL 表示"不校验"
        if (string.IsNullOrEmpty(context.VisitType))
        {
            return true;
        }

        // 支持逗号分隔的多值匹配（IN 语义）。
        // 例如 RightValue = "OUTPATIENT,EMERGENCY" 表示门诊或急诊均匹配。
        // 按逗号拆分后逐一比较，任一匹配即返回 true。
        var allowedTypes = condition.RightValue.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var allowedType in allowedTypes)
        {
            // 就诊类型按业务编码比较，不区分大小写（OrdinalIgnoreCase），
            // 兼容上游系统大小写差异（如 "Outpatient" vs "OUTPATIENT"）。
            if (string.Equals(context.VisitType, allowedType.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
