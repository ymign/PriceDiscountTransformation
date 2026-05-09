using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

/// <summary>
/// 超额归零动作执行器。
/// </summary>
/// <remarks>
/// 该执行器用于兜底处理前置限额动作已经把可收费数量截断为 0 的场景。它不主动判断限额，
/// 只根据当前 FinalQty 判断是否把金额同步归零，避免数量为 0 但金额仍残留。
/// </remarks>
public sealed class ExceedToZeroExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码，对应规则动作中的超额归零动作。
    /// </summary>
    public string ActionType => "DISCOUNT_EXCEED_TO_ZERO";

    /// <summary>
    /// 根据当前最终数量决定是否把金额归零。
    /// </summary>
    /// <param name="action">规则动作配置；当前执行器不需要读取参数。</param>
    /// <param name="context">计价上下文，提供 FinalQty 和 FinalAmount。</param>
    /// <returns>已完成的异步任务。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // 只有数量已经被前置动作处理成 0 或负数时才归零金额，避免误伤正常折扣金额。
        if (context.FinalQty <= 0)
        {
            context.FinalAmount = 0;
        }

        return Task.CompletedTask;
    }
}
