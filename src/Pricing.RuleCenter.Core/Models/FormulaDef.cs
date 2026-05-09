using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_FORMULA_DEF")]
/// <summary>
/// 公式定义实体，对应 PR_FORMULA_DEF。
/// </summary>
/// <remarks>
/// 公式定义把业务可选公式、执行器编码和参数结构保存为可配置元数据。
/// </remarks>
public sealed class FormulaDef
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "FORMULA_ID")]
    /// <summary>
    /// 公式定义主键。
    /// </summary>
    public long FormulaId { get; set; }

    [SugarColumn(ColumnName = "FORMULA_CODE")]
    /// <summary>
    /// 公式编码，是规则动作配置引用的稳定业务键。
    /// </summary>
    public string FormulaCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "FORMULA_NAME")]
    /// <summary>
    /// 公式显示名称。
    /// </summary>
    public string FormulaName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "FORMULA_DESC", IsNullable = true)]
    /// <summary>
    /// 公式说明，描述适用业务和计算口径。
    /// </summary>
    public string? FormulaDesc { get; set; }

    [SugarColumn(ColumnName = "EXECUTOR_CODE")]
    /// <summary>
    /// 执行器编码，用于在同一动作类型下区分具体公式或策略
    /// </summary>
    public string ExecutorCode { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "PARAM_SCHEMA_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 参数结构 JSON，用于约束动作参数配置。
    /// </summary>
    public string? ParamSchemaJson { get; set; }

    [SugarColumn(ColumnName = "IS_ENABLED")]
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; set; } = "Y";

    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    /// <summary>
    /// 公式备注。
    /// </summary>
    public string? Remark { get; set; }
}
