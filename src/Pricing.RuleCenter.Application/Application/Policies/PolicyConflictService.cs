using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略运行时冲突检测服务。
/// </summary>
internal sealed class PolicyConflictService : IPolicyConflictService
{
    /// <summary>
    /// 校验候选运行时规则快照之间不存在单胜者冲突。
    /// </summary>
    public void EnsureNoConflicts(IReadOnlyList<RuntimeRuleSnapshot> ruleSnapshots)
    {
        for (var i = 0; i < ruleSnapshots.Count; i++)
        {
            for (var j = i + 1; j < ruleSnapshots.Count; j++)
            {
                var left = ruleSnapshots[i];
                var right = ruleSnapshots[j];
                if (!HasConflict(left, right))
                {
                    continue;
                }

                throw new BizException(
                    BizErrorCode.RuntimePackageBuildConflict,
                    409,
                    $"能力族 {left.Rule.CapabilityFamily} 在相同绑定与作用域上出现并列胜者冲突，策略版本 {left.Rule.SourcePolicyVersionId} 与 {right.Rule.SourcePolicyVersionId} 不能同时入包。");
            }
        }
    }

    private static bool HasConflict(RuntimeRuleSnapshot left, RuntimeRuleSnapshot right)
    {
        if (!string.Equals(left.Rule.CapabilityFamily, right.Rule.CapabilityFamily, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(left.Rule.MergeMode, RuntimeMergeModeCodes.SingleWinner, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(right.Rule.MergeMode, RuntimeMergeModeCodes.SingleWinner, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!HasBindingOverlap(left.Rule, right.Rule))
        {
            return false;
        }

        if (!HasEffectiveRangeOverlap(left.Rule, right.Rule))
        {
            return false;
        }

        if (!HasScopeOverlap(left.Conditions, right.Conditions))
        {
            return false;
        }

        return string.Equals(
            GetPriorityConflictKey(left.Rule.PriorityKey),
            GetPriorityConflictKey(right.Rule.PriorityKey),
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasBindingOverlap(RuntimeRule left, RuntimeRule right)
    {
        if (!string.IsNullOrWhiteSpace(left.TargetItemCode) && !string.IsNullOrWhiteSpace(right.TargetItemCode))
        {
            return string.Equals(left.TargetItemCode, right.TargetItemCode, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(left.TargetGroupCode) && !string.IsNullOrWhiteSpace(right.TargetGroupCode))
        {
            return string.Equals(left.TargetGroupCode, right.TargetGroupCode, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static bool HasEffectiveRangeOverlap(RuntimeRule left, RuntimeRule right)
    {
        var leftFrom = left.EffectiveFrom ?? DateTime.MinValue;
        var leftTo = left.EffectiveTo ?? DateTime.MaxValue;
        var rightFrom = right.EffectiveFrom ?? DateTime.MinValue;
        var rightTo = right.EffectiveTo ?? DateTime.MaxValue;
        return leftFrom <= rightTo && rightFrom <= leftTo;
    }

    private static bool HasScopeOverlap(IReadOnlyList<RuntimeCondition> left, IReadOnlyList<RuntimeCondition> right)
    {
        var leftByType = BuildConditionScopeMap(left);
        var rightByType = BuildConditionScopeMap(right);

        return HasDimensionOverlap(leftByType, rightByType, RuleConditionTypeCodes.GetAliases(RuleConditionTypeCodes.ChargeScene)) &&
               HasDimensionOverlap(leftByType, rightByType, RuleConditionTypeCodes.GetAliases(RuleConditionTypeCodes.BodyPart)) &&
               HasDimensionOverlap(leftByType, rightByType, new[] { RuleConditionTypeCodes.VisitTypeMatch }) &&
               HasDimensionOverlap(leftByType, rightByType, new[] { RuleConditionTypeCodes.TimeRange });
    }

    private static Dictionary<string, HashSet<string>> BuildConditionScopeMap(IReadOnlyList<RuntimeCondition> conditions)
    {
        return conditions
            .Where(condition => !string.IsNullOrWhiteSpace(condition.ConditionType))
            .GroupBy(condition => condition.ConditionType, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(condition => condition.RightValue?.Trim())
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Select(value => value!)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
    }

    private static bool HasDimensionOverlap(
        IReadOnlyDictionary<string, HashSet<string>> left,
        IReadOnlyDictionary<string, HashSet<string>> right,
        IReadOnlyCollection<string> aliases)
    {
        var leftValues = aliases
            .Where(left.ContainsKey)
            .SelectMany(alias => left[alias])
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var rightValues = aliases
            .Where(right.ContainsKey)
            .SelectMany(alias => right[alias])
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return leftValues.Count == 0 || rightValues.Count == 0 || leftValues.Overlaps(rightValues);
    }

    private static string GetPriorityConflictKey(string priorityKey) =>
        string.Join("|", priorityKey.Split('|', StringSplitOptions.TrimEntries).Take(5));
}
