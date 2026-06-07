using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Controllers;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class TemplateControllerRouteTests
{
    [Fact]
    public void SaveVersionAsync_ExposesTemplateVersionRoute()
    {
        var method = typeof(TemplateController).GetMethod(nameof(TemplateController.SaveVersionAsync));

        Assert.NotNull(method);
        var route = Assert.Single(method!.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>());
        Assert.Equal("{templateId:long}/versions", route.Template);
    }
}
