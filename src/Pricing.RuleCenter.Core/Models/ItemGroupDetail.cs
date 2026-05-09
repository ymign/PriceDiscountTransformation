using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_ITEM_GROUP_DETAIL")]
/// <summary>
/// 项目组明细实体，对应 PR_ITEM_GROUP_DETAIL。
/// </summary>
public sealed class ItemGroupDetail
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "DETAIL_ID")]
    /// <summary>
    /// 项目组明细主键。
    /// </summary>
    public long DetailId { get; set; }

    [SugarColumn(ColumnName = "GROUP_ID")]
    /// <summary>
    /// 所属项目组主键。
    /// </summary>
    public long GroupId { get; set; }

    [SugarColumn(ColumnName = "ITEM_CODE")]
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string ItemCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "ITEM_NAME", IsNullable = true)]
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明
    /// </summary>
    public string? ItemName { get; set; }

    [SugarColumn(ColumnName = "ROLE_TYPE")]
    /// <summary>
    /// 项目在组内的角色，例如 MAIN 主项目或 CHILD 子项目。
    /// </summary>
    public string RoleType { get; set; } = "MAIN";

    [SugarColumn(ColumnName = "SORT_NO")]
    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "IS_ENABLED")]
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; set; } = "Y";
}
