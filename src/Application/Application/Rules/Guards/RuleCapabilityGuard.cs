using Newtonsoft.Json.Linq;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Engine.Formula;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.Rules.Guards;

/// <summary>
/// 规则能力可表达性门禁。
/// </summary>
public sealed class RuleCapabilityGuard
{
    private const string ExpressionParamName = "expression";

    private readonly RuleCapabilityRegistry _capabilityRegistry;
    private readonly FormulaExpressionValidator _expressionValidator;

    /// <summary>
    /// 初始化规则能力可表达性门禁。
    /// </summary>
    public RuleCapabilityGuard(
        RuleCapabilityRegistry capabilityRegistry,
        FormulaExpressionValidator expressionValidator)
    {
        _capabilityRegistry = capabilityRegistry;
        _expressionValidator = expressionValidator;
    }

    /// <summary>
    /// 确认条件和动作均可由当前引擎表达。
    /// </summary>
    public void EnsureSupported(
        IReadOnlyList<RuleCondition> conditions,
        IReadOnlyList<RuleAction> actions)
    {
        foreach (var condition in conditions.Where(c => c.IsEnabled == EnableFlag.Yes))
        {
            if (!_capabilityRegistry.SupportsConditionType(condition.ConditionType))
            {
                throw new BizException(
                    BizErrorCode.RuleConditionUnsupported,
                    409,
                    $"不支持的规则条件类型: {condition.ConditionType}");
            }
        }

        foreach (var action in actions.Where(a => a.IsEnabled == EnableFlag.Yes))
        {
            EnsureActionSupported(action);
        }
    }

    private void EnsureActionSupported(RuleAction action)
    {
        if (!_capabilityRegistry.SupportsActionType(action.ActionType))
        {
            throw new BizException(
                BizErrorCode.RuleActionUnsupported,
                409,
                $"不支持的规则动作类型: {action.ActionType}");
        }

        if (!string.Equals(action.ActionType, RuleActionTypeCodes.FormulaCalc, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!_capabilityRegistry.SupportsFormulaExecutorCode(action.ExecutorCode))
        {
            throw new BizException(
                BizErrorCode.RuleCapabilityUnsupported,
                409,
                $"不支持的公式执行器: {action.ExecutorCode}");
        }

        if (FormulaExecutorCodes.IsExpressionFormula(action.ExecutorCode))
        {
            EnsureExpressionSupported(action);
        }
    }

    private void EnsureExpressionSupported(RuleAction action)
    {
        var expressionParams = ParseParams(action.ParamsJson);
        var expression = expressionParams?.TryGetValue(ExpressionParamName, StringComparison.OrdinalIgnoreCase, out var token) == true
            ? token.ToString()
            : null;

        try
        {
            _expressionValidator.Validate(expression);
        }
        catch (InvalidOperationException ex)
        {
            throw new BizException(
                BizErrorCode.RuleFormulaUnsupported,
                409,
                $"表达式公式不支持: {ex.Message}");
        }
    }

    private static JObject? ParseParams(string? paramsJson)
    {
        if (string.IsNullOrWhiteSpace(paramsJson))
        {
            return null;
        }

        try
        {
            return JObject.Parse(paramsJson);
        }
        catch
        {
            throw new BizException(
                BizErrorCode.RuleFormulaUnsupported,
                409,
                "表达式公式参数不是合法 JSON");
        }
    }
}
