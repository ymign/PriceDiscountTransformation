namespace Pricing.RuleCenter.Core.Aggregates.Policies;

public sealed class PolicyScope
{
    public long PolicyScopeId { get; set; }

    public long PolicyVersionId { get; set; }

    public string ScopeDimension { get; set; } = string.Empty;

    public string ScopeOperator { get; set; } = string.Empty;

    public string? ScopeValueText { get; set; }

    public decimal? ScopeValueNumber { get; set; }

    public DateTime? ScopeValueDate { get; set; }

    public string? ScopeJson { get; set; }
}
