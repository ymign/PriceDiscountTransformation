using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface IChargeRequestLogRepository
{
    Task<ChargeRequestLog?> GetByIdAsync(long requestId);
    Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint);
    Task<long> InsertAsync(ChargeRequestLog entity);
    Task UpdateAsync(ChargeRequestLog entity);
    Task<(IReadOnlyList<ChargeRequestLog> Items, int Total)> GetPagedAsync(
        string? patientId, string? itemCode, string? chargeNo,
        DateTime? startTime, DateTime? endTime,
        int pageIndex, int pageSize);
    Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore);
}
