using Pricing.RuleCenter.Core.Aggregates.Runtime;

namespace Pricing.RuleCenter.Core.Interfaces.Runtime;

public interface IRuntimePackageStateRepository
{
    Task<RuntimePackageState?> GetActiveAsync();

    Task<RuntimePackageState?> GetActiveForUpdateAsync();

    Task UpsertAsync(RuntimePackageState entity);
}
