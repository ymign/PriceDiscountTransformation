using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

public class TemplateResponse
{
    [JsonPropertyName("template_id")]
    public long TemplateId { get; init; }
    [JsonPropertyName("template_code")]
    public string TemplateCode { get; init; } = string.Empty;
    [JsonPropertyName("template_name")]
    public string TemplateName { get; init; } = string.Empty;
    [JsonPropertyName("category")]
    public string Category { get; init; } = string.Empty;
    [JsonPropertyName("risk_level")]
    public string RiskLevel { get; init; } = string.Empty;
    [JsonPropertyName("expression_mode")]
    public string ExpressionMode { get; init; } = string.Empty;
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
    [JsonPropertyName("current_version_no")]
    public int CurrentVersionNo { get; init; }
}

public sealed class TemplateDetailResponse : TemplateResponse
{
    [JsonPropertyName("versions")]
    public IReadOnlyList<TemplateVersionResponse> Versions { get; init; } = Array.Empty<TemplateVersionResponse>();
}

public sealed class TemplateVersionResponse
{
    [JsonPropertyName("template_version_id")]
    public long TemplateVersionId { get; init; }
    [JsonPropertyName("template_id")]
    public long TemplateId { get; init; }
    [JsonPropertyName("version_no")]
    public int VersionNo { get; init; }
    [JsonPropertyName("version_status")]
    public string VersionStatus { get; init; } = string.Empty;
    [JsonPropertyName("capability_family")]
    public string CapabilityFamily { get; init; } = string.Empty;
    [JsonPropertyName("merge_mode")]
    public string MergeMode { get; init; } = string.Empty;
    [JsonPropertyName("checksum")]
    public string? Checksum { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("param_defs")]
    public IReadOnlyList<TemplateParamDefDto> ParamDefs { get; init; } = Array.Empty<TemplateParamDefDto>();
    [JsonPropertyName("step_defs")]
    public IReadOnlyList<TemplateStepDefDto> StepDefs { get; init; } = Array.Empty<TemplateStepDefDto>();
    [JsonPropertyName("scope_defs")]
    public IReadOnlyList<TemplateScopeDefDto> ScopeDefs { get; init; } = Array.Empty<TemplateScopeDefDto>();
}

public sealed class TemplateParamDefDto
{
    [Required]
    [JsonPropertyName("param_code")]
    public string ParamCode { get; init; } = string.Empty;
    [Required]
    [JsonPropertyName("param_name")]
    public string ParamName { get; init; } = string.Empty;
    [Required]
    [JsonPropertyName("value_type")]
    public string ValueType { get; init; } = string.Empty;
    [JsonPropertyName("is_required")]
    public bool IsRequired { get; init; }
    [JsonPropertyName("default_text")]
    public string? DefaultText { get; init; }
    [JsonPropertyName("default_number")]
    public decimal? DefaultNumber { get; init; }
    [JsonPropertyName("default_bool")]
    public bool? DefaultBool { get; init; }
    [JsonPropertyName("dict_type")]
    public string? DictType { get; init; }
    [JsonPropertyName("min_value")]
    public decimal? MinValue { get; init; }
    [JsonPropertyName("max_value")]
    public decimal? MaxValue { get; init; }
    [JsonPropertyName("regex_rule")]
    public string? RegexRule { get; init; }
    [JsonPropertyName("ui_control")]
    public string? UiControl { get; init; }
    [JsonPropertyName("help_text")]
    public string? HelpText { get; init; }
    [JsonPropertyName("risk_flag")]
    public string? RiskFlag { get; init; }
    [JsonPropertyName("sort_no")]
    public int SortNo { get; init; }
}

public sealed class TemplateStepDefDto
{
    [JsonPropertyName("step_no")]
    public int StepNo { get; init; }
    [Required]
    [JsonPropertyName("step_kind")]
    public string StepKind { get; init; } = string.Empty;
    [Required]
    [JsonPropertyName("capability_code")]
    public string CapabilityCode { get; init; } = string.Empty;
    [JsonPropertyName("action_type")]
    public string? ActionType { get; init; }
    [JsonPropertyName("executor_code")]
    public string? ExecutorCode { get; init; }
    [JsonPropertyName("on_error")]
    public string OnError { get; init; } = "STOP";
    [JsonPropertyName("step_config_clob")]
    public string? StepConfigClob { get; init; }
}

public sealed class TemplateScopeDefDto
{
    [Required]
    [JsonPropertyName("scope_dimension")]
    public string ScopeDimension { get; init; } = string.Empty;
    [JsonPropertyName("is_required")]
    public bool IsRequired { get; init; }
    [JsonPropertyName("allow_multiple")]
    public bool AllowMultiple { get; init; }
    [JsonPropertyName("sort_no")]
    public int SortNo { get; init; }
}

public sealed class TemplateCreateRequest
{
    [Required, MaxLength(60)]
    [JsonPropertyName("template_code")]
    public string TemplateCode { get; init; } = string.Empty;
    [Required, MaxLength(200)]
    [JsonPropertyName("template_name")]
    public string TemplateName { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("category")]
    public string Category { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("risk_level")]
    public string RiskLevel { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("expression_mode")]
    public string ExpressionMode { get; init; } = string.Empty;
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; init; }
}

public sealed class TemplateUpdateRequest
{
    [Required, MaxLength(200)]
    [JsonPropertyName("template_name")]
    public string TemplateName { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("category")]
    public string Category { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("risk_level")]
    public string RiskLevel { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("expression_mode")]
    public string ExpressionMode { get; init; } = string.Empty;
    [Required, MaxLength(20)]
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
    [JsonPropertyName("updated_by")]
    public string? UpdatedBy { get; init; }
}

public sealed class TemplateVersionSaveRequest
{
    [JsonPropertyName("template_version_id")]
    public long? TemplateVersionId { get; init; }
    [JsonPropertyName("version_no")]
    public int? VersionNo { get; init; }
    [Required, MaxLength(50)]
    [JsonPropertyName("capability_family")]
    public string CapabilityFamily { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("merge_mode")]
    public string MergeMode { get; init; } = string.Empty;
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("checksum")]
    public string? Checksum { get; init; }
    [JsonPropertyName("param_defs")]
    public IReadOnlyList<TemplateParamDefDto> ParamDefs { get; init; } = Array.Empty<TemplateParamDefDto>();
    [JsonPropertyName("step_defs")]
    public IReadOnlyList<TemplateStepDefDto> StepDefs { get; init; } = Array.Empty<TemplateStepDefDto>();
    [JsonPropertyName("scope_defs")]
    public IReadOnlyList<TemplateScopeDefDto> ScopeDefs { get; init; } = Array.Empty<TemplateScopeDefDto>();
}
