using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

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
