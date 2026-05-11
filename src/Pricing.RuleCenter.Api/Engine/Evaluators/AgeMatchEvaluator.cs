using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

/// <summary>
/// 年龄匹配条件评估器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】判断患者年龄是否在规则配置的范围内。典型场景：
/// <list type="bullet">
///   <item><description>儿童加收规则：年龄 >= 0 且 < 14 岁的患者适用</description></item>
///   <item><description>老年优惠规则：年龄 >= 60 岁的患者适用</description></item>
///   <item><description>新生儿特殊计价：年龄 < 28 天的患者适用（需渠道以小数形式传入）</description></item>
/// </list>
/// </para>
/// <para>
/// 【范围格式】condition.RightValue 使用半开区间表示法，支持以下格式：
/// <list type="bullet">
///   <item><description>">=0,&lt;14" — 年龄 >= 0 且 &lt; 14（左闭右开）</description></item>
///   <item><description>">=14" — 年龄 >= 14（无上界）</description></item>
///   <item><description>"&lt;6" — 年龄 &lt; 6（无下界）</description></item>
///   <item><description>">=0,&lt;=14" — 年龄 >= 0 且 &lt;= 14（左闭右闭）</description></item>
/// </list>
/// 多个范围条件用逗号分隔，整体按 AND 语义组合。
/// </para>
/// <para>
/// 【空值处理策略】
/// <list type="bullet">
///   <item><description>condition.RightValue 为空 → 通过（规则未配置年龄限制）</description></item>
///   <item><description>context.PatientAge 为空 → 通过（渠道未传入年龄，不校验）</description></item>
/// </list>
/// 这与业务规则"NULL ≠ 0"一致：NULL 表示"不校验"。
/// </para>
/// <para>
/// 【保守策略】
/// <list type="bullet">
///   <item><description>范围解析失败 → 不通过（配置错误保守处理）</description></item>
///   <item><description>评估器异常 → 不通过（安全优先）</description></item>
/// </list>
/// 配置错误应在规则发布校验阶段发现，运行期做保守处理。
/// </para>
/// </remarks>
public sealed class AgeMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码，对应 PR_RULE_CONDITION.CONDITION_TYPE = "AGE_MATCH"。
    /// </summary>
    public string ConditionType => "AGE_MATCH";

    /// <summary>
    /// 判断患者年龄是否在规则配置的范围内。
    /// </summary>
    /// <param name="condition">
    /// 规则条件配置。RightValue 保存年龄范围表达式（来自 PR_RULE_CONDITION.RIGHT_VALUE 列）。
    /// 格式示例：">=0,&lt;14" 或 ">=60"。
    /// </param>
    /// <param name="context">
    /// 计价上下文。PatientAge 来自本次收费请求，由渠道（HIS/自助机/微信）传入。
    /// </param>
    /// <returns>
    /// 年龄在配置范围内时返回 <c>true</c>。
    /// RightValue 为空时返回 <c>true</c>（不限制年龄）。
    /// PatientAge 为空时返回 <c>true</c>（渠道未传入，不校验）。
    /// 范围解析失败时返回 <c>false</c>（配置错误保守处理）。
    /// </returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 年龄条件为空时视为不限制年龄，避免可选条件缺失导致规则整体失效。
        // 典型场景：某规则适用于所有年龄段，无需配置年龄条件。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // 患者年龄为空时按"不校验"处理。原因：
        // 1. 部分渠道可能不传入年龄信息，不应因此阻断规则匹配
        // 2. 与"NULL ≠ 0"业务规则一致：NULL 表示"不校验"
        if (context.PatientAge == null)
        {
            return true;
        }

        var age = context.PatientAge.Value;

        // ========== 解析并校验范围表达式 ==========
        // RightValue 可能包含多个范围条件（逗号分隔），整体按 AND 语义组合。
        // 例如 ">=0,<14" 表示 age >= 0 AND age < 14。
        var rangeParts = condition.RightValue.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in rangeParts)
        {
            if (!EvaluateSingleRange(part.Trim(), age))
            {
                // 任一范围条件不满足即返回 false（AND 语义）。
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 评估单个范围条件（如 ">=0" 或 "&lt;14"）。
    /// </summary>
    /// <param name="rangeExpr">单个范围表达式。</param>
    /// <param name="age">患者年龄。</param>
    /// <returns>年龄满足该范围条件时返回 <c>true</c>；解析失败返回 <c>false</c>。</returns>
    /// <remarks>
    /// 【支持的操作符】
    /// <list type="bullet">
    ///   <item><description>>= — 大于等于</description></item>
    ///   <item><description>&lt;= — 小于等于</description></item>
    ///   <item><description>> — 大于</description></item>
    ///   <item><description>&lt; — 小于</description></item>
    ///   <item><description>= — 等于</description></item>
    /// </list>
    /// 【解析策略】从左到右扫描操作符前缀，最长匹配优先（>= 优先于 >）。
    /// 解析失败时返回 false，配置错误应在规则发布校验阶段发现。
    /// </remarks>
    private static bool EvaluateSingleRange(string rangeExpr, int age)
    {
        if (string.IsNullOrEmpty(rangeExpr))
        {
            return false;
        }

        // 从左到右尝试匹配操作符前缀，最长匹配优先。
        // >= 优先于 >，<= 优先于 <。
        if (rangeExpr.StartsWith(">="))
        {
            if (int.TryParse(rangeExpr[2..].Trim(), out var threshold))
            {
                return age >= threshold;
            }
        }
        else if (rangeExpr.StartsWith("<="))
        {
            if (int.TryParse(rangeExpr[2..].Trim(), out var threshold))
            {
                return age <= threshold;
            }
        }
        else if (rangeExpr.StartsWith('>'))
        {
            if (int.TryParse(rangeExpr[1..].Trim(), out var threshold))
            {
                return age > threshold;
            }
        }
        else if (rangeExpr.StartsWith('<'))
        {
            if (int.TryParse(rangeExpr[1..].Trim(), out var threshold))
            {
                return age < threshold;
            }
        }
        else if (rangeExpr.StartsWith('='))
        {
            if (int.TryParse(rangeExpr[1..].Trim(), out var threshold))
            {
                return age == threshold;
            }
        }

        // 解析失败（格式不正确），按不命中处理。
        // 配置错误应在规则发布校验阶段发现，运行期做保守处理。
        return false;
    }
}
