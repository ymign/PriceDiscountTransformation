using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 已提交计价结果冲正请求 DTO。
/// </summary>
/// <remarks>
/// reverse 针对已经 commit 成功的原请求，通常用于退费或撤销收费。冲正会写入冲正日志，
/// 并通过负向占用释放对应限额。
/// </remarks>
public sealed class PricingReverseRequest
{
    /// <summary>
    /// 原始确认请求的 RequestId，必须指向已经提交成功的计价记录。
    /// </summary>
    [Required(ErrorMessage = "原请求ID不能为空")]
    [JsonPropertyName("original_request_id")]
    public long OriginalRequestId { get; init; }

    /// <summary>
    /// 调用方冲正流水号，用于和 HIS 退费或撤销单据关联。
    /// 必填项，用于幂等校验和冲正日志关联。
    /// </summary>
    [Required(ErrorMessage = "冲正流水号不能为空")]
    [JsonPropertyName("reverse_no")]
    public string? ReverseNo { get; init; }
    /// <summary>
    /// 发起冲正的来源系统。为空时沿用原收费请求的来源系统。
    /// </summary>
    [JsonPropertyName("source_system")]
    public string? SourceSystem { get; init; }
    /// <summary>
    /// 发起冲正的来源终端或工作站。为空时沿用原收费请求的来源终端。
    /// </summary>
    [JsonPropertyName("source_terminal")]
    public string? SourceTerminal { get; init; }
    /// <summary>
    /// 被退费的原收费明细号。多费用明细请求执行部分退费时必须提供。
    /// </summary>
    [JsonPropertyName("charge_detail_no")]
    public string? ChargeDetailNo { get; init; }
    /// <summary>
    /// 被退费的项目编码。多费用明细请求执行部分退费时用于二次定位。
    /// </summary>
    [JsonPropertyName("item_code")]
    public string? ItemCode { get; init; }
    /// <summary>
    /// 多部位或多片段退费时的片段序号。
    /// </summary>
    [JsonPropertyName("part_seq")]
    public int? PartSeq { get; init; }
    /// <summary>
    /// 退费业务发生时间。为空时使用当前时间。
    /// </summary>
    [JsonPropertyName("reverse_time")]
    public DateTime? ReverseTime { get; init; }
    /// <summary>
    /// 本次冲正数量；为空时可由服务层按原请求数量处理。
    /// </summary>
    [JsonPropertyName("reverse_qty")]
    public decimal? ReverseQty { get; init; }
    /// <summary>
    /// 本次冲正金额；为空时可由服务层按原请求金额处理。
    /// </summary>
    [JsonPropertyName("reverse_amt")]
    public decimal? ReverseAmt { get; init; }
    /// <summary>
    /// 冲正操作人或系统账号。
    /// </summary>
    [JsonPropertyName("reversed_by")]
    public string? ReversedBy { get; init; }
    /// <summary>
    /// 冲正原因说明，用于审计和追踪页面展示。
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}
