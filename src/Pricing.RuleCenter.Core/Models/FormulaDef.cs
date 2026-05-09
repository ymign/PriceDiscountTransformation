using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_FORMULA_DEF")]
public sealed class FormulaDef
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "FORMULA_ID")]
    public long FormulaId { get; set; }

    [SugarColumn(ColumnName = "FORMULA_CODE")]
    public string FormulaCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "FORMULA_NAME")]
    public string FormulaName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "FORMULA_DESC", IsNullable = true)]
    public string? FormulaDesc { get; set; }

    [SugarColumn(ColumnName = "EXECUTOR_CODE")]
    public string ExecutorCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "PARAM_SCHEMA_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    public string? ParamSchemaJson { get; set; }

    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";

    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    public string? Remark { get; set; }
}
