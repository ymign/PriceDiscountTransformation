using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Application.Pricing.Validation;

/// <summary>
/// 计价应用服务不可绕过的请求校验入口。
/// </summary>
internal static class PricingRequestGuard
{
    private const int MaxCalculateItemCount = 50;

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

    public static void EnsureConfirmRequest(PricingCalculateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            throw new ArgumentException("CONFIRM 必须传入稳定的 BusinessRequestNo");
        }
    }

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

    public static void EnsureCancelRequest(PricingCancelRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.RequestId <= 0)
        {
            throw new ArgumentException("RequestId必须大于0", nameof(request.RequestId));
        }
    }

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
    }

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
