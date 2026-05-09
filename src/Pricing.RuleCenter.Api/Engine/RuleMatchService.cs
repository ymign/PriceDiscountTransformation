using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine;

/// <summary>
/// 规则匹配服务，负责从已发布规则中找出本次计价应执行的规则和动作链。
/// </summary>
/// <remarks>
/// 规则匹配分为三层：先按项目编码和生效时间筛选候选规则，再按条件组执行 AND/OR 判断，
/// 最后收集全部命中规则的动作并按全局动作顺序排序。这样可以支持多规则叠加，同时避免
/// 换算、公式、限额、折价动作因为规则优先级不同而乱序执行。
/// </remarks>
public sealed class RuleMatchService
{
    /// <summary>
    /// 规则主档仓储，用于按项目读取候选规则。
    /// </summary>
    private readonly IRuleHeaderRepository _headerRepository;
    /// <summary>
    /// 规则条件仓储，用于读取候选规则当前版本下的条件集合。
    /// </summary>
    private readonly IRuleConditionRepository _conditionRepository;
    /// <summary>
    /// 规则动作仓储，用于读取命中规则当前版本下的动作链。
    /// </summary>
    private readonly IRuleActionRepository _actionRepository;
    /// <summary>
    /// 条件评估器工厂，用于按条件类型选择具体评估器。
    /// </summary>
    private readonly ConditionEvaluatorFactory _evaluatorFactory;
    /// <summary>
    /// 匹配日志，用于记录未知评估器和最终命中数量。
    /// </summary>
    private readonly ILogger<RuleMatchService> _logger;

    /// <summary>
    /// 全局动作执行顺序。这个顺序高于单条规则内的 SortNo，确保所有规则先换算、再公式、再限额、最后折价。
    /// </summary>
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
    /// <param name="context">计价上下文。</param>
    /// <returns>命中的规则集合，以及按全局顺序排列后的动作集合。</returns>
    public async Task<(IReadOnlyList<RuleHeader> Rules, IReadOnlyList<RuleAction> Actions)>
        MatchAsync(PricingContext context)
    {
        // ========== 第一阶段：按项目编码取候选规则 ==========
        // 初筛只看 ITEM_CODE，避免全表扫描。项目组规则后续可扩展为按 GROUP_CODE 查询。
        var candidates = await _headerRepository.GetByItemCodeAsync(context.ItemCode);

        // ========== 第二阶段：过滤已发布、启用、业务时间有效的规则 ==========
        // 业务时间使用 BusinessChargeTime，而不是当前系统时间，确保补录按真实收费时间匹配历史规则。
        var published = candidates
            .Where(r => r.Status == "PUBLISHED" && r.IsEnabled == "Y")
            .Where(r => IsInEffectiveRange(r, context.BusinessChargeTime))
            .ToList();

        // ========== 第三阶段：逐条评估条件组 ==========
        // 条件表支持同组 AND、跨组 OR。只要任一条件组全部满足，该规则就命中。
        var matchedRules = new List<RuleHeader>();

        foreach (var rule in published)
        {
            var conditions = await _conditionRepository.GetByRuleAndVersionAsync(
                rule.RuleId, rule.CurrentVersion);

            if (EvaluateConditions(conditions, context))
            {
                matchedRules.Add(rule);
            }
        }

        // ========== 第四阶段：按规则优先级排序 ==========
        // 数字越小优先级越高。后续同一动作类型内部再用 SortNo 排序。
        matchedRules.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        // ========== 第五阶段：收集命中规则的动作 ==========
        // 动作是否真正执行由 ActionExecutionPipeline 和具体 Executor 决定。
        var allActions = new List<RuleAction>();
        foreach (var rule in matchedRules)
        {
            var actions = await _actionRepository.GetByRuleAndVersionAsync(
                rule.RuleId, rule.CurrentVersion);
            allActions.AddRange(actions.Where(a => a.IsEnabled == "Y"));
        }

        // ========== 第六阶段：按全局动作顺序整理动作链 ==========
        // 多规则叠加时，如果只按每条规则 SortNo 执行，可能出现先限额后公式的错误顺序。
        var ordered = OrderActions(allActions);

        _logger.LogInformation(
            "规则匹配 ItemCode={ItemCode}, 命中 {RuleCount} 条规则, {ActionCount} 个动作",
            context.ItemCode, matchedRules.Count, ordered.Count);

        return (matchedRules, ordered);
    }

    /// <summary>
    /// 按条件组判断当前上下文是否满足规则条件集合。
    /// </summary>
    /// <param name="conditions">规则条件集合。</param>
    /// <param name="context">计价上下文。</param>
    /// <returns>任一条件组全部满足时返回 <c>true</c>。</returns>
    private bool EvaluateConditions(IReadOnlyList<RuleCondition> conditions, PricingContext context)
    {
        // 没有条件的规则视为兜底规则。这样可以配置“只要项目命中就执行”的简单规则。
        if (conditions.Count == 0)
        {
            return true;
        }

        // 所有条件都禁用时同样按兜底规则处理，便于临时关闭某些附加条件。
        var enabled = conditions.Where(c => c.IsEnabled == "Y").ToList();
        if (enabled.Count == 0)
        {
            return true;
        }

        // 同一 CONDITION_GROUP 内是 AND，不同组之间是 OR。
        var groups = enabled.GroupBy(c => c.ConditionGroup);

        foreach (var group in groups)
        {
            var allMatch = true;
            foreach (var condition in group)
            {
                var evaluator = _evaluatorFactory.GetEvaluator(condition.ConditionType);
                if (evaluator is null)
                {
                    // 找不到评估器时不能默认匹配。否则未知条件会被忽略，导致规则误命中。
                    _logger.LogWarning("未找到条件评估器: {ConditionType}", condition.ConditionType);
                    allMatch = false;
                    break;
                }

                if (!evaluator.Evaluate(condition, context))
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 按全局动作类别顺序和动作内排序号整理动作链。
    /// </summary>
    /// <param name="actions">待排序动作集合。</param>
    /// <returns>排序后的动作集合。</returns>
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
    /// 获取动作类型在全局动作链中的顺序。
    /// </summary>
    /// <param name="actionType">动作类型编码。</param>
    /// <returns>动作排序序号，未知动作返回较大的兜底序号。</returns>
    private static int GetActionTypeOrder(string actionType)
    {
        // 未识别的动作排到最后。正常情况下发布校验应阻断未知动作；
        // 这里保留兜底排序，避免运行期因新动作暂未登记导致排序异常。
        var index = Array.IndexOf(ActionTypeOrder, actionType);
        return index >= 0 ? index : 999;
    }

    /// <summary>
    /// 判断业务时间是否落在规则生效区间内。
    /// </summary>
    /// <param name="rule">规则主档。</param>
    /// <param name="businessTime">业务收费时间。</param>
    /// <returns>在生效区间内返回 <c>true</c>。</returns>
    private static bool IsInEffectiveRange(RuleHeader rule, DateTime businessTime)
    {
        // 生效区间按闭区间处理：小于开始时间不生效，大于结束时间不生效。
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
