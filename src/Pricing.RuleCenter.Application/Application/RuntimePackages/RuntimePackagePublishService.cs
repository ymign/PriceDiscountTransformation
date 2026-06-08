using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
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
        var requestedPolicyVersionIds = policyVersionIds
            .Where(policyVersionId => policyVersionId > 0)
            .Distinct()
            .ToArray();
        if (requestedPolicyVersionIds.Length == 0)
        {
            throw new BizException(
                BizErrorCode.PolicyStatusNotAllowed,
                400,
                "发布策略版本不能为空。");
        }

        foreach (var policyVersionId in requestedPolicyVersionIds)
        {
            var version = await _policyRepository.GetVersionAsync(policyVersionId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {policyVersionId}");
            var policy = await _policyRepository.GetByIdAsync(version.PolicyId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略不存在: {version.PolicyId}");

            await _eligibilityService.EnsureEligibleAsync(policy, version);
        }

        var result = await _compiler.CompileAsync(new RuntimePackageBuildContext
        {
            BuiltBy = publishedBy.Trim(),
            BuildAt = buildAt,
            PolicyVersionIds = requestedPolicyVersionIds,
            RequirePublishReadyStatus = false
        });

        foreach (var policyVersionId in requestedPolicyVersionIds)
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
