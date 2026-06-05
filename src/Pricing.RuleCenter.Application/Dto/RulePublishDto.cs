using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 规则发布流水响应 DTO，返回规则发布/停用/回滚的操作记录。
/// </summary>
/// <remarks>
/// <para>
/// 每次规则生命周期变更（发布、停用、回滚）都会在 PR_RULE_PUBLISH 表生成一条流水记录，
/// 用于审计追踪和问题排查。<see cref="FromVersion"/> 和 <see cref="ToVersion"/> 记录了
/// 版本号的变更轨迹，便于回溯规则的演进历史。
/// </para>
/// <para>
/// <see cref="PublishNo"/> 是每次操作的唯一流水号，可与变更日志（PR_RULE_CHANGE_LOG）
/// 和计价追溯日志（PR_CHARGE_REQUEST_LOG）交叉定位。
/// </para>
/// <para>
/// 对应接口：GET <c>/api/rules/{ruleId}/publishes</c> 发布历史查询。
/// </para>
/// </remarks>
public sealed class RulePublishResponse
{
    /// <summary>
    /// 发布流水主键，对应 PR_RULE_PUBLISH.PUBLISH_ID，由序列 PR_RULE_PUBLISH_SEQ 生成。
    /// </summary>
    public long PublishId { get; init; }

    /// <summary>
    /// 发布流水号，每次发布/停用/回滚操作生成的唯一业务编号。
    /// 格式建议：PUB + 日期 + 序号，如 PUB20260510001。用于审计日志和变更追溯的交叉定位。
    /// </summary>
    public string PublishNo { get; init; } = string.Empty;

    /// <summary>
    /// 规则主键，关联 PR_RULE_HEADER.RULE_ID。
    /// </summary>
    public long RuleId { get; init; }

    /// <summary>
    /// 操作前版本号。首次发布时为 null（无历史版本）；回滚时为被回滚的版本号。
    /// </summary>
    public int? FromVersion { get; init; }

    /// <summary>
    /// 操作后版本号。发布时为新发布的版本号；回滚时为回滚目标版本号。
    /// 停用操作时此字段可能为 0 或当前生效版本号，具体取决于实现。
    /// </summary>
    public int ToVersion { get; init; }

    /// <summary>
    /// 操作类型，描述本次生命周期变更的动作。
    /// 常见值：PUBLISH（发布新版本）、DISABLE（停用规则）、ROLLBACK（回滚到历史版本）。
    /// 值来自内置字典 PUBLISH_ACTION 域。
    /// </summary>
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// 操作人，记录执行发布/停用/回滚的操作人员标识（如工号或用户名）。
    /// </summary>
    public string? PublishedBy { get; init; }

    /// <summary>
    /// 操作发生时间，由数据库在 INSERT 时自动填充。
    /// </summary>
    public DateTime PublishedAt { get; init; }

    /// <summary>
    /// 操作备注，记录发布原因、停用原因或回滚原因等维护信息。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 规则发布请求 DTO，用于将指定版本发布为生效版本。
/// </summary>
/// <remarks>
/// <para>
/// 发布操作会执行以下步骤：校验版本状态为 DRAFT → 校验规则重叠和动作组冲突 →
/// 生成规则快照（RULE_SNAPSHOT）→ 更新版本状态为 PUBLISHED → 更新规则头 CurrentVersion →
/// 写入发布流水 → 失效规则缓存。
/// </para>
/// <para>
/// 发布前的阻断项校验包括：规则重叠（同项目同场景同时段多条生效规则）、
/// 动作组冲突、重复子项目、缺少测试用例等。
/// </para>
/// <para>
/// 对应接口：POST <c>/api/rules/{ruleId}/publish</c>。
/// </para>
/// </remarks>
public sealed class RulePublishRequest
{
    [Required(ErrorMessage = "版本号不能为空")]
    /// <summary>
    /// 要发布的规则版本号（必填），必须对应一个 DRAFT 状态的版本。
    /// 发布后该版本状态变为 PUBLISHED，规则头的 CurrentVersion 更新为此值。
    /// </summary>
    public int VersionNo { get; init; }

    /// <summary>
    /// 操作人（选填），通常由系统从登录上下文自动填充。
    /// </summary>
    public string? PublishedBy { get; init; }

    /// <summary>
    /// 发布备注（选填），记录发布原因或说明。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 规则停用请求 DTO，用于将已发布规则置为停用状态。
/// </summary>
/// <remarks>
/// <para>
/// 停用操作会使当前生效版本失效，规则不再参与计价匹配。
/// 停用后规则状态变为 DISABLED，规则头 CurrentVersion 重置为 0。
/// 停用不会删除规则和版本数据，可随时通过重新发布恢复。
/// </para>
/// <para>
/// 停用后必须立即失效规则缓存，确保各渠道不再使用旧规则计价。
/// </para>
/// <para>
/// 对应接口：POST <c>/api/rules/{ruleId}/disable</c>。
/// </para>
/// </remarks>
public sealed class RuleDisableRequest
{
    /// <summary>
    /// 操作人（选填），通常由系统从登录上下文自动填充。
    /// </summary>
    public string? PublishedBy { get; init; }

    /// <summary>
    /// 停用备注（选填），记录停用原因。建议必填，便于后续审计。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 规则回滚请求 DTO，用于将规则回滚到上一个已发布版本。
/// </summary>
/// <remarks>
/// <para>
/// 回滚操作会将当前 PUBLISHED 版本状态置为 ROLLED_BACK，并将上一个 PUBLISHED 版本
/// 重新激活为当前生效版本。规则头的 CurrentVersion 更新为回滚目标版本号。
/// </para>
/// <para>
/// 回滚是紧急操作，通常在发现规则配置错误导致计价异常时使用。
/// 回滚后必须立即失效规则缓存，确保各渠道立即使用正确的规则。
/// </para>
/// <para>
/// 对应接口：POST <c>/api/rules/{ruleId}/rollback</c>。
/// </para>
/// </remarks>
public sealed class RuleRollbackRequest
{
    /// <summary>
    /// 操作人（选填），通常由系统从登录上下文自动填充。
    /// </summary>
    public string? PublishedBy { get; init; }

    /// <summary>
    /// 回滚备注（选填），记录回滚原因。建议必填，便于后续审计。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 规则变更日志响应 DTO，返回规则配置变更的审计记录。
/// </summary>
/// <remarks>
/// <para>
/// 变更日志（PR_RULE_CHANGE_LOG 表）记录了规则的所有配置变更，包括条件修改、
/// 动作修改、属性修改等。与发布流水（PR_RULE_PUBLISH）的区别在于：
/// 发布流水记录生命周期操作（发布/停用/回滚），变更日志记录配置内容的变更。
/// </para>
/// <para>
/// <see cref="ChangeSummary"/> 面向配置人员展示，以自然语言描述变更内容，
/// 如"新增条件：部位=面部；修改动作：折价比例从 0.8 改为 0.7"。
/// </para>
/// <para>
/// 对应接口：GET <c>/api/rules/{ruleId}/change-logs</c>。
/// </para>
/// </remarks>
public sealed class RuleChangeLogResponse
{
    /// <summary>
    /// 变更日志主键，对应 PR_RULE_CHANGE_LOG.CHANGE_ID，由序列 PR_RULE_CHANGE_LOG_SEQ 生成。
    /// </summary>
    public long ChangeId { get; init; }

    /// <summary>
    /// 规则主键，关联 PR_RULE_HEADER.RULE_ID。
    /// </summary>
    public long RuleId { get; init; }

    /// <summary>
    /// 变更涉及的规则版本号。为 null 表示变更发生在规则头级别（如修改规则名称）。
    /// </summary>
    public int? VersionNo { get; init; }

    /// <summary>
    /// 变更类型，描述变更的性质。
    /// 常见值：PUBLISH（发布）、DISABLE（停用）、ROLLBACK（回滚）、
    /// CONDITION_UPDATE（条件变更）、ACTION_UPDATE（动作变更）、HEADER_UPDATE（头信息变更）。
    /// </summary>
    public string ChangeType { get; init; } = string.Empty;

    /// <summary>
    /// 变更摘要，面向配置人员的自然语言描述。
    /// 记录变更的具体内容，如"新增条件：部位=面部""修改动作：折价比例从 0.8 改为 0.7"。
    /// </summary>
    public string? ChangeSummary { get; init; }

    /// <summary>
    /// 变更操作人，记录执行变更的操作人员标识。
    /// </summary>
    public string? ChangedBy { get; init; }

    /// <summary>
    /// 变更发生时间。
    /// </summary>
    public DateTime ChangedAt { get; init; }

    /// <summary>
    /// 来源系统编码，标识变更由哪个系统发起。
    /// 常见值：HIS（医院信息系统）、SELF_SERVICE（自助机）、WECHAT（微信公众号）、
    /// ADMIN（规则管理工作台）。用于多渠道变更溯源。
    /// </summary>
    public string? SourceSystem { get; init; }
}

