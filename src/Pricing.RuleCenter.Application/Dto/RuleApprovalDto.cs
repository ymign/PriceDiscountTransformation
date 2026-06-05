using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 规则审批记录响应 DTO。
/// </summary>
public sealed class RuleApprovalResponse
{
    public long ApprovalId { get; init; }
    public long RuleId { get; init; }
    public int? VersionNo { get; init; }
    public string ActionType { get; init; } = string.Empty;
    public string ApprovalStatus { get; init; } = string.Empty;
    public string SubmittedBy { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
    public string? ReviewedBy { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public string? ReviewComment { get; init; }
}

/// <summary>
/// 提交审批请求。
/// </summary>
public sealed class RuleApprovalSubmitRequest
{
    [Required(ErrorMessage = "操作类型不能为空")]
    public string ActionType { get; init; } = string.Empty;

    [Required(ErrorMessage = "提交人不能为空")]
    public string SubmittedBy { get; init; } = string.Empty;

    public string? Remark { get; init; }
}

/// <summary>
/// 审批通过请求。
/// </summary>
public sealed class RuleApprovalDecisionRequest
{
    [Required(ErrorMessage = "操作类型不能为空")]
    public string ActionType { get; init; } = string.Empty;

    [Required(ErrorMessage = "审核人不能为空")]
    public string ReviewedBy { get; init; } = string.Empty;

    public string? ReviewComment { get; init; }
}

