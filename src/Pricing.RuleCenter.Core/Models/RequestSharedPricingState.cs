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
    /// 记录请求内占额草稿。
    /// </summary>
    public void AddLimitOccupy(LimitOccupy occupy)
    {
        if (occupy.OccupyQty != 0 || occupy.OccupyAmt != 0)
        {
            LimitOccupies.Add(occupy);
        }
    }

    /// <summary>
    /// 增加指定限额维度的请求内累计数量。
    /// </summary>
    public void AddLimitQty(string limitType, string dimensionCode, decimal qty)
    {
        var key = RequestSharedStateKeys.BuildLimitDimensionKey(limitType, dimensionCode);
        AccumulatedValues.TryGetValue(key, out var existingQty);
        AccumulatedValues[key] = existingQty + qty;
    }

    /// <summary>
    /// 增加同组互斥已通过项目数。
    /// </summary>
    public void IncrementMutexCount(string groupCode, decimal increment = 1m)
    {
        var key = RequestSharedStateKeys.BuildMutexKey(groupCode);
        AccumulatedValues.TryGetValue(key, out var existingCount);
        AccumulatedValues[key] = existingCount + increment;
    }

    /// <summary>
    /// 增加同手术封顶请求内累计金额。
    /// </summary>
    public void AddOperationAmount(string operationNo, string groupCode, decimal amount)
    {
        var key = RequestSharedStateKeys.BuildOperationCeilingKey(operationNo, groupCode);
        AccumulatedValues.TryGetValue(key, out var existingAmount);
        AccumulatedValues[key] = existingAmount + amount;
    }

    /// <summary>
    /// 设置父项目最终金额。
    /// </summary>
    public void SetParentItemAmount(string itemCode, decimal amount)
    {
        AccumulatedValues[RequestSharedStateKeys.BuildParentItemAmountKey(itemCode)] = amount;
    }

    /// <summary>
    /// 尝试读取父项目最终金额。
    /// </summary>
    public bool TryGetParentItemAmount(string itemCode, out decimal amount)
    {
        return AccumulatedValues.TryGetValue(
            RequestSharedStateKeys.BuildParentItemAmountKey(itemCode),
            out amount);
    }

    /// <summary>
    /// 获取同组互斥已通过项目数。
    /// </summary>
    public decimal GetMutexCount(string groupCode)
    {
        return GetAccumulatedValue(RequestSharedStateKeys.BuildMutexKey(groupCode));
    }

    /// <summary>
    /// 获取同手术封顶请求内累计金额。
    /// </summary>
    public decimal GetOperationAmount(string operationNo, string groupCode)
    {
        return GetAccumulatedValue(RequestSharedStateKeys.BuildOperationCeilingKey(operationNo, groupCode));
    }

    /// <summary>
    /// 获取指定限额维度的请求内累计数量。
    /// </summary>
    public decimal GetLimitQty(string limitType, string dimensionCode)
    {
        return GetAccumulatedValue(RequestSharedStateKeys.BuildLimitDimensionKey(limitType, dimensionCode));
    }

    /// <summary>
    /// 尝试读取任意共享状态键的累计值。
    /// </summary>
    public bool TryGetAccumulatedValue(string key, out decimal value)
    {
        return AccumulatedValues.TryGetValue(key, out value);
    }

    /// <summary>
    /// 将当前明细的计价结果累积到请求共享状态，供后续明细使用。
    /// </summary>
    public void Accumulate(PricingResult result, PricingContext context)
    {
        foreach (var occupy in result.LimitOccupies)
        {
            AddLimitOccupy(occupy);

            if (string.IsNullOrWhiteSpace(occupy.LimitType) ||
                string.IsNullOrWhiteSpace(occupy.LimitDimensionCode))
            {
                continue;
            }

            AddLimitQty(occupy.LimitType, occupy.LimitDimensionCode, occupy.OccupyQty);
        }

        if (result.FinalQty > 0 && !string.IsNullOrWhiteSpace(context.ItemGroupCode))
        {
            IncrementMutexCount(context.ItemGroupCode);
        }

        var operationNo = GetExtraParam(context.ExtraParams, "operationNo")
            ?? GetExtraParam(context.ExtraParams, "operationId");
        if (!string.IsNullOrWhiteSpace(operationNo) && !string.IsNullOrWhiteSpace(context.ItemGroupCode))
        {
            AddOperationAmount(operationNo, context.ItemGroupCode, result.FinalAmount);
        }

        if (!string.IsNullOrWhiteSpace(context.ItemCode))
        {
            SetParentItemAmount(context.ItemCode, result.FinalAmount);
        }
    }

    public static string BuildLimitDimensionKey(string limitType, string dimensionCode)
    {
        return RequestSharedStateKeys.BuildLimitDimensionKey(limitType, dimensionCode);
    }

    private decimal GetAccumulatedValue(string key)
    {
        return AccumulatedValues.TryGetValue(key, out var value) ? value : 0m;
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
