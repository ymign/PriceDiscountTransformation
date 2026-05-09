namespace Pricing.RuleCenter.Core.Models;

public sealed class PricingContext
{
    public string PatientId { get; set; } = string.Empty;
    public string? VisitId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string? ItemName { get; set; }
    public decimal InputQty { get; set; }
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public string? BodyPartCode { get; set; }
    public string? ChargeScene { get; set; }
    public DateTime BusinessChargeTime { get; set; }
    public string SourceSystem { get; set; } = string.Empty;
    public string? ChargeNo { get; set; }
    public string? BusinessRequestNo { get; set; }

    public IReadOnlyList<PricingPartItem>? PricingParts { get; set; }

    public IReadOnlyList<RuleHeader> MatchedRules { get; set; } = Array.Empty<RuleHeader>();
    public IReadOnlyList<RuleAction> OrderedActions { get; set; } = Array.Empty<RuleAction>();

    public decimal ConvertedQty { get; set; }
    public decimal FormulaAmount { get; set; }
    public decimal LimitedAmount { get; set; }
    public decimal FinalQty { get; set; }
    public decimal FinalAmount { get; set; }
    public decimal DiscountAmount { get; set; }

    public List<TraceStep> TraceSteps { get; set; } = new();
}

public sealed class PricingPartItem
{
    public string? PartCode { get; set; }
    public string? PartName { get; set; }
    public decimal Qty { get; set; }
    public decimal? Area { get; set; }
}

public sealed class TraceStep
{
    public int StepNo { get; set; }
    public string StepType { get; set; } = string.Empty;
    public string? StepDesc { get; set; }
    public decimal? InputValue { get; set; }
    public decimal? OutputValue { get; set; }
    public string? ParamsJson { get; set; }
}
