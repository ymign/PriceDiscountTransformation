using Pricing.RuleCenter.Core.Aggregates.Policies;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略发布资格校验服务契约。
/// </summary>
public interface IPolicyPublishEligibilityService
{
    /// <summary>
    /// 校验策略版本是否允许进入运行时包发布流程。
    /// </summary>
    /// <param name="policy">策略主档。</param>
    /// <param name="version">策略版本。</param>
    Task EnsureEligibleAsync(PolicyAggregate policy, PolicyVersion version);
}
