using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class RuleChangeLogRepository : IRuleChangeLogRepository
{
    private readonly ISqlSugarClient _db;

    public RuleChangeLogRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId)
    {
        return await _db.Queryable<RuleChangeLog>()
            .Where(c => c.RuleId == ruleId)
            .OrderByDescending(c => c.ChangedAt)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(RuleChangeLog entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_CHANGE_LOG.NEXTVAL FROM DUAL");
        entity.ChangeId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }
}
