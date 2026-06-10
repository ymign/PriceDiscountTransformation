using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class ChildItemPercentExecutorTests
{
    private readonly ChildItemPercentExecutor _executor = new();

    private static RuleAction Action(
        string parentItemCode,
        decimal childRate = 0m,
        decimal childPercent = 0m)
    {
        return new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "CHILD_ITEM_PERCENT",
            ParamsJson = JsonConvert.SerializeObject(new
            {
                ParentItemCode = parentItemCode,
                ChildRate = childRate,
                ChildPercent = childPercent
            })
        };
    }

    private static PricingContext ContextWithParentAmount(decimal parentFinalAmount)
    {
        var sharedState = new RequestSharedPricingState();
        sharedState.SetParentItemAmount("PARENT001", parentFinalAmount);
        return new PricingContext
        {
            UnitPrice = 0m,
            FinalAmount = 0m,
            FinalQty = 1,
            RequestSharedState = sharedState
        };
    }

    [Fact]
    public async Task ExecuteAsync_calculates_child_amount_by_child_rate()
    {
        var context = ContextWithParentAmount(1000m);

        await _executor.ExecuteAsync(Action("PARENT001", childRate: 0.30m), context);

        Assert.Equal(300m, context.FinalAmount);
        Assert.Equal(300m, context.FormulaAmount);
        Assert.NotEmpty(context.TraceSteps);
    }

    [Fact]
    public async Task ExecuteAsync_uses_child_percent_when_child_rate_is_zero()
    {
        var context = ContextWithParentAmount(1000m);

        await _executor.ExecuteAsync(Action("PARENT001", childPercent: 30m), context);

        Assert.Equal(300m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_prefers_child_rate_over_child_percent()
    {
        var context = ContextWithParentAmount(1000m);

        await _executor.ExecuteAsync(Action("PARENT001", childRate: 0.20m, childPercent: 30m), context);

        Assert.Equal(200m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_finds_parent_amount_case_insensitively()
    {
        var context = ContextWithParentAmount(500m);

        await _executor.ExecuteAsync(Action("parent001", childRate: 0.30m), context);

        Assert.Equal(150m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_skips_when_parent_item_is_not_in_current_batch()
    {
        var context = new PricingContext
        {
            UnitPrice = 50m,
            FinalAmount = 50m,
            FinalQty = 1,
            RequestSharedState = new RequestSharedPricingState()
        };

        await _executor.ExecuteAsync(Action("PARENT001", childRate: 0.30m), context);

        Assert.Equal(50m, context.FinalAmount);
        Assert.NotEmpty(context.TraceSteps);
    }

    [Fact]
    public async Task ExecuteAsync_skips_when_parent_item_code_is_empty()
    {
        var context = new PricingContext
        {
            FinalAmount = 50m,
            FinalQty = 1
        };

        await _executor.ExecuteAsync(Action(string.Empty, childRate: 0.30m), context);

        Assert.Equal(50m, context.FinalAmount);
    }

    [Fact]
    public async Task ExecuteAsync_ignores_action_when_executor_code_does_not_match()
    {
        var context = new PricingContext
        {
            FinalAmount = 50m,
            FinalQty = 1
        };
        var action = new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "OTHER_EXECUTOR",
            ParamsJson = JsonConvert.SerializeObject(new { ParentItemCode = "P001", ChildRate = 0.30m })
        };

        await _executor.ExecuteAsync(action, context);

        Assert.Equal(50m, context.FinalAmount);
    }
}
