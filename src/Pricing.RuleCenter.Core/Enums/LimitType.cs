namespace Pricing.RuleCenter.Core.Enums;

/// <summary>
/// 额度限制类型枚举，定义计价引擎中可用的各类数量/金额/维度限制。
///
/// 每种限制类型对应 PR_LIMIT_OCCUPY 表中的一种占用模式，
/// 计价引擎在规则动作（PR_RULE_ACTION）执行阶段按此类型查询已占用量并校验是否超限。
///
/// 核心业务约束：
/// - NULL 表示"不校验"，0 表示"限制为零"（即不允许收费）。
/// - 公式优先于限制：先算公式结果，再与金额/数量限制比较。
/// - 单次限额维度使用 sourceSystem + businessRequestNo + itemCode 或同等收费动作键。
/// - 时间窗限制按业务收费发生时间（非技术占用时间）向前滑动查询。
/// </summary>
public enum LimitType
{
    /// <summary>
    /// 日数量限制：按自然日累计同一项目的收费数量。
    /// 全院累计，按收费途径统计（门诊收费、手术/医技划价、终端确认、护士过医嘱/分解医嘱）。
    /// 已退费记录不计入累计。
    /// </summary>
    DayQty,

    /// <summary>
    /// 日金额限制：按自然日累计同一项目的收费金额（decimal，NUMBER(18,4)）。
    /// 同样按全院、按收费途径累计，已退费金额不计入。
    /// </summary>
    DayAmount,

    /// <summary>
    /// 单次数量限制：单次收费动作中允许的最大数量。
    /// "单次"指单次收费动作（门诊收费保存、手术/医技划价、终端确认、护士过医嘱/分解医嘱）。
    /// 维度键：sourceSystem + businessRequestNo + itemCode。
    /// </summary>
    OnceQty,

    /// <summary>
    /// 时间窗数量限制：在滑动时间窗口内（如2小时）累计同一项目的收费数量。
    /// 按业务收费发生时间向前查询，非技术占用时间。
    /// 并发锁定时必须覆盖窗口内全部小时桶（SELECT ... FOR UPDATE）。
    /// </summary>
    TimeWindow,

    /// <summary>
    /// 同手术限制：同一手术中某些项目的累计数量/金额限制。
    /// 来源待确认："同手术"标识在现有系统中的获取方式。
    /// </summary>
    SameOperation,

    /// <summary>
    /// 同孕次限制：同一孕次中某些项目的累计数量/金额限制。
    /// 来源待确认："同孕次"标识在现有系统中的获取方式。
    /// </summary>
    SamePregnancy,

    /// <summary>
    /// 同组互斥限制：同一互斥组内的项目共享数量/金额配额。
    /// 由 PR_ITEM_GROUP + PR_ITEM_GROUP_DETAIL 定义分组关系。
    /// 超出部分按0元处理（不是整单归零，不是拒单）。
    /// </summary>
    SameGroup
}
