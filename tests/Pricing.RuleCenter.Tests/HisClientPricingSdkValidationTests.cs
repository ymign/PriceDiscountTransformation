using HIS.Pricing.Client;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class HisClientPricingSdkValidationTests
{
    [Fact]
    public void PrepareCalculateRequest_RejectsEmptyItemCodeBeforeSpecialFlag()
    {
        var sdk = CreateSdk();
        var request = CreateValidRequest();
        request.Items[0].ItemCode = string.Empty;

        var ex = Assert.Throws<InvalidOperationException>(() => sdk.PrepareCalculateRequest(request));

        Assert.Contains("ItemCode", ex.Message);
    }

    [Fact]
    public void CheckSpecialPricingRequired_BlocksEmptyItemCode()
    {
        var sdk = CreateSdk();

        var decision = sdk.CheckSpecialPricingRequired(" ");

        Assert.True(decision.ServiceUnavailable);
        Assert.False(decision.AllowOrdinaryPricing);
        Assert.Contains("项目编码", decision.Message);
    }

    [Fact]
    public void ValidateConfirmRequest_RejectsMissingStableBusinessRequestNo()
    {
        var sdk = CreateSdk();
        var request = CreateValidRequest();
        request.BusinessRequestNo = null;
        request.ChargeNo = null;

        var ex = Assert.Throws<InvalidOperationException>(() => sdk.ValidateConfirmRequest(request));

        Assert.Contains("BusinessRequestNo", ex.Message);
    }

    [Fact]
    public void ValidateConfirmRequest_UsesChargeNoAsStableBusinessRequestNo()
    {
        var sdk = CreateSdk();
        var request = CreateValidRequest();
        request.BusinessRequestNo = null;
        request.ChargeNo = "C001";

        sdk.ValidateConfirmRequest(request);

        Assert.Equal("HIS_CHARGE_C001", request.BusinessRequestNo);
    }

    [Fact]
    public void ValidateCommitActuals_RejectsInvalidActualItems()
    {
        var actualItems = new List<PricingCommitActualItemRequest>
        {
            new PricingCommitActualItemRequest
            {
                ChargeDetailNo = "CD001",
                ItemCode = string.Empty,
                FinalQty = 1m,
                FinalAmount = 10m
            }
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            PricingCalculateRequestValidator.ValidateCommitActuals(1, actualItems));

        Assert.Contains("ItemCode", ex.Message);
    }

    [Fact]
    public void ValidateCancelRequest_RejectsInvalidRequestId()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            PricingCalculateRequestValidator.ValidateCancelRequest(0));

        Assert.Contains("RequestId", ex.Message);
    }

    private static PricingSdk CreateSdk()
    {
        return new PricingSdk(new PricingApiClient("http://pricing-rule-center"));
    }

    private static PricingCalculateRequest CreateValidRequest()
    {
        return new PricingCalculateRequest
        {
            SourceSystem = "HIS",
            PatientId = "P001",
            BusinessRequestNo = "BR001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new List<PricingCalculateItemRequest>
            {
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = "CD001",
                    ItemCode = "ITEM001",
                    ItemName = "测试项目",
                    InputQty = 1m,
                    UnitPrice = 10m
                }
            }
        };
    }
}
