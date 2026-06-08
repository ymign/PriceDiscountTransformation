using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 规则审批记录响应 DTO。
/// </summary>
public sealed class RuleApprovalResponse
{
    /// <summary>
    /// 获取审批记录主键。
    /// </summary>
    [JsonPropertyName("approval_id")]
    public long ApprovalId { get; init; }

    /// <summary>
    /// 获取关联规则主键。
    /// </summary>
    [JsonPropertyName("rule_id")]
    public long RuleId { get; init; }

    /// <summary>
    /// 获取关联规则版本号。
    /// </summary>
    [JsonPropertyName("version_no")]
    public int? VersionNo { get; init; }

    /// <summary>
    /// 获取审批动作类型，例如发布、停用或回滚。
    /// </summary>
    [JsonPropertyName("action_type")]
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// 获取审批状态，例如待审、通过或驳回。
    /// </summary>
    [JsonPropertyName("approval_status")]
    public string ApprovalStatus { get; init; } = string.Empty;

    /// <summary>
    /// 获取提交人。
    /// </summary>
    [JsonPropertyName("submitted_by")]
    public string SubmittedBy { get; init; } = string.Empty;

    /// <summary>
    /// 获取提交时间。
    /// </summary>
    [JsonPropertyName("submitted_at")]
    public DateTime SubmittedAt { get; init; }

    /// <summary>
    /// 获取审核人。
    /// </summary>
    [JsonPropertyName("reviewed_by")]
    public string? ReviewedBy { get; init; }

    /// <summary>
    /// 获取审核时间。
    /// </summary>
    [JsonPropertyName("reviewed_at")]
    public DateTime? ReviewedAt { get; init; }

    /// <summary>
    /// 获取审核意见或提交备注。
    /// </summary>
    [JsonPropertyName("review_comment")]
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
    [JsonPropertyName("action_type")]
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// 获取提交人。
    /// </summary>
    [Required(ErrorMessage = "提交人不能为空")]
    [JsonPropertyName("submitted_by")]
    public string SubmittedBy { get; init; } = string.Empty;

    /// <summary>
    /// 获取提交备注。
    /// </summary>
    [JsonPropertyName("remark")]
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
    [JsonPropertyName("action_type")]
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// 获取审核人。
    /// </summary>
    [Required(ErrorMessage = "审核人不能为空")]
    [JsonPropertyName("reviewed_by")]
    public string ReviewedBy { get; init; } = string.Empty;

    /// <summary>
    /// 获取审核意见。
    /// </summary>
    [JsonPropertyName("review_comment")]
    public string? ReviewComment { get; init; }
}

