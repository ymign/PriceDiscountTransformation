using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_DICT")]
/// <summary>
/// 字典项实体，对应 PR_DICT。
/// </summary>
public sealed class Dict
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "DICT_ID")]
    /// <summary>
    /// 字典项主键。
    /// </summary>
    public long DictId { get; set; }

    [SugarColumn(ColumnName = "DICT_TYPE")]
    /// <summary>
    /// 字典类型编码。
    /// </summary>
    public string DictType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "DICT_CODE")]
    /// <summary>
    /// 字典项编码，同一类型下应保持唯一。
    /// </summary>
    public string DictCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "DICT_NAME")]
    /// <summary>
    /// 字典项显示名称。
    /// </summary>
    public string DictName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "PARENT_CODE", IsNullable = true)]
    /// <summary>
    /// 父级字典编码，用于级联或分组展示。
    /// </summary>
    public string? ParentCode { get; set; }

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

    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    /// <summary>
    /// 字典项备注。
    /// </summary>
    public string? Remark { get; set; }
}
