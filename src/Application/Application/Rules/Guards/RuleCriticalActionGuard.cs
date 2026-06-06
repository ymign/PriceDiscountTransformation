using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.Rules.Guards;

/// <summary>
/// 资金关键动作异常策略门禁。
/// </summary>
public sealed class RuleCriticalActionGuard
{
    private static readonly HashSet<string> CriticalActionTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        RuleActionTypeCodes.ConvertQty,
        RuleActionTypeCodes.FormulaCalc,
        RuleActionTypeCodes.ApplyDayLimitQty,
        RuleActionTypeCodes.ApplyTimeWindowLimit,
        RuleActionTypeCodes.ApplyOnceLimitQty,
        RuleActionTypeCodes.SameGroupMutex,
        RuleActionTypeCodes.ApplyMinAmount,
        RuleActionTypeCodes.ApplyMaxAmount,
        RuleActionTypeCodes.SameOperationCeiling,
        RuleActionTypeCodes.AddChildItem,
        RuleActionTypeCodes.DiscountExceedToZero
    };

    /// <summary>
    /// 确保资金关键动作失败时必须中断计价。
    /// </summary>
    public void EnsureStopOnError(IReadOnlyList<RuleAction> actions)
    {
        foreach (var action in actions.Where(a => a.IsEnabled == EnableFlag.Yes))
        {
            var actionType = NormalizeActionType(action.ActionType);
            if (!CriticalActionTypes.Contains(actionType))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(action.OnError) ||
                string.Equals(action.OnError, ActionOnErrorCodes.Stop, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            throw new BizException(
                BizErrorCode.ActionOnErrorInvalid,
                409,
                $"ActionType={action.ActionType} 的 OnError 必须为 STOP");
        }
    }

    private static string NormalizeActionType(string? actionType) =>
        actionType?.Trim().ToUpperInvariant() ?? string.Empty;
}
