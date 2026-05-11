using Pricing.RuleCenter.Api.Dto;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PricingCalculateRequestTests
{
    [Fact]
    public void CalculateRequest_UsesItemsForMultipleChargeDetails()
    {
        var request = new PricingCalculateRequest
        {
            RequestNo = "REQ-001",
            BusinessRequestNo = "BIZ-001",
            SourceSystem = "HIS",
            PatientId = "P001",
            VisitType = "OUTPATIENT",
            PatientAge = 7,
            ChargeNo = "C001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new List<PricingCalculateItemRequest>
            {
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = "CD001",
                    ItemCode = "CT001",
                    ItemName = "CT平扫",
                    ItemGroupCode = "CT_GROUP",
                    InputQty = 2m,
                    Unit = "PART",
                    UnitPrice = 300m
                },
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = "CD002",
                    ItemCode = "SKIN001",
                    ItemName = "皮肤治疗",
                    InputQty = 18m,
                    Unit = "CM2",
                    UnitPrice = 200m
                }
            }
        };

        Assert.Equal(2, request.Items.Count);
        Assert.Equal("CT001", request.Items[0].ItemCode);
        Assert.Equal("CT_GROUP", request.Items[0].ItemGroupCode);
        Assert.Equal("OUTPATIENT", request.VisitType);
        Assert.Equal(7, request.PatientAge);
        Assert.Equal("SKIN001", request.Items[1].ItemCode);
    }
}
