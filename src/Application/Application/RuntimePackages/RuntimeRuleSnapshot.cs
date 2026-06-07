using Pricing.RuleCenter.Core.Aggregates.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimeRuleSnapshot
{
    public RuntimeRule Rule { get; init; } = null!;

    public IReadOnlyList<RuntimeCondition> Conditions { get; init; } = Array.Empty<RuntimeCondition>();

    public IReadOnlyList<RuntimeAction> Actions { get; init; } = Array.Empty<RuntimeAction>();
}
