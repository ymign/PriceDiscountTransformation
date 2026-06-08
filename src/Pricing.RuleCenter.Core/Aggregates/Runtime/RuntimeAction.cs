using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Runtime;

public sealed class RuntimeAction
{
    public long RuntimeActionId { get; set; }

    public long RuntimeRuleId { get; set; }

    public int StepNo { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string ExecutorCode { get; set; } = string.Empty;

    public string? ParamsJson { get; set; }

    public string? ExclusiveGroup { get; set; }

    public int SortNo { get; set; }

    public string OnError { get; set; } = ActionOnErrorCodes.Stop;
}
