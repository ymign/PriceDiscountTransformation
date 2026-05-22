using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Core.Tests;

internal sealed class InMemoryRuleHeaderRepository : IRuleHeaderRepository
{
    private readonly List<RuleAggregate> _rules;

    public InMemoryRuleHeaderRepository(params RuleAggregate[] rules)
    {
        _rules = rules.ToList();
    }

    public Task<RuleAggregate?> GetByIdAsync(long ruleId)
    {
        return Task.FromResult(_rules.FirstOrDefault(r => r.RuleId == ruleId));
    }

    public Task<RuleAggregate?> GetByIdForUpdateAsync(long ruleId)
    {
        return GetByIdAsync(ruleId);
    }

    public Task<RuleAggregate?> GetByCodeAsync(string ruleCode)
    {
        return Task.FromResult(_rules.FirstOrDefault(r =>
            string.Equals(r.RuleCode, ruleCode, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<IReadOnlyList<RuleAggregate>> GetByItemCodeAsync(string itemCode)
    {
        IReadOnlyList<RuleAggregate> result = _rules
            .Where(r => string.Equals(r.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.Priority)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<(IReadOnlyList<RuleAggregate> Items, int Total)> GetPagedAsync(
        string? itemCode, string? status, string? category, int pageIndex, int pageSize)
    {
        throw new NotSupportedException();
    }

    public Task<IReadOnlyList<RuleAggregate>> GetEffectiveAsync(DateTime businessTime)
    {
        throw new NotSupportedException();
    }

    public Task<long> InsertAsync(RuleAggregate entity)
    {
        throw new NotSupportedException();
    }

    public Task<bool> UpdateAsync(RuleAggregate entity, string? expectedCurrentStatus = null)
    {
        throw new NotSupportedException();
    }

    public Task<bool> ExistsAsync(string ruleCode)
    {
        throw new NotSupportedException();
    }
}

internal sealed class StubRuleConditionRepository : IRuleConditionRepository
{
    public Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        return Task.FromResult<IReadOnlyList<RuleCondition>>(Array.Empty<RuleCondition>());
    }

    public Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities)
    {
        throw new NotSupportedException();
    }

    public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        throw new NotSupportedException();
    }
}

internal sealed class StubRuleActionRepository : IRuleActionRepository
{
    private readonly List<RuleAction> _actions;

    public StubRuleActionRepository(params RuleAction[] actions)
    {
        _actions = actions.ToList();
    }

    public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        IReadOnlyList<RuleAction> result = _actions
            .Where(a => a.RuleId == ruleId && a.VersionNo == versionNo)
            .OrderBy(a => a.SortNo)
            .ToList();
        return Task.FromResult(result);
    }

    public Task InsertBatchAsync(IReadOnlyList<RuleAction> entities)
    {
        throw new NotSupportedException();
    }

    public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        throw new NotSupportedException();
    }
}

internal sealed class StubDictRepository : IDictRepository
{
    public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType)
    {
        return Task.FromResult<IReadOnlyList<Dict>>(Array.Empty<Dict>());
    }

    public Task<Dict?> GetByIdAsync(long dictId)
    {
        throw new NotSupportedException();
    }

    public Task<IReadOnlyList<string>> GetAllTypesAsync()
    {
        throw new NotSupportedException();
    }

    public Task<long> InsertAsync(Dict entity)
    {
        throw new NotSupportedException();
    }

    public Task<bool> UpdateAsync(Dict entity)
    {
        throw new NotSupportedException();
    }

    public Task<bool> SetEnabledAsync(long dictId, string isEnabled)
    {
        throw new NotSupportedException();
    }

    public Task<bool> ExistsAsync(string dictType, string dictCode)
    {
        throw new NotSupportedException();
    }
}

internal sealed class InMemoryLimitOccupyRepository : ILimitOccupyRepository
{
    private readonly List<LimitOccupy> _occupies = new();

    public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status)
    {
        return Task.FromResult(_occupies
            .Where(o => string.Equals(o.LimitKey, limitKey, StringComparison.OrdinalIgnoreCase))
            .Where(o => string.Equals(o.Status, status, StringComparison.OrdinalIgnoreCase))
            .Sum(o => o.OccupyQty));
    }

    public Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query)
    {
        return Task.FromResult(_occupies
            .Where(o => string.Equals(o.LimitType, query.LimitType, StringComparison.OrdinalIgnoreCase))
            .Where(o => string.Equals(o.LimitDimensionCode, query.LimitDimensionCode, StringComparison.OrdinalIgnoreCase))
            .Where(o => query.Statuses.Contains(o.Status, StringComparer.OrdinalIgnoreCase))
            .Where(o => o.BusinessChargeTime >= query.StartTime && o.BusinessChargeTime <= query.EndTime)
            .Sum(o => o.OccupyQty));
    }

    public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status)
    {
        return Task.FromResult(_occupies
            .Where(o => string.Equals(o.LimitKey, limitKey, StringComparison.OrdinalIgnoreCase))
            .Where(o => string.Equals(o.Status, status, StringComparison.OrdinalIgnoreCase))
            .Sum(o => o.OccupyAmt));
    }

    public Task<decimal> GetOccupiedAmtByDimensionAsync(string dimensionCode, string status)
    {
        return Task.FromResult(_occupies
            .Where(o => string.Equals(o.LimitDimensionCode, dimensionCode, StringComparison.OrdinalIgnoreCase))
            .Where(o => string.Equals(o.Status, status, StringComparison.OrdinalIgnoreCase))
            .Sum(o => o.OccupyAmt));
    }

    public Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId)
    {
        IReadOnlyList<LimitOccupy> result = _occupies.Where(o => o.RequestId == requestId).ToList();
        return Task.FromResult(result);
    }

    public Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys)
    {
        return Task.CompletedTask;
    }

    public Task<long> InsertAsync(LimitOccupy entity)
    {
        entity.OccupyId = _occupies.Count + 1;
        _occupies.Add(entity);
        return Task.FromResult(entity.OccupyId);
    }

    public Task UpdateStatusAsync(long occupyId, string status)
    {
        var occupy = _occupies.First(o => o.OccupyId == occupyId);
        occupy.Status = status;
        return Task.CompletedTask;
    }

    public Task UpdateStatusByRequestIdAsync(long requestId, string status)
    {
        foreach (var occupy in _occupies.Where(o => o.RequestId == requestId))
        {
            occupy.Status = status;
        }

        return Task.CompletedTask;
    }

    public void Seed(params LimitOccupy[] occupies)
    {
        _occupies.AddRange(occupies);
    }
}
