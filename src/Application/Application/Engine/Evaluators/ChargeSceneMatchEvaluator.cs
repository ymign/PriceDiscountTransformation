using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Evaluators;

/// <summary>
/// 收费场景匹配条件评估器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】收费场景用于区分门诊、住院、急诊或其他业务来源。
/// 不同场景可能适用不同的折价规则（例如住院项目不参与门诊折价活动）。
/// </para>
/// <para>
/// 【空值处理】RightValue 为空时表示规则不限制场景，返回通过。
/// 这允许配置跨场景通用规则（适用于所有收费途径）。
/// </para>
/// <para>
/// 【收费途径统计】场景信息还用于全院累计的按收费途径统计，
/// 范围包括门诊收费、手术/医技划价、终端确认、护士过医嘱/分解医嘱。
/// </para>
/// </remarks>
public sealed class ChargeSceneMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码，对应 PR_RULE_CONDITION.CONDITION_TYPE = "CHARGE_SCENE"。
    /// </summary>
    public string ConditionType => "CHARGE_SCENE";

    /// <summary>
    /// 异步评估收费场景条件。当前评估只依赖内存上下文，使用 <see cref="ValueTask{TResult}"/> 避免额外任务分配。
    /// </summary>
    /// <param name="condition">规则条件配置。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>收费场景条件满足时返回 <c>true</c>。</returns>
    public ValueTask<bool> EvaluateAsync(RuleCondition condition, PricingContext context)
    {
        return ValueTask.FromResult(Evaluate(condition, context));
    }

    /// <summary>
    /// 判断请求上下文的收费场景是否满足规则条件。
    /// </summary>
    /// <param name="condition">
    /// 规则条件配置。RightValue 保存目标收费场景编码（如 OUTPATIENT、INPATIENT、EMERGENCY）。
    /// </param>
    /// <param name="context">
    /// 计价上下文。ChargeScene 来自本次收费请求，由渠道传入。
    /// </param>
    /// <returns>
    /// 满足场景条件时返回 <c>true</c>。
    /// RightValue 为空时返回 <c>true</c>（不限制场景）。
    /// </returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 场景条件为空时视为通配，便于配置跨场景通用规则。
        // 典型场景：某折价公式对门诊和住院都适用，无需维护两条规则。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // 场景编码按业务编码比较，不区分大小写（OrdinalIgnoreCase），
        // 减少上游编码大小写差异（如 "Outpatient" vs "OUTPATIENT"）带来的误不命中。
        return string.Equals(context.ChargeScene, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
