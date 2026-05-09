using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

public sealed class ItemMatchEvaluator : IRuleConditionEvaluator
{
    public string ConditionType => "ITEM_MATCH";

    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return false;
        }

        return string.Equals(context.ItemCode, condition.RightValue, StringComparison.OrdinalIgnoreCase);
    }
}
