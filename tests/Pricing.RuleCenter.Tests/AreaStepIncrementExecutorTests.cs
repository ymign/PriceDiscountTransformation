using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class AreaStepIncrementExecutorTests
{
    private readonly AreaStepIncrementExecutor _executor = new();

    private static RuleAction Action(decimal baseArea = 15m, decimal stepRate = 0.15m) =>
        new()
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "AREA_STEP_INCREMENT",
            ParamsJson = JsonConvert.SerializeObject(new { BaseArea = baseArea, StepRate = stepRate })
        };

    private static PricingContext Context(decimal unitPrice, params decimal[] areas)
    {
        var ctx = new PricingContext { UnitPrice = unitPrice, FinalAmount = unitPrice, FinalQty = 1 };
        if (areas.Length > 0)
        {
            ctx.PricingParts = areas.Select((a, i) => new PricingPartItem { PartSeq = i + 1, Area = a }).ToList();
        }
        return ctx;
    }

    [Fact]
    public async Task 无明细面积为零_金额等于单价()
    {
        var ctx = Context(200m);
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.Equal(200m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 面积等于基础面积15_步数0_金额等于单价()
    {
        var ctx = Context(200m, 15m);
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.Equal(200m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 面积小于基础面积_步数0_金额等于单价()
    {
        var ctx = Context(200m, 10m);
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.Equal(200m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 面积16_步数1_金额加15()
    {
        // steps=ceil((16-15)/15)=1, amount=200×(1+1×0.15)=230
        var ctx = Context(200m, 16m);
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.Equal(230m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 面积30_步数1_金额加15()
    {
        // steps=ceil((30-15)/15)=1, amount=200×1.15=230
        var ctx = Context(200m, 30m);
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.Equal(230m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 面积31_步数2_金额加30()
    {
        // steps=ceil((31-15)/15)=ceil(1.067)=2, amount=200×(1+2×0.15)=260
        var ctx = Context(200m, 31m);
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.Equal(260m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 多片段面积求和_正确计算步数()
    {
        // 两片段面积 12+6=18，steps=ceil((18-15)/15)=1, amount=200×1.15=230
        var ctx = Context(200m, 12m, 6m);
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.Equal(230m, ctx.FinalAmount);
    }

    [Fact]
    public async Task ExecutorCode不匹配_跳过不修改金额()
    {
        var ctx = Context(200m, 30m);
        var action = new RuleAction
        {
            ActionType = "FORMULA_CALC",
            ExecutorCode = "OTHER_EXECUTOR",
            ParamsJson = JsonConvert.SerializeObject(new { BaseArea = 15m, StepRate = 0.15m })
        };

        await _executor.ExecuteAsync(action, ctx);

        Assert.Equal(200m, ctx.FinalAmount);
    }

    [Fact]
    public async Task FormulaAmount写入正确()
    {
        var ctx = Context(200m, 16m);
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.Equal(ctx.FinalAmount, ctx.FormulaAmount);
    }

    [Fact]
    public async Task 追踪步骤已记录()
    {
        var ctx = Context(200m, 16m);
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.NotEmpty(ctx.TraceSteps);
    }
}
