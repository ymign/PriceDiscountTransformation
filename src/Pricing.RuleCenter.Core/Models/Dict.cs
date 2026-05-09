using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_DICT")]
public sealed class Dict
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "DICT_ID")]
    public long DictId { get; set; }

    [SugarColumn(ColumnName = "DICT_TYPE")]
    public string DictType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "DICT_CODE")]
    public string DictCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "DICT_NAME")]
    public string DictName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "PARENT_CODE", IsNullable = true)]
    public string? ParentCode { get; set; }

    [SugarColumn(ColumnName = "SORT_NO")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";

    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    public string? Remark { get; set; }
}
