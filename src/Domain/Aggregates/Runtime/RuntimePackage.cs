using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Runtime;

public sealed class RuntimePackage
{
    public long PackageId { get; set; }

    public long PackageVersion { get; set; }

    public string PackageStatus { get; set; } = RuntimePackageStatusCodes.Building;

    public string BuildScope { get; set; } = RuntimeBuildScopeCodes.Full;

    public string? SourceChecksum { get; set; }

    public string? BuiltBy { get; set; }

    public DateTime? BuiltAt { get; set; }

    public string? ActivatedBy { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public long? RolledBackFromPackageId { get; set; }
}
