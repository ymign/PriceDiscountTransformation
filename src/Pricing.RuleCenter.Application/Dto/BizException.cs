namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 结构化业务异常。
/// </summary>
/// <remarks>
/// 用于把“业务错误码 + HTTP 状态码 + 可读消息”一起从服务层抛出，避免再依赖异常字符串前缀做语义约定。
/// </remarks>
public sealed class BizException : Exception
{
    public BizException(int code, int httpStatusCode, string message)
        : base(message)
    {
        Code = code;
        HttpStatusCode = httpStatusCode;
    }

    public int Code { get; }

    public int HttpStatusCode { get; }
}

