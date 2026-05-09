using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class DictRepository : IDictRepository
{
    private readonly ISqlSugarClient _db;

    public DictRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType)
    {
        return await _db.Queryable<Dict>()
            .Where(d => d.DictType == dictType && d.IsEnabled == "Y")
            .OrderBy(d => d.SortNo)
            .ToListAsync();
    }

    public async Task<Dict?> GetByIdAsync(long dictId)
    {
        return await _db.Queryable<Dict>()
            .InSingleAsync(dictId);
    }

    public async Task<IReadOnlyList<string>> GetAllTypesAsync()
    {
        return await _db.Queryable<Dict>()
            .Where(d => d.IsEnabled == "Y")
            .GroupBy(d => d.DictType)
            .Select(d => d.DictType)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(Dict entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_DICT.NEXTVAL FROM DUAL");
        entity.DictId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    public async Task<bool> UpdateAsync(Dict entity)
    {
        var rows = await _db.Updateable(entity)
            .IgnoreColumns(d => d.DictId)
            .ExecuteCommandAsync();
        return rows > 0;
    }

    public async Task<bool> SetEnabledAsync(long dictId, string isEnabled)
    {
        var rows = await _db.Updateable<Dict>()
            .SetColumns(d => d.IsEnabled == isEnabled)
            .Where(d => d.DictId == dictId)
            .ExecuteCommandAsync();
        return rows > 0;
    }

    public async Task<bool> ExistsAsync(string dictType, string dictCode)
    {
        return await _db.Queryable<Dict>()
            .AnyAsync(d => d.DictType == dictType && d.DictCode == dictCode);
    }
}
