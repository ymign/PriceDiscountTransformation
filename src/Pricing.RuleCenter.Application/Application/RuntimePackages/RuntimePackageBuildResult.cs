using Pricing.RuleCenter.Core.Aggregates.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageBuildResult
{
    public RuntimePackage Package { get; init; } = null!;

    public IReadOnlyList<RuntimePackagePolicy> PackagePolicies { get; init; } = Array.Empty<RuntimePackagePolicy>();

    public IReadOnlyList<RuntimeRule> Rules { get; init; } = Array.Empty<RuntimeRule>();

    public IReadOnlyList<RuntimeCondition> Conditions { get; init; } = Array.Empty<RuntimeCondition>();

    public IReadOnlyList<RuntimeAction> Actions { get; init; } = Array.Empty<RuntimeAction>();
}
