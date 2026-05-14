namespace Pricing.RuleCenter.Core.ValueObjects;

/// <summary>
/// 生效期值对象，封装生效开始时间和结束时间以及相关判断逻辑。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// 规则和版本的生效期由 EffectiveFrom + EffectiveTo 两个可空日期组成，
/// 判断是否生效、是否与另一生效期重叠等逻辑散落在多处。
/// 本值对象统一封装这些行为。
/// </para>
/// <para>
/// 【空值语义】
/// NULL 表示无限制：
///   - From 为空 = 从创建时起生效（无开始限制）
///   - To 为空 = 长期有效（无结束限制）
///   - 两个都为空 = 任何时间点都生效
/// </para>
/// <para>
/// 【业务时间优先】
/// 生效判断必须使用业务收费发生时间（BusinessChargeTime），
/// 不能用 DateTime.Now 或 RequestAt 替代。
/// 否则规则生效判断会受服务器时钟漂移影响，导致计价结果不稳定。
/// </para>
/// <para>
/// 【值对象特性】
/// 本类型为 record，具有不可变性、值相等性和编译器生成的 Equals/GetHashCode。
/// 所有运算返回新的 EffectivePeriod 实例，不修改原实例。
/// </para>
/// </remarks>
public sealed record EffectivePeriod
{
    /// <summary>
    /// 生效开始时间，业务收费时间早于该时间的请求不匹配该规则。
    /// </summary>
    /// <remarks>
    /// 空值表示无生效开始时间限制（从创建时起生效）。
    /// </remarks>
    public DateTime? From { get; init; }

    /// <summary>
    /// 生效结束时间，业务收费时间晚于该时间的请求不匹配该规则。
    /// </summary>
    /// <remarks>
    /// 空值表示未设失效时间（长期有效）。
    /// </remarks>
    public DateTime? To { get; init; }

    /// <summary>
    /// 创建生效期值对象。
    /// </summary>
    /// <param name="From">生效开始时间，空值表示无开始限制。</param>
    /// <param name="To">生效结束时间，空值表示无结束限制。</param>
    public EffectivePeriod(DateTime? From, DateTime? To)
    {
        this.From = From;
        this.To = To;
    }

    /// <summary>
    /// 判断指定业务时间是否在生效期内。
    /// </summary>
    /// <param name="businessTime">业务收费发生时间，不得使用技术时间替代。</param>
    /// <returns>生效期内返回 true，否则返回 false。</returns>
    /// <remarks>
    /// <para>
    /// 【边界条件】
    ///   - From 非空且 businessTime &lt; From：未生效，返回 false
    ///   - To 非空且 businessTime &gt; To：已失效，返回 false
    ///   - 其他情况：生效中，返回 true
    /// </para>
    /// <para>
    /// 【业务时间优先】
    /// 参数必须是 BusinessChargeTime（调用方传入的业务收费时间），
    /// 不能用 DateTime.Now 或 RequestAt 替代。
    /// </para>
    /// </remarks>
    public bool IsEffectiveAt(DateTime businessTime)
    {
        if (From.HasValue && businessTime < From.Value)
            return false;
        if (To.HasValue && businessTime > To.Value)
            return false;
        return true;
    }

    /// <summary>
    /// 判断当前生效期是否与另一生效期存在重叠。
    /// </summary>
    /// <param name="other">另一生效期。</param>
    /// <returns>存在重叠返回 true，否则返回 false。</returns>
    /// <remarks>
    /// <para>
    /// 【重叠判断逻辑】
    /// 两个区间 [thisStart, thisEnd] 和 [otherStart, otherEnd] 重叠的条件：
    /// thisStart &lt;= otherEnd AND otherStart &lt;= thisEnd
    /// </para>
    /// <para>
    /// 【空值处理】
    ///   - From 为空时，使用 DateTime.MinValue 作为起点
    ///   - To 为空时，使用 DateTime.MaxValue 作为终点
    /// </para>
    /// <para>
    /// 【用途】
    /// 用于规则冲突校验：同一项目、同一场景、同一生效期内不允许多套折价公式或不同折价额度规则同时生效。
    /// </para>
    /// </remarks>
    public bool Overlaps(EffectivePeriod other)
    {
        var thisStart = From ?? DateTime.MinValue;
        var thisEnd = To ?? DateTime.MaxValue;
        var otherStart = other.From ?? DateTime.MinValue;
        var otherEnd = other.To ?? DateTime.MaxValue;
        return thisStart <= otherEnd && otherStart <= thisEnd;
    }

    /// <summary>
    /// 返回生效期的字符串表示，格式为 "[开始日期 ~ 结束日期]"。
    /// </summary>
    /// <returns>格式化的生效期字符串，空值显示为 "∞"。</returns>
    public override string ToString()
    {
        var from = From?.ToString("yyyy-MM-dd") ?? "∞";
        var to = To?.ToString("yyyy-MM-dd") ?? "∞";
        return $"[{from} ~ {to}]";
    }
}
