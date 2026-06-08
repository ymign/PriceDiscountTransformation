namespace Pricing.RuleCenter.Core.Aggregates.Rules;

/// <summary>
/// 规则版本实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_RULE_VERSION
/// </para>
/// <para>
/// 在规则体系中的角色：规则版本承载发布状态、生效期和发布快照。
/// 条件（PR_RULE_CONDITION）与动作（PR_RULE_ACTION）通过 RuleId + VersionNo 与该版本关联。
/// </para>
/// <para>
/// 版本管理机制：
/// <list type="bullet">
/// <item>规则头（PR_RULE_HEADER）的 CurrentVersion 指向当前生效版本</item>
/// <item>编辑规则时创建新版本（DRAFT），不影响已发布版本</item>
/// <item>发布时将新版本状态推进为 PUBLISHED，更新规则头的 CurrentVersion</item>
/// <item>回滚时恢复到指定版本，更新规则头的 CurrentVersion</item>
/// </list>
/// </para>
/// <para>
/// 版本一致性约束：规则条件和动作的 VERSION_NO 必须与规则版本的 VERSION_NO 对齐。
/// </para>
/// <para>
/// 发布快照：RULE_SNAPSHOT 存储发布时的完整规则状态（条件+动作），用于历史计价追溯。
/// 即使后续版本变更，已发出的计价结果仍可通过快照还原当时的计算依据。
/// </para>
/// </remarks>
public sealed class RuleVersion
{
    /// <summary>
    /// 规则版本主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 VERSION_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条规则版本记录。
    /// </remarks>
    public long VersionId { get; set; }

    /// <summary>
    /// 关联的规则主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_HEADER.RULE_ID，标识该版本属于哪条规则。
    /// 同一规则可以有多个版本（VERSION_NO 不同）。
    /// </remarks>
    public long RuleId { get; set; }

    /// <summary>
    /// 规则版本号。
    /// </summary>
    /// <remarks>
    /// 同一规则下从 1 开始递增，每次创建新版本自动 +1。
    /// 规则条件和动作的 VERSION_NO 必须与该值对齐。
    /// 规则头的 CurrentVersion 指向该值，决定计价时读取哪套版本明细。
    /// </remarks>
    public int VersionNo { get; set; }

    /// <summary>
    /// 版本状态。
    /// </summary>
    /// <remarks>
    /// 版本生命周期状态：
    /// <list type="bullet">
    /// <item>DRAFT — 草稿，正在编辑中，不影响计价</item>
    /// <item>PUBLISHED — 已发布，规则生效中</item>
    /// <item>DISABLED — 已停用，不再参与匹配</item>
    /// <item>ROLLED_BACK — 已回滚，被其他版本替代</item>
    /// </list>
    /// 默认值为 "DRAFT"。
    /// </remarks>
    public string VersionStatus { get; set; } = "DRAFT";

    /// <summary>
    /// 版本生效开始时间。
    /// </summary>
    /// <remarks>
    /// 该版本的生效开始时间，业务收费时间早于该时间的请求不使用该版本。
    /// 空值表示无生效开始时间限制。
    /// 规则生效期判断必须使用业务收费发生时间（BusinessChargeTime），
    /// 不得使用技术占用时间替代。
    /// </remarks>
    public DateTime? EffectiveFrom { get; set; }

    /// <summary>
    /// 版本生效结束时间。
    /// </summary>
    /// <remarks>
    /// 该版本的生效结束时间，业务收费时间晚于该时间的请求不使用该版本。
    /// 空值表示未设失效时间（长期有效）。
    /// </remarks>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// 规则发布时的完整快照 JSON。
    /// </summary>
    /// <remarks>
    /// 存储发布时该版本的完整状态（包括所有条件和动作），用于历史计价追溯。
    /// 即使后续版本变更，已发出的计价结果仍可通过快照还原当时的计算依据。
    /// Oracle 存储类型为 CLOB，无原生 JSON 支持。
    /// 仅在版本发布（PUBLISH）时生成，草稿版本该字段为空。
    /// </remarks>
    public string? RuleSnapshot { get; set; }

    /// <summary>
    /// 发布、停用或回滚操作人。
    /// </summary>
    /// <remarks>
    /// 来源为工作台登录用户，用于审计和责任追溯。
    /// </remarks>
    public string? PublishedBy { get; set; }

    /// <summary>
    /// 发布、停用或回滚发生时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心自动填充，用于审计和时间线展示。
    /// </remarks>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// 发布说明或审批备注。
    /// </summary>
    /// <remarks>
    /// 操作人填写的发布说明或审批备注。
    /// 例如："根据收费处通知调整折价比例，经审核通过后发布"。
    /// </remarks>
    public string? PublishRemark { get; set; }
}
