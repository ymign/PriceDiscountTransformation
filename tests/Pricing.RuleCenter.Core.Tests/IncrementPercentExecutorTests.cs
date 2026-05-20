using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class IncrementPercentExecutorTests
{
    private readonly IncrementPercentExecutor _executor = new();

    [Fact]
    public async Task ExecuteAsync_calculates_first_unit_full_price_and_remaining_units_by_rate()
    {
        var context = new PricingContext
        {
            UnitPrice = 200m,
            ConvertedQty = 3m,
            FinalQty = 3m,
            FinalAmount = 600m
        };
        var action = new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "INCREMENT_PERCENT",
            ParamsJson = JsonConvert.SerializeObject(new { Rate = 0.5m })
        };

        await _executor.ExecuteAsync(action, context);

        Assert.Equal(400m, context.FormulaAmount);
        Assert.Equal(400m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_uses_limited_final_qty_after_his_quantity_limit()
    {
        var context = new PricingContext
        {
            UnitPrice = 100m,
            ConvertedQty = 3m,
            FinalQty = 2m,
            FinalAmount = 200m
        };
        var action = new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "INCREMENT_PERCENT",
            ParamsJson = JsonConvert.SerializeObject(new { Rate = 0.5m })
        };

        await _executor.ExecuteAsync(action, context);

        Assert.Equal(150m, context.FormulaAmount);
        Assert.Equal(150m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_keeps_amount_zero_when_quantity_limit_blocks_all_qty()
    {
        var context = new PricingContext
        {
            UnitPrice = 100m,
            ConvertedQty = 3m,
            FinalQty = 0m,
            FinalAmount = 0m
        };
        var action = new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "INCREMENT_PERCENT",
            ParamsJson = JsonConvert.SerializeObject(new { Rate = 0.5m })
        };

        await _executor.ExecuteAsync(action, context);

        Assert.Equal(0m, context.FormulaAmount);
        Assert.Equal(0m, context.FinalAmount);
    }
}
