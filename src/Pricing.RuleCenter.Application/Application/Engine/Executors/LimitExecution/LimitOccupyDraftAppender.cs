using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 限额占额草稿追加器。
/// </summary>
internal static class LimitOccupyDraftAppender
{
    /// <summary>
    /// 向当前计价上下文追加占额草稿，并按维度去重。
    /// </summary>
    public static void AddDraft(
        PricingContext context,
        string limitType,
        string limitKey,
        string dimensionCode,
        bool requirePositiveFinalQty = false)
    {
        if (requirePositiveFinalQty && context.FinalQty <= 0)
        {
            return;
        }

        if (HasDraft(context, limitType, dimensionCode))
        {
            return;
        }

        context.PendingLimitOccupies.Add(new LimitOccupy
        {
            PatientId = context.PatientId,
            ItemCode = context.ItemCode,
            LimitType = limitType,
            LimitKey = limitKey,
            LimitDimensionCode = dimensionCode,
            BusinessChargeTime = context.BusinessChargeTime,
            OccupyType = "CHARGE"
        });
    }

    private static bool HasDraft(PricingContext context, string limitType, string dimensionCode)
    {
        return context.PendingLimitOccupies.Any(occupy =>
            string.Equals(occupy.LimitType, limitType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(occupy.LimitDimensionCode, dimensionCode, StringComparison.OrdinalIgnoreCase));
    }
}
