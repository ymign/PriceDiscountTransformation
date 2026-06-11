namespace Pricing.RuleCenter.Application.Engine.Formula;

/// <summary>
/// 表达式公式求值上下文。
/// </summary>
/// <remarks>
/// 该上下文是表达式公式可访问变量的白名单。表达式不能直接访问完整 PricingContext，
/// 只能读取这里公开的 decimal 字段，防止配置公式越权读取患者、项目或其他对象。
/// </remarks>
public sealed class FormulaEvaluationContext
{
    /// <summary>原始录入数量。</summary>
    public decimal InputQty { get; init; }

    /// <summary>双单位换算后的数量。</summary>
    public decimal ConvertedQty { get; init; }

    /// <summary>数量限制后的最终计价数量。</summary>
    public decimal FinalQty { get; init; }

    /// <summary>权威项目单价。</summary>
    public decimal UnitPrice { get; init; }

    /// <summary>原始金额，通常为单价乘原始数量。</summary>
    public decimal OriginalAmount { get; init; }

    /// <summary>当前动作执行前的最终金额中间值。</summary>
    public decimal FinalAmount { get; init; }

    /// <summary>计价片段数量。</summary>
    public decimal PartCount { get; init; }

    /// <summary>计价片段面积汇总。</summary>
    public decimal Area { get; init; }

    /// <summary>
    /// 按白名单变量名读取表达式变量值。
    /// </summary>
    /// <param name="name">变量名，大小写敏感，必须是白名单字段。</param>
    /// <param name="value">变量值；未命中时为 0。</param>
    /// <returns>变量存在时返回 true。</returns>
    public bool TryGetVariable(string name, out decimal value)
    {
        // 变量名不做大小写兼容，目的是让公式配置保持稳定、可读，并在发布校验阶段暴露拼写错误。
        switch (name)
        {
            case "inputQty":
                value = InputQty;
                return true;
            case "convertedQty":
                value = ConvertedQty;
                return true;
            case "finalQty":
                value = FinalQty;
                return true;
            case "unitPrice":
                value = UnitPrice;
                return true;
            case "originalAmount":
                value = OriginalAmount;
                return true;
            case "finalAmount":
                value = FinalAmount;
                return true;
            case "partCount":
                value = PartCount;
                return true;
            case "area":
                value = Area;
                return true;
            default:
                value = 0m;
                return false;
        }
    }
}
