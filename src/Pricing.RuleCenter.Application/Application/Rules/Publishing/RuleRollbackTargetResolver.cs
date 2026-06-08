using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则回滚目标版本解析器。
/// </summary>
public sealed class RuleRollbackTargetResolver
{
    private readonly IRulePublishRepository _publishRepository;
    private readonly IRuleVersionRepository _versionRepository;

    /// <summary>
    /// 初始化规则回滚目标版本解析器。
    /// </summary>
    public RuleRollbackTargetResolver(
        IRulePublishRepository publishRepository,
        IRuleVersionRepository versionRepository)
    {
        _publishRepository = publishRepository;
        _versionRepository = versionRepository;
    }

    /// <summary>
    /// 解析当前版本对应的回滚目标版本号。
    /// </summary>
    public async Task<int> ResolveAsync(long ruleId, int currentVersionNo)
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
}
