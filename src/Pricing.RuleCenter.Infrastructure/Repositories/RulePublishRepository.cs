using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class RulePublishRepository : IRulePublishRepository
{
    private readonly ISqlSugarClient _db;

    public RulePublishRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RulePublish>> GetByRuleIdAsync(long ruleId)
    {
        return await _db.Queryable<RulePublish>()
            .Where(p => p.RuleId == ruleId)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(RulePublish entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_PUBLISH.NEXTVAL FROM DUAL");
        entity.PublishId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }
}
