using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_ACTION")]
public sealed class RuleAction
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "ACTION_ID")]
    public long ActionId { get; set; }

    [SugarColumn(ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "VERSION_NO")]
    public int VersionNo { get; set; }

    [SugarColumn(ColumnName = "ACTION_TYPE")]
    public string ActionType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "EXECUTOR_CODE")]
    public string ExecutorCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "PARAMS_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    public string? ParamsJson { get; set; }

    [SugarColumn(ColumnName = "EXCLUSIVE_GROUP", IsNullable = true)]
    public string? ExclusiveGroup { get; set; }

    [SugarColumn(ColumnName = "SORT_NO")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "ON_ERROR")]
    public string OnError { get; set; } = "STOP";

    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";
}
