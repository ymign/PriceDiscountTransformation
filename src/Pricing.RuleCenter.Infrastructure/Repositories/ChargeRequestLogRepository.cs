using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class ChargeRequestLogRepository : IChargeRequestLogRepository
{
    private readonly ISqlSugarClient _db;

    public ChargeRequestLogRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<ChargeRequestLog?> GetByIdAsync(long requestId)
    {
        return await _db.Queryable<ChargeRequestLog>().InSingleAsync(requestId);
    }

    public async Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint)
    {
        return await _db.Queryable<ChargeRequestLog>()
            .FirstAsync(r => r.RequestFingerprint == fingerprint);
    }

    public async Task<long> InsertAsync(ChargeRequestLog entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_CHARGE_REQUEST_LOG.NEXTVAL FROM DUAL");
        entity.RequestId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    public async Task UpdateAsync(ChargeRequestLog entity)
    {
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task<(IReadOnlyList<ChargeRequestLog> Items, int Total)> GetPagedAsync(
        string? patientId, string? itemCode, string? chargeNo,
        DateTime? startTime, DateTime? endTime,
        int pageIndex, int pageSize)
    {
        var total = new RefAsync<int>();
        var items = await _db.Queryable<ChargeRequestLog>()
            .WhereIF(!string.IsNullOrEmpty(patientId), r => r.PatientId == patientId)
            .WhereIF(!string.IsNullOrEmpty(itemCode), r => r.ItemCode == itemCode)
            .WhereIF(!string.IsNullOrEmpty(chargeNo), r => r.ChargeNo == chargeNo)
            .WhereIF(startTime.HasValue, r => r.RequestAt >= startTime!.Value)
            .WhereIF(endTime.HasValue, r => r.RequestAt <= endTime!.Value)
            .OrderByDescending(r => r.RequestAt)
            .ToPageListAsync(pageIndex, pageSize, total);
        return (items, total.Value);
    }

    public async Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore)
    {
        return await _db.Queryable<ChargeRequestLog>()
            .Where(r => r.BusinessStatus == "CONFIRMED" && r.RequestAt < expireBefore)
            .ToListAsync();
    }
}
