namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 应用统一时间源，用于把可测试的技术时间从 <see cref="DateTime.Now"/> 中解耦。
/// </summary>
public interface IClock
{
    /// <summary>
    /// 获取当前本地时间。
    /// </summary>
    DateTime Now { get; }
}
