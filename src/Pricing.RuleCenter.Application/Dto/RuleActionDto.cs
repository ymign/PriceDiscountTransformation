using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 规则动作响应 DTO，返回规则条件满足后要执行的动作配置。
/// </summary>
/// <remarks>
/// <para>
/// 规则动作（PR_RULE_ACTION 表）是"条件-动作"模型的执行侧。当规则条件全部满足后，
/// 计价引擎按 <see cref="SortNo"/> 顺序依次执行该版本下的所有动作。
/// 每个动作通过 <see cref="ActionType"/> + <see cref="ExecutorCode"/> 路由到对应的
/// <c>IRuleActionExecutor</c> 实现，如金额折价、数量限制、公式计价、子项加收等。
/// </para>
/// <para>
/// 同一规则版本下的动作构成一条有序的动作链。链内动作支持通过
/// <see cref="ExclusiveGroup"/> 实现互斥：同组动作只执行排序最前（SortNo 最小）的一条。
/// </para>
/// <para>
/// 对应接口：GET <c>/api/rules/{ruleId}/versions/{versionNo}/actions</c>。
/// </para>
/// </remarks>
public sealed class RuleActionResponse
{
    /// <summary>
    /// 规则动作主键，对应 PR_RULE_ACTION.ACTION_ID，由序列 PR_RULE_ACTION_SEQ 生成。
    /// </summary>
    [JsonPropertyName("action_id")]
    public long ActionId { get; init; }

    /// <summary>
    /// 规则主键，关联 PR_RULE_HEADER.RULE_ID，用于将动作与规则头、版本、条件、追溯结果串联。
    /// </summary>
    [JsonPropertyName("rule_id")]
    public long RuleId { get; init; }

    /// <summary>
    /// 规则版本号，与 PR_RULE_VERSION.VERSION_NO 对齐。
    /// 同一规则下不同版本的动作相互独立，发布时以版本为单位快照。
    /// </summary>
    [JsonPropertyName("version_no")]
    public int VersionNo { get; init; }

    /// <summary>
    /// 动作类型，决定由哪个动作执行器处理。
    /// 值来自内置字典 ACTION_TYPE 域，常见值：DISCOUNT_AMOUNT（金额折价）、
    /// LIMIT_QTY（数量限制）、FORMULA（公式计价）、ADD_ITEM（子项加收）等。
    /// </summary>
    [JsonPropertyName("action_type")]
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// 执行器编码，与 <see cref="ActionType"/> 配合路由到具体的执行器实现。
    /// 同一 ActionType 下可有多个执行器，通过此编码区分不同计算策略。
    /// 编码必须与后端已注册的 <c>IRuleActionExecutor</c> 实现匹配。
    /// </summary>
    [JsonPropertyName("executor_code")]
    public string ExecutorCode { get; init; } = string.Empty;

    /// <summary>
    /// 扩展参数 JSON，承载该动作的可变配置，由执行器自行解析。
    /// 例如金额折价动作的参数可能包含 { "discountRate": 0.8, "minAmount": 10.00 }；
    /// 数量限制动作的参数可能包含 { "dailyLimit": 3, "timeWindowMinutes": 120 }。
    /// 为 null 表示该动作不需要额外参数。
    /// </summary>
    [JsonPropertyName("params_json")]
    public string? ParamsJson { get; init; }

    /// <summary>
    /// 互斥组编码。同一 <see cref="ExclusiveGroup"/> 下的多个动作互斥，
    /// 计价引擎只执行 SortNo 最小（优先级最高）的那一条。
    /// 为 null 表示该动作不参与互斥，始终执行。
    /// 典型场景：同一项目配置了多种折价方案（如按比例折和按金额折），运行时只取一种。
    /// </summary>
    [JsonPropertyName("exclusive_group")]
    public string? ExclusiveGroup { get; init; }

    /// <summary>
    /// 排序号，控制同一规则版本下动作的执行顺序和互斥组内的优先级。
    /// 值越小越先执行。默认值 0。
    /// </summary>
    [JsonPropertyName("sort_no")]
    public int SortNo { get; init; }

    /// <summary>
    /// 动作异常处理策略。"STOP" 表示该动作执行失败时终止整个规则链并向调用方报错；
    /// "CONTINUE" 表示跳过该动作继续执行后续动作。
    /// 资金相关动作（如金额折价、限额扣减）必须为 "STOP"，防止异常状态下继续计价导致资损。
    /// </summary>
    [JsonPropertyName("on_error")]
    public string OnError { get; init; } = "STOP";

    /// <summary>
    /// 启用标识。"Y" 表示该动作参与规则执行；"N" 表示已停用，执行时跳过。
    /// 停用动作不会从版本中删除，便于后续重新启用。
    /// </summary>
    [JsonPropertyName("is_enabled")]
    public string IsEnabled { get; init; } = "Y";
}

/// <summary>
/// 单条规则动作保存项 DTO，用于批量保存请求中的单个动作条目。
/// </summary>
/// <remarks>
/// <para>
/// 保存请求以"整体替换"模式工作：提交的动作列表会完全覆盖该版本下的旧动作。
/// 因此每次保存必须包含该版本下所有需要保留的动作，遗漏的动作会被删除。
/// </para>
/// </remarks>
public sealed class RuleActionItemRequest
{
    /// <summary>
    /// 动作类型（必填），决定由哪个动作执行器处理。
    /// 值来自内置字典 ACTION_TYPE 域，必须是系统已注册的有效类型。
    /// </summary>
    [Required(ErrorMessage = "动作类型不能为空")]
    [MaxLength(50)]
    [JsonPropertyName("action_type")]
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// 执行器编码（必填），与 ActionType 配合路由到具体执行器实现。
    /// </summary>
    [Required(ErrorMessage = "执行器编码不能为空")]
    [MaxLength(50)]
    [JsonPropertyName("executor_code")]
    public string ExecutorCode { get; init; } = string.Empty;

    /// <summary>
    /// 扩展参数 JSON（选填），由执行器定义参数结构。
    /// 前端工作台根据公式定义的 ParamSchemaJson 渲染配置表单，提交时序列化为此字段。
    /// </summary>
    [JsonPropertyName("params_json")]
    public string? ParamsJson { get; init; }

    /// <summary>
    /// 互斥组编码（选填）。同组动作互斥，只执行 SortNo 最小的一条。
    /// 为 null 或空表示不参与互斥。
    /// </summary>
    [MaxLength(50)]
    [JsonPropertyName("exclusive_group")]
    public string? ExclusiveGroup { get; init; }

    /// <summary>
    /// 排序号（选填），控制执行顺序和互斥组内优先级。默认值 0，值越小越先执行。
    /// </summary>
    [JsonPropertyName("sort_no")]
    public int SortNo { get; init; }

    /// <summary>
    /// 动作异常处理策略（选填），默认 "STOP"。
    /// "STOP"：失败终止；"CONTINUE"：失败跳过。资金相关动作必须为 "STOP"。
    /// </summary>
    [MaxLength(20)]
    [JsonPropertyName("on_error")]
    public string OnError { get; init; } = "STOP";

    /// <summary>
    /// 启用标识（选填），默认 "Y"。设为 "N" 可临时停用该动作而不删除。
    /// </summary>
    [JsonPropertyName("is_enabled")]
    public string IsEnabled { get; init; } = "Y";
}

/// <summary>
/// 规则动作集合保存请求 DTO，用于整体替换某规则版本下的全部动作。
/// </summary>
/// <remarks>
/// <para>
/// 采用"整体替换"语义：提交的 <see cref="Actions"/> 列表会完全覆盖该版本下的旧动作。
/// 这是为了保证动作链的完整性和顺序一致性，避免部分更新导致动作链断裂或重复。
/// </para>
/// <para>
/// 保存前系统会校验：动作类型和执行器编码是否有效、互斥组内是否有重复、
/// 资金相关动作的 OnError 是否为 STOP 等。
/// </para>
/// <para>
/// 对应接口：PUT <c>/api/rules/{ruleId}/versions/{versionNo}/actions</c>。
/// </para>
/// </remarks>
public sealed class RuleActionSaveRequest
{
    /// <summary>
    /// 当前规则版本下的完整动作链（必填），保存时整体替换旧动作。
    /// 列表中的每个元素对应一条动作配置，按 SortNo 排序后构成有序执行链。
    /// 空列表会导致该版本下所有动作被清除。
    /// </summary>
    [Required(ErrorMessage = "动作列表不能为空")]
    [JsonPropertyName("actions")]
    public IReadOnlyList<RuleActionItemRequest> Actions { get; init; } = Array.Empty<RuleActionItemRequest>();
}

