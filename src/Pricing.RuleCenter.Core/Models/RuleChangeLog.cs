using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_CHANGE_LOG")]
public sealed class RuleChangeLog
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "CHANGE_ID")]
    public long ChangeId { get; set; }

    [SugarColumn(ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "VERSION_NO", IsNullable = true)]
    public int? VersionNo { get; set; }

    [SugarColumn(ColumnName = "CHANGE_TYPE")]
    public string ChangeType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "CHANGE_SUMMARY", IsNullable = true)]
    public string? ChangeSummary { get; set; }

    [SugarColumn(ColumnName = "BEFORE_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    public string? BeforeSnapshot { get; set; }

    [SugarColumn(ColumnName = "AFTER_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    public string? AfterSnapshot { get; set; }

    [SugarColumn(ColumnName = "CHANGED_BY", IsNullable = true)]
    public string? ChangedBy { get; set; }

    [SugarColumn(ColumnName = "CHANGED_AT")]
    public DateTime ChangedAt { get; set; }

    [SugarColumn(ColumnName = "SOURCE_SYSTEM", IsNullable = true)]
    public string? SourceSystem { get; set; }
}
