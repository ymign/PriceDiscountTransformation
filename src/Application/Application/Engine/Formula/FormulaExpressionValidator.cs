namespace Pricing.RuleCenter.Core.Engine.Formula;

/// <summary>
/// 表达式公式发布前校验器。
/// </summary>
public sealed class FormulaExpressionValidator
{
    private readonly FormulaExpressionEvaluator _evaluator;

    /// <summary>
    /// 初始化表达式公式校验器。
    /// </summary>
    public FormulaExpressionValidator(FormulaExpressionEvaluator evaluator)
    {
        _evaluator = evaluator;
    }

    /// <summary>
    /// 校验表达式是否只使用白名单变量、函数和受控运算符。
    /// </summary>
    public void Validate(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new InvalidOperationException("表达式不能为空");
        }

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
