using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 公式定义响应 DTO。
/// </summary>
public sealed class FormulaDefResponse
{
    /// <summary>
    /// 公式定义主键。
    /// </summary>
    public long FormulaId { get; init; }
    /// <summary>
    /// 公式编码，是规则动作配置引用的稳定业务键。
    /// </summary>
    public string FormulaCode { get; init; } = string.Empty;
    /// <summary>
    /// 公式显示名称。
    /// </summary>
    public string FormulaName { get; init; } = string.Empty;
    /// <summary>
    /// 公式说明，描述适用业务和计算口径。
    /// </summary>
    public string? FormulaDesc { get; init; }
    /// <summary>
    /// 执行器编码，用于在同一动作类型下区分具体公式或策略
    /// </summary>
    public string ExecutorCode { get; init; } = string.Empty;
    /// <summary>
    /// 参数结构 JSON，用于约束前端配置动作参数。
    /// </summary>
    public string? ParamSchemaJson { get; init; }
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; init; } = "Y";
    /// <summary>
    /// 公式备注。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 公式定义新增请求 DTO。
/// </summary>
public sealed class FormulaDefCreateRequest
{
    [Required(ErrorMessage = "公式编码不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 公式编码，新增后作为稳定引用键。
    /// </summary>
    public string FormulaCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "公式名称不能为空")]
    [MaxLength(200)]
    /// <summary>
    /// 公式显示名称。
    /// </summary>
    public string FormulaName { get; init; } = string.Empty;

    /// <summary>
    /// 公式说明。
    /// </summary>
    public string? FormulaDesc { get; init; }

    [Required(ErrorMessage = "执行器编码不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 执行器编码，用于在同一动作类型下区分具体公式或策略
    /// </summary>
    public string ExecutorCode { get; init; } = string.Empty;

    /// <summary>
    /// 参数结构 JSON。
    /// </summary>
    public string? ParamSchemaJson { get; init; }
    /// <summary>
    /// 公式备注。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 公式定义更新请求 DTO。
/// </summary>
public sealed class FormulaDefUpdateRequest
{
    [Required(ErrorMessage = "公式名称不能为空")]
    [MaxLength(200)]
    /// <summary>
    /// 公式显示名称。
    /// </summary>
    public string FormulaName { get; init; } = string.Empty;

    /// <summary>
    /// 公式说明。
    /// </summary>
    public string? FormulaDesc { get; init; }

    [Required(ErrorMessage = "执行器编码不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 执行器编码，用于在同一动作类型下区分具体公式或策略
    /// </summary>
    public string ExecutorCode { get; init; } = string.Empty;

    /// <summary>
    /// 参数结构 JSON。
    /// </summary>
    public string? ParamSchemaJson { get; init; }
    /// <summary>
    /// 公式备注。
    /// </summary>
    public string? Remark { get; init; }
}
