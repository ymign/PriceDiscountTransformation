using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Controllers;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PolicyControllerRouteTests
{
    [Fact]
    public void PreviewAsync_ExposesPreviewRoute()
    {
        var method = typeof(PolicyController).GetMethod(nameof(PolicyController.PreviewAsync));

        Assert.NotNull(method);
        var route = Assert.Single(method!.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>());
        Assert.Equal("versions/{policyVersionId:long}/preview", route.Template);
    }

    [Fact]
    public void SubmitReviewAsync_ExposesReviewSubmitRoute()
    {
        var method = typeof(PolicyController).GetMethod(nameof(PolicyController.SubmitReviewAsync));

        Assert.NotNull(method);
        var route = Assert.Single(method!.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>());
        Assert.Equal("versions/{policyVersionId:long}/review/submit", route.Template);
    }

    [Fact]
    public void ImportLegacyRulesAsync_ExposesImportRoute()
    {
        var method = typeof(PolicyController).GetMethod(nameof(PolicyController.ImportLegacyRulesAsync));

        Assert.NotNull(method);
        var route = Assert.Single(method!.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>());
        Assert.Equal("import", route.Template);
    }
}
