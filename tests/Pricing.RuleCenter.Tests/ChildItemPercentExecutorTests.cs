using System.Text.Json;
using Pricing.RuleCenter.Application.Engine.Executors;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ChildItemPercentExecutorTests
{
    private readonly ChildItemPercentExecutor _executor = new();

    private static RuleAction Action(string parentItemCode, decimal childRate = 0m, decimal childPercent = 0m) =>
        new()
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "CHILD_ITEM_PERCENT",
            ParamsJson = JsonSerializer.Serialize(new
            {
                ParentItemCode = parentItemCode,
                ChildRate = childRate,
                ChildPercent = childPercent
            })
        };

    private static RequestSharedPricingState CreateSharedState(decimal parentFinalAmount)
    {
        var state = new RequestSharedPricingState();
        state.SetParentItemAmount("PARENT001", parentFinalAmount);
        return state;
    }

    [Fact]
    public async Task 父项金额存在_ChildRate计算子项金额()
    {
        var ctx = new PricingContext
        {
            UnitPrice = 0m,
            FinalAmount = 0m,
            FinalQty = 1,
            RequestSharedState = CreateSharedState(1000m)
        };

        await _executor.ExecuteAsync(Action("PARENT001", childRate: 0.30m), ctx);

        Assert.Equal(300m, ctx.FinalAmount);
        Assert.Equal(300m, ctx.FormulaAmount);
    }

    [Fact]
    public async Task ChildPercent_30等效于ChildRate_0_30()
    {
        var ctx = new PricingContext
        {
            UnitPrice = 0m,
            FinalAmount = 0m,
            FinalQty = 1,
            RequestSharedState = CreateSharedState(1000m)
        };

        await _executor.ExecuteAsync(Action("PARENT001", childRate: 0m, childPercent: 30m), ctx);

        Assert.Equal(300m, ctx.FinalAmount);
    }

    [Fact]
    public async Task ChildRate非零时优先于ChildPercent()
    {
        // ChildRate=0.20，ChildPercent=30 → 应用0.20，不应用30%
        var ctx = new PricingContext
        {
            UnitPrice = 0m,
            FinalAmount = 0m,
            FinalQty = 1,
            RequestSharedState = CreateSharedState(1000m)
        };

        await _executor.ExecuteAsync(Action("PARENT001", childRate: 0.20m, childPercent: 30m), ctx);

        Assert.Equal(200m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 父项不在同批次_静默跳过()
    {
        var ctx = new PricingContext
        {
            UnitPrice = 50m,
            FinalAmount = 50m,
            FinalQty = 1,
            RequestSharedState = new RequestSharedPricingState()
        };

        await _executor.ExecuteAsync(Action("PARENT001", childRate: 0.30m), ctx);

        // 父项不在上下文中，跳过，FinalAmount 不变
        Assert.Equal(50m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 父项编码大写不敏感_能找到()
    {
        // 存入的键是大写，传入的 parentItemCode 是小写，应能匹配
        var ctx = new PricingContext
        {
            UnitPrice = 0m,
            FinalAmount = 0m,
            FinalQty = 1,
            RequestSharedState = CreateSharedState(500m)
        };

        await _executor.ExecuteAsync(Action("parent001", childRate: 0.30m), ctx);

        Assert.Equal(150m, ctx.FinalAmount);
    }

    [Fact]
    public async Task ParentItemCode为空_跳过()
    {
        var ctx = new PricingContext { FinalAmount = 50m, FinalQty = 1 };
        var action = new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "CHILD_ITEM_PERCENT",
            ParamsJson = JsonSerializer.Serialize(new { ParentItemCode = "", ChildRate = 0.30m })
        };

        await _executor.ExecuteAsync(action, ctx);

        Assert.Equal(50m, ctx.FinalAmount);
    }

    [Fact]
    public async Task ExecutorCode不匹配_跳过()
    {
        var ctx = new PricingContext { FinalAmount = 50m, FinalQty = 1 };
        var action = new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "OTHER_EXECUTOR",
            ParamsJson = JsonSerializer.Serialize(new { ParentItemCode = "P001", ChildRate = 0.30m })
        };

        await _executor.ExecuteAsync(action, ctx);

        Assert.Equal(50m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 父项金额为零_子项金额为零()
    {
        var ctx = new PricingContext
        {
            UnitPrice = 0m,
            FinalAmount = 100m,
            FinalQty = 1,
            RequestSharedState = CreateSharedState(0m)
        };

        await _executor.ExecuteAsync(Action("PARENT001", childRate: 0.30m), ctx);

        Assert.Equal(0m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 追踪步骤已记录()
    {
        var ctx = new PricingContext
        {
            UnitPrice = 0m,
            FinalAmount = 0m,
            FinalQty = 1,
            RequestSharedState = CreateSharedState(1000m)
        };

        await _executor.ExecuteAsync(Action("PARENT001", childRate: 0.30m), ctx);

        Assert.NotEmpty(ctx.TraceSteps);
    }
}
