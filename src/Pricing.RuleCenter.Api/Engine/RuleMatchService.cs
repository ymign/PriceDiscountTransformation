using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine;

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
    /// 匹配日志，用于记录未知评估器、命中数量等运行期诊断信息。
    /// </summary>
    private readonly ILogger<RuleMatchService> _logger;

    /// <summary>
    /// 全局动作执行顺序。这个顺序高于单条规则内的 SortNo，确保所有规则先换算、再公式、再限额、最后折价。
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
    /// 新增动作类型必须插入到正确位置，否则会破坏计算顺序。
    /// </remarks>
    private static readonly string[] ActionTypeOrder =
    {
        "CONVERT_QTY",
        "FORMULA_CALC",
        "APPLY_MIN_AMOUNT",
        "APPLY_MAX_AMOUNT",
        "APPLY_DAY_LIMIT_QTY",
        "APPLY_TIME_WINDOW_LIMIT",
        "APPLY_ONCE_LIMIT_QTY",
        "SAME_GROUP_MUTEX",
        "ADD_CHILD_ITEM",
        "DISCOUNT_EXCEED_TO_ZERO"
    };

    /// <summary>
    /// 初始化规则匹配服务。
    /// </summary>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="conditionRepository">规则条件仓储。</param>
    /// <param name="actionRepository">规则动作仓储。</param>
    /// <param name="evaluatorFactory">条件评估器工厂。</param>
    /// <param name="logger">日志对象。</param>
    public RuleMatchService(
        IRuleHeaderRepository headerRepository,
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        ConditionEvaluatorFactory evaluatorFactory,
        ILogger<RuleMatchService> logger)
    {
        _headerRepository = headerRepository;
        _conditionRepository = conditionRepository;
        _actionRepository = actionRepository;
        _evaluatorFactory = evaluatorFactory;
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

        // ========== 第六阶段：按全局动作顺序整理动作链 ==========
        // 多规则叠加时，如果只按每条规则的 SortNo 执行，可能出现先限额后公式的错误顺序。
        // OrderActions 先按全局类别排序，再按同类 SortNo 排序，确保：
        //   换算 → 公式 → 下限 → 上限 → 日限 → 窗限 → 单次限 → 互斥 → 加收 → 归零
        var ordered = OrderActions(allActions);

        _logger.LogInformation(
            "规则匹配 ItemCode={ItemCode}, 命中 {RuleCount} 条规则, {ActionCount} 个动作",
            context.ItemCode, matchedRules.Count, ordered.Count);

        return (matchedRules, ordered);
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
    ///   1. 先按 ActionTypeOrder 定义的全局类别顺序
    ///   2. 同类动作内按规则配置的 SortNo 排序
    /// </returns>
    /// <remarks>
    /// 这个排序是资金安全的关键。如果多规则叠加时只按每条规则的 SortNo 执行，
    /// 可能出现"先执行限额截断，再执行公式计算"的错误顺序，导致公式结果被限额误截断。
    /// </remarks>
    private static IReadOnlyList<RuleAction> OrderActions(List<RuleAction> actions)
    {
        // 先按全局动作类别排序，再按规则配置的 SortNo 排序。
        // SortNo 只在同类动作内部生效，避免跨类别动作破坏资金计算顺序。
        return actions
            .OrderBy(a => GetActionTypeOrder(a.ActionType))
            .ThenBy(a => a.SortNo)
            .ToList();
    }

    /// <summary>
    /// 获取动作类型在全局动作链中的排序序号。
    /// </summary>
    /// <param name="actionType">动作类型编码。</param>
    /// <returns>
    /// 动作排序序号（从 0 开始）；未知动作返回 999 作为兜底序号排到最后。
    /// </returns>
    private static int GetActionTypeOrder(string actionType)
    {
        // 未识别的动作排到最后。正常情况下发布校验（PR_RULE_APPROVAL）应阻断未知动作类型；
        // 这里保留兜底排序，避免运行期因新动作暂未登记导致排序异常抛出。
        var index = Array.IndexOf(ActionTypeOrder, actionType);
        return index >= 0 ? index : 999;
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
