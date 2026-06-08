using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Rules.Profiles;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules.Guards;

/// <summary>
/// 规则发布冲突检测器。
/// </summary>
public sealed class RuleConflictDetector
{
    private const string MutuallyExclusiveActionTypeDictType = "MUTUALLY_EXCLUSIVE_ACTION_TYPE";

    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IRuleConditionRepository _conditionRepository;
    private readonly IRuleActionRepository _actionRepository;
    private readonly IDictRepository _dictRepository;
    private readonly ILogger<RuleConflictDetector> _logger;
    private readonly SemaphoreSlim _cacheLock = new(1, 1);
    private HashSet<string>? _mutuallyExclusiveActionsCache;

    private static readonly HashSet<string> DefaultMutuallyExclusiveActions = new(StringComparer.OrdinalIgnoreCase)
    {
        RuleActionTypeCodes.FormulaCalc,
        RuleActionTypeCodes.ApplyMinAmount,
        RuleActionTypeCodes.ApplyMaxAmount,
        RuleActionTypeCodes.ApplyDayLimitQty,
        RuleActionTypeCodes.ApplyOnceLimitQty,
        RuleActionTypeCodes.ApplyTimeWindowLimit
    };

    /// <summary>
    /// 初始化规则发布冲突检测器。
    /// </summary>
    public RuleConflictDetector(
        IRuleHeaderRepository headerRepository,
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        IDictRepository dictRepository,
        ILogger<RuleConflictDetector> logger)
    {
        _headerRepository = headerRepository;
        _conditionRepository = conditionRepository;
        _actionRepository = actionRepository;
        _dictRepository = dictRepository;
        _logger = logger;
    }

    /// <summary>
    /// 校验目标版本与同项目已发布规则不存在互斥冲突。
    /// </summary>
    public async Task EnsureNoConflictAsync(RuleAggregate targetHeader, int targetVersionNo)
    {
        if (string.IsNullOrWhiteSpace(targetHeader.ItemCode))
        {
            return;
        }

        var sameItemRules = await _headerRepository.GetByItemCodeAsync(targetHeader.ItemCode);
        var publishedRules = sameItemRules
            .Where(r => r.RuleId != targetHeader.RuleId)
            .Where(r => r.Status == RuleStatusCodes.Published && r.IsEnabled == EnableFlag.Yes)
            .Where(r => IsEffectiveRangeOverlap(targetHeader, r))
            .ToList();

        var profiles = await BuildRuleProfilesAsync(
            new[] { (Header: targetHeader, VersionNo: targetVersionNo) }
                .Concat(publishedRules.Select(rule => (Header: rule, VersionNo: rule.CurrentVersion)))
                .ToArray());
        var targetProfile = profiles[(targetHeader.RuleId, targetVersionNo)];

        foreach (var existingRule in publishedRules)
        {
            var existingProfile = profiles[(existingRule.RuleId, existingRule.CurrentVersion)];
            if (!HasSceneOverlap(targetProfile.ConditionScopes, existingProfile.ConditionScopes))
            {
                continue;
            }

            var (hasConflict, conflictActionType) = await HasForbiddenActionConflictAsync(targetProfile.Actions, existingProfile.Actions);
            if (hasConflict)
            {
                throw new BizException(
                    BizErrorCode.RuleOverlapConflict,
                    409,
                    $"项目 {targetHeader.ItemCode} 在相同场景和生效期内已存在 {conflictActionType} 规则，RuleId={existingRule.RuleId}");
            }

            if (targetProfile.Actions.Contains(RuleActionTypeCodes.ConvertQty) &&
                existingProfile.Actions.Contains(RuleActionTypeCodes.ConvertQty) &&
                HasSceneAndBodyPartOverlap(targetProfile.ConditionScopes, existingProfile.ConditionScopes))
            {
                throw new BizException(
                    BizErrorCode.RuleOverlapConflict,
                    409,
                    $"项目 {targetHeader.ItemCode} 的换算规则部位范围重叠，RuleId={existingRule.RuleId}");
            }
        }
    }

    /// <summary>
    /// 清除互斥动作类型缓存。
    /// </summary>
    public void ClearCache()
    {
        _mutuallyExclusiveActionsCache = null;
    }

    private async Task<RuleConflictProfile> BuildRuleProfileAsync(RuleAggregate header, int versionNo)
    {
        var conditions = await _conditionRepository.GetByRuleAndVersionAsync(header.RuleId, versionNo);
        var actions = await _actionRepository.GetByRuleAndVersionAsync(header.RuleId, versionNo);

        return new RuleConflictProfile(
            BuildConditionScopes(conditions),
            actions
                .Where(a => a.IsEnabled == EnableFlag.Yes)
                .Select(a => a.ActionType)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToHashSet(StringComparer.OrdinalIgnoreCase));
    }

    private async Task<IReadOnlyDictionary<(long RuleId, int VersionNo), RuleConflictProfile>> BuildRuleProfilesAsync(
        IReadOnlyCollection<(RuleAggregate Header, int VersionNo)> ruleVersions)
    {
        var keys = ruleVersions
            .Select(item => (item.Header.RuleId, item.VersionNo))
            .Distinct()
            .ToArray();
        var conditionsByRuleVersion = await _conditionRepository.GetByRuleVersionsAsync(keys);
        var actionsByRuleVersion = await _actionRepository.GetByRuleVersionsAsync(keys);

        var result = new Dictionary<(long RuleId, int VersionNo), RuleConflictProfile>();
        foreach (var ruleVersion in ruleVersions)
        {
            var key = (ruleVersion.Header.RuleId, ruleVersion.VersionNo);
            var conditions = conditionsByRuleVersion.TryGetValue(key, out var conditionItems)
                ? conditionItems
                : Array.Empty<RuleCondition>();
            var actions = actionsByRuleVersion.TryGetValue(key, out var actionItems)
                ? actionItems
                : Array.Empty<RuleAction>();

            result[key] = new RuleConflictProfile(
                BuildConditionScopes(conditions),
                actions
                    .Where(action => action.IsEnabled == EnableFlag.Yes)
                    .Select(action => action.ActionType)
                    .Where(actionType => !string.IsNullOrWhiteSpace(actionType))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase));
        }

        return result;
    }

    private static IReadOnlyList<RuleConditionScope> BuildConditionScopes(
        IReadOnlyList<RuleCondition> conditions)
    {
        var enabled = conditions
            .Where(c => c.IsEnabled == EnableFlag.Yes)
            .ToList();
        if (enabled.Count == 0)
        {
            return new[] { RuleConditionScope.Wildcard };
        }

        return enabled
            .GroupBy(c => string.IsNullOrWhiteSpace(c.ConditionGroup)
                ? "DEFAULT"
                : c.ConditionGroup.Trim())
            .Select(group => new RuleConditionScope(
                GetConditionValues(group, RuleConditionTypeCodes.GetAliases(RuleConditionTypeCodes.ChargeScene)),
                GetConditionValues(group, RuleConditionTypeCodes.GetAliases(RuleConditionTypeCodes.BodyPart))))
            .ToList();
    }

    private static HashSet<string> GetConditionValues(
        IEnumerable<RuleCondition> conditions,
        IReadOnlyList<string> conditionTypes)
    {
        var conditionTypeSet = conditionTypes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return conditions
            .Where(c => conditionTypeSet.Contains(c.ConditionType))
            .Select(c => c.RightValue?.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsEffectiveRangeOverlap(RuleAggregate left, RuleAggregate right)
    {
        var leftFrom = left.EffectiveFrom ?? DateTime.MinValue;
        var leftTo = left.EffectiveTo ?? DateTime.MaxValue;
        var rightFrom = right.EffectiveFrom ?? DateTime.MinValue;
        var rightTo = right.EffectiveTo ?? DateTime.MaxValue;
        return leftFrom <= rightTo && rightFrom <= leftTo;
    }

    private static bool HasSceneOverlap(
        IReadOnlyList<RuleConditionScope> left,
        IReadOnlyList<RuleConditionScope> right)
    {
        return left.Any(l => right.Any(r => IsDimensionOverlap(l.ChargeScenes, r.ChargeScenes)));
    }

    private static bool HasSceneAndBodyPartOverlap(
        IReadOnlyList<RuleConditionScope> left,
        IReadOnlyList<RuleConditionScope> right)
    {
        return left.Any(l => right.Any(r =>
            IsDimensionOverlap(l.ChargeScenes, r.ChargeScenes) &&
            IsDimensionOverlap(l.BodyParts, r.BodyParts)));
    }

    private static bool IsDimensionOverlap(HashSet<string> left, HashSet<string> right)
    {
        return left.Count == 0 || right.Count == 0 || left.Overlaps(right);
    }

    private async Task<(bool HasConflict, string ActionType)> HasForbiddenActionConflictAsync(
        HashSet<string> left,
        HashSet<string> right)
    {
        var forbiddenActions = await GetMutuallyExclusiveActionsAsync();
        var actionType = forbiddenActions.FirstOrDefault(a => left.Contains(a) && right.Contains(a)) ?? string.Empty;
        return (!string.IsNullOrEmpty(actionType), actionType);
    }

    private async Task<HashSet<string>> GetMutuallyExclusiveActionsAsync()
    {
        if (_mutuallyExclusiveActionsCache is not null)
        {
            return _mutuallyExclusiveActionsCache;
        }

        await _cacheLock.WaitAsync();
        try
        {
            if (_mutuallyExclusiveActionsCache is not null)
            {
                return _mutuallyExclusiveActionsCache;
            }

            return await LoadMutuallyExclusiveActionsAsync();
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private async Task<HashSet<string>> LoadMutuallyExclusiveActionsAsync()
    {
        try
        {
            var dictItems = await _dictRepository.GetByTypeAsync(MutuallyExclusiveActionTypeDictType);

            if (dictItems.Count == 0)
            {
                _logger.LogWarning(
                    "PR_DICT 中未找到字典类型={DictType} 的字典项，使用默认互斥动作类型列表",
                    MutuallyExclusiveActionTypeDictType);
                _mutuallyExclusiveActionsCache = new HashSet<string>(
                    DefaultMutuallyExclusiveActions,
                    StringComparer.OrdinalIgnoreCase);
                return _mutuallyExclusiveActionsCache;
            }

            var result = new HashSet<string>(
                DefaultMutuallyExclusiveActions,
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in dictItems.Where(d => d.IsEnabled == EnableFlag.Yes))
            {
                result.Add(item.DictCode);
            }

            _mutuallyExclusiveActionsCache = result;
            _logger.LogInformation("已从 PR_DICT 加载互斥动作类型，共 {Count} 个", result.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "从 PR_DICT 加载互斥动作类型失败，使用默认互斥列表");
            _mutuallyExclusiveActionsCache = new HashSet<string>(
                DefaultMutuallyExclusiveActions,
                StringComparer.OrdinalIgnoreCase);
            return _mutuallyExclusiveActionsCache;
        }
    }
}
