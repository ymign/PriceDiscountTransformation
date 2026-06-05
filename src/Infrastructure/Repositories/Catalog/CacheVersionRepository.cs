using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Catalog;

/// <summary>
/// 缓存版本仓储实现。
/// </summary>
public sealed class CacheVersionRepository : ICacheVersionRepository
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheVersionRepository"/> class.
    /// </summary>
    /// <param name="db">SqlSugar 数据库访问客户端。</param>
    public CacheVersionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 按缓存作用域查询当前共享版本。
    /// </summary>
    /// <param name="cacheScope">缓存作用域编码。</param>
    /// <returns>找到时返回缓存版本记录；不存在时返回 <see langword="null"/>。</returns>
    public async Task<CacheVersion?> GetByScopeAsync(string cacheScope)
    {
        return await _db.Queryable<CacheVersion>()
            .InSingleAsync(cacheScope);
    }

    /// <summary>
    /// 递增指定缓存作用域的共享版本号。
    /// </summary>
    /// <param name="cacheScope">缓存作用域编码。</param>
    /// <returns>递增后的版本号。</returns>
    public async Task<long> IncreaseVersionAsync(string cacheScope)
    {
        var normalizedScope = cacheScope.Trim().ToUpperInvariant();
        await _db.Ado.ExecuteCommandAsync(
            "MERGE INTO PR_CACHE_VERSION T " +
            "USING (SELECT :CacheScope CACHE_SCOPE FROM DUAL) S " +
            "ON (T.CACHE_SCOPE = S.CACHE_SCOPE) " +
            "WHEN MATCHED THEN UPDATE SET T.VERSION_NO = T.VERSION_NO + 1, T.UPDATED_AT = SYSDATE " +
            "WHEN NOT MATCHED THEN INSERT (CACHE_SCOPE, VERSION_NO, UPDATED_AT) VALUES (S.CACHE_SCOPE, 1, SYSDATE)",
            new { CacheScope = normalizedScope });

        var current = await GetByScopeAsync(normalizedScope)
            ?? throw new InvalidOperationException($"缓存版本不存在: {normalizedScope}");
        return current.VersionNo;
    }
}
