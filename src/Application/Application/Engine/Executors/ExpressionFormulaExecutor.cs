using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Engine.Formula;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 表达式公式执行器。
/// </summary>
public sealed class ExpressionFormulaExecutor : IRuleActionExecutor
{
    private readonly FormulaExpressionEvaluator _evaluator;

    /// <summary>
    /// 初始化表达式公式执行器。
    /// </summary>
    public ExpressionFormulaExecutor(FormulaExpressionEvaluator evaluator)
    {
        _evaluator = evaluator;
    }

    /// <summary>
    /// 获取动作类型编码。表达式公式与其他公式执行器共享 FORMULA_CALC。
    /// </summary>
    public string ActionType => RuleActionTypeCodes.FormulaCalc;

    /// <summary>
    /// 判断当前动作是否由表达式公式执行器处理。
    /// </summary>
    public bool CanHandle(RuleAction action)
    {
        return FormulaExecutorCodes.IsExpressionFormula(action.ExecutorCode);
    }

    /// <summary>
    /// 执行表达式公式，并写入公式金额和最终金额。
    /// </summary>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        if (!CanHandle(action))
        {
            return Task.CompletedTask;
        }

        var param = DeserializeParams(action.ParamsJson);
        if (param is null || string.IsNullOrWhiteSpace(param.Expression))
        {
            throw new InvalidOperationException(
                $"EXPRESSION_FORMULA 缺少 expression 参数: ActionId={action.ActionId}");
        }

        var amount = _evaluator.Evaluate(param.Expression, BuildContext(context));
        context.FormulaAmount = amount;
        if (string.IsNullOrWhiteSpace(param.AmountField) ||
            string.Equals(param.AmountField, "FinalAmount", StringComparison.OrdinalIgnoreCase))
        {
            context.FinalAmount = amount;
        }

        return Task.CompletedTask;
    }

    private static FormulaEvaluationContext BuildContext(PricingContext context)
    {
        return new FormulaEvaluationContext
        {
            InputQty = context.InputQty,
            ConvertedQty = context.ConvertedQty,
            FinalQty = context.FinalQty,
            UnitPrice = context.UnitPrice,
            OriginalAmount = context.UnitPrice * context.InputQty,
            FinalAmount = context.FinalAmount,
            PartCount = context.PricingParts?.Count ?? 0,
            Area = context.PricingParts?.Sum(p => p.Area ?? 0m) ?? 0m
        };
    }

    private static ExpressionFormulaParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<ExpressionFormulaParams>(json);
    }

    private sealed class ExpressionFormulaParams
    {
        public string? Expression { get; set; }

        public string? AmountField { get; set; }
    }
}
