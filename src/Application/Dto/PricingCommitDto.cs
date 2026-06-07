using System.ComponentModel.DataAnnotations;

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
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    [Required(ErrorMessage = "请求ID不能为空")]
    public long RequestId { get; init; }

    /// <summary>
    /// 收费单号，用于与 HIS 落账结果关联
    /// </summary>
    public string? ChargeNo { get; init; }

    /// <summary>
    /// HIS 实际落账明细。commit 时必须按收费明细号、项目编码、片段序号回传实际落账数量和金额。
    /// 规则中心会与 confirm 阶段保存的折价明细逐项比对，防止 HIS 侧落账金额与计价结果不一致。
    /// </summary>
    public IReadOnlyList<PricingCommitActualItemRequest>? ActualItems { get; init; }

    /// <summary>
    /// HIS 实际落账总金额。为空时只校验 ActualItems 明细合计；传入时会同时校验总金额。
    /// </summary>
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
    public string? ChargeDetailNo { get; init; }

    /// <summary>
    /// HIS 实际落账项目编码。
    /// </summary>
    [Required(ErrorMessage = "实际落账项目编码不能为空")]
    public string ItemCode { get; init; } = string.Empty;

    /// <summary>
    /// 多部位或多片段明细序号。
    /// </summary>
    public int? PartSeq { get; init; }

    /// <summary>
    /// HIS 实际落账数量。
    /// </summary>
    public decimal FinalQty { get; init; }

    /// <summary>
    /// HIS 实际落账金额，最终金额保留 2 位小数。
    /// </summary>
    public decimal FinalAmount { get; init; }
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
    /// 计价请求日志主键，用于串联请求、步骤、折价明细和限额占用
    /// </summary>
    [Required(ErrorMessage = "请求ID不能为空")]
    public long RequestId { get; init; }
}
