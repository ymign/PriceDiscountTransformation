using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Application.Engine.Formula;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Executors;

/// <summary>
/// 表达式公式执行器。
/// </summary>
/// <remarks>
/// <para>
/// 表达式公式用于承载规则配置中无法用固定公式执行器覆盖的简单金额公式。
/// 它仍然运行在受控表达式解析器中，只允许白名单变量、函数和四则运算，不执行 C# 代码或脚本。
/// </para>
/// <para>
/// 与其他公式执行器一样，本执行器共享 ActionType=FORMULA_CALC，并通过 ExecutorCode=EXPRESSION_FORMULA 做二级分派。
/// </para>
/// </remarks>
public sealed class ExpressionFormulaExecutor : IRuleActionExecutor, IFormulaExecutorCapabilityMetadata
{
    /// <summary>
    /// 受控表达式求值器。
    /// </summary>
    private readonly FormulaExpressionEvaluator _evaluator;

    /// <inheritdoc />
    public IReadOnlyCollection<string> SupportedExecutorCodes { get; } = new[]
    {
        FormulaExecutorCodes.ExpressionFormula
    };

    /// <summary>
    /// 初始化表达式公式执行器。
    /// </summary>
    /// <param name="evaluator">受控表达式求值器。</param>
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
    /// <param name="action">规则动作。</param>
    /// <returns>ExecutorCode 为表达式公式编码时返回 true。</returns>
    public bool CanHandle(RuleAction action)
    {
        return FormulaExecutorCodes.IsExpressionFormula(action.ExecutorCode);
    }

    /// <summary>
    /// 执行表达式公式，并写入公式金额和最终金额。
    /// </summary>
    /// <param name="action">规则动作配置，ParamsJson 必须包含 expression。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>已完成任务。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // FORMULA_CALC 是公式动作大类，不匹配本执行器编码时交给其他公式执行器处理。
        if (!CanHandle(action))
        {
            return Task.CompletedTask;
        }

        var param = DeserializeParams(action.ParamsJson);
        if (param is null || string.IsNullOrWhiteSpace(param.Expression))
        {
            // 表达式公式缺少 expression 属于资金动作配置错误，必须 STOP，不能静默跳过。
            throw new InvalidOperationException(
                $"EXPRESSION_FORMULA 缺少 expression 参数: ActionId={action.ActionId}");
        }

        // 表达式只读取受控上下文变量，不允许访问外部对象或执行脚本。
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
        // 只把公式需要的数值字段投影给表达式解析器，避免表达式访问患者、项目等非数值业务对象。
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

        return RuleCenterJsonSerializer.Deserialize<ExpressionFormulaParams>(json);
    }

    private sealed class ExpressionFormulaParams
    {
        /// <summary>
        /// 受控表达式文本。
        /// </summary>
        public string? Expression { get; set; }

        /// <summary>
        /// 公式结果写入的金额字段。为空或 FinalAmount 时写入最终金额；其他值仅更新 FormulaAmount。
        /// </summary>
        public string? AmountField { get; set; }
    }
}
