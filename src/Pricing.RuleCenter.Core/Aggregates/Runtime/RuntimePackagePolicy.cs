namespace Pricing.RuleCenter.Core.Aggregates.Runtime;

public sealed class RuntimePackagePolicy
{
    public long PackagePolicyId { get; set; }

    public long PackageId { get; set; }

    public long PolicyVersionId { get; set; }

    public string PolicyCode { get; set; } = string.Empty;

    public long TemplateVersionId { get; set; }

    public string CapabilityFamily { get; set; } = string.Empty;

    public string PriorityKey { get; set; } = string.Empty;
}
