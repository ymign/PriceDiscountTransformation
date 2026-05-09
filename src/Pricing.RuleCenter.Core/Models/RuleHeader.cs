using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_HEADER")]
public sealed class RuleHeader
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "RULE_CODE")]
    public string RuleCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "RULE_NAME")]
    public string RuleName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "RULE_CATEGORY")]
    public string RuleCategory { get; set; } = "MIXED";

    [SugarColumn(ColumnName = "RULE_SCOPE")]
    public string RuleScope { get; set; } = "ITEM";

    [SugarColumn(ColumnName = "ITEM_CODE", IsNullable = true)]
    public string? ItemCode { get; set; }

    [SugarColumn(ColumnName = "ITEM_NAME", IsNullable = true)]
    public string? ItemName { get; set; }

    [SugarColumn(ColumnName = "GROUP_CODE", IsNullable = true)]
    public string? GroupCode { get; set; }

    [SugarColumn(ColumnName = "PRIORITY")]
    public int Priority { get; set; } = 100;

    [SugarColumn(ColumnName = "CURRENT_VERSION")]
    public int CurrentVersion { get; set; }

    [SugarColumn(ColumnName = "STATUS")]
    public string Status { get; set; } = "DRAFT";

    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";

    [SugarColumn(ColumnName = "EFFECTIVE_FROM", IsNullable = true)]
    public DateTime? EffectiveFrom { get; set; }

    [SugarColumn(ColumnName = "EFFECTIVE_TO", IsNullable = true)]
    public DateTime? EffectiveTo { get; set; }

    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    public string? Remark { get; set; }

    [SugarColumn(ColumnName = "CREATED_BY", IsNullable = true)]
    public string? CreatedBy { get; set; }

    [SugarColumn(ColumnName = "CREATED_AT")]
    public DateTime CreatedAt { get; set; }

    [SugarColumn(ColumnName = "UPDATED_BY", IsNullable = true)]
    public string? UpdatedBy { get; set; }

    [SugarColumn(ColumnName = "UPDATED_AT")]
    public DateTime UpdatedAt { get; set; }
}
