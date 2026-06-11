using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 停用规则事务主流程 workflow。
/// </summary>
internal sealed class RuleDisableExecutionWorkflow
{
    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IRuleVersionRepository _versionRepository;
    private readonly IRulePublishRepository _publishRepository;
    private readonly IRuleChangeLogRepository _changeLogRepository;
    private readonly RulePublishTransactionWriter _transactionWriter;
    private readonly RulePublishCacheCoordinator _cacheCoordinator;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化停用规则事务主流程 workflow。
    /// </summary>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="publishRepository">发布流水仓储。</param>
    /// <param name="changeLogRepository">变更日志仓储。</param>
    /// <param name="transactionWriter">事务执行器。</param>
    /// <param name="cacheCoordinator">缓存失效协调器。</param>
    /// <param name="clock">技术时间提供者。</param>
    public RuleDisableExecutionWorkflow(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRulePublishRepository publishRepository,
        IRuleChangeLogRepository changeLogRepository,
        RulePublishTransactionWriter transactionWriter,
        RulePublishCacheCoordinator cacheCoordinator,
        IClock clock)
    {
        _headerRepository = headerRepository;
        _versionRepository = versionRepository;
        _publishRepository = publishRepository;
        _changeLogRepository = changeLogRepository;
        _transactionWriter = transactionWriter;
        _cacheCoordinator = cacheCoordinator;
        _clock = clock;
    }

    /// <summary>
    /// 执行停用规则事务主流程。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">停用请求。</param>
    public async Task ExecuteAsync(long ruleId, RuleDisableRequest request)
    {
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

            var now = _clock.Now;
            currentHeader.Status = RuleStatusCodes.Disabled;
            currentHeader.IsEnabled = EnableFlag.No;
            currentHeader.UpdatedAt = now;
            var disableHeaderUpdated = await _headerRepository.UpdateAsync(currentHeader, RuleStatusCodes.Published);
            if (!disableHeaderUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleHeaderConcurrencyConflict,
                    409,
                    $"RuleId={ruleId} 主档状态已变化，请刷新后重试");
            }

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

            await _cacheCoordinator.EnqueueAsync(
                ruleId,
                currentHeader.CurrentVersion,
                ApprovalActionCodes.Disable,
                now);
        });
    }
}
