namespace Pricing.RuleCenter.Application.Engine.Formula;

/// <summary>
/// 表达式公式发布前校验器。
/// </summary>
/// <remarks>
/// 发布前校验通过试运行表达式，确保只使用白名单变量、函数和受控运算符。
/// 这样运行期计价时不必再承担“公式语法是否安全”的主要发现成本。
/// </remarks>
public sealed class FormulaExpressionValidator
{
    /// <summary>
    /// 受控表达式求值器。
    /// </summary>
    private readonly FormulaExpressionEvaluator _evaluator;

    /// <summary>
    /// 初始化表达式公式校验器。
    /// </summary>
    /// <param name="evaluator">受控表达式求值器。</param>
    public FormulaExpressionValidator(FormulaExpressionEvaluator evaluator)
    {
        _evaluator = evaluator;
    }

    /// <summary>
    /// 校验表达式是否只使用白名单变量、函数和受控运算符。
    /// </summary>
    /// <param name="expression">表达式文本。</param>
    public void Validate(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new InvalidOperationException("表达式不能为空");
        }

        // 使用非零样例上下文试算，尽量覆盖除法、函数和变量解析。
        // 真正业务数值仍在运行期由 PricingContext 投影提供。
        _evaluator.Evaluate(expression, new FormulaEvaluationContext
        {
            InputQty = 1m,
            ConvertedQty = 1m,
            FinalQty = 1m,
            UnitPrice = 1m,
            OriginalAmount = 1m,
            FinalAmount = 1m,
            PartCount = 1m,
            Area = 1m
        });
    }
}
