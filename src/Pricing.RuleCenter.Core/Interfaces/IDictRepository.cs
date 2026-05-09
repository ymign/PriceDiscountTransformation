using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IDictRepository
{
    Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType);
    Task<Dict?> GetByIdAsync(long dictId);
    Task<IReadOnlyList<string>> GetAllTypesAsync();
    Task<long> InsertAsync(Dict entity);
    Task<bool> UpdateAsync(Dict entity);
    Task<bool> SetEnabledAsync(long dictId, string isEnabled);
    Task<bool> ExistsAsync(string dictType, string dictCode);
}
