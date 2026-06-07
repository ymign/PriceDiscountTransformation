namespace Pricing.RuleCenter.Core.Aggregates.Runtime;

public sealed class RuntimeCondition
{
    public long RuntimeConditionId { get; set; }

    public long RuntimeRuleId { get; set; }

    public string ConditionGroup { get; set; } = "DEFAULT";

    public string ConditionType { get; set; } = string.Empty;

    public string? OperatorType { get; set; }

    public string? LeftKey { get; set; }

    public string? RightValue { get; set; }

    public string? ParamsJson { get; set; }

    public int SortNo { get; set; }
}
