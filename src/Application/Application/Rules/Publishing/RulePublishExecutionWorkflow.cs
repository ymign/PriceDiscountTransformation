using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 发布规则事务主流程 workflow。
/// </summary>
public sealed class RulePublishExecutionWorkflow
{
    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IRuleVersionRepository _versionRepository;
    private readonly IRulePublishRepository _publishRepository;
    private readonly IRuleChangeLogRepository _changeLogRepository;
    private readonly RulePublishTransactionWriter _transactionWriter;
    private readonly RulePublishCacheCoordinator _cacheCoordinator;
    private readonly IClock _clock;
    private readonly ILogger _logger;

    /// <summary>
    /// 初始化发布规则事务主流程 workflow。
    /// </summary>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="publishRepository">发布流水仓储。</param>
    /// <param name="changeLogRepository">变更日志仓储。</param>
    /// <param name="transactionWriter">事务执行器。</param>
    /// <param name="cacheCoordinator">缓存失效协调器。</param>
    /// <param name="clock">技术时间提供者。</param>
    /// <param name="logger">日志组件。</param>
    public RulePublishExecutionWorkflow(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRulePublishRepository publishRepository,
        IRuleChangeLogRepository changeLogRepository,
        RulePublishTransactionWriter transactionWriter,
        RulePublishCacheCoordinator cacheCoordinator,
        IClock clock,
        ILogger logger)
    {
        _headerRepository = headerRepository;
        _versionRepository = versionRepository;
        _publishRepository = publishRepository;
        _changeLogRepository = changeLogRepository;
        _transactionWriter = transactionWriter;
        _cacheCoordinator = cacheCoordinator;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行发布规则事务主流程。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">发布请求。</param>
    public async Task ExecuteAsync(long ruleId, RulePublishRequest request)
    {
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

            if (oldVersion > 0)
            {
                var oldVersionEntity = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, oldVersion);
                if (oldVersionEntity is null)
                {
                    _logger.LogWarning("发布规则时未找到旧版本记录 RuleId={RuleId}, OldVersion={OldVersion}", ruleId, oldVersion);
                }
                else if (string.Equals(oldVersionEntity.VersionStatus, VersionStatusCodes.Published, StringComparison.OrdinalIgnoreCase))
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
            currentHeader.IsEnabled = EnableFlag.Yes;
            currentHeader.UpdatedAt = now;
            var publishHeaderUpdated = await _headerRepository.UpdateAsync(currentHeader, previousHeaderStatus);
            if (!publishHeaderUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleHeaderConcurrencyConflict,
                    409,
                    $"RuleId={ruleId} 主档状态已变化，请刷新后重试");
            }

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

            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = ruleId,
                VersionNo = request.VersionNo,
                ChangeType = ApprovalActionCodes.Publish,
                ChangeSummary = $"发布版本 V{request.VersionNo}",
                ChangedBy = request.PublishedBy,
                ChangedAt = now
            });

            await _cacheCoordinator.EnqueueAsync(ruleId, request.VersionNo, ApprovalActionCodes.Publish, now);
        });
    }
}
