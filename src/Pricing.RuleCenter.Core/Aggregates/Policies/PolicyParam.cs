namespace Pricing.RuleCenter.Core.Aggregates.Policies;

public sealed class PolicyParam
{
    public long PolicyParamId { get; set; }

    public long PolicyVersionId { get; set; }

    public string ParamCode { get; set; } = string.Empty;

    public string ValueType { get; set; } = string.Empty;

    public string? ValueText { get; set; }

    public decimal? ValueNumber { get; set; }

    public DateTime? ValueDate { get; set; }

    public string? ValueBool { get; set; }

    public string? ExprText { get; set; }

    public string? ExprLevel { get; set; }
}
