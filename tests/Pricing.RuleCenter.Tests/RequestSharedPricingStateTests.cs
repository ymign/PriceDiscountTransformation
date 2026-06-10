using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RequestSharedPricingStateTests
{
    [Fact]
    public void Accumulate_AddsLimitMutexOperationAndParentAmountState()
    {
        var state = new RequestSharedPricingState();
        var context = new PricingContext
        {
            ItemCode = "ITEM001",
            ItemGroupCode = "GROUP_A",
            ExtraParams = new Dictionary<string, string> { ["operationNo"] = "OP001" }
        };
        var result = new PricingResult
        {
            FinalQty = 2m,
            FinalAmount = 88m,
            LimitOccupies = new[]
            {
                new LimitOccupy
                {
                    LimitType = "DAY_QTY",
                    LimitDimensionCode = "patient:item:20260510",
                    OccupyQty = 2m,
                    OccupyAmt = 88m,
                    BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
                },
                new LimitOccupy { LimitType = "", LimitDimensionCode = "", OccupyQty = 99m, OccupyAmt = 99m },
                new LimitOccupy
                {
                    LimitType = "TIME_WINDOW",
                    LimitDimensionCode = "patient:item:window",
                    OccupyQty = 0m,
                    OccupyAmt = 0m
                }
            }
        };

        state.Accumulate(result, context);

        Assert.Equal(2m, state.AccumulatedValues["DAY_QTY:PATIENT:ITEM:20260510"]);
        Assert.Equal(0m, state.AccumulatedValues["TIME_WINDOW:PATIENT:ITEM:WINDOW"]);
        Assert.Equal(1m, state.AccumulatedValues["MUTEX:GROUP_A"]);
        Assert.Equal(88m, state.AccumulatedValues["OP_CEILING:OP001:GROUP_A"]);
        Assert.Equal(88m, state.AccumulatedValues["ITEM_AMT:ITEM001"]);
        Assert.Equal(2, state.LimitOccupies.Count);
    }

    [Fact]
    public void Accumulate_DoesNotCountZeroQtyItemAsMutexOccupy()
    {
        var state = new RequestSharedPricingState();
        var context = new PricingContext
        {
            ItemCode = "ITEM_A",
            ItemGroupCode = "GROUP_A"
        };
        var result = new PricingResult
        {
            IsSpecialItem = true,
            FinalQty = 0m,
            FinalAmount = 0m
        };

        state.Accumulate(result, context);

        Assert.False(state.AccumulatedValues.ContainsKey("MUTEX:GROUP_A"));
    }
}
