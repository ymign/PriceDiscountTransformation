using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class RuleVersionRepository : IRuleVersionRepository
{
    private readonly ISqlSugarClient _db;

    public RuleVersionRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<RuleVersion?> GetByIdAsync(long versionId)
    {
        return await _db.Queryable<RuleVersion>()
            .InSingleAsync(versionId);
    }

    public async Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo)
    {
        return await _db.Queryable<RuleVersion>()
            .FirstAsync(v => v.RuleId == ruleId && v.VersionNo == versionNo);
    }

    public async Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId)
    {
        return await _db.Queryable<RuleVersion>()
            .Where(v => v.RuleId == ruleId)
            .OrderByDescending(v => v.VersionNo)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(RuleVersion entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_VERSION.NEXTVAL FROM DUAL");
        entity.VersionId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    public async Task<bool> UpdateStatusAsync(long versionId, string status)
    {
        var rows = await _db.Updateable<RuleVersion>()
            .SetColumns(v => v.VersionStatus == status)
            .Where(v => v.VersionId == versionId)
            .ExecuteCommandAsync();
        return rows > 0;
    }
}
