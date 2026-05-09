using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Evaluators;

public sealed class TimeRangeEvaluator : IRuleConditionEvaluator
{
    public string ConditionType => "TIME_RANGE";

    public bool Evaluate(RuleCondition condition, PricingContext context)
    {
        if (string.IsNullOrEmpty(condition.RightValue))
        {
            return true;
        }

        var parts = condition.RightValue.Split('~', 2);
        if (parts.Length != 2)
        {
            return false;
        }

        if (!DateTime.TryParse(parts[0].Trim(), out var from) ||
            !DateTime.TryParse(parts[1].Trim(), out var to))
        {
            return false;
        }

        return context.BusinessChargeTime >= from && context.BusinessChargeTime <= to;
    }
}
