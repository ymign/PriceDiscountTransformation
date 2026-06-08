using System.Security.Cryptography;
using System.Text;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Templates;

namespace Pricing.RuleCenter.Application.RuntimePackages;

public sealed class RuntimePackageCompiler
{
    private readonly IPolicyRepository _policyRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IRuntimePackageRepository _runtimePackageRepository;
    private readonly IRuntimeRuleBuildRepository _runtimeRuleBuildRepository;
    private readonly PolicyValidationService _validationService;
    private readonly PolicyConflictService _conflictService;
    private readonly RuntimeRuleProjectionFactory _projectionFactory;
    private readonly IClock _clock;

    public RuntimePackageCompiler(
        IPolicyRepository policyRepository,
        ITemplateRepository templateRepository,
        IRuntimePackageRepository runtimePackageRepository,
        IRuntimeRuleBuildRepository runtimeRuleBuildRepository,
        PolicyValidationService validationService,
        PolicyConflictService conflictService,
        RuntimeRuleProjectionFactory projectionFactory,
        IClock clock)
    {
        _policyRepository = policyRepository;
        _templateRepository = templateRepository;
        _runtimePackageRepository = runtimePackageRepository;
        _runtimeRuleBuildRepository = runtimeRuleBuildRepository;
        _validationService = validationService;
        _conflictService = conflictService;
        _projectionFactory = projectionFactory;
        _clock = clock;
    }

    public async Task<RuntimePackageBuildResult> CompileAsync(RuntimePackageBuildContext context)
    {
        var buildAt = context.BuildAt ?? _clock.Now;
        var candidateVersions = await LoadCandidateVersionsAsync(context.PolicyVersionIds);
        var packagePolicies = new List<RuntimePackagePolicy>();
        var projections = new List<RuntimeRuleSnapshot>();
        var checksumBuilder = new StringBuilder();

        foreach (var version in candidateVersions)
        {
            var policy = await _policyRepository.GetByIdAsync(version.PolicyId)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略不存在，PolicyId={version.PolicyId}");
            var templateVersion = await _templateRepository.GetVersionAsync(version.TemplateVersionId)
                ?? throw new BizException(BizErrorCode.TemplateVersionNotFound, 404, $"模板版本不存在，TemplateVersionId={version.TemplateVersionId}");
            var bindings = await _policyRepository.GetBindingsAsync(version.PolicyVersionId);
            var scopes = await _policyRepository.GetScopesAsync(version.PolicyVersionId);
            var parameters = await _policyRepository.GetParamsAsync(version.PolicyVersionId);
            var paramDefs = await _templateRepository.GetParamDefsAsync(version.TemplateVersionId);
            var stepDefs = await _templateRepository.GetStepDefsAsync(version.TemplateVersionId);
            var scopeDefs = await _templateRepository.GetScopeDefsAsync(version.TemplateVersionId);

            _validationService.ValidateForCompile(
                policy,
                version,
                templateVersion,
                paramDefs,
                stepDefs,
                scopeDefs,
                bindings,
                scopes,
                parameters,
                context.RequirePublishReadyStatus);
            packagePolicies.Add(new RuntimePackagePolicy
            {
                PolicyVersionId = version.PolicyVersionId,
                PolicyCode = policy.PolicyCode,
                TemplateVersionId = templateVersion.TemplateVersionId,
                CapabilityFamily = templateVersion.CapabilityFamily
            });

            projections.AddRange(_projectionFactory.Create(version, templateVersion, bindings, scopes, parameters, stepDefs));
            checksumBuilder
                .Append(policy.PolicyCode).Append('|')
                .Append(version.PolicyVersionId).Append('|')
                .Append(version.Checksum).Append('|')
                .Append(templateVersion.TemplateVersionId).Append('|')
                .Append(templateVersion.Checksum).AppendLine();
        }

        _conflictService.EnsureNoConflicts(projections);

        var package = new RuntimePackage
        {
            PackageVersion = buildAt.Ticks,
            PackageStatus = RuntimePackageStatusCodes.Building,
            BuildScope = string.IsNullOrWhiteSpace(context.BuildScope) ? RuntimeBuildScopeCodes.Full : context.BuildScope,
            BuiltBy = context.BuiltBy,
            SourceChecksum = BuildChecksum(checksumBuilder.ToString())
        };

        package.PackageId = await _runtimePackageRepository.InsertAsync(package);
        await AssignIdentifiersAsync(package, packagePolicies, projections);

        await _runtimeRuleBuildRepository.InsertPackagePoliciesAsync(packagePolicies);
        await _runtimeRuleBuildRepository.InsertRulesAsync(projections.Select(snapshot => snapshot.Rule).ToList());
        await _runtimeRuleBuildRepository.InsertConditionsAsync(projections.SelectMany(snapshot => snapshot.Conditions).ToList());
        await _runtimeRuleBuildRepository.InsertActionsAsync(projections.SelectMany(snapshot => snapshot.Actions).ToList());

        package.PackageStatus = RuntimePackageStatusCodes.Built;
        package.BuiltAt = buildAt;
        await _runtimePackageRepository.UpdateAsync(package);

        return new RuntimePackageBuildResult
        {
            Package = package,
            PackagePolicies = packagePolicies,
            Rules = projections.Select(snapshot => snapshot.Rule).ToList(),
            Conditions = projections.SelectMany(snapshot => snapshot.Conditions).ToList(),
            Actions = projections.SelectMany(snapshot => snapshot.Actions).ToList()
        };
    }

    private async Task<IReadOnlyList<PolicyVersion>> LoadCandidateVersionsAsync(IReadOnlyCollection<long>? policyVersionIds)
    {
        if (policyVersionIds is null || policyVersionIds.Count == 0)
        {
            return await _policyRepository.GetPublishReadyVersionsAsync();
        }

        var requested = policyVersionIds.ToHashSet();
        var filtered = new List<PolicyVersion>(requested.Count);
        foreach (var policyVersionId in requested)
        {
            var version = await _policyRepository.GetVersionAsync(policyVersionId);
            if (version is not null)
            {
                filtered.Add(version);
            }
        }

        if (filtered.Count != requested.Count)
        {
            throw new BizException(
                BizErrorCode.PolicyNotFound,
                404,
                "存在不存在的策略版本，不能参与候选包构建。");
        }

        return filtered;
    }

    private async Task AssignIdentifiersAsync(
        RuntimePackage package,
        IReadOnlyList<RuntimePackagePolicy> packagePolicies,
        IReadOnlyList<RuntimeRuleSnapshot> projections)
    {
        var packagePolicyIds = await _runtimeRuleBuildRepository.ReservePackagePolicyIdsAsync(packagePolicies.Count);
        for (var i = 0; i < packagePolicies.Count; i++)
        {
            packagePolicies[i].PackagePolicyId = packagePolicyIds[i];
            packagePolicies[i].PackageId = package.PackageId;
            packagePolicies[i].PriorityKey = projections
                .Where(snapshot => snapshot.Rule.SourcePolicyVersionId == packagePolicies[i].PolicyVersionId)
                .Select(snapshot => snapshot.Rule.PriorityKey)
                .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault() ?? string.Empty;
        }

        var ruleIds = await _runtimeRuleBuildRepository.ReserveRuleIdsAsync(projections.Count);
        for (var i = 0; i < projections.Count; i++)
        {
            projections[i].Rule.RuntimeRuleId = ruleIds[i];
            projections[i].Rule.PackageId = package.PackageId;
        }

        var allConditions = projections.SelectMany(snapshot => snapshot.Conditions).ToList();
        var conditionIds = await _runtimeRuleBuildRepository.ReserveConditionIdsAsync(allConditions.Count);
        for (var i = 0; i < allConditions.Count; i++)
        {
            allConditions[i].RuntimeConditionId = conditionIds[i];
        }

        var allActions = projections.SelectMany(snapshot => snapshot.Actions).ToList();
        var actionIds = await _runtimeRuleBuildRepository.ReserveActionIdsAsync(allActions.Count);
        for (var i = 0; i < allActions.Count; i++)
        {
            allActions[i].RuntimeActionId = actionIds[i];
        }

        foreach (var snapshot in projections)
        {
            foreach (var condition in snapshot.Conditions)
            {
                condition.RuntimeRuleId = snapshot.Rule.RuntimeRuleId;
            }

            foreach (var action in snapshot.Actions)
            {
                action.RuntimeRuleId = snapshot.Rule.RuntimeRuleId;
            }
        }
    }

    private static string BuildChecksum(string source)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes);
    }
}
