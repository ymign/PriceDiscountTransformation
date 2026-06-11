using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine;

/// <summary>
/// 同组互斥占额草稿结算器。
/// </summary>
public sealed class SameGroupLimitOccupyValueFinalizer : ILimitOccupyValueFinalizer
{
    /// <summary>
    /// 只处理同组互斥占额。
    /// </summary>
    public bool CanHandle(LimitOccupy occupy)
    {
        return string.Equals(occupy.LimitType, "SAME_GROUP", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 同组互斥按“是否占一个名额”回填占额数量。
    /// </summary>
    public void Apply(LimitOccupy occupy, PricingContext context)
    {
        occupy.OccupyQty = context.FinalQty > 0 ? 1m : 0m;
        occupy.OccupyAmt = 0m;
    }
}
