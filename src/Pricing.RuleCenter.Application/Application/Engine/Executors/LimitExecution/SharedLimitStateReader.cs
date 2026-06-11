using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Executors;

/// <summary>
/// 请求共享限额状态读取器。
/// </summary>
internal static class SharedLimitStateReader
{
    /// <summary>
    /// 读取请求共享状态中的数量累计。
    /// </summary>
    public static decimal GetOccupiedQty(
        PricingContext context,
        string limitType,
        string dimensionCode,
        DateTime? startTime = null,
        DateTime? endTime = null,
        params string[] fallbackKeys)
    {
        var candidates = GetMatchingOccupies(context, limitType, dimensionCode);
        if (candidates.Count > 0)
        {
            return FilterByBusinessTime(candidates, startTime, endTime).Sum(occupy => occupy.OccupyQty);
        }

        return GetAccumulatedValue(context, limitType, dimensionCode, fallbackKeys);
    }

    /// <summary>
    /// 读取请求共享状态中的金额累计。
    /// </summary>
    public static decimal GetOccupiedAmt(
        PricingContext context,
        string limitType,
        string dimensionCode,
        params string[] fallbackKeys)
    {
        var candidates = GetMatchingOccupies(context, limitType, dimensionCode);
        if (candidates.Count > 0)
        {
            return candidates.Sum(occupy => occupy.OccupyAmt);
        }

        return GetAccumulatedValue(context, limitType, dimensionCode, fallbackKeys);
    }

    private static List<LimitOccupy> GetMatchingOccupies(
        PricingContext context,
        string limitType,
        string dimensionCode)
    {
        return context.RequestSharedState.LimitOccupies
            .Where(occupy => string.Equals(occupy.LimitType, limitType, StringComparison.OrdinalIgnoreCase))
            .Where(occupy => string.Equals(occupy.LimitDimensionCode, dimensionCode, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static IEnumerable<LimitOccupy> FilterByBusinessTime(
        IEnumerable<LimitOccupy> occupies,
        DateTime? startTime,
        DateTime? endTime)
    {
        if (!startTime.HasValue || !endTime.HasValue)
        {
            return occupies;
        }

        return occupies.Where(occupy =>
            occupy.BusinessChargeTime >= startTime.Value &&
            occupy.BusinessChargeTime <= endTime.Value);
    }

    private static decimal GetAccumulatedValue(
        PricingContext context,
        string limitType,
        string dimensionCode,
        IReadOnlyCollection<string> fallbackKeys)
    {
        var keys = new List<string>(fallbackKeys.Count + 1)
        {
            RequestSharedStateKeys.BuildLimitDimensionKey(limitType, dimensionCode)
        };
        keys.AddRange(fallbackKeys);

        foreach (var key in keys)
        {
            if (context.RequestSharedState.TryGetAccumulatedValue(key, out var value))
            {
                return value;
            }
        }

        return 0m;
    }
}
