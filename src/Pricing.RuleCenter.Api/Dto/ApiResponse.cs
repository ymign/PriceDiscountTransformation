namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 带业务数据的统一接口响应。
/// </summary>
/// <typeparam name="T">响应数据类型。</typeparam>
/// <remarks>
/// 规则中心所有 HTTP 接口都返回统一结构。成功时 Code 为 0，失败时 Code 通常与业务错误或 HTTP 状态码对应。
/// </remarks>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// 统一响应码，0 表示成功，非 0 表示业务或系统错误
    /// </summary>
    public int Code { get; init; }
    /// <summary>
    /// 统一响应消息
    /// </summary>
    public string Message { get; init; } = string.Empty;
    /// <summary>
    /// 统一响应业务数据
    /// </summary>
    public T? Data { get; init; }
    /// <summary>
    /// 计价追踪号，用于跨表查看一次计价过程
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// 创建成功响应。
    /// </summary>
    /// <param name="data">业务数据。</param>
    /// <param name="message">成功消息，默认 success。</param>
    /// <returns>Code 为 0 的统一响应。</returns>
    public static ApiResponse<T> Ok(T data, string message = "success")
    {
        return new ApiResponse<T> { Code = 0, Message = message, Data = data };
    }

    /// <summary>
    /// 创建失败响应。
    /// </summary>
    /// <param name="message">失败消息。</param>
    /// <param name="code">失败编码，默认 -1；全局异常过滤器会传入 400、404、409 或 500。</param>
    /// <returns>统一失败响应。</returns>
    public static ApiResponse<T> Fail(string message, int code = -1)
    {
        return new ApiResponse<T> { Code = code, Message = message };
    }
}

/// <summary>
/// 不带业务数据的统一接口响应。
/// </summary>
public sealed class ApiResponse
{
    /// <summary>
    /// 统一响应码，0 表示成功，非 0 表示业务或系统错误
    /// </summary>
    public int Code { get; init; }
    /// <summary>
    /// 统一响应消息
    /// </summary>
    public string Message { get; init; } = string.Empty;
    /// <summary>
    /// 计价追踪号，用于跨表查看一次计价过程
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// 创建成功响应。
    /// </summary>
    /// <param name="message">成功消息，默认 success。</param>
    /// <returns>Code 为 0 的统一响应。</returns>
    public static ApiResponse Ok(string message = "success")
    {
        return new ApiResponse { Code = 0, Message = message };
    }

    /// <summary>
    /// 创建失败响应。
    /// </summary>
    /// <param name="message">失败消息。</param>
    /// <param name="code">失败编码，默认 -1；全局异常过滤器会传入 400、404、409 或 500。</param>
    /// <returns>统一失败响应。</returns>
    public static ApiResponse Fail(string message, int code = -1)
    {
        return new ApiResponse { Code = code, Message = message };
    }
}
