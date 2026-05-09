using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine;

public sealed class RuleMatchService
{
    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IRuleConditionRepository _conditionRepository;
    private readonly IRuleActionRepository _actionRepository;
    private readonly ConditionEvaluatorFactory _evaluatorFactory;
    private readonly ILogger<RuleMatchService> _logger;

    private static readonly string[] ActionTypeOrder =
    {
        "UNIT_CONVERT",
        "FORMULA_CALC",
        "AMOUNT_FLOOR",
        "AMOUNT_CEILING",
        "DAILY_QTY_LIMIT",
        "TIME_WINDOW_LIMIT",
        "SINGLE_QTY_LIMIT",
        "EXCLUSIVE_GROUP",
        "SURGERY_CAP",
        "CHILD_ITEM_SURCHARGE",
        "EXCEED_TO_ZERO"
    };

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

    public async Task<(IReadOnlyList<RuleHeader> Rules, IReadOnlyList<RuleAction> Actions)>
        MatchAsync(PricingContext context)
    {
        var candidates = await _headerRepository.GetByItemCodeAsync(context.ItemCode);

        var published = candidates
            .Where(r => r.Status == "PUBLISHED" && r.IsEnabled == "Y")
            .Where(r => IsInEffectiveRange(r, context.BusinessChargeTime))
            .ToList();

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

        matchedRules.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        var allActions = new List<RuleAction>();
        foreach (var rule in matchedRules)
        {
            var actions = await _actionRepository.GetByRuleAndVersionAsync(
                rule.RuleId, rule.CurrentVersion);
            allActions.AddRange(actions.Where(a => a.IsEnabled == "Y"));
        }

        var ordered = OrderActions(allActions);

        _logger.LogInformation(
            "规则匹配 ItemCode={ItemCode}, 命中 {RuleCount} 条规则, {ActionCount} 个动作",
            context.ItemCode, matchedRules.Count, ordered.Count);

        return (matchedRules, ordered);
    }

    private bool EvaluateConditions(IReadOnlyList<RuleCondition> conditions, PricingContext context)
    {
        if (conditions.Count == 0)
        {
            return true;
        }

        var enabled = conditions.Where(c => c.IsEnabled == "Y").ToList();
        if (enabled.Count == 0)
        {
            return true;
        }

        var groups = enabled.GroupBy(c => c.ConditionGroup);

        foreach (var group in groups)
        {
            var allMatch = true;
            foreach (var condition in group)
            {
                var evaluator = _evaluatorFactory.GetEvaluator(condition.ConditionType);
                if (evaluator is null)
                {
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

    private static IReadOnlyList<RuleAction> OrderActions(List<RuleAction> actions)
    {
        return actions
            .OrderBy(a => GetActionTypeOrder(a.ActionType))
            .ThenBy(a => a.SortNo)
            .ToList();
    }

    private static int GetActionTypeOrder(string actionType)
    {
        var index = Array.IndexOf(ActionTypeOrder, actionType);
        return index >= 0 ? index : 999;
    }

    private static bool IsInEffectiveRange(RuleHeader rule, DateTime businessTime)
    {
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
