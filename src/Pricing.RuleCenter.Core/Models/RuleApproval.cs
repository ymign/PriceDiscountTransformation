using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 规则审核表实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_RULE_APPROVAL
/// </para>
/// <para>
/// 在规则生命周期中的角色：审核表记录规则发布、停用、回滚等操作的审核流程。
/// 每次规则状态变更前需要提交审核申请，审核通过后方可执行实际操作。
/// </para>
/// <para>
/// 审核状态流转：
/// <list type="bullet">
/// <item>PENDING — 待审核，已提交但未处理</item>
/// <item>APPROVED — 已通过，允许执行对应操作</item>
/// <item>REJECTED — 已驳回，不允许执行对应操作</item>
/// </list>
/// </para>
/// <para>
/// 操作类型说明：
/// <list type="bullet">
/// <item>PUBLISH — 发布，规则开始生效</item>
/// <item>DISABLE — 停用，规则不再参与匹配</item>
/// <item>ROLLBACK — 回滚，恢复到之前版本</item>
/// </list>
/// </para>
/// </remarks>
[SugarTable("PR_RULE_APPROVAL")]
public sealed class RuleApproval
{
    /// <summary>
    /// 审核主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 APPROVAL_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条审核记录。
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true, ColumnName = "APPROVAL_ID")]
    public long ApprovalId { get; set; }

    /// <summary>
    /// 关联的规则主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_HEADER.RULE_ID，标识本次审核针对哪条规则。
    /// </remarks>
    [SugarColumn(ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    /// <summary>
    /// 版本号。
    /// </summary>
    /// <remarks>
    /// 标识本次审核针对规则的哪个版本。
    /// 发布操作时指向待发布的版本号，停用和回滚操作时指向当前生效版本号。
    /// 可为空，表示操作不特定针对某个版本。
    /// </remarks>
    [SugarColumn(ColumnName = "VERSION_NO", IsNullable = true)]
    public int? VersionNo { get; set; }

    /// <summary>
    /// 操作类型。
    /// </summary>
    /// <remarks>
    /// 标识本次审核申请的操作类型：
    /// <list type="bullet">
    /// <item>PUBLISH — 发布，规则开始生效</item>
    /// <item>DISABLE — 停用，规则不再参与匹配</item>
    /// <item>ROLLBACK — 回滚，恢复到之前版本</item>
    /// </list>
    /// 不可为空。该值应与 PR_DICT 中的 ACTION_TYPE 字典对应。
    /// </remarks>
    [SugarColumn(ColumnName = "ACTION_TYPE")]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// 审核状态。
    /// </summary>
    /// <remarks>
    /// 审核流程的状态标识：
    /// <list type="bullet">
    /// <item>PENDING — 待审核，已提交但未处理</item>
    /// <item>APPROVED — 已通过，允许执行对应操作</item>
    /// <item>REJECTED — 已驳回，不允许执行对应操作</item>
    /// </list>
    /// 默认值为 "PENDING"。
    /// </remarks>
    [SugarColumn(ColumnName = "APPROVAL_STATUS")]
    public string ApprovalStatus { get; set; } = "PENDING";

    /// <summary>
    /// 提交人。
    /// </summary>
    /// <remarks>
    /// 提交审核申请的操作人，来源为工作台登录用户。
    /// 不可为空。
    /// </remarks>
    [SugarColumn(ColumnName = "SUBMITTED_BY")]
    public string SubmittedBy { get; set; } = string.Empty;

    /// <summary>
    /// 提交时间。
    /// </summary>
    /// <remarks>
    /// 审核申请提交的时间，由计价中心自动填充。
    /// </remarks>
    [SugarColumn(ColumnName = "SUBMITTED_AT")]
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// 审核人。
    /// </summary>
    /// <remarks>
    /// 执行审核操作的人员，来源为工作台登录用户。
    /// 在审核状态为 PENDING 时为空，审核通过或驳回后填充。
    /// </remarks>
    [SugarColumn(ColumnName = "REVIEWED_BY", IsNullable = true)]
    public string? ReviewedBy { get; set; }

    /// <summary>
    /// 审核时间。
    /// </summary>
    /// <remarks>
    /// 审核操作执行的时间。
    /// 在审核状态为 PENDING 时为空，审核通过或驳回后填充。
    /// </remarks>
    [SugarColumn(ColumnName = "REVIEWED_AT", IsNullable = true)]
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// 审核意见。
    /// </summary>
    /// <remarks>
    /// 审核人填写的意见说明。
    /// 通过时可填写同意原因，驳回时应填写驳回理由。
    /// 用于审计和问题追溯。
    /// </remarks>
    [SugarColumn(ColumnName = "REVIEW_COMMENT", IsNullable = true)]
    public string? ReviewComment { get; set; }
}
