using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

public sealed class AmountFloorExecutor : IRuleActionExecutor
{
    public string ActionType => "AMOUNT_FLOOR";

    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        var param = DeserializeParams(action.ParamsJson);
        if (param?.FloorAmount is null)
        {
            return Task.CompletedTask;
        }

        if (context.FinalAmount < param.FloorAmount.Value)
        {
            context.FinalAmount = param.FloorAmount.Value;
        }

        return Task.CompletedTask;
    }

    private static AmountFloorParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<AmountFloorParams>(json);
    }

    private sealed class AmountFloorParams
    {
        public decimal? FloorAmount { get; set; }
    }
}
