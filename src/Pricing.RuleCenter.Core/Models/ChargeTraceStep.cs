using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_CHARGE_TRACE_STEP")]
public sealed class ChargeTraceStep
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "STEP_ID")]
    public long StepId { get; set; }

    [SugarColumn(ColumnName = "REQUEST_ID")]
    public long RequestId { get; set; }

    [SugarColumn(ColumnName = "TRACE_ID", IsNullable = true)]
    public string? TraceId { get; set; }

    [SugarColumn(ColumnName = "STEP_NO")]
    public int StepNo { get; set; }

    [SugarColumn(ColumnName = "STEP_NAME")]
    public string StepName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "STEP_TYPE")]
    public string StepType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "RULE_ID", IsNullable = true)]
    public long? RuleId { get; set; }

    [SugarColumn(ColumnName = "RULE_VERSION_NO", IsNullable = true)]
    public int? RuleVersionNo { get; set; }

    [SugarColumn(ColumnName = "INPUT_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    public string? InputSnapshot { get; set; }

    [SugarColumn(ColumnName = "OUTPUT_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    public string? OutputSnapshot { get; set; }

    [SugarColumn(ColumnName = "STEP_DESC", IsNullable = true)]
    public string? StepDesc { get; set; }

    [SugarColumn(ColumnName = "CREATED_AT")]
    public DateTime CreatedAt { get; set; }
}
