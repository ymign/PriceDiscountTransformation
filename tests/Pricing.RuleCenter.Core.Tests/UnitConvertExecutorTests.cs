using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class UnitConvertExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_RecalculatesAmountFromConvertedQty()
    {
        var executor = new UnitConvertExecutor();
        var context = new PricingContext
        {
            InputQty = 10m,
            ConvertedQty = 10m,
            FinalQty = 10m,
            UnitPrice = 100m,
            FinalAmount = 1000m
        };
        var action = new RuleAction
        {
            ActionType = "CONVERT_QTY",
            ParamsJson = JsonConvert.SerializeObject(new
            {
                DefaultDivisor = 4m,
                DefaultRoundMode = "CEILING"
            })
        };

        await executor.ExecuteAsync(action, context);

        Assert.Equal(3m, context.FinalQty);
        Assert.Equal(300m, context.FinalAmount);
    }
}
