namespace Pricing.RuleCenter.Core.Engine.Formula;

/// <summary>
/// 表达式公式求值上下文。
/// </summary>
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
    public bool TryGetVariable(string name, out decimal value)
    {
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
