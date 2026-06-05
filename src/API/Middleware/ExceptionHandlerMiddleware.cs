using System.Text.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Exceptions;
using ValidationException = Pricing.RuleCenter.Core.Exceptions.ValidationException;

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
        var (statusCode, code, message, errors) = MapException(exception);

        if (statusCode >= 500)
        {
            _logger.LogError(exception, "未处理异常 TraceId={TraceId}, Path={Path}", traceId, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "业务异常 TraceId={TraceId}, Path={Path}", traceId, context.Request.Path);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var result = ApiResult.Fail(code, message, traceId, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private static (int StatusCode, int Code, string Message, IReadOnlyDictionary<string, string[]>? Errors) MapException(Exception exception)
    {
        return exception switch
        {
            BizException ex => (ex.HttpStatusCode, ex.Code, ex.Message, null),
            ValidationException ex => (400, ex.Code, ex.Message, ex.Errors),
            NotFoundException ex => (404, ex.Code, ex.Message, null),
            DomainException ex => (409, ex.Code, ex.Message, null),
            LimitLockException ex => (ex.IsConcurrencyConflict ? 409 : 500,
                ex.IsConcurrencyConflict ? BizErrorCode.ConcurrencyConflict : BizErrorCode.LimitLockFailed,
                ex.Message,
                null),
            ArgumentException ex => (400, 400, ex.Message, null),
            KeyNotFoundException ex => (404, 404, ex.Message, null),
            InvalidOperationException ex => (409, 409, ex.Message, null),
            _ => (500, 500, "服务器内部错误", null)
        };
    }
}
