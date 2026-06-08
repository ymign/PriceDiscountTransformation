using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 回滚规则事务主流程 workflow。
/// </summary>
public sealed class RuleRollbackExecutionWorkflow
{
    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IRuleVersionRepository _versionRepository;
    private readonly IRulePublishRepository _publishRepository;
    private readonly IRuleChangeLogRepository _changeLogRepository;
    private readonly RulePublishTransactionWriter _transactionWriter;
    private readonly RulePublishCacheCoordinator _cacheCoordinator;
    private readonly RuleRollbackTargetResolver _rollbackTargetResolver;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化回滚规则事务主流程 workflow。
    /// </summary>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="publishRepository">发布流水仓储。</param>
    /// <param name="changeLogRepository">变更日志仓储。</param>
    /// <param name="transactionWriter">事务执行器。</param>
    /// <param name="cacheCoordinator">缓存失效协调器。</param>
    /// <param name="rollbackTargetResolver">回滚目标解析器。</param>
    /// <param name="clock">技术时间提供者。</param>
    public RuleRollbackExecutionWorkflow(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRulePublishRepository publishRepository,
        IRuleChangeLogRepository changeLogRepository,
        RulePublishTransactionWriter transactionWriter,
        RulePublishCacheCoordinator cacheCoordinator,
        RuleRollbackTargetResolver rollbackTargetResolver,
        IClock clock)
    {
        _headerRepository = headerRepository;
        _versionRepository = versionRepository;
        _publishRepository = publishRepository;
        _changeLogRepository = changeLogRepository;
        _transactionWriter = transactionWriter;
        _cacheCoordinator = cacheCoordinator;
        _rollbackTargetResolver = rollbackTargetResolver;
        _clock = clock;
    }

    /// <summary>
    /// 执行回滚规则事务主流程。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">回滚请求。</param>
    public async Task ExecuteAsync(long ruleId, RuleRollbackRequest request)
    {
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
            var rollbackVersionNo = await _rollbackTargetResolver.ResolveAsync(ruleId, oldVersionNo);

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
            var rollbackHeaderUpdated = await _headerRepository.UpdateAsync(currentHeader, RuleStatusCodes.Published);
            if (!rollbackHeaderUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleHeaderConcurrencyConflict,
                    409,
                    $"RuleId={ruleId} 主档状态已变化，请刷新后重试");
            }

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

            await _cacheCoordinator.EnqueueAsync(ruleId, rollbackVersionNo, ApprovalActionCodes.Rollback, now);
        });
    }
}
