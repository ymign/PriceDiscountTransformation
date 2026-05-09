namespace Pricing.RuleCenter.Api.Dto;

public sealed class RuleVersionResponse
{
    public long VersionId { get; init; }
    public long RuleId { get; init; }
    public int VersionNo { get; init; }
    public string VersionStatus { get; init; } = string.Empty;
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public string? RuleSnapshot { get; init; }
    public string? PublishedBy { get; init; }
    public DateTime? PublishedAt { get; init; }
    public string? PublishRemark { get; init; }
}
