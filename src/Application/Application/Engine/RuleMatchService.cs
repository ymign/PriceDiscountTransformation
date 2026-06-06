using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;
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
/// 多规则的动作统一按全局动作顺序排列，确保换算 → 数量限制/互斥 → 公式折价 → TOPPRICE 封顶 → 超限归零兜底
/// 的执行顺序不被单条规则内部 SortNo 打乱。
/// </para>
/// <para>
/// 【业务时间】匹配使用 BusinessChargeTime（HIS 业务发生时间），而不是服务器当前时间。
/// 这确保补录历史费用时按历史规则时间窗口命中，而不是按补录操作时间。
/// </para>
/// <para>
/// 【核心约束引用】
/// <list type="bullet">
///   <item><description>NULL ≠ 0：条件值为 NULL 表示"不校验"，0 表示"限制为零"</description></item>
///   <item><description>HIS 兼容顺序：先做数量类限制，再用限制后的数量执行比例折价，最后做 TOPPRICE 封顶</description></item>
///   <item><description>超出 = 0元：不是拒单，不是整单归零——由 ExceedToZeroExecutor 实现</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class RuleMatchService : IRuleRuntimeCacheInvalidator
{
    private readonly EffectiveRuleSnapshotCache _snapshotCache;

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
    /// 内置动作执行顺序。在字典未维护时作为首次部署基线；已知动作之外的类型必须通过字典显式登记。
    /// </summary>
    /// <remarks>
    /// 顺序设计依据旧 HIS 正式环境的实际收费链路：
    /// <list type="number">
    ///   <item><description>CONVERT_QTY — 双单位换算，公式依赖换算后数量</description></item>
    ///   <item><description>APPLY_DAY_LIMIT_QTY — 日数量限制，先截断可收费数量</description></item>
    ///   <item><description>APPLY_TIME_WINDOW_LIMIT — 时间窗数量限制（如旧 HIS 2 小时窗），先截断可收费数量</description></item>
    ///   <item><description>APPLY_ONCE_LIMIT_QTY — 单次数量限制，先截断可收费数量</description></item>
    ///   <item><description>SAME_GROUP_MUTEX — 同组互斥，先决定当前项目是否还能收费</description></item>
    ///   <item><description>FORMULA_CALC — 公式折价，必须使用前面限制后的 FinalQty</description></item>
    ///   <item><description>APPLY_MIN_AMOUNT — 金额下限，公式之后才能比较（FIN_DISCOUNT_FEE 当前不生成该动作）</description></item>
    ///   <item><description>APPLY_MAX_AMOUNT — 金额上限/TOPPRICE，必须在比例折价之后截断</description></item>
    ///   <item><description>SAME_OPERATION_CEILING — 同手术封顶，金额类累计封顶必须在公式和单项封顶之后执行</description></item>
    ///   <item><description>ADD_CHILD_ITEM — 子项加收，不参与 FIN_DISCOUNT_FEE 公式</description></item>
    ///   <item><description>DISCOUNT_EXCEED_TO_ZERO — 超出部分归零兜底，必须最后同步超限状态</description></item>
    /// </list>
    /// 新增动作类型必须在 PR_DICT 表的 ACTION_TYPE_ORDER 类型中插入记录，否则运行期直接失败。
    /// </remarks>
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
    /// 字典类型编码：动作执行顺序。
    /// 对应 PR_DICT 表中 DICT_TYPE = "ACTION_TYPE_ORDER" 的字典项。
    /// </summary>
    private const string ActionTypeOrderDictType = "ACTION_TYPE_ORDER";

    /// <summary>
    /// 动作执行顺序缓存（静态共享）。RuleMatchService 注册为 Scoped，
    /// 若缓存放实例字段则每个 HTTP 请求都重新创建，缓存永远不命中。
    /// 改为 static 后全进程共享，首次加载后后续请求直接命中。
    /// </summary>
    private static volatile Dictionary<string, int> s_actionTypeOrderCache =
        new(DefaultActionTypeOrder, StringComparer.OrdinalIgnoreCase);

    private static readonly SemaphoreSlim s_cacheLock = new(1, 1);

    private static volatile bool s_cacheLoaded;

    /// <summary>
    /// 初始化规则匹配服务。
    /// </summary>
    /// <param name="repositories">规则匹配只读仓储集合。</param>
    /// <param name="evaluatorFactory">条件评估器工厂。</param>
    /// <param name="logger">日志对象。</param>
    public RuleMatchService(
        RuleMatchRepositories repositories,
        ConditionEvaluatorFactory evaluatorFactory,
        ILogger<RuleMatchService> logger)
        : this(repositories, CreateDefaultSnapshotCache(repositories), evaluatorFactory, logger)
    {
    }

    /// <summary>
    /// 初始化规则匹配服务。
    /// </summary>
    /// <param name="repositories">规则匹配只读仓储集合。</param>
    /// <param name="snapshotCache">运行期生效规则快照缓存。</param>
    /// <param name="evaluatorFactory">条件评估器工厂。</param>
    /// <param name="logger">日志对象。</param>
    public RuleMatchService(
        RuleMatchRepositories repositories,
        EffectiveRuleSnapshotCache snapshotCache,
        ConditionEvaluatorFactory evaluatorFactory,
        ILogger<RuleMatchService> logger)
    {
        _snapshotCache = snapshotCache;
        _evaluatorFactory = evaluatorFactory;
        _dictRepository = repositories.DictRepository;
        _logger = logger;
    }

    /// <summary>
    /// 为直接构造规则匹配服务的测试创建默认快照缓存。
    /// </summary>
    public static EffectiveRuleSnapshotCache CreateDefaultSnapshotCache(RuleMatchRepositories repositories)
    {
        return new EffectiveRuleSnapshotCache(
            new MemoryCache(new MemoryCacheOptions()),
            new EffectiveRuleSnapshotLoader(repositories));
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
    public async Task<(IReadOnlyList<RuleAggregate> Rules, IReadOnlyList<RuleAction> Actions)>
        MatchAsync(PricingContext context)
    {
        // ========== 第一阶段：按项目编码取候选规则 ==========
        // 初筛只看 ITEM_CODE，避免全表扫描。项目组规则后续可扩展为按 GROUP_CODE 查询。
        // 仓储实现应利用索引 PR_RULE_HEADER(ITEM_CODE, STATUS, IS_ENABLED) 加速查询。
        var candidates = await _snapshotCache.GetByItemCodeAsync(context.ItemCode);

        // ========== 第二阶段：过滤已发布、启用、业务时间有效的规则 ==========
        // 三重过滤：
        //   Status == "PUBLISHED"：只使用已发布的规则，草稿和审批中的规则不参与计价
        //   IsEnabled == "Y"：运行时可临时禁用规则而无需删除
        //   业务时间在生效区间内：EffectiveFrom <= BusinessChargeTime <= EffectiveTo
        // 业务时间使用 BusinessChargeTime，而不是当前系统时间，确保补录按真实收费时间匹配历史规则。
        var published = candidates
            .Where(snapshot => snapshot.Header.Status == "PUBLISHED" && snapshot.Header.IsEnabled == "Y")
            .Where(snapshot => IsInEffectiveRange(snapshot.Header, context.BusinessChargeTime))
            .ToList();

        // ========== 第三阶段：逐条评估条件组 ==========
        // 条件表支持同组 AND、跨组 OR 语义（参见 EvaluateConditionsAsync 方法）。
        // 只要任一条件组全部满足，该规则就命中。这是"OR 组，AND 组内"的经典规则引擎模式。
        var matchedRules = new List<RuleAggregate>();

        foreach (var snapshot in published)
        {
            if (await EvaluateConditionsAsync(snapshot.Conditions, context))
            {
                matchedRules.Add(snapshot.Header);
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
            var snapshot = published.First(item => item.Header.RuleId == rule.RuleId);
            allActions.AddRange(snapshot.Actions.Where(a => a.IsEnabled == "Y"));
        }
        var ruleOrder = BuildRuleOrder(matchedRules);
        var executableActions = ApplyExclusiveGroups(allActions, ruleOrder);

        // ========== 第六阶段：按全局动作顺序整理动作链 ==========
        // 多规则叠加时，如果只按每条规则的 SortNo 执行，可能出现公式先于数量限制的错误顺序。
        // OrderActions 先按全局类别排序，再按同类 SortNo 排序，确保：
        //   换算 → 数量限制/互斥 → 公式折价 → TOPPRICE 封顶 → 加收 → 归零兜底
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

    private static IReadOnlyDictionary<long, int> BuildRuleOrder(IReadOnlyList<RuleAggregate> matchedRules)
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
    private async Task<bool> EvaluateConditionsAsync(IReadOnlyList<RuleCondition> conditions, PricingContext context)
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

                if (!await evaluator.EvaluateAsync(condition, context))
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
    /// <param name="ruleOrder">规则优先级映射，用于在同一动作类型内保持规则匹配顺序。</param>
    /// <returns>
    /// 排序后的动作集合。排序规则：
    ///   1. 先按 PR_DICT 字典表定义的全局类别顺序
    ///   2. 同类动作内按规则配置的 SortNo 排序
    /// </returns>
    /// <remarks>
    /// 这个排序是资金安全的关键。如果多规则叠加时只按每条规则的 SortNo 执行，
    /// 可能出现"先执行公式计算，再执行数量限制"的错误顺序，导致比例折价没有使用旧 HIS 限制后的数量。
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
    /// 动作排序序号（数字越小越先执行）。
    /// </returns>
    /// <remarks>
    /// 未识别动作表示发布校验或字典配置漏网。运行期必须失败，避免动作被静默排到最后造成资金口径错误。
    /// 使用 OrdinalIgnoreCase 比较，确保动作类型编码大小写不敏感。
    /// </remarks>
    private int GetActionTypeSortOrder(string actionType)
    {
        if (s_actionTypeOrderCache.TryGetValue(actionType, out var order))
        {
            return order;
        }

        throw new InvalidOperationException(
            $"动作类型 {actionType} 未在 {ActionTypeOrderDictType} 字典中登记");
    }

    /// <summary>
    /// 确保动作执行顺序缓存已从字典加载。
    ///
    /// 首次调用时从 PR_DICT 表读取 DICT_TYPE = "ACTION_TYPE_ORDER" 的字典项，
    /// 构建动作类型编码到排序序号的映射字典并缓存。后续调用直接使用缓存值。
    ///
    /// 加载策略：
    ///   - 使用 SemaphoreSlim 保证线程安全，避免并发重复加载。
    ///   - 加载失败时直接抛出异常，避免字典异常被静默降级成旧顺序。
    ///   - 加载成功后设置 s_cacheLoaded 标记，后续调用跳过数据库查询。
    /// </summary>
    private async Task EnsureActionTypeOrderLoadedAsync()
    {
        // 快速路径：缓存已加载，直接返回，避免获取锁的开销。
        if (s_cacheLoaded)
        {
            return;
        }

        await s_cacheLock.WaitAsync();
        try
        {
            // 双重检查：进入锁之后再次判断，避免并发场景下重复加载。
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

    /// <summary>
    /// 从 PR_DICT 字典表加载动作执行顺序到缓存。
    ///
    /// 读取 DICT_TYPE = "ACTION_TYPE_ORDER" 的所有启用字典项，
    /// 以 DICT_CODE 为动作类型编码、SORT_NO 为排序序号，构建映射字典。
    ///
    /// 加载失败时抛出异常。动作顺序是资金安全配置，不能在字典不可用时静默继续。
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
        s_cacheLoaded = false;
        _logger.LogDebug("动作执行顺序缓存已清除，下次匹配将从字典重新加载");
    }

    /// <summary>
    /// 清除规则匹配运行期缓存。
    /// </summary>
    /// <remarks>
    /// 目前运行期缓存主要是 ACTION_TYPE_ORDER 动作顺序。通过接口暴露失效能力，
    /// 发布服务不需要知道 RuleMatchService 的具体缓存结构。
    /// </remarks>
    public void ClearRuntimeCache()
    {
        ClearActionTypeOrderCache();
        _snapshotCache.Clear();
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
    private static bool IsInEffectiveRange(RuleAggregate rule, DateTime businessTime)
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
