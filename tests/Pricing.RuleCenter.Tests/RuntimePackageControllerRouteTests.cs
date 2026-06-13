using Pricing.RuleCenter.Api.Controllers;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuntimePackageControllerRouteTests
{
    [Fact]
    public void RuntimePackageController_ShouldBeRetired()
    {
        var controllerType = typeof(PricingController).Assembly.GetType(
            "Pricing.RuleCenter.Api.Controllers.RuntimePackageController",
            throwOnError: false);

        Assert.Null(controllerType);
    }
}
