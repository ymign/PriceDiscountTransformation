using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 共享费用明细计价运行器。
/// </summary>
/// <remarks>
/// 负责把多条费用明细转换成逐条 <see cref="PricingContext"/>，并维护请求内累计与批量上下文。
/// simulate 和 confirm 只需声明自己的调用类型和是否需要锁定限额。
/// </remarks>
public sealed class PricingItemCalculationRunner
{
    private readonly IPricingEngine _engine;

    /// <summary>
    /// 初始化共享费用明细计价运行器。
    /// </summary>
    /// <param name="engine">计价核心引擎。</param>
    public PricingItemCalculationRunner(IPricingEngine engine)
    {
        _engine = engine;
    }

    /// <summary>
    /// 执行一次多明细计价。
    /// </summary>
    /// <param name="request">原始计价请求。</param>
    /// <param name="items">经过基础校验后的费用明细。</param>
    /// <param name="callType">调用类型，例如 SIMULATE/CONFIRM。</param>
    /// <param name="shouldLockLimits">是否允许执行器锁定数据库限额。</param>
    /// <returns>逐条费用明细的输入输出配对结果。</returns>
    internal async Task<IReadOnlyList<ItemPricingCalculation>> RunAsync(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items,
        string callType,
        bool shouldLockLimits)
    {
        var inRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        var inRequestLimitOccupies = new List<LimitOccupy>();
        var batchContext = items.Count > 1 ? new BatchPricingContext() : null;
        var calculations = new List<ItemPricingCalculation>(items.Count);

        foreach (var item in items)
        {
            var context = PricingContextFactory.Create(new PricingContextBuildInput
            {
                Request = request,
                Item = item,
                CallType = callType,
                ShouldLockLimits = shouldLockLimits,
                InRequestOccupiedQtyByLimitDimension = inRequestOccupiedQtyByLimitDimension,
                InRequestLimitOccupies = inRequestLimitOccupies
            });
            var result = await _engine.CalculateAsync(context, batchContext);
            PricingInRequestLimitAccumulator.Accumulate(
                inRequestOccupiedQtyByLimitDimension,
                inRequestLimitOccupies,
                result);
            calculations.Add(new ItemPricingCalculation(item, result));
        }

        return calculations;
    }
}
