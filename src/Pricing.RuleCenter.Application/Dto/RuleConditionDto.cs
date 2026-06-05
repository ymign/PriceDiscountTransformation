using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 规则条件响应 DTO，返回规则匹配所需的条件配置。
/// </summary>
/// <remarks>
/// <para>
/// 规则条件（PR_RULE_CONDITION 表）是"条件-动作"模型的匹配侧。计价引擎在执行前，
/// 按 <see cref="ConditionGroup"/> 分组评估所有条件：同一 ConditionGroup 内的条件按 AND
/// 逻辑（全部满足），不同 ConditionGroup 之间按 OR 逻辑（任一组满足即可）。
/// </para>
/// <para>
/// 每个条件通过 <see cref="ConditionType"/> 路由到对应的 <c>IRuleConditionEvaluator</c> 实现，
/// 评估器从计价请求上下文中提取 <see cref="LeftKey"/> 对应的值，与 <see cref="RightValue"/>
/// 按 <see cref="OperatorType"/> 进行比较。
/// </para>
/// <para>
/// 对应接口：GET <c>/api/rules/{ruleId}/versions/{versionNo}/conditions</c>。
/// </para>
/// </remarks>
public sealed class RuleConditionResponse
{
    /// <summary>
    /// 规则条件主键，对应 PR_RULE_CONDITION.CONDITION_ID，由序列 PR_RULE_CONDITION_SEQ 生成。
    /// </summary>
    public long ConditionId { get; init; }

    /// <summary>
    /// 规则主键，关联 PR_RULE_HEADER.RULE_ID，用于将条件与规则头、版本、动作、追溯结果串联。
    /// </summary>
    public long RuleId { get; init; }

    /// <summary>
    /// 规则版本号，与 PR_RULE_VERSION.VERSION_NO 对齐。
    /// 同一规则下不同版本的条件相互独立，发布时以版本为单位快照。
    /// </summary>
    public int VersionNo { get; init; }

    /// <summary>
    /// 条件组编码。同一 ConditionGroup 内的条件按 AND 逻辑处理（全部满足才通过），
    /// 不同 ConditionGroup 之间按 OR 逻辑处理（任一组满足即通过）。
    /// 例如 "GROUP_1" 表示第一组条件，"GROUP_2" 表示第二组条件。
    /// </summary>
    public string ConditionGroup { get; init; } = string.Empty;

    /// <summary>
    /// 条件类型，决定由哪个条件评估器处理。
    /// 值来自内置字典 CONDITION_TYPE 域，常见值：ITEM_CODE（项目编码）、
    /// SCENE（收费场景）、BODY_PART（部位）、DEPARTMENT（科室）等。
    /// </summary>
    public string ConditionType { get; init; } = string.Empty;

    /// <summary>
    /// 比较运算符，定义左值与右值的比较方式。
    /// 常见值：EQ（等于）、IN（包含于，逗号分隔多值）、BETWEEN（区间）、
    /// GT（大于）、LT（小于）、LIKE（模糊匹配）。
    /// 为 null 时默认按 EQ 处理。
    /// </summary>
    public string? OperatorType { get; init; }

    /// <summary>
    /// 条件左值字段名，对应计价请求上下文中的结构化字段路径。
    /// 例如 "itemCode"（项目编码）、"scene"（收费场景）、"bodyPart"（部位编码）。
    /// 评估器运行时通过此字段名从上下文中提取实际值。
    /// </summary>
    public string? LeftKey { get; init; }

    /// <summary>
    /// 条件右值，来自规则配置的比较目标。
    /// 当 OperatorType 为 IN 时，RightValue 为逗号分隔的多值，如 "A,B,C"；
    /// 为 BETWEEN 时，RightValue 为 "~" 分隔的区间，如 "1~10"。
    /// 为 null 表示该条件类型不需要右值（如某些自定义评估器）。
    /// </summary>
    public string? RightValue { get; init; }

    /// <summary>
    /// 扩展参数 JSON，承载条件评估器的额外配置。
    /// 例如自定义评估器可能需要传入特殊阈值或业务规则参数。
    /// 为 null 表示该条件不需要额外参数。
    /// </summary>
    public string? ParamsJson { get; init; }

    /// <summary>
    /// 排序号，控制同一条件组内条件的评估顺序和展示顺序。值越小越先评估。
    /// </summary>
    public int SortNo { get; init; }

    /// <summary>
    /// 启用标识。"Y" 表示该条件参与规则匹配；"N" 表示已停用，评估时跳过。
    /// 停用条件不会从版本中删除，便于后续重新启用。
    /// </summary>
    public string IsEnabled { get; init; } = "Y";
}

/// <summary>
/// 单条规则条件保存项 DTO，用于批量保存请求中的单个条件条目。
/// </summary>
/// <remarks>
/// <para>
/// 保存请求以"整体替换"模式工作：提交的条件列表会完全覆盖该版本下的旧条件。
/// 因此每次保存必须包含该版本下所有需要保留的条件，遗漏的条件会被删除。
/// </para>
/// </remarks>
public sealed class RuleConditionItemRequest
{
    [Required(ErrorMessage = "条件组不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 条件组编码（必填），默认 "DEFAULT"。
    /// 同组条件按 AND 处理，不同组按 OR 处理。大多数简单规则只需一个条件组。
    /// </summary>
    public string ConditionGroup { get; init; } = "DEFAULT";

    [Required(ErrorMessage = "条件类型不能为空")]
    [MaxLength(50)]
    /// <summary>
    /// 条件类型（必填），决定由哪个条件评估器处理。
    /// 值来自内置字典 CONDITION_TYPE 域，必须是系统已注册的有效类型。
    /// </summary>
    public string ConditionType { get; init; } = string.Empty;

    [MaxLength(20)]
    /// <summary>
    /// 比较运算符（选填），默认 "EQ"（等于）。
    /// 支持的值取决于条件评估器实现，常见值：EQ、IN、BETWEEN、GT、LT、LIKE。
    /// </summary>
    public string? OperatorType { get; init; } = "EQ";

    [MaxLength(200)]
    /// <summary>
    /// 条件左值字段名（选填），对应计价请求上下文中的字段路径。
    /// 例如 "itemCode""scene""bodyPart"。部分条件类型（如自定义评估器）可能不需要此字段。
    /// </summary>
    public string? LeftKey { get; init; }

    [MaxLength(500)]
    /// <summary>
    /// 条件右值（选填），来自规则配置的比较目标。
    /// IN 运算符为逗号分隔多值，BETWEEN 为 "~" 分隔区间。
    /// </summary>
    public string? RightValue { get; init; }

    /// <summary>
    /// 扩展参数 JSON（选填），由条件评估器定义参数结构。
    /// </summary>
    public string? ParamsJson { get; init; }

    /// <summary>
    /// 排序号（选填），控制评估顺序。默认值 0，值越小越先评估。
    /// </summary>
    public int SortNo { get; init; }

    /// <summary>
    /// 启用标识（选填），默认 "Y"。设为 "N" 可临时停用该条件而不删除。
    /// </summary>
    public string IsEnabled { get; init; } = "Y";
}

/// <summary>
/// 规则条件集合保存请求 DTO，用于整体替换某规则版本下的全部条件。
/// </summary>
/// <remarks>
/// <para>
/// 采用"整体替换"语义：提交的 <see cref="Conditions"/> 列表会完全覆盖该版本下的旧条件。
/// 这是为了保证条件集合的完整性和分组一致性，避免部分更新导致条件逻辑断裂。
/// </para>
/// <para>
/// 保存前系统会校验：条件类型是否有效、ConditionGroup 是否合理、
/// LeftKey 和 RightValue 格式是否与运算符匹配等。
/// </para>
/// <para>
/// 对应接口：PUT <c>/api/rules/{ruleId}/versions/{versionNo}/conditions</c>。
/// </para>
/// </remarks>
public sealed class RuleConditionSaveRequest
{
    [Required(ErrorMessage = "条件列表不能为空")]
    /// <summary>
    /// 当前规则版本下的完整条件集合（必填），保存时整体替换旧条件。
    /// 列表中的每个元素对应一条条件配置，按 ConditionGroup 和 SortNo 组织评估逻辑。
    /// 空列表会导致该版本下所有条件被清除（等于无条件匹配所有请求）。
    /// </summary>
    public IReadOnlyList<RuleConditionItemRequest> Conditions { get; init; } = Array.Empty<RuleConditionItemRequest>();
}

