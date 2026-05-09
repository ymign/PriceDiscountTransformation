using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

public sealed class FormulaDefResponse
{
    public long FormulaId { get; init; }
    public string FormulaCode { get; init; } = string.Empty;
    public string FormulaName { get; init; } = string.Empty;
    public string? FormulaDesc { get; init; }
    public string ExecutorCode { get; init; } = string.Empty;
    public string? ParamSchemaJson { get; init; }
    public string IsEnabled { get; init; } = "Y";
    public string? Remark { get; init; }
}

public sealed class FormulaDefCreateRequest
{
    [Required(ErrorMessage = "公式编码不能为空")]
    [MaxLength(50)]
    public string FormulaCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "公式名称不能为空")]
    [MaxLength(200)]
    public string FormulaName { get; init; } = string.Empty;

    public string? FormulaDesc { get; init; }

    [Required(ErrorMessage = "执行器编码不能为空")]
    [MaxLength(50)]
    public string ExecutorCode { get; init; } = string.Empty;

    public string? ParamSchemaJson { get; init; }
    public string? Remark { get; init; }
}

public sealed class FormulaDefUpdateRequest
{
    [Required(ErrorMessage = "公式名称不能为空")]
    [MaxLength(200)]
    public string FormulaName { get; init; } = string.Empty;

    public string? FormulaDesc { get; init; }

    [Required(ErrorMessage = "执行器编码不能为空")]
    [MaxLength(50)]
    public string ExecutorCode { get; init; } = string.Empty;

    public string? ParamSchemaJson { get; init; }
    public string? Remark { get; init; }
}
