using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则缓存失效 outbox 处理器。
/// </summary>
public sealed class RuleCacheInvalidationOutboxProcessor
{
    private const int DefaultBatchSize = 50;

    private readonly IRuleCacheInvalidationOutboxRepository _outboxRepository;
    private readonly ICacheVersionSynchronizer _cacheVersionSynchronizer;
    private readonly ILogger<RuleCacheInvalidationOutboxProcessor> _logger;

    /// <summary>
    /// 初始化规则缓存失效 outbox 处理器。
    /// </summary>
    public RuleCacheInvalidationOutboxProcessor(
        IRuleCacheInvalidationOutboxRepository outboxRepository,
        ICacheVersionSynchronizer cacheVersionSynchronizer,
        ILogger<RuleCacheInvalidationOutboxProcessor> logger)
    {
        _outboxRepository = outboxRepository;
        _cacheVersionSynchronizer = cacheVersionSynchronizer;
        _logger = logger;
    }

    /// <summary>
    /// 处理当前到期的缓存失效任务。
    /// </summary>
    public async Task ProcessPendingAsync(
        int maxCount = DefaultBatchSize,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = DateTime.Now;
        var items = await _outboxRepository.GetPendingAsync(now, maxCount);
        foreach (var item in items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ProcessAsync(item, cancellationToken);
        }
    }

    private async Task ProcessAsync(
        RuleCacheInvalidationOutbox item,
        CancellationToken cancellationToken)
    {
        try
        {
            await _cacheVersionSynchronizer.IncreaseVersionAsync(item.CacheScope, cancellationToken);
            await _outboxRepository.MarkProcessedAsync(item.OutboxId, DateTime.Now);
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            var retryCount = item.RetryCount + 1;
            var nextRetryAt = DateTime.Now + GetRetryDelay(retryCount);
            await _outboxRepository.MarkFailedAsync(
                item.OutboxId,
                Truncate(ex.Message, 1000),
                retryCount,
                nextRetryAt);

            _logger.LogWarning(
                ex,
                "规则缓存失效 outbox 处理失败 OutboxId={OutboxId}, Scope={Scope}, RetryCount={RetryCount}, NextRetryAt={NextRetryAt}",
                item.OutboxId,
                item.CacheScope,
                retryCount,
                nextRetryAt);
        }
    }

    private static TimeSpan GetRetryDelay(int retryCount)
    {
        var seconds = Math.Min(300, Math.Pow(2, Math.Min(retryCount, 8)));
        return TimeSpan.FromSeconds(seconds);
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];
}
