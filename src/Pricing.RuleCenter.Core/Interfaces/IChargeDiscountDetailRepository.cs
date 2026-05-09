using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IChargeDiscountDetailRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IChargeDiscountDetailRepository
{
    Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId);
    Task<long> InsertAsync(ChargeDiscountDetail entity);
    Task UpdateStatusByRequestIdAsync(long requestId, string status);
}
