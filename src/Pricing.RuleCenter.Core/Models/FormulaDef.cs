using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 公式定义实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_FORMULA_DEF
/// </para>
/// <para>
/// 在规则体系中的角色：公式定义把业务可选公式、执行器编码和参数结构保存为可配置元数据。
/// 工作台配置人员通过下拉选择公式名称来关联规则动作，无需手动输入执行器编码。
/// </para>
/// <para>
/// 公式与限制的交互（关键业务约束）：
/// <list type="bullet">
/// <item>公式优先于限制 — 公式 + 金额限制共存时，先算公式，再与限制比较</item>
/// <item>公式不自动跳过限制 — 公式项目是否执行数量、时间窗、同组、同手术限制，
/// 以规则动作配置为准；禁止代码层一见公式就跳过全部数量类限制</item>
/// <item>换算数量固定为 1 — 公式使用换算后数量，非输入数量</item>
/// </list>
/// </para>
/// </remarks>
[SugarTable("PR_FORMULA_DEF")]
public sealed class FormulaDef
{
    /// <summary>
    /// 公式定义主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 FORMULA_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条公式定义记录。
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true, ColumnName = "FORMULA_ID")]
    public long FormulaId { get; set; }

    /// <summary>
    /// 公式编码。
    /// </summary>
    /// <remarks>
    /// 全局唯一的业务键，是规则动作配置引用的稳定标识。
    /// 例如：AREA_PRICE（面积单价公式）、PART_PRICE（部位单价公式）、FIXED_PRICE（固定价格）。
    /// 规则动作的 ExecutorCode 字段引用该编码。
    /// 修改编码需评估所有引用该公式的规则动作。
    /// </remarks>
    [SugarColumn(ColumnName = "FORMULA_CODE")]
    public string FormulaCode { get; set; } = string.Empty;

    /// <summary>
    /// 公式显示名称。
    /// </summary>
    /// <remarks>
    /// 面向配置人员展示的中文名称，用于工作台下拉选择。
    /// 例如：公式编码 "AREA_PRICE" 对应名称 "面积单价公式"。
    /// </remarks>
    [SugarColumn(ColumnName = "FORMULA_NAME")]
    public string FormulaName { get; set; } = string.Empty;

    /// <summary>
    /// 公式说明。
    /// </summary>
    /// <remarks>
    /// 描述公式的适用业务场景和计算口径。
    /// 例如："按面积（CM2）乘以单价系数计算收费金额，适用于皮肤科等按面积收费项目"。
    /// 帮助配置人员理解公式的用途和参数含义。
    /// </remarks>
    [SugarColumn(ColumnName = "FORMULA_DESC", IsNullable = true)]
    public string? FormulaDesc { get; set; }

    /// <summary>
    /// 执行器编码。
    /// </summary>
    /// <remarks>
    /// 指定由哪个 IPricingFormulaExecutor 实现来执行该公式。
    /// 例如：AREA_FORMULA_EXECUTOR、PART_FORMULA_EXECUTOR。
    /// 计价引擎根据该编码在依赖注入容器中查找对应的执行器实例。
    /// 新增公式类型只需新增执行器实现并注册到容器。
    /// </remarks>
    [SugarColumn(ColumnName = "EXECUTOR_CODE")]
    public string ExecutorCode { get; set; } = string.Empty;

    /// <summary>
    /// 参数结构 JSON Schema。
    /// </summary>
    /// <remarks>
    /// 用于约束规则动作的 PARAMS_JSON 配置，定义参数名称、类型、必填性、默认值等。
    /// 工作台根据该 Schema 动态渲染参数配置表单，防止配置人员输入错误参数。
    /// 例如：{"baseQty": {"type": "decimal", "required": true}, "multiplier": {"type": "decimal", "default": 1.0}}
    /// Oracle 存储类型为 CLOB，无原生 JSON 支持。
    /// </remarks>
    [SugarColumn(ColumnName = "PARAM_SCHEMA_JSON", ColumnDataType = "CLOB", IsNullable = true)]
    public string? ParamSchemaJson { get; set; }

    /// <summary>
    /// 启用标识。
    /// </summary>
    /// <remarks>
    /// "Y" 表示该公式在工作台下拉列表中可选，"N" 表示禁用。
    /// 禁用的公式不出现在下拉列表中，但已引用的规则动作不受影响（仍可正常执行）。
    /// 默认值为 "Y"。
    /// </remarks>
    [SugarColumn(ColumnName = "IS_ENABLED")]
    public string IsEnabled { get; set; } = "Y";

    /// <summary>
    /// 公式备注。
    /// </summary>
    /// <remarks>
    /// 配置人员或开发人员填写的补充说明。
    /// 例如："该公式于 2026 年 5 月新增，替代旧版 AREA_PRICE_V1"。
    /// </remarks>
    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    public string? Remark { get; set; }
}
