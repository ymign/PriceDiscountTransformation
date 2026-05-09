namespace Pricing.RuleCenter.Core.Enums;

/// <summary>
/// LimitType 枚举定义规则中心中受控状态或类型值，避免在业务代码中散落魔法字符串。
/// </summary>
public enum LimitType
{
    DayQty,
    DayAmount,
    OnceQty,
    TimeWindow,
    SameOperation,
    SamePregnancy,
    SameGroup
}
