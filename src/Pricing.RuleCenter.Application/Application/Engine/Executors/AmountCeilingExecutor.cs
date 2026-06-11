using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Executors;

/// <summary>
/// 金额封顶（上限）动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】处理"本项目最高收费不超过某金额"的规则。
/// 例如：某检查项目无论公式计算结果多高，最多只收 500 元。
/// </para>
/// <para>
/// 【执行顺序】通常放在公式计算（FORMULA_CALC）之后，确保在公式结果上做截断。
/// 在全局动作排序中，APPLY_MAX_AMOUNT 排在 FORMULA_CALC 之后。
/// </para>
/// <para>
/// 【约束引用】
/// <list type="bullet">
///   <item><description>只约束 FinalAmount，不改变 FinalQty（数量不变）</description></item>
///   <item><description>低于上限时不改金额，避免破坏前置动作已计算出的优惠结果</description></item>
///   <item><description>金额取整不在本执行器处理，最终由 PricingEngine 统一保留 2 位小数、四舍五入</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class AmountCeilingExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码，对应规则动作中的金额封顶动作。
    /// </summary>
    public string ActionType => "APPLY_MAX_AMOUNT";

    /// <summary>
    /// 执行金额封顶处理。仅在当前 FinalAmount 超过上限时截断。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段（二选一）：
    /// <list type="bullet">
    ///   <item><description>MaxAmount — 推荐字段，允许收取的最高金额</description></item>
    ///   <item><description>CeilingAmount — 兼容字段，语义同 MaxAmount</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文。本执行器读取并修改 FinalAmount。
    /// </param>
    /// <returns>已完成的异步任务。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：解析封顶金额 ==========
        // MaxAmount 是当前推荐字段，CeilingAmount 用于兼容旧配置。
        // 两个字段都为 null 时静默跳过，不抛异常——缺失参数应在发布校验阶段发现。
        var param = DeserializeParams(action.ParamsJson);
        var maxAmount = param?.MaxAmount ?? param?.CeilingAmount;
        if (maxAmount is null)
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：仅在超出上限时截断 ==========
        // 使用 Math.Min 实现截断，语义清晰：FinalAmount = min(当前金额, 上限金额)。
        // 低于上限时不改金额，避免破坏前置动作（如公式计算、保底）已经确定的金额。
        if (context.FinalAmount > maxAmount.Value)
        {
            context.FinalAmount = maxAmount.Value;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 解析金额封顶参数。
    /// </summary>
    /// <param name="json">动作参数 JSON 字符串。</param>
    /// <returns>解析后的参数对象；JSON 为空时返回 <c>null</c>。</returns>
    private static AmountCeilingParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return RuleCenterJsonSerializer.Deserialize<AmountCeilingParams>(json);
    }

    /// <summary>
    /// 金额封顶参数模型。
    /// </summary>
    private sealed class AmountCeilingParams
    {
        /// <summary>
        /// 推荐字段，表示允许收取的最高金额。
        /// 使用 decimal? 而非 decimal，以便区分"未配置"和"配置为 0"。
        /// NULL 表示未配置（不校验），0 表示限制为零（免费）。
        /// </summary>
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// 兼容字段，语义同 MaxAmount。用于旧配置系统迁移。
        /// </summary>
        public decimal? CeilingAmount { get; set; }
    }
}
