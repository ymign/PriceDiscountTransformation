using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_ITEM_GROUP_DETAIL")]
public sealed class ItemGroupDetail
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "DETAIL_ID")]
    public long DetailId { get; set; }

    [SugarColumn(ColumnName = "GROUP_ID")]
    public long GroupId { get; set; }

    [SugarColumn(ColumnName = "ITEM_CODE")]
    public string ItemCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "ITEM_NAME", IsNullable = true)]
    public string? ItemName { get; set; }

    [SugarColumn(ColumnName = "ROLE_TYPE")]
    public string RoleType { get; set; } = "MAIN";

    [SugarColumn(ColumnName = "SORT_NO")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";
}
