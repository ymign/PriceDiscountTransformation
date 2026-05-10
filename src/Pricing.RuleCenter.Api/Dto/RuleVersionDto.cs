namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 规则版本响应 DTO，返回规则版本的状态、生效信息和发布快照。
/// </summary>
/// <remarks>
/// <para>
/// 规则版本（PR_RULE_VERSION 表）是规则体系的版本管理单元。每条规则头可有多个版本，
/// 版本通过 <see cref="VersionNo"/> 递增管理。同一规则同一时刻只有一个 PUBLISHED 状态的版本。
/// </para>
/// <para>
/// 版本的条件和动作明细通过各自接口读取（GET conditions / GET actions），
/// 此 DTO 仅返回版本级别的元数据。
/// </para>
/// <para>
/// <see cref="RuleSnapshot"/> 是发布时生成的完整规则快照（JSON 格式），包含规则头、
/// 所有条件和动作的完整配置。快照用于历史计价追溯：即使规则后续被修改或停用，
/// 仍可通过快照还原当时生效的规则配置。
/// </para>
/// <para>
/// 对应接口：GET <c>/api/rules/{ruleId}/versions</c> 版本列表、
/// GET <c>/api/rules/{ruleId}/versions/{versionNo}</c> 单条查询。
/// </para>
/// </remarks>
public sealed class RuleVersionResponse
{
    /// <summary>
    /// 规则版本主键，对应 PR_RULE_VERSION.VERSION_ID，由序列 PR_RULE_VERSION_SEQ 生成。
    /// </summary>
    public long VersionId { get; init; }

    /// <summary>
    /// 规则主键，关联 PR_RULE_HEADER.RULE_ID。
    /// </summary>
    public long RuleId { get; init; }

    /// <summary>
    /// 规则版本号，同一规则下从 1 开始递增。
    /// 版本号与 PR_RULE_CONDITION.VERSION_NO 和 PR_RULE_ACTION.VERSION_NO 对齐，
    /// 确保同一版本的条件和动作能正确关联。
    /// </summary>
    public int VersionNo { get; init; }

    /// <summary>
    /// 版本状态，描述版本在生命周期中的位置。
    /// 常见值：DRAFT（草稿，可编辑条件和动作）、PUBLISHED（已发布，当前生效版本）、
    /// DISABLED（已停用，规则整体停用时关联版本置为此状态）、
    /// ROLLED_BACK（已回滚，被回滚操作替换的版本）。
    /// </summary>
    public string VersionStatus { get; init; } = string.Empty;

    /// <summary>
    /// 版本生效开始时间。为 null 表示不限制生效起始时间。
    /// 与规则头的 EffectiveFrom 配合，计价引擎取两者中较晚的时间作为实际生效起始。
    /// </summary>
    public DateTime? EffectiveFrom { get; init; }

    /// <summary>
    /// 版本生效结束时间。为 null 表示未设失效时间（永久生效）。
    /// 与规则头的 EffectiveTo 配合，计价引擎取两者中较早的时间作为实际生效截止。
    /// </summary>
    public DateTime? EffectiveTo { get; init; }

    /// <summary>
    /// 规则发布时的完整快照 JSON，在版本状态变为 PUBLISHED 时由系统自动生成。
    /// 快照包含规则头、所有条件和动作的完整配置，格式为序列化后的 JSON 字符串。
    /// 用于历史计价追溯：即使规则后续被修改或停用，仍可通过快照还原当时生效的配置。
    /// DRAFT 状态的版本此字段为 null。
    /// </summary>
    public string? RuleSnapshot { get; init; }

    /// <summary>
    /// 最后一次生命周期操作的操作人（发布人、停用人或回滚人）。
    /// DRAFT 状态的版本此字段为 null。
    /// </summary>
    public string? PublishedBy { get; init; }

    /// <summary>
    /// 最后一次生命周期操作的发生时间。
    /// DRAFT 状态的版本此字段为 null。
    /// </summary>
    public DateTime? PublishedAt { get; init; }

    /// <summary>
    /// 发布说明或审批备注，记录发布原因、审批意见或变更说明。
    /// 用于审计追溯和问题排查。
    /// </summary>
    public string? PublishRemark { get; init; }
}
