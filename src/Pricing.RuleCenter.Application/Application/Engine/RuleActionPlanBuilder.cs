using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 规则动作执行计划构建器。
/// </summary>
public sealed class RuleActionPlanBuilder
{
    private const string ActionTypeOrderDictType = "ACTION_TYPE_ORDER";

    private static readonly Dictionary<string, int> DefaultActionTypeOrder = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CONVERT_QTY"] = 0,
        ["APPLY_DAY_LIMIT_QTY"] = 1,
        ["APPLY_TIME_WINDOW_LIMIT"] = 2,
        ["APPLY_ONCE_LIMIT_QTY"] = 3,
        ["SAME_GROUP_MUTEX"] = 4,
        ["FORMULA_CALC"] = 5,
        ["APPLY_MIN_AMOUNT"] = 6,
        ["APPLY_MAX_AMOUNT"] = 7,
        ["SAME_OPERATION_CEILING"] = 8,
        ["ADD_CHILD_ITEM"] = 9,
        ["DISCOUNT_EXCEED_TO_ZERO"] = 10
    };

    private static volatile Dictionary<string, int> s_actionTypeOrderCache =
        new(DefaultActionTypeOrder, StringComparer.OrdinalIgnoreCase);

    private static readonly SemaphoreSlim s_cacheLock = new(1, 1);

    private static volatile bool s_cacheLoaded;

    private readonly IDictRepository _dictRepository;
    private readonly ILogger _logger;

    /// <summary>
    /// 初始化规则动作执行计划构建器。
    /// </summary>
    public RuleActionPlanBuilder(
        IDictRepository dictRepository,
        ILogger logger)
    {
        _dictRepository = dictRepository;
        _logger = logger;
    }

    /// <summary>
    /// 生成按全局动作顺序排列后的可执行动作链。
    /// </summary>
    public async Task<IReadOnlyList<RuleAction>> BuildAsync(
        IReadOnlyList<RuleAction> actions,
        IReadOnlyList<RuleAggregate> matchedRules)
    {
        var ruleOrder = BuildRuleOrder(matchedRules);
        var executableActions = ApplyExclusiveGroups(actions, ruleOrder);
        await EnsureActionTypeOrderLoadedAsync();
        return OrderActions(executableActions, ruleOrder);
    }

    /// <summary>
    /// 清理动作执行顺序缓存。
    /// </summary>
    public void ClearCache()
    {
        s_cacheLoaded = false;
        _logger.LogDebug("动作执行顺序缓存已清除，下次匹配将从字典重新加载");
    }

    private static List<RuleAction> ApplyExclusiveGroups(
        IReadOnlyList<RuleAction> actions,
        IReadOnlyDictionary<long, int> ruleOrder)
    {
        if (actions.Count == 0)
        {
            return new List<RuleAction>();
        }

        var result = new List<RuleAction>();
        var exclusiveGroups = new Dictionary<string, List<RuleAction>>(StringComparer.OrdinalIgnoreCase);

        foreach (var action in actions)
        {
            if (string.IsNullOrWhiteSpace(action.ExclusiveGroup))
            {
                result.Add(action);
                continue;
            }

            var groupKey = action.ExclusiveGroup.Trim();
            if (!exclusiveGroups.TryGetValue(groupKey, out var groupActions))
            {
                groupActions = new List<RuleAction>();
                exclusiveGroups[groupKey] = groupActions;
            }

            groupActions.Add(action);
        }

        foreach (var group in exclusiveGroups.Values)
        {
            var selected = group
                .OrderBy(action => ruleOrder.TryGetValue(action.RuleId, out var order) ? order : int.MaxValue)
                .ThenBy(action => action.SortNo)
                .ThenBy(action => action.ActionId)
                .First();
            result.Add(selected);
        }

        return result;
    }

    private static IReadOnlyDictionary<long, int> BuildRuleOrder(IReadOnlyList<RuleAggregate> matchedRules)
    {
        return matchedRules
            .Select((rule, index) => new { rule.RuleId, Order = index })
            .ToDictionary(item => item.RuleId, item => item.Order);
    }

    private IReadOnlyList<RuleAction> OrderActions(
        List<RuleAction> actions,
        IReadOnlyDictionary<long, int> ruleOrder)
    {
        return actions
            .OrderBy(a => GetActionTypeSortOrder(a.ActionType))
            .ThenBy(a => ruleOrder.TryGetValue(a.RuleId, out var order) ? order : int.MaxValue)
            .ThenBy(a => a.SortNo)
            .ToList();
    }

    private int GetActionTypeSortOrder(string actionType)
    {
        if (s_actionTypeOrderCache.TryGetValue(actionType, out var order))
        {
            return order;
        }

        throw new InvalidOperationException(
            $"动作类型 {actionType} 未在 {ActionTypeOrderDictType} 字典中登记");
    }

    private async Task EnsureActionTypeOrderLoadedAsync()
    {
        if (s_cacheLoaded)
        {
            return;
        }

        await s_cacheLock.WaitAsync();
        try
        {
            if (s_cacheLoaded)
            {
                return;
            }

            await LoadActionTypeOrderAsync();
        }
        finally
        {
            s_cacheLock.Release();
        }
    }

    private async Task LoadActionTypeOrderAsync()
    {
        try
        {
            var dictItems = await _dictRepository.GetByTypeAsync(ActionTypeOrderDictType);

            if (dictItems.Count == 0)
            {
                _logger.LogWarning(
                    "PR_DICT 中未找到字典类型={DictType} 的字典项，使用默认动作执行顺序",
                    ActionTypeOrderDictType);
                s_actionTypeOrderCache = new Dictionary<string, int>(DefaultActionTypeOrder, StringComparer.OrdinalIgnoreCase);
                s_cacheLoaded = true;
                return;
            }

            var newCache = new Dictionary<string, int>(
                DefaultActionTypeOrder,
                StringComparer.OrdinalIgnoreCase);
            var loadedActionTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in dictItems)
            {
                if (item.IsEnabled != "Y")
                {
                    continue;
                }

                if (loadedActionTypes.Add(item.DictCode))
                {
                    newCache[item.DictCode] = item.SortNo;
                }
            }

            s_actionTypeOrderCache = newCache;
            s_cacheLoaded = true;

            _logger.LogInformation(
                "已从 PR_DICT 加载动作执行顺序，共 {Count} 个动作类型",
                newCache.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "从 PR_DICT 加载动作执行顺序失败");
            throw new InvalidOperationException(
                $"加载 {ActionTypeOrderDictType} 动作执行顺序失败", ex);
        }
    }
}
