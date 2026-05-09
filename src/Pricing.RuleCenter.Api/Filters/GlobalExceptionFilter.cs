using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pricing.RuleCenter.Api.Dto;

namespace Pricing.RuleCenter.Api.Filters;

public sealed class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "未处理异常: {Path}", context.HttpContext.Request.Path);

        var response = context.Exception switch
        {
            ArgumentException ex => ApiResponse.Fail(ex.Message, 400),
            KeyNotFoundException ex => ApiResponse.Fail(ex.Message, 404),
            InvalidOperationException ex => ApiResponse.Fail(ex.Message, 409),
            _ => ApiResponse.Fail("服务器内部错误", 500)
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = response.Code switch
            {
                400 => 400,
                404 => 404,
                409 => 409,
                _ => 500
            }
        };

        context.ExceptionHandled = true;
    }
}
