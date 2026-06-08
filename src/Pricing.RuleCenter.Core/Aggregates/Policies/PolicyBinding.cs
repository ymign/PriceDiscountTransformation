namespace Pricing.RuleCenter.Core.Aggregates.Policies;

public sealed class PolicyBinding
{
    public long PolicyBindingId { get; set; }

    public long PolicyVersionId { get; set; }

    public string BindingType { get; set; } = string.Empty;

    public string? ItemCode { get; set; }

    public string? ItemName { get; set; }

    public string? GroupCode { get; set; }

    public string? GroupName { get; set; }
}
