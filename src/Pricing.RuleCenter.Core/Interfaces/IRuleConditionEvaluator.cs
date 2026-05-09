using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IRuleConditionEvaluator
{
    string ConditionType { get; }
    bool Evaluate(RuleCondition condition, PricingContext context);
}
