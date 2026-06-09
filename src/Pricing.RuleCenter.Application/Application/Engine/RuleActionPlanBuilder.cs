using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 规则动作执行计划构建器。
/// </summary>
/// <remarks>
/// <para>
/// 该类型把多条命中规则的动作合并成一条可执行动作链。排序优先级为：
/// 动作类型全局顺序 → 命中规则优先级 → 动作 SortNo。
/// </para>
/// <para>
/// 全局动作顺序是资金口径，不能只按每条规则内部 SortNo 排序。
/// 例如必须先执行换算和数量限制，再执行公式折价，最后金额封顶和超限归零。
/// </para>
/// </remarks>
public sealed class RuleActionPlanBuilder : IRuleActionPlanBuilder
{
    /// <summary>
    /// 动作类型顺序字典类型。
    /// </summary>
    private const string ActionTypeOrderDictType = "ACTION_TYPE_ORDER";

    /// <summary>
    /// 默认动作顺序，作为 PR_DICT 缺失时的资金安全兜底顺序。
    /// </summary>
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

    /// <summary>
    /// 动作类型顺序缓存。
    /// </summary>
    private static volatile Dictionary<string, int> s_actionTypeOrderCache =
        new(DefaultActionTypeOrder, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 缓存加载锁，防止并发请求重复读取字典。
    /// </summary>
    private static readonly SemaphoreSlim s_cacheLock = new(1, 1);

    /// <summary>
    /// 缓存是否已从字典加载。
    /// </summary>
    private static volatile bool s_cacheLoaded;

    /// <summary>
    /// 字典仓储，用于读取 ACTION_TYPE_ORDER。
    /// </summary>
    private readonly IDictRepository _dictRepository;
    /// <summary>
    /// 运行期诊断日志。
    /// </summary>
    private readonly ILogger<RuleActionPlanBuilder> _logger;

    /// <summary>
    /// 初始化规则动作执行计划构建器。
    /// </summary>
    /// <param name="dictRepository">字典仓储。</param>
    /// <param name="logger">日志组件。</param>
    public RuleActionPlanBuilder(
        IDictRepository dictRepository,
        ILogger<RuleActionPlanBuilder> logger)
    {
        _dictRepository = dictRepository;
        _logger = logger;
    }

    /// <summary>
    /// 生成按全局动作顺序排列后的可执行动作链。
    /// </summary>
    /// <param name="actions">命中规则下已启用的动作集合。</param>
    /// <param name="matchedRules">已按优先级排序的命中规则集合。</param>
    /// <returns>去除互斥动作并按全局顺序排列的动作链。</returns>
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
        // ExclusiveGroup 表示同一组动作只能执行一个。
        // 例如同一项目命中多条换算动作时，只选择优先级最高规则下的动作，避免重复换算。
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
            // 互斥组内先按规则优先级选，再按动作 SortNo 和 ActionId 稳定排序。
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
        // 先按全局动作类型顺序保证资金口径，再按规则优先级和动作 SortNo 保证同类动作稳定顺序。
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

        // 未登记动作类型属于配置缺陷。这里必须抛异常，不能把未知动作排到最后继续收费。
        throw new InvalidOperationException(
            $"动作类型 {actionType} 未在 {ActionTypeOrderDictType} 字典中登记");
    }

    private async Task EnsureActionTypeOrderLoadedAsync()
    {
        if (s_cacheLoaded)
        {
            return;
        }

        // 双重检查减少高并发计价时的字典读取开销。
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
                // 字典为空时使用默认顺序，保证系统可以在初始化字典缺失时继续按已确认资金口径运行。
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
                    // 禁用的字典项不覆盖默认顺序。
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
