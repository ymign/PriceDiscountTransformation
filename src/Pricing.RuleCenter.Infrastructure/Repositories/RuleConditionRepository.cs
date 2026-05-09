using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class RuleConditionRepository : IRuleConditionRepository
{
    private readonly ISqlSugarClient _db;

    public RuleConditionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        return await _db.Queryable<RuleCondition>()
            .Where(c => c.RuleId == ruleId && c.VersionNo == versionNo && c.IsEnabled == "Y")
            .OrderBy(c => c.ConditionGroup)
            .OrderBy(c => c.SortNo)
            .ToListAsync();
    }

    public async Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities)
    {
        foreach (var entity in entities)
        {
            var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_CONDITION.NEXTVAL FROM DUAL");
            entity.ConditionId = seq;
        }

        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }

    public async Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        await _db.Deleteable<RuleCondition>()
            .Where(c => c.RuleId == ruleId && c.VersionNo == versionNo)
            .ExecuteCommandAsync();
    }
}
