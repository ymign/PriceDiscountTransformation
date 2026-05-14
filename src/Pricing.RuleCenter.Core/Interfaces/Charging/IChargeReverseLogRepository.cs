using Pricing.RuleCenter.Core.Aggregates.Charging;

namespace Pricing.RuleCenter.Core.Interfaces.Charging;

/// <summary>
/// IChargeReverseLogRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IChargeReverseLogRepository
{
    Task<IReadOnlyList<ChargeReverseLog>> GetByOriginalRequestIdAsync(long originalRequestId);
    Task<long> InsertAsync(ChargeReverseLog entity);
}
