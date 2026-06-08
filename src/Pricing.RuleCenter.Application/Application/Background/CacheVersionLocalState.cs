using System.Collections.Concurrent;

namespace Pricing.RuleCenter.Application.Background;

/// <summary>
/// 单实例缓存版本本地状态。
/// </summary>
/// <remarks>
/// 用于保存当前服务实例已经看到的共享缓存版本号。注册为单例后，同一实例内所有
/// <see cref="CacheVersionSynchronizer"/> 作用域共享状态；不同实例则互不影响，便于模拟多实例收敛。
/// </remarks>
public sealed class CacheVersionLocalState
{
    private readonly ConcurrentDictionary<string, long> _versions =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 读取指定作用域的本地版本号。
    /// </summary>
    public long GetVersion(string cacheScope)
    {
        return _versions.TryGetValue(cacheScope, out var version) ? version : 0L;
    }

    /// <summary>
    /// 更新指定作用域的本地版本号。
    /// </summary>
    public void SetVersion(string cacheScope, long version)
    {
        _versions[cacheScope] = version;
    }
}
