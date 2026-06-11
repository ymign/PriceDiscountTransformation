using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略发布配置解析器。
/// </summary>
internal sealed class PolicyPublishProfileResolver
{
    /// <summary>
    /// 判断指定策略是否必须经过审批后才能发布。
    /// </summary>
    public bool RequiresReview(PolicyAggregate policy)
    {
        return !string.Equals(
            policy.PublishProfile,
            PolicyPublishProfileCodes.Direct,
            StringComparison.OrdinalIgnoreCase);
    }
}
