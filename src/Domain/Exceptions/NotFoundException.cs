namespace Pricing.RuleCenter.Core.Exceptions;

/// <summary>
/// 资源不存在异常。
/// </summary>
public sealed class NotFoundException : DomainException
{
    /// <summary>初始化资源不存在异常。</summary>
    public NotFoundException(string message, int code = 404)
        : base(message, code)
    {
    }
}
