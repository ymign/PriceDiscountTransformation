using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Controllers;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuntimePackageControllerRouteTests
{
    [Fact]
    public void PublishAsync_ExposesPublishRoute()
    {
        var method = typeof(RuntimePackageController).GetMethod(nameof(RuntimePackageController.PublishAsync));

        Assert.NotNull(method);
        var route = Assert.Single(method!.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>());
        Assert.Equal("publish", route.Template);
    }

    [Fact]
    public void RollbackAsync_ExposesRollbackRoute()
    {
        var method = typeof(RuntimePackageController).GetMethod(nameof(RuntimePackageController.RollbackAsync));

        Assert.NotNull(method);
        var route = Assert.Single(method!.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>());
        Assert.Equal("{packageId:long}/rollback", route.Template);
    }
}
