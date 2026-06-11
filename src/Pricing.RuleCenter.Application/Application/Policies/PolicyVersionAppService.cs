using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Templates;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略版本应用服务，负责草稿版本读取、保存和发布前校验。
/// </summary>
public sealed class PolicyVersionAppService
{
    private readonly IPolicyRepository _policyRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IPolicyExpressionGuard _expressionGuard;
    private readonly IPolicyValidationService _validationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化策略版本应用服务。
    /// </summary>
    public PolicyVersionAppService(
        IPolicyRepository policyRepository,
        ITemplateRepository templateRepository,
        IPolicyExpressionGuard expressionGuard,
        IPolicyValidationService validationService,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _policyRepository = policyRepository;
        _templateRepository = templateRepository;
        _expressionGuard = expressionGuard;
        _validationService = validationService;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    /// <summary>
    /// 按主键查询策略版本详情。
    /// </summary>
    public async Task<PolicyVersionResponse?> GetByIdAsync(long policyVersionId)
    {
        var version = await _policyRepository.GetVersionAsync(policyVersionId);
        if (version is null)
        {
            return null;
        }

        return await BuildResponseAsync(version);
    }

    /// <summary>
    /// 创建或更新策略草稿版本。
    /// </summary>
    public async Task<long> SaveDraftAsync(long policyId, PolicyVersionSaveRequest request)
    {
        var policy = await _policyRepository.GetByIdAsync(policyId)
            ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略不存在: {policyId}");
        _expressionGuard.EnsureAllowed(request.Params);

        var versions = await _policyRepository.GetVersionsByPolicyIdAsync(policyId);
        var version = request.PolicyVersionId.HasValue
            ? await _policyRepository.GetVersionAsync(request.PolicyVersionId.Value)
                ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略版本不存在: {request.PolicyVersionId.Value}")
            : new PolicyVersion
            {
                PolicyId = policyId,
                VersionNo = request.VersionNo ?? ((versions.Count == 0 ? 0 : versions.Max(item => item.VersionNo)) + 1)
            };
        if (version.PolicyId != policyId)
        {
            throw new BizException(
                BizErrorCode.PolicyNotFound,
                404,
                $"策略版本 {version.PolicyVersionId} 不属于策略 {policyId}。");
        }

        version.TemplateVersionId = request.TemplateVersionId;
        version.BindingType = request.BindingType.Trim();
        version.ScopeLevel = request.ScopeLevel.Trim();
        version.PriorityWeight = request.PriorityWeight;
        version.EffectiveFrom = request.EffectiveFrom;
        version.EffectiveTo = request.EffectiveTo;
        version.PolicyStatus = PolicyLifecycleCodes.Draft;
        version.Checksum = string.IsNullOrWhiteSpace(request.Checksum)
            ? ComputeChecksum(request)
            : request.Checksum.Trim();

        await _unitOfWork.BeginAsync();
        try
        {
            if (version.PolicyVersionId == 0)
            {
                version.PolicyVersionId = await _policyRepository.InsertVersionAsync(version);
            }
            else
            {
                await _policyRepository.UpdateVersionAsync(version);
            }

            await _policyRepository.ReplaceBindingsAsync(version.PolicyVersionId, request.Bindings.Select(MapBinding).ToList());
            await _policyRepository.ReplaceScopesAsync(version.PolicyVersionId, request.Scopes.Select(MapScope).ToList());
            await _policyRepository.ReplaceParamsAsync(version.PolicyVersionId, request.Params.Select(MapParam).ToList());

            policy.CurrentVersionNo = Math.Max(policy.CurrentVersionNo, version.VersionNo);
            policy.UpdatedAt = _clock.Now;
            await _policyRepository.UpdateAsync(policy);

            await _unitOfWork.CommitAsync();
            return version.PolicyVersionId;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// 校验指定策略版本是否达到可发布状态。
    /// </summary>
    public async Task<PolicyValidateResponse> ValidateAsync(long policyVersionId)
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
            CloneForValidation(version),
            templateVersion,
            paramDefs,
            stepDefs,
            scopeDefs,
            bindings,
            scopes,
            parameters);
        version.PolicyStatus = PolicyLifecycleCodes.Validated;
        await _policyRepository.UpdateVersionAsync(version);
        return new PolicyValidateResponse
        {
            PolicyVersionId = version.PolicyVersionId,
            PolicyStatus = version.PolicyStatus
        };
    }

    private async Task<PolicyVersionResponse> BuildResponseAsync(PolicyVersion version)
    {
        var bindings = await _policyRepository.GetBindingsAsync(version.PolicyVersionId);
        var scopes = await _policyRepository.GetScopesAsync(version.PolicyVersionId);
        var parameters = await _policyRepository.GetParamsAsync(version.PolicyVersionId);
        return new PolicyVersionResponse
        {
            PolicyVersionId = version.PolicyVersionId,
            PolicyId = version.PolicyId,
            TemplateVersionId = version.TemplateVersionId,
            VersionNo = version.VersionNo,
            PolicyStatus = version.PolicyStatus,
            EffectiveFrom = version.EffectiveFrom,
            EffectiveTo = version.EffectiveTo,
            BindingType = version.BindingType,
            ScopeLevel = version.ScopeLevel,
            PriorityWeight = version.PriorityWeight,
            Checksum = version.Checksum,
            LastBuiltPackageId = version.LastBuiltPackageId,
            Bindings = bindings.Select(item => new PolicyBindingDto
            {
                BindingType = item.BindingType,
                ItemCode = item.ItemCode,
                ItemName = item.ItemName,
                GroupCode = item.GroupCode,
                GroupName = item.GroupName
            }).ToList(),
            Scopes = scopes.Select(item => new PolicyScopeDto
            {
                ScopeDimension = item.ScopeDimension,
                ScopeOperator = item.ScopeOperator,
                ScopeValueText = item.ScopeValueText,
                ScopeValueNumber = item.ScopeValueNumber,
                ScopeValueDate = item.ScopeValueDate,
                ScopeJson = item.ScopeJson
            }).ToList(),
            Params = parameters.Select(item => new PolicyParamDto
            {
                ParamCode = item.ParamCode,
                ValueType = item.ValueType,
                ValueText = item.ValueText,
                ValueNumber = item.ValueNumber,
                ValueDate = item.ValueDate,
                ValueBool = item.ValueBool == EnableFlag.Yes ? true : item.ValueBool == EnableFlag.No ? false : null,
                ExprText = item.ExprText,
                ExprLevel = item.ExprLevel
            }).ToList()
        };
    }

    private static PolicyBinding MapBinding(PolicyBindingDto dto)
    {
        return new PolicyBinding
        {
            BindingType = dto.BindingType.Trim(),
            ItemCode = dto.ItemCode?.Trim(),
            ItemName = dto.ItemName?.Trim(),
            GroupCode = dto.GroupCode?.Trim(),
            GroupName = dto.GroupName?.Trim()
        };
    }

    private static PolicyScope MapScope(PolicyScopeDto dto)
    {
        return new PolicyScope
        {
            ScopeDimension = dto.ScopeDimension.Trim(),
            ScopeOperator = dto.ScopeOperator.Trim(),
            ScopeValueText = dto.ScopeValueText?.Trim(),
            ScopeValueNumber = dto.ScopeValueNumber,
            ScopeValueDate = dto.ScopeValueDate,
            ScopeJson = dto.ScopeJson
        };
    }

    private static PolicyParam MapParam(PolicyParamDto dto)
    {
        return new PolicyParam
        {
            ParamCode = dto.ParamCode.Trim(),
            ValueType = dto.ValueType.Trim(),
            ValueText = dto.ValueText?.Trim(),
            ValueNumber = dto.ValueNumber,
            ValueDate = dto.ValueDate,
            ValueBool = dto.ValueBool.HasValue ? (dto.ValueBool.Value ? EnableFlag.Yes : EnableFlag.No) : null,
            ExprText = dto.ExprText?.Trim(),
            ExprLevel = dto.ExprLevel?.Trim()
        };
    }

    private static string ComputeChecksum(PolicyVersionSaveRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json)));
    }

    private static PolicyVersion CloneForValidation(PolicyVersion source)
    {
        return new PolicyVersion
        {
            PolicyVersionId = source.PolicyVersionId,
            PolicyId = source.PolicyId,
            TemplateVersionId = source.TemplateVersionId,
            VersionNo = source.VersionNo,
            PolicyStatus = PolicyLifecycleCodes.PublishReady,
            EffectiveFrom = source.EffectiveFrom,
            EffectiveTo = source.EffectiveTo,
            BindingType = source.BindingType,
            ScopeLevel = source.ScopeLevel,
            PriorityWeight = source.PriorityWeight,
            Checksum = source.Checksum,
            LastBuiltPackageId = source.LastBuiltPackageId
        };
    }
}
