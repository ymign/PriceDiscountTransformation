using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 超额归零动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】当收费数量超出限额时，超出部分金额归零。
/// 这是"超出 = 0元"业务规则的具体实现——不是拒单，不是整单归零，仅超出部分为 0 元。
/// </para>
/// <para>
/// 【执行顺序】在全局动作排序中，DISCOUNT_EXCEED_TO_ZERO 排在所有限额动作之后，
/// 确保先由限额动作（日限、时间窗限、单次限、互斥）截断数量，再由本执行器同步归零金额。
/// </para>
/// <para>
/// 【触发条件】本执行器不主动判断限额——它只检查前置限额动作是否已经把 FinalQty 处理为 0 或负数。
/// 如果 FinalQty 仍为正数，说明未超出限额，本执行器不修改任何金额。
/// 这是"只负责同步，不负责判断"的设计。
/// </para>
/// <para>
/// 【约束引用】
/// <list type="bullet">
///   <item><description>超出 = 0元：不是拒单，不是整单归零，仅超出部分为 0 元</description></item>
///   <item><description>FinalQty 为 0 时金额归零；FinalQty 为负数时同样归零（防御性处理）</description></item>
///   <item><description>不读取 ParamsJson——本执行器不需要额外配置参数</description></item>
/// </list>
/// </para>
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
    /// <param name="action">
    /// 规则动作配置。本执行器不需要读取 ParamsJson——归零逻辑完全由 FinalQty 驱动。
    /// </param>
    /// <param name="context">
    /// 计价上下文，提供：
    /// <list type="bullet">
    ///   <item><description>FinalQty — 最终数量（已被前置限额动作截断）</description></item>
    ///   <item><description>FinalAmount — 最终金额（本执行器可能归零）</description></item>
    /// </list>
    /// </param>
    /// <returns>已完成的异步任务。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // 只有数量已经被前置动作处理成 0 或负数时才归零金额，避免误伤正常折扣金额。
        // <= 0 判断而非 == 0，是为了防御性处理负数场景（理论上不应出现，但避免金额残留）。
        if (context.FinalQty <= 0)
        {
            context.FinalAmount = 0;
        }

        return Task.CompletedTask;
    }
}
