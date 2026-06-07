using Pricing.RuleCenter.Core.Aggregates.Runtime;

namespace Pricing.RuleCenter.Core.Interfaces.Runtime;

public interface IRuntimePackageRepository
{
    Task<RuntimePackage?> GetByIdAsync(long packageId);

    Task<IReadOnlyList<RuntimePackage>> GetHistoryAsync(int take);

    Task<long> InsertAsync(RuntimePackage entity);

    Task UpdateAsync(RuntimePackage entity);
}
