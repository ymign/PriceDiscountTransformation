using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Catalog;

namespace Pricing.RuleCenter.Application.Rules;

/// <summary>
/// 规则缓存失效 outbox 运维查询服务。
/// </summary>
public sealed class RuleCacheOutboxAppService
{
    private readonly IRuleCacheInvalidationOutboxRepository _outboxRepository;

    /// <summary>
    /// 初始化规则缓存失效 outbox 运维查询服务。
    /// </summary>
    public RuleCacheOutboxAppService(IRuleCacheInvalidationOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }

    /// <summary>
    /// 读取规则缓存失效 outbox 的运维汇总。
    /// </summary>
    public async Task<RuleCacheOutboxSummaryResponse> GetSummaryAsync(int maxFailedCount = 20)
    {
        var items = await _outboxRepository.GetForDashboardAsync(maxFailedCount);
        var pending = items
            .Where(item => string.Equals(item.Status, CacheInvalidationOutboxStatusCodes.Pending, StringComparison.OrdinalIgnoreCase))
            .ToList();
        var failed = items
            .Where(item => string.Equals(item.Status, CacheInvalidationOutboxStatusCodes.Failed, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.CreatedAt)
            .Take(maxFailedCount)
            .ToList();

        return new RuleCacheOutboxSummaryResponse
        {
            PendingCount = pending.Count,
            FailedCount = failed.Count,
            OldestUnprocessedCreatedAt = items.Count == 0 ? null : items.Min(item => item.CreatedAt),
            FailedItems = failed.Select(item => new RuleCacheOutboxItemResponse
            {
                OutboxId = item.OutboxId,
                CacheScope = item.CacheScope,
                OperationType = item.OperationType,
                RuleId = item.RuleId,
                VersionNo = item.VersionNo,
                Status = item.Status,
                RetryCount = item.RetryCount,
                NextRetryAt = item.NextRetryAt,
                LastError = item.LastError,
                CreatedAt = item.CreatedAt
            }).ToList()
        };
    }
}
