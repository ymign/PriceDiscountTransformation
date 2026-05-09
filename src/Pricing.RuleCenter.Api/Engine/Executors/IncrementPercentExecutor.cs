using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

public sealed class IncrementPercentExecutor : IRuleActionExecutor
{
    public string ActionType => "FORMULA_CALC";

    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        if (action.ExecutorCode != "INCREMENT_PERCENT")
        {
            return Task.CompletedTask;
        }

        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return Task.CompletedTask;
        }

        var baseAmount = context.UnitPrice * context.ConvertedQty;
        var increment = baseAmount * (param.Percent / 100m);
        context.FormulaAmount = baseAmount + increment;
        context.FinalAmount = context.FormulaAmount;

        return Task.CompletedTask;
    }

    private static IncrementPercentParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<IncrementPercentParams>(json);
    }

    private sealed class IncrementPercentParams
    {
        public decimal Percent { get; set; }
    }
}
