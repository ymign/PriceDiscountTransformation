namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 计价冲正日志实体，对应 PR_CHARGE_REVERSE_LOG。
/// </summary>
/// <remarks>
/// 冲正日志保存对已提交计价结果的撤销动作，便于和 HIS 退费单、原计价请求及释放占用进行审计关联。
/// </remarks>
public sealed class ChargeReverseLog
{
    /// <summary>
    /// 冲正日志主键。
    /// </summary>
    public long ReverseId { get; set; }

    /// <summary>
    /// 被冲正的原始计价请求主键。
    /// </summary>
    public long OriginalRequestId { get; set; }

    /// <summary>
    /// 本次冲正形成的新请求日志主键。
    /// </summary>
    public long? ReverseRequestId { get; set; }

    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    public string? ChargeNo { get; set; }

    /// <summary>
    /// 调用方冲正流水号。
    /// </summary>
    public string? ReverseNo { get; set; }

    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; set; }

    /// <summary>
    /// 被退费或冲正的原收费明细号。
    /// </summary>
    public string? ChargeDetailNo { get; set; }

    /// <summary>
    /// 多部位或多片段退费时的片段序号。
    /// </summary>
    public int? PartSeq { get; set; }

    /// <summary>
    /// 本次冲正数量。
    /// </summary>
    public decimal? ReverseQty { get; set; }

    /// <summary>
    /// 本次冲正金额。
    /// </summary>
    public decimal? ReverseAmt { get; set; }

    /// <summary>
    /// 冲正原因说明。
    /// </summary>
    public string? ReverseReason { get; set; }

    /// <summary>
    /// 冲正操作人或来源系统账号。
    /// </summary>
    public string? ReversedBy { get; set; }

    /// <summary>
    /// 冲正发生时间。
    /// </summary>
    public DateTime ReversedAt { get; set; }
}
