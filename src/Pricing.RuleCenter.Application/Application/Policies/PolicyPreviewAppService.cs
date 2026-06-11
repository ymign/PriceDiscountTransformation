using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Templates;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略预览应用服务，负责把策略版本解析为可读预览信息。
/// </summary>
public sealed class PolicyPreviewAppService
{
    private readonly IPolicyRepository _policyRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IPolicyValidationService _validationService;

    /// <summary>
    /// 初始化策略预览应用服务。
    /// </summary>
    public PolicyPreviewAppService(
        IPolicyRepository policyRepository,
        ITemplateRepository templateRepository,
        IPolicyValidationService validationService)
    {
        _policyRepository = policyRepository;
        _templateRepository = templateRepository;
        _validationService = validationService;
    }

    /// <summary>
    /// 预览指定策略版本的绑定、作用域和动作链。
    /// </summary>
    public async Task<PolicyPreviewResponse> PreviewAsync(long policyVersionId)
    {
        var version = await _policyRepository.GetVersionAsync(policyVersionId)
            ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {policyVersionId}");
        var policy = await _policyRepository.GetByIdAsync(version.PolicyId)
            ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略不存在: {version.PolicyId}");
        var templateVersion = await _templateRepository.GetVersionAsync(version.TemplateVersionId)
            ?? throw new BizException(BizErrorCode.TemplateVersionNotFound, 404, $"模板版本不存在: {version.TemplateVersionId}");
        var bindings = await _policyRepository.GetBindingsAsync(policyVersionId);
        var scopes = await _policyRepository.GetScopesAsync(policyVersionId);
        var parameters = await _policyRepository.GetParamsAsync(policyVersionId);
        var paramDefs = await _templateRepository.GetParamDefsAsync(version.TemplateVersionId);
        var stepDefs = await _templateRepository.GetStepDefsAsync(version.TemplateVersionId);
        var scopeDefs = await _templateRepository.GetScopeDefsAsync(version.TemplateVersionId);

        _validationService.ValidateForCompile(
            policy,
            new Core.Aggregates.Policies.PolicyVersion
            {
                PolicyVersionId = version.PolicyVersionId,
                PolicyId = version.PolicyId,
                TemplateVersionId = version.TemplateVersionId,
                VersionNo = version.VersionNo,
                PolicyStatus = PolicyLifecycleCodes.PublishReady,
                EffectiveFrom = version.EffectiveFrom,
                EffectiveTo = version.EffectiveTo,
                BindingType = version.BindingType,
                ScopeLevel = version.ScopeLevel,
                PriorityWeight = version.PriorityWeight,
                Checksum = version.Checksum,
                LastBuiltPackageId = version.LastBuiltPackageId
            },
            templateVersion,
            paramDefs,
            stepDefs,
            scopeDefs,
            bindings,
            scopes,
            parameters);

        return new PolicyPreviewResponse
        {
            PolicyVersionId = version.PolicyVersionId,
            PolicyCode = policy.PolicyCode,
            TemplateVersionId = templateVersion.TemplateVersionId,
            CapabilityFamily = templateVersion.CapabilityFamily,
            MergeMode = templateVersion.MergeMode,
            BindingSummary = bindings.Select(binding =>
                    !string.IsNullOrWhiteSpace(binding.ItemCode)
                        ? $"ITEM:{binding.ItemCode}"
                        : $"GROUP:{binding.GroupCode}")
                .ToList(),
            ScopeSummary = scopes.Select(scope =>
                    $"{scope.ScopeDimension} {scope.ScopeOperator} {scope.ScopeValueText ?? scope.ScopeValueNumber?.ToString() ?? scope.ScopeValueDate?.ToString("yyyy-MM-dd") ?? scope.ScopeJson}")
                .ToList(),
            ActionChain = stepDefs
                .Where(step => string.Equals(step.StepKind, "ACTION", StringComparison.OrdinalIgnoreCase))
                .OrderBy(step => step.StepNo)
                .Select(step => $"{step.StepNo}:{step.ActionType}/{step.ExecutorCode}")
                .ToList()
        };
    }
}
