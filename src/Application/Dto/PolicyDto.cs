using System.ComponentModel.DataAnnotations;
using Pricing.RuleCenter.Application.Policies;

namespace Pricing.RuleCenter.Application.Dto;

public class PolicyResponse
{
    public long PolicyId { get; init; }
    public string PolicyCode { get; init; } = string.Empty;
    public string PolicyName { get; init; } = string.Empty;
    public long TemplateId { get; init; }
    public string OwnerType { get; init; } = string.Empty;
    public string PublishProfile { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int CurrentVersionNo { get; init; }
}

public sealed class PolicyDetailResponse : PolicyResponse
{
    public IReadOnlyList<PolicyVersionResponse> Versions { get; init; } = Array.Empty<PolicyVersionResponse>();
}

public sealed class PolicyVersionResponse
{
    public long PolicyVersionId { get; init; }
    public long PolicyId { get; init; }
    public long TemplateVersionId { get; init; }
    public int VersionNo { get; init; }
    public string PolicyStatus { get; init; } = string.Empty;
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public string BindingType { get; init; } = string.Empty;
    public string ScopeLevel { get; init; } = string.Empty;
    public int PriorityWeight { get; init; }
    public string? Checksum { get; init; }
    public long? LastBuiltPackageId { get; init; }
    public IReadOnlyList<PolicyBindingDto> Bindings { get; init; } = Array.Empty<PolicyBindingDto>();
    public IReadOnlyList<PolicyScopeDto> Scopes { get; init; } = Array.Empty<PolicyScopeDto>();
    public IReadOnlyList<PolicyParamDto> Params { get; init; } = Array.Empty<PolicyParamDto>();
}

public sealed class PolicyBindingDto
{
    [Required]
    public string BindingType { get; init; } = string.Empty;
    public string? ItemCode { get; init; }
    public string? ItemName { get; init; }
    public string? GroupCode { get; init; }
    public string? GroupName { get; init; }
}

public sealed class PolicyScopeDto
{
    [Required]
    public string ScopeDimension { get; init; } = string.Empty;
    [Required]
    public string ScopeOperator { get; init; } = string.Empty;
    public string? ScopeValueText { get; init; }
    public decimal? ScopeValueNumber { get; init; }
    public DateTime? ScopeValueDate { get; init; }
    public string? ScopeJson { get; init; }
}

public sealed class PolicyParamDto
{
    [Required]
    public string ParamCode { get; init; } = string.Empty;
    [Required]
    public string ValueType { get; init; } = string.Empty;
    public string? ValueText { get; init; }
    public decimal? ValueNumber { get; init; }
    public DateTime? ValueDate { get; init; }
    public bool? ValueBool { get; init; }
    public string? ExprText { get; init; }
    public string? ExprLevel { get; init; }
}

public sealed class PolicyCreateRequest
{
    [Required, MaxLength(60)]
    public string PolicyCode { get; init; } = string.Empty;
    [Required, MaxLength(200)]
    public string PolicyName { get; init; } = string.Empty;
    public long TemplateId { get; init; }
    [Required, MaxLength(30)]
    public string OwnerType { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string PublishProfile { get; init; } = string.Empty;
    public string? CreatedBy { get; init; }
}

public sealed class PolicyUpdateRequest
{
    [Required, MaxLength(200)]
    public string PolicyName { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string OwnerType { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string PublishProfile { get; init; } = string.Empty;
    [Required, MaxLength(20)]
    public string Status { get; init; } = string.Empty;
    public string? UpdatedBy { get; init; }
}

public sealed class PolicyVersionSaveRequest
{
    public long? PolicyVersionId { get; init; }
    public int? VersionNo { get; init; }
    public long TemplateVersionId { get; init; }
    [Required, MaxLength(20)]
    public string BindingType { get; init; } = string.Empty;
    [Required, MaxLength(30)]
    public string ScopeLevel { get; init; } = string.Empty;
    public int PriorityWeight { get; init; } = 100;
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public string? Checksum { get; init; }
    public IReadOnlyList<PolicyBindingDto> Bindings { get; init; } = Array.Empty<PolicyBindingDto>();
    public IReadOnlyList<PolicyScopeDto> Scopes { get; init; } = Array.Empty<PolicyScopeDto>();
    public IReadOnlyList<PolicyParamDto> Params { get; init; } = Array.Empty<PolicyParamDto>();
}

public sealed class PolicyPreviewResponse
{
    public long PolicyVersionId { get; init; }
    public string PolicyCode { get; init; } = string.Empty;
    public long TemplateVersionId { get; init; }
    public string CapabilityFamily { get; init; } = string.Empty;
    public string MergeMode { get; init; } = string.Empty;
    public IReadOnlyList<string> BindingSummary { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> ScopeSummary { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> ActionChain { get; init; } = Array.Empty<string>();
}

public sealed class PolicyValidateResponse
{
    public long PolicyVersionId { get; init; }
    public string PolicyStatus { get; init; } = string.Empty;
}

public sealed class PolicyReviewSubmitRequest
{
    [Required]
    public string SubmittedBy { get; init; } = string.Empty;
    [Required]
    public string ReviewStage { get; init; } = string.Empty;
}

public sealed class PolicyReviewDecisionRequest
{
    [Required]
    public string ReviewedBy { get; init; } = string.Empty;
    public string? ReviewComment { get; init; }
}

public sealed class RuntimePackagePublishRequest
{
    [Required]
    public IReadOnlyList<long> PolicyVersionIds { get; init; } = Array.Empty<long>();
    [Required]
    public string PublishedBy { get; init; } = string.Empty;
}

public sealed class RuntimePackageOperationRequest
{
    [Required]
    public string OperatedBy { get; init; } = string.Empty;
}

public sealed class RuntimePackageHistoryResponse
{
    public long PackageId { get; init; }
    public long PackageVersion { get; init; }
    public string PackageStatus { get; init; } = string.Empty;
    public string? BuiltBy { get; init; }
    public DateTime? BuiltAt { get; init; }
    public string? ActivatedBy { get; init; }
    public DateTime? ActivatedAt { get; init; }
}
