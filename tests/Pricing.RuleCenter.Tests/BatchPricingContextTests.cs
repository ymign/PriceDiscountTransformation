using System.Reflection;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class BatchPricingContextTests
{
    [Fact]
    public void InjectBatchContext_AddsMutexAndOperationKeysWithoutDuplicatingOccupies()
    {
        var occupy = new LimitOccupy
        {
            LimitType = "TIME_WINDOW",
            LimitDimensionCode = "P001:ITEM001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0),
            OccupyQty = 2m
        };
        var context = new PricingContext
        {
            InRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>
            {
                ["TIME_WINDOW:P001:ITEM001"] = 2m
            },
            InRequestLimitOccupies = new[] { occupy }
        };
        var batchContext = new BatchPricingContext
        {
            InBatchOccupiedQtyByDimension = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                ["TIME_WINDOW:P001:ITEM001"] = 2m
            },
            InBatchItemCountByGroup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["GROUP_A"] = 1
            },
            InBatchOccupiedAmtByOperation = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                ["OP001:GROUP_A"] = 30m
            },
            InBatchLimitOccupies = new List<LimitOccupy> { occupy }
        };

        InvokeInjectBatchContext(context, batchContext);

        Assert.Equal(2m, context.InRequestOccupiedQtyByLimitDimension["TIME_WINDOW:P001:ITEM001"]);
        Assert.Equal(1m, context.InRequestOccupiedQtyByLimitDimension["MUTEX:GROUP_A"]);
        Assert.Equal(30m, context.InRequestOccupiedQtyByLimitDimension["OP_CEILING:OP001:GROUP_A"]);
        Assert.Single(context.InRequestLimitOccupies);
    }

    private static void InvokeInjectBatchContext(
        PricingContext context,
        BatchPricingContext batchContext)
    {
        var method = typeof(PricingEngine).GetMethod(
            "InjectBatchContext",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);
        method!.Invoke(null, new object[] { context, batchContext });
    }
}
