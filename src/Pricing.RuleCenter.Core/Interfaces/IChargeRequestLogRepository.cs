using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IChargeRequestLogRepository
{
    Task<ChargeRequestLog?> GetByIdAsync(long requestId);
    Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint);
    Task<long> InsertAsync(ChargeRequestLog entity);
    Task UpdateAsync(ChargeRequestLog entity);
}
