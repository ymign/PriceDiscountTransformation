using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.Policies;

public sealed class PolicyPublishProfileResolver
{
    public bool RequiresReview(PolicyAggregate policy)
    {
        return !string.Equals(
            policy.PublishProfile,
            PolicyPublishProfileCodes.Direct,
            StringComparison.OrdinalIgnoreCase);
    }
}
