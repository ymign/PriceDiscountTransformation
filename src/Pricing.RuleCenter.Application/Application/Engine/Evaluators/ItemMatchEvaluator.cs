using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Evaluators;

/// <summary>
/// 收费项目匹配条件评估器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】项目匹配是最基础的规则命中条件。每条规则必须关联到具体收费项目，
/// 否则可能影响到不相关的项目。
/// </para>
/// <para>
/// 【空值处理策略——与其他评估器不同】
/// 与部位、场景等可选维度不同，项目条件如果没有 RightValue，视为配置不完整并返回不命中。
/// 这是为了避免一条本应限定单项目的条件变成全局通配——在计价场景下，
/// 一条错误的折价规则可能导致大量项目被错误折价，造成资金风险。
/// </para>
/// <para>
/// 【与 RuleMatchService 的关系】
/// RuleMatchService 在第一阶段已按 ItemCode 筛选了候选规则（PR_RULE_HEADER.ITEM_CODE），
/// 本评估器做的是条件级别的二次确认。两层匹配确保：
/// <list type="number">
///   <item><description>仓储层按项目索引快速过滤（O(1) 查找）</description></item>
///   <item><description>条件层按规则配置精确匹配（防御性校验）</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class ItemMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码，对应 PR_RULE_CONDITION.CONDITION_TYPE = "ITEM_MATCH"。
    /// </summary>
    public string ConditionType => RuleConditionTypeCodes.ItemMatch;

    /// <summary>
    /// 异步评估项目编码条件。当前评估只依赖内存上下文，使用 <see cref="ValueTask{TResult}"/> 避免额外任务分配。
    /// </summary>
    /// <param name="condition">规则条件配置。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>项目编码条件满足时返回 <c>true</c>。</returns>
    public ValueTask<bool> EvaluateAsync(RuleCondition condition, PricingContext context)
    {
        return ValueTask.FromResult(Evaluate(condition, context));
    }

    /// <summary>
    /// 判断请求项目编码是否等于规则条件中的目标项目编码。
    /// </summary>
    /// <param name="condition">
    /// 规则条件配置。RightValue 保存目标项目编码（来自 PR_RULE_CONDITION.RIGHT_VALUE 列）。
    /// </param>
    /// <param name="context">
    /// 计价上下文。ItemCode 来自本次收费请求。
    /// </param>
    /// <returns>
    /// 项目编码相同时返回 <c>true</c>；RightValue 为空时返回 <c>false</c>（配置不完整）。
    /// </returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 项目条件缺少目标值时不应放行，否则可能让本该限定单项目的规则影响所有项目。
        // 这是与其他可选评估器（部位、场景）的关键区别：
        //   - 部位/场景为空 → 通过（可选维度，不限制）
        //   - 项目为空 → 不通过（必选维度，配置缺失应阻断）
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return false;
        }

        // 项目编码按业务编码比较，不区分大小写（OrdinalIgnoreCase），
        // 兼容 HIS 或配置端大小写差异（如 "ITEM001" vs "item001"）。
        return string.Equals(context.ItemCode, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
