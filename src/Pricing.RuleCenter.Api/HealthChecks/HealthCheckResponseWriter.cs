using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Api.HealthChecks;

/// <summary>
/// HealthChecks 中间件统一响应写入器。
/// </summary>
public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    /// <summary>
    /// 将 <see cref="HealthReport"/> 写成统一 <see cref="ApiResult{T}"/> JSON 响应。
    /// </summary>
    /// <param name="context">HTTP 上下文。</param>
    /// <param name="report">健康检查报告。</param>
    /// <returns>异步写入任务。</returns>
    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var payload = new HealthCheckSummary
        {
            Status = report.Status.ToString(),
            TotalDurationMs = report.TotalDuration.TotalMilliseconds,
            Checks = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new HealthCheckEntrySummary
                {
                    Status = entry.Value.Status.ToString(),
                    Description = entry.Value.Description,
                    DurationMs = entry.Value.Duration.TotalMilliseconds,
                    Data = entry.Value.Data
                })
        };

        var result = report.Status == HealthStatus.Healthy
            ? ApiResult<HealthCheckSummary>.Ok(payload, "healthy", context.TraceIdentifier)
            : ApiResult<HealthCheckSummary>.Fail(503, "unhealthy", context.TraceIdentifier);

        if (result.Code != 0)
        {
            result = new ApiResult<HealthCheckSummary>
            {
                Code = result.Code,
                Message = result.Message,
                TraceId = result.TraceId,
                Data = payload
            };
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(result, s_jsonOptions));
    }
}

/// <summary>
/// 健康检查汇总响应 DTO。
/// </summary>
public sealed class HealthCheckSummary
{
    /// <summary>
    /// 综合健康状态。
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// 全部检查耗时，单位毫秒。
    /// </summary>
    public double TotalDurationMs { get; init; }

    /// <summary>
    /// 各检查项结果。
    /// </summary>
    public IReadOnlyDictionary<string, HealthCheckEntrySummary> Checks { get; init; } =
        new Dictionary<string, HealthCheckEntrySummary>();
}

/// <summary>
/// 单个健康检查项响应 DTO。
/// </summary>
public sealed class HealthCheckEntrySummary
{
    /// <summary>
    /// 检查项状态。
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// 检查项描述。
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 检查项耗时，单位毫秒。
    /// </summary>
    public double DurationMs { get; init; }

    /// <summary>
    /// 检查项附加数据。
    /// </summary>
    public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();
}
