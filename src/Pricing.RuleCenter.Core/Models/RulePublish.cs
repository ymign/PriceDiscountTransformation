using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_PUBLISH")]
public sealed class RulePublish
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "PUBLISH_ID")]
    public long PublishId { get; set; }

    [SugarColumn(ColumnName = "PUBLISH_NO")]
    public string PublishNo { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "FROM_VERSION", IsNullable = true)]
    public int? FromVersion { get; set; }

    [SugarColumn(ColumnName = "TO_VERSION")]
    public int ToVersion { get; set; }

    [SugarColumn(ColumnName = "ACTION_TYPE")]
    public string ActionType { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "PUBLISHED_BY", IsNullable = true)]
    public string? PublishedBy { get; set; }

    [SugarColumn(ColumnName = "PUBLISHED_AT")]
    public DateTime PublishedAt { get; set; }

    [SugarColumn(ColumnName = "REMARK", IsNullable = true)]
    public string? Remark { get; set; }
}
