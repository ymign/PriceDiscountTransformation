using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Engine.Formula;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.Rules.Guards;

/// <summary>
/// 规则动作参数校验器。
/// </summary>
public sealed class RuleActionParameterValidator
{
    private const string ExpressionParamName = "expression";

    private readonly FormulaExpressionValidator _expressionValidator;

    /// <summary>
    /// 初始化规则动作参数校验器。
    /// </summary>
    public RuleActionParameterValidator(FormulaExpressionValidator expressionValidator)
    {
        _expressionValidator = expressionValidator;
    }

    /// <summary>
    /// 校验动作参数 JSON 和动作类型必填参数。
    /// </summary>
    public void Validate(IReadOnlyList<RuleAction> actions)
    {
        foreach (var action in actions.Where(a => a.IsEnabled == EnableFlag.Yes))
        {
            Validate(action, _expressionValidator);
        }
    }

    private static void Validate(RuleAction action, FormulaExpressionValidator expressionValidator)
    {
        var json = ParseParams(action.ParamsJson);
        switch (action.ActionType?.Trim().ToUpperInvariant())
        {
            case RuleActionTypeCodes.ApplyTimeWindowLimit:
                if (!HasPositiveNumber(json, "limitQty", "maxQty") ||
                    !HasPositiveNumber(json, "windowMinutes", "windowHours"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 limitQty/maxQty 或 windowMinutes/windowHours");
                }
                break;

            case RuleActionTypeCodes.ApplyDayLimitQty:
                if (!HasPositiveNumber(json, "maxDailyQty"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 maxDailyQty");
                }
                break;

            case RuleActionTypeCodes.ApplyOnceLimitQty:
                if (!HasNonNegativeNumber(json, "maxOnceQty", "maxQty"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 maxOnceQty/maxQty");
                }
                break;

            case RuleActionTypeCodes.ApplyMaxAmount:
                if (!HasNonNegativeNumber(json, "maxAmount", "ceilingAmount"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 maxAmount/ceilingAmount");
                }
                break;

            case RuleActionTypeCodes.ApplyMinAmount:
                if (!HasNonNegativeNumber(json, "minAmount", "floorAmount"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 minAmount/floorAmount");
                }
                break;

            case RuleActionTypeCodes.FormulaCalc:
                ValidateFormulaAction(action, json, expressionValidator);
                break;
        }
    }

    private static void ValidateFormulaAction(
        RuleAction action,
        JObject? json,
        FormulaExpressionValidator expressionValidator)
    {
        if (!FormulaExecutorCodes.IsExpressionFormula(action.ExecutorCode))
        {
            return;
        }

        var expression = TryGetString(json, ExpressionParamName);
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new BizException(
                BizErrorCode.ActionParamMissing,
                409,
                "EXPRESSION_FORMULA 缺少 expression");
        }

        try
        {
            expressionValidator.Validate(expression);
        }
        catch (InvalidOperationException ex)
        {
            throw new BizException(
                BizErrorCode.ActionParamInvalid,
                409,
                $"EXPRESSION_FORMULA 表达式非法: {ex.Message}");
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
                BizErrorCode.ActionParamInvalid,
                409,
                "动作参数不是合法 JSON");
        }
    }

    private static bool HasPositiveNumber(JObject? json, params string[] keys)
    {
        return TryGetNumber(json, keys, out var value) && value > 0m;
    }

    private static bool HasNonNegativeNumber(JObject? json, params string[] keys)
    {
        return TryGetNumber(json, keys, out var value) && value >= 0m;
    }

    private static bool TryGetNumber(JObject? json, string[] keys, out decimal value)
    {
        value = 0m;
        if (json is null)
        {
            return false;
        }

        foreach (var key in keys)
        {
            if (json.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var token) &&
                token.Type != JTokenType.Null &&
                decimal.TryParse(token.ToString(), out value))
            {
                return true;
            }
        }

        return false;
    }

    private static string? TryGetString(JObject? json, string key)
    {
        if (json is null)
        {
            return null;
        }

        return json.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var token) &&
            token.Type != JTokenType.Null
                ? token.ToString()
                : null;
    }
}
