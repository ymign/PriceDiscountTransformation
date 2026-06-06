using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules.Guards;

/// <summary>
/// 规则生命周期操作审批门禁。
/// </summary>
public sealed class RuleApprovalGate
{
    private readonly IRuleApprovalRepository _approvalRepository;
    private readonly IRuleChangeLogRepository _changeLogRepository;

    private static readonly HashSet<string> DraftChangeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "SAVE_CONDITIONS",
        "SAVE_ACTIONS",
        "UPDATE_RULE"
    };

    /// <summary>
    /// 初始化审批门禁。
    /// </summary>
    public RuleApprovalGate(
        IRuleApprovalRepository approvalRepository,
        IRuleChangeLogRepository changeLogRepository)
    {
        _approvalRepository = approvalRepository;
        _changeLogRepository = changeLogRepository;
    }

    /// <summary>
    /// 确认指定生命周期操作存在最新有效的审批通过记录。
    /// </summary>
    public async Task EnsurePassedAsync(long ruleId, int? versionNo, string actionType)
    {
        var approvals = (await _approvalRepository.GetByRuleIdAsync(ruleId))
            .Where(a => a.VersionNo == versionNo)
            .Where(a => string.Equals(a.ActionType, actionType, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(a => a.ReviewedAt ?? DateTime.MinValue)
            .ThenByDescending(a => a.SubmittedAt)
            .ThenByDescending(a => a.ApprovalId)
            .ToList();

        var latestApproval = approvals.FirstOrDefault();
        if (latestApproval is null)
        {
            throw new BizException(
                BizErrorCode.ApprovalRequired,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo}, ActionType={actionType} 缺少审批通过记录");
        }

        if (string.Equals(latestApproval.ApprovalStatus, ApprovalStatusCodes.Rejected, StringComparison.OrdinalIgnoreCase))
        {
            throw new BizException(
                BizErrorCode.ApprovalRejected,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo}, ActionType={actionType} 最近一次审批已驳回");
        }

        if (!string.Equals(latestApproval.ApprovalStatus, ApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase) ||
            latestApproval.ReviewedAt is null)
        {
            throw new BizException(
                BizErrorCode.ApprovalRequired,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo}, ActionType={actionType} 尚未审批通过");
        }

        var latestDraftChangeTime = await GetLatestDraftChangeTimeAsync(ruleId, versionNo);
        if (latestDraftChangeTime.HasValue && latestDraftChangeTime.Value > latestApproval.ReviewedAt.Value)
        {
            throw new BizException(
                BizErrorCode.ApprovalOutdated,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo}, ActionType={actionType} 审批后规则又被修改，请重新提审");
        }
    }

    private async Task<DateTime?> GetLatestDraftChangeTimeAsync(long ruleId, int? versionNo)
    {
        var changeLogs = await _changeLogRepository.GetByRuleIdAsync(ruleId);
        return changeLogs
            .Where(log => log.VersionNo == versionNo)
            .Where(log => DraftChangeTypes.Contains(log.ChangeType ?? string.Empty))
            .Select(log => (DateTime?)log.ChangedAt)
            .OrderByDescending(time => time)
            .FirstOrDefault();
    }
}
