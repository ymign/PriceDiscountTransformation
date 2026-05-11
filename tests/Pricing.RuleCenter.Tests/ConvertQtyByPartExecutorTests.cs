using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

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
    public async Task PricingParts为空_跳过不修改金额()
    {
        var ctx = new PricingContext { UnitPrice = 100m, FinalAmount = 100m, FinalQty = 1 };
        await _executor.ExecuteAsync(Action(), ctx);

        Assert.Equal(100m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 单病灶使用DefaultBaseArea换算()
    {
        // area=200, defaultBaseArea=100 → convertedQty=ceil(200/100)=2, amount=50×2=100
        var ctx = new PricingContext
        {
            UnitPrice = 50m,
            FinalAmount = 50m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem> { new() { Area = 200m } }
        };
        await _executor.ExecuteAsync(Action(defaultBaseArea: 100m), ctx);

        Assert.Equal(100m, ctx.FinalAmount);
        Assert.Equal(2m, ctx.ConvertedQty);
    }

    [Fact]
    public async Task 面积不足一个基准单位_按1个计()
    {
        // area=50, defaultBaseArea=100 → ceil(50/100)=1, amount=50×1=50
        var ctx = new PricingContext
        {
            UnitPrice = 50m,
            FinalAmount = 50m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem> { new() { Area = 50m } }
        };
        await _executor.ExecuteAsync(Action(defaultBaseArea: 100m), ctx);

        Assert.Equal(50m, ctx.FinalAmount);
        Assert.Equal(1m, ctx.ConvertedQty);
    }

    [Fact]
    public async Task 按部位编码命中对应基准面积()
    {
        // HEAD_FACE 基准4cm²，area=9 → ceil(9/4)=3, amount=100×3=300
        var partRules = new object[]
        {
            new { BodyPartCode = "HEAD_FACE", BaseArea = 4m },
            new { BodyPartCode = "TRUNK", BaseArea = 144m }
        };
        var ctx = new PricingContext
        {
            UnitPrice = 100m,
            FinalAmount = 100m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem> { new() { Area = 9m, BodyPartCode = "HEAD_FACE" } }
        };
        await _executor.ExecuteAsync(Action(defaultBaseArea: 100m, partRules: partRules), ctx);

        Assert.Equal(300m, ctx.FinalAmount);
        Assert.Equal(3m, ctx.ConvertedQty);
    }

    [Fact]
    public async Task 单病灶超过MaxAmountPerLesion_截断至封顶()
    {
        // area=200, defaultBaseArea=100 → qty=2, amount=50×2=100 → 超顶价80 → 截断为80
        var ctx = new PricingContext
        {
            UnitPrice = 50m,
            FinalAmount = 50m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem> { new() { Area = 200m } }
        };
        await _executor.ExecuteAsync(Action(defaultBaseArea: 100m, maxAmountPerLesion: 80m), ctx);

        Assert.Equal(80m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 多病灶各自独立封顶后相加()
    {
        // 两个病灶各area=200，单价50，defaultBaseArea=100
        // 病灶1: qty=2, amount=100 > 封顶80 → 80
        // 病灶2: qty=2, amount=100 > 封顶80 → 80
        // total = 160
        var ctx = new PricingContext
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
        await _executor.ExecuteAsync(Action(defaultBaseArea: 100m, maxAmountPerLesion: 80m), ctx);

        Assert.Equal(160m, ctx.FinalAmount);
    }

    [Fact]
    public async Task 多病灶未封顶时汇总正确()
    {
        // 两个病灶：area=100各，qty=1各，amount=50×1=50各，总100
        var ctx = new PricingContext
        {
            UnitPrice = 50m,
            FinalAmount = 50m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem>
            {
                new() { Area = 100m },
                new() { Area = 100m }
            }
        };
        await _executor.ExecuteAsync(Action(defaultBaseArea: 100m), ctx);

        Assert.Equal(100m, ctx.FinalAmount);
        Assert.Equal(2m, ctx.ConvertedQty);
    }

    [Fact]
    public async Task 病灶部位为空时回落到context中的BodyPartCode()
    {
        var partRules = new object[] { new { BodyPartCode = "HEAD_FACE", BaseArea = 4m } };
        var ctx = new PricingContext
        {
            UnitPrice = 100m,
            FinalAmount = 100m,
            FinalQty = 1,
            BodyPartCode = "HEAD_FACE",
            PricingParts = new List<PricingPartItem> { new() { Area = 8m, BodyPartCode = null } }
        };
        await _executor.ExecuteAsync(Action(defaultBaseArea: 100m, partRules: partRules), ctx);

        // area=8, HEAD_FACE baseArea=4 → ceil(8/4)=2, amount=100×2=200
        Assert.Equal(200m, ctx.FinalAmount);
    }

    [Fact]
    public async Task ExecutorCode不匹配_跳过不修改金额()
    {
        var ctx = new PricingContext
        {
            UnitPrice = 100m,
            FinalAmount = 100m,
            FinalQty = 1,
            PricingParts = new List<PricingPartItem> { new() { Area = 200m } }
        };
        var action = new RuleAction { ActionType = "FORMULA_CALC", ExecutorCode = "OTHER" };
        await _executor.ExecuteAsync(action, ctx);

        Assert.Equal(100m, ctx.FinalAmount);
    }
}
