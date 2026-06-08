using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 规则审批记录响应 DTO。
/// </summary>
public sealed class RuleApprovalResponse
{
    /// <summary>
    /// 获取审批记录主键。
    /// </summary>
    public long ApprovalId { get; init; }

    /// <summary>
    /// 获取关联规则主键。
    /// </summary>
    public long RuleId { get; init; }

    /// <summary>
    /// 获取关联规则版本号。
    /// </summary>
    public int? VersionNo { get; init; }

    /// <summary>
    /// 获取审批动作类型，例如发布、停用或回滚。
    /// </summary>
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// 获取审批状态，例如待审、通过或驳回。
    /// </summary>
    public string ApprovalStatus { get; init; } = string.Empty;

    /// <summary>
    /// 获取提交人。
    /// </summary>
    public string SubmittedBy { get; init; } = string.Empty;

    /// <summary>
    /// 获取提交时间。
    /// </summary>
    public DateTime SubmittedAt { get; init; }

    /// <summary>
    /// 获取审核人。
    /// </summary>
    public string? ReviewedBy { get; init; }

    /// <summary>
    /// 获取审核时间。
    /// </summary>
    public DateTime? ReviewedAt { get; init; }

    /// <summary>
    /// 获取审核意见或提交备注。
    /// </summary>
    public string? ReviewComment { get; init; }
}

/// <summary>
/// 提交审批请求。
/// </summary>
public sealed class RuleApprovalSubmitRequest
{
    /// <summary>
    /// 获取审批动作类型，例如 PUBLISH、DISABLE 或 ROLLBACK。
    /// </summary>
    [Required(ErrorMessage = "操作类型不能为空")]
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// 获取提交人。
    /// </summary>
    [Required(ErrorMessage = "提交人不能为空")]
    public string SubmittedBy { get; init; } = string.Empty;

    /// <summary>
    /// 获取提交备注。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 审批通过请求。
/// </summary>
public sealed class RuleApprovalDecisionRequest
{
    /// <summary>
    /// 获取审批动作类型，必须与待审批记录一致。
    /// </summary>
    [Required(ErrorMessage = "操作类型不能为空")]
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// 获取审核人。
    /// </summary>
    [Required(ErrorMessage = "审核人不能为空")]
    public string ReviewedBy { get; init; } = string.Empty;

    /// <summary>
    /// 获取审核意见。
    /// </summary>
    public string? ReviewComment { get; init; }
}

