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

    public CacheVersionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<CacheVersion?> GetByScopeAsync(string cacheScope)
    {
        return await _db.Queryable<CacheVersion>()
            .InSingleAsync(cacheScope);
    }

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
