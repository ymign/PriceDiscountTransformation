using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

public sealed class DictResponse
{
    public long DictId { get; init; }
    public string DictType { get; init; } = string.Empty;
    public string DictCode { get; init; } = string.Empty;
    public string DictName { get; init; } = string.Empty;
    public string? ParentCode { get; init; }
    public int SortNo { get; init; }
    public string IsEnabled { get; init; } = "Y";
    public string? Remark { get; init; }
}

public sealed class DictCreateRequest
{
    [Required(ErrorMessage = "字典类型不能为空")]
    [MaxLength(50)]
    public string DictType { get; init; } = string.Empty;

    [Required(ErrorMessage = "字典编码不能为空")]
    [MaxLength(50)]
    public string DictCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "字典名称不能为空")]
    [MaxLength(200)]
    public string DictName { get; init; } = string.Empty;

    public string? ParentCode { get; init; }
    public int SortNo { get; init; }
    public string? Remark { get; init; }
}

public sealed class DictUpdateRequest
{
    [Required(ErrorMessage = "字典名称不能为空")]
    [MaxLength(200)]
    public string DictName { get; init; } = string.Empty;

    public string? ParentCode { get; init; }
    public int SortNo { get; init; }
    public string? Remark { get; init; }
}
