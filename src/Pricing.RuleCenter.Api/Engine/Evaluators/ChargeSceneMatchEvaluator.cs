using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

public sealed class ChargeSceneMatchEvaluator : IRuleConditionEvaluator
{
    public string ConditionType => "CHARGE_SCENE";

    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        return string.Equals(context.ChargeScene, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
