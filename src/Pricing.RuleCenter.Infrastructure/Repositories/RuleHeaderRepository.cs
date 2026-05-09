using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class RuleHeaderRepository : IRuleHeaderRepository
{
    private readonly ISqlSugarClient _db;

    public RuleHeaderRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<RuleHeader?> GetByIdAsync(long ruleId)
    {
        return await _db.Queryable<RuleHeader>()
            .InSingleAsync(ruleId);
    }

    public async Task<RuleHeader?> GetByCodeAsync(string ruleCode)
    {
        return await _db.Queryable<RuleHeader>()
            .FirstAsync(r => r.RuleCode == ruleCode);
    }

    public async Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode)
    {
        return await _db.Queryable<RuleHeader>()
            .Where(r => r.ItemCode == itemCode && r.IsEnabled == "Y")
            .OrderBy(r => r.Priority)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(
        string? itemCode, string? status, string? category, int pageIndex, int pageSize)
    {
        var total = new RefAsync<int>();
        var items = await _db.Queryable<RuleHeader>()
            .WhereIF(!string.IsNullOrEmpty(itemCode), r => r.ItemCode == itemCode)
            .WhereIF(!string.IsNullOrEmpty(status), r => r.Status == status)
            .WhereIF(!string.IsNullOrEmpty(category), r => r.RuleCategory == category)
            .OrderBy(r => r.Priority)
            .ToPageListAsync(pageIndex, pageSize, total);
        return (items, total.Value);
    }

    public async Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime)
    {
        return await _db.Queryable<RuleHeader>()
            .Where(r => r.IsEnabled == "Y" && r.Status == "PUBLISHED")
            .Where(r => (r.EffectiveFrom == null || r.EffectiveFrom <= businessTime))
            .Where(r => (r.EffectiveTo == null || r.EffectiveTo >= businessTime))
            .OrderBy(r => r.Priority)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(RuleHeader entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_RULE_HEADER.NEXTVAL FROM DUAL");
        entity.RuleId = seq;
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    public async Task<bool> UpdateAsync(RuleHeader entity)
    {
        entity.UpdatedAt = DateTime.Now;
        var rows = await _db.Updateable(entity)
            .IgnoreColumns(r => new { r.RuleId, r.RuleCode, r.CreatedBy, r.CreatedAt })
            .ExecuteCommandAsync();
        return rows > 0;
    }

    public async Task<bool> ExistsAsync(string ruleCode)
    {
        return await _db.Queryable<RuleHeader>()
            .AnyAsync(r => r.RuleCode == ruleCode);
    }
}
