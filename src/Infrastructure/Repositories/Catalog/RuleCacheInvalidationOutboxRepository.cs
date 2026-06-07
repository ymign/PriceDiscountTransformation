using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Catalog;

/// <summary>
/// 规则缓存失效 outbox 仓储实现。
/// </summary>
public sealed class RuleCacheInvalidationOutboxRepository : IRuleCacheInvalidationOutboxRepository
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 初始化规则缓存失效 outbox 仓储。
    /// </summary>
    public RuleCacheInvalidationOutboxRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<long> InsertAsync(RuleCacheInvalidationOutbox entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_CACHE_INV_OUTBOX.NEXTVAL FROM DUAL");
        entity.OutboxId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetPendingAsync(DateTime now, int maxCount)
    {
        return await _db.Queryable<RuleCacheInvalidationOutbox>()
            .Where(item => item.Status != CacheInvalidationOutboxStatusCodes.Processed)
            .Where(item => item.NextRetryAt == null || item.NextRetryAt <= now)
            .OrderBy(item => item.CreatedAt)
            .OrderBy(item => item.OutboxId)
            .Take(maxCount)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetForDashboardAsync(int maxFailedCount)
    {
        return await _db.Queryable<RuleCacheInvalidationOutbox>()
            .Where(item => item.Status != CacheInvalidationOutboxStatusCodes.Processed)
            .OrderBy(item => item.Status)
            .OrderBy(item => item.CreatedAt)
            .Take(maxFailedCount)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> MarkProcessedAsync(long outboxId, DateTime processedAt)
    {
        return await _db.Updateable<RuleCacheInvalidationOutbox>()
            .SetColumns(item => item.Status == CacheInvalidationOutboxStatusCodes.Processed)
            .SetColumns(item => item.ProcessedAt == processedAt)
            .SetColumns(item => item.NextRetryAt == null)
            .SetColumns(item => item.LastError == null)
            .Where(item => item.OutboxId == outboxId)
            .ExecuteCommandAsync() > 0;
    }

    /// <inheritdoc />
    public async Task<bool> MarkFailedAsync(long outboxId, string lastError, int retryCount, DateTime nextRetryAt)
    {
        return await _db.Updateable<RuleCacheInvalidationOutbox>()
            .SetColumns(item => item.Status == CacheInvalidationOutboxStatusCodes.Failed)
            .SetColumns(item => item.LastError == lastError)
            .SetColumns(item => item.RetryCount == retryCount)
            .SetColumns(item => item.NextRetryAt == nextRetryAt)
            .Where(item => item.OutboxId == outboxId)
            .ExecuteCommandAsync() > 0;
    }
}
