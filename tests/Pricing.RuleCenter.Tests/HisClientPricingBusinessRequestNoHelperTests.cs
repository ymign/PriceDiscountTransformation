using HIS.Pricing.Client;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class HisClientPricingBusinessRequestNoHelperTests
{
    [Fact]
    public void EnsureBusinessRequestNo_ReturnsExistingBusinessRequestNo()
    {
        var value = PricingBusinessRequestNoHelper.EnsureBusinessRequestNo("BR001", "C001");

        Assert.Equal("BR001", value);
    }

    [Fact]
    public void EnsureBusinessRequestNo_UsesStableChargeNoWhenBusinessRequestNoIsMissing()
    {
        var value = PricingBusinessRequestNoHelper.EnsureBusinessRequestNo(null!, "C001");

        Assert.Equal("HIS_CHARGE_C001", value);
    }

    [Fact]
    public void EnsureBusinessRequestNo_ReturnsEmptyWhenNoStableKeyExists()
    {
        var value = PricingBusinessRequestNoHelper.EnsureBusinessRequestNo(null!, null!);

        Assert.Equal(string.Empty, value);
    }
}
