using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

public sealed class BodyPartMatchEvaluator : IRuleConditionEvaluator
{
    public string ConditionType => "BODY_PART";

    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        return string.Equals(context.BodyPartCode, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
