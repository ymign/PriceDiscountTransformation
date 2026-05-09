using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class ChargeTraceStepRepository : IChargeTraceStepRepository
{
    private readonly ISqlSugarClient _db;

    public ChargeTraceStepRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ChargeTraceStep>> GetByRequestIdAsync(long requestId)
    {
        return await _db.Queryable<ChargeTraceStep>()
            .Where(s => s.RequestId == requestId)
            .OrderBy(s => s.StepNo)
            .ToListAsync();
    }

    public async Task InsertBatchAsync(IReadOnlyList<ChargeTraceStep> entities)
    {
        foreach (var entity in entities)
        {
            var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_CHARGE_TRACE_STEP.NEXTVAL FROM DUAL");
            entity.StepId = seq;
        }

        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }
}
