using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IChargeTraceStepRepository
{
    Task<IReadOnlyList<ChargeTraceStep>> GetByRequestIdAsync(long requestId);
    Task InsertBatchAsync(IReadOnlyList<ChargeTraceStep> entities);
}
