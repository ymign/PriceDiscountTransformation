using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 规则动作响应 DTO。
/// </summary>
public sealed class RuleActionResponse
{
    /// <summary>
    /// 规则动作主键
    /// </summary>
    public long ActionId { get; init; }
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; init; }
    /// <summary>
    /// 规则版本号，与规则条件和动作的 VERSION_NO 对齐
    /// </summary>
    public int VersionNo { get; init; }
    /// <summary>
    /// 动作类型，决定由哪个动作执行器处理
    /// </summary>
    public string ActionType { get; init; } = string.Empty;
    /// <summary>
    /// 执行器编码，用于在同一动作类型下区分具体公式或策略
    /// </summary>
    public string ExecutorCode { get; init; } = string.Empty;
    /// <summary>
    /// 扩展参数 JSON，用于承载动作或条件的可变配置
    /// </summary>
    public string? ParamsJson { get; init; }
    /// <summary>
    /// 互斥组编码，同组动作只应执行优先级最高的一条
    /// </summary>
    public string? ExclusiveGroup { get; init; }
    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; init; }
    /// <summary>
    /// 动作异常处理策略，资金相关动作默认应 STOP
    /// </summary>
    public string OnError { get; init; } = "STOP";
    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; init; } = "Y";
}

/// <summary>
/// 单条规则动作保存项 DTO。
/// </summary>
public sealed class RuleActionItemRequest
{
    [Required(ErrorMessage = "动作类型不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 动作类型，决定由哪个动作执行器处理
    /// </summary>
    public string ActionType { get; init; } = string.Empty;

    [Required(ErrorMessage = "执行器编码不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 执行器编码，用于在同一动作类型下区分具体公式或策略
    /// </summary>
    public string ExecutorCode { get; init; } = string.Empty;

    /// <summary>
    /// 扩展参数 JSON，用于承载动作或条件的可变配置
    /// </summary>
    public string? ParamsJson { get; init; }

    [MaxLength(50)]
    /// <summary>
    /// 互斥组编码，同组动作只应执行优先级最高的一条
    /// </summary>
    public string? ExclusiveGroup { get; init; }

    /// <summary>
    /// 排序号，用于控制展示顺序或同类动作内部顺序
    /// </summary>
    public int SortNo { get; init; }

    [MaxLength(20)]
    /// <summary>
    /// 动作异常处理策略，资金相关动作默认应 STOP
    /// </summary>
    public string OnError { get; init; } = "STOP";

    /// <summary>
    /// 启用标识，Y 表示参与查询或匹配
    /// </summary>
    public string IsEnabled { get; init; } = "Y";
}

/// <summary>
/// 规则动作集合保存请求 DTO。
/// </summary>
public sealed class RuleActionSaveRequest
{
    [Required(ErrorMessage = "动作列表不能为空")]
    /// <summary>
    /// 当前规则版本下的完整动作链，保存时会整体替换旧动作。
    /// </summary>
    public IReadOnlyList<RuleActionItemRequest> Actions { get; init; } = Array.Empty<RuleActionItemRequest>();
}
