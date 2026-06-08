using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 结构化业务异常。
/// </summary>
/// <remarks>
/// 用于把“业务错误码 + HTTP 状态码 + 可读消息”一起从服务层抛出，避免再依赖异常字符串前缀做语义约定。
/// </remarks>
public sealed class BizException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BizException"/> class.
    /// </summary>
    /// <param name="code">业务错误码。</param>
    /// <param name="httpStatusCode">建议映射的 HTTP 状态码。</param>
    /// <param name="message">面向调用方的错误消息。</param>
    public BizException(int code, int httpStatusCode, string message)
        : base(message)
    {
        Code = code;
        HttpStatusCode = httpStatusCode;
    }

    /// <summary>
    /// 获取业务错误码。
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; }

    /// <summary>
    /// 获取建议映射的 HTTP 状态码。
    /// </summary>
    [JsonPropertyName("http_status_code")]
    public int HttpStatusCode { get; }
}

