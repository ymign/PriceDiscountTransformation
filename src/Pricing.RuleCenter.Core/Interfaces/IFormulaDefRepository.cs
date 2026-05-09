using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IFormulaDefRepository
{
    Task<IReadOnlyList<FormulaDef>> GetAllAsync();
    Task<FormulaDef?> GetByIdAsync(long formulaId);
    Task<FormulaDef?> GetByCodeAsync(string formulaCode);
    Task<long> InsertAsync(FormulaDef entity);
    Task<bool> UpdateAsync(FormulaDef entity);
    Task<bool> SetEnabledAsync(long formulaId, string isEnabled);
}
