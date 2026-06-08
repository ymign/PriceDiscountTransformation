using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 规则主档响应 DTO，返回规则头（Rule Header）的完整信息。
/// </summary>
/// <remarks>
/// <para>
/// 规则主档（PR_RULE_HEADER 表）是规则体系的顶层实体，每条规则头代表一条独立的计价规则。
/// 规则头承载规则的业务属性（编码、名称、类别、范围、关联项目）和生命周期状态，
/// 而规则的具体条件和动作分别由 PR_RULE_CONDITION 和 PR_RULE_ACTION 通过 RuleId 关联。
/// </para>
/// <para>
/// 规则的版本管理通过 PR_RULE_VERSION 实现，<see cref="CurrentVersion"/> 指向当前生效的版本号。
/// 发布新版本或回滚时，由规则生命周期服务自动维护此字段。
/// </para>
/// <para>
/// 对应接口：GET <c>/api/rules</c> 分页列表、GET <c>/api/rules/{ruleId}</c> 单条查询。
/// </para>
/// </remarks>
public sealed class RuleHeaderResponse
{
    /// <summary>
    /// 规则主键，对应 PR_RULE_HEADER.RULE_ID，由序列 PR_RULE_HEADER_SEQ 生成。
    /// 是关联规则头、版本、条件、动作和追溯结果的核心外键。
    /// </summary>
    [JsonPropertyName("rule_id")]
    public long RuleId { get; init; }

    /// <summary>
    /// 规则编码，全局唯一的稳定业务键，用于业务配置和运维识别。
    /// 例如 "RULE_SKIN_AREA_DISCOUNT"（皮肤科面积折价规则）。
    /// 编码创建后不可修改，被规则版本、条件、动作和追溯日志引用。
    /// </summary>
    [JsonPropertyName("rule_code")]
    public string RuleCode { get; init; } = string.Empty;

    /// <summary>
    /// 规则名称，面向配置人员的可读文本，用于工作台展示和审计报告。
    /// 例如 "皮肤科多肿物面积折价规则"。可修改，不影响规则匹配。
    /// </summary>
    [JsonPropertyName("rule_name")]
    public string RuleName { get; init; } = string.Empty;

    /// <summary>
    /// 规则类别，用于工作台分类展示和筛选。
    /// 值来自内置字典 RULE_CATEGORY 域，常见值：DISCOUNT（折价规则）、FORMULA（公式规则）、
    /// LIMIT（限额规则）、MIXED（混合规则，同时包含折价和限额）。
    /// </summary>
    [JsonPropertyName("rule_category")]
    public string RuleCategory { get; init; } = string.Empty;

    /// <summary>
    /// 规则作用范围，决定规则匹配的粒度。
    /// 常见值：ITEM（单项目，通过 ItemCode 精确匹配）、GROUP（项目组，通过 GroupCode 匹配）、
    /// SCENE（场景级，匹配特定收费场景下的所有项目）。
    /// </summary>
    [JsonPropertyName("rule_scope")]
    public string RuleScope { get; init; } = string.Empty;

    /// <summary>
    /// 项目编码，当 <see cref="RuleScope"/> 为 ITEM 时必填。
    /// 是规则匹配、权威单价校验和限额累计的核心维度。
    /// 编码对应 HIS 物价主数据中的项目编码。
    /// </summary>
    [JsonPropertyName("item_code")]
    public string? ItemCode { get; init; }

    /// <summary>
    /// 项目名称，仅用于工作台展示和审计追溯说明，不参与规则匹配。
    /// 从 HIS 物价主数据同步，便于配置人员在工作台中识别项目。
    /// </summary>
    [JsonPropertyName("item_name")]
    public string? ItemName { get; init; }

    /// <summary>
    /// 项目组编码，当 <see cref="RuleScope"/> 为 GROUP 时必填。
    /// 对应 PR_ITEM_GROUP 表中的组编码，组内项目通过 PR_ITEM_GROUP_DETAIL 关联。
    /// </summary>
    [JsonPropertyName("group_code")]
    public string? GroupCode { get; init; }

    /// <summary>
    /// 规则优先级，数字越小越先参与匹配。当同一项目有多条规则时，按 Priority 升序匹配，
    /// 第一条条件全部满足的规则生效。默认值 100，建议按 10 的倍数递增便于插入。
    /// </summary>
    [JsonPropertyName("priority")]
    public int Priority { get; init; }

    /// <summary>
    /// 当前生效版本号，指向 PR_RULE_VERSION 中状态为 PUBLISHED 的版本。
    /// 发布新版本或回滚时由规则生命周期服务自动更新。
    /// 为 0 表示该规则尚未发布任何版本。
    /// </summary>
    [JsonPropertyName("current_version")]
    public int CurrentVersion { get; init; }

    /// <summary>
    /// 规则状态，描述规则在生命周期中的位置。
    /// 常见值：DRAFT（草稿，可编辑）、PUBLISHED（已发布，有生效版本）、
    /// DISABLED（已停用，所有版本失效）、ARCHIVED（已归档，不可操作）。
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// 启用标识。"Y" 表示该规则参与计价匹配；"N" 表示已停用，计价时跳过。
    /// 停用与状态 DISABLED 的区别：停用是软开关，可随时恢复；DISABLED 是生命周期状态。
    /// </summary>
    [JsonPropertyName("is_enabled")]
    public string IsEnabled { get; init; } = "Y";

    /// <summary>
    /// 规则生效开始时间。为 null 表示不限制生效起始时间。
    /// 计价引擎在匹配时会校验业务时间是否在 [EffectiveFrom, EffectiveTo] 区间内。
    /// </summary>
    [JsonPropertyName("effective_from")]
    public DateTime? EffectiveFrom { get; init; }

    /// <summary>
    /// 规则生效结束时间。为 null 表示未设失效时间（永久生效）。
    /// 计价引擎在匹配时会校验业务时间是否在 [EffectiveFrom, EffectiveTo] 区间内。
    /// </summary>
    [JsonPropertyName("effective_to")]
    public DateTime? EffectiveTo { get; init; }

    /// <summary>
    /// 回滚模式，决定计价服务不可用时的降级策略。
    /// 常见值：STOP_CHARGE（暂停收费转人工，默认最安全）、
    /// LEGACY_EQUIVALENT（自动切回旧计价逻辑，需审批）、
    /// MANUAL_REVIEW（继续使用新服务但标记需人工复核）。
    /// 为空时等价于 STOP_CHARGE。
    /// </summary>
    [JsonPropertyName("rollback_mode")]
    public string? RollbackMode { get; init; }

    /// <summary>
    /// 规则备注，用于记录业务背景、变更原因、待确认事项等维护信息。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; init; }

    /// <summary>
    /// 创建人，记录首次创建该规则的操作人员标识（如工号或用户名）。
    /// </summary>
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; init; }

    /// <summary>
    /// 记录创建时间，由数据库在 INSERT 时自动填充。
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// 最后修改人，记录最近一次修改该规则头信息的操作人员标识。
    /// </summary>
    [JsonPropertyName("updated_by")]
    public string? UpdatedBy { get; init; }

    /// <summary>
    /// 记录最后更新时间，由数据库在 UPDATE 时自动刷新。
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; init; }
}

/// <summary>
/// 规则主档新增请求 DTO，用于创建新的计价规则。
/// </summary>
/// <remarks>
/// <para>
/// 新增规则后默认状态为 DRAFT（草稿），需要后续创建版本、配置条件和动作、发布后才能生效。
/// 系统会校验 <see cref="RuleCode"/> 全局唯一性，重复则返回 409 冲突错误。
/// </para>
/// <para>
/// 对应接口：POST <c>/api/rules</c>。
/// </para>
/// </remarks>
public sealed class RuleHeaderCreateRequest
{
    /// <summary>
    /// 规则编码（必填），全局唯一的稳定业务键。
    /// 编码规则：大写字母、数字和下划线，如 RULE_SKIN_AREA_DISCOUNT。创建后不可修改。
    /// </summary>
    [Required(ErrorMessage = "规则编码不能为空")]
    [MaxLength(50)]
    [JsonPropertyName("rule_code")]
    public string RuleCode { get; init; } = string.Empty;

    /// <summary>
    /// 规则名称（必填），面向配置人员的可读文本。
    /// </summary>
    [Required(ErrorMessage = "规则名称不能为空")]
    [MaxLength(200)]
    [JsonPropertyName("rule_name")]
    public string RuleName { get; init; } = string.Empty;

    /// <summary>
    /// 规则类别（选填），默认 "MIXED"（混合规则）。
    /// 值来自内置字典 RULE_CATEGORY 域。
    /// </summary>
    [MaxLength(20)]
    [JsonPropertyName("rule_category")]
    public string RuleCategory { get; init; } = "MIXED";

    /// <summary>
    /// 规则作用范围（选填），默认 "ITEM"（单项目）。
    /// 值来自内置字典 RULE_SCOPE 域：ITEM（单项目）、GROUP（项目组）、SCENE（场景）。
    /// </summary>
    [MaxLength(20)]
    [JsonPropertyName("rule_scope")]
    public string RuleScope { get; init; } = "ITEM";

    /// <summary>
    /// 项目编码（选填），当 RuleScope 为 ITEM 时必填。
    /// 对应 HIS 物价主数据中的项目编码。
    /// </summary>
    [MaxLength(50)]
    [JsonPropertyName("item_code")]
    public string? ItemCode { get; init; }

    /// <summary>
    /// 项目名称（选填），仅用于展示，不参与匹配。建议与 ItemCode 一并填写。
    /// </summary>
    [MaxLength(200)]
    [JsonPropertyName("item_name")]
    public string? ItemName { get; init; }

    /// <summary>
    /// 项目组编码（选填），当 RuleScope 为 GROUP 时必填。
    /// 对应 PR_ITEM_GROUP 表中的组编码。
    /// </summary>
    [MaxLength(50)]
    [JsonPropertyName("group_code")]
    public string? GroupCode { get; init; }

    /// <summary>
    /// 规则优先级（选填），默认 100。数字越小越先参与匹配。
    /// 建议按 10 的倍数递增，便于在两条规则之间插入新规则。
    /// </summary>
    [JsonPropertyName("priority")]
    public int Priority { get; init; } = 100;

    /// <summary>
    /// 规则生效开始时间（选填）。为 null 表示不限制生效起始时间。
    /// </summary>
    [JsonPropertyName("effective_from")]
    public DateTime? EffectiveFrom { get; init; }

    /// <summary>
    /// 规则生效结束时间（选填）。为 null 表示永久生效。
    /// </summary>
    [JsonPropertyName("effective_to")]
    public DateTime? EffectiveTo { get; init; }

    /// <summary>
    /// 回滚模式（选填），决定计价服务不可用时的降级策略。
    /// 可选值：STOP_CHARGE（暂停收费转人工，默认）、LEGACY_EQUIVALENT（自动切回旧逻辑，需审批）、
    /// MANUAL_REVIEW（继续使用新服务但标记需人工复核）。为空时等价于 STOP_CHARGE。
    /// </summary>
    [MaxLength(30)]
    [JsonPropertyName("rollback_mode")]
    public string? RollbackMode { get; init; }

    /// <summary>
    /// 规则备注（选填），用于记录业务背景或维护说明。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; init; }

    /// <summary>
    /// 创建人（选填），通常由系统从登录上下文自动填充。
    /// </summary>
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; init; }
}

/// <summary>
/// 规则主档更新请求 DTO，用于修改已有规则的头信息。
/// </summary>
/// <remarks>
/// <para>
/// 更新操作仅修改规则头的展示和业务属性，不影响已发布的版本。
/// <c>RuleCode</c> 不可修改（因为已被版本、条件、动作和追溯日志引用）。
/// 已发布的规则修改属性后需要重新发布版本才能使变更生效。
/// </para>
/// <para>
/// 对应接口：PUT <c>/api/rules/{ruleId}</c>。
/// </para>
/// </remarks>
public sealed class RuleHeaderUpdateRequest
{
    /// <summary>
    /// 规则名称（必填），面向配置人员的可读文本。
    /// </summary>
    [Required(ErrorMessage = "规则名称不能为空")]
    [MaxLength(200)]
    [JsonPropertyName("rule_name")]
    public string RuleName { get; init; } = string.Empty;

    /// <summary>
    /// 规则类别（选填），默认 "MIXED"。
    /// </summary>
    [MaxLength(20)]
    [JsonPropertyName("rule_category")]
    public string RuleCategory { get; init; } = "MIXED";

    /// <summary>
    /// 规则作用范围（选填），默认 "ITEM"。
    /// </summary>
    [MaxLength(20)]
    [JsonPropertyName("rule_scope")]
    public string RuleScope { get; init; } = "ITEM";

    /// <summary>
    /// 项目编码（选填），当 RuleScope 为 ITEM 时必填。
    /// </summary>
    [MaxLength(50)]
    [JsonPropertyName("item_code")]
    public string? ItemCode { get; init; }

    /// <summary>
    /// 项目名称（选填），仅用于展示。
    /// </summary>
    [MaxLength(200)]
    [JsonPropertyName("item_name")]
    public string? ItemName { get; init; }

    /// <summary>
    /// 项目组编码（选填），当 RuleScope 为 GROUP 时必填。
    /// </summary>
    [MaxLength(50)]
    [JsonPropertyName("group_code")]
    public string? GroupCode { get; init; }

    /// <summary>
    /// 规则优先级（选填），默认 100。
    /// </summary>
    [JsonPropertyName("priority")]
    public int Priority { get; init; } = 100;

    /// <summary>
    /// 规则生效开始时间（选填）。为 null 表示不限制生效起始时间。
    /// </summary>
    [JsonPropertyName("effective_from")]
    public DateTime? EffectiveFrom { get; init; }

    /// <summary>
    /// 规则生效结束时间（选填）。为 null 表示永久生效。
    /// </summary>
    [JsonPropertyName("effective_to")]
    public DateTime? EffectiveTo { get; init; }

    /// <summary>
    /// 回滚模式（选填），决定计价服务不可用时的降级策略。
    /// 可选值：STOP_CHARGE（暂停收费转人工，默认）、LEGACY_EQUIVALENT（自动切回旧逻辑，需审批）、
    /// MANUAL_REVIEW（继续使用新服务但标记需人工复核）。为空时等价于 STOP_CHARGE。
    /// </summary>
    [MaxLength(30)]
    [JsonPropertyName("rollback_mode")]
    public string? RollbackMode { get; init; }

    /// <summary>
    /// 规则备注（选填）。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; init; }

    /// <summary>
    /// 最后修改人（选填），通常由系统从登录上下文自动填充。
    /// </summary>
    [JsonPropertyName("updated_by")]
    public string? UpdatedBy { get; init; }
}

/// <summary>
/// 规则主档分页查询请求 DTO，用于工作台规则列表的筛选和分页。
/// </summary>
/// <remarks>
/// <para>
/// 所有筛选条件均为选填，为空时不过滤。多个条件之间按 AND 逻辑组合。
/// </para>
/// <para>
/// 对应接口：GET <c>/api/rules</c>。
/// </para>
/// </remarks>
public sealed class RuleHeaderPagedRequest
{
    /// <summary>
    /// 项目编码筛选条件（选填）。精确匹配，用于查找某项目关联的所有规则。
    /// </summary>
    [JsonPropertyName("item_code")]
    public string? ItemCode { get; init; }

    /// <summary>
    /// 规则状态筛选条件（选填）。精确匹配，如 DRAFT、PUBLISHED、DISABLED。
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// 规则分类筛选条件（选填）。精确匹配，如 DISCOUNT、FORMULA、LIMIT、MIXED。
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; init; }

    /// <summary>
    /// 页码（选填），从 1 开始，默认 1。
    /// </summary>
    [JsonPropertyName("page_index")]
    public int PageIndex { get; init; } = 1;

    /// <summary>
    /// 每页记录数（选填），默认 20。建议不超过 100 以保证查询性能。
    /// </summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// 通用分页响应 DTO，封装分页查询结果的元数据和数据集。
/// </summary>
/// <remarks>
/// <para>
/// 所有分页查询接口统一返回此结构。<see cref="Total"/> 用于前端计算总页数，
/// <see cref="PageIndex"/> 和 <see cref="PageSize"/> 回显当前分页参数。
/// </para>
/// </remarks>
/// <typeparam name="T">分页数据项的类型，由各接口自行定义。</typeparam>
public sealed class PagedResponse<T>
{
    /// <summary>
    /// 当前页数据集合。类型 T 由各接口定义，如 <see cref="RuleHeaderResponse"/>、
    /// <see cref="RulePublishResponse"/> 等。
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// 符合筛选条件的总记录数，用于前端计算总页数。
    /// 由数据库 COUNT 查询得出，与当前页数据无关。
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; init; }

    /// <summary>
    /// 当前页码，从 1 开始。回显请求中的 PageIndex 参数。
    /// </summary>
    [JsonPropertyName("page_index")]
    public int PageIndex { get; init; }

    /// <summary>
    /// 每页记录数。回显请求中的 PageSize 参数。
    /// </summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; init; }
}

