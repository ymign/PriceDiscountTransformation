using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Policies;

public sealed class PolicyVersion
{
    public long PolicyVersionId { get; set; }

    public long PolicyId { get; set; }

    public long TemplateVersionId { get; set; }

    public int VersionNo { get; set; }

    public string PolicyStatus { get; set; } = PolicyLifecycleCodes.Draft;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public string BindingType { get; set; } = string.Empty;

    public string ScopeLevel { get; set; } = string.Empty;

    public int PriorityWeight { get; set; } = 100;

    public string? Checksum { get; set; }

    public long? LastBuiltPackageId { get; set; }
}
