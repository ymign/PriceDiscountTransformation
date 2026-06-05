using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 公式定义响应 DTO，返回规则中心已注册的计价公式元数据。
/// </summary>
/// <remarks>
/// <para>
/// 公式定义（PR_FORMULA_DEF 表）是规则动作执行器的配置基座。规则动作通过
/// <see cref="FormulaCode"/> 引用公式，计价引擎在执行阶段根据
/// <see cref="ExecutorCode"/> 路由到对应的 <c>IPricingFormulaExecutor</c> 实现。
/// </para>
/// <para>
/// 公式与规则动作是"一对多"关系：同一个公式可被多条规则的动作引用。
/// 新增公式类型只需新增执行器实现并注册公式定义，无需修改各渠道代码。
/// </para>
/// <para>
/// <see cref="ParamSchemaJson"/> 以 JSON Schema 格式描述公式接受的参数结构，
/// 前端工作台据此动态渲染参数配置表单，确保配置人员输入的参数格式合法。
/// </para>
/// <para>
/// 对应接口：GET <c>/api/formulas</c> 列表查询、GET <c>/api/formulas/{formulaId}</c> 单条查询。
/// </para>
/// </remarks>
public sealed class FormulaDefResponse
{
    /// <summary>
    /// 公式定义主键，对应 PR_FORMULA_DEF.FORMULA_ID，由序列 PR_FORMULA_DEF_SEQ 生成。
    /// </summary>
    public long FormulaId { get; init; }

    /// <summary>
    /// 公式编码，全局唯一的稳定业务键，规则动作通过此编码引用公式。
    /// 例如 "AREA_PRICE"（面积计价）、"WEIGHT_DISCOUNT"（重量折价）。
    /// 一旦被规则引用，不可修改编码值。
    /// </summary>
    public string FormulaCode { get; init; } = string.Empty;

    /// <summary>
    /// 公式显示名称，面向配置人员的可读文本。
    /// 例如 "按面积计价公式""按重量阶梯折价"。
    /// </summary>
    public string FormulaName { get; init; } = string.Empty;

    /// <summary>
    /// 公式说明，描述该公式的适用业务场景、计算口径和使用注意事项。
    /// 例如 "适用于皮肤科多肿物项目，按每个肿物面积独立计价后汇总"。
    /// </summary>
    public string? FormulaDesc { get; init; }

    /// <summary>
    /// 执行器编码，决定计价引擎路由到哪个 <c>IPricingFormulaExecutor</c> 实现。
    /// 同一动作类型（如 FORMULA）下可有多个执行器，通过此编码区分具体计算策略。
    /// 执行器编码与公式编码是"多对一"关系：多个公式可共享同一执行器实现。
    /// </summary>
    public string ExecutorCode { get; init; } = string.Empty;

    /// <summary>
    /// 参数结构 JSON（JSON Schema 格式），描述该公式接受的参数名称、类型、取值范围和必填性。
    /// 前端工作台据此动态渲染参数配置表单；计价引擎执行前据此校验动作参数合法性。
    /// 为 null 表示该公式不需要额外参数。
    /// </summary>
    public string? ParamSchemaJson { get; init; }

    /// <summary>
    /// 启用标识。"Y" 表示该公式可被规则动作引用和执行；"N" 表示已停用，
    /// 前端不再展示，但已引用该公式的历史规则仍可正常追溯。
    /// </summary>
    public string IsEnabled { get; init; } = "Y";

    /// <summary>
    /// 公式备注，用于记录维护说明、变更原因或待确认事项。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 公式定义新增请求 DTO，用于向规则中心注册新的计价公式。
/// </summary>
/// <remarks>
/// <para>
/// 新增公式后，还需在规则动作中通过 <see cref="FormulaCode"/> 引用才能生效。
/// 系统会校验 <see cref="FormulaCode"/> 全局唯一性，重复则返回 409 冲突错误。
/// </para>
/// <para>
/// 对应接口：POST <c>/api/formulas</c>。
/// </para>
/// </remarks>
public sealed class FormulaDefCreateRequest
{
    /// <summary>
    /// 公式编码（必填），全局唯一的稳定业务键。
    /// 编码规则：大写字母和下划线，如 AREA_PRICE。创建后不可修改。
    /// </summary>
    [Required(ErrorMessage = "公式编码不能为空")]
    [MaxLength(50)]
    public string FormulaCode { get; init; } = string.Empty;

    /// <summary>
    /// 公式显示名称（必填），面向配置人员的可读文本。
    /// </summary>
    [Required(ErrorMessage = "公式名称不能为空")]
    [MaxLength(200)]
    public string FormulaName { get; init; } = string.Empty;

    /// <summary>
    /// 公式说明（选填），描述适用业务场景和计算口径。
    /// </summary>
    public string? FormulaDesc { get; init; }

    /// <summary>
    /// 执行器编码（必填），决定计价引擎路由到哪个执行器实现。
    /// 必须与后端已注册的 <c>IPricingFormulaExecutor</c> 实现匹配，否则运行时会报执行器未找到。
    /// </summary>
    [Required(ErrorMessage = "执行器编码不能为空")]
    [MaxLength(50)]
    public string ExecutorCode { get; init; } = string.Empty;

    /// <summary>
    /// 参数结构 JSON（选填），JSON Schema 格式。
    /// 提供后前端工作台会据此渲染参数配置表单；为 null 表示不需要额外参数。
    /// </summary>
    public string? ParamSchemaJson { get; init; }

    /// <summary>
    /// 公式备注（选填）。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 公式定义更新请求 DTO，用于修改已有公式的展示信息和参数配置。
/// </summary>
/// <remarks>
/// <para>
/// 更新操作允许修改 <see cref="FormulaName"/>、<see cref="FormulaDesc"/>、
/// <see cref="ExecutorCode"/>、<see cref="ParamSchemaJson"/>、<see cref="Remark"/>，
/// 但不允许修改 <c>FormulaCode</c>（因为已被规则动作引用）。
/// </para>
/// <para>
/// 修改 <see cref="ExecutorCode"/> 或 <see cref="ParamSchemaJson"/> 可能影响已引用该公式的规则，
/// 操作前应确认影响范围。
/// </para>
/// <para>
/// 对应接口：PUT <c>/api/formulas/{formulaId}</c>。
/// </para>
/// </remarks>
public sealed class FormulaDefUpdateRequest
{
    /// <summary>
    /// 公式显示名称（必填）。
    /// </summary>
    [Required(ErrorMessage = "公式名称不能为空")]
    [MaxLength(200)]
    public string FormulaName { get; init; } = string.Empty;

    /// <summary>
    /// 公式说明（选填）。
    /// </summary>
    public string? FormulaDesc { get; init; }

    /// <summary>
    /// 执行器编码（必填），决定计价引擎路由到哪个执行器实现。
    /// </summary>
    [Required(ErrorMessage = "执行器编码不能为空")]
    [MaxLength(50)]
    public string ExecutorCode { get; init; } = string.Empty;

    /// <summary>
    /// 参数结构 JSON（选填），JSON Schema 格式。更新后会影响前端表单渲染。
    /// </summary>
    public string? ParamSchemaJson { get; init; }

    /// <summary>
    /// 公式备注（选填）。
    /// </summary>
    public string? Remark { get; init; }
}

