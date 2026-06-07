using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Application.Dto;

public class TemplateResponse
{
    public long TemplateId { get; init; }
    public string TemplateCode { get; init; } = string.Empty;
    public string TemplateName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string RiskLevel { get; init; } = string.Empty;
    public string ExpressionMode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int CurrentVersionNo { get; init; }
}

public sealed class TemplateDetailResponse : TemplateResponse
{
    public IReadOnlyList<TemplateVersionResponse> Versions { get; init; } = Array.Empty<TemplateVersionResponse>();
}

public sealed class TemplateVersionResponse
{
    public long TemplateVersionId { get; init; }
    public long TemplateId { get; init; }
    public int VersionNo { get; init; }
    public string VersionStatus { get; init; } = string.Empty;
    public string CapabilityFamily { get; init; } = string.Empty;
    public string MergeMode { get; init; } = string.Empty;
    public string? Checksum { get; init; }
    public string? Description { get; init; }
    public IReadOnlyList<TemplateParamDefDto> ParamDefs { get; init; } = Array.Empty<TemplateParamDefDto>();
    public IReadOnlyList<TemplateStepDefDto> StepDefs { get; init; } = Array.Empty<TemplateStepDefDto>();
    public IReadOnlyList<TemplateScopeDefDto> ScopeDefs { get; init; } = Array.Empty<TemplateScopeDefDto>();
}

public sealed class TemplateParamDefDto
{
    [Required]
    public string ParamCode { get; init; } = string.Empty;
    [Required]
    public string ParamName { get; init; } = string.Empty;
    [Required]
    public string ValueType { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public string? DefaultText { get; init; }
    public decimal? DefaultNumber { get; init; }
    public bool? DefaultBool { get; init; }
    public string? DictType { get; init; }
    public decimal? MinValue { get; init; }
    public decimal? MaxValue { get; init; }
    public string? RegexRule { get; init; }
    public string? UiControl { get; init; }
    public string? HelpText { get; init; }
    public string? RiskFlag { get; init; }
    public int SortNo { get; init; }
}

public sealed class TemplateStepDefDto
{
    public int StepNo { get; init; }
    [Required]
    public string StepKind { get; init; } = string.Empty;
    [Required]
    public string CapabilityCode { get; init; } = string.Empty;
    public string? ActionType { get; init; }
    public string? ExecutorCode { get; init; }
    public string OnError { get; init; } = "STOP";
    public string? StepConfigClob { get; init; }
}

public sealed class TemplateScopeDefDto
{
    [Required]
    public string ScopeDimension { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public bool AllowMultiple { get; init; }
    public int SortNo { get; init; }
}

public sealed class TemplateCreateRequest
{
    [Required, MaxLength(60)]
    public string TemplateCode { get; init; } = string.Empty;
    [Required, MaxLength(200)]
    public string TemplateName { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string Category { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string RiskLevel { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string ExpressionMode { get; init; } = string.Empty;
    public string? CreatedBy { get; init; }
}

public sealed class TemplateUpdateRequest
{
    [Required, MaxLength(200)]
    public string TemplateName { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string Category { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string RiskLevel { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string ExpressionMode { get; init; } = string.Empty;
    [Required, MaxLength(20)]
    public string Status { get; init; } = string.Empty;
    public string? UpdatedBy { get; init; }
}

public sealed class TemplateVersionSaveRequest
{
    public long? TemplateVersionId { get; init; }
    public int? VersionNo { get; init; }
    [Required, MaxLength(50)]
    public string CapabilityFamily { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string MergeMode { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Checksum { get; init; }
    public IReadOnlyList<TemplateParamDefDto> ParamDefs { get; init; } = Array.Empty<TemplateParamDefDto>();
    public IReadOnlyList<TemplateStepDefDto> StepDefs { get; init; } = Array.Empty<TemplateStepDefDto>();
    public IReadOnlyList<TemplateScopeDefDto> ScopeDefs { get; init; } = Array.Empty<TemplateScopeDefDto>();
}
