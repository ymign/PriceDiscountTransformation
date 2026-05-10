namespace Pricing.RuleCenter.Core.Services;

/// <summary>
/// 计价金额最终取整组件。
/// </summary>
/// <remarks>
/// 规则执行链中的公式、封顶和限额计算保留 decimal 全精度；只有对外响应和落账明细使用本组件统一保留
/// 两位小数，并按四舍五入口径处理。
/// </remarks>
public static class PricingAmountRounder
{
    /// <summary>
    /// 最终金额保留的小数位数。
    /// </summary>
    public const int FinalAmountScale = 2;

    /// <summary>
    /// 对最终对外金额或落账金额执行两位小数四舍五入。
    /// </summary>
    /// <param name="amount">待取整金额。</param>
    /// <returns>保留两位小数后的金额。</returns>
    public static decimal RoundFinalAmount(decimal amount)
    {
        return Math.Round(amount, FinalAmountScale, MidpointRounding.AwayFromZero);
    }
}
