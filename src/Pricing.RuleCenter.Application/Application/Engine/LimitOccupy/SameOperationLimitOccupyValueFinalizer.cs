using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 同手术封顶占额草稿结算器。
/// </summary>
public sealed class SameOperationLimitOccupyValueFinalizer : ILimitOccupyValueFinalizer
{
    /// <summary>
    /// 只处理同手术封顶占额。
    /// </summary>
    public bool CanHandle(LimitOccupy occupy)
    {
        return string.Equals(occupy.LimitType, "SAME_OPERATION", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 同手术封顶只累计金额，不累计数量。
    /// </summary>
    public void Apply(LimitOccupy occupy, PricingContext context)
    {
        occupy.OccupyQty = 0m;
        occupy.OccupyAmt = context.FinalAmount;
    }
}
