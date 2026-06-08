using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageRollbackService
{
    private readonly RuntimePackageActivationService _activationService;
    private readonly IRuntimePackageRepository _runtimePackageRepository;

    public RuntimePackageRollbackService(
        RuntimePackageActivationService activationService,
        IRuntimePackageRepository runtimePackageRepository)
    {
        _activationService = activationService;
        _runtimePackageRepository = runtimePackageRepository;
    }

    public async Task<Core.Aggregates.Runtime.RuntimePackage> RollbackAsync(long packageId, string rolledBackBy)
    {
        var target = await _runtimePackageRepository.GetByIdAsync(packageId)
            ?? throw new BizException(BizErrorCode.RuntimePackageNotFound, 404, $"运行时包不存在: {packageId}");

        if (!string.Equals(target.PackageStatus, RuntimePackageStatusCodes.Built, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(target.PackageStatus, RuntimePackageStatusCodes.Superseded, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(target.PackageStatus, RuntimePackageStatusCodes.Active, StringComparison.OrdinalIgnoreCase))
        {
            throw new BizException(
                BizErrorCode.RuntimePackageStatusNotAllowed,
                409,
                $"运行时包 {packageId} 当前状态 {target.PackageStatus} 不能回滚激活。");
        }

        return await _activationService.SwitchActivePackageAsync(packageId, rolledBackBy, "ROLLBACK", allowBuiltOnly: false);
    }
}
