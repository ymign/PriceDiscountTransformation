using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories;

public sealed class FormulaDefRepository : IFormulaDefRepository
{
    private readonly ISqlSugarClient _db;

    public FormulaDefRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<FormulaDef>> GetAllAsync()
    {
        return await _db.Queryable<FormulaDef>()
            .OrderBy(f => f.FormulaId)
            .ToListAsync();
    }

    public async Task<FormulaDef?> GetByIdAsync(long formulaId)
    {
        return await _db.Queryable<FormulaDef>()
            .InSingleAsync(formulaId);
    }

    public async Task<FormulaDef?> GetByCodeAsync(string formulaCode)
    {
        return await _db.Queryable<FormulaDef>()
            .FirstAsync(f => f.FormulaCode == formulaCode);
    }

    public async Task<long> InsertAsync(FormulaDef entity)
    {
        var seq = await _db.Ado.GetLongAsync("SELECT SEQ_PR_FORMULA_DEF.NEXTVAL FROM DUAL");
        entity.FormulaId = seq;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return seq;
    }

    public async Task<bool> UpdateAsync(FormulaDef entity)
    {
        var rows = await _db.Updateable(entity)
            .IgnoreColumns(f => f.FormulaId)
            .ExecuteCommandAsync();
        return rows > 0;
    }

    public async Task<bool> SetEnabledAsync(long formulaId, string isEnabled)
    {
        var rows = await _db.Updateable<FormulaDef>()
            .SetColumns(f => f.IsEnabled == isEnabled)
            .Where(f => f.FormulaId == formulaId)
            .ExecuteCommandAsync();
        return rows > 0;
    }
}
