using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Templates;

public sealed class TemplateAggregate
{
    public long TemplateId { get; set; }

    public string TemplateCode { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string RiskLevel { get; set; } = string.Empty;

    public string ExpressionMode { get; set; } = string.Empty;

    public string Status { get; set; } = TemplateLifecycleCodes.Draft;

    public int CurrentVersionNo { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
}
