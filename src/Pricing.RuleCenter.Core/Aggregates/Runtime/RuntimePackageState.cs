using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Core.Aggregates.Runtime;

public sealed class RuntimePackageState
{
    public string StateCode { get; set; } = RuntimePackageStateCodes.Active;

    public long ActivePackageId { get; set; }

    public long ActivePackageVersion { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
