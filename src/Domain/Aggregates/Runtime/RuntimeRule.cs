using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Runtime;

public sealed class RuntimeRule
{
    public long RuntimeRuleId { get; set; }

    public long PackageId { get; set; }

    public long SourceTemplateVersionId { get; set; }

    public long SourcePolicyVersionId { get; set; }

    public string CapabilityFamily { get; set; } = string.Empty;

    public string MergeMode { get; set; } = RuntimeMergeModeCodes.SingleWinner;

    public string? TargetItemCode { get; set; }

    public string? TargetGroupCode { get; set; }

    public string ScopeLevel { get; set; } = string.Empty;

    public string PriorityKey { get; set; } = string.Empty;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public string MatchKey { get; set; } = string.Empty;
}
