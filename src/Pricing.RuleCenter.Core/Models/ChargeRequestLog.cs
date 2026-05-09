using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_CHARGE_REQUEST_LOG")]
public sealed class ChargeRequestLog
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "REQUEST_ID")]
    public long RequestId { get; set; }

    [SugarColumn(ColumnName = "REQUEST_NO")]
    public string RequestNo { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "BUSINESS_REQUEST_NO", IsNullable = true)]
    public string? BusinessRequestNo { get; set; }

    [SugarColumn(ColumnName = "REQUEST_FINGERPRINT", IsNullable = true)]
    public string? RequestFingerprint { get; set; }

    [SugarColumn(ColumnName = "TRACE_ID", IsNullable = true)]
    public string? TraceId { get; set; }

    [SugarColumn(ColumnName = "CALL_TYPE")]
    public string CallType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "BUSINESS_STATUS")]
    public string BusinessStatus { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "SOURCE_SYSTEM")]
    public string SourceSystem { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "SOURCE_TERMINAL", IsNullable = true)]
    public string? SourceTerminal { get; set; }

    [SugarColumn(ColumnName = "PATIENT_ID", IsNullable = true)]
    public string? PatientId { get; set; }

    [SugarColumn(ColumnName = "VISIT_ID", IsNullable = true)]
    public string? VisitId { get; set; }

    [SugarColumn(ColumnName = "CHARGE_SCENE", IsNullable = true)]
    public string? ChargeScene { get; set; }

    [SugarColumn(ColumnName = "CHARGE_NO", IsNullable = true)]
    public string? ChargeNo { get; set; }

    [SugarColumn(ColumnName = "CHARGE_DETAIL_NO", IsNullable = true)]
    public string? ChargeDetailNo { get; set; }

    [SugarColumn(ColumnName = "RESULT_GROUP_NO", IsNullable = true)]
    public string? ResultGroupNo { get; set; }

    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    public string? ItemCode { get; set; }

    [SugarColumn(ColumnName = "ITEM_NAME", IsNullable = true)]
    public string? ItemName { get; set; }

    [SugarColumn(ColumnName = "INPUT_QTY", IsNullable = true)]
    public decimal? InputQty { get; set; }

    [SugarColumn(ColumnName = "INPUT_UNIT", IsNullable = true)]
    public string? InputUnit { get; set; }

    [SugarColumn(ColumnName = "BODY_PART_CODE", IsNullable = true)]
    public string? BodyPartCode { get; set; }

    [SugarColumn(ColumnName = "BUSINESS_CHARGE_TIME", IsNullable = true)]
    public DateTime? BusinessChargeTime { get; set; }

    [SugarColumn(ColumnName = "PRICE_VERSION", IsNullable = true)]
    public string? PriceVersion { get; set; }

    [SugarColumn(ColumnName = "REQUEST_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    public string? RequestJson { get; set; }

    [SugarColumn(ColumnName = "RESPONSE_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    public string? ResponseJson { get; set; }

    [SugarColumn(ColumnName = "REQUEST_AT")]
    public DateTime RequestAt { get; set; }

    [SugarColumn(ColumnName = "RESPONSE_AT", IsNullable = true)]
    public DateTime? ResponseAt { get; set; }

    [SugarColumn(ColumnName = "IS_SUCCESS")]
    public string IsSuccess { get; set; } = "N";

    [SugarColumn(ColumnName = "ERROR_MESSAGE", IsNullable = true)]
    public string? ErrorMessage { get; set; }
}
