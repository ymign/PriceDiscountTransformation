using Pricing.RuleCenter.Core.Aggregates.Policies;

namespace Pricing.RuleCenter.Core.Interfaces.Policies;

public interface IPolicyRepository
{
    Task<IReadOnlyList<PolicyAggregate>> GetAllAsync();

    Task<PolicyAggregate?> GetByIdAsync(long policyId);

    Task<PolicyAggregate?> GetByCodeAsync(string policyCode);

    Task<IReadOnlyList<PolicyVersion>> GetVersionsByPolicyIdAsync(long policyId);

    Task<PolicyVersion?> GetVersionAsync(long policyVersionId);

    Task<IReadOnlyList<PolicyBinding>> GetBindingsAsync(long policyVersionId);

    Task<IReadOnlyList<PolicyScope>> GetScopesAsync(long policyVersionId);

    Task<IReadOnlyList<PolicyParam>> GetParamsAsync(long policyVersionId);

    Task<IReadOnlyList<PolicyVersion>> GetPublishReadyVersionsAsync();

    Task<long> InsertAsync(PolicyAggregate entity);

    Task UpdateAsync(PolicyAggregate entity);

    Task<long> InsertVersionAsync(PolicyVersion entity);

    Task UpdateVersionAsync(PolicyVersion entity);

    Task ReplaceBindingsAsync(long policyVersionId, IReadOnlyList<PolicyBinding> entities);

    Task ReplaceScopesAsync(long policyVersionId, IReadOnlyList<PolicyScope> entities);

    Task ReplaceParamsAsync(long policyVersionId, IReadOnlyList<PolicyParam> entities);
}
