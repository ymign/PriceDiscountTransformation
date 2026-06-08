using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageQueryAppService
{
    private readonly IRuntimePackageRepository _runtimePackageRepository;

    public RuntimePackageQueryAppService(IRuntimePackageRepository runtimePackageRepository)
    {
        _runtimePackageRepository = runtimePackageRepository;
    }

    public async Task<IReadOnlyList<RuntimePackageHistoryResponse>> GetHistoryAsync(int take = 20)
    {
        var items = await _runtimePackageRepository.GetHistoryAsync(take);
        return items.Select(item => new RuntimePackageHistoryResponse
        {
            PackageId = item.PackageId,
            PackageVersion = item.PackageVersion,
            PackageStatus = item.PackageStatus,
            BuiltBy = item.BuiltBy,
            BuiltAt = item.BuiltAt,
            ActivatedBy = item.ActivatedBy,
            ActivatedAt = item.ActivatedAt
        }).ToList();
    }
}
