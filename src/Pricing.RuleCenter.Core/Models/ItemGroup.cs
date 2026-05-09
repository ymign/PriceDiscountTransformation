using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_ITEM_GROUP")]
public sealed class ItemGroup
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "GROUP_ID")]
    public long GroupId { get; set; }

    [SugarColumn(ColumnName = "GROUP_CODE")]
    public string GroupCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "GROUP_NAME")]
    public string GroupName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "GROUP_TYPE")]
    public string GroupType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";

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
