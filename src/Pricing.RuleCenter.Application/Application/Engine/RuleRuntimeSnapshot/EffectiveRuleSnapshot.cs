using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;

/// <summary>
/// 单条运行期生效规则快照。
/// </summary>
/// <remarks>
/// 规则匹配服务只消费快照，不直接关心快照来自旧 PR_RULE_* 表还是新运行包读模型。
/// 该对象把规则主档、当前版本条件和动作放在一起，保证同一次匹配使用同一版本配置。
/// </remarks>
public sealed class EffectiveRuleSnapshot
{
    /// <summary>
    /// 规则主档，包含项目编码、状态、生效期、优先级等匹配前置字段。
    /// </summary>
    public RuleAggregate Header { get; init; } = null!;

    /// <summary>
    /// 当前版本条件集合，按“组内 AND、组间 OR”参与匹配。
    /// </summary>
    public IReadOnlyList<RuleCondition> Conditions { get; init; } = Array.Empty<RuleCondition>();

    /// <summary>
    /// 当前版本动作集合，命中后由 <see cref="RuleActionPlanBuilder"/> 统一排序。
    /// </summary>
    public IReadOnlyList<RuleAction> Actions { get; init; } = Array.Empty<RuleAction>();
}
