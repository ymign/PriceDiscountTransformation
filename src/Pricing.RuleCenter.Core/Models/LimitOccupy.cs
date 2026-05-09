using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_LIMIT_OCCUPY")]
public sealed class LimitOccupy
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "OCCUPY_ID")]
    public long OccupyId { get; set; }

    [SugarColumn(ColumnName = "REQUEST_ID")]
    public long RequestId { get; set; }

    [SugarColumn(ColumnName = "TRACE_ID", IsNullable = true)]
    public string? TraceId { get; set; }

    [SugarColumn(ColumnName = "PATIENT_ID")]
    public string PatientId { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    public string? ItemCode { get; set; }

    [SugarColumn(ColumnName = "RULE_ID", IsNullable = true)]
    public long? RuleId { get; set; }

    [SugarColumn(ColumnName = "RULE_VERSION_NO", IsNullable = true)]
    public int? RuleVersionNo { get; set; }

    [SugarColumn(ColumnName = "LIMIT_TYPE")]
    public string LimitType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "LIMIT_KEY")]
    public string LimitKey { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "OCCUPY_QTY")]
    public decimal OccupyQty { get; set; }

    [SugarColumn(ColumnName = "OCCUPY_AMT")]
    public decimal OccupyAmt { get; set; }

    [SugarColumn(ColumnName = "OCCUPY_TYPE")]
    public string OccupyType { get; set; } = "CHARGE";

    [SugarColumn(ColumnName = "ORIGINAL_OCCUPY_ID", IsNullable = true)]
    public long? OriginalOccupyId { get; set; }

    [SugarColumn(ColumnName = "BUSINESS_CHARGE_TIME")]
    public DateTime BusinessChargeTime { get; set; }

    [SugarColumn(ColumnName = "LIMIT_DIMENSION_CODE", IsNullable = true)]
    public string? LimitDimensionCode { get; set; }

    [SugarColumn(ColumnName = "PART_SEQ", IsNullable = true)]
    public int? PartSeq { get; set; }

    [SugarColumn(ColumnName = "STATUS")]
    public string Status { get; set; } = "PENDING";

    [SugarColumn(ColumnName = "OCCUPIED_AT")]
    public DateTime OccupiedAt { get; set; }

    [SugarColumn(ColumnName = "CONFIRMED_AT", IsNullable = true)]
    public DateTime? ConfirmedAt { get; set; }

    [SugarColumn(ColumnName = "EXPIRE_AT", IsNullable = true)]
    public DateTime? ExpireAt { get; set; }
}
