using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 请求内限额累计器。
/// </summary>
/// <remarks>
/// simulate 和 confirm 都需要让“同一请求前序明细产生的占额”影响后续明细。
/// 该类型集中维护这套口径，避免多个 workflow 各自复制同一段累计逻辑。
/// </remarks>
internal static class PricingInRequestLimitAccumulator
{
    /// <summary>
    /// 把当前明细产出的占额候选累加到请求内上下文。
    /// </summary>
    public static void Accumulate(
        Dictionary<string, decimal> inRequestOccupiedQtyByLimitDimension,
        List<LimitOccupy> inRequestLimitOccupies,
        PricingResult result)
    {
        foreach (var occupy in result.LimitOccupies.Where(o =>
                     !string.IsNullOrWhiteSpace(o.LimitType) &&
                     !string.IsNullOrWhiteSpace(o.LimitDimensionCode)))
        {
            var key = $"{occupy.LimitType.Trim().ToUpperInvariant()}:{occupy.LimitDimensionCode?.Trim().ToUpperInvariant()}";
            inRequestOccupiedQtyByLimitDimension.TryGetValue(key, out var existingQty);
            inRequestOccupiedQtyByLimitDimension[key] = existingQty + occupy.OccupyQty;
            inRequestLimitOccupies.Add(occupy);
        }
    }
}
