using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Policies;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackagePublishService
{
    private readonly IPolicyRepository _policyRepository;
    private readonly PolicyPublishEligibilityService _eligibilityService;
    private readonly RuntimePackageCompiler _compiler;
    private readonly RuntimePackageActivationService _activationService;

    public RuntimePackagePublishService(
        IPolicyRepository policyRepository,
        PolicyPublishEligibilityService eligibilityService,
        RuntimePackageCompiler compiler,
        RuntimePackageActivationService activationService)
    {
        _policyRepository = policyRepository;
        _eligibilityService = eligibilityService;
        _compiler = compiler;
        _activationService = activationService;
    }

    public async Task<RuntimePackageBuildResult> PublishAsync(
        IReadOnlyCollection<long> policyVersionIds,
        string publishedBy,
        DateTime? buildAt = null)
    {
        foreach (var policyVersionId in policyVersionIds.Distinct())
        {
            var version = await _policyRepository.GetVersionAsync(policyVersionId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {policyVersionId}");
            var policy = await _policyRepository.GetByIdAsync(version.PolicyId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略不存在: {version.PolicyId}");

            await _eligibilityService.EnsureEligibleAsync(policy, version);
            if (!string.Equals(version.PolicyStatus, PolicyLifecycleCodes.PublishReady, StringComparison.OrdinalIgnoreCase))
            {
                version.PolicyStatus = PolicyLifecycleCodes.PublishReady;
                await _policyRepository.UpdateVersionAsync(version);
            }
        }

        var result = await _compiler.CompileAsync(new RuntimePackageBuildContext
        {
            BuiltBy = publishedBy.Trim(),
            BuildAt = buildAt,
            PolicyVersionIds = policyVersionIds
        });

        foreach (var policyVersionId in policyVersionIds.Distinct())
        {
            var version = await _policyRepository.GetVersionAsync(policyVersionId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {policyVersionId}");
            version.LastBuiltPackageId = result.Package.PackageId;
            await _policyRepository.UpdateVersionAsync(version);
        }

        await _activationService.ActivateAsync(result.Package.PackageId, publishedBy);
        return result;
    }
}
