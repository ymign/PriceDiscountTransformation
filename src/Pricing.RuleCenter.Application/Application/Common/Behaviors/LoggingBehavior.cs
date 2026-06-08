using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Pricing.RuleCenter.Application.Common.Behaviors;

/// <summary>
/// MediatR 日志管道，记录请求处理耗时与链路标识。
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>初始化日志管道。</summary>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("开始处理请求 {RequestName}, TraceId={TraceId}", requestName, traceId);
        try
        {
            var response = await next();
            stopwatch.Stop();
            _logger.LogInformation(
                "完成处理请求 {RequestName}, TraceId={TraceId}, ElapsedMs={ElapsedMs}",
                requestName,
                traceId,
                stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "处理请求失败 {RequestName}, TraceId={TraceId}, ElapsedMs={ElapsedMs}",
                requestName,
                traceId,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
