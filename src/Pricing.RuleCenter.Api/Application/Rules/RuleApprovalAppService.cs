using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Api.Application.Rules;

/// <summary>
/// 规则审批应用服务，负责提交审批、审核通过和驳回。
/// </summary>
public sealed class RuleApprovalAppService
{
    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IRuleVersionRepository _versionRepository;
    private readonly IRuleApprovalRepository _approvalRepository;
    private readonly IRuleChangeLogRepository _changeLogRepository;
    private readonly ILogger<RuleApprovalAppService> _logger;

    public RuleApprovalAppService(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRuleApprovalRepository approvalRepository,
        IRuleChangeLogRepository changeLogRepository,
        ILogger<RuleApprovalAppService> logger)
    {
        _headerRepository = headerRepository;
        _versionRepository = versionRepository;
        _approvalRepository = approvalRepository;
        _changeLogRepository = changeLogRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RuleApprovalResponse>> GetByRuleIdAsync(long ruleId)
    {
        var items = await _approvalRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapToResponse).ToList();
    }

    public async Task<long> SubmitAsync(long ruleId, int versionNo, RuleApprovalSubmitRequest request)
    {
        await EnsureRuleAndVersionExistAsync(ruleId, versionNo);

        var approvals = await _approvalRepository.GetByRuleIdAsync(ruleId);
        var normalizedActionType = NormalizeActionType(request.ActionType);
        var latest = approvals
            .Where(a => a.VersionNo == versionNo)
            .Where(a => string.Equals(a.ActionType, normalizedActionType, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(a => a.SubmittedAt)
            .ThenByDescending(a => a.ApprovalId)
            .FirstOrDefault();

        if (latest is not null &&
            string.Equals(latest.ApprovalStatus, "PENDING", StringComparison.OrdinalIgnoreCase))
        {
            throw new BizException(
                BizErrorCode.ResourceAlreadyExists,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo}, ActionType={normalizedActionType} 已存在待审核记录");
        }

        var now = DateTime.Now;
        var approval = new RuleApproval
        {
            RuleId = ruleId,
            VersionNo = versionNo,
            ActionType = normalizedActionType,
            ApprovalStatus = "PENDING",
            SubmittedBy = request.SubmittedBy.Trim(),
            SubmittedAt = now,
            ReviewComment = request.Remark
        };
        var approvalId = await _approvalRepository.InsertAsync(approval);

        await TryWriteChangeLogAsync(new RuleChangeLog
        {
            RuleId = ruleId,
            VersionNo = versionNo,
            ChangeType = "SUBMIT_APPROVAL",
            ChangeSummary = $"提交{normalizedActionType}审批",
            ChangedBy = request.SubmittedBy.Trim(),
            ChangedAt = now,
            SourceSystem = "API"
        });

        _logger.LogInformation(
            "提交规则审批 RuleId={RuleId}, VersionNo={VersionNo}, ActionType={ActionType}, ApprovalId={ApprovalId}",
            ruleId, versionNo, normalizedActionType, approvalId);

        return approvalId;
    }

    public async Task ApproveAsync(long ruleId, int versionNo, RuleApprovalDecisionRequest request)
    {
        var pending = await GetLatestPendingApprovalAsync(ruleId, versionNo);
        await _approvalRepository.UpdateStatusAsync(
            pending.ApprovalId,
            "APPROVED",
            request.ReviewedBy.Trim(),
            request.ReviewComment ?? string.Empty);

        pending.ReviewedAt = DateTime.Now;
        await TryWriteChangeLogAsync(new RuleChangeLog
        {
            RuleId = ruleId,
            VersionNo = versionNo,
            ChangeType = "APPROVE",
            ChangeSummary = $"审批通过：{pending.ActionType}",
            ChangedBy = request.ReviewedBy.Trim(),
            ChangedAt = pending.ReviewedAt.Value,
            SourceSystem = "API"
        });
    }

    public async Task RejectAsync(long ruleId, int versionNo, RuleApprovalDecisionRequest request)
    {
        var pending = await GetLatestPendingApprovalAsync(ruleId, versionNo);
        await _approvalRepository.UpdateStatusAsync(
            pending.ApprovalId,
            "REJECTED",
            request.ReviewedBy.Trim(),
            request.ReviewComment ?? string.Empty);

        pending.ReviewedAt = DateTime.Now;
        await TryWriteChangeLogAsync(new RuleChangeLog
        {
            RuleId = ruleId,
            VersionNo = versionNo,
            ChangeType = "REJECT",
            ChangeSummary = $"审批驳回：{pending.ActionType}",
            ChangedBy = request.ReviewedBy.Trim(),
            ChangedAt = pending.ReviewedAt.Value,
            SourceSystem = "API"
        });
    }

    private async Task EnsureRuleAndVersionExistAsync(long ruleId, int versionNo)
    {
        _ = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");
        _ = await _versionRepository.GetByRuleAndVersionAsync(ruleId, versionNo)
            ?? throw new BizException(
                BizErrorCode.RuleVersionNotFound,
                404,
                $"规则版本不存在: RuleId={ruleId}, VersionNo={versionNo}");
    }

    private async Task<RuleApproval> GetLatestPendingApprovalAsync(long ruleId, int versionNo)
    {
        await EnsureRuleAndVersionExistAsync(ruleId, versionNo);
        return (await _approvalRepository.GetByRuleIdAsync(ruleId))
            .Where(a => a.VersionNo == versionNo)
            .Where(a => string.Equals(a.ApprovalStatus, "PENDING", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(a => a.SubmittedAt)
            .ThenByDescending(a => a.ApprovalId)
            .FirstOrDefault()
            ?? throw new BizException(
                BizErrorCode.ApprovalRequired,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo} 不存在待审核记录");
    }

    private async Task TryWriteChangeLogAsync(RuleChangeLog entity)
    {
        try
        {
            await _changeLogRepository.InsertAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "写入审批变更日志失败 RuleId={RuleId}, ChangeType={ChangeType}", entity.RuleId, entity.ChangeType);
        }
    }

    private static RuleApprovalResponse MapToResponse(RuleApproval entity)
    {
        return new RuleApprovalResponse
        {
            ApprovalId = entity.ApprovalId,
            RuleId = entity.RuleId,
            VersionNo = entity.VersionNo,
            ActionType = entity.ActionType,
            ApprovalStatus = entity.ApprovalStatus,
            SubmittedBy = entity.SubmittedBy,
            SubmittedAt = entity.SubmittedAt,
            ReviewedBy = entity.ReviewedBy,
            ReviewedAt = entity.ReviewedAt,
            ReviewComment = entity.ReviewComment
        };
    }

    private static string NormalizeActionType(string actionType)
    {
        var normalized = actionType?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new BizException(BizErrorCode.ApprovalRequired, 400, "审批操作类型不能为空");
        }

        return normalized;
    }
}
