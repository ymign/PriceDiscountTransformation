using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Evaluators;

/// <summary>
/// 收费科室排除条件评估器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务背景】旧系统 CTMRFeeRule.GetFeeItemList 中，数量折价规则有硬编码前置条件：
/// 部门编码 != "7021"（挂号部不执行数量折价）。迁移到规则中心后，该条件改为规则配置，
/// 需要在折价规则的条件列表中配置一条 CHARGE_DEPT_EXCLUDE 条件。
/// </para>
/// <para>
/// 【语义说明】本评估器是"排除"语义而非"匹配"语义：
/// 当请求的收费科室编码存在于排除列表中时，返回 false（规则不适用）；
/// 不在列表中时返回 true（规则正常执行）。
/// 这与 ChargeSceneMatchEvaluator 的"匹配"语义相反，专门用于豁免特定科室。
/// </para>
/// <para>
/// 【RightValue 格式】逗号分隔的科室编码列表，如 "7021" 或 "7021,7022"。
/// RightValue 为空时视为没有排除科室，返回通过（允许所有科室执行规则）。
/// </para>
/// </remarks>
public sealed class ChargeDeptExcludeEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码，对应 PR_RULE_CONDITION.CONDITION_TYPE = "CHARGE_DEPT_EXCLUDE"。
    /// </summary>
    public string ConditionType => "CHARGE_DEPT_EXCLUDE";

    /// <summary>
    /// 异步评估收费科室排除条件。当前评估只依赖内存上下文，使用 <see cref="ValueTask{TResult}"/> 避免额外任务分配。
    /// </summary>
    /// <param name="condition">规则条件配置。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>当前科室未被排除时返回 <c>true</c>。</returns>
    public ValueTask<bool> EvaluateAsync(RuleCondition condition, PricingContext context)
    {
        return ValueTask.FromResult(Evaluate(condition, context));
    }

    /// <summary>
    /// 判断请求上下文的收费科室是否不在排除列表中。
    /// </summary>
    /// <param name="condition">
    /// 规则条件配置。RightValue 保存逗号分隔的排除科室编码，如 "7021"。
    /// </param>
    /// <param name="context">
    /// 计价上下文。ChargeDeptCode 来自本次收费请求，由渠道传入。
    /// </param>
    /// <returns>
    /// 收费科室不在排除列表中时返回 <c>true</c>（规则可以执行）。
    /// 收费科室在排除列表中时返回 <c>false</c>（规则不适用于该科室）。
    /// RightValue 为空时返回 <c>true</c>（不限制科室）。
    /// ChargeDeptCode 为空时返回 <c>true</c>（未传入科室信息，默认不排除）。
    /// </returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 排除列表为空时视为不限制科室，所有科室均可执行规则。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // 未传入科室编码时，按"不排除"处理，规则正常执行。
        // 场景：旧渠道未携带 ChargeDeptCode 字段，不应因此阻断折价规则。
        if (string.IsNullOrEmpty(context.ChargeDeptCode))
        {
            return true;
        }

        // 将 RightValue 按逗号拆分，检查当前科室是否在排除列表中。
        // 拆分时去除两端空格，避免配置人员误输入空格导致比较失败。
        var excludedDepts = condition.RightValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var dept in excludedDepts)
        {
            if (string.Equals(context.ChargeDeptCode.Trim(), dept.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                // 科室在排除列表中，规则不适用。
                return false;
            }
        }

        return true;
    }
}
