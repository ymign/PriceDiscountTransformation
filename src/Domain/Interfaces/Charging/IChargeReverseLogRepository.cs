using Pricing.RuleCenter.Core.Aggregates.Charging;

namespace Pricing.RuleCenter.Core.Interfaces.Charging;

/// <summary>
/// IChargeReverseLogRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IChargeReverseLogRepository
{
    /// <summary>
    /// Gets reverse logs associated with the specified original charge request.
    /// </summary>
    /// <param name="originalRequestId">The original charge request identifier.</param>
    /// <returns>The reverse logs already recorded for the original request.</returns>
    Task<IReadOnlyList<ChargeReverseLog>> GetByOriginalRequestIdAsync(long originalRequestId);

    /// <summary>
    /// Inserts a charge reverse log.
    /// </summary>
    /// <param name="entity">The reverse log entity to persist.</param>
    /// <returns>The generated reverse log identifier.</returns>
    Task<long> InsertAsync(ChargeReverseLog entity);
}
