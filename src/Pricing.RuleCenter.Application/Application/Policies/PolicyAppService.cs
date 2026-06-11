using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Policies;

namespace Pricing.RuleCenter.Application.Policies;

/// <summary>
/// 策略主档应用服务，负责策略主记录的查询、创建和更新。
/// </summary>
public sealed class PolicyAppService
{
    private readonly IPolicyRepository _policyRepository;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化策略主档应用服务。
    /// </summary>
    /// <param name="policyRepository">策略仓储。</param>
    /// <param name="clock">技术时间提供者。</param>
    public PolicyAppService(IPolicyRepository policyRepository, IClock clock)
    {
        _policyRepository = policyRepository;
        _clock = clock;
    }

    /// <summary>
    /// 查询全部策略主档列表。
    /// </summary>
    /// <returns>策略概要集合。</returns>
    public async Task<IReadOnlyList<PolicyResponse>> GetAllAsync()
    {
        var items = await _policyRepository.GetAllAsync();
        return items.Select(MapToResponse).ToList();
    }

    /// <summary>
    /// 按主键查询策略详情及版本概要。
    /// </summary>
    /// <param name="policyId">策略主键。</param>
    /// <returns>命中时返回策略详情；不存在时返回 <c>null</c>。</returns>
    public async Task<PolicyDetailResponse?> GetByIdAsync(long policyId)
    {
        var policy = await _policyRepository.GetByIdAsync(policyId);
        if (policy is null)
        {
            return null;
        }

        var versions = await _policyRepository.GetVersionsByPolicyIdAsync(policyId);
        return new PolicyDetailResponse
        {
            PolicyId = policy.PolicyId,
            PolicyCode = policy.PolicyCode,
            PolicyName = policy.PolicyName,
            TemplateId = policy.TemplateId,
            OwnerType = policy.OwnerType,
            PublishProfile = policy.PublishProfile,
            Status = policy.Status,
            CurrentVersionNo = policy.CurrentVersionNo,
            Versions = versions.Select(MapToVersionSummary).ToList()
        };
    }

    /// <summary>
    /// 创建策略主档。
    /// </summary>
    /// <param name="request">策略创建请求。</param>
    /// <returns>新建策略主键。</returns>
    public async Task<long> CreateAsync(PolicyCreateRequest request)
    {
        var existing = await _policyRepository.GetByCodeAsync(request.PolicyCode);
        if (existing is not null)
        {
            throw new BizException(BizErrorCode.ResourceAlreadyExists, 409, $"策略编码已存在: {request.PolicyCode}");
        }

        var entity = new PolicyAggregate
        {
            PolicyCode = request.PolicyCode.Trim(),
            PolicyName = request.PolicyName.Trim(),
            TemplateId = request.TemplateId,
            OwnerType = request.OwnerType.Trim(),
            PublishProfile = request.PublishProfile.Trim(),
            Status = PolicyLifecycleCodes.Draft,
            CurrentVersionNo = 0,
            CreatedBy = request.CreatedBy?.Trim(),
            CreatedAt = _clock.Now,
            UpdatedBy = request.CreatedBy?.Trim(),
            UpdatedAt = _clock.Now
        };

        return await _policyRepository.InsertAsync(entity);
    }

    /// <summary>
    /// 更新策略主档基础信息。
    /// </summary>
    /// <param name="policyId">策略主键。</param>
    /// <param name="request">策略更新请求。</param>
    public async Task UpdateAsync(long policyId, PolicyUpdateRequest request)
    {
        var entity = await _policyRepository.GetByIdAsync(policyId)
            ?? throw new BizException(BizErrorCode.PolicyNotFound, 404, $"策略不存在: {policyId}");

        entity.PolicyName = request.PolicyName.Trim();
        entity.OwnerType = request.OwnerType.Trim();
        entity.PublishProfile = request.PublishProfile.Trim();
        entity.Status = request.Status.Trim();
        entity.UpdatedBy = request.UpdatedBy?.Trim();
        entity.UpdatedAt = _clock.Now;
        await _policyRepository.UpdateAsync(entity);
    }

    private static PolicyResponse MapToResponse(PolicyAggregate entity)
    {
        return new PolicyResponse
        {
            PolicyId = entity.PolicyId,
            PolicyCode = entity.PolicyCode,
            PolicyName = entity.PolicyName,
            TemplateId = entity.TemplateId,
            OwnerType = entity.OwnerType,
            PublishProfile = entity.PublishProfile,
            Status = entity.Status,
            CurrentVersionNo = entity.CurrentVersionNo
        };
    }

    private static PolicyVersionResponse MapToVersionSummary(PolicyVersion entity)
    {
        return new PolicyVersionResponse
        {
            PolicyVersionId = entity.PolicyVersionId,
            PolicyId = entity.PolicyId,
            TemplateVersionId = entity.TemplateVersionId,
            VersionNo = entity.VersionNo,
            PolicyStatus = entity.PolicyStatus,
            EffectiveFrom = entity.EffectiveFrom,
            EffectiveTo = entity.EffectiveTo,
            BindingType = entity.BindingType,
            ScopeLevel = entity.ScopeLevel,
            PriorityWeight = entity.PriorityWeight,
            Checksum = entity.Checksum,
            LastBuiltPackageId = entity.LastBuiltPackageId
        };
    }
}
