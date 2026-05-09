using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

public sealed class ExceedToZeroExecutor : IRuleActionExecutor
{
    public string ActionType => "EXCEED_TO_ZERO";

    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        if (context.FinalQty <= 0)
        {
            context.FinalAmount = 0;
        }

        return Task.CompletedTask;
    }
}
