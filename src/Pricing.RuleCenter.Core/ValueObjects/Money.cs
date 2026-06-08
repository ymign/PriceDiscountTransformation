namespace Pricing.RuleCenter.Core.ValueObjects;

/// <summary>
/// 金额值对象，封装 decimal 金额与统一取整逻辑。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// 项目中金额必须使用 decimal 类型（禁止 double/float），取整策略为：
/// 中间计算保留全部精度，最终金额保留 2 位小数、四舍五入（MidpointRounding.AwayFromZero）。
/// 本值对象封装金额和取整行为，防止散落在各处的取整逻辑不一致。
/// </para>
/// <para>
/// 【金额类型约束】
/// C# 始终使用 decimal，Oracle 使用 NUMBER(18,4)。
/// 禁止使用 double 或 float，否则会导致金额精度丢失。
/// </para>
/// <para>
/// 【取整策略】
/// 中间金额不提前取整；最终对外金额和落账金额统一保留 2 位小数、四舍五入。
/// 实现必须统一且可配置（roundMode、roundScale、minChargeQty、ceilBaseQty）。
/// </para>
/// <para>
/// 【值对象特性】
/// 本类型为 record，具有不可变性、值相等性和编译器生成的 Equals/GetHashCode。
/// 所有运算返回新的 Money 实例，不修改原实例。
/// </para>
/// </remarks>
public sealed record Money
{
    /// <summary>
    /// 金额数值，精度为 decimal（28-29 位有效数字）。
    /// </summary>
    /// <remarks>
    /// 存储时对应 Oracle NUMBER(18,4)，即最大 18 位整数 + 4 位小数。
    /// </remarks>
    public decimal Value { get; }

    /// <summary>
    /// 创建金额值对象。
    /// </summary>
    /// <param name="value">金额数值，可为正数、负数或零。</param>
    public Money(decimal value) => Value = value;

    /// <summary>
    /// 返回四舍五入后的金额，保留指定小数位数。
    /// </summary>
    /// <param name="scale">保留小数位数，默认为 2（对应金额的"分"精度）。</param>
    /// <returns>四舍五入后的新金额值对象。</returns>
    /// <remarks>
    /// 使用 MidpointRounding.AwayFromZero 策略，即"四舍五入"而非"银行家舍入"。
    /// 例如：1.005 → 1.01（而非 1.00）。
    /// </remarks>
    public Money Round(int scale = 2)
    {
        return new Money(Math.Round(Value, scale, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    /// 判断金额是否为零。
    /// </summary>
    /// <remarks>
    /// 用于判断是否需要执行金额相关的业务逻辑（如折价、限额校验）。
    /// </remarks>
    public bool IsZero => Value == 0m;

    /// <summary>
    /// 零金额单例，用于初始化或比较。
    /// </summary>
    public static Money Zero => new(0m);

    /// <summary>
    /// 金额加法运算。
    /// </summary>
    /// <param name="a">左操作数。</param>
    /// <param name="b">右操作数。</param>
    /// <returns>两金额之和。</returns>
    public static Money operator +(Money a, Money b) => new(a.Value + b.Value);

    /// <summary>
    /// 金额减法运算。
    /// </summary>
    /// <param name="a">左操作数。</param>
    /// <param name="b">右操作数。</param>
    /// <returns>两金额之差。</returns>
    public static Money operator -(Money a, Money b) => new(a.Value - b.Value);

    /// <summary>
    /// 金额乘法运算（金额 × 数量）。
    /// </summary>
    /// <param name="a">金额。</param>
    /// <param name="multiplier">乘数（数量）。</param>
    /// <returns>金额与数量的乘积。</returns>
    public static Money operator *(Money a, decimal multiplier) => new(a.Value * multiplier);

    /// <summary>
    /// 金额大于比较。
    /// </summary>
    public static bool operator >(Money a, Money b) => a.Value > b.Value;

    /// <summary>
    /// 金额小于比较。
    /// </summary>
    public static bool operator <(Money a, Money b) => a.Value < b.Value;

    /// <summary>
    /// 金额大于等于比较。
    /// </summary>
    public static bool operator >=(Money a, Money b) => a.Value >= b.Value;

    /// <summary>
    /// 金额小于等于比较。
    /// </summary>
    public static bool operator <=(Money a, Money b) => a.Value <= b.Value;

    /// <summary>
    /// 隐式转换：decimal → Money。
    /// </summary>
    /// <param name="value">decimal 金额值。</param>
    /// <remarks>
    /// 允许直接将 decimal 赋值给 Money 类型变量，简化调用代码。
    /// </remarks>
    public static implicit operator Money(decimal value) => new(value);

    /// <summary>
    /// 显式转换：Money → decimal。
    /// </summary>
    /// <param name="money">Money 值对象。</param>
    /// <remarks>
    /// 需要显式转换，提醒调用方注意从值对象退回到原始类型。
    /// </remarks>
    public static explicit operator decimal(Money money) => money.Value;

    /// <summary>
    /// 返回金额的字符串表示，格式为 4 位小数。
    /// </summary>
    /// <returns>格式化的金额字符串。</returns>
    public override string ToString() => Value.ToString("F4");
}
