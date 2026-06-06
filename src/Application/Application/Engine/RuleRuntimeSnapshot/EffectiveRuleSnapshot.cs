using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;

/// <summary>
/// 单条运行期生效规则快照。
/// </summary>
public sealed class EffectiveRuleSnapshot
{
    /// <summary>规则主档。</summary>
    public RuleAggregate Header { get; init; } = null!;

    /// <summary>当前版本条件集合。</summary>
    public IReadOnlyList<RuleCondition> Conditions { get; init; } = Array.Empty<RuleCondition>();

    /// <summary>当前版本动作集合。</summary>
    public IReadOnlyList<RuleAction> Actions { get; init; } = Array.Empty<RuleAction>();
}
