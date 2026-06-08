using Pricing.RuleCenter.Core.Aggregates.Policies;

namespace Pricing.RuleCenter.Core.Interfaces.Policies;

public interface IPolicyReviewRepository
{
    Task<PolicyReview?> GetLatestByPolicyVersionIdAsync(long policyVersionId);

    Task<IReadOnlyList<PolicyReview>> GetByPolicyVersionIdAsync(long policyVersionId);

    Task<long> InsertAsync(PolicyReview entity);

    Task UpdateAsync(PolicyReview entity);
}
