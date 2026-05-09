namespace Pricing.RuleCenter.Core.Enums;

/// <summary>
/// OccupyStatus 枚举定义规则中心中受控状态或类型值，避免在业务代码中散落魔法字符串。
/// </summary>
public enum OccupyStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Reversed,
    Expired
}
