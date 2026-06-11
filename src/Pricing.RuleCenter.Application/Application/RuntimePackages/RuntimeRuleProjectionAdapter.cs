using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Application.Engine.RuleRuntimeSnapshot;

namespace Pricing.RuleCenter.Application.RuntimePackages;

/// <summary>
/// 运行时规则快照适配器，负责把运行包规则还原为旧规则聚合视角。
/// </summary>
internal sealed class RuntimeRuleProjectionAdapter
{
    /// <summary>
    /// 把运行时规则快照转换为引擎兼容的生效规则快照。
    /// </summary>
    /// <param name="snapshot">运行时规则快照。</param>
    /// <returns>适配后的生效规则快照。</returns>
    public EffectiveRuleSnapshot Adapt(RuntimeRuleSnapshot snapshot)
    {
        var rule = snapshot.Rule;
        return new EffectiveRuleSnapshot
        {
            Header = new RuleAggregate
            {
                RuleId = rule.RuntimeRuleId,
                RuleCode = $"RUNTIME_{rule.RuntimeRuleId}",
                RuleName = $"RuntimeRule_{rule.RuntimeRuleId}",
                RuleCategory = rule.CapabilityFamily,
                RuleScope = string.IsNullOrWhiteSpace(rule.TargetItemCode) ? "GROUP" : "ITEM",
                ItemCode = rule.TargetItemCode,
                GroupCode = rule.TargetGroupCode,
                Priority = ParsePriority(rule.PriorityKey),
                CurrentVersion = 1,
                Status = RuleStatusCodes.Published,
                IsEnabled = EnableFlag.Yes,
                EffectiveFrom = rule.EffectiveFrom,
                EffectiveTo = rule.EffectiveTo
            },
            Conditions = snapshot.Conditions.Select(MapCondition).ToList(),
            Actions = snapshot.Actions.Select(MapAction).ToList()
        };
    }

    private static RuleCondition MapCondition(Core.Aggregates.Runtime.RuntimeCondition condition)
    {
        return new RuleCondition
        {
            ConditionId = condition.RuntimeConditionId,
            RuleId = condition.RuntimeRuleId,
            VersionNo = 1,
            ConditionGroup = condition.ConditionGroup,
            ConditionType = condition.ConditionType,
            OperatorType = condition.OperatorType,
            LeftKey = condition.LeftKey,
            RightValue = condition.RightValue,
            ParamsJson = condition.ParamsJson,
            SortNo = condition.SortNo,
            IsEnabled = EnableFlag.Yes
        };
    }

    private static RuleAction MapAction(Core.Aggregates.Runtime.RuntimeAction action)
    {
        return new RuleAction
        {
            ActionId = action.RuntimeActionId,
            RuleId = action.RuntimeRuleId,
            VersionNo = 1,
            ActionType = action.ActionType,
            ExecutorCode = action.ExecutorCode,
            ParamsJson = action.ParamsJson,
            ExclusiveGroup = action.ExclusiveGroup,
            SortNo = action.SortNo,
            OnError = action.OnError,
            IsEnabled = EnableFlag.Yes
        };
    }

    private static int ParsePriority(string priorityKey)
    {
        if (string.IsNullOrWhiteSpace(priorityKey))
        {
            return 100;
        }

        var firstSegment = priorityKey.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        return int.TryParse(firstSegment, out var parsed)
            ? parsed
            : 100;
    }
}
