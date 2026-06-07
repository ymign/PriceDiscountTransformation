using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class ChargeRequestStatusTests
{
    [Fact]
    public void MarkCommitted_UsesConfirmedStateAsCanonicalCommittedResult()
    {
        var now = new DateTime(2026, 5, 10, 10, 0, 0);
        var request = new ChargeRequest
        {
            RequestNo = "REQ-001",
            BusinessStatus = BusinessStatusCodes.ConfirmPending
        };

        request.MarkCommitted(now);

        Assert.Equal(BusinessStatusCodes.Confirmed, request.BusinessStatus);
        Assert.Equal(now, request.ResponseAt);
    }

    [Fact]
    public void MarkReversed_AllowsConfirmedState()
    {
        var now = new DateTime(2026, 5, 10, 10, 1, 0);
        var request = new ChargeRequest
        {
            RequestNo = "REQ-002",
            BusinessStatus = BusinessStatusCodes.Confirmed
        };

        request.MarkReversed(now);

        Assert.Equal(BusinessStatusCodes.Reversed, request.BusinessStatus);
        Assert.Equal(now, request.ResponseAt);
    }
}
