using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageBuildContext
{
    public string BuiltBy { get; init; } = string.Empty;

    public DateTime? BuildAt { get; init; }

    public string BuildScope { get; init; } = RuntimeBuildScopeCodes.Full;

    public IReadOnlyCollection<long>? PolicyVersionIds { get; init; }
}
