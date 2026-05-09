using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_PUBLISH")]
/// <summary>
/// 规则发布流水实体，对应 PR_RULE_PUBLISH。
/// </summary>
/// <remarks>
/// 发布流水记录规则发布、停用和回滚事件。它用于审计和页面展示，不直接作为计价匹配依据。
/// </remarks>
public sealed class RulePublish
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "PUBLISH_ID")]
    /// <summary>
    /// 发布流水主键。
    /// </summary>
    public long PublishId { get; set; }

    [SugarColumn(ColumnName = "PUBLISH_NO")]
    /// <summary>
    /// 发布流水号，用于审计和日志交叉定位。
    /// </summary>
    public string PublishNo { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "RULE_ID")]
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "FROM_VERSION", IsNullable = true)]
    /// <summary>
    /// 操作前版本号；首次发布时可以为空。
    /// </summary>
    public int? FromVersion { get; set; }

    [SugarColumn(ColumnName = "TO_VERSION")]
    /// <summary>
    /// 操作后版本号。
    /// </summary>
    public int ToVersion { get; set; }

    [SugarColumn(ColumnName = "ACTION_TYPE")]
    /// <summary>
    /// 动作类型，决定由哪个动作执行器处理
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "PUBLISHED_BY", IsNullable = true)]
    /// <summary>
    /// 发布、停用或回滚操作人
    /// </summary>
    public string? PublishedBy { get; set; }

    [SugarColumn(ColumnName = "PUBLISHED_AT")]
    /// <summary>
    /// 发布、停用或回滚发生时间
    /// </summary>
    public DateTime PublishedAt { get; set; }

    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    /// <summary>
    /// 发布、停用或回滚备注。
    /// </summary>
    public string? Remark { get; set; }
}
