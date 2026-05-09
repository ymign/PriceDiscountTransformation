namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 计价引擎输出结果。
/// </summary>
/// <remarks>
/// 引擎只负责计算并返回该对象，不直接决定是否写请求日志、折扣明细或限额占用。应用服务会根据调用类型
/// 决定是仅返回试算结果，还是把结果持久化并进入 confirm/commit 状态机。
/// </remarks>
public sealed class PricingResult
{
    /// <summary>
    /// 是否命中特殊计价规则。true 表示该项目不是普通按单价乘数量直接收费。
    /// </summary>
    public bool IsSpecialItem { get; set; }
    /// <summary>
    /// 调用方录入的原始数量，金额计算前不得随意覆盖
    /// </summary>
    public decimal InputQty { get; set; }
    /// <summary>
    /// 最终可收费数量
    /// </summary>
    public decimal FinalQty { get; set; }
    /// <summary>
    /// 项目单价，confirm 时应与权威物价主数据强校验
    /// </summary>
    public decimal UnitPrice { get; set; }
    /// <summary>
    /// 最终应返回给 HIS 的可收费金额。
    /// </summary>
    public decimal FinalAmount { get; set; }
    /// <summary>
    /// 折价金额，通常等于原始金额减最终金额。
    /// </summary>
    public decimal DiscountAmount { get; set; }
    /// <summary>
    /// 本次计价的追踪步骤，用于解释命中规则和动作执行过程。
    /// </summary>
    public IReadOnlyList<TraceStep> TraceSteps { get; set; } = Array.Empty<TraceStep>();
    /// <summary>
    /// 本次计价命中的规则主键集合。
    /// </summary>
    public IReadOnlyList<long> MatchedRuleIds { get; set; } = Array.Empty<long>();
    /// <summary>
    /// 本次计价生成的限额占用草稿或明细，用于 confirm 阶段写入保护占额。
    /// </summary>
    public IReadOnlyList<LimitOccupy> LimitOccupies { get; set; } = Array.Empty<LimitOccupy>();
}
