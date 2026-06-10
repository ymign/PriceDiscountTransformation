using Pricing.RuleCenter.Core.Aggregates.Quota;

namespace Pricing.RuleCenter.Core.Models;

/// <summary>
/// 单次计价请求共享状态。
/// </summary>
/// <remarks>
/// 一次 simulate/confirm 请求通常会携带整单费用明细。明细按顺序逐条进入引擎时，
/// 后面的项目需要看到前面项目已经产生的请求内占额、互斥计数、同手术累计金额和父项最终金额。
/// 该对象就是这份“单次请求共享工作区”，不是跨请求缓存，也不是多批次 HTTP 状态。
/// </remarks>
public sealed class RequestSharedPricingState
{
    /// <summary>
    /// 请求内累计值字典。
    /// </summary>
    /// <remarks>
    /// 当前统一存放三类键：
    /// <list type="bullet">
    ///   <item><description><c>{LIMIT_TYPE}:{dimensionCode}</c> — 数量类限制的请求内累计</description></item>
    ///   <item><description><c>MUTEX:{groupCode}</c> — 同组互斥已通过项目数</description></item>
    ///   <item><description><c>OP_CEILING:{operationNo}:{groupCode}</c> — 同手术封顶请求内累计金额</description></item>
    ///   <item><description><c>ITEM_AMT:{itemCode}</c> — 父项目最终金额，供子项金额百分比公式使用</description></item>
    /// </list>
    /// </remarks>
    public Dictionary<string, decimal> AccumulatedValues { get; init; } =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 请求内已经产生的占额草稿。
    /// </summary>
    /// <remarks>
    /// TIME_WINDOW、DAY_QTY 等执行器需要按业务时间精确过滤前序明细是否落在窗口内，
    /// 仅保留累计数量会丢失时间维度，因此仍保留占额草稿列表。
    /// </remarks>
    public List<LimitOccupy> LimitOccupies { get; init; } = new();

    /// <summary>
    /// 创建当前请求共享状态的快照，供单条明细在本次计算期间只读使用。
    /// </summary>
    public RequestSharedPricingState CreateSnapshot()
    {
        return new RequestSharedPricingState
        {
            AccumulatedValues = new Dictionary<string, decimal>(
                AccumulatedValues,
                StringComparer.OrdinalIgnoreCase),
            LimitOccupies = LimitOccupies.ToList()
        };
    }

    /// <summary>
    /// 将当前明细的计价结果累积到请求共享状态，供后续明细使用。
    /// </summary>
    public void Accumulate(PricingResult result, PricingContext context)
    {
        foreach (var occupy in result.LimitOccupies)
        {
            if (occupy.OccupyQty != 0 || occupy.OccupyAmt != 0)
            {
                LimitOccupies.Add(occupy);
            }

            if (string.IsNullOrWhiteSpace(occupy.LimitType) ||
                string.IsNullOrWhiteSpace(occupy.LimitDimensionCode))
            {
                continue;
            }

            var dimensionKey = BuildLimitDimensionKey(occupy.LimitType, occupy.LimitDimensionCode);
            AccumulatedValues.TryGetValue(dimensionKey, out var existingQty);
            AccumulatedValues[dimensionKey] = existingQty + occupy.OccupyQty;
        }

        if (result.FinalQty > 0 && !string.IsNullOrWhiteSpace(context.ItemGroupCode))
        {
            var mutexKey = $"MUTEX:{context.ItemGroupCode.Trim().ToUpperInvariant()}";
            AccumulatedValues.TryGetValue(mutexKey, out var existingCount);
            AccumulatedValues[mutexKey] = existingCount + 1m;
        }

        var operationNo = GetExtraParam(context.ExtraParams, "operationNo")
            ?? GetExtraParam(context.ExtraParams, "operationId");
        if (!string.IsNullOrWhiteSpace(operationNo) && !string.IsNullOrWhiteSpace(context.ItemGroupCode))
        {
            var opCeilingKey =
                $"OP_CEILING:{operationNo.Trim().ToUpperInvariant()}:{context.ItemGroupCode.Trim().ToUpperInvariant()}";
            AccumulatedValues.TryGetValue(opCeilingKey, out var existingAmount);
            AccumulatedValues[opCeilingKey] = existingAmount + result.FinalAmount;
        }

        if (!string.IsNullOrWhiteSpace(context.ItemCode))
        {
            var parentAmountKey = $"ITEM_AMT:{context.ItemCode.Trim().ToUpperInvariant()}";
            AccumulatedValues[parentAmountKey] = result.FinalAmount;
        }
    }

    public static string BuildLimitDimensionKey(string limitType, string dimensionCode)
    {
        return $"{limitType.Trim().ToUpperInvariant()}:{dimensionCode.Trim().ToUpperInvariant()}";
    }

    private static string? GetExtraParam(IReadOnlyDictionary<string, string>? extraParams, string key)
    {
        if (extraParams is null || !extraParams.TryGetValue(key, out var value))
        {
            return null;
        }

        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}
