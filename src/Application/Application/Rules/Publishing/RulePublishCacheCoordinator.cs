using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Application.Rules.Guards;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则发布后的缓存失效协调器。
/// </summary>
public sealed class RulePublishCacheCoordinator
{
    private readonly RulePublishGuard _publishGuard;
    private readonly RulePublishCacheInvalidator _cacheInvalidator;
    private readonly IRuleCacheInvalidationOutboxRepository _outboxRepository;
    private readonly RuleCacheInvalidationOutboxProcessor _outboxProcessor;

    /// <summary>
    /// 初始化规则发布缓存失效协调器。
    /// </summary>
    public RulePublishCacheCoordinator(
        RulePublishGuard publishGuard,
        RulePublishCacheInvalidator cacheInvalidator,
        IRuleCacheInvalidationOutboxRepository outboxRepository,
        RuleCacheInvalidationOutboxProcessor outboxProcessor)
    {
        _publishGuard = publishGuard;
        _cacheInvalidator = cacheInvalidator;
        _outboxRepository = outboxRepository;
        _outboxProcessor = outboxProcessor;
    }

    /// <summary>
    /// 在事务内登记缓存失效 outbox。
    /// </summary>
    public async Task EnqueueAsync(
        long ruleId,
        int versionNo,
        string operationType,
        DateTime now)
    {
        await EnqueueAsync(CacheVersionSynchronizer.EffectiveRulesScope, ruleId, versionNo, operationType, now);
        await EnqueueAsync(CacheVersionSynchronizer.ActionTypeOrderScope, ruleId, versionNo, operationType, now);
    }

    /// <summary>
    /// 事务提交后清理本机缓存并驱动 outbox 处理。
    /// </summary>
    public async Task InvalidateAfterCommitAsync()
    {
        _publishGuard.ClearCache();
        _cacheInvalidator.ClearEffectiveCache();
        var result = await _outboxProcessor.ProcessPendingAsync();
        if (result.FailedCount > 0)
        {
            throw new BizException(
                BizErrorCode.ServiceDegraded,
                503,
                $"规则已发布，但缓存广播未完成：失败 {result.FailedCount} 条，请稍后重试或联系管理员处理");
        }
    }

    private async Task EnqueueAsync(
        string cacheScope,
        long ruleId,
        int versionNo,
        string operationType,
        DateTime now)
    {
        await _outboxRepository.InsertAsync(new RuleCacheInvalidationOutbox
        {
            CacheScope = cacheScope,
            OperationType = operationType,
            RuleId = ruleId,
            VersionNo = versionNo,
            Status = CacheInvalidationOutboxStatusCodes.Pending,
            RetryCount = 0,
            CreatedAt = now
        });
    }
}
