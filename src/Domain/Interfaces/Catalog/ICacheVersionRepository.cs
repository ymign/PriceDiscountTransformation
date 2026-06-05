using Pricing.RuleCenter.Core.Aggregates.Catalog;

namespace Pricing.RuleCenter.Core.Interfaces.Catalog;

/// <summary>
/// 缓存版本仓储接口。
/// </summary>
/// <remarks>
/// 用于在多实例部署下共享缓存版本号，帮助各实例判断本地内存缓存是否已经过期。
/// </remarks>
public interface ICacheVersionRepository
{
    /// <summary>
    /// 读取指定缓存作用域的当前版本。
    /// </summary>
    Task<CacheVersion?> GetByScopeAsync(string cacheScope);

    /// <summary>
    /// 递增指定缓存作用域的版本号；不存在时自动创建并从 1 开始。
    /// </summary>
    Task<long> IncreaseVersionAsync(string cacheScope);
}
