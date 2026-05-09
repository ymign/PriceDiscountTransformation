using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_CHARGE_DISCOUNT_DETAIL")]
public sealed class ChargeDiscountDetail
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "DISCOUNT_ID")]
    public long DiscountId { get; set; }

    [SugarColumn(ColumnName = "REQUEST_ID")]
    public long RequestId { get; set; }

    [SugarColumn(ColumnName = "TRACE_ID", IsNullable = true)]
    public string? TraceId { get; set; }

    [SugarColumn(ColumnName = "CHARGE_NO", IsNullable = true)]
    public string? ChargeNo { get; set; }

    [SugarColumn(ColumnName = "CHARGE_DETAIL_NO", IsNullable = true)]
    public string? ChargeDetailNo { get; set; }

    [SugarColumn(ColumnName = "PATIENT_ID", IsNullable = true)]
    public string? PatientId { get; set; }

    [SugarColumn(ColumnName = "VISIT_ID", IsNullable = true)]
    public string? VisitId { get; set; }

    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    public string? ItemCode { get; set; }

    [SugarColumn(ColumnName = "ITEM_NAME", IsNullable = true)]
    public string? ItemName { get; set; }

    [SugarColumn(ColumnName = "RULE_ID", IsNullable = true)]
    public long? RuleId { get; set; }

    [SugarColumn(ColumnName = "RULE_VERSION_NO", IsNullable = true)]
    public int? RuleVersionNo { get; set; }

    [SugarColumn(ColumnName = "DISCOUNT_TYPE", IsNullable = true)]
    public string? DiscountType { get; set; }

    [SugarColumn(ColumnName = "STATUS")]
    public string Status { get; set; } = "PENDING";

    [SugarColumn(ColumnName = "RESULT_GROUP_NO", IsNullable = true)]
    public string? ResultGroupNo { get; set; }

    [SugarColumn(ColumnName = "PARENT_DISCOUNT_ID", IsNullable = true)]
    public long? ParentDiscountId { get; set; }

    [SugarColumn(ColumnName = "PART_SEQ", IsNullable = true)]
    public int? PartSeq { get; set; }

    [SugarColumn(ColumnName = "ORIGINAL_QTY", IsNullable = true)]
    public decimal? OriginalQty { get; set; }

    [SugarColumn(ColumnName = "CONVERTED_QTY", IsNullable = true)]
    public decimal? ConvertedQty { get; set; }

    [SugarColumn(ColumnName = "FINAL_QTY", IsNullable = true)]
    public decimal? FinalQty { get; set; }

    [SugarColumn(ColumnName = "UNIT_PRICE", IsNullable = true)]
    public decimal? UnitPrice { get; set; }

    [SugarColumn(ColumnName = "ORIGINAL_AMT", IsNullable = true)]
    public decimal? OriginalAmt { get; set; }

    [SugarColumn(ColumnName = "CALCULATED_AMT", IsNullable = true)]
    public decimal? CalculatedAmt { get; set; }

    [SugarColumn(ColumnName = "FINAL_AMT", IsNullable = true)]
    public decimal? FinalAmt { get; set; }

    [SugarColumn(ColumnName = "DISCOUNT_AMT", IsNullable = true)]
    public decimal? DiscountAmt { get; set; }

    [SugarColumn(ColumnName = "REASON_CODE", IsNullable = true)]
    public string? ReasonCode { get; set; }

    [SugarColumn(ColumnName = "REASON_DESC", IsNullable = true)]
    public string? ReasonDesc { get; set; }

    [SugarColumn(ColumnName = "LIMIT_BASE_INFO", ColumnDataType = "CLOB", IsNullable = true)]
    public string? LimitBaseInfo { get; set; }

    [SugarColumn(ColumnName = "OCCURRED_AT")]
    public DateTime OccurredAt { get; set; }

    [SugarColumn(ColumnName = "CONFIRMED_BY", IsNullable = true)]
    public string? ConfirmedBy { get; set; }

    [SugarColumn(ColumnName = "COMMITTED_AT", IsNullable = true)]
    public DateTime? CommittedAt { get; set; }

    [SugarColumn(ColumnName = "CANCELLED_AT", IsNullable = true)]
    public DateTime? CancelledAt { get; set; }

    [SugarColumn(ColumnName = "EXPIRED_AT", IsNullable = true)]
    public DateTime? ExpiredAt { get; set; }

    [SugarColumn(ColumnName = "REVERSED_AT", IsNullable = true)]
    public DateTime? ReversedAt { get; set; }
}
