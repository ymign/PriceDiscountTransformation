using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Templates;

namespace Pricing.RuleCenter.Application.Templates;

public sealed class TemplateVersionAppService
{
    private readonly ITemplateRepository _templateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public TemplateVersionAppService(
        ITemplateRepository templateRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<TemplateVersionResponse?> GetByIdAsync(long templateVersionId)
    {
        var version = await _templateRepository.GetVersionAsync(templateVersionId);
        if (version is null)
        {
            return null;
        }

        return await BuildResponseAsync(version);
    }

    public async Task<long> SaveAsync(long templateId, TemplateVersionSaveRequest request)
    {
        var template = await _templateRepository.GetByIdAsync(templateId)
            ?? throw new BizException(BizErrorCode.TemplateNotFound, 404, $"模板不存在: {templateId}");

        var versions = await _templateRepository.GetVersionsByTemplateIdAsync(templateId);
        var version = request.TemplateVersionId.HasValue
            ? await _templateRepository.GetVersionAsync(request.TemplateVersionId.Value)
                ?? throw new BizException(BizErrorCode.TemplateVersionNotFound, 404, $"模板版本不存在: {request.TemplateVersionId.Value}")
            : new TemplateVersion
            {
                TemplateId = templateId,
                VersionNo = request.VersionNo ?? ((versions.Count == 0 ? 0 : versions.Max(item => item.VersionNo)) + 1),
                VersionStatus = TemplateLifecycleCodes.Draft
            };

        version.CapabilityFamily = request.CapabilityFamily.Trim();
        version.MergeMode = request.MergeMode.Trim();
        version.Description = request.Description?.Trim();
        version.Checksum = request.Checksum?.Trim();

        await _unitOfWork.BeginAsync();
        try
        {
            if (version.TemplateVersionId == 0)
            {
                version.TemplateVersionId = await _templateRepository.InsertVersionAsync(version);
            }
            else
            {
                await _templateRepository.UpdateVersionAsync(version);
            }

            await _templateRepository.ReplaceParamDefsAsync(version.TemplateVersionId, request.ParamDefs.Select(MapParamDef).ToList());
            await _templateRepository.ReplaceStepDefsAsync(version.TemplateVersionId, request.StepDefs.Select(MapStepDef).ToList());
            await _templateRepository.ReplaceScopeDefsAsync(version.TemplateVersionId, request.ScopeDefs.Select(MapScopeDef).ToList());

            template.CurrentVersionNo = Math.Max(template.CurrentVersionNo, version.VersionNo);
            template.UpdatedAt = _clock.Now;
            await _templateRepository.UpdateAsync(template);

            await _unitOfWork.CommitAsync();
            return version.TemplateVersionId;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task<TemplateVersionResponse> BuildResponseAsync(TemplateVersion version)
    {
        var paramDefs = await _templateRepository.GetParamDefsAsync(version.TemplateVersionId);
        var stepDefs = await _templateRepository.GetStepDefsAsync(version.TemplateVersionId);
        var scopeDefs = await _templateRepository.GetScopeDefsAsync(version.TemplateVersionId);
        return new TemplateVersionResponse
        {
            TemplateVersionId = version.TemplateVersionId,
            TemplateId = version.TemplateId,
            VersionNo = version.VersionNo,
            VersionStatus = version.VersionStatus,
            CapabilityFamily = version.CapabilityFamily,
            MergeMode = version.MergeMode,
            Checksum = version.Checksum,
            Description = version.Description,
            ParamDefs = paramDefs.Select(item => new TemplateParamDefDto
            {
                ParamCode = item.ParamCode,
                ParamName = item.ParamName,
                ValueType = item.ValueType,
                IsRequired = item.IsRequired == EnableFlag.Yes,
                DefaultText = item.DefaultText,
                DefaultNumber = item.DefaultNumber,
                DefaultBool = item.DefaultBool == EnableFlag.Yes ? true : item.DefaultBool == EnableFlag.No ? false : null,
                DictType = item.DictType,
                MinValue = item.MinValue,
                MaxValue = item.MaxValue,
                RegexRule = item.RegexRule,
                UiControl = item.UiControl,
                HelpText = item.HelpText,
                RiskFlag = item.RiskFlag,
                SortNo = item.SortNo
            }).ToList(),
            StepDefs = stepDefs.Select(item => new TemplateStepDefDto
            {
                StepNo = item.StepNo,
                StepKind = item.StepKind,
                CapabilityCode = item.CapabilityCode,
                ActionType = item.ActionType,
                ExecutorCode = item.ExecutorCode,
                OnError = item.OnError,
                StepConfigClob = item.StepConfigClob
            }).ToList(),
            ScopeDefs = scopeDefs.Select(item => new TemplateScopeDefDto
            {
                ScopeDimension = item.ScopeDimension,
                IsRequired = item.IsRequired == EnableFlag.Yes,
                AllowMultiple = item.AllowMultiple == EnableFlag.Yes,
                SortNo = item.SortNo
            }).ToList()
        };
    }

    private static TemplateParamDef MapParamDef(TemplateParamDefDto dto)
    {
        return new TemplateParamDef
        {
            ParamCode = dto.ParamCode.Trim(),
            ParamName = dto.ParamName.Trim(),
            ValueType = dto.ValueType.Trim(),
            IsRequired = dto.IsRequired ? EnableFlag.Yes : EnableFlag.No,
            DefaultText = dto.DefaultText,
            DefaultNumber = dto.DefaultNumber,
            DefaultBool = dto.DefaultBool.HasValue ? (dto.DefaultBool.Value ? EnableFlag.Yes : EnableFlag.No) : null,
            DictType = dto.DictType,
            MinValue = dto.MinValue,
            MaxValue = dto.MaxValue,
            RegexRule = dto.RegexRule,
            UiControl = dto.UiControl,
            HelpText = dto.HelpText,
            RiskFlag = dto.RiskFlag,
            SortNo = dto.SortNo
        };
    }

    private static TemplateStepDef MapStepDef(TemplateStepDefDto dto)
    {
        return new TemplateStepDef
        {
            StepNo = dto.StepNo,
            StepKind = dto.StepKind.Trim(),
            CapabilityCode = dto.CapabilityCode.Trim(),
            ActionType = dto.ActionType?.Trim(),
            ExecutorCode = dto.ExecutorCode?.Trim(),
            OnError = dto.OnError.Trim(),
            StepConfigClob = dto.StepConfigClob
        };
    }

    private static TemplateScopeDef MapScopeDef(TemplateScopeDefDto dto)
    {
        return new TemplateScopeDef
        {
            ScopeDimension = dto.ScopeDimension.Trim(),
            IsRequired = dto.IsRequired ? EnableFlag.Yes : EnableFlag.No,
            AllowMultiple = dto.AllowMultiple ? EnableFlag.Yes : EnableFlag.No,
            SortNo = dto.SortNo
        };
    }
}
