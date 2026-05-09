using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class ChargeDiscountDetailRepository : IChargeDiscountDetailRepository
{
    private readonly ISqlSugarClient _db;

    public ChargeDiscountDetailRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId)
    {
        return await _db.Queryable<ChargeDiscountDetail>()
            .Where(d => d.RequestId == requestId)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(ChargeDiscountDetail entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_CHARGE_DISCOUNT.NEXTVAL FROM DUAL");
        entity.DiscountId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    public async Task UpdateStatusByRequestIdAsync(long requestId, string status)
    {
        await _db.Updateable<ChargeDiscountDetail>()
            .SetColumns(d => d.Status == status)
            .Where(d => d.RequestId == requestId)
            .ExecuteCommandAsync();
    }
}
