using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 规则条件响应 DTO。
/// </summary>
public sealed class RuleConditionResponse
{
    /// <summary>
    /// 规则条件主键
    /// </summary>
    public long ConditionId { get; init; }
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; init; }
    /// <summary>
    /// 规则版本号，与规则条件和动作的 VERSION_NO 对齐
    /// </summary>
    public int VersionNo { get; init; }
    /// <summary>
    /// 条件组，同组条件按 AND 处理，不同组按 OR 处理
    /// </summary>
    public string ConditionGroup { get; init; } = string.Empty;
    /// <summary>
    /// 条件类型，决定由哪个条件评估器处理
    /// </summary>
    public string ConditionType { get; init; } = string.Empty;
    /// <summary>
    /// 比较运算符，例如 EQ、IN、BETWEEN
    /// </summary>
    public string? OperatorType { get; init; }
    /// <summary>
    /// 条件左值字段名，通常对应请求上下文中的结构化字段
    /// </summary>
    public string? LeftKey { get; init; }
    /// <summary>
    /// 条件右值，来自规则配置
    /// </summary>
    public string? RightValue { get; init; }
    /// <summary>
    /// 扩展参数 JSON，用于承载动作或条件的可变配置
    /// </summary>
    public string? ParamsJson { get; init; }
    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; init; }
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; init; } = "Y";
}

/// <summary>
/// 单条规则条件保存项 DTO。
/// </summary>
public sealed class RuleConditionItemRequest
{
    [Required(ErrorMessage = "条件组不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 条件组，同组条件按 AND 处理，不同组按 OR 处理
    /// </summary>
    public string ConditionGroup { get; init; } = "DEFAULT";

    [Required(ErrorMessage = "条件类型不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 条件类型，决定由哪个条件评估器处理
    /// </summary>
    public string ConditionType { get; init; } = string.Empty;

    [MaxLength(20)]
    /// <summary>
    /// 比较运算符，例如 EQ、IN、BETWEEN
    /// </summary>
    public string? OperatorType { get; init; } = "EQ";

    [MaxLength(200)]
    /// <summary>
    /// 条件左值字段名，通常对应请求上下文中的结构化字段
    /// </summary>
    public string? LeftKey { get; init; }

    [MaxLength(500)]
    /// <summary>
    /// 条件右值，来自规则配置
    /// </summary>
    public string? RightValue { get; init; }

    /// <summary>
    /// 扩展参数 JSON，用于承载动作或条件的可变配置
    /// </summary>
    public string? ParamsJson { get; init; }
    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; init; }
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; init; } = "Y";
}

/// <summary>
/// 规则条件集合保存请求 DTO。
/// </summary>
public sealed class RuleConditionSaveRequest
{
    [Required(ErrorMessage = "条件列表不能为空")]
    /// <summary>
    /// 当前规则版本下的完整条件集合，保存时会整体替换旧条件。
    /// </summary>
    public IReadOnlyList<RuleConditionItemRequest> Conditions { get; init; } = Array.Empty<RuleConditionItemRequest>();
}
