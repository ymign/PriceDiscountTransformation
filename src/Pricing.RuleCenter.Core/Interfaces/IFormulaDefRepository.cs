using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IFormulaDefRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IFormulaDefRepository
{
    Task<IReadOnlyList<FormulaDef>> GetAllAsync();
    Task<FormulaDef?> GetByIdAsync(long formulaId);
    Task<FormulaDef?> GetByCodeAsync(string formulaCode);
    Task<long> InsertAsync(FormulaDef entity);
    Task<bool> UpdateAsync(FormulaDef entity);
    Task<bool> SetEnabledAsync(long formulaId, string isEnabled);
}
