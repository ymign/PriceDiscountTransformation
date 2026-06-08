using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Policies;

public sealed class PolicyRepository : IPolicyRepository
{
    private readonly ISqlSugarClient _db;

    public PolicyRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<PolicyAggregate>> GetAllAsync()
    {
        return await _db.Queryable<PolicyAggregate>()
            .OrderBy(item => item.PolicyCode)
            .ToListAsync();
    }

    public async Task<PolicyAggregate?> GetByIdAsync(long policyId)
    {
        return await _db.Queryable<PolicyAggregate>()
            .InSingleAsync(policyId);
    }

    public async Task<PolicyAggregate?> GetByCodeAsync(string policyCode)
    {
        return await _db.Queryable<PolicyAggregate>()
            .FirstAsync(item => item.PolicyCode == policyCode);
    }

    public async Task<IReadOnlyList<PolicyVersion>> GetVersionsByPolicyIdAsync(long policyId)
    {
        return await _db.Queryable<PolicyVersion>()
            .Where(item => item.PolicyId == policyId)
            .OrderByDescending(item => item.VersionNo)
            .ToListAsync();
    }

    public async Task<PolicyVersion?> GetVersionAsync(long policyVersionId)
    {
        return await _db.Queryable<PolicyVersion>()
            .InSingleAsync(policyVersionId);
    }

    public async Task<IReadOnlyList<PolicyBinding>> GetBindingsAsync(long policyVersionId)
    {
        return await _db.Queryable<PolicyBinding>()
            .Where(item => item.PolicyVersionId == policyVersionId)
            .OrderBy(item => item.PolicyBindingId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PolicyScope>> GetScopesAsync(long policyVersionId)
    {
        return await _db.Queryable<PolicyScope>()
            .Where(item => item.PolicyVersionId == policyVersionId)
            .OrderBy(item => item.PolicyScopeId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PolicyParam>> GetParamsAsync(long policyVersionId)
    {
        return await _db.Queryable<PolicyParam>()
            .Where(item => item.PolicyVersionId == policyVersionId)
            .OrderBy(item => item.PolicyParamId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PolicyVersion>> GetPublishReadyVersionsAsync()
    {
        return await _db.Queryable<PolicyVersion>()
            .Where(item => item.PolicyStatus == PolicyLifecycleCodes.PublishReady)
            .OrderBy(item => item.PolicyVersionId)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(PolicyAggregate entity)
    {
        var policyId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_POLICY.NEXTVAL FROM DUAL");
        entity.PolicyId = policyId;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return policyId;
    }

    public async Task UpdateAsync(PolicyAggregate entity)
    {
        await _db.Updateable(entity)
            .Where(item => item.PolicyId == entity.PolicyId)
            .ExecuteCommandAsync();
    }

    public async Task<long> InsertVersionAsync(PolicyVersion entity)
    {
        var policyVersionId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_POLICY_VERSION.NEXTVAL FROM DUAL");
        entity.PolicyVersionId = policyVersionId;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return policyVersionId;
    }

    public async Task UpdateVersionAsync(PolicyVersion entity)
    {
        await _db.Updateable(entity)
            .Where(item => item.PolicyVersionId == entity.PolicyVersionId)
            .ExecuteCommandAsync();
    }

    public async Task ReplaceBindingsAsync(long policyVersionId, IReadOnlyList<PolicyBinding> entities)
    {
        await _db.Deleteable<PolicyBinding>()
            .Where(item => item.PolicyVersionId == policyVersionId)
            .ExecuteCommandAsync();

        if (entities.Count == 0)
        {
            return;
        }

        foreach (var entity in entities)
        {
            entity.PolicyBindingId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_POLICY_BINDING.NEXTVAL FROM DUAL");
            entity.PolicyVersionId = policyVersionId;
        }

        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }

    public async Task ReplaceScopesAsync(long policyVersionId, IReadOnlyList<PolicyScope> entities)
    {
        await _db.Deleteable<PolicyScope>()
            .Where(item => item.PolicyVersionId == policyVersionId)
            .ExecuteCommandAsync();

        if (entities.Count == 0)
        {
            return;
        }

        foreach (var entity in entities)
        {
            entity.PolicyScopeId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_POLICY_SCOPE.NEXTVAL FROM DUAL");
            entity.PolicyVersionId = policyVersionId;
        }

        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }

    public async Task ReplaceParamsAsync(long policyVersionId, IReadOnlyList<PolicyParam> entities)
    {
        await _db.Deleteable<PolicyParam>()
            .Where(item => item.PolicyVersionId == policyVersionId)
            .ExecuteCommandAsync();

        if (entities.Count == 0)
        {
            return;
        }

        foreach (var entity in entities)
        {
            entity.PolicyParamId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_POLICY_PARAM.NEXTVAL FROM DUAL");
            entity.PolicyVersionId = policyVersionId;
        }

        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }
}
