using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class RuleActionRepository : IRuleActionRepository
{
    private readonly ISqlSugarClient _db;

    public RuleActionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        return await _db.Queryable<RuleAction>()
            .Where(a => a.RuleId == ruleId && a.VersionNo == versionNo && a.IsEnabled == "Y")
            .OrderBy(a => a.SortNo)
            .ToListAsync();
    }

    public async Task InsertBatchAsync(IReadOnlyList<RuleAction> entities)
    {
        foreach (var entity in entities)
        {
            var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_ACTION.NEXTVAL FROM DUAL");
            entity.ActionId = seq;
        }

        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }

    public async Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        await _db.Deleteable<RuleAction>()
            .Where(a => a.RuleId == ruleId && a.VersionNo == versionNo)
            .ExecuteCommandAsync();
    }
}
