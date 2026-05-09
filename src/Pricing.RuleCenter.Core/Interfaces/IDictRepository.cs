using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IDictRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
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
