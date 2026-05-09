namespace Pricing.RuleCenter.Core.Models;

public sealed class PricingResult
{
    public bool IsSpecialItem { get; set; }
    public decimal InputQty { get; set; }
    public decimal FinalQty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal FinalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public IReadOnlyList<TraceStep> TraceSteps { get; set; } = Array.Empty<TraceStep>();
    public IReadOnlyList<long> MatchedRuleIds { get; set; } = Array.Empty<long>();
}
