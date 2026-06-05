using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 字典项响应 DTO，用于返回规则中心内置字典的单条记录。
/// </summary>
/// <remarks>
/// <para>
/// 规则中心的内置字典（PR_DICT 表）承载了计价类型、单位、公式类型、条件类型、动作类型、
/// 规则分类等下拉选项的数据来源。前端工作台通过字典接口获取可选项，规则配置中保存的是
/// <see cref="DictCode"/>（稳定编码），展示时使用 <see cref="DictName"/>（可读名称）。
/// </para>
/// <para>
/// 字典通过 <see cref="DictType"/> 划分域，同一 <see cref="DictType"/> 下的
/// <see cref="DictCode"/> 必须唯一。<see cref="ParentCode"/> 支持两级级联关系，
/// 如"动作类型"下挂"公式类动作""限额类动作"等子项。
/// </para>
/// <para>
/// 对应接口：GET <c>/api/dict</c> 列表查询、GET <c>/api/dict/{dictId}</c> 单条查询。
/// </para>
/// </remarks>
public sealed class DictResponse
{
    /// <summary>
    /// 字典项主键，对应 PR_DICT.DICT_ID，由序列 PR_DICT_SEQ 生成。
    /// </summary>
    public long DictId { get; init; }

    /// <summary>
    /// 字典类型编码，用于区分不同的字典域。
    /// 常见值：ACTION_TYPE（动作类型）、CONDITION_TYPE（条件类型）、RULE_CATEGORY（规则分类）、
    /// FORMULA_TYPE（公式类型）、UNIT（计量单位）等。
    /// 同一 DictType 下的 DictCode 构成一个下拉选项组。
    /// </summary>
    public string DictType { get; init; } = string.Empty;

    /// <summary>
    /// 字典项编码，是规则配置中保存到数据库的稳定业务键。
    /// 例如 ACTION_TYPE 域下的 "DISCOUNT_AMOUNT"（金额折价）、"LIMIT_QTY"（数量限制）。
    /// 编码一旦被规则引用，不应随意修改，否则会导致规则配置失效。
    /// </summary>
    public string DictCode { get; init; } = string.Empty;

    /// <summary>
    /// 字典项显示名称，面向配置人员的可读文本。
    /// 例如 "金额折价""数量限制""双单位换算" 等。仅用于前端展示，不参与规则匹配。
    /// </summary>
    public string DictName { get; init; } = string.Empty;

    /// <summary>
    /// 父级字典编码，用于表达两级级联或分组关系。
    /// 为 null 时表示该字典项是顶级项；非空时指向同 DictType 或关联 DictType 下的父项 DictCode。
    /// 例如"公式类动作"的 ParentCode 指向"动作类型"顶级分类。
    /// </summary>
    public string? ParentCode { get; init; }

    /// <summary>
    /// 排序号，控制同一字典类型下的展示顺序和同类动作的执行优先级。
    /// 值越小越靠前。默认值为 0。
    /// </summary>
    public int SortNo { get; init; }

    /// <summary>
    /// 启用标识。"Y" 表示该字典项参与前端下拉展示和规则匹配；"N" 表示已停用，
    /// 前端不再展示，但已引用该编码的历史规则仍可正常追溯。
    /// </summary>
    public string IsEnabled { get; init; } = "Y";

    /// <summary>
    /// 字典项备注，用于记录该字典项的业务说明、使用场景或维护注意事项。
    /// 仅面向配置和运维人员，不参与业务逻辑。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 字典项新增请求 DTO，用于向规则中心内置字典添加新的下拉选项。
/// </summary>
/// <remarks>
/// <para>
/// 新增时系统会校验同一 <see cref="DictType"/> 下 <see cref="DictCode"/> 是否重复，
/// 重复则返回 409 冲突错误。新增后默认 IsEnabled = "Y"。
/// </para>
/// <para>
/// 对应接口：POST <c>/api/dict</c>。
/// </para>
/// </remarks>
public sealed class DictCreateRequest
{
    [Required(ErrorMessage = "字典类型不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 字典类型编码（必填）。决定该字典项归属哪个域，如 ACTION_TYPE、CONDITION_TYPE 等。
    /// 如果指定的 DictType 不存在，接口会返回参数校验失败。
    /// </summary>
    public string DictType { get; init; } = string.Empty;

    [Required(ErrorMessage = "字典编码不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 字典项编码（必填），同一 DictType 下必须全局唯一。
    /// 编码规则：大写字母和下划线，如 DISCOUNT_AMOUNT。一旦创建后被规则引用，不可随意修改。
    /// </summary>
    public string DictCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "字典名称不能为空")]
    [MaxLength(200)]
    /// <summary>
    /// 字典项显示名称（必填），面向配置人员的可读文本。
    /// 允许中文，如"金额折价""数量限制"。
    /// </summary>
    public string DictName { get; init; } = string.Empty;

    /// <summary>
    /// 父级字典编码（选填）。为空时表示顶级项；非空时必须指向同域或关联域中已存在的 DictCode。
    /// 用于构建前端级联选择器的数据关系。
    /// </summary>
    public string? ParentCode { get; init; }

    /// <summary>
    /// 排序号（选填），控制展示顺序。默认值 0。值越小越靠前。
    /// </summary>
    public int SortNo { get; init; }

    /// <summary>
    /// 字典项备注（选填），用于记录业务说明或维护注意事项。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 字典项更新请求 DTO，用于修改已有字典项的展示信息。
/// </summary>
/// <remarks>
/// <para>
/// 更新操作仅允许修改 <see cref="DictName"/>、<see cref="ParentCode"/>、<see cref="SortNo"/>、
/// <see cref="Remark"/> 等展示属性，不允许修改 <see cref="DictCode"/>（因为已被规则引用）。
/// 如需废弃某字典项，应通过启用/停用接口将 IsEnabled 置为 "N"。
/// </para>
/// <para>
/// 对应接口：PUT <c>/api/dict/{dictId}</c>。
/// </para>
/// </remarks>
public sealed class DictUpdateRequest
{
    [Required(ErrorMessage = "字典名称不能为空")]
    [MaxLength(200)]
    /// <summary>
    /// 字典项显示名称（必填），面向配置人员的可读文本。
    /// </summary>
    public string DictName { get; init; } = string.Empty;

    /// <summary>
    /// 父级字典编码（选填）。更新时可重新指定父级关系，但必须确保不形成循环引用。
    /// </summary>
    public string? ParentCode { get; init; }

    /// <summary>
    /// 排序号（选填），控制展示顺序。值越小越靠前。
    /// </summary>
    public int SortNo { get; init; }

    /// <summary>
    /// 字典项备注（选填）。
    /// </summary>
    public string? Remark { get; init; }
}

