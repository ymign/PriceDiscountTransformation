using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

public sealed class RuleActionResponse
{
    public long ActionId { get; init; }
    public long RuleId { get; init; }
    public int VersionNo { get; init; }
    public string ActionType { get; init; } = string.Empty;
    public string ExecutorCode { get; init; } = string.Empty;
    public string? ParamsJson { get; init; }
    public string? ExclusiveGroup { get; init; }
    public int SortNo { get; init; }
    public string OnError { get; init; } = "STOP";
    public string IsEnabled { get; init; } = "Y";
}

public sealed class RuleActionItemRequest
{
    [Required(ErrorMessage = "动作类型不能为空")]
    [MaxLength(50)]
    public string ActionType { get; init; } = string.Empty;

    [Required(ErrorMessage = "执行器编码不能为空")]
    [MaxLength(50)]
    public string ExecutorCode { get; init; } = string.Empty;

    public string? ParamsJson { get; init; }

    [MaxLength(50)]
    public string? ExclusiveGroup { get; init; }

    public int SortNo { get; init; }

    [MaxLength(20)]
    public string OnError { get; init; } = "STOP";

    public string IsEnabled { get; init; } = "Y";
}

public sealed class RuleActionSaveRequest
{
    [Required(ErrorMessage = "动作列表不能为空")]
    public IReadOnlyList<RuleActionItemRequest> Actions { get; init; } = Array.Empty<RuleActionItemRequest>();
}
