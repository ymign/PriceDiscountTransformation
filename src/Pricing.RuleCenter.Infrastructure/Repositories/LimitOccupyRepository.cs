using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class LimitOccupyRepository : ILimitOccupyRepository
{
    private readonly ISqlSugarClient _db;

    public LimitOccupyRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<decimal> GetOccupiedQtyAsync(string limitKey, string status)
    {
        var result = await _db.Queryable<LimitOccupy>()
            .Where(o => o.LimitKey == limitKey && o.Status == status)
            .SumAsync(o => o.OccupyQty);
        return result;
    }

    public async Task<decimal> GetOccupiedAmtAsync(string limitKey, string status)
    {
        var result = await _db.Queryable<LimitOccupy>()
            .Where(o => o.LimitKey == limitKey && o.Status == status)
            .SumAsync(o => o.OccupyAmt);
        return result;
    }

    public async Task<long> InsertAsync(LimitOccupy entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_LIMIT_OCCUPY.NEXTVAL FROM DUAL");
        entity.OccupyId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    public async Task UpdateStatusAsync(long occupyId, string status)
    {
        await _db.Updateable<LimitOccupy>()
            .SetColumns(o => o.Status == status)
            .Where(o => o.OccupyId == occupyId)
            .ExecuteCommandAsync();
    }

    public async Task UpdateStatusByRequestIdAsync(long requestId, string status)
    {
        await _db.Updateable<LimitOccupy>()
            .SetColumns(o => o.Status == status)
            .Where(o => o.RequestId == requestId)
            .ExecuteCommandAsync();
    }
}
