namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 系统技术时间提供者。
/// </summary>
public interface IClock
{
    /// <summary>
    /// 获取当前本地技术时间。
    /// </summary>
    DateTime Now { get; }
}
