namespace Pricing.RuleCenter.Core.Aggregates.Catalog;

/// <summary>
/// 缓存版本记录。
/// </summary>
/// <remarks>
/// <para>
/// 对应 Oracle 表：PR_CACHE_VERSION。
/// </para>
/// <para>
/// 用于在多实例部署下共享“哪一类缓存已经失效”的版本号。
/// 各实例仍然使用本地内存缓存，但会对比数据库中的版本号判断本机缓存是否过期。
/// </para>
/// </remarks>
public sealed class CacheVersion
{
    /// <summary>
    /// 缓存作用域编码。
    /// </summary>
    /// <remarks>
    /// 例如：
    /// <list type="bullet">
    /// <item>EFFECTIVE_RULES — 生效规则查询缓存</item>
    /// <item>ACTION_TYPE_ORDER — 动作执行顺序运行期缓存</item>
    /// </list>
    /// 作为主键使用。
    /// </remarks>
    public string CacheScope { get; set; } = string.Empty;

    /// <summary>
    /// 当前版本号。
    /// </summary>
    /// <remarks>
    /// 每次命中发布/停用/回滚或字典变更时递增 1。
    /// 各实例比较本地已知版本和数据库版本，不一致则清空本地缓存并更新本地版本。
    /// </remarks>
    public long VersionNo { get; set; }

    /// <summary>
    /// 最后更新时间。
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
