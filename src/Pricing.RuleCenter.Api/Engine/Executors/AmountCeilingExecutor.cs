using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

/// <summary>
/// 金额封顶动作执行器。
/// </summary>
/// <remarks>
/// 该执行器只约束最终金额上限，不改变 FinalQty。它通常放在公式计算之后，
/// 用于处理“本项目最高收费不超过某金额”的规则。
/// </remarks>
public sealed class AmountCeilingExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码，对应规则动作中的金额封顶动作。
    /// </summary>
    public string ActionType => "APPLY_MAX_AMOUNT";

    /// <summary>
    /// 执行金额封顶处理。
    /// </summary>
    /// <param name="action">规则动作配置，ParamsJson 中支持 MaxAmount 或 CeilingAmount。</param>
    /// <param name="context">计价上下文，当前 FinalAmount 会被封顶。</param>
    /// <returns>已完成的异步任务。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：解析封顶金额 ==========
        // MaxAmount 是当前推荐字段，CeilingAmount 用于兼容旧配置。
        var param = DeserializeParams(action.ParamsJson);
        var maxAmount = param?.MaxAmount ?? param?.CeilingAmount;
        if (maxAmount is null)
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：仅在超出上限时截断 ==========
        // 低于上限时不改金额，避免破坏前置动作已经计算出的优惠结果。
        if (context.FinalAmount > maxAmount.Value)
        {
            context.FinalAmount = maxAmount.Value;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 解析金额封顶参数。
    /// </summary>
    /// <param name="json">动作参数 JSON。</param>
    /// <returns>解析后的参数；参数为空时返回 <c>null</c>。</returns>
    private static AmountCeilingParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<AmountCeilingParams>(json);
    }

    /// <summary>
    /// 金额封顶参数。
    /// </summary>
    private sealed class AmountCeilingParams
    {
        /// <summary>
        /// 推荐字段，表示允许收取的最高金额。
        /// </summary>
        public decimal? MaxAmount { get; set; }
        /// <summary>
        /// 兼容字段，语义同 MaxAmount。
        /// </summary>
        public decimal? CeilingAmount { get; set; }
    }
}
