using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Policies;

public sealed class PolicyAggregate
{
    public long PolicyId { get; set; }

    public string PolicyCode { get; set; } = string.Empty;

    public string PolicyName { get; set; } = string.Empty;

    public long TemplateId { get; set; }

    public string OwnerType { get; set; } = string.Empty;

    public string PublishProfile { get; set; } = string.Empty;

    public string Status { get; set; } = PolicyLifecycleCodes.Draft;

    public int CurrentVersionNo { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
}
