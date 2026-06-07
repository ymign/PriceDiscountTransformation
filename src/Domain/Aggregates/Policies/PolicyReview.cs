using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Policies;

public sealed class PolicyReview
{
    public long ReviewId { get; set; }

    public long PolicyVersionId { get; set; }

    public string ReviewStatus { get; set; } = PolicyReviewStatusCodes.Pending;

    public string ReviewStage { get; set; } = string.Empty;

    public string? SubmittedBy { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public string? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewComment { get; set; }

    public string? SourceChecksum { get; set; }
}
