using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 限额执行通用辅助方法。
/// </summary>
internal static class LimitExecutionSupport
{
    /// <summary>
    /// 按剩余可收费数量截断当前明细的数量和金额。
    /// </summary>
    public static void ApplyRemainingQty(PricingContext context, decimal remainingQty)
    {
        if (remainingQty <= 0)
        {
            context.FinalQty = 0;
            context.FinalAmount = 0;
            return;
        }

        if (context.FinalQty <= remainingQty)
        {
            return;
        }

        var beforeQty = context.FinalQty;
        context.FinalQty = remainingQty;
        context.FinalAmount = ScaleAmountByQty(context.FinalAmount, beforeQty, remainingQty);
    }

    /// <summary>
    /// 按数量比例缩放金额，保留中间精度，不提前取整。
    /// </summary>
    public static decimal ScaleAmountByQty(decimal amount, decimal beforeQty, decimal afterQty)
    {
        return beforeQty == 0 ? 0 : amount * afterQty / beforeQty;
    }
}
