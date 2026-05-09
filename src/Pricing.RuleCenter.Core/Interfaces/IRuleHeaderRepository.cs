using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IRuleHeaderRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IRuleHeaderRepository
{
    Task<RuleHeader?> GetByIdAsync(long ruleId);
    Task<RuleHeader?> GetByCodeAsync(string ruleCode);
    Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode);
    Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(
        string? itemCode, string? status, string? category, int pageIndex, int pageSize);
    Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime);
    Task<long> InsertAsync(RuleHeader entity);
    Task<bool> UpdateAsync(RuleHeader entity);
    Task<bool> ExistsAsync(string ruleCode);
}
