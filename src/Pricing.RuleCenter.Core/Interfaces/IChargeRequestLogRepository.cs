using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// IChargeRequestLogRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface IChargeRequestLogRepository
{
    Task<ChargeRequestLog?> GetByIdAsync(long requestId);
    Task<ChargeRequestLog?> GetByBusinessKeyAsync(
        string sourceSystem, string businessRequestNo, string callType);
    Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint);
    Task<long> InsertAsync(ChargeRequestLog entity);
    Task UpdateAsync(ChargeRequestLog entity);
    Task<(IReadOnlyList<ChargeRequestLog> Items, int Total)> GetPagedAsync(
        string? patientId, string? itemCode, string? chargeNo,
        DateTime? startTime, DateTime? endTime,
        int pageIndex, int pageSize);
    Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore);
}
