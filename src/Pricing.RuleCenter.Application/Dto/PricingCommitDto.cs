using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 确认计费提交请求 DTO。
/// </summary>
/// <remarks>
/// commit 用于告诉规则中心 HIS 已经成功落账。成功后 CONFIRM_PENDING 会推进到 CONFIRMED，
/// 保护占额会变成正式占用。
/// </remarks>
public sealed class PricingCommitRequest
{
    /// <summary>
    /// 计价请求日志主键。commit/cancel 不再使用 business_request_no 定位，必须使用 confirm 响应返回的 request_id。
    /// </summary>
    [Required(ErrorMessage = "请求ID不能为空")]
    [JsonPropertyName("request_id")]
    public long RequestId { get; init; }

    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    [JsonPropertyName("charge_no")]
    public string? ChargeNo { get; init; }

    /// <summary>
    /// HIS 提交流水号。建议由 HIS 生成稳定值，用于审计一次 commit 回调。
    /// </summary>
    [JsonPropertyName("commit_no")]
    public string? CommitNo { get; init; }

    /// <summary>
    /// 执行落账提交的操作人、工作站账号或系统账号。
    /// </summary>
    [JsonPropertyName("committed_by")]
    public string? CommittedBy { get; init; }

    /// <summary>
    /// HIS 实际落账成功的业务时间。为空时计价中心仅记录收到 commit 的技术时间。
    /// </summary>
    [JsonPropertyName("committed_at")]
    public DateTime? CommittedAt { get; init; }

    /// <summary>
    /// HIS 实际落账明细。commit 时必须按收费明细号、项目编码、片段序号回传实际落账数量和金额。
    /// 规则中心会与 confirm 阶段保存的折价明细逐项比对，防止 HIS 侧落账金额与计价结果不一致。
    /// </summary>
    [JsonPropertyName("actual_items")]
    public IReadOnlyList<PricingCommitActualItemRequest>? ActualItems { get; init; }

    /// <summary>
    /// HIS 实际落账总金额。为空时只校验 ActualItems 明细合计；传入时会同时校验总金额。
    /// </summary>
    [JsonPropertyName("actual_total_amount")]
    public decimal? ActualTotalAmount { get; init; }
}

/// <summary>
/// commit 阶段 HIS 实际落账明细 DTO。
/// </summary>
public sealed class PricingCommitActualItemRequest
{
    /// <summary>
    /// HIS 实际落账后的收费明细号。普通项目和主项目必须与 confirm 保存的折价明细一致；
    /// 替换子项、加收子项允许 HIS 落账时生成新的收费明细号。
    /// </summary>
    [JsonPropertyName("charge_detail_no")]
    public string? ChargeDetailNo { get; init; }

    /// <summary>
    /// HIS 实际落账项目编码。
    /// </summary>
    [Required(ErrorMessage = "实际落账项目编码不能为空")]
    [JsonPropertyName("item_code")]
    public string ItemCode { get; init; } = string.Empty;

    /// <summary>
    /// 多部位或多片段明细序号。
    /// </summary>
    [JsonPropertyName("part_seq")]
    public int? PartSeq { get; init; }

    /// <summary>
    /// HIS 实际落账数量。
    /// </summary>
    [JsonPropertyName("final_qty")]
    public decimal FinalQty { get; init; }

    /// <summary>
    /// HIS 实际落账金额，最终金额保留 2 位小数。
    /// </summary>
    [JsonPropertyName("final_amount")]
    public decimal FinalAmount { get; init; }
}

/// <summary>
/// 确认计费提交响应 DTO。
/// </summary>
public sealed class PricingCommitResponse
{
    /// <summary>本次 commit 对应的 confirm 请求 ID。</summary>
    [JsonPropertyName("request_id")]
    public long RequestId { get; init; }

    /// <summary>推进后的业务状态，成功时通常为 CONFIRMED。</summary>
    [JsonPropertyName("business_status")]
    public string BusinessStatus { get; init; } = string.Empty;

    /// <summary>HIS 实际收费单号。</summary>
    [JsonPropertyName("charge_no")]
    public string? ChargeNo { get; init; }

    /// <summary>计价中心确认收到 commit 的时间。</summary>
    [JsonPropertyName("committed_at")]
    public DateTime? CommittedAt { get; init; }

    /// <summary>下一步动作。commit 成功后通常无需继续调用计价中心。</summary>
    [JsonPropertyName("next_action")]
    public string NextAction { get; init; } = PricingNextActionCodes.NoFurtherAction;
}

/// <summary>
/// 确认计费取消请求 DTO。
/// </summary>
/// <remarks>
/// cancel 用于释放 confirm 阶段已经产生、但最终未落账的保护占用。
/// </remarks>
public sealed class PricingCancelRequest
{
    /// <summary>
    /// 计价请求日志主键。commit/cancel 不再使用 business_request_no 定位，必须使用 confirm 响应返回的 request_id。
    /// </summary>
    [Required(ErrorMessage = "请求ID不能为空")]
    [JsonPropertyName("request_id")]
    public long RequestId { get; init; }

    /// <summary>
    /// HIS 取消流水号。建议由 HIS 生成稳定值，用于审计一次 cancel 回调。
    /// </summary>
    [JsonPropertyName("cancel_no")]
    public string? CancelNo { get; init; }

    /// <summary>
    /// 执行取消的操作人、工作站账号或系统账号。
    /// </summary>
    [JsonPropertyName("cancelled_by")]
    public string? CancelledBy { get; init; }

    /// <summary>
    /// 取消原因，例如用户放弃收费、支付失败、HIS 落账失败。
    /// </summary>
    [JsonPropertyName("cancel_reason")]
    public string? CancelReason { get; init; }

    /// <summary>
    /// HIS 侧确认取消的业务时间。为空时计价中心仅记录收到 cancel 的技术时间。
    /// </summary>
    [JsonPropertyName("cancelled_at")]
    public DateTime? CancelledAt { get; init; }
}

/// <summary>
/// 确认计费取消响应 DTO。
/// </summary>
public sealed class PricingCancelResponse
{
    /// <summary>本次 cancel 对应的 confirm 请求 ID。</summary>
    [JsonPropertyName("request_id")]
    public long RequestId { get; init; }

    /// <summary>推进后的业务状态，成功时通常为 CANCELLED。</summary>
    [JsonPropertyName("business_status")]
    public string BusinessStatus { get; init; } = string.Empty;

    /// <summary>下一步动作。cancel 成功后通常无需继续调用计价中心。</summary>
    [JsonPropertyName("next_action")]
    public string NextAction { get; init; } = PricingNextActionCodes.NoFurtherAction;
}
