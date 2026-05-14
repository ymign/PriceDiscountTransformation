namespace Pricing.RuleCenter.Core.Services;

/// <summary>
/// 计价金额取整领域服务，统一金额取整策略。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// 项目中金额必须使用 decimal 类型（禁止 double/float），取整策略必须统一。
/// 本服务封装取整逻辑，防止散落在各处的取整逻辑不一致导致资金偏差。
/// </para>
/// <para>
/// 【业务规则】
/// 中间计算保留全部精度，最终金额保留 2 位小数，四舍五入（MidpointRounding.AwayFromZero）。
/// </para>
/// <para>
/// 【取整策略】
/// 使用 MidpointRounding.AwayFromZero，即"四舍五入"而非"银行家舍入"。
/// 例如：1.005 → 1.01（而非 1.00）。
/// </para>
/// <para>
/// 【金额类型约束】
/// C# 始终使用 decimal，Oracle 使用 NUMBER(18,4)。
/// 禁止使用 double 或 float，否则会导致金额精度丢失。
/// </para>
/// <para>
/// 【静态类设计】
/// 本服务为静态类，因为：
///   - 无状态依赖
///   - 纯函数式设计（输入 → 输出，无副作用）
///   - 可全局调用，无需 DI 注册
/// </para>
/// </remarks>
public static class PricingAmountRounder
{
    /// <summary>
    /// 对金额执行最终取整（保留指定小数位数，四舍五入）。
    /// </summary>
    /// <param name="amount">待取整的金额，精度为 decimal。</param>
    /// <param name="scale">保留小数位数，默认为 2（对应金额的"分"精度）。</param>
    /// <returns>取整后的金额。</returns>
    /// <remarks>
    /// <para>
    /// 【调用时机】
    /// 仅在最终金额输出前调用，中间计算过程不得提前取整。
    /// 例如：
    ///   - 计价引擎计算完成后，输出最终折价金额前
    ///   - 写入数据库前，确保金额精度一致
    ///   - 返回给渠道前，确保金额格式规范
    /// </para>
    /// <para>
    /// 【取整策略】
    /// 使用 MidpointRounding.AwayFromZero，即"四舍五入"。
    /// 例如：
    ///   - 1.005 → 1.01
    ///   - 1.004 → 1.00
    ///   - -1.005 → -1.01（负数同样四舍五入）
    /// </para>
    /// <para>
    /// 【精度约束】
    /// 输入金额精度不受限制，输出精度由 scale 参数控制。
    /// Oracle 存储精度为 NUMBER(18,4)，建议 scale 不超过 4。
    /// </para>
    /// </remarks>
    public static decimal RoundFinal(decimal amount, int scale = 2)
    {
        return Math.Round(amount, scale, MidpointRounding.AwayFromZero);
    }
}
