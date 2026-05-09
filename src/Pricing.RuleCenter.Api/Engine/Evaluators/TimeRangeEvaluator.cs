using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

/// <summary>
/// 业务时间范围条件评估器。
/// </summary>
/// <remarks>
/// 该评估器用于限制规则只在某段业务时间内生效。RightValue 使用“开始~结束”的文本格式，
/// 比较对象是 BusinessChargeTime，而不是服务器当前时间。
/// </remarks>
public sealed class TimeRangeEvaluator : IRuleConditionEvaluator
{
    /// <summary>
    /// 获取条件类型编码。
    /// </summary>
    public string ConditionType => "TIME_RANGE";

    /// <summary>
    /// 判断本次业务收费时间是否落在规则配置的时间范围内。
    /// </summary>
    /// <param name="condition">规则条件，RightValue 格式为“开始时间~结束时间”。</param>
    /// <param name="context">计价上下文，BusinessChargeTime 表示 HIS 业务发生时间。</param>
    /// <returns>业务时间落在闭区间内时返回 <c>true</c>。</returns>
    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        // 时间范围为空时视为不限制时间，避免可选条件缺失导致规则整体失效。
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        // ========== 第一阶段：拆分开始和结束时间 ==========
        // 只按第一个 ~ 拆分，允许时间文本中保留常见空格格式。
        var parts = condition.RightValue.Split('~', 2);
        if (parts.Length != 2)
        {
            return false;
        }

        // ========== 第二阶段：解析时间文本 ==========
        // 解析失败按不命中处理，不抛异常中断整条计价链路；配置错误可通过追踪和发布校验发现。
        if (!DateTime.TryParse(parts[0].Trim(), out var from) ||
            !DateTime.TryParse(parts[1].Trim(), out var to))
        {
            return false;
        }

        // ========== 第三阶段：按闭区间判断 ==========
        // 使用业务收费时间比较，确保补录历史费用时按历史规则时间窗口命中。
        return context.BusinessChargeTime >= from && context.BusinessChargeTime <= to;
    }
}
