namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 带业务数据的统一接口响应包装。
/// </summary>
/// <typeparam name="T">响应业务数据的类型，由各接口自行定义。</typeparam>
/// <remarks>
/// <para>
/// 物价规则中心所有 HTTP 接口均返回此统一结构，确保前端、HIS、自助机、微信等调用方
/// 以一致的方式解析响应。成功时 <see cref="Code"/> 固定为 0，失败时根据异常类型映射
/// 为不同编码：业务校验失败返回 -1，参数错误返回 400，资源不存在返回 409，系统异常返回 500。
/// </para>
/// <para>
/// <see cref="TraceId"/> 是贯穿一次计价请求全链路的追踪号，由计价引擎在 confirm/simulate
/// 时生成，落库到 PR_CHARGE_REQUEST_LOG，并透传到响应中，便于运维跨表定位问题。
/// </para>
/// </remarks>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// 统一响应码。0 表示业务处理成功；非 0 表示业务或系统错误。
    /// 常见值：0（成功）、-1（通用业务失败）、400（参数校验失败）、409（资源冲突/不存在）、500（系统异常）。
    /// </summary>
    public int Code { get; init; }

    /// <summary>
    /// 统一响应消息。成功时默认为 "success"，失败时为面向开发或运维的可读错误描述。
    /// 全局异常过滤器会将不同层级的异常转换为对应的中文消息。
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// 响应业务数据。成功时由各接口填充具体的 DTO；失败时为 null。
    /// 泛型参数 T 由各接口端点自行定义，如 <see cref="RuleHeaderResponse"/>、<see cref="PagedResponse{T}"/> 等。
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// 计价追踪号，贯穿一次计价请求的全链路（请求日志 → 追踪步骤 → 折价明细 → 限额占用）。
    /// 由计价引擎在 confirm/simulate 时生成，落库到 PR_CHARGE_REQUEST_LOG.REQUEST_NO，
    /// 透传到响应中供调用方在追溯查询接口 <c>/api/pricing/trace/query</c> 中使用。
    /// 非计价类接口（如规则 CRUD）此字段为 null。
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// 创建成功响应（带业务数据）。
    /// </summary>
    /// <param name="data">业务数据，直接赋值到 <see cref="Data"/> 属性。</param>
    /// <param name="message">成功消息，默认 "success"。特殊场景可覆盖为业务提示。</param>
    /// <returns>Code 为 0、Data 有值的统一响应。</returns>
    public static ApiResponse<T> Ok(T data, string message = "success")
    {
        return new ApiResponse<T> { Code = 0, Message = message, Data = data };
    }

    /// <summary>
    /// 创建失败响应。
    /// </summary>
    /// <param name="message">面向开发或运维的可读错误描述。</param>
    /// <param name="code">
    /// 失败编码，默认 -1（通用业务失败）。
    /// 全局异常过滤器会根据异常类型传入 400（参数校验）、404（路由未匹配）、409（资源冲突/不存在）或 500（系统异常）。
    /// 推荐使用 <see cref="BizErrorCode"/> 中定义的业务错误码常量。
    /// </param>
    /// <returns>Code 非 0、Data 为 null 的统一失败响应。</returns>
    public static ApiResponse<T> Fail(string message, int code = -1)
    {
        return new ApiResponse<T> { Code = code, Message = message };
    }

    /// <summary>
    /// 创建带业务错误码的失败响应。
    /// </summary>
    /// <param name="code">
    /// 业务错误码，应使用 <see cref="BizErrorCode"/> 中定义的常量。
    /// </param>
    /// <param name="message">面向开发或运维的可读错误描述。</param>
    /// <returns>Code 非 0、Data 为 null 的统一失败响应。</returns>
    public static ApiResponse<T> Fail(int code, string message)
    {
        return new ApiResponse<T> { Code = code, Message = message };
    }
}

/// <summary>
/// 不带业务数据的统一接口响应包装。
/// </summary>
/// <remarks>
/// 用于不需要返回业务数据的操作类接口，如规则发布（POST）、停用、回滚、取消确认等。
/// 调用方只需关注 <see cref="Code"/> 是否为 0 即可判断操作是否成功。
/// </remarks>
public sealed class ApiResponse
{
    /// <summary>
    /// 统一响应码。0 表示业务处理成功；非 0 表示业务或系统错误。
    /// 常见值：0（成功）、-1（通用业务失败）、400（参数校验失败）、409（资源冲突/不存在）、500（系统异常）。
    /// </summary>
    public int Code { get; init; }

    /// <summary>
    /// 统一响应消息。成功时默认为 "success"，失败时为面向开发或运维的可读错误描述。
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// 计价追踪号，贯穿一次计价请求的全链路。
    /// 仅计价类接口（confirm/simulate/cancel/reverse）会填充此字段，操作类接口为 null。
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// 创建成功响应（无业务数据）。
    /// </summary>
    /// <param name="message">成功消息，默认 "success"。</param>
    /// <returns>Code 为 0 的统一响应。</returns>
    public static ApiResponse Ok(string message = "success")
    {
        return new ApiResponse { Code = 0, Message = message };
    }

    /// <summary>
    /// 创建失败响应。
    /// </summary>
    /// <param name="message">面向开发或运维的可读错误描述。</param>
    /// <param name="code">
    /// 失败编码，默认 -1（通用业务失败）。
    /// 全局异常过滤器会根据异常类型传入 400、404、409 或 500。
    /// 推荐使用 <see cref="BizErrorCode"/> 中定义的业务错误码常量。
    /// </param>
    /// <returns>Code 非 0 的统一失败响应。</returns>
    public static ApiResponse Fail(string message, int code = -1)
    {
        return new ApiResponse { Code = code, Message = message };
    }

    /// <summary>
    /// 创建带业务错误码的失败响应。
    /// </summary>
    /// <param name="code">
    /// 业务错误码，应使用 <see cref="BizErrorCode"/> 中定义的常量。
    /// </param>
    /// <param name="message">面向开发或运维的可读错误描述。</param>
    /// <returns>Code 非 0 的统一失败响应。</returns>
    public static ApiResponse Fail(int code, string message)
    {
        return new ApiResponse { Code = code, Message = message };
    }
}

