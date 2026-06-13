using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Validation;

/// <summary>
/// 计价应用服务不可绕过的请求校验入口。
/// </summary>
internal static class PricingRequestGuard
{
    /// <summary>
    /// 单次计价请求最多允许的费用明细数。
    /// </summary>
    /// <remarks>
    /// 该限制保护 simulate/confirm 不被超大请求拖垮，同时与一次收费动作最多 50 条明细的约束保持一致。
    /// </remarks>
    private const int MaxCalculateItemCount = 50;

    /// <summary>
    /// 校验试算/确认共享的计价请求结构，并返回非空费用明细集合。
    /// </summary>
    /// <param name="request">计价请求。</param>
    /// <returns>已经过基础校验的费用明细集合。</returns>
    /// <remarks>
    /// 这里是 workflow 内部不可绕过的防线。即使控制器入口已经做了模型绑定和 DTO 校验，
    /// 资金相关 workflow 仍需在应用层再次保护关键字段，避免内部调用或测试夹具绕过边界。
    /// </remarks>
    public static IReadOnlyList<PricingCalculateItemRequest> GetRequiredItems(
        PricingCalculateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SourceSystem))
        {
            throw new ArgumentException("来源系统不能为空", nameof(request.SourceSystem));
        }

        if (string.IsNullOrWhiteSpace(request.PatientId))
        {
            throw new ArgumentException("患者ID不能为空", nameof(request.PatientId));
        }

        if (request.BusinessChargeTime == default)
        {
            throw new ArgumentException("业务收费发生时间不能为空", nameof(request.BusinessChargeTime));
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException("费用明细不能为空", nameof(request.Items));
        }

        if (request.Items.Count > MaxCalculateItemCount)
        {
            throw new ArgumentException($"费用明细不能超过{MaxCalculateItemCount}条", nameof(request.Items));
        }

        for (var itemIndex = 0; itemIndex < request.Items.Count; itemIndex++)
        {
            var item = request.Items[itemIndex]
                ?? throw new ArgumentException($"费用明细[{itemIndex}]不能为空", nameof(request.Items));

            EnsureCalculateItem(item, itemIndex);
        }

        return request.Items;
    }

    /// <summary>
    /// 校验 confirm 请求必须携带稳定业务请求号。
    /// </summary>
    /// <param name="request">确认计价请求。</param>
    /// <remarks>
    /// BusinessRequestNo 是 confirm 幂等键的一部分。缺失时无法区分渠道重试和新的收费动作，存在重复占额风险。
    /// </remarks>
    public static void EnsureConfirmRequest(PricingCalculateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            throw new ArgumentException("CONFIRM 必须传入稳定的 BusinessRequestNo");
        }
    }

    /// <summary>
    /// 校验 commit 请求结构。
    /// </summary>
    /// <param name="request">落账提交请求。</param>
    /// <remarks>
    /// commit 的核心定位字段是 RequestId；ActualItems 的数量和金额允许为 0，但不能为负数。
    /// 真实明细是否覆盖 confirm 保存的全部结果，由 <see cref="PricingCommitActualValidator"/> 做业务对账。
    /// </remarks>
    public static void EnsureCommitRequest(PricingCommitRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.RequestId <= 0)
        {
            throw new ArgumentException("RequestId必须大于0", nameof(request.RequestId));
        }

        if (request.ActualTotalAmount.HasValue && request.ActualTotalAmount.Value < 0)
        {
            throw new ArgumentException("实际落账总金额不能小于0", nameof(request.ActualTotalAmount));
        }

        if (request.CommittedAt.HasValue && request.CommittedAt.Value == default)
        {
            throw new ArgumentException("HIS落账业务时间不能为空", nameof(request.CommittedAt));
        }

        if (request.ActualItems is null)
        {
            return;
        }

        for (var index = 0; index < request.ActualItems.Count; index++)
        {
            var item = request.ActualItems[index]
                ?? throw new ArgumentException($"实际落账明细[{index}]不能为空", nameof(request.ActualItems));

            if (string.IsNullOrWhiteSpace(item.ItemCode))
            {
                throw new ArgumentException($"实际落账明细[{index}]项目编码不能为空", nameof(request.ActualItems));
            }

            if (item.FinalQty < 0)
            {
                throw new ArgumentException($"实际落账明细[{index}]数量不能小于0", nameof(request.ActualItems));
            }

            if (item.FinalAmount < 0)
            {
                throw new ArgumentException($"实际落账明细[{index}]金额不能小于0", nameof(request.ActualItems));
            }
        }
    }

    /// <summary>
    /// 校验 cancel 请求结构。
    /// </summary>
    /// <param name="request">取消确认请求。</param>
    /// <remarks>
    /// cancel 只能按 RequestId 定位待取消记录，不能只靠业务号释放额度，避免误释放其他收费动作。
    /// </remarks>
    public static void EnsureCancelRequest(PricingCancelRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.RequestId <= 0)
        {
            throw new ArgumentException("RequestId必须大于0", nameof(request.RequestId));
        }

        if (request.CancelledAt.HasValue && request.CancelledAt.Value == default)
        {
            throw new ArgumentException("HIS取消业务时间不能为空", nameof(request.CancelledAt));
        }
    }

    /// <summary>
    /// 校验 reverse 请求结构。
    /// </summary>
    /// <param name="request">退费冲正请求。</param>
    /// <remarks>
    /// ReverseNo 是退费幂等键。数量和金额可以为空，表示由 workflow 按原明细推导；显式传入时必须为正/非负。
    /// </remarks>
    public static void EnsureReverseRequest(PricingReverseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.OriginalRequestId <= 0)
        {
            throw new ArgumentException("OriginalRequestId必须大于0", nameof(request.OriginalRequestId));
        }

        if (string.IsNullOrWhiteSpace(request.ReverseNo))
        {
            throw new ArgumentException("REVERSE 必须传入稳定的 ReverseNo", nameof(request.ReverseNo));
        }

        if (request.ReverseQty.HasValue && request.ReverseQty.Value <= 0)
        {
            throw new ArgumentException("退费数量必须大于0", nameof(request.ReverseQty));
        }

        if (request.ReverseAmt.HasValue && request.ReverseAmt.Value < 0)
        {
            throw new ArgumentException("退费金额不能小于0", nameof(request.ReverseAmt));
        }

        if (request.ReverseTime.HasValue && request.ReverseTime.Value == default)
        {
            throw new ArgumentException("退费业务时间不能为空", nameof(request.ReverseTime));
        }
    }

    /// <summary>
    /// 校验单条费用明细及多片段明细。
    /// </summary>
    /// <param name="item">费用明细。</param>
    /// <param name="itemIndex">明细在请求中的索引，用于错误消息定位。</param>
    private static void EnsureCalculateItem(PricingCalculateItemRequest item, int itemIndex)
    {
        if (string.IsNullOrWhiteSpace(item.ItemCode))
        {
            throw new ArgumentException($"费用明细[{itemIndex}]项目编码不能为空", "Items");
        }

        if (item.InputQty <= 0)
        {
            throw new ArgumentException($"费用明细[{itemIndex}]数量必须大于0", "Items");
        }

        if (item.UnitPrice < 0)
        {
            throw new ArgumentException($"费用明细[{itemIndex}]单价不能小于0", "Items");
        }

        if (item.BusinessChargeTime.HasValue && item.BusinessChargeTime.Value == default)
        {
            throw new ArgumentException($"费用明细[{itemIndex}]业务收费发生时间不能为空", "Items");
        }

        if (item.PricingParts is null)
        {
            return;
        }

        for (var partIndex = 0; partIndex < item.PricingParts.Count; partIndex++)
        {
            var part = item.PricingParts[partIndex]
                ?? throw new ArgumentException($"费用明细[{itemIndex}].PricingParts[{partIndex}]不能为空", "Items");

            if (part.Qty <= 0)
            {
                throw new ArgumentException($"费用明细[{itemIndex}].PricingParts[{partIndex}]数量必须大于0", "Items");
            }
        }
    }
}
