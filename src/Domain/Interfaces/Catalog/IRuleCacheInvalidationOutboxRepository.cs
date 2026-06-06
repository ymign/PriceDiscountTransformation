using Pricing.RuleCenter.Core.Aggregates.Catalog;

namespace Pricing.RuleCenter.Core.Interfaces.Catalog;

/// <summary>
/// 规则缓存失效 outbox 仓储接口。
/// </summary>
public interface IRuleCacheInvalidationOutboxRepository
{
    /// <summary>
    /// 新增待处理缓存失效任务。
    /// </summary>
    Task<long> InsertAsync(RuleCacheInvalidationOutbox entity);

    /// <summary>
    /// 读取当前到期的待处理或失败重试任务。
    /// </summary>
    Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetPendingAsync(DateTime now, int maxCount);

    /// <summary>
    /// 标记任务已处理。
    /// </summary>
    Task<bool> MarkProcessedAsync(long outboxId, DateTime processedAt);

    /// <summary>
    /// 标记任务处理失败，并写入下一次重试时间。
    /// </summary>
    Task<bool> MarkFailedAsync(long outboxId, string lastError, int retryCount, DateTime nextRetryAt);
}
