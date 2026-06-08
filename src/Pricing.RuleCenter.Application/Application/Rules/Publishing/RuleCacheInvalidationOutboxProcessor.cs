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
    private readonly IClock _clock;
    private readonly ILogger<RuleCacheInvalidationOutboxProcessor> _logger;

    /// <summary>
    /// 初始化规则缓存失效 outbox 处理器。
    /// </summary>
    public RuleCacheInvalidationOutboxProcessor(
        IRuleCacheInvalidationOutboxRepository outboxRepository,
        ICacheVersionSynchronizer cacheVersionSynchronizer,
        IClock clock,
        ILogger<RuleCacheInvalidationOutboxProcessor> logger)
    {
        _outboxRepository = outboxRepository;
        _cacheVersionSynchronizer = cacheVersionSynchronizer;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 处理当前到期的缓存失效任务。
    /// </summary>
    public async Task<RuleCacheInvalidationProcessResult> ProcessPendingAsync(
        int maxCount = DefaultBatchSize,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var items = await _outboxRepository.GetPendingAsync(_clock.Now, maxCount);
        var processedCount = 0;
        var failedCount = 0;
        foreach (var item in items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var processed = await ProcessAsync(item, cancellationToken);
            if (processed)
            {
                processedCount++;
            }
            else
            {
                failedCount++;
            }
        }

        return new RuleCacheInvalidationProcessResult(processedCount, failedCount);
    }

    private async Task<bool> ProcessAsync(
        RuleCacheInvalidationOutbox item,
        CancellationToken cancellationToken)
    {
        try
        {
            await _cacheVersionSynchronizer.IncreaseVersionAsync(item.CacheScope, cancellationToken);
            await _outboxRepository.MarkProcessedAsync(item.OutboxId, _clock.Now);
            return true;
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            var retryCount = item.RetryCount + 1;
            var nextRetryAt = _clock.Now + GetRetryDelay(retryCount);
            await _outboxRepository.MarkFailedAsync(
                item.OutboxId,
                Truncate(ex.Message, 1000),
                retryCount,
                nextRetryAt);

            _logger.LogWarning(
                ex,
                "规则缓存失效发件箱处理失败 发件箱ID={OutboxId}, 范围={Scope}, 重试次数={RetryCount}, 下次重试时间={NextRetryAt}",
                item.OutboxId,
                item.CacheScope,
                retryCount,
                nextRetryAt);

            return false;
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
