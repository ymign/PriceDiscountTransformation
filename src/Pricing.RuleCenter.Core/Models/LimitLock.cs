using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_LIMIT_LOCK")]
public sealed class LimitLock
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "LOCK_KEY")]
    public string LockKey { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "LOCK_DESC", IsNullable = true)]
    public string? LockDesc { get; set; }

    [SugarColumn(ColumnName = "UPDATED_AT")]
    public DateTime UpdatedAt { get; set; }
}
