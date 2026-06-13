using System.Text.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Api.Serialization;

namespace Pricing.RuleCenter.Api.Middleware;

/// <summary>
/// 全局异常处理中间件，统一输出 API 错误响应。
/// </summary>
public sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    /// <summary>初始化异常处理中间件。</summary>
    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>执行中间件。</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var mapping = ApiExceptionMapper.Map(exception);

        if (mapping.StatusCode >= 500)
        {
            _logger.LogError(exception, "未处理异常 追踪号={TraceId}, 路径={Path}", traceId, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "业务异常 追踪号={TraceId}, 路径={Path}", traceId, context.Request.Path);
        }

        context.Response.StatusCode = mapping.StatusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var result = ApiResult.Fail(mapping.Code, mapping.Message, traceId, mapping.Errors, mapping.ErrorCode);
        await context.Response.WriteAsync(JsonSerializer.Serialize(result, ApiJsonSerializerOptions.Create()));
    }
}
