using Pricing.RuleCenter.Core.Services;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PricingAmountRounderTests
{
    [Theory]
    [InlineData(10.005, 10.01)]
    [InlineData(10.004, 10.00)]
    [InlineData(0.005, 0.01)]
    public void RoundFinalAmount_UsesTwoDecimalAwayFromZero(decimal input, decimal expected)
    {
        Assert.Equal(expected, PricingAmountRounder.RoundFinal(input));
    }
}
