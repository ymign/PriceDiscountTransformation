using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

public sealed class RuleConditionResponse
{
    public long ConditionId { get; init; }
    public long RuleId { get; init; }
    public int VersionNo { get; init; }
    public string ConditionGroup { get; init; } = string.Empty;
    public string ConditionType { get; init; } = string.Empty;
    public string? OperatorType { get; init; }
    public string? LeftKey { get; init; }
    public string? RightValue { get; init; }
    public string? ParamsJson { get; init; }
    public int SortNo { get; init; }
    public string IsEnabled { get; init; } = "Y";
}

public sealed class RuleConditionItemRequest
{
    [Required(ErrorMessage = "条件组不能为空")]
    [MaxLength(50)]
    public string ConditionGroup { get; init; } = "DEFAULT";

    [Required(ErrorMessage = "条件类型不能为空")]
    [MaxLength(50)]
    public string ConditionType { get; init; } = string.Empty;

    [MaxLength(20)]
    public string? OperatorType { get; init; } = "EQ";

    [MaxLength(200)]
    public string? LeftKey { get; init; }

    [MaxLength(500)]
    public string? RightValue { get; init; }

    public string? ParamsJson { get; init; }
    public int SortNo { get; init; }
    public string IsEnabled { get; init; } = "Y";
}

public sealed class RuleConditionSaveRequest
{
    [Required(ErrorMessage = "条件列表不能为空")]
    public IReadOnlyList<RuleConditionItemRequest> Conditions { get; init; } = Array.Empty<RuleConditionItemRequest>();
}
