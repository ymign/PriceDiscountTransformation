using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_ITEM_GROUP")]
/// <summary>
/// 项目组实体，对应 PR_ITEM_GROUP。
/// </summary>
public sealed class ItemGroup
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "GROUP_ID")]
    /// <summary>
    /// 项目组主键。
    /// </summary>
    public long GroupId { get; set; }

    [SugarColumn(ColumnName = "GROUP_CODE")]
    /// <summary>
    /// 项目组编码，规则作用范围为项目组时使用。
    /// </summary>
    public string GroupCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "GROUP_NAME")]
    /// <summary>
    /// 项目组名称。
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "GROUP_TYPE")]
    /// <summary>
    /// 项目组类型，例如主子项目组或同类互斥项目组。
    /// </summary>
    public string GroupType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "IS_ENABLED")]
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; set; } = "Y";

    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    /// <summary>
    /// 项目组备注。
    /// </summary>
    public string? Remark { get; set; }

    [SugarColumn(ColumnName = "CREATED_BY", IsNullable = true)]
    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }

    [SugarColumn(ColumnName = "CREATED_AT")]
    /// <summary>
    /// 记录创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    [SugarColumn(ColumnName = "UPDATED_BY", IsNullable = true)]
    /// <summary>
    /// 最后修改人
    /// </summary>
    public string? UpdatedBy { get; set; }

    [SugarColumn(ColumnName = "UPDATED_AT")]
    /// <summary>
    /// 记录最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
