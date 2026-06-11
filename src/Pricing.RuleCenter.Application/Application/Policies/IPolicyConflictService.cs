using Pricing.RuleCenter.Application.RuntimePackages;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略运行时冲突检测服务契约。
/// </summary>
public interface IPolicyConflictService
{
    /// <summary>
    /// 校验候选运行时规则快照之间不存在单胜者冲突。
    /// </summary>
    /// <param name="ruleSnapshots">候选规则快照集合。</param>
    void EnsureNoConflicts(IReadOnlyList<RuntimeRuleSnapshot> ruleSnapshots);
}
