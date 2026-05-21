using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class ChargeRequestStatusTests
{
    [Fact]
    public void MarkCommitted_UsesConfirmedStateAsCanonicalCommittedResult()
    {
        var request = new ChargeRequest
        {
            RequestNo = "REQ-001",
            BusinessStatus = BusinessStatusCodes.ConfirmPending
        };

        request.MarkCommitted();

        Assert.Equal(BusinessStatusCodes.Confirmed, request.BusinessStatus);
    }

    [Fact]
    public void MarkReversed_AllowsConfirmedState()
    {
        var request = new ChargeRequest
        {
            RequestNo = "REQ-002",
            BusinessStatus = BusinessStatusCodes.Confirmed
        };

        request.MarkReversed();

        Assert.Equal(BusinessStatusCodes.Reversed, request.BusinessStatus);
    }
}
