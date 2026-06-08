using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Runtime;

/// <summary>
/// 运行时包构建明细写仓储。
/// </summary>
public sealed class RuntimeRuleBuildRepository : IRuntimeRuleBuildRepository
{
    private readonly ISqlSugarClient _db;

    public RuntimeRuleBuildRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public Task<IReadOnlyList<long>> ReservePackagePolicyIdsAsync(int count) =>
        ReserveSequenceValuesAsync("SEQ_PR_RUNTIME_PKG_POLICY", count);

    public Task<IReadOnlyList<long>> ReserveRuleIdsAsync(int count) =>
        ReserveSequenceValuesAsync("SEQ_PR_RUNTIME_RULE", count);

    public Task<IReadOnlyList<long>> ReserveConditionIdsAsync(int count) =>
        ReserveSequenceValuesAsync("SEQ_PR_RUNTIME_CONDITION", count);

    public Task<IReadOnlyList<long>> ReserveActionIdsAsync(int count) =>
        ReserveSequenceValuesAsync("SEQ_PR_RUNTIME_ACTION", count);

    public async Task InsertPackagePoliciesAsync(IReadOnlyList<RuntimePackagePolicy> packagePolicies)
    {
        if (packagePolicies.Count == 0)
        {
            return;
        }

        await _db.Insertable(packagePolicies.ToList()).ExecuteCommandAsync();
    }

    public async Task InsertRulesAsync(IReadOnlyList<RuntimeRule> rules)
    {
        if (rules.Count == 0)
        {
            return;
        }

        await _db.Insertable(rules.ToList()).ExecuteCommandAsync();
    }

    public async Task InsertConditionsAsync(IReadOnlyList<RuntimeCondition> conditions)
    {
        if (conditions.Count == 0)
        {
            return;
        }

        await _db.Insertable(conditions.ToList()).ExecuteCommandAsync();
    }

    public async Task InsertActionsAsync(IReadOnlyList<RuntimeAction> actions)
    {
        if (actions.Count == 0)
        {
            return;
        }

        await _db.Insertable(actions.ToList()).ExecuteCommandAsync();
    }

    private async Task<IReadOnlyList<long>> ReserveSequenceValuesAsync(string sequenceName, int count)
    {
        if (count <= 0)
        {
            return Array.Empty<long>();
        }

        var values = new List<long>(count);
        for (var i = 0; i < count; i++)
        {
            values.Add(await _db.Ado.GetLongAsync($"SELECT {sequenceName}.NEXTVAL FROM DUAL"));
        }

        return values;
    }
}
