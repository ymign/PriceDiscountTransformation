using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

/// <summary>
/// 金额保底动作执行器。
/// </summary>
/// <remarks>
/// 该执行器只约束最终金额下限，不改变 FinalQty。它通常用于处理“最低收费金额”或
/// “折扣后不能低于某金额”的规则，通常放在公式计算之后。
/// </remarks>
public sealed class AmountFloorExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码，对应规则动作中的金额保底动作。
    /// </summary>
    public string ActionType => "APPLY_MIN_AMOUNT";

    /// <summary>
    /// 执行金额保底处理。
    /// </summary>
    /// <param name="action">规则动作配置，ParamsJson 中支持 MinAmount 或 FloorAmount。</param>
    /// <param name="context">计价上下文，当前 FinalAmount 会被保底。</param>
    /// <returns>已完成的异步任务。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：解析保底金额 ==========
        // MinAmount 是当前推荐字段，FloorAmount 用于兼容历史配置字段。
        var param = DeserializeParams(action.ParamsJson);
        var minAmount = param?.MinAmount ?? param?.FloorAmount;
        if (minAmount is null)
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：仅在低于下限时抬高金额 ==========
        // 高于下限时不改金额，确保前置动作的计算结果不被无意义覆盖。
        if (context.FinalAmount < minAmount.Value)
        {
            context.FinalAmount = minAmount.Value;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 解析金额保底参数。
    /// </summary>
    /// <param name="json">动作参数 JSON。</param>
    /// <returns>解析后的参数；参数为空时返回 <c>null</c>。</returns>
    private static AmountFloorParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<AmountFloorParams>(json);
    }

    /// <summary>
    /// 金额保底参数。
    /// </summary>
    private sealed class AmountFloorParams
    {
        /// <summary>
        /// 推荐字段，表示最终金额不能低于该值。
        /// </summary>
        public decimal? MinAmount { get; set; }
        /// <summary>
        /// 兼容字段，语义同 MinAmount。
        /// </summary>
        public decimal? FloorAmount { get; set; }
    }
}
