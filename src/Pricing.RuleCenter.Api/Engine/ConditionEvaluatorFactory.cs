using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Api.Engine;

public sealed class ConditionEvaluatorFactory
{
    private readonly Dictionary<string, IRuleConditionEvaluator> _evaluators;

    public ConditionEvaluatorFactory(IEnumerable<IRuleConditionEvaluator> evaluators)
    {
        _evaluators = evaluators.ToDictionary(e => e.ConditionType, StringComparer.OrdinalIgnoreCase);
    }

    public IRuleConditionEvaluator? GetEvaluator(string conditionType)
    {
        _evaluators.TryGetValue(conditionType, out var evaluator);
        return evaluator;
    }
}
