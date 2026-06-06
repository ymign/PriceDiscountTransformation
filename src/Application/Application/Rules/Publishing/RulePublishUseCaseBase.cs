using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Rules.Guards;
using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则发布应用服务，负责发布、停用和回滚规则版本。
/// </summary>
/// <remarks>
/// <para>
/// 这是规则配置链路中最重要的状态机入口。它负责在规则主档、版本表、发布记录和变更日志之间保持一致：
/// 发布会把草稿版本变成当前生效版本；停用会让规则整体退出匹配；回滚会把当前版本切回最近一个历史版本。
/// </para>
/// <para>
/// 缓存失效：发布、停用、回滚操作完成后，必须立即清除生效规则缓存，
/// 确保计价引擎在下一次请求时读到最新规则集。这是资金安全硬约束。
/// </para>
/// </remarks>
public abstract class RulePublishUseCaseBase
{
    /// <summary>
    /// 规则主档仓储，用于读取和更新当前版本、规则状态及启用标志。
    /// </summary>
    private readonly IRuleHeaderRepository _headerRepository;
    /// <summary>
    /// 规则版本仓储，用于校验草稿、禁用旧版本、发布新版本和回滚历史版本。
    /// </summary>
    private readonly IRuleVersionRepository _versionRepository;
    /// <summary>
    /// 发布记录仓储，用于保存发布、停用和回滚的可审计流水。
    /// </summary>
    private readonly IRulePublishRepository _publishRepository;
    /// <summary>
    /// 变更日志仓储，用于记录面向配置人员的规则变更摘要。
    /// </summary>
    private readonly IRuleChangeLogRepository _changeLogRepository;
    /// <summary>
    /// 服务日志，用于记录状态机入口的关键操作。
    /// </summary>
    private readonly ILogger _logger;
    /// <summary>
    /// 规则发布生命周期事务执行器。
    /// </summary>
    private readonly RulePublishTransactionWriter _transactionWriter;
    /// <summary>
    /// 规则运行期缓存失效器，用于清除计价引擎侧跨请求共享缓存。
    /// </summary>
    private readonly RulePublishCacheInvalidator _cacheInvalidator;
    private readonly IRuleCacheInvalidationOutboxRepository _cacheInvalidationOutboxRepository;
    private readonly RuleCacheInvalidationOutboxProcessor _cacheInvalidationOutboxProcessor;
    private readonly IClock _clock;
    private readonly RuleApprovalGate _approvalGate;
    private readonly RulePublishGuard _publishGuard;

    /// <summary>
    /// 初始化规则发布服务。
    /// </summary>
    /// <param name="lifecycleRepositories">规则发布生命周期仓储集合。</param>
    /// <param name="definitionRepositories">规则定义仓储集合，用于发布前冲突校验。</param>
    /// <param name="transactionWriter">规则发布生命周期事务执行器。</param>
    /// <param name="cacheInvalidator">规则发布缓存失效器。</param>
    /// <param name="cacheInvalidationOutboxRepository">规则缓存失效 outbox 仓储。</param>
    /// <param name="cacheInvalidationOutboxProcessor">规则缓存失效 outbox 处理器。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    /// <param name="approvalGate">生命周期操作审批门禁。</param>
    /// <param name="publishGuard">规则发布前聚合门禁。</param>
    /// <param name="logger">日志对象。</param>
    protected RulePublishUseCaseBase(
        RulePublishLifecycleRepositories lifecycleRepositories,
        RulePublishDefinitionRepositories definitionRepositories,
        RulePublishTransactionWriter transactionWriter,
        RulePublishCacheInvalidator cacheInvalidator,
        IRuleCacheInvalidationOutboxRepository cacheInvalidationOutboxRepository,
        RuleCacheInvalidationOutboxProcessor cacheInvalidationOutboxProcessor,
        IClock clock,
        RuleApprovalGate approvalGate,
        RulePublishGuard publishGuard,
        ILogger logger)
    {
        _headerRepository = lifecycleRepositories.HeaderRepository;
        _versionRepository = lifecycleRepositories.VersionRepository;
        _publishRepository = lifecycleRepositories.PublishRepository;
        _changeLogRepository = lifecycleRepositories.ChangeLogRepository;
        _transactionWriter = transactionWriter;
        _cacheInvalidator = cacheInvalidator;
        _cacheInvalidationOutboxRepository = cacheInvalidationOutboxRepository;
        _cacheInvalidationOutboxProcessor = cacheInvalidationOutboxProcessor;
        _clock = clock;
        _approvalGate = approvalGate;
        _publishGuard = publishGuard;
        _logger = logger;
    }

    /// <summary>
    /// 读取规则的发布历史。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>发布、停用、回滚流水列表。</returns>
    public async Task<IReadOnlyList<RulePublishResponse>> GetPublishHistoryAsync(long ruleId)
    {
        var items = await _publishRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapPublishToResponse).ToList();
    }

    /// <summary>
    /// 读取规则变更日志。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>规则变更摘要列表。</returns>
    public async Task<IReadOnlyList<RuleChangeLogResponse>> GetChangeLogsAsync(long ruleId)
    {
        var items = await _changeLogRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapChangeLogToResponse).ToList();
    }

    /// <summary>
    /// 发布指定草稿版本，让其成为规则当前生效版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">发布请求，包含目标版本号、发布人和备注。</param>
    /// <exception cref="KeyNotFoundException">规则主档或目标版本不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">目标版本不是草稿状态时抛出。</exception>
    /// <remarks>
    /// 【事务保护】发布操作涉及多表状态变更（版本状态、主档状态、发布流水、变更日志），
    /// 必须在同一事务中执行，确保要么全部成功要么全部回滚。否则可能出现：
    /// - 版本状态已更新但主档未更新 → 计价引擎匹配到不一致的规则
    /// - 发布流水缺失 → 无法追溯发布历史
    /// </remarks>
    protected async Task ExecutePublishAsync(long ruleId, RulePublishRequest request)
    {
        // ========== 第一阶段：读取主档和目标版本（事务外） ==========
        // 发布必须同时拿到主档和版本。主档提供当前版本号，版本表提供目标版本的状态边界。
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");

        var version = await _versionRepository.GetByRuleAndVersionAsync(ruleId, request.VersionNo)
            ?? throw new BizException(
                BizErrorCode.RuleVersionNotFound,
                404,
                $"规则版本不存在: RuleId={ruleId}, VersionNo={request.VersionNo}");

        // ========== 第二阶段：校验目标版本必须是草稿（事务外） ==========
        // 只允许 DRAFT -> PUBLISHED。已经发布、禁用或回滚过的版本不能再次走普通发布入口，
        // 避免发布历史出现同一个版本被多次发布的歧义。
        if (version.VersionStatus != VersionStatusCodes.Draft)
        {
            throw new BizException(
                BizErrorCode.VersionStatusNotAllowed,
                409,
                $"只有草稿版本可以发布, 当前状态: {version.VersionStatus}");
        }

        await _approvalGate.EnsurePassedAsync(ruleId, request.VersionNo, ApprovalActionCodes.Publish);
        await ValidatePublishConflictsAsync(header, request.VersionNo);

        // ========== 第三阶段：事务内执行状态变更和流水写入 ==========
        // 版本状态、主档状态、发布流水、变更日志必须在同一事务中更新。
        // 如果任何一步失败，整体回滚，避免数据不一致。
        await _transactionWriter.ExecuteAsync(async () =>
        {
            var currentHeader = await _headerRepository.GetByIdForUpdateAsync(ruleId)
                ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");
            var currentVersion = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, request.VersionNo)
                ?? throw new BizException(
                    BizErrorCode.RuleVersionNotFound,
                    404,
                    $"规则版本不存在: RuleId={ruleId}, VersionNo={request.VersionNo}");
            if (currentVersion.VersionStatus != VersionStatusCodes.Draft)
            {
                throw new BizException(
                    BizErrorCode.VersionStatusNotAllowed,
                    409,
                    $"只有草稿版本可以发布, 当前状态: {currentVersion.VersionStatus}");
            }

            var oldVersion = currentHeader.CurrentVersion;
            var previousHeaderStatus = currentHeader.Status;

            // 禁用旧生效版本
            if (oldVersion > 0)
            {
                var oldVersionEntity = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, oldVersion);
                if (oldVersionEntity is null)
                {
                    _logger.LogWarning("发布规则时未找到旧版本记录 RuleId={RuleId}, OldVersion={OldVersion}", ruleId, oldVersion);
                }
                else
                {
                    if (string.Equals(oldVersionEntity.VersionStatus, VersionStatusCodes.Published, StringComparison.OrdinalIgnoreCase))
                    {
                        var disableOldVersionUpdated = await _versionRepository.UpdateStatusAsync(
                            oldVersionEntity.VersionId,
                            VersionStatusCodes.Disabled,
                            VersionStatusCodes.Published);
                        if (!disableOldVersionUpdated)
                        {
                            throw new BizException(
                                BizErrorCode.RuleVersionConcurrencyConflict,
                                409,
                                $"RuleId={ruleId}, OldVersionNo={oldVersion} 状态已变化，请刷新后重试");
                        }
                    }
                }
            }

            // 发布目标版本并推进主档
            var now = _clock.Now;
            var publishVersionUpdated = await _versionRepository.UpdateStatusAsync(
                currentVersion.VersionId,
                VersionStatusCodes.Published,
                VersionStatusCodes.Draft);
            if (!publishVersionUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleVersionConcurrencyConflict,
                    409,
                    $"RuleId={ruleId}, VersionNo={request.VersionNo} 状态已变化，请刷新后重试");
            }

            currentHeader.CurrentVersion = request.VersionNo;
            currentHeader.Status = RuleStatusCodes.Published;
            // 规则从停用态重新发布时，必须同步恢复启用标志；
            // 否则 GetEffective/GetSpecialFlag 仍会把它排除，形成“已发布但不生效”的假发布。
            currentHeader.IsEnabled = EnableFlag.Yes;
            currentHeader.UpdatedAt = now;
            var publishHeaderUpdated = await _headerRepository.UpdateAsync(
                currentHeader,
                previousHeaderStatus);
            if (!publishHeaderUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleHeaderConcurrencyConflict,
                    409,
                    $"RuleId={ruleId} 主档状态已变化，请刷新后重试");
            }

            // 写发布流水
            var publishNo = $"PUB-{ruleId}-{request.VersionNo}-{now:yyyyMMddHHmmss}";
            await _publishRepository.InsertAsync(new RulePublish
            {
                PublishNo = publishNo,
                RuleId = ruleId,
                FromVersion = oldVersion > 0 ? oldVersion : null,
                ToVersion = request.VersionNo,
                ActionType = ApprovalActionCodes.Publish,
                PublishedBy = request.PublishedBy,
                PublishedAt = now,
                Remark = request.Remark
            });

            // 写变更摘要
            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = ruleId,
                VersionNo = request.VersionNo,
                ChangeType = ApprovalActionCodes.Publish,
                ChangeSummary = $"发布版本 V{request.VersionNo}",
                ChangedBy = request.PublishedBy,
                ChangedAt = now
            });
            await AddCacheInvalidationOutboxAsync(ruleId, request.VersionNo, ApprovalActionCodes.Publish, now);
        });

        // ========== 第四阶段：清除生效规则缓存（事务外） ==========
        // 发布改变了当前生效版本，必须立即清除缓存，确保计价引擎读到最新规则集。
        await InvalidateCachesAfterCommitAsync();

        _logger.LogInformation("发布规则 RuleId={RuleId}, VersionNo={VersionNo}", ruleId, request.VersionNo);
    }

    /// <summary>
    /// 停用已发布规则。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">停用请求，包含操作人和备注。</param>
    /// <exception cref="KeyNotFoundException">规则不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">规则不是已发布状态时抛出。</exception>
    protected async Task ExecuteDisableAsync(long ruleId, RuleDisableRequest request)
    {
        // ========== 第一阶段：读取并校验主档状态 ==========
        // 只有已发布规则才有"停用"的业务含义，草稿规则不应通过停用入口改变可编辑状态。
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");

        if (header.Status != RuleStatusCodes.Published)
        {
            throw new BizException(
                BizErrorCode.RuleAlreadyDisabled,
                409,
                $"只有已发布的规则可以停用, 当前状态: {header.Status}");
        }

        await _approvalGate.EnsurePassedAsync(ruleId, header.CurrentVersion, ApprovalActionCodes.Disable);

        // ========== 第二阶段：事务内执行状态变更和流水写入 ==========
        await _transactionWriter.ExecuteAsync(async () =>
        {
            var currentHeader = await _headerRepository.GetByIdForUpdateAsync(ruleId)
                ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");
            if (currentHeader.Status != RuleStatusCodes.Published)
            {
                throw new BizException(
                    BizErrorCode.RuleAlreadyDisabled,
                    409,
                    $"只有已发布的规则可以停用, 当前状态: {currentHeader.Status}");
            }

            // 禁用当前版本
            if (currentHeader.CurrentVersion > 0)
            {
                var currentVersion = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, currentHeader.CurrentVersion);
                if (currentVersion is not null)
                {
                    var disableCurrentVersionUpdated = await _versionRepository.UpdateStatusAsync(
                        currentVersion.VersionId,
                        VersionStatusCodes.Disabled,
                        VersionStatusCodes.Published);
                    if (!disableCurrentVersionUpdated)
                    {
                        throw new BizException(
                            BizErrorCode.RuleVersionConcurrencyConflict,
                            409,
                            $"RuleId={ruleId}, CurrentVersionNo={currentHeader.CurrentVersion} 状态已变化，请刷新后重试");
                    }
                }
            }

            // 更新主档启用状态
            var now = _clock.Now;
            currentHeader.Status = RuleStatusCodes.Disabled;
            currentHeader.IsEnabled = EnableFlag.No;
            currentHeader.UpdatedAt = now;
            var disableHeaderUpdated = await _headerRepository.UpdateAsync(
                currentHeader,
                RuleStatusCodes.Published);
            if (!disableHeaderUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleHeaderConcurrencyConflict,
                    409,
                    $"RuleId={ruleId} 主档状态已变化，请刷新后重试");
            }

            // 写停用流水和变更日志
            await _publishRepository.InsertAsync(new RulePublish
            {
                PublishNo = $"DIS-{ruleId}-{now:yyyyMMddHHmmss}",
                RuleId = ruleId,
                FromVersion = currentHeader.CurrentVersion,
                ToVersion = currentHeader.CurrentVersion,
                ActionType = ApprovalActionCodes.Disable,
                PublishedBy = request.PublishedBy,
                PublishedAt = now,
                Remark = request.Remark
            });

            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = ruleId,
                VersionNo = currentHeader.CurrentVersion,
                ChangeType = ApprovalActionCodes.Disable,
                ChangeSummary = $"停用规则, 当前版本 V{currentHeader.CurrentVersion}",
                ChangedBy = request.PublishedBy,
                ChangedAt = now
            });
            await AddCacheInvalidationOutboxAsync(
                ruleId,
                currentHeader.CurrentVersion,
                ApprovalActionCodes.Disable,
                now);
        });

        // ========== 第三阶段：清除生效规则缓存 ==========
        await InvalidateCachesAfterCommitAsync();

        _logger.LogInformation("停用规则 RuleId={RuleId}", ruleId);
    }

    /// <summary>
    /// 将已发布规则回滚到最近一个历史发布版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">回滚请求，包含操作人和备注。</param>
    /// <exception cref="KeyNotFoundException">规则不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">规则未发布或没有可回滚版本时抛出。</exception>
    protected async Task ExecuteRollbackAsync(long ruleId, RuleRollbackRequest request)
    {
        // ========== 第一阶段：读取并校验主档 ==========
        // 回滚只对已发布规则有效；停用或草稿状态下回滚会让规则是否参与匹配变得不清晰。
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");

        if (header.Status != RuleStatusCodes.Published)
        {
            throw new BizException(
                BizErrorCode.RollbackTargetNotAvailable,
                409,
                $"只有已发布的规则可以回滚, 当前状态: {header.Status}");
        }

        await _approvalGate.EnsurePassedAsync(ruleId, header.CurrentVersion, ApprovalActionCodes.Rollback);

        // ========== 第三阶段：事务内执行状态变更和流水写入 ==========
        await _transactionWriter.ExecuteAsync(async () =>
        {
            var currentHeader = await _headerRepository.GetByIdForUpdateAsync(ruleId)
                ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");
            if (currentHeader.Status != RuleStatusCodes.Published)
            {
                throw new BizException(
                    BizErrorCode.RollbackTargetNotAvailable,
                    409,
                    $"只有已发布的规则可以回滚, 当前状态: {currentHeader.Status}");
            }

            var oldVersionNo = currentHeader.CurrentVersion;
            var rollbackVersionNo = await ResolveRollbackVersionNoAsync(ruleId, oldVersionNo);

            // 把当前版本标记为已回滚
            var currentVersion = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, oldVersionNo);
            if (currentVersion is not null)
            {
                var rollbackCurrentVersionUpdated = await _versionRepository.UpdateStatusAsync(
                    currentVersion.VersionId,
                    VersionStatusCodes.RolledBack,
                    VersionStatusCodes.Published);
                if (!rollbackCurrentVersionUpdated)
                {
                    throw new BizException(
                        BizErrorCode.RuleVersionConcurrencyConflict,
                        409,
                        $"RuleId={ruleId}, CurrentVersionNo={oldVersionNo} 状态已变化，请刷新后重试");
                }
            }

            // 恢复历史版本为发布状态
            var rollbackVersion = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, rollbackVersionNo)
                ?? throw new BizException(
                    BizErrorCode.RollbackTargetNotAvailable,
                    409,
                    $"回滚目标版本不存在: RuleId={ruleId}, VersionNo={rollbackVersionNo}");
            var rollbackVersionUpdated = await _versionRepository.UpdateStatusAsync(
                rollbackVersion.VersionId,
                VersionStatusCodes.Published,
                VersionStatusCodes.Disabled);
            if (!rollbackVersionUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleVersionConcurrencyConflict,
                    409,
                    $"RuleId={ruleId}, RollbackVersionNo={rollbackVersionNo} 状态已变化，请刷新后重试");
            }

            currentHeader.CurrentVersion = rollbackVersionNo;
            var now = _clock.Now;
            currentHeader.Status = RuleStatusCodes.Published;
            currentHeader.IsEnabled = EnableFlag.Yes;
            currentHeader.UpdatedAt = now;
            var rollbackHeaderUpdated = await _headerRepository.UpdateAsync(
                currentHeader,
                RuleStatusCodes.Published);
            if (!rollbackHeaderUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleHeaderConcurrencyConflict,
                    409,
                    $"RuleId={ruleId} 主档状态已变化，请刷新后重试");
            }

            // 记录回滚流水和变更摘要
            await _publishRepository.InsertAsync(new RulePublish
            {
                PublishNo = $"RB-{ruleId}-{rollbackVersionNo}-{now:yyyyMMddHHmmss}",
                RuleId = ruleId,
                FromVersion = oldVersionNo,
                ToVersion = rollbackVersionNo,
                ActionType = ApprovalActionCodes.Rollback,
                PublishedBy = request.PublishedBy,
                PublishedAt = now,
                Remark = request.Remark
            });

            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = ruleId,
                VersionNo = rollbackVersionNo,
                ChangeType = ApprovalActionCodes.Rollback,
                ChangeSummary = $"从 V{oldVersionNo} 回滚到 V{rollbackVersionNo}",
                ChangedBy = request.PublishedBy,
                ChangedAt = now
            });
            await AddCacheInvalidationOutboxAsync(ruleId, rollbackVersionNo, ApprovalActionCodes.Rollback, now);
        });

        // ========== 第四阶段：清除生效规则缓存 ==========
        await InvalidateCachesAfterCommitAsync();

        var currentHeaderAfterRollback = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");
        _logger.LogInformation("回滚规则 RuleId={RuleId}, 从 V{FromVersion} 到 V{ToVersion}",
            ruleId, header.CurrentVersion, currentHeaderAfterRollback.CurrentVersion);
    }

    /// <summary>
    /// 解析回滚目标版本号。
    /// </summary>
    /// <remarks>
    /// 优先按发布流水追溯“当前版本之前最近一次激活的版本”，避免把一个从未发布过、但碰巧处于 DISABLED 的草稿版本误当成回滚目标。
    /// 当历史流水缺失时，再退回旧的版本状态兜底策略，兼容历史数据。
    /// </remarks>
    private async Task<int> ResolveRollbackVersionNoAsync(long ruleId, int currentVersionNo)
    {
        var publishHistory = await _publishRepository.GetByRuleIdAsync(ruleId);
        var activationHistory = publishHistory
            .Where(p => string.Equals(p.ActionType, ApprovalActionCodes.Publish, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(p.ActionType, ApprovalActionCodes.Rollback, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(p => p.PublishedAt)
            .ThenByDescending(p => p.PublishId)
            .ToList();

        if (activationHistory.Count > 0)
        {
            var currentActivationIndex = activationHistory.FindIndex(p => p.ToVersion == currentVersionNo);
            if (currentActivationIndex >= 0)
            {
                var previousActivation = activationHistory
                    .Skip(currentActivationIndex + 1)
                    .FirstOrDefault(p => p.ToVersion != currentVersionNo);
                if (previousActivation is not null)
                {
                    return previousActivation.ToVersion;
                }
            }
        }

        var versions = await _versionRepository.GetByRuleIdAsync(ruleId);
        var previousPublished = versions
            .Where(v => v.VersionNo < currentVersionNo && v.VersionStatus == VersionStatusCodes.Disabled)
            .OrderByDescending(v => v.VersionNo)
            .FirstOrDefault()
            ?? throw new BizException(
                BizErrorCode.RollbackTargetNotAvailable,
                409,
                "没有可回滚的历史版本");
        return previousPublished.VersionNo;
    }

    private async Task ValidatePublishConflictsAsync(RuleAggregate targetHeader, int targetVersionNo)
    {
        if (string.IsNullOrWhiteSpace(targetHeader.ItemCode))
        {
            return;
        }

        await _publishGuard.EnsureCanPublishAsync(targetHeader, targetVersionNo);
    }

    /// <summary>
    /// 清除生效规则缓存和互斥动作类型缓存。
    /// </summary>
    /// <remarks>
    /// 发布、停用、回滚操作后必须调用，确保计价引擎在下一次请求时读到最新规则集。
    /// IMemoryCache 不提供按前缀批量删除，因此移除已知 key。
    ///
    /// 同时清除互斥动作类型缓存，因为字典数据可能在发布周期内被修改。
    /// </remarks>
    private void ClearEffectiveCache()
    {
        _publishGuard.ClearCache();
        _cacheInvalidator.ClearEffectiveCache();
    }

    private async Task AddCacheInvalidationOutboxAsync(
        long ruleId,
        int versionNo,
        string operationType,
        DateTime now)
    {
        await AddCacheInvalidationOutboxAsync(
            CacheVersionSynchronizer.EffectiveRulesScope,
            ruleId,
            versionNo,
            operationType,
            now);
        await AddCacheInvalidationOutboxAsync(
            CacheVersionSynchronizer.ActionTypeOrderScope,
            ruleId,
            versionNo,
            operationType,
            now);
    }

    private async Task AddCacheInvalidationOutboxAsync(
        string cacheScope,
        long ruleId,
        int versionNo,
        string operationType,
        DateTime now)
    {
        await _cacheInvalidationOutboxRepository.InsertAsync(new RuleCacheInvalidationOutbox
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

    private async Task InvalidateCachesAfterCommitAsync()
    {
        ClearEffectiveCache();
        await _cacheInvalidationOutboxProcessor.ProcessPendingAsync();
    }

    /// <summary>
    /// 将发布流水实体映射为接口响应。
    /// </summary>
    /// <param name="entity">发布流水实体。</param>
    /// <returns>发布流水响应 DTO。</returns>
    private static RulePublishResponse MapPublishToResponse(RulePublish entity)
    {
        return new RulePublishResponse
        {
            PublishId = entity.PublishId,
            PublishNo = entity.PublishNo,
            RuleId = entity.RuleId,
            FromVersion = entity.FromVersion,
            ToVersion = entity.ToVersion,
            ActionType = entity.ActionType,
            PublishedBy = entity.PublishedBy,
            PublishedAt = entity.PublishedAt,
            Remark = entity.Remark
        };
    }

    /// <summary>
    /// 将规则变更日志实体映射为接口响应。
    /// </summary>
    /// <param name="entity">规则变更日志实体。</param>
    /// <returns>规则变更日志响应 DTO。</returns>
    private static RuleChangeLogResponse MapChangeLogToResponse(RuleChangeLog entity)
    {
        return new RuleChangeLogResponse
        {
            ChangeId = entity.ChangeId,
            RuleId = entity.RuleId,
            VersionNo = entity.VersionNo,
            ChangeType = entity.ChangeType,
            ChangeSummary = entity.ChangeSummary,
            ChangedBy = entity.ChangedBy,
            ChangedAt = entity.ChangedAt,
            SourceSystem = entity.SourceSystem
        };
    }

}
