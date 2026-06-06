using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Infrastructure;

/// <summary>
/// 系统本地时间源的默认实现。
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTime Now => DateTime.Now;
}
