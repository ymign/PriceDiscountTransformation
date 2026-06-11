using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces.Runtime;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 运行时包策略差异分析服务。
/// </summary>
public sealed class PolicyPackageDiffService
{
    private readonly IRuntimePackageStateRepository _runtimePackageStateRepository;
    private readonly IRuntimePackageRepository _runtimePackageRepository;

    /// <summary>
    /// 初始化运行时包策略差异分析服务。
    /// </summary>
    public PolicyPackageDiffService(
        IRuntimePackageStateRepository runtimePackageStateRepository,
        IRuntimePackageRepository runtimePackageRepository)
    {
        _runtimePackageStateRepository = runtimePackageStateRepository;
        _runtimePackageRepository = runtimePackageRepository;
    }

    /// <summary>
    /// 比较候选运行时包与当前激活包之间的策略版本差异。
    /// </summary>
    public async Task<PolicyPackageDiffResult> DiffAgainstActiveAsync(long candidatePackageId)
    {
        var candidatePackage = await _runtimePackageRepository.GetByIdAsync(candidatePackageId)
            ?? throw new BizException(BizErrorCode.RuntimePackageNotFound, 404, $"运行时包不存在: {candidatePackageId}");
        var activeState = await _runtimePackageStateRepository.GetActiveAsync();

        var candidatePolicies = await _runtimePackageRepository.GetPackagePoliciesAsync(candidatePackage.PackageId);
        var activePolicies = activeState is null || activeState.ActivePackageId <= 0
            ? Array.Empty<Core.Aggregates.Runtime.RuntimePackagePolicy>()
            : await _runtimePackageRepository.GetPackagePoliciesAsync(activeState.ActivePackageId);

        var candidateIds = candidatePolicies.Select(item => item.PolicyVersionId).ToHashSet();
        var activeIds = activePolicies.Select(item => item.PolicyVersionId).ToHashSet();

        return new PolicyPackageDiffResult
        {
            CandidatePackageId = candidatePackage.PackageId,
            ActivePackageId = activeState?.ActivePackageId,
            AddedPolicyVersionIds = candidateIds.Except(activeIds).OrderBy(id => id).ToList(),
            RemovedPolicyVersionIds = activeIds.Except(candidateIds).OrderBy(id => id).ToList(),
            UnchangedPolicyVersionIds = candidateIds.Intersect(activeIds).OrderBy(id => id).ToList()
        };
    }
}
