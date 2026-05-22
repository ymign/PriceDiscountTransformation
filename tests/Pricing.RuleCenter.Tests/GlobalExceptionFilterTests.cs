using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Filters;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class GlobalExceptionFilterTests
{
    [Fact]
    public void OnException_ShouldMapBizExceptionToConfiguredBusinessCodeAndHttpStatus()
    {
        var filter = new GlobalExceptionFilter(NullLogger<GlobalExceptionFilter>.Instance);
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        var context = new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = new BizException(BizErrorCode.RuleVersionConcurrencyConflict, 409, "版本并发冲突")
        };

        filter.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(409, result.StatusCode);
        var response = Assert.IsType<ApiResponse>(result.Value);
        Assert.Equal(BizErrorCode.RuleVersionConcurrencyConflict, response.Code);
        Assert.Equal("版本并发冲突", response.Message);
        Assert.True(context.ExceptionHandled);
    }
}
