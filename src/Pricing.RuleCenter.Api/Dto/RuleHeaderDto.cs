using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

public sealed class RuleHeaderResponse
{
    public long RuleId { get; init; }
    public string RuleCode { get; init; } = string.Empty;
    public string RuleName { get; init; } = string.Empty;
    public string RuleCategory { get; init; } = string.Empty;
    public string RuleScope { get; init; } = string.Empty;
    public string? ItemCode { get; init; }
    public string? ItemName { get; init; }
    public string? GroupCode { get; init; }
    public int Priority { get; init; }
    public int CurrentVersion { get; init; }
    public string Status { get; init; } = string.Empty;
    public string IsEnabled { get; init; } = "Y";
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public string? Remark { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? UpdatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed class RuleHeaderCreateRequest
{
    [Required(ErrorMessage = "规则编码不能为空")]
    [MaxLength(50)]
    public string RuleCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "规则名称不能为空")]
    [MaxLength(200)]
    public string RuleName { get; init; } = string.Empty;

    [MaxLength(20)]
    public string RuleCategory { get; init; } = "MIXED";

    [MaxLength(20)]
    public string RuleScope { get; init; } = "ITEM";

    [MaxLength(50)]
    public string? ItemCode { get; init; }

    [MaxLength(200)]
    public string? ItemName { get; init; }

    [MaxLength(50)]
    public string? GroupCode { get; init; }

    public int Priority { get; init; } = 100;

    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public string? Remark { get; init; }
    public string? CreatedBy { get; init; }
}

public sealed class RuleHeaderUpdateRequest
{
    [Required(ErrorMessage = "规则名称不能为空")]
    [MaxLength(200)]
    public string RuleName { get; init; } = string.Empty;

    [MaxLength(20)]
    public string RuleCategory { get; init; } = "MIXED";

    [MaxLength(20)]
    public string RuleScope { get; init; } = "ITEM";

    [MaxLength(50)]
    public string? ItemCode { get; init; }

    [MaxLength(200)]
    public string? ItemName { get; init; }

    [MaxLength(50)]
    public string? GroupCode { get; init; }

    public int Priority { get; init; } = 100;

    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public string? Remark { get; init; }
    public string? UpdatedBy { get; init; }
}

public sealed class RuleHeaderPagedRequest
{
    public string? ItemCode { get; init; }
    public string? Status { get; init; }
    public string? Category { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class PagedResponse<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Total { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
}
