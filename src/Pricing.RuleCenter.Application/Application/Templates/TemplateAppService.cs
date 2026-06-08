using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Templates;

namespace Pricing.RuleCenter.Application.Templates;

public sealed class TemplateAppService
{
    private readonly ITemplateRepository _templateRepository;
    private readonly IClock _clock;

    public TemplateAppService(ITemplateRepository templateRepository, IClock clock)
    {
        _templateRepository = templateRepository;
        _clock = clock;
    }

    public async Task<IReadOnlyList<TemplateResponse>> GetAllAsync()
    {
        var items = await _templateRepository.GetAllAsync();
        return items.Select(MapToResponse).ToList();
    }

    public async Task<TemplateDetailResponse?> GetByIdAsync(long templateId)
    {
        var template = await _templateRepository.GetByIdAsync(templateId);
        if (template is null)
        {
            return null;
        }

        var versions = await _templateRepository.GetVersionsByTemplateIdAsync(templateId);
        return new TemplateDetailResponse
        {
            TemplateId = template.TemplateId,
            TemplateCode = template.TemplateCode,
            TemplateName = template.TemplateName,
            Category = template.Category,
            RiskLevel = template.RiskLevel,
            ExpressionMode = template.ExpressionMode,
            Status = template.Status,
            CurrentVersionNo = template.CurrentVersionNo,
            Versions = versions.Select(MapToVersionSummary).ToList()
        };
    }

    public async Task<long> CreateAsync(TemplateCreateRequest request)
    {
        var existing = await _templateRepository.GetByCodeAsync(request.TemplateCode);
        if (existing is not null)
        {
            throw new BizException(BizErrorCode.ResourceAlreadyExists, 409, $"模板编码已存在: {request.TemplateCode}");
        }

        var entity = new TemplateAggregate
        {
            TemplateCode = request.TemplateCode.Trim(),
            TemplateName = request.TemplateName.Trim(),
            Category = request.Category.Trim(),
            RiskLevel = request.RiskLevel.Trim(),
            ExpressionMode = request.ExpressionMode.Trim(),
            Status = TemplateLifecycleCodes.Draft,
            CurrentVersionNo = 0,
            CreatedBy = request.CreatedBy?.Trim(),
            CreatedAt = _clock.Now,
            UpdatedBy = request.CreatedBy?.Trim(),
            UpdatedAt = _clock.Now
        };

        return await _templateRepository.InsertAsync(entity);
    }

    public async Task UpdateAsync(long templateId, TemplateUpdateRequest request)
    {
        var entity = await _templateRepository.GetByIdAsync(templateId)
            ?? throw new BizException(BizErrorCode.TemplateNotFound, 404, $"模板不存在: {templateId}");

        entity.TemplateName = request.TemplateName.Trim();
        entity.Category = request.Category.Trim();
        entity.RiskLevel = request.RiskLevel.Trim();
        entity.ExpressionMode = request.ExpressionMode.Trim();
        entity.Status = request.Status.Trim();
        entity.UpdatedBy = request.UpdatedBy?.Trim();
        entity.UpdatedAt = _clock.Now;
        await _templateRepository.UpdateAsync(entity);
    }

    private static TemplateResponse MapToResponse(TemplateAggregate entity)
    {
        return new TemplateResponse
        {
            TemplateId = entity.TemplateId,
            TemplateCode = entity.TemplateCode,
            TemplateName = entity.TemplateName,
            Category = entity.Category,
            RiskLevel = entity.RiskLevel,
            ExpressionMode = entity.ExpressionMode,
            Status = entity.Status,
            CurrentVersionNo = entity.CurrentVersionNo
        };
    }

    private static TemplateVersionResponse MapToVersionSummary(TemplateVersion entity)
    {
        return new TemplateVersionResponse
        {
            TemplateVersionId = entity.TemplateVersionId,
            TemplateId = entity.TemplateId,
            VersionNo = entity.VersionNo,
            VersionStatus = entity.VersionStatus,
            CapabilityFamily = entity.CapabilityFamily,
            MergeMode = entity.MergeMode,
            Checksum = entity.Checksum,
            Description = entity.Description
        };
    }
}
