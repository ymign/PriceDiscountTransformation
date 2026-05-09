using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// ILimitOccupyRepository 定义规则中心内部的依赖契约，用于隔离应用层、领域层和基础设施层的实现细节。
/// </summary>
public interface ILimitOccupyRepository
{
    Task<decimal> GetOccupiedQtyAsync(string limitKey, string status);
    Task<decimal> GetOccupiedQtyAsync(
        string limitType,
        string limitDimensionCode,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<string> statuses);
    Task<decimal> GetOccupiedAmtAsync(string limitKey, string status);
    Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId);
    Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys);
    Task<long> InsertAsync(LimitOccupy entity);
    Task UpdateStatusAsync(long occupyId, string status);
    Task UpdateStatusByRequestIdAsync(long requestId, string status);
}
