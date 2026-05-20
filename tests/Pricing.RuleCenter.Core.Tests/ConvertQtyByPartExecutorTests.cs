using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class ConvertQtyByPartExecutorTests
{
    private readonly ConvertQtyByPartExecutor _executor = new();

    private static RuleAction Action(
        decimal defaultBaseArea = 100m,
        decimal? maxAmountPerLesion = null,
        object[]? partRules = null)
    {
        return new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "CONVERT_QTY_BY_PART",
            ParamsJson = JsonConvert.SerializeObject(new
            {
                DefaultBaseArea = defaultBaseArea,
                MaxAmountPerLesion = maxAmountPerLesion,
                PartConvertRules = partRules
            })
        };
    }

    [Fact]
    public async Task ExecuteAsync_skips_when_pricing_parts_are_missing()
    {
        var context = new PricingContext
        {
            UnitPrice = 100m,
            FinalAmount = 100m,
            FinalQty = 1
        };

        await _executor.ExecuteAsync(Action(), context);

        Assert.Equal(100m, context.FinalAmount);
        Assert.NotEmpty(context.TraceSteps);
    }

    [Theory]
    [InlineData(200, 100, 2, 100)]
    [InlineData(50, 100, 1, 50)]
    public async Task ExecuteAsync_converts_area_to_qty_by_default_base_area(
        decimal area,
        decimal defaultBaseArea,
        decimal expectedQty,
        decimal expectedAmount)
    {
        var context = new PricingContext
        {
            UnitPrice = 50m,
            FinalAmount = 50m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem> { new() { Area = area } }
        };

        await _executor.ExecuteAsync(Action(defaultBaseArea: defaultBaseArea), context);

        Assert.Equal(expectedQty, context.ConvertedQty);
        Assert.Equal(expectedAmount, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_uses_matching_body_part_base_area()
    {
        var partRules = new object[]
        {
            new { BodyPartCode = "HEAD_FACE", BaseArea = 4m },
            new { BodyPartCode = "TRUNK", BaseArea = 144m }
        };
        var context = new PricingContext
        {
            UnitPrice = 100m,
            FinalAmount = 100m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem>
            {
                new() { Area = 9m, BodyPartCode = "HEAD_FACE" }
            }
        };

        await _executor.ExecuteAsync(Action(defaultBaseArea: 100m, partRules: partRules), context);

        Assert.Equal(3m, context.ConvertedQty);
        Assert.Equal(300m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_applies_max_amount_per_lesion_independently()
    {
        var context = new PricingContext
        {
            UnitPrice = 50m,
            FinalAmount = 50m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem>
            {
                new() { Area = 200m },
                new() { Area = 200m }
            }
        };

        await _executor.ExecuteAsync(
            Action(defaultBaseArea: 100m, maxAmountPerLesion: 80m),
            context);

        Assert.Equal(4m, context.ConvertedQty);
        Assert.Equal(160m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_falls_back_to_context_body_part_when_part_body_part_is_empty()
    {
        var partRules = new object[]
        {
            new { BodyPartCode = "HEAD_FACE", BaseArea = 4m }
        };
        var context = new PricingContext
        {
            UnitPrice = 100m,
            FinalAmount = 100m,
            FinalQty = 1,
            BodyPartCode = "HEAD_FACE",
            PricingParts = new List<PricingPartItem>
            {
                new() { Area = 8m }
            }
        };

        await _executor.ExecuteAsync(Action(defaultBaseArea: 100m, partRules: partRules), context);

        Assert.Equal(2m, context.ConvertedQty);
        Assert.Equal(200m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_ignores_action_when_executor_code_does_not_match()
    {
        var context = new PricingContext
        {
            UnitPrice = 100m,
            FinalAmount = 100m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem> { new() { Area = 200m } }
        };
        var action = new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "OTHER"
        };

        await _executor.ExecuteAsync(action, context);

        Assert.Equal(100m, context.FinalAmount);
    }
}
