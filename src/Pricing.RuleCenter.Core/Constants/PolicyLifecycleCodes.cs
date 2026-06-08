namespace Pricing.RuleCenter.Core.Constants;

public static class TemplateLifecycleCodes
{
    public const string Draft = "DRAFT";
    public const string Published = "PUBLISHED";
    public const string Disabled = "DISABLED";
}

public static class PolicyLifecycleCodes
{
    public const string Draft = "DRAFT";
    public const string Validated = "VALIDATED";
    public const string ReviewPending = "REVIEW_PENDING";
    public const string Approved = "APPROVED";
    public const string PublishReady = "PUBLISH_READY";
    public const string Superseded = "SUPERSEDED";
}

public static class PolicyReviewStatusCodes
{
    public const string Pending = "PENDING";
    public const string Approved = "APPROVED";
    public const string Rejected = "REJECTED";
    public const string Outdated = "OUTDATED";
}

public static class PolicyPublishProfileCodes
{
    public const string Direct = "DIRECT";
    public const string ReviewRequired = "REVIEW_REQUIRED";
}
