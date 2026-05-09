using SqlSugar;

namespace Pricing.RuleCenter.Core.Models;

[SugarTable("PR_LIMIT_LOCK")]
/// <summary>
/// 限额锁实体，对应 PR_LIMIT_LOCK。
/// </summary>
/// <remarks>
/// 该表不保存占用事实，只提供 Oracle SELECT FOR UPDATE 的锁行载体，用于串行化同一限额维度的 confirm。
/// </remarks>
public sealed class LimitLock
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "LOCK_KEY")]
    /// <summary>
    /// 锁键，由限额类型、患者、项目和时间桶等维度组成。
    /// </summary>
    public string LockKey { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "LOCK_DESC", IsNullable = true)]
    /// <summary>
    /// 锁说明，用于人工排查锁行来源。
    /// </summary>
    public string? LockDesc { get; set; }

    [SugarColumn(ColumnName = "UPDATED_AT")]
    /// <summary>
    /// 记录最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
