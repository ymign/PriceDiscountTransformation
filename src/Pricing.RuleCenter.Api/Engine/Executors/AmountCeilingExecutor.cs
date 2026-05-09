using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

public sealed class AmountCeilingExecutor : IRuleActionExecutor
{
    public string ActionType => "AMOUNT_CEILING";

    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        var param = DeserializeParams(action.ParamsJson);
        if (param?.CeilingAmount is null)
        {
            return Task.CompletedTask;
        }

        if (context.FinalAmount > param.CeilingAmount.Value)
        {
            context.FinalAmount = param.CeilingAmount.Value;
        }

        return Task.CompletedTask;
    }

    private static AmountCeilingParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<AmountCeilingParams>(json);
    }

    private sealed class AmountCeilingParams
    {
        public decimal? CeilingAmount { get; set; }
    }
}
