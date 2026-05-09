using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 规则主档响应 DTO。
/// </summary>
public sealed class RuleHeaderResponse
{
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; init; }
    /// <summary>
    /// 规则编码，全局唯一，用于业务配置和运维识别
    /// </summary>
    public string RuleCode { get; init; } = string.Empty;
    /// <summary>
    /// 规则名称，用于工作台展示和审计
    /// </summary>
    public string RuleName { get; init; } = string.Empty;
    /// <summary>
    /// 规则类别，例如折价、公式、限额或混合规则
    /// </summary>
    public string RuleCategory { get; init; } = string.Empty;
    /// <summary>
    /// 规则作用范围，例如单项目、项目组或场景
    /// </summary>
    public string RuleScope { get; init; } = string.Empty;
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; init; }
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明
    /// </summary>
    public string? ItemName { get; init; }
    /// <summary>
    /// 项目组编码，规则作用范围为项目组时用于匹配。
    /// </summary>
    public string? GroupCode { get; init; }
    /// <summary>
    /// 规则优先级，数字越小越先参与匹配和动作排序
    /// </summary>
    public int Priority { get; init; }
    /// <summary>
    /// 当前生效版本号，发布或回滚时由规则生命周期服务维护
    /// </summary>
    public int CurrentVersion { get; init; }
    /// <summary>
    /// 当前记录状态，具体含义由所在表的状态机定义
    /// </summary>
    public string Status { get; init; } = string.Empty;
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; init; } = "Y";
    /// <summary>
    /// 规则或版本的生效开始时间
    /// </summary>
    public DateTime? EffectiveFrom { get; init; }
    /// <summary>
    /// 规则或版本的生效结束时间，空值表示未设失效时间
    /// </summary>
    public DateTime? EffectiveTo { get; init; }
    /// <summary>
    /// 规则备注。
    /// </summary>
    public string? Remark { get; init; }
    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; init; }
    /// <summary>
    /// 记录创建时间
    /// </summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>
    /// 最后修改人
    /// </summary>
    public string? UpdatedBy { get; init; }
    /// <summary>
    /// 记录最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; init; }
}

/// <summary>
/// 规则主档新增请求 DTO。
/// </summary>
public sealed class RuleHeaderCreateRequest
{
    [Required(ErrorMessage = "规则编码不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 规则编码，全局唯一，用于业务配置和运维识别
    /// </summary>
    public string RuleCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "规则名称不能为空")]
    [MaxLength(200)]
    /// <summary>
    /// 规则名称，用于工作台展示和审计
    /// </summary>
    public string RuleName { get; init; } = string.Empty;

    [MaxLength(20)]
    /// <summary>
    /// 规则类别，例如折价、公式、限额或混合规则
    /// </summary>
    public string RuleCategory { get; init; } = "MIXED";

    [MaxLength(20)]
    /// <summary>
    /// 规则作用范围，例如单项目、项目组或场景
    /// </summary>
    public string RuleScope { get; init; } = "ITEM";

    [MaxLength(50)]
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; init; }

    [MaxLength(200)]
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明
    /// </summary>
    public string? ItemName { get; init; }

    [MaxLength(50)]
    /// <summary>
    /// 项目组编码，规则作用范围为项目组时填写。
    /// </summary>
    public string? GroupCode { get; init; }

    /// <summary>
    /// 规则优先级，数字越小越先参与匹配和动作排序
    /// </summary>
    public int Priority { get; init; } = 100;

    /// <summary>
    /// 规则或版本的生效开始时间
    /// </summary>
    public DateTime? EffectiveFrom { get; init; }
    /// <summary>
    /// 规则或版本的生效结束时间，空值表示未设失效时间
    /// </summary>
    public DateTime? EffectiveTo { get; init; }
    /// <summary>
    /// 规则备注。
    /// </summary>
    public string? Remark { get; init; }
    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; init; }
}

/// <summary>
/// 规则主档更新请求 DTO。
/// </summary>
public sealed class RuleHeaderUpdateRequest
{
    [Required(ErrorMessage = "规则名称不能为空")]
    [MaxLength(200)]
    /// <summary>
    /// 规则名称，用于工作台展示和审计
    /// </summary>
    public string RuleName { get; init; } = string.Empty;

    [MaxLength(20)]
    /// <summary>
    /// 规则类别，例如折价、公式、限额或混合规则
    /// </summary>
    public string RuleCategory { get; init; } = "MIXED";

    [MaxLength(20)]
    /// <summary>
    /// 规则作用范围，例如单项目、项目组或场景
    /// </summary>
    public string RuleScope { get; init; } = "ITEM";

    [MaxLength(50)]
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; init; }

    [MaxLength(200)]
    /// <summary>
    /// 项目名称，用于展示、审计和追溯说明
    /// </summary>
    public string? ItemName { get; init; }

    [MaxLength(50)]
    /// <summary>
    /// 项目组编码，规则作用范围为项目组时填写。
    /// </summary>
    public string? GroupCode { get; init; }

    /// <summary>
    /// 规则优先级，数字越小越先参与匹配和动作排序
    /// </summary>
    public int Priority { get; init; } = 100;

    /// <summary>
    /// 规则或版本的生效开始时间
    /// </summary>
    public DateTime? EffectiveFrom { get; init; }
    /// <summary>
    /// 规则或版本的生效结束时间，空值表示未设失效时间
    /// </summary>
    public DateTime? EffectiveTo { get; init; }
    /// <summary>
    /// 规则备注。
    /// </summary>
    public string? Remark { get; init; }
    /// <summary>
    /// 最后修改人
    /// </summary>
    public string? UpdatedBy { get; init; }
}

/// <summary>
/// 规则主档分页查询请求 DTO。
/// </summary>
public sealed class RuleHeaderPagedRequest
{
    /// <summary>
    /// 项目编码，是规则匹配、价格校验和限额累计的核心维度
    /// </summary>
    public string? ItemCode { get; init; }
    /// <summary>
    /// 当前记录状态，具体含义由所在表的状态机定义
    /// </summary>
    public string? Status { get; init; }
    /// <summary>
    /// 规则分类筛选条件。
    /// </summary>
    public string? Category { get; init; }
    /// <summary>
    /// 页码，从 1 开始。
    /// </summary>
    public int PageIndex { get; init; } = 1;
    /// <summary>
    /// 每页记录数。
    /// </summary>
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// 通用分页响应 DTO。
/// </summary>
public sealed class PagedResponse<T>
{
    /// <summary>
    /// 当前页数据集合。
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    /// <summary>
    /// 符合条件的总记录数。
    /// </summary>
    public int Total { get; init; }
    /// <summary>
    /// 当前页码。
    /// </summary>
    public int PageIndex { get; init; }
    /// <summary>
    /// 每页记录数。
    /// </summary>
    public int PageSize { get; init; }
}
