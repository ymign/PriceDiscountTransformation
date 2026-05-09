using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

public interface ILimitOccupyRepository
{
    Task<decimal> GetOccupiedQtyAsync(string limitKey, string status);
    Task<decimal> GetOccupiedAmtAsync(string limitKey, string status);
    Task<long> InsertAsync(LimitOccupy entity);
    Task UpdateStatusAsync(long occupyId, string status);
    Task UpdateStatusByRequestIdAsync(long requestId, string status);
}
