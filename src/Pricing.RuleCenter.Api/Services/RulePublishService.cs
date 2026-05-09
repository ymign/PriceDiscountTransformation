using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

public sealed class RulePublishService
{
    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IRuleVersionRepository _versionRepository;
    private readonly IRulePublishRepository _publishRepository;
    private readonly IRuleChangeLogRepository _changeLogRepository;
    private readonly ILogger<RulePublishService> _logger;

    public RulePublishService(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRulePublishRepository publishRepository,
        IRuleChangeLogRepository changeLogRepository,
        ILogger<RulePublishService> logger)
    {
        _headerRepository = headerRepository;
        _versionRepository = versionRepository;
        _publishRepository = publishRepository;
        _changeLogRepository = changeLogRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RulePublishResponse>> GetPublishHistoryAsync(long ruleId)
    {
        var items = await _publishRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapPublishToResponse).ToList();
    }

    public async Task<IReadOnlyList<RuleChangeLogResponse>> GetChangeLogsAsync(long ruleId)
    {
        var items = await _changeLogRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapChangeLogToResponse).ToList();
    }

    public async Task PublishAsync(long ruleId, RulePublishRequest request)
    {
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");

        var version = await _versionRepository.GetByRuleAndVersionAsync(ruleId, request.VersionNo)
            ?? throw new KeyNotFoundException($"规则版本不存在: RuleId={ruleId}, VersionNo={request.VersionNo}");

        if (version.VersionStatus != "DRAFT")
        {
            throw new InvalidOperationException($"只有草稿版本可以发布, 当前状态: {version.VersionStatus}");
        }

        var oldVersion = header.CurrentVersion;

        if (oldVersion > 0)
        {
            await _versionRepository.UpdateStatusAsync(
                (await _versionRepository.GetByRuleAndVersionAsync(ruleId, oldVersion))!.VersionId,
                "DISABLED");
        }

        await _versionRepository.UpdateStatusAsync(version.VersionId, "PUBLISHED");

        header.CurrentVersion = request.VersionNo;
        header.Status = "PUBLISHED";
        header.UpdatedAt = DateTime.Now;
        await _headerRepository.UpdateAsync(header);

        var publishNo = $"PUB-{ruleId}-{request.VersionNo}-{DateTime.Now:yyyyMMddHHmmss}";
        await _publishRepository.InsertAsync(new RulePublish
        {
            PublishNo = publishNo,
            RuleId = ruleId,
            FromVersion = oldVersion > 0 ? oldVersion : null,
            ToVersion = request.VersionNo,
            ActionType = "PUBLISH",
            PublishedBy = request.PublishedBy,
            PublishedAt = DateTime.Now,
            Remark = request.Remark
        });

        await _changeLogRepository.InsertAsync(new RuleChangeLog
        {
            RuleId = ruleId,
            VersionNo = request.VersionNo,
            ChangeType = "PUBLISH",
            ChangeSummary = $"发布版本 V{request.VersionNo}",
            ChangedBy = request.PublishedBy,
            ChangedAt = DateTime.Now
        });

        _logger.LogInformation("发布规则 RuleId={RuleId}, VersionNo={VersionNo}", ruleId, request.VersionNo);
    }

    public async Task DisableAsync(long ruleId, RuleDisableRequest request)
    {
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");

        if (header.Status != "PUBLISHED")
        {
            throw new InvalidOperationException($"只有已发布的规则可以停用, 当前状态: {header.Status}");
        }

        if (header.CurrentVersion > 0)
        {
            var currentVersion = await _versionRepository.GetByRuleAndVersionAsync(ruleId, header.CurrentVersion);
            if (currentVersion is not null)
            {
                await _versionRepository.UpdateStatusAsync(currentVersion.VersionId, "DISABLED");
            }
        }

        header.Status = "DISABLED";
        header.IsEnabled = "N";
        header.UpdatedAt = DateTime.Now;
        await _headerRepository.UpdateAsync(header);

        await _publishRepository.InsertAsync(new RulePublish
        {
            PublishNo = $"DIS-{ruleId}-{DateTime.Now:yyyyMMddHHmmss}",
            RuleId = ruleId,
            FromVersion = header.CurrentVersion,
            ToVersion = header.CurrentVersion,
            ActionType = "DISABLE",
            PublishedBy = request.PublishedBy,
            PublishedAt = DateTime.Now,
            Remark = request.Remark
        });

        await _changeLogRepository.InsertAsync(new RuleChangeLog
        {
            RuleId = ruleId,
            VersionNo = header.CurrentVersion,
            ChangeType = "DISABLE",
            ChangeSummary = $"停用规则, 当前版本 V{header.CurrentVersion}",
            ChangedBy = request.PublishedBy,
            ChangedAt = DateTime.Now
        });

        _logger.LogInformation("停用规则 RuleId={RuleId}", ruleId);
    }

    public async Task RollbackAsync(long ruleId, RuleRollbackRequest request)
    {
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");

        if (header.Status != "PUBLISHED")
        {
            throw new InvalidOperationException($"只有已发布的规则可以回滚, 当前状态: {header.Status}");
        }

        var versions = await _versionRepository.GetByRuleIdAsync(ruleId);
        var previousPublished = versions
            .Where(v => v.VersionNo < header.CurrentVersion && v.VersionStatus == "DISABLED")
            .OrderByDescending(v => v.VersionNo)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("没有可回滚的历史版本");

        var currentVersion = await _versionRepository.GetByRuleAndVersionAsync(ruleId, header.CurrentVersion);
        if (currentVersion is not null)
        {
            await _versionRepository.UpdateStatusAsync(currentVersion.VersionId, "ROLLED_BACK");
        }

        await _versionRepository.UpdateStatusAsync(previousPublished.VersionId, "PUBLISHED");

        var oldVersionNo = header.CurrentVersion;
        header.CurrentVersion = previousPublished.VersionNo;
        header.UpdatedAt = DateTime.Now;
        await _headerRepository.UpdateAsync(header);

        await _publishRepository.InsertAsync(new RulePublish
        {
            PublishNo = $"RB-{ruleId}-{previousPublished.VersionNo}-{DateTime.Now:yyyyMMddHHmmss}",
            RuleId = ruleId,
            FromVersion = oldVersionNo,
            ToVersion = previousPublished.VersionNo,
            ActionType = "ROLLBACK",
            PublishedBy = request.PublishedBy,
            PublishedAt = DateTime.Now,
            Remark = request.Remark
        });

        await _changeLogRepository.InsertAsync(new RuleChangeLog
        {
            RuleId = ruleId,
            VersionNo = previousPublished.VersionNo,
            ChangeType = "ROLLBACK",
            ChangeSummary = $"从 V{oldVersionNo} 回滚到 V{previousPublished.VersionNo}",
            ChangedBy = request.PublishedBy,
            ChangedAt = DateTime.Now
        });

        _logger.LogInformation("回滚规则 RuleId={RuleId}, 从 V{FromVersion} 到 V{ToVersion}",
            ruleId, oldVersionNo, previousPublished.VersionNo);
    }

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
