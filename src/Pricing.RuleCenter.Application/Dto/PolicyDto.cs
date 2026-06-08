using System.ComponentModel.DataAnnotations;
using Pricing.RuleCenter.Application.Policies;
using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

public class PolicyResponse
{
    [JsonPropertyName("policy_id")]
    public long PolicyId { get; init; }
    [JsonPropertyName("policy_code")]
    public string PolicyCode { get; init; } = string.Empty;
    [JsonPropertyName("policy_name")]
    public string PolicyName { get; init; } = string.Empty;
    [JsonPropertyName("template_id")]
    public long TemplateId { get; init; }
    [JsonPropertyName("owner_type")]
    public string OwnerType { get; init; } = string.Empty;
    [JsonPropertyName("publish_profile")]
    public string PublishProfile { get; init; } = string.Empty;
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
    [JsonPropertyName("current_version_no")]
    public int CurrentVersionNo { get; init; }
}

public sealed class PolicyDetailResponse : PolicyResponse
{
    [JsonPropertyName("versions")]
    public IReadOnlyList<PolicyVersionResponse> Versions { get; init; } = Array.Empty<PolicyVersionResponse>();
}

public sealed class PolicyVersionResponse
{
    [JsonPropertyName("policy_version_id")]
    public long PolicyVersionId { get; init; }
    [JsonPropertyName("policy_id")]
    public long PolicyId { get; init; }
    [JsonPropertyName("template_version_id")]
    public long TemplateVersionId { get; init; }
    [JsonPropertyName("version_no")]
    public int VersionNo { get; init; }
    [JsonPropertyName("policy_status")]
    public string PolicyStatus { get; init; } = string.Empty;
    [JsonPropertyName("effective_from")]
    public DateTime? EffectiveFrom { get; init; }
    [JsonPropertyName("effective_to")]
    public DateTime? EffectiveTo { get; init; }
    [JsonPropertyName("binding_type")]
    public string BindingType { get; init; } = string.Empty;
    [JsonPropertyName("scope_level")]
    public string ScopeLevel { get; init; } = string.Empty;
    [JsonPropertyName("priority_weight")]
    public int PriorityWeight { get; init; }
    [JsonPropertyName("checksum")]
    public string? Checksum { get; init; }
    [JsonPropertyName("last_built_package_id")]
    public long? LastBuiltPackageId { get; init; }
    [JsonPropertyName("bindings")]
    public IReadOnlyList<PolicyBindingDto> Bindings { get; init; } = Array.Empty<PolicyBindingDto>();
    [JsonPropertyName("scopes")]
    public IReadOnlyList<PolicyScopeDto> Scopes { get; init; } = Array.Empty<PolicyScopeDto>();
    [JsonPropertyName("params")]
    public IReadOnlyList<PolicyParamDto> Params { get; init; } = Array.Empty<PolicyParamDto>();
}

public sealed class PolicyBindingDto
{
    [Required]
    [JsonPropertyName("binding_type")]
    public string BindingType { get; init; } = string.Empty;
    [JsonPropertyName("item_code")]
    public string? ItemCode { get; init; }
    [JsonPropertyName("item_name")]
    public string? ItemName { get; init; }
    [JsonPropertyName("group_code")]
    public string? GroupCode { get; init; }
    [JsonPropertyName("group_name")]
    public string? GroupName { get; init; }
}

public sealed class PolicyScopeDto
{
    [Required]
    [JsonPropertyName("scope_dimension")]
    public string ScopeDimension { get; init; } = string.Empty;
    [Required]
    [JsonPropertyName("scope_operator")]
    public string ScopeOperator { get; init; } = string.Empty;
    [JsonPropertyName("scope_value_text")]
    public string? ScopeValueText { get; init; }
    [JsonPropertyName("scope_value_number")]
    public decimal? ScopeValueNumber { get; init; }
    [JsonPropertyName("scope_value_date")]
    public DateTime? ScopeValueDate { get; init; }
    [JsonPropertyName("scope_json")]
    public string? ScopeJson { get; init; }
}

public sealed class PolicyParamDto
{
    [Required]
    [JsonPropertyName("param_code")]
    public string ParamCode { get; init; } = string.Empty;
    [Required]
    [JsonPropertyName("value_type")]
    public string ValueType { get; init; } = string.Empty;
    [JsonPropertyName("value_text")]
    public string? ValueText { get; init; }
    [JsonPropertyName("value_number")]
    public decimal? ValueNumber { get; init; }
    [JsonPropertyName("value_date")]
    public DateTime? ValueDate { get; init; }
    [JsonPropertyName("value_bool")]
    public bool? ValueBool { get; init; }
    [JsonPropertyName("expr_text")]
    public string? ExprText { get; init; }
    [JsonPropertyName("expr_level")]
    public string? ExprLevel { get; init; }
}

public sealed class PolicyCreateRequest
{
    [Required, MaxLength(60)]
    [JsonPropertyName("policy_code")]
    public string PolicyCode { get; init; } = string.Empty;
    [Required, MaxLength(200)]
    [JsonPropertyName("policy_name")]
    public string PolicyName { get; init; } = string.Empty;
    [JsonPropertyName("template_id")]
    public long TemplateId { get; init; }
    [Required, MaxLength(30)]
    [JsonPropertyName("owner_type")]
    public string OwnerType { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("publish_profile")]
    public string PublishProfile { get; init; } = string.Empty;
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; init; }
}

public sealed class PolicyUpdateRequest
{
    [Required, MaxLength(200)]
    [JsonPropertyName("policy_name")]
    public string PolicyName { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("owner_type")]
    public string OwnerType { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("publish_profile")]
    public string PublishProfile { get; init; } = string.Empty;
    [Required, MaxLength(20)]
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
    [JsonPropertyName("updated_by")]
    public string? UpdatedBy { get; init; }
}

public sealed class PolicyVersionSaveRequest
{
    [JsonPropertyName("policy_version_id")]
    public long? PolicyVersionId { get; init; }
    [JsonPropertyName("version_no")]
    public int? VersionNo { get; init; }
    [JsonPropertyName("template_version_id")]
    public long TemplateVersionId { get; init; }
    [Required, MaxLength(20)]
    [JsonPropertyName("binding_type")]
    public string BindingType { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    [JsonPropertyName("scope_level")]
    public string ScopeLevel { get; init; } = string.Empty;
    [JsonPropertyName("priority_weight")]
    public int PriorityWeight { get; init; } = 100;
    [JsonPropertyName("effective_from")]
    public DateTime? EffectiveFrom { get; init; }
    [JsonPropertyName("effective_to")]
    public DateTime? EffectiveTo { get; init; }
    [JsonPropertyName("checksum")]
    public string? Checksum { get; init; }
    [JsonPropertyName("bindings")]
    public IReadOnlyList<PolicyBindingDto> Bindings { get; init; } = Array.Empty<PolicyBindingDto>();
    [JsonPropertyName("scopes")]
    public IReadOnlyList<PolicyScopeDto> Scopes { get; init; } = Array.Empty<PolicyScopeDto>();
    [JsonPropertyName("params")]
    public IReadOnlyList<PolicyParamDto> Params { get; init; } = Array.Empty<PolicyParamDto>();
}

public sealed class PolicyPreviewResponse
{
    [JsonPropertyName("policy_version_id")]
    public long PolicyVersionId { get; init; }
    [JsonPropertyName("policy_code")]
    public string PolicyCode { get; init; } = string.Empty;
    [JsonPropertyName("template_version_id")]
    public long TemplateVersionId { get; init; }
    [JsonPropertyName("capability_family")]
    public string CapabilityFamily { get; init; } = string.Empty;
    [JsonPropertyName("merge_mode")]
    public string MergeMode { get; init; } = string.Empty;
    [JsonPropertyName("binding_summary")]
    public IReadOnlyList<string> BindingSummary { get; init; } = Array.Empty<string>();
    [JsonPropertyName("scope_summary")]
    public IReadOnlyList<string> ScopeSummary { get; init; } = Array.Empty<string>();
    [JsonPropertyName("action_chain")]
    public IReadOnlyList<string> ActionChain { get; init; } = Array.Empty<string>();
}

public sealed class PolicyValidateResponse
{
    [JsonPropertyName("policy_version_id")]
    public long PolicyVersionId { get; init; }
    [JsonPropertyName("policy_status")]
    public string PolicyStatus { get; init; } = string.Empty;
}

public sealed class PolicyReviewSubmitRequest
{
    [Required]
    [JsonPropertyName("submitted_by")]
    public string SubmittedBy { get; init; } = string.Empty;
    [Required]
    [JsonPropertyName("review_stage")]
    public string ReviewStage { get; init; } = string.Empty;
}

public sealed class PolicyReviewDecisionRequest
{
    [Required]
    [JsonPropertyName("reviewed_by")]
    public string ReviewedBy { get; init; } = string.Empty;
    [JsonPropertyName("review_comment")]
    public string? ReviewComment { get; init; }
}

public sealed class RuntimePackagePublishRequest
{
    [Required]
    [JsonPropertyName("policy_version_ids")]
    public IReadOnlyList<long> PolicyVersionIds { get; init; } = Array.Empty<long>();
    [Required]
    [JsonPropertyName("published_by")]
    public string PublishedBy { get; init; } = string.Empty;
}

public sealed class RuntimePackageOperationRequest
{
    [Required]
    [JsonPropertyName("operated_by")]
    public string OperatedBy { get; init; } = string.Empty;
}

public sealed class PolicyImportRequest
{
    [Required]
    [JsonPropertyName("rule_ids")]
    public IReadOnlyList<long> RuleIds { get; init; } = Array.Empty<long>();
    [Required]
    [JsonPropertyName("imported_by")]
    public string ImportedBy { get; init; } = string.Empty;
}

public sealed class RuntimePackageHistoryResponse
{
    [JsonPropertyName("package_id")]
    public long PackageId { get; init; }
    [JsonPropertyName("package_version")]
    public long PackageVersion { get; init; }
    [JsonPropertyName("package_status")]
    public string PackageStatus { get; init; } = string.Empty;
    [JsonPropertyName("built_by")]
    public string? BuiltBy { get; init; }
    [JsonPropertyName("built_at")]
    public DateTime? BuiltAt { get; init; }
    [JsonPropertyName("activated_by")]
    public string? ActivatedBy { get; init; }
    [JsonPropertyName("activated_at")]
    public DateTime? ActivatedAt { get; init; }
}
