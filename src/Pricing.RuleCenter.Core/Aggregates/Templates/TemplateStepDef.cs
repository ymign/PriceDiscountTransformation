using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Templates;

public sealed class TemplateStepDef
{
    public long StepDefId { get; set; }

    public long TemplateVersionId { get; set; }

    public int StepNo { get; set; }

    public string StepKind { get; set; } = string.Empty;

    public string CapabilityCode { get; set; } = string.Empty;

    public string? ActionType { get; set; }

    public string? ExecutorCode { get; set; }

    public string OnError { get; set; } = ActionOnErrorCodes.Stop;

    public string? StepConfigClob { get; set; }
}
