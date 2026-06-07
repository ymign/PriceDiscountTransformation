using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuleCacheOutboxAppServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_ReturnsPendingAndFailedCountsAndRecentFailures()
    {
        var repository = new InMemoryRuleCacheInvalidationOutboxRepository(
            new RuleCacheInvalidationOutbox
            {
                OutboxId = 1,
                CacheScope = "EFFECTIVE_RULES",
                OperationType = "PUBLISH",
                RuleId = 10,
                VersionNo = 1,
                Status = CacheInvalidationOutboxStatusCodes.Pending,
                CreatedAt = new DateTime(2026, 6, 7, 10, 0, 0)
            },
            new RuleCacheInvalidationOutbox
            {
                OutboxId = 2,
                CacheScope = "ACTION_TYPE_ORDER",
                OperationType = "ROLLBACK",
                RuleId = 11,
                VersionNo = 2,
                Status = CacheInvalidationOutboxStatusCodes.Failed,
                RetryCount = 3,
                NextRetryAt = new DateTime(2026, 6, 7, 10, 5, 0),
                LastError = "broadcast failed",
                CreatedAt = new DateTime(2026, 6, 7, 10, 1, 0)
            });
        var service = new RuleCacheOutboxAppService(repository);

        var result = await service.GetSummaryAsync();

        Assert.Equal(1, result.PendingCount);
        Assert.Equal(1, result.FailedCount);
        Assert.Equal(new DateTime(2026, 6, 7, 10, 0, 0), result.OldestUnprocessedCreatedAt);
        var failed = Assert.Single(result.FailedItems);
        Assert.Equal(2L, failed.OutboxId);
        Assert.Equal("broadcast failed", failed.LastError);
        Assert.Equal(3, failed.RetryCount);
    }

    private sealed class InMemoryRuleCacheInvalidationOutboxRepository : IRuleCacheInvalidationOutboxRepository
    {
        private readonly IReadOnlyList<RuleCacheInvalidationOutbox> _items;

        public InMemoryRuleCacheInvalidationOutboxRepository(params RuleCacheInvalidationOutbox[] items)
        {
            _items = items;
        }

        public Task<long> InsertAsync(RuleCacheInvalidationOutbox entity) => Task.FromResult(0L);
        public Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetPendingAsync(DateTime now, int maxCount) => Task.FromResult((IReadOnlyList<RuleCacheInvalidationOutbox>)Array.Empty<RuleCacheInvalidationOutbox>());
        public Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetForDashboardAsync(int maxFailedCount) => Task.FromResult(_items);
        public Task<bool> MarkProcessedAsync(long outboxId, DateTime processedAt) => Task.FromResult(true);
        public Task<bool> MarkFailedAsync(long outboxId, string lastError, int retryCount, DateTime nextRetryAt) => Task.FromResult(true);
    }
}
