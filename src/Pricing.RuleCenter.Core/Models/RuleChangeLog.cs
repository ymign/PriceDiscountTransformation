using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_CHANGE_LOG")]
/// <summary>
/// 规则变更日志实体，对应 PR_RULE_CHANGE_LOG。
/// </summary>
/// <remarks>
/// 变更日志保存面向配置人员的变更摘要和可选快照，主要用于规则生命周期审计。
/// </remarks>
public sealed class RuleChangeLog
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "CHANGE_ID")]
    /// <summary>
    /// 变更日志主键。
    /// </summary>
    public long ChangeId { get; set; }

    [SugarColumn(ColumnName = "RULE_ID")]
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "VERSION_NO", IsNullable = true)]
    /// <summary>
    /// 规则版本号，与规则条件和动作的 VERSION_NO 对齐
    /// </summary>
    public int? VersionNo { get; set; }

    [SugarColumn(ColumnName = "CHANGE_TYPE")]
    /// <summary>
    /// 变更类型，例如 CREATE、UPDATE、PUBLISH、DISABLE 或 ROLLBACK。
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "CHANGE_SUMMARY", IsNullable = true)]
    /// <summary>
    /// 变更摘要，面向配置人员展示。
    /// </summary>
    public string? ChangeSummary { get; set; }

    [SugarColumn(ColumnName = "BEFORE_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 变更前快照 JSON。
    /// </summary>
    public string? BeforeSnapshot { get; set; }

    [SugarColumn(ColumnName = "AFTER_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    /// <summary>
    /// 变更后快照 JSON。
    /// </summary>
    public string? AfterSnapshot { get; set; }

    [SugarColumn(ColumnName = "CHANGED_BY", IsNullable = true)]
    /// <summary>
    /// 变更操作人。
    /// </summary>
    public string? ChangedBy { get; set; }

    [SugarColumn(ColumnName = "CHANGED_AT")]
    /// <summary>
    /// 变更发生时间。
    /// </summary>
    public DateTime ChangedAt { get; set; }

    [SugarColumn(ColumnName = "SOURCE_SYSTEM", IsNullable = true)]
    /// <summary>
    /// 来源系统编码，例如 HIS、自助机或微信公众号
    /// </summary>
    public string? SourceSystem { get; set; }
}
