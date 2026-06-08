using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Policies;

public sealed class PolicyReviewRepository : IPolicyReviewRepository
{
    private readonly ISqlSugarClient _db;

    public PolicyReviewRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<PolicyReview?> GetLatestByPolicyVersionIdAsync(long policyVersionId)
    {
        return await _db.Queryable<PolicyReview>()
            .Where(item => item.PolicyVersionId == policyVersionId)
            .OrderByDescending(item => item.ReviewedAt ?? item.SubmittedAt ?? DateTime.MinValue)
            .OrderByDescending(item => item.ReviewId)
            .FirstAsync();
    }

    public async Task<IReadOnlyList<PolicyReview>> GetByPolicyVersionIdAsync(long policyVersionId)
    {
        return await _db.Queryable<PolicyReview>()
            .Where(item => item.PolicyVersionId == policyVersionId)
            .OrderByDescending(item => item.ReviewId)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(PolicyReview entity)
    {
        var reviewId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_POLICY_REVIEW.NEXTVAL FROM DUAL");
        entity.ReviewId = reviewId;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return reviewId;
    }

    public async Task UpdateAsync(PolicyReview entity)
    {
        await _db.Updateable(entity)
            .Where(item => item.ReviewId == entity.ReviewId)
            .ExecuteCommandAsync();
    }
}
