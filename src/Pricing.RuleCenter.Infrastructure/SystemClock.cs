using Pricing.RuleCenter.Core.Interfaces;

namespace Pricing.RuleCenter.Infrastructure;

/// <summary>
/// 默认系统技术时间提供者。
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTime Now => DateTime.Now;
}
