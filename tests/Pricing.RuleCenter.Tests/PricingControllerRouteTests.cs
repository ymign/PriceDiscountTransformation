using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Controllers;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PricingControllerRouteTests
{
    [Fact]
    public void BatchSimulateAsync_ExposesDocumentedRoute()
    {
        var method = typeof(PricingController).GetMethod(nameof(PricingController.BatchSimulateAsync));

        Assert.NotNull(method);
        var route = Assert.Single(method!.GetCustomAttributes(typeof(HttpPostAttribute), inherit: false).Cast<HttpPostAttribute>());
        Assert.Equal("calculate/batch-simulate", route.Template);
    }

    [Fact]
    public void BatchSpecialFlagsAsync_ExposesDocumentedRoute()
    {
        var method = typeof(PricingController).GetMethod(nameof(PricingController.BatchSpecialFlagsAsync));

        Assert.NotNull(method);
        var route = Assert.Single(method!.GetCustomAttributes(typeof(HttpPostAttribute), inherit: false).Cast<HttpPostAttribute>());
        Assert.Equal("items/special-flags", route.Template);
    }
}
