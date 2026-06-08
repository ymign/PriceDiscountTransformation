using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Runtime;

/// <summary>
/// 运行时规则只读仓储。
/// </summary>
public sealed class RuntimeRuleReadRepository : IRuntimeRuleReadRepository
{
    private readonly ISqlSugarClient _db;

    public RuntimeRuleReadRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RuntimeRule>> GetRulesByItemCodeAsync(long packageId, string itemCode)
    {
        if (packageId <= 0 || string.IsNullOrWhiteSpace(itemCode))
        {
            return Array.Empty<RuntimeRule>();
        }

        var normalizedItemCode = itemCode.Trim();
        var groupCodes = await _db.Queryable<ItemGroupDetail, ItemGroup>((detail, group) => new JoinQueryInfos(
                JoinType.Inner, detail.GroupId == group.GroupId))
            .Where((detail, group) =>
                detail.ItemCode == normalizedItemCode &&
                detail.IsEnabled == EnableFlag.Yes &&
                group.IsEnabled == EnableFlag.Yes)
            .Select((detail, group) => group.GroupCode)
            .Distinct()
            .ToListAsync();

        var query = _db.Queryable<RuntimeRule>()
            .Where(rule => rule.PackageId == packageId);

        query = groupCodes.Count == 0
            ? query.Where(rule => rule.TargetItemCode == normalizedItemCode)
            : query.Where(rule =>
                rule.TargetItemCode == normalizedItemCode ||
                (rule.TargetGroupCode != null && groupCodes.Contains(rule.TargetGroupCode)));

        return await query
            .OrderBy(rule => rule.PriorityKey)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<RuntimeRule>> GetRulesByIdsAsync(IReadOnlyCollection<long> runtimeRuleIds)
    {
        if (runtimeRuleIds.Count == 0)
        {
            return Array.Empty<RuntimeRule>();
        }

        var keys = runtimeRuleIds.Distinct().ToArray();
        return await _db.Queryable<RuntimeRule>()
            .Where(rule => keys.Contains(rule.RuntimeRuleId))
            .ToListAsync();
    }

    public async Task<IReadOnlyDictionary<long, IReadOnlyList<RuntimeCondition>>> GetConditionsByRuleIdsAsync(IReadOnlyCollection<long> runtimeRuleIds)
    {
        if (runtimeRuleIds.Count == 0)
        {
            return new Dictionary<long, IReadOnlyList<RuntimeCondition>>();
        }

        var keys = runtimeRuleIds.Distinct().ToArray();
        var items = await _db.Queryable<RuntimeCondition>()
            .Where(condition => keys.Contains(condition.RuntimeRuleId))
            .OrderBy(condition => condition.SortNo)
            .ToListAsync();

        return keys.ToDictionary(
            key => key,
            key => (IReadOnlyList<RuntimeCondition>)items
                .Where(item => item.RuntimeRuleId == key)
                .ToList());
    }

    public async Task<IReadOnlyDictionary<long, IReadOnlyList<RuntimeAction>>> GetActionsByRuleIdsAsync(IReadOnlyCollection<long> runtimeRuleIds)
    {
        if (runtimeRuleIds.Count == 0)
        {
            return new Dictionary<long, IReadOnlyList<RuntimeAction>>();
        }

        var keys = runtimeRuleIds.Distinct().ToArray();
        var items = await _db.Queryable<RuntimeAction>()
            .Where(action => keys.Contains(action.RuntimeRuleId))
            .OrderBy(action => action.SortNo)
            .ToListAsync();

        return keys.ToDictionary(
            key => key,
            key => (IReadOnlyList<RuntimeAction>)items
                .Where(item => item.RuntimeRuleId == key)
                .ToList());
    }
}
