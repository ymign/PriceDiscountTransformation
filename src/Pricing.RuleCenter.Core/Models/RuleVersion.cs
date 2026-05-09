using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_RULE_VERSION")]
public sealed class RuleVersion
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "VERSION_ID")]
    public long VersionId { get; set; }

    [SugarColumn(ColumnName = "RULE_ID")]
    public long RuleId { get; set; }

    [SugarColumn(ColumnName = "VERSION_NO")]
    public int VersionNo { get; set; }

    [SugarColumn(ColumnName = "VERSION_STATUS")]
    public string VersionStatus { get; set; } = "DRAFT";

    [SugarColumn(ColumnName = "EFFECTIVE_FROM", IsNullable = true)]
    public DateTime? EffectiveFrom { get; set; }

    [SugarColumn(ColumnName = "EFFECTIVE_TO", IsNullable = true)]
    public DateTime? EffectiveTo { get; set; }

    [SugarColumn(ColumnName = "RULE_SNAPSHOT", ColumnDataType = "CLOB", IsNullable = true)]
    public string? RuleSnapshot { get; set; }

    [SugarColumn(ColumnName = "PUBLISHED_BY", IsNullable = true)]
    public string? PublishedBy { get; set; }

    [SugarColumn(ColumnName = "PUBLISHED_AT", IsNullable = true)]
    public DateTime? PublishedAt { get; set; }

    [SugarColumn(ColumnName = "PUBLISH_REMARK", IsNullable = true)]
    public string? PublishRemark { get; set; }
}
