using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_CHARGE_REVERSE_LOG")]
public sealed class ChargeReverseLog
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "REVERSE_ID")]
    public long ReverseId { get; set; }

    [SugarColumn(ColumnName = "ORIGINAL_REQUEST_ID")]
    public long OriginalRequestId { get; set; }

    [SugarColumn(ColumnName = "REVERSE_REQUEST_ID", IsNullable = true)]
    public long? ReverseRequestId { get; set; }

    [SugarColumn(ColumnName = "CHARGE_NO", IsNullable = true)]
    public string? ChargeNo { get; set; }

    [SugarColumn(ColumnName = "REVERSE_NO", IsNullable = true)]
    public string? ReverseNo { get; set; }

    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    public string? ItemCode { get; set; }

    [SugarColumn(ColumnName = "REVERSE_QTY", IsNullable = true)]
    public decimal? ReverseQty { get; set; }

    [SugarColumn(ColumnName = "REVERSE_AMT", IsNullable = true)]
    public decimal? ReverseAmt { get; set; }

    [SugarColumn(ColumnName = "REVERSE_REASON", IsNullable = true)]
    public string? ReverseReason { get; set; }

    [SugarColumn(ColumnName = "REVERSED_BY", IsNullable = true)]
    public string? ReversedBy { get; set; }

    [SugarColumn(ColumnName = "REVERSED_AT")]
    public DateTime ReversedAt { get; set; }
}
