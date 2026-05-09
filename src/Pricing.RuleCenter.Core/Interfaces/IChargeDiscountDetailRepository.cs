using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IChargeDiscountDetailRepository
{
    Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId);
    Task<long> InsertAsync(ChargeDiscountDetail entity);
    Task UpdateStatusByRequestIdAsync(long requestId, string status);
}
