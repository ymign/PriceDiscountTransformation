using Microsoft.Extensions.Logging;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 规则匹配服务，负责从已发布规则中找出本次计价应执行的规则和动作链。
/// </summary>
/// <remarks>
/// <para>
/// 【匹配流程概述】规则匹配分为六阶段：
/// <list type="number">
///   <item><description>按项目编码取候选规则（初筛）</description></item>
///   <item><description>过滤已发布、启用、业务时间有效的规则</description></item>
///   <item><description>逐条评估条件组（同组 AND，跨组 OR）</description></item>
///   <item><description>按规则优先级排序</description></item>
///   <item><description>收集命中规则的动作</description></item>
///   <item><description>按全局动作顺序整理动作链</description></item>
/// </list>
/// </para>
/// <para>
/// 【多规则叠加】同一项目可能命中多条规则（如不同部位的换算规则）。
/// 多规则的动作统一按全局动作顺序排列，确保换算 → 公式 → 限额 → 折价的执行顺序不被破坏。
/// </para>
/// <para>
/// 【业务时间】匹配使用 BusinessChargeTime（HIS 业务发生时间），而不是服务器当前时间。
/// 这确保补录历史费用时按历史规则时间窗口命中，而不是按补录操作时间。
/// </para>
/// <para>
/// 【核心约束引用】
/// <list type="bullet">
///   <item><description>NULL ≠ 0：条件值为 NULL 表示"不校验"，0 表示"限制为零"</description></item>
///   <item><description>公式优先于限制：先算公式，再与限制比较——由动作排序保证</description></item>
///   <item><description>超出 = 0元：不是拒单，不是整单归零——由 ExceedToZeroExecutor 实现</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class RuleMatchService
{
    /// <summary>
    /// 规则主档仓储，用于按项目编码读取候选规则（PR_RULE_HEADER）。
    /// </summary>
    private readonly IRuleHeaderRepository _headerRepository;

    /// <summary>
    /// 规则条件仓储，用于读取候选规则当前版本下的条件集合（PR_RULE_CONDITION）。
    /// </summary>
    private readonly IRuleConditionRepository _conditionRepository;

    /// <summary>
    /// 规则动作仓储，用于读取命中规则当前版本下的动作链（PR_RULE_ACTION）。
    /// </summary>
    private readonly IRuleActionRepository _actionRepository;

    /// <summary>
    /// 条件评估器工厂，用于按条件类型（ConditionType）选择具体评估器。
    /// </summary>
    private readonly ConditionEvaluatorFactory _evaluatorFactory;

    /// <summary>
    /// 字典仓储，用于从 PR_DICT 表读取动作执行顺序等可配置数据。
    /// </summary>
    private readonly IDictRepository _dictRepository;

    /// <summary>
    /// 匹配日志，用于记录未知评估器、命中数量等运行期诊断信息。
    /// </summary>
    private readonly ILogger<RuleMatchService> _logger;

    /// <summary>
    /// 默认动作执行顺序。在字典数据尚未加载时使用，确保向后兼容。
    /// </summary>
    /// <remarks>
    /// 顺序设计依据业务计算规则：
    /// <list type="number">
    ///   <item><description>CONVERT_QTY — 双单位换算，公式依赖换算后数量</description></item>
    ///   <item><description>FORMULA_CALC — 公式计算，结果写入 FormulaAmount 和 FinalAmount</description></item>
    ///   <item><description>APPLY_MIN_AMOUNT — 金额下限，公式之后才能比较</description></item>
    ///   <item><description>APPLY_MAX_AMOUNT — 金额上限，公式之后才能比较</description></item>
    ///   <item><description>APPLY_DAY_LIMIT_QTY — 日数量限制，需要查询全院累计</description></item>
    ///   <item><description>APPLY_TIME_WINDOW_LIMIT — 时间窗数量限制（如2小时窗）</description></item>
    ///   <item><description>APPLY_ONCE_LIMIT_QTY — 单次数量限制</description></item>
    ///   <item><description>SAME_GROUP_MUTEX — 同组互斥</description></item>
    ///   <item><description>ADD_CHILD_ITEM — 子项加收</description></item>
    ///   <item><description>DISCOUNT_EXCEED_TO_ZERO — 超出部分归零，必须最后执行</description></item>
    /// </list>
    /// 新增动作类型不再需要修改此默认列表——只需在 PR_DICT 表的 ACTION_TYPE_ORDER 类型中插入记录即可。
    /// </remarks>
    private static readonly Dictionary<string, int> DefaultActionTypeOrder = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CONVERT_QTY"] = 0,
        ["FORMULA_CALC"] = 1,
        ["APPLY_MIN_AMOUNT"] = 2,
        ["APPLY_MAX_AMOUNT"] = 3,
        ["APPLY_DAY_LIMIT_QTY"] = 4,
        ["APPLY_TIME_WINDOW_LIMIT"] = 5,
        ["APPLY_ONCE_LIMIT_QTY"] = 6,
        ["SAME_GROUP_MUTEX"] = 7,
        ["SAME_OPERATION_CEILING"] = 8,
        ["ADD_CHILD_ITEM"] = 9,
        ["DISCOUNT_EXCEED_TO_ZERO"] = 10
    };

    /// <summary>
    /// 字典类型编码：动作执行顺序。
    /// 对应 PR_DICT 表中 DICT_TYPE = "ACTION_TYPE_ORDER" 的字典项。
    /// </summary>
    private const string ActionTypeOrderDictType = "ACTION_TYPE_ORDER";

    /// <summary>
    /// 动作执行顺序缓存。键为动作类型编码，值为排序序号（数字越小越先执行）。
    /// 在 MatchAsync 首次调用时从字典加载，之后使用缓存值。
    /// 规则发布/停用/回滚时应调用 ClearActionTypeOrderCache 清除缓存，确保下次读取最新顺序。
    /// </summary>
    private volatile Dictionary<string, int> _actionTypeOrderCache =
        new(DefaultActionTypeOrder, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 缓存加载锁，确保多线程并发时只加载一次字典数据。
    /// </summary>
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    /// <summary>
    /// 标记缓存是否已从字典加载过。false 表示尚未加载，下次 MatchAsync 将尝试从字典读取。
    /// </summary>
    private volatile bool _cacheLoaded;

    /// <summary>
    /// 初始化规则匹配服务。
    /// </summary>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="conditionRepository">规则条件仓储。</param>
    /// <param name="actionRepository">规则动作仓储。</param>
    /// <param name="evaluatorFactory">条件评估器工厂。</param>
    /// <param name="dictRepository">字典仓储，用于读取动作执行顺序。</param>
    /// <param name="logger">日志对象。</param>
    public RuleMatchService(
        IRuleHeaderRepository headerRepository,
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        ConditionEvaluatorFactory evaluatorFactory,
        IDictRepository dictRepository,
        ILogger<RuleMatchService> logger)
    {
        _headerRepository = headerRepository;
        _conditionRepository = conditionRepository;
        _actionRepository = actionRepository;
        _evaluatorFactory = evaluatorFactory;
        _dictRepository = dictRepository;
        _logger = logger;
    }

    /// <summary>
    /// 匹配当前计价上下文对应的规则和动作链。
    /// </summary>
    /// <param name="context">
    /// 计价上下文，至少包含 ItemCode（项目编码）和 BusinessChargeTime（业务收费时间）。
    /// </param>
    /// <returns>
    /// 命中的规则集合（按优先级排序），以及按全局顺序排列后的动作集合。
    /// 两个集合的对应关系通过 RuleId + Version 可追溯。
    /// </returns>
    public async Task<(IReadOnlyList<RuleHeader> Rules, IReadOnlyList<RuleAction> Actions)>
        MatchAsync(PricingContext context)
    {
        // ========== 第一阶段：按项目编码取候选规则 ==========
        // 初筛只看 ITEM_CODE，避免全表扫描。项目组规则后续可扩展为按 GROUP_CODE 查询。
        // 仓储实现应利用索引 PR_RULE_HEADER(ITEM_CODE, STATUS, IS_ENABLED) 加速查询。
        var candidates = await _headerRepository.GetByItemCodeAsync(context.ItemCode);

        // ========== 第二阶段：过滤已发布、启用、业务时间有效的规则 ==========
        // 三重过滤：
        //   Status == "PUBLISHED"：只使用已发布的规则，草稿和审批中的规则不参与计价
        //   IsEnabled == "Y"：运行时可临时禁用规则而无需删除
        //   业务时间在生效区间内：EffectiveFrom <= BusinessChargeTime <= EffectiveTo
        // 业务时间使用 BusinessChargeTime，而不是当前系统时间，确保补录按真实收费时间匹配历史规则。
        var published = candidates
            .Where(r => r.Status == "PUBLISHED" && r.IsEnabled == "Y")
            .Where(r => IsInEffectiveRange(r, context.BusinessChargeTime))
            .ToList();

        // ========== 第三阶段：逐条评估条件组 ==========
        // 条件表支持同组 AND、跨组 OR 语义（参见 EvaluateConditions 方法）。
        // 只要任一条件组全部满足，该规则就命中。这是"OR 组，AND 组内"的经典规则引擎模式。
        var matchedRules = new List<RuleHeader>();

        foreach (var rule in published)
        {
            // 读取该规则当前版本下的所有条件。版本号由规则主档 CurrentVersion 控制，
            // 确保始终使用最新发布版本的条件，而不是历史版本。
            var conditions = await _conditionRepository.GetByRuleAndVersionAsync(
                rule.RuleId, rule.CurrentVersion);

            if (EvaluateConditions(conditions, context))
            {
                matchedRules.Add(rule);
            }
        }

        // ========== 第四阶段：按规则优先级排序 ==========
        // 数字越小优先级越高。当多条规则同时命中时，优先级决定动作的执行顺序基准。
        // 注意：最终动作顺序还受全局 ActionTypeOrder 影响，优先级主要影响同类动作的先后。
        matchedRules.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        // ========== 第五阶段：收集命中规则的动作 ==========
        // 只收集已启用的动作（IsEnabled == "Y"），允许规则维护时临时禁用单个动作。
        // 动作是否真正执行还取决于 ActionExecutionPipeline 和具体 Executor 的判断。
        var allActions = new List<RuleAction>();
        foreach (var rule in matchedRules)
        {
            var actions = await _actionRepository.GetByRuleAndVersionAsync(
                rule.RuleId, rule.CurrentVersion);
            allActions.AddRange(actions.Where(a => a.IsEnabled == "Y"));
        }
        var ruleOrder = BuildRuleOrder(matchedRules);
        var executableActions = ApplyExclusiveGroups(allActions, ruleOrder);

        // ========== 第六阶段：按全局动作顺序整理动作链 ==========
        // 多规则叠加时，如果只按每条规则的 SortNo 执行，可能出现先限额后公式的错误顺序。
        // OrderActions 先按全局类别排序，再按同类 SortNo 排序，确保：
        //   换算 → 公式 → 下限 → 上限 → 日限 → 窗限 → 单次限 → 互斥 → 加收 → 归零
        //
        // 动作执行顺序从 PR_DICT 字典表读取（DICT_TYPE = "ACTION_TYPE_ORDER"），
        // 首次调用时加载并缓存，后续使用缓存值。规则发布/停用/回滚时会清除缓存。
        await EnsureActionTypeOrderLoadedAsync();
        var ordered = OrderActions(executableActions, ruleOrder);

        _logger.LogInformation(
            "规则匹配 ItemCode={ItemCode}, 命中 {RuleCount} 条规则, {ActionCount} 个动作",
            context.ItemCode, matchedRules.Count, ordered.Count);

        return (matchedRules, ordered);
    }

    /// <summary>
    /// 应用规则动作互斥组，只保留同一 ExclusiveGroup 中规则优先级最高、动作排序最靠前的一条动作。
    /// </summary>
    /// <param name="actions">已命中规则收集到的动作集合。</param>
    /// <param name="ruleOrder">命中规则优先级顺序索引。</param>
    /// <returns>互斥组过滤后的动作集合。</returns>
    /// <remarks>
    /// 同一项目允许按不同部位命中不同换算动作，但同一互斥组内不能重复执行多套换算、公式或封顶动作。
    /// 规则发布校验会阻断明显冲突；运行期仍按优先级做一次保守收敛，避免历史脏配置导致重复计价。
    /// </remarks>
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

    private static IReadOnlyDictionary<long, int> BuildRuleOrder(IReadOnlyList<RuleHeader> matchedRules)
    {
        return matchedRules
            .Select((rule, index) => new { rule.RuleId, Order = index })
            .ToDictionary(item => item.RuleId, item => item.Order);
    }

    /// <summary>
    /// 按条件组判断当前上下文是否满足规则条件集合。
    /// </summary>
    /// <param name="conditions">规则条件集合，来自 PR_RULE_CONDITION 表。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>
    /// 任一条件组全部满足时返回 <c>true</c>（OR 组，AND 组内）。
    /// </returns>
    /// <remarks>
    /// 条件组逻辑：
    /// <list type="bullet">
    ///   <item><description>同一 CONDITION_GROUP 内的条件是 AND 关系——全部满足才算该组通过</description></item>
    ///   <item><description>不同 CONDITION_GROUP 之间是 OR 关系——任一组通过即规则命中</description></item>
    ///   <item><description>没有条件的规则视为兜底规则（只要项目命中就执行）</description></item>
    ///   <item><description>所有条件都禁用时同样按兜底处理，便于临时关闭附加条件</description></item>
    /// </list>
    /// </remarks>
    private bool EvaluateConditions(IReadOnlyList<RuleCondition> conditions, PricingContext context)
    {
        // 没有条件的规则视为兜底规则。这样可以配置"只要项目命中就执行"的简单规则。
        // 典型场景：某个项目在所有场景、所有部位都执行相同的折价公式，无需配置额外条件。
        if (conditions.Count == 0)
        {
            return true;
        }

        // 所有条件都禁用时同样按兜底规则处理，便于临时关闭某些附加条件而不删除条件记录。
        var enabled = conditions.Where(c => c.IsEnabled == "Y").ToList();
        if (enabled.Count == 0)
        {
            return true;
        }

        // 同一 CONDITION_GROUP 内是 AND，不同组之间是 OR。
        // 例如：GROUP 1 = (项目A AND 部位B)，GROUP 2 = (项目A AND 场景C)，
        // 意味着"部位B"或"场景C"任一满足即可命中。
        var groups = enabled.GroupBy(c => c.ConditionGroup);

        foreach (var group in groups)
        {
            var allMatch = true;

            foreach (var condition in group)
            {
                // 按条件类型查找对应评估器。未注册的条件类型返回 null。
                var evaluator = _evaluatorFactory.GetEvaluator(condition.ConditionType);

                if (evaluator is null)
                {
                    // 找不到评估器时不能默认匹配（保守策略）。
                    // 否则未知条件会被忽略，导致规则误命中——这在计价场景下是资金风险。
                    // 运维可通过追溯查询中的警告日志发现未识别的条件类型。
                    _logger.LogWarning("未找到条件评估器: {ConditionType}，RuleId={RuleId}",
                        condition.ConditionType, condition.RuleId);
                    allMatch = false;
                    break;
                }

                if (!evaluator.Evaluate(condition, context))
                {
                    // 组内任一条件不满足，该组判定为不通过，立即短路。
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                // 任一条件组全部满足，规则命中。OR 语义：立即返回，不再评估剩余组。
                return true;
            }
        }

        // 所有条件组都不满足，规则不命中。
        return false;
    }

    /// <summary>
    /// 按全局动作类别顺序和动作内排序号整理动作链。
    /// </summary>
    /// <param name="actions">待排序动作集合，可能来自多条命中规则。</param>
    /// <returns>
    /// 排序后的动作集合。排序规则：
    ///   1. 先按 PR_DICT 字典表定义的全局类别顺序
    ///   2. 同类动作内按规则配置的 SortNo 排序
    /// </returns>
    /// <remarks>
    /// 这个排序是资金安全的关键。如果多规则叠加时只按每条规则的 SortNo 执行，
    /// 可能出现"先执行限额截断，再执行公式计算"的错误顺序，导致公式结果被限额误截断。
    ///
    /// 动作执行顺序从缓存字典中读取，缓存来源为 PR_DICT 表的 ACTION_TYPE_ORDER 类型。
    /// 字典未加载时使用默认顺序，确保向后兼容。
    /// </remarks>
    private IReadOnlyList<RuleAction> OrderActions(
        List<RuleAction> actions,
        IReadOnlyDictionary<long, int> ruleOrder)
    {
        // 先按全局动作类别排序，再按规则配置的 SortNo 排序。
        // SortNo 只在同类动作内部生效，避免跨类别动作破坏资金计算顺序。
        return actions
            .OrderBy(a => GetActionTypeSortOrder(a.ActionType))
            .ThenBy(a => ruleOrder.TryGetValue(a.RuleId, out var order) ? order : int.MaxValue)
            .ThenBy(a => a.SortNo)
            .ToList();
    }

    /// <summary>
    /// 获取动作类型在全局动作链中的排序序号。
    ///
    /// 从缓存的动作执行顺序字典中查找指定动作类型的排序序号。
    /// 缓存来源为 PR_DICT 表中 DICT_TYPE = "ACTION_TYPE_ORDER" 的字典项，
    /// 其中 DICT_CODE = 动作类型编码，SORT_NO = 执行顺序。
    /// </summary>
    /// <param name="actionType">动作类型编码（如 "CONVERT_QTY"、"FORMULA_CALC"）。</param>
    /// <returns>
    /// 动作排序序号（数字越小越先执行）；未知动作返回 999 作为兜底序号排到最后。
    /// </returns>
    /// <remarks>
    /// 未识别的动作排到最后。正常情况下发布校验应阻断未知动作类型；
    /// 这里保留兜底排序，避免运行期因新动作暂未登记到字典中导致排序异常抛出。
    ///
    /// 使用 OrdinalIgnoreCase 比较，确保动作类型编码大小写不敏感。
    /// </remarks>
    private int GetActionTypeSortOrder(string actionType)
    {
        if (_actionTypeOrderCache.TryGetValue(actionType, out var order))
        {
            return order;
        }

        // 未识别的动作排到最后。运维可通过追溯查询中的警告日志发现未登记的动作类型。
        _logger.LogWarning("动作类型 {ActionType} 未在 ACTION_TYPE_ORDER 字典中登记，使用兜底排序 999", actionType);
        return 999;
    }

    /// <summary>
    /// 确保动作执行顺序缓存已从字典加载。
    ///
    /// 首次调用时从 PR_DICT 表读取 DICT_TYPE = "ACTION_TYPE_ORDER" 的字典项，
    /// 构建动作类型编码到排序序号的映射字典并缓存。后续调用直接使用缓存值。
    ///
    /// 加载策略：
    ///   - 使用 SemaphoreSlim 保证线程安全，避免并发重复加载。
    ///   - 加载失败时保留默认顺序（DefaultActionTypeOrder），确保系统可用性。
    ///   - 加载成功后设置 _cacheLoaded 标记，后续调用跳过数据库查询。
    /// </summary>
    private async Task EnsureActionTypeOrderLoadedAsync()
    {
        // 快速路径：缓存已加载，直接返回，避免获取锁的开销。
        if (_cacheLoaded)
        {
            return;
        }

        await _cacheLock.WaitAsync();
        try
        {
            // 双重检查：进入锁之后再次判断，避免并发场景下重复加载。
            if (_cacheLoaded)
            {
                return;
            }

            await LoadActionTypeOrderAsync();
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    /// <summary>
    /// 从 PR_DICT 字典表加载动作执行顺序到缓存。
    ///
    /// 读取 DICT_TYPE = "ACTION_TYPE_ORDER" 的所有启用字典项，
    /// 以 DICT_CODE 为动作类型编码、SORT_NO 为排序序号，构建映射字典。
    ///
    /// 加载失败时保留当前缓存（可能是默认顺序），记录错误日志但不抛出异常。
    /// 这确保字典表不可用时计价引擎仍能使用默认顺序正常工作。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 字典项字段映射：
    /// <list type="bullet">
    ///   <item><description>DICT_CODE → 动作类型编码（如 "CONVERT_QTY"）</description></item>
    ///   <item><description>SORT_NO → 执行顺序序号（数字越小越先执行）</description></item>
    ///   <item><description>IS_ENABLED → 是否参与排序（"N" 时跳过）</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// 缓存失效时机：
    ///   - 规则发布、停用、回滚时调用 ClearActionTypeOrderCache 清除缓存。
    ///   - 字典维护界面修改 ACTION_TYPE_ORDER 类型字典后应清除缓存。
    /// </para>
    /// </remarks>
    private async Task LoadActionTypeOrderAsync()
    {
        try
        {
            var dictItems = await _dictRepository.GetByTypeAsync(ActionTypeOrderDictType);

            if (dictItems.Count == 0)
            {
                // 字典表中没有 ACTION_TYPE_ORDER 数据，使用默认顺序。
                // 这通常发生在系统首次部署、种子数据尚未初始化时。
                _logger.LogWarning(
                    "PR_DICT 中未找到 DICT_TYPE = {DictType} 的字典项，使用默认动作执行顺序",
                    ActionTypeOrderDictType);
                _actionTypeOrderCache = new Dictionary<string, int>(DefaultActionTypeOrder, StringComparer.OrdinalIgnoreCase);
                _cacheLoaded = true;
                return;
            }

            var newCache = new Dictionary<string, int>(
                DefaultActionTypeOrder,
                StringComparer.OrdinalIgnoreCase);
            var loadedActionTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in dictItems)
            {
                // 默认动作顺序是资金安全保底，字典只允许覆盖排序或追加新动作，不能因缺项削弱默认顺序。
                // 同一动作类型出现重复时保留字典排序后的首个。
                if (item.IsEnabled != "Y")
                {
                    continue;
                }

                if (loadedActionTypes.Add(item.DictCode))
                {
                    newCache[item.DictCode] = item.SortNo;
                }
            }

            _actionTypeOrderCache = newCache;
            _cacheLoaded = true;

            _logger.LogInformation(
                "已从 PR_DICT 加载动作执行顺序，共 {Count} 个动作类型",
                newCache.Count);
        }
        catch (Exception ex)
        {
            // 加载失败时保留当前缓存（可能是默认顺序），记录错误日志但不抛出异常。
            // 这确保字典表不可用时计价引擎仍能使用默认顺序正常工作。
            _logger.LogError(ex,
                "从 PR_DICT 加载动作执行顺序失败，保留当前缓存顺序（可能是默认顺序）");
        }
    }

    /// <summary>
    /// 清除动作执行顺序缓存，强制下次 MatchAsync 从字典重新加载。
    ///
    /// 调用时机：
    ///   - 规则发布（PublishAsync）后，动作顺序可能已调整。
    ///   - 规则停用（DisableAsync）后，需要刷新缓存。
    ///   - 规则回滚（RollbackAsync）后，需要刷新缓存。
    ///   - 字典维护界面修改 ACTION_TYPE_ORDER 类型字典后。
    ///
    /// 线程安全：
    ///   使用 volatile 写入确保多线程可见性。
    ///   清除后下一个 MatchAsync 调用会通过 EnsureActionTypeOrderLoadedAsync 重新加载。
    /// </summary>
    public void ClearActionTypeOrderCache()
    {
        _cacheLoaded = false;
        _logger.LogDebug("动作执行顺序缓存已清除，下次匹配将从字典重新加载");
    }

    /// <summary>
    /// 判断业务时间是否落在规则生效区间内（闭区间）。
    /// </summary>
    /// <param name="rule">规则主档，包含 EffectiveFrom 和 EffectiveTo。</param>
    /// <param name="businessTime">业务收费时间（来自 HIS，非服务器时间）。</param>
    /// <returns>在生效区间内返回 <c>true</c>。</returns>
    /// <remarks>
    /// 生效区间按闭区间处理：[EffectiveFrom, EffectiveTo]。
    /// NULL 值表示不设限：EffectiveFrom 为 null 表示无开始时间限制，
    /// EffectiveTo 为 null 表示无结束时间限制。
    /// 这与业务规则"NULL ≠ 0"一致：NULL 表示"不校验"。
    /// </remarks>
    private static bool IsInEffectiveRange(RuleHeader rule, DateTime businessTime)
    {
        // 生效区间按闭区间处理：小于开始时间不生效，大于结束时间不生效。
        // EffectiveFrom/EffectiveTo 为 null 时不设限。
        if (rule.EffectiveFrom.HasValue && businessTime < rule.EffectiveFrom.Value)
        {
            return false;
        }

        if (rule.EffectiveTo.HasValue && businessTime > rule.EffectiveTo.Value)
        {
            return false;
        }

        return true;
    }
}
