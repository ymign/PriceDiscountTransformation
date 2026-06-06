using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules.Guards;

/// <summary>
/// 规则发布前聚合门禁。
/// </summary>
public sealed class RulePublishGuard
{
    private readonly IRuleConditionRepository _conditionRepository;
    private readonly IRuleActionRepository _actionRepository;
    private readonly RuleActionParameterValidator _actionParameterValidator;
    private readonly RuleCriticalActionGuard _criticalActionGuard;
    private readonly RuleChildItemGuard _childItemGuard;
    private readonly RuleTestCaseGate _testCaseGate;
    private readonly RuleConflictDetector _conflictDetector;
    private readonly RuleCapabilityGuard _capabilityGuard;

    /// <summary>
    /// 初始化规则发布前聚合门禁。
    /// </summary>
    public RulePublishGuard(
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        RuleActionParameterValidator actionParameterValidator,
        RuleCriticalActionGuard criticalActionGuard,
        RuleChildItemGuard childItemGuard,
        RuleTestCaseGate testCaseGate,
        RuleConflictDetector conflictDetector,
        RuleCapabilityGuard capabilityGuard)
    {
        _conditionRepository = conditionRepository;
        _actionRepository = actionRepository;
        _actionParameterValidator = actionParameterValidator;
        _criticalActionGuard = criticalActionGuard;
        _childItemGuard = childItemGuard;
        _testCaseGate = testCaseGate;
        _conflictDetector = conflictDetector;
        _capabilityGuard = capabilityGuard;
    }

    /// <summary>
    /// 校验目标草稿版本是否满足发布条件。
    /// </summary>
    public async Task EnsureCanPublishAsync(RuleAggregate targetHeader, int targetVersionNo)
    {
        if (string.IsNullOrWhiteSpace(targetHeader.ItemCode))
        {
            return;
        }

        var conditions = await _conditionRepository.GetByRuleAndVersionAsync(targetHeader.RuleId, targetVersionNo);
        var actions = await _actionRepository.GetByRuleAndVersionAsync(targetHeader.RuleId, targetVersionNo);
        _capabilityGuard.EnsureSupported(conditions, actions);
        _criticalActionGuard.EnsureStopOnError(actions);
        _childItemGuard.EnsureValid(actions);
        _actionParameterValidator.Validate(actions);

        await _testCaseGate.EnsurePassedAsync(targetHeader.RuleId, targetVersionNo);
        await _conflictDetector.EnsureNoConflictAsync(targetHeader, targetVersionNo);
    }

    /// <summary>
    /// 清除发布门禁内部缓存。
    /// </summary>
    public void ClearCache()
    {
        _conflictDetector.ClearCache();
    }
}
