using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_CONDITION")]
public sealed class RuleCondition
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "CONDITION_ID")]
    public long ConditionId { get; set; }

    [SugarColumn(ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "VERSION_NO")]
    public int VersionNo { get; set; }

    [SugarColumn(ColumnName = "CONDITION_GROUP")]
    public string ConditionGroup { get; set; } = "DEFAULT";

    [SugarColumn(ColumnName = "CONDITION_TYPE")]
    public string ConditionType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "OPERATOR_TYPE", IsNullable = true)]
    public string? OperatorType { get; set; } = "EQ";

    [SugarColumn(ColumnName = "LEFT_KEY", IsNullable = true)]
    public string? LeftKey { get; set; }

    [SugarColumn(ColumnName = "RIGHT_VALUE", IsNullable = true)]
    public string? RightValue { get; set; }

    [SugarColumn(ColumnName = "PARAMS_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    public string? ParamsJson { get; set; }

    [SugarColumn(ColumnName = "SORT_NO")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";
}
