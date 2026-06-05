namespace Pricing.RuleCenter.Core.Exceptions;

/// <summary>
/// 领域异常基类，用于表达领域规则或不变性约束被违反。
/// </summary>
public class DomainException : Exception
{
    /// <summary>初始化领域异常。</summary>
    public DomainException(string message, int code = -1)
        : base(message)
    {
        Code = code;
    }

    /// <summary>业务错误码。</summary>
    public int Code { get; }
}
