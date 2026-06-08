using Pricing.RuleCenter.Core.Aggregates.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageTraceResolution
{
    public long? RuntimePackageId { get; init; }

    public long? RuntimePackageVersion { get; init; }

    public IReadOnlyDictionary<long, RuntimeRule> RuntimeRulesById { get; init; } =
        new Dictionary<long, RuntimeRule>();

    public RuntimeRule? FindRule(long? runtimeRuleId)
    {
        return runtimeRuleId.HasValue && RuntimeRulesById.TryGetValue(runtimeRuleId.Value, out var rule)
            ? rule
            : null;
    }
}
