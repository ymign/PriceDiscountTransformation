using Pricing.RuleCenter.Core.Aggregates.Policies;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略运行时优先级键生成器契约。
/// </summary>
public interface IPolicyPriorityKeyFactory
{
    /// <summary>
    /// 根据版本、绑定和作用域计算稳定优先级键。
    /// </summary>
    /// <param name="version">策略版本。</param>
    /// <param name="binding">绑定对象。</param>
    /// <param name="scopes">作用域集合。</param>
    /// <returns>用于运行期排序和冲突判定的优先级键。</returns>
    string Build(PolicyVersion version, PolicyBinding binding, IReadOnlyList<PolicyScope> scopes);
}
