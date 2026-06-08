namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 统一 API 响应的时间源访问器，由组合根在启动时配置。
/// </summary>
public static class ApiResultClock
{
    private static Func<DateTime> s_now = static () => default;

    /// <summary>
    /// 读取当前响应时间。
    /// </summary>
    public static DateTime Current => s_now();

    /// <summary>
    /// 配置统一 API 响应使用的时间源。
    /// </summary>
    public static void Configure(Func<DateTime> nowFactory)
    {
        s_now = nowFactory ?? throw new ArgumentNullException(nameof(nowFactory));
    }
}

/// <summary>
/// 统一 API 响应包装。
/// </summary>
/// <typeparam name="T">响应数据类型。</typeparam>
public sealed class ApiResult<T>
{
    /// <summary>响应码，0 表示成功。</summary>
    public int Code { get; init; }

    /// <summary>响应消息。</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>链路追踪号。</summary>
    public string TraceId { get; init; } = string.Empty;

    /// <summary>响应生成时间。</summary>
    public DateTime Timestamp { get; init; } = ApiResultClock.Current;

    /// <summary>响应数据。</summary>
    public T? Data { get; init; }

    /// <summary>字段级错误详情。</summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    /// <summary>创建成功响应。</summary>
    public static ApiResult<T> Ok(T data, string message = "success", string? traceId = null)
    {
        return new ApiResult<T>
        {
            Code = 0,
            Message = message,
            Data = data,
            TraceId = traceId ?? string.Empty
        };
    }

    /// <summary>创建失败响应。</summary>
    public static ApiResult<T> Fail(int code, string message, string? traceId = null, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ApiResult<T>
        {
            Code = code,
            Message = message,
            TraceId = traceId ?? string.Empty,
            Errors = errors
        };
    }
}

/// <summary>
/// 无数据统一 API 响应包装。
/// </summary>
public sealed class ApiResult
{
    /// <summary>响应码，0 表示成功。</summary>
    public int Code { get; init; }

    /// <summary>响应消息。</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>链路追踪号。</summary>
    public string TraceId { get; init; } = string.Empty;

    /// <summary>响应生成时间。</summary>
    public DateTime Timestamp { get; init; } = ApiResultClock.Current;

    /// <summary>字段级错误详情。</summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    /// <summary>创建成功响应。</summary>
    public static ApiResult Ok(string message = "success", string? traceId = null)
    {
        return new ApiResult { Code = 0, Message = message, TraceId = traceId ?? string.Empty };
    }

    /// <summary>创建失败响应。</summary>
    public static ApiResult Fail(int code, string message, string? traceId = null, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ApiResult { Code = code, Message = message, TraceId = traceId ?? string.Empty, Errors = errors };
    }
}
