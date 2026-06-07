using Pricing.RuleCenter.Core.Aggregates.Policies;

namespace Pricing.RuleCenter.Core.Interfaces.Policies;

public interface IPolicyRepository
{
    Task<PolicyAggregate?> GetByIdAsync(long policyId);

    Task<PolicyAggregate?> GetByCodeAsync(string policyCode);

    Task<PolicyVersion?> GetVersionAsync(long policyVersionId);

    Task<IReadOnlyList<PolicyBinding>> GetBindingsAsync(long policyVersionId);

    Task<IReadOnlyList<PolicyScope>> GetScopesAsync(long policyVersionId);

    Task<IReadOnlyList<PolicyParam>> GetParamsAsync(long policyVersionId);

    Task<IReadOnlyList<PolicyVersion>> GetPublishReadyVersionsAsync();

    Task<long> InsertAsync(PolicyAggregate entity);

    Task UpdateAsync(PolicyAggregate entity);
}
