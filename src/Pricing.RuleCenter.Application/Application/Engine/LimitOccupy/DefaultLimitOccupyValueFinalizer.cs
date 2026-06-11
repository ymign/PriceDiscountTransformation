using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine;

/// <summary>
/// 默认占额草稿结算器。
/// </summary>
public sealed class DefaultLimitOccupyValueFinalizer : ILimitOccupyValueFinalizer
{
    /// <summary>
    /// 默认结算器只作为兜底实现使用。
    /// </summary>
    public bool IsFallback => true;

    /// <summary>
    /// 默认结算器接受任意限额类型，由引擎在没有专项结算器命中时回退使用。
    /// </summary>
    public bool CanHandle(LimitOccupy occupy)
    {
        return true;
    }

    /// <summary>
    /// 默认按最终数量和最终金额回填占额值。
    /// </summary>
    public void Apply(LimitOccupy occupy, PricingContext context)
    {
        occupy.OccupyQty = context.FinalQty;
        occupy.OccupyAmt = context.FinalAmount;
    }
}
