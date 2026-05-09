using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 字典项响应 DTO。
/// </summary>
public sealed class DictResponse
{
    /// <summary>
    /// 字典项主键。
    /// </summary>
    public long DictId { get; init; }
    /// <summary>
    /// 字典类型编码，用于区分动作类型、条件类型、规则分类等字典域。
    /// </summary>
    public string DictType { get; init; } = string.Empty;
    /// <summary>
    /// 字典项编码，是规则配置中保存的稳定值。
    /// </summary>
    public string DictCode { get; init; } = string.Empty;
    /// <summary>
    /// 字典项显示名称。
    /// </summary>
    public string DictName { get; init; } = string.Empty;
    /// <summary>
    /// 父级字典编码，用于表达级联或分组关系。
    /// </summary>
    public string? ParentCode { get; init; }
    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; init; }
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; init; } = "Y";
    /// <summary>
    /// 字典项备注。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 字典项新增请求 DTO。
/// </summary>
public sealed class DictCreateRequest
{
    [Required(ErrorMessage = "字典类型不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 字典类型编码。
    /// </summary>
    public string DictType { get; init; } = string.Empty;

    [Required(ErrorMessage = "字典编码不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 字典项编码，同一字典类型下必须唯一。
    /// </summary>
    public string DictCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "字典名称不能为空")]
    [MaxLength(200)]
    /// <summary>
    /// 字典项显示名称。
    /// </summary>
    public string DictName { get; init; } = string.Empty;

    /// <summary>
    /// 父级字典编码，用于级联展示。
    /// </summary>
    public string? ParentCode { get; init; }
    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; init; }
    /// <summary>
    /// 字典项备注。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 字典项更新请求 DTO。
/// </summary>
public sealed class DictUpdateRequest
{
    [Required(ErrorMessage = "字典名称不能为空")]
    [MaxLength(200)]
    /// <summary>
    /// 字典项显示名称。
    /// </summary>
    public string DictName { get; init; } = string.Empty;

    /// <summary>
    /// 父级字典编码，用于级联展示。
    /// </summary>
    public string? ParentCode { get; init; }
    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; init; }
    /// <summary>
    /// 字典项备注。
    /// </summary>
    public string? Remark { get; init; }
}
