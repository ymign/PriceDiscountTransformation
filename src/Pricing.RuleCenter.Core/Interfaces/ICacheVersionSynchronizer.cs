namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 缓存版本同步器。
/// </summary>
/// <remarks>
/// 用于比较数据库中的共享缓存版本和本机已知版本。
/// 当版本变化时，同步器负责清理本机缓存并更新本地版本快照。
/// </remarks>
public interface ICacheVersionSynchronizer
{
    /// <summary>
    /// 确保本机缓存版本与数据库一致；若检测到版本变化，则清理本机缓存。
    /// </summary>
    Task SyncAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 递增指定缓存作用域的数据库版本。
    /// </summary>
    Task<long> IncreaseVersionAsync(string cacheScope, CancellationToken cancellationToken = default);
}
