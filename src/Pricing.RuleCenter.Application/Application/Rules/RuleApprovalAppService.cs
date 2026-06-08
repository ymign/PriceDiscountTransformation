using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules;

/// <summary>
/// 规则审批应用服务，负责提交审批、审核通过和驳回。
/// </summary>
public sealed class RuleApprovalAppService
{
    private static readonly HashSet<string> SupportedActionTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ApprovalActionCodes.Publish,
        ApprovalActionCodes.Disable,
        ApprovalActionCodes.Rollback
    };

    private readonly IRuleHeaderRepository _headerRepository;
    private readonly IRuleVersionRepository _versionRepository;
    private readonly IRuleApprovalRepository _approvalRepository;
    private readonly IRuleChangeLogRepository _changeLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly ILogger<RuleApprovalAppService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleApprovalAppService"/> class.
    /// </summary>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="approvalRepository">规则审批仓储。</param>
    /// <param name="changeLogRepository">规则变更日志仓储。</param>
    /// <param name="unitOfWork">工作单元，用于发布审批提交时保护版本状态。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    /// <param name="logger">日志对象。</param>
    public RuleApprovalAppService(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRuleApprovalRepository approvalRepository,
        IRuleChangeLogRepository changeLogRepository,
        IUnitOfWork unitOfWork,
        IClock clock,
        ILogger<RuleApprovalAppService> logger)
    {
        _headerRepository = headerRepository;
        _versionRepository = versionRepository;
        _approvalRepository = approvalRepository;
        _changeLogRepository = changeLogRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 查询指定规则的审批记录。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>审批记录响应列表。</returns>
    public async Task<IReadOnlyList<RuleApprovalResponse>> GetByRuleIdAsync(long ruleId)
    {
        var items = await _approvalRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapToResponse).ToList();
    }

    /// <summary>
    /// 提交指定规则版本的审批申请。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">审批提交请求。</param>
    /// <returns>新建审批记录主键。</returns>
    public async Task<long> SubmitAsync(long ruleId, int versionNo, RuleApprovalSubmitRequest request)
    {
        var normalizedActionType = NormalizeActionType(request.ActionType);
        return string.Equals(normalizedActionType, ApprovalActionCodes.Publish, StringComparison.OrdinalIgnoreCase)
            ? await SubmitPublishApprovalWithVersionLockAsync(ruleId, versionNo, request, normalizedActionType)
            : await SubmitApprovalAsync(ruleId, versionNo, request, normalizedActionType);
    }

    private async Task<long> SubmitApprovalAsync(
        long ruleId,
        int versionNo,
        RuleApprovalSubmitRequest request,
        string normalizedActionType)
    {
        var (header, version) = await EnsureRuleAndVersionExistAsync(ruleId, versionNo);
        ValidateApprovalActionState(header, version, normalizedActionType);
        return await InsertPendingApprovalAsync(ruleId, versionNo, request, normalizedActionType);
    }

    private async Task<long> SubmitPublishApprovalWithVersionLockAsync(
        long ruleId,
        int versionNo,
        RuleApprovalSubmitRequest request,
        string normalizedActionType)
    {
        await _unitOfWork.BeginAsync();
        try
        {
            var header = await _headerRepository.GetByIdAsync(ruleId)
                ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");
            var version = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, versionNo)
                ?? throw new BizException(
                    BizErrorCode.RuleVersionNotFound,
                    404,
                    $"规则版本不存在: RuleId={ruleId}, VersionNo={versionNo}");

            ValidateApprovalActionState(header, version, normalizedActionType);

            var approvalId = await InsertPendingApprovalAsync(ruleId, versionNo, request, normalizedActionType);
            await _unitOfWork.CommitAsync();
            return approvalId;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task<long> InsertPendingApprovalAsync(
        long ruleId,
        int versionNo,
        RuleApprovalSubmitRequest request,
        string normalizedActionType)
    {
        var approvals = await _approvalRepository.GetByRuleIdAsync(ruleId);

        var latest = approvals
            .Where(a => a.VersionNo == versionNo)
            .Where(a => string.Equals(a.ActionType, normalizedActionType, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(a => a.SubmittedAt)
            .ThenByDescending(a => a.ApprovalId)
            .FirstOrDefault();

        if (latest is not null &&
            string.Equals(latest.ApprovalStatus, ApprovalStatusCodes.Pending, StringComparison.OrdinalIgnoreCase))
        {
            throw new BizException(
                BizErrorCode.ResourceAlreadyExists,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo}, ActionType={normalizedActionType} 已存在待审核记录");
        }

        var now = _clock.Now;
        var approval = new RuleApproval
        {
            RuleId = ruleId,
            VersionNo = versionNo,
            ActionType = normalizedActionType,
            ApprovalStatus = ApprovalStatusCodes.Pending,
            SubmittedBy = request.SubmittedBy.Trim(),
            SubmittedAt = now,
            ReviewComment = request.Remark
        };
        long approvalId;
        try
        {
            approvalId = await _approvalRepository.InsertAsync(approval);
        }
        catch (Exception ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new BizException(
                BizErrorCode.ResourceAlreadyExists,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo}, ActionType={normalizedActionType} 已存在待审核记录");
        }

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

    /// <summary>
    /// 审批通过指定规则版本的待审申请。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">审批决策请求。</param>
    /// <returns>表示异步审批操作的任务。</returns>
    public async Task ApproveAsync(long ruleId, int versionNo, RuleApprovalDecisionRequest request)
    {
        var pending = await GetLatestPendingApprovalAsync(ruleId, versionNo, request.ActionType);
        var updated = await _approvalRepository.UpdateStatusAsync(
            pending.ApprovalId,
            ApprovalStatusCodes.Approved,
            request.ReviewedBy.Trim(),
            request.ReviewComment ?? string.Empty,
            ApprovalStatusCodes.Pending);
        if (!updated)
        {
            throw new BizException(
                BizErrorCode.ConcurrencyConflict,
                409,
                $"ApprovalId={pending.ApprovalId} 状态已变化，请刷新后重试");
        }

        pending.ReviewedAt = _clock.Now;
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

    /// <summary>
    /// 驳回指定规则版本的待审申请。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">审批决策请求。</param>
    /// <returns>表示异步驳回操作的任务。</returns>
    public async Task RejectAsync(long ruleId, int versionNo, RuleApprovalDecisionRequest request)
    {
        var pending = await GetLatestPendingApprovalAsync(ruleId, versionNo, request.ActionType);
        var updated = await _approvalRepository.UpdateStatusAsync(
            pending.ApprovalId,
            ApprovalStatusCodes.Rejected,
            request.ReviewedBy.Trim(),
            request.ReviewComment ?? string.Empty,
            ApprovalStatusCodes.Pending);
        if (!updated)
        {
            throw new BizException(
                BizErrorCode.ConcurrencyConflict,
                409,
                $"ApprovalId={pending.ApprovalId} 状态已变化，请刷新后重试");
        }

        pending.ReviewedAt = _clock.Now;
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

    private async Task<(RuleAggregate Header, RuleVersion Version)> EnsureRuleAndVersionExistAsync(long ruleId, int versionNo)
    {
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");
        var version = await _versionRepository.GetByRuleAndVersionAsync(ruleId, versionNo)
            ?? throw new BizException(
                BizErrorCode.RuleVersionNotFound,
                404,
                $"规则版本不存在: RuleId={ruleId}, VersionNo={versionNo}");

        return (header, version);
    }

    private async Task<RuleApproval> GetLatestPendingApprovalAsync(long ruleId, int versionNo, string actionType)
    {
        await EnsureRuleAndVersionExistAsync(ruleId, versionNo);
        var normalizedActionType = NormalizeActionType(actionType);
        return (await _approvalRepository.GetByRuleIdAsync(ruleId))
            .Where(a => a.VersionNo == versionNo)
            .Where(a => string.Equals(a.ActionType, normalizedActionType, StringComparison.OrdinalIgnoreCase))
            .Where(a => string.Equals(a.ApprovalStatus, ApprovalStatusCodes.Pending, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(a => a.SubmittedAt)
            .ThenByDescending(a => a.ApprovalId)
            .FirstOrDefault()
            ?? throw new BizException(
                BizErrorCode.ApprovalRequired,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo}, ActionType={normalizedActionType} 不存在待审核记录");
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
            throw new BizException(BizErrorCode.ApprovalActionInvalid, 400, "审批操作类型不能为空");
        }

        if (!SupportedActionTypes.Contains(normalized))
        {
            throw new BizException(
                BizErrorCode.ApprovalActionInvalid,
                400,
                $"不支持的审批操作类型: {normalized}");
        }

        return normalized;
    }

    private static void ValidateApprovalActionState(RuleAggregate header, RuleVersion version, string actionType)
    {
        switch (actionType)
        {
            case ApprovalActionCodes.Publish:
                if (!string.Equals(version.VersionStatus, VersionStatusCodes.Draft, StringComparison.OrdinalIgnoreCase))
                {
                    throw new BizException(
                        BizErrorCode.VersionStatusNotAllowed,
                        409,
                        $"只有草稿版本可以提交发布审批, 当前状态: {version.VersionStatus}");
                }
                break;

            case ApprovalActionCodes.Disable:
                if (!string.Equals(header.Status, RuleStatusCodes.Published, StringComparison.OrdinalIgnoreCase) ||
                    header.CurrentVersion != version.VersionNo)
                {
                    throw new BizException(
                        BizErrorCode.VersionStatusNotAllowed,
                        409,
                        $"只有当前已发布版本可以提交停用审批, 当前主档状态: {header.Status}, CurrentVersion={header.CurrentVersion}");
                }
                break;

            case ApprovalActionCodes.Rollback:
                if (!string.Equals(header.Status, RuleStatusCodes.Published, StringComparison.OrdinalIgnoreCase) ||
                    header.CurrentVersion != version.VersionNo)
                {
                    throw new BizException(
                        BizErrorCode.VersionStatusNotAllowed,
                        409,
                        $"只有当前已发布版本可以提交回滚审批, 当前主档状态: {header.Status}, CurrentVersion={header.CurrentVersion}");
                }
                break;
        }
    }

    private static bool IsUniqueConstraintViolation(Exception ex)
    {
        return ex.Message.Contains("ORA-00001", StringComparison.OrdinalIgnoreCase) ||
               ex.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase);
    }
}
