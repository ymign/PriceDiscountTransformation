using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Templates;

public sealed class TemplateVersion
{
    public long TemplateVersionId { get; set; }

    public long TemplateId { get; set; }

    public int VersionNo { get; set; }

    public string VersionStatus { get; set; } = TemplateLifecycleCodes.Draft;

    public string CapabilityFamily { get; set; } = string.Empty;

    public string MergeMode { get; set; } = "SINGLE_WINNER";

    public string? Checksum { get; set; }

    public string? Description { get; set; }

    public string? PublishedBy { get; set; }

    public DateTime? PublishedAt { get; set; }
}
