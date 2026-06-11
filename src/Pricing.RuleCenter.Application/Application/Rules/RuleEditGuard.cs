using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules;

/// <summary>
/// 规则编辑保护器：发布审批待处理期间冻结会改变规则语义的编辑入口。
/// </summary>
public sealed class RuleEditGuard
{
    private const string SubmitApproval = "SUBMIT_APPROVAL";
    private const string Approve = "APPROVE";
    private const string Reject = "REJECT";
    private const string Publish = "PUBLISH";

    private readonly IRuleChangeLogRepository _changeLogRepository;

    /// <summary>
    /// 初始化规则编辑保护器。
    /// </summary>
    /// <param name="changeLogRepository">规则变更日志仓储，用于识别待处理发布审批。</param>
    public RuleEditGuard(IRuleChangeLogRepository changeLogRepository)
    {
        _changeLogRepository = changeLogRepository;
    }

    /// <summary>
    /// 确保指定规则或版本没有处于待处理发布审批状态。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号；为空时检查该规则所有版本。</param>
    /// <returns>表示异步校验操作的任务。</returns>
    public async Task EnsureNoPendingPublishApprovalAsync(long ruleId, int? versionNo = null)
    {
        var logs = await _changeLogRepository.GetByRuleIdAsync(ruleId);
        var latestApprovalLogs = logs
            .Where(log => IsPublishApprovalLog(log) && IsVersionMatch(log, versionNo))
            .GroupBy(log => log.VersionNo ?? 0)
            .Select(group => group
                .OrderByDescending(log => log.ChangedAt)
                .ThenByDescending(log => log.ChangeId)
                .First())
            .ToList();

        if (latestApprovalLogs.Any(IsSubmitApproval))
        {
            var blockedVersion = latestApprovalLogs
                .Where(IsSubmitApproval)
                .Select(log => log.VersionNo)
                .FirstOrDefault();
            throw new BizException(
                BizErrorCode.ApprovalPendingEditNotAllowed,
                409,
                $"RuleId={ruleId}, VersionNo={blockedVersion} 已提交发布审批，审批完成前不允许继续编辑");
        }
    }

    private static bool IsVersionMatch(RuleChangeLog log, int? versionNo)
    {
        return !versionNo.HasValue ||
               versionNo.Value <= 0 ||
               log.VersionNo == versionNo.Value;
    }

    private static bool IsPublishApprovalLog(RuleChangeLog log)
    {
        return IsApprovalLifecycleLog(log) &&
               log.ChangeSummary?.Contains(Publish, StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool IsApprovalLifecycleLog(RuleChangeLog log)
    {
        return string.Equals(log.ChangeType, SubmitApproval, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(log.ChangeType, Approve, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(log.ChangeType, Reject, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSubmitApproval(RuleChangeLog log)
    {
        return string.Equals(log.ChangeType, SubmitApproval, StringComparison.OrdinalIgnoreCase);
    }
}


