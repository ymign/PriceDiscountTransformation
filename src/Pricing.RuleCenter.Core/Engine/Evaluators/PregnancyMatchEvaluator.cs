using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Evaluators;

/// <summary>
/// 同胎次匹配条件评估器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】判断当前计价项目是否属于指定胎次，用于产科彩超的同胎次限制。
/// 典型场景：某产妇怀双胎，第一次彩超检查按胎次A收费，第二次按胎次B收费，
/// 同胎次的多次检查共享限额，不同胎次独立累计。
/// </para>
/// <para>
/// 【空值处理策略】
/// <list type="bullet">
///   <item><description>condition.RightValue 为空 → 通过（规则未配置胎次限制，不限制）</description></item>
///   <item><description>context.ExtraParams 为空或不包含 pregnancyId → 通过（非产科项目或渠道未传入胎次）</description></item>
///   <item><description>pregnancyId 值为空字符串 → 通过（等同于未传入）</description></item>
/// </list>
/// 这与业务规则"NULL ≠ 0"一致：NULL 表示"不校验"。
/// </para>
/// <para>
/// 【与限额累计的关系】
/// 胎次标识不仅用于条件匹配，还是限额累计的重要维度。
/// PR_LIMIT_OCCUPY 表中应包含胎次维度，确保同胎次共享限额、不同胎次独立累计。
/// </para>
/// <para>
/// 【保守策略】评估器异常时默认不通过（安全优先），防止因异常导致错误放行。
/// </para>
/// </remarks>
public sealed class PregnancyMatchEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码，对应 PR_RULE_CONDITION.CONDITION_TYPE = "PREGNANCY_MATCH"。
    /// </summary>
    public string ConditionType => "PREGNANCY_MATCH";

    /// <summary>
    /// 异步评估同胎次条件。当前评估只依赖内存上下文，使用 <see cref="ValueTask{TResult}"/> 避免额外任务分配。
    /// </summary>
    /// <param name="condition">规则条件配置。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>胎次条件满足时返回 <c>true</c>。</returns>
    public ValueTask<bool> EvaluateAsync(RuleCondition condition, PricingContext context)
    {
        return ValueTask.FromResult(Evaluate(condition, context));
    }

    /// <summary>
    /// 判断当前计价项目的胎次标识是否满足规则条件。
    /// </summary>
    /// <param name="condition">
    /// 规则条件配置。RightValue 保存目标胎次标识模式（来自 PR_RULE_CONDITION.RIGHT_VALUE 列）。
    /// </param>
    /// <param name="context">
    /// 计价上下文。胎次标识从 ExtraParams 字典中以键名 "pregnancyId" 获取。
    /// </param>
    /// <returns>
    /// 满足胎次条件时返回 <c>true</c>。
    /// RightValue 为空时返回 <c>true</c>（不限制胎次）。
    /// ExtraParams 为空或不包含 pregnancyId 时返回 <c>true</c>（非产科项目不校验）。
    /// </returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 胎次条件为空时视为不限制胎次，避免可选条件缺失导致规则整体失效。
        // 典型场景：某规则不区分胎次，适用于所有产科检查。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // 从扩展参数中获取胎次标识。ExtraParams 可能为 null（非产科项目或渠道未传入），
        // 按"不校验"（通过）处理，与"NULL ≠ 0"业务规则一致。
        var pregnancyId = context.ExtraParams?.GetValueOrDefault("pregnancyId");

        // 胎次标识为空时按"不校验"处理。原因：
        // 1. 非产科项目不会传入胎次信息，不应因此阻断规则匹配
        // 2. 渠道未传入胎次时，按保守但不阻断的方式处理
        if (string.IsNullOrEmpty(pregnancyId))
        {
            return true;
        }

        // 胎次标识按业务编码比较，不区分大小写（OrdinalIgnoreCase），
        // 兼容上游系统大小写差异。
        return string.Equals(pregnancyId, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
