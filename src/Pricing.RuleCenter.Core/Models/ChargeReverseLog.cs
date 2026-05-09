using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_CHARGE_REVERSE_LOG")]
/// <summary>
/// 计价冲正日志实体，对应 PR_CHARGE_REVERSE_LOG。
/// </summary>
/// <remarks>
/// 冲正日志保存对已提交计价结果的撤销动作，便于和 HIS 退费单、原计价请求及释放占用进行审计关联。
/// </remarks>
public sealed class ChargeReverseLog
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "REVERSE_ID")]
    /// <summary>
    /// 冲正日志主键。
    /// </summary>
    public long ReverseId { get; set; }

    [SugarColumn(ColumnName = "ORIGINAL_REQUEST_ID")]
    /// <summary>
    /// 被冲正的原始计价请求主键。
    /// </summary>
    public long OriginalRequestId { get; set; }

    [SugarColumn(ColumnName = "REVERSE_REQUEST_ID", IsNullable = true)]
    /// <summary>
    /// 本次冲正形成的新请求日志主键。
    /// </summary>
    public long? ReverseRequestId { get; set; }

    [SugarColumn(ColumnName = "CHARGE_NO", IsNullable = true)]
    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    public string? ChargeNo { get; set; }

    [SugarColumn(ColumnName = "REVERSE_NO", IsNullable = true)]
    /// <summary>
    /// 调用方冲正流水号。
    /// </summary>
    public string? ReverseNo { get; set; }

    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; set; }

    [SugarColumn(ColumnName = "REVERSE_QTY", IsNullable = true)]
    /// <summary>
    /// 本次冲正数量。
    /// </summary>
    public decimal? ReverseQty { get; set; }

    [SugarColumn(ColumnName = "REVERSE_AMT", IsNullable = true)]
    /// <summary>
    /// 本次冲正金额。
    /// </summary>
    public decimal? ReverseAmt { get; set; }

    [SugarColumn(ColumnName = "REVERSE_REASON", IsNullable = true)]
    /// <summary>
    /// 冲正原因说明。
    /// </summary>
    public string? ReverseReason { get; set; }

    [SugarColumn(ColumnName = "REVERSED_BY", IsNullable = true)]
    /// <summary>
    /// 冲正操作人或来源系统账号。
    /// </summary>
    public string? ReversedBy { get; set; }

    [SugarColumn(ColumnName = "REVERSED_AT")]
    /// <summary>
    /// 冲正发生时间。
    /// </summary>
    public DateTime ReversedAt { get; set; }
}
