using System.Text.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Application.Engine.Formula;
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
        using var json = ParseParams(action.ParamsJson);
        var root = json?.RootElement;
        switch (action.ActionType?.Trim().ToUpperInvariant())
        {
            case RuleActionTypeCodes.ApplyTimeWindowLimit:
                if (!HasPositiveNumber(root, "limitQty", "maxQty") ||
                    !HasPositiveNumber(root, "windowMinutes", "windowHours"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 limitQty/maxQty 或 windowMinutes/windowHours");
                }
                break;

            case RuleActionTypeCodes.ApplyDayLimitQty:
                if (!HasPositiveNumber(root, "maxDailyQty"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 maxDailyQty");
                }
                break;

            case RuleActionTypeCodes.ApplyOnceLimitQty:
                if (!HasNonNegativeNumber(root, "maxOnceQty", "maxQty"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 maxOnceQty/maxQty");
                }
                break;

            case RuleActionTypeCodes.ApplyMaxAmount:
                if (!HasNonNegativeNumber(root, "maxAmount", "ceilingAmount"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 maxAmount/ceilingAmount");
                }
                break;

            case RuleActionTypeCodes.ApplyMinAmount:
                if (!HasNonNegativeNumber(root, "minAmount", "floorAmount"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 minAmount/floorAmount");
                }
                break;

            case RuleActionTypeCodes.FormulaCalc:
                ValidateFormulaAction(action, root, expressionValidator);
                break;
        }
    }

    private static void ValidateFormulaAction(
        RuleAction action,
        JsonElement? json,
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

    private static JsonDocument? ParseParams(string? paramsJson)
    {
        if (string.IsNullOrWhiteSpace(paramsJson))
        {
            return null;
        }

        try
        {
            return RuleCenterJsonSerializer.ParseDocument(paramsJson);
        }
        catch (JsonException)
        {
            throw new BizException(
                BizErrorCode.ActionParamInvalid,
                409,
                "动作参数不是合法 JSON");
        }
    }

    private static bool HasPositiveNumber(JsonElement? json, params string[] keys)
    {
        return TryGetNumber(json, keys, out var value) && value > 0m;
    }

    private static bool HasNonNegativeNumber(JsonElement? json, params string[] keys)
    {
        return TryGetNumber(json, keys, out var value) && value >= 0m;
    }

    private static bool TryGetNumber(JsonElement? json, string[] keys, out decimal value)
    {
        value = 0m;
        if (json is null)
        {
            return false;
        }

        foreach (var key in keys)
        {
            if (json.Value.TryGetPropertyIgnoreCase(key, out var token) &&
                !token.IsNullOrUndefined() &&
                token.TryReadDecimal(out value))
            {
                return true;
            }
        }

        return false;
    }

    private static string? TryGetString(JsonElement? json, string key)
    {
        if (json is null)
        {
            return null;
        }

        return json.Value.TryGetPropertyIgnoreCase(key, out var token) &&
               !token.IsNullOrUndefined()
            ? token.ReadAsString()
            : null;
    }
}
