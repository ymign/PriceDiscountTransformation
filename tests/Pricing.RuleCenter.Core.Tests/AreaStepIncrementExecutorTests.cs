using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class AreaStepIncrementExecutorTests
{
    private readonly AreaStepIncrementExecutor _executor = new();

    private static RuleAction Action(decimal baseArea = 15m, decimal stepRate = 0.15m)
    {
        return new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "AREA_STEP_INCREMENT",
            ParamsJson = JsonConvert.SerializeObject(new { BaseArea = baseArea, StepRate = stepRate })
        };
    }

    private static PricingContext Context(decimal unitPrice, params decimal[] areas)
    {
        var context = new PricingContext
        {
            UnitPrice = unitPrice,
            FinalAmount = unitPrice,
            FinalQty = 1
        };

        if (areas.Length > 0)
        {
            context.PricingParts = areas
                .Select((area, index) => new PricingPartItem { PartSeq = index + 1, Area = area })
                .ToList();
        }

        return context;
    }

    [Fact]
    public async Task ExecuteAsync_keeps_unit_price_when_area_is_missing_or_not_greater_than_base_area()
    {
        var withoutArea = Context(200m);
        var baseArea = Context(200m, 15m);
        var belowBaseArea = Context(200m, 10m);

        await _executor.ExecuteAsync(Action(), withoutArea);
        await _executor.ExecuteAsync(Action(), baseArea);
        await _executor.ExecuteAsync(Action(), belowBaseArea);

        Assert.Equal(200m, withoutArea.FinalAmount);
        Assert.Equal(200m, baseArea.FinalAmount);
        Assert.Equal(200m, belowBaseArea.FinalAmount);
    }

    [Theory]
    [InlineData(16, 230)]
    [InlineData(30, 230)]
    [InlineData(31, 260)]
    public async Task ExecuteAsync_calculates_area_step_increment(decimal area, decimal expectedAmount)
    {
        var context = Context(200m, area);

        await _executor.ExecuteAsync(Action(), context);

        Assert.Equal(expectedAmount, context.FinalAmount);
        Assert.Equal(expectedAmount, context.FormulaAmount);
    }

    [Fact]
    public async Task ExecuteAsync_sums_multiple_part_areas_before_calculating_steps()
    {
        var context = Context(200m, 12m, 6m);

        await _executor.ExecuteAsync(Action(), context);

        Assert.Equal(230m, context.FinalAmount);
        Assert.NotEmpty(context.TraceSteps);
    }

    [Fact]
    public async Task ExecuteAsync_ignores_action_when_executor_code_does_not_match()
    {
        var context = Context(200m, 30m);
        var action = new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "OTHER_EXECUTOR",
            ParamsJson = JsonConvert.SerializeObject(new { BaseArea = 15m, StepRate = 0.15m })
        };

        await _executor.ExecuteAsync(action, context);

        Assert.Equal(200m, context.FinalAmount);
    }
}
