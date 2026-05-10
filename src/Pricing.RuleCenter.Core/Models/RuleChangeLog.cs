using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 规则变更日志实体。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_RULE_CHANGE_LOG
/// </para>
/// <para>
/// 在规则生命周期中的角色：变更日志保存面向配置人员的变更摘要和可选快照，
/// 主要用于规则生命周期审计。
/// </para>
/// <para>
/// 与 PR_RULE_PUBLISH 的区别：
/// <list type="bullet">
/// <item>PR_RULE_PUBLISH 记录发布、停用、回滚等"发布动作"流水</item>
/// <item>PR_RULE_CHANGE_LOG 记录更细粒度的变更，包括创建、编辑、提交审核等</item>
/// </list>
/// 两者互补，共同构成规则变更的完整审计链。
/// </para>
/// <para>
/// 可追溯性作为架构原则：每条变更日志必须可追溯到操作人、操作时间、变更前后快照。
/// </para>
/// </remarks>
[SugarTable("PR_RULE_CHANGE_LOG")]
public sealed class RuleChangeLog
{
    /// <summary>
    /// 变更日志主键。
    /// </summary>
    /// <remarks>
    /// 对应 Oracle 列 CHANGE_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// 全局唯一标识一条变更日志记录。
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true, ColumnName = "CHANGE_ID")]
    public long ChangeId { get; set; }

    /// <summary>
    /// 关联的规则主键。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_HEADER.RULE_ID，标识本次变更针对哪条规则。
    /// </remarks>
    [SugarColumn(ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    /// <summary>
    /// 规则版本号。
    /// </summary>
    /// <remarks>
    /// 对应 PR_RULE_VERSION.VERSION_NO，标识本次变更发生在哪个版本。
    /// 可为空，空值表示变更不关联特定版本（如规则级别的变更）。
    /// </remarks>
    [SugarColumn(ColumnName = "VERSION_NO", IsNullable = true)]
    public int? VersionNo { get; set; }

    /// <summary>
    /// 变更类型编码。
    /// </summary>
    /// <remarks>
    /// 标识本次变更的操作类型：
    /// <list type="bullet">
    /// <item>CREATE — 创建规则或版本</item>
    /// <item>UPDATE — 编辑规则条件或动作</item>
    /// <item>PUBLISH — 正式发布</item>
    /// <item>DISABLE — 停用</item>
    /// <item>ROLLBACK — 回滚到之前版本</item>
    /// <item>APPROVE — 审核通过</item>
    /// <item>REJECT — 审核驳回</item>
    /// </list>
    /// 不可为空。
    /// </remarks>
    [SugarColumn(ColumnName = "CHANGE_TYPE")]
    public string ChangeType { get; set; } = string.Empty;

    /// <summary>
    /// 变更摘要。
    /// </summary>
    /// <remarks>
    /// 面向配置人员展示的变更摘要文本。
    /// 例如："新增条件：收费场景 = 门诊；新增动作：公式折算"。
    /// 用于工作台变更历史列表展示。
    /// </remarks>
    [SugarColumn(ColumnName = "CHANGE_SUMMARY", IsNullable = true)]
    public string? ChangeSummary { get; set; }

    /// <summary>
    /// 变更前快照 JSON。
    /// </summary>
    /// <remarks>
    /// 存储变更前的规则完整状态（包括条件和动作），用于回滚和审计。
    /// Oracle 存储类型为 CLOB，无原生 JSON 支持。
    /// 空值表示创建操作（无变更前状态）。
    /// </remarks>
    [SugarColumn(ColumnName = "BEFORE_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    public string? BeforeSnapshot { get; set; }

    /// <summary>
    /// 变更后快照 JSON。
    /// </summary>
    /// <remarks>
    /// 存储变更后的规则完整状态（包括条件和动作），用于回滚和审计。
    /// Oracle 存储类型为 CLOB，无原生 JSON 支持。
    /// </remarks>
    [SugarColumn(ColumnName = "AFTER_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    public string? AfterSnapshot { get; set; }

    /// <summary>
    /// 变更操作人。
    /// </summary>
    /// <remarks>
    /// 来源为工作台登录用户，用于审计和责任追溯。
    /// </remarks>
    [SugarColumn(ColumnName = "CHANGED_BY", IsNullable = true)]
    public string? ChangedBy { get; set; }

    /// <summary>
    /// 变更发生时间。
    /// </summary>
    /// <remarks>
    /// 由计价中心自动填充，用于审计和时间线展示。
    /// </remarks>
    [SugarColumn(ColumnName = "CHANGED_AT")]
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// 来源系统编码。
    /// </summary>
    /// <remarks>
    /// 标识本次变更来自哪个系统。
    /// 常见值：WORKBENCH（规则维护工作台）、API（接口调用）、MIGRATION（历史数据迁移）。
    /// 空值表示来源系统未知。
    /// </remarks>
    [SugarColumn(ColumnName = "SOURCE_SYSTEM", IsNullable = true)]
    public string? SourceSystem { get; set; }
}
