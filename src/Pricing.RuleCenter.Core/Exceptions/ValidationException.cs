namespace Pricing.RuleCenter.Core.Exceptions;

/// <summary>
/// 参数或业务校验异常，支持字段级错误详情。
/// </summary>
public sealed class ValidationException : DomainException
{
    /// <summary>初始化校验异常。</summary>
    public ValidationException(string message, IReadOnlyDictionary<string, string[]>? errors = null, int code = 400)
        : base(message, code)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    /// <summary>字段级错误详情。</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
