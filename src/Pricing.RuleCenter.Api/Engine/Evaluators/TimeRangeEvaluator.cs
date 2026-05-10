using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

/// <summary>
/// 业务时间范围条件评估器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】限制规则只在某段业务时间内生效。典型场景：
/// <list type="bullet">
///   <item><description>夜间急诊加收规则（22:00 ~ 次日 06:00）</description></item>
///   <item><description>节假日折价活动（某日期范围）</description></item>
///   <item><description>限时促销规则（开始日期 ~ 结束日期）</description></item>
/// </list>
/// </para>
/// <para>
/// 【时间格式】RightValue 使用"开始时间~结束时间"的文本格式，以波浪号（~）分隔。
/// 支持标准 DateTime 格式（如 "2026-01-01 00:00:00~2026-12-31 23:59:59"）。
/// </para>
/// <para>
/// 【业务时间优先】比较对象是 BusinessChargeTime（HIS 业务发生时间），
/// 而不是服务器当前时间（DateTime.UtcNow）。这确保：
/// <list type="bullet">
///   <item><description>补录历史费用时按历史规则时间窗口命中</description></item>
///   <item><description>2 小时窗口按收费时间（包含重收时间）向前查 2 小时</description></item>
///   <item><description>单日累计按业务日期统计，而非技术操作日期</description></item>
/// </list>
/// </para>
/// <para>
/// 【与 IsInEffectiveRange 的区别】
/// <list type="bullet">
///   <item><description>IsInEffectiveRange（RuleMatchService 中）：校验规则主档的生效区间（年/月级别）</description></item>
///   <item><description>本评估器：校验条件级别的业务时间范围（小时/分钟级别，更细粒度）</description></item>
/// </list>
/// 两者互补：规则可能在生效期内，但条件限制只在特定时段触发。
/// </para>
/// </remarks>
public sealed class TimeRangeEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码，对应 PR_RULE_CONDITION.CONDITION_TYPE = "TIME_RANGE"。
    /// </summary>
    public string ConditionType => "TIME_RANGE";

    /// <summary>
    /// 判断本次业务收费时间是否落在规则配置的时间范围内。
    /// </summary>
    /// <param name="condition">
    /// 规则条件配置。RightValue 格式为"开始时间~结束时间"（波浪号分隔）。
    /// </param>
    /// <param name="context">
    /// 计价上下文。BusinessChargeTime 表示 HIS 业务发生时间，非服务器时间。
    /// </param>
    /// <returns>
    /// 业务时间落在闭区间 [from, to] 内时返回 <c>true</c>。
    /// RightValue 为空时返回 <c>true</c>（不限制时间）。
    /// 解析失败时返回 <c>false</c>（配置错误保守处理）。
    /// </returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 时间范围为空时视为不限制时间，避免可选条件缺失导致规则整体失效。
        // 典型场景：某规则不区分时段，全天有效。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // ========== 第一阶段：拆分开始和结束时间 ==========
        // 只按第一个 ~ 拆分（count=2），允许时间文本中保留常见空格格式。
        // 例如 "2026-01-01 00:00:00~2026-12-31 23:59:59" 拆分为两段。
        var parts = condition.RightValue.Split('~', 2);
        if (parts.Length != 2)
        {
            // 格式不正确（缺少分隔符），按不命中处理。
            // 配置错误应在规则发布校验阶段发现，运行期做保守处理。
            return false;
        }

        // ========== 第二阶段：解析时间文本 ==========
        // 解析失败按不命中处理，不抛异常中断整条计价链路。
        // 配置错误可通过追溯查询（TraceStep）和发布校验发现。
        if (!DateTime.TryParse(parts[0].Trim(), out var from) ||
            !DateTime.TryParse(parts[1].Trim(), out var to))
        {
            return false;
        }

        // ========== 第三阶段：按闭区间判断 ==========
        // 使用业务收费时间比较，确保补录历史费用时按历史规则时间窗口命中。
        // 闭区间 [from, to]：from <= BusinessChargeTime <= to。
        return context.BusinessChargeTime >= from && context.BusinessChargeTime <= to;
    }
}
