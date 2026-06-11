using Pricing.RuleCenter.Application.Serialization;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine.Executors;

/// <summary>
/// 金额保底（下限）动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】处理"最低收费金额"或"折扣后不能低于某金额"的规则。
/// 例如：某项目折后最低收 10 元，避免折价后金额过低甚至为 0。
/// </para>
/// <para>
/// 【执行顺序】通常放在公式计算（FORMULA_CALC）之后、封顶（APPLY_MAX_AMOUNT）之前。
/// 在全局动作排序中，APPLY_MIN_AMOUNT 排在 FORMULA_CALC 之后、APPLY_MAX_AMOUNT 之前。
/// 先保底再封顶，确保保底结果可能被封顶截断（配置冲突时应由发布校验发现）。
/// </para>
/// <para>
/// 【约束引用】
/// <list type="bullet">
///   <item><description>只约束 FinalAmount，不改变 FinalQty（数量不变）</description></item>
///   <item><description>高于下限时不改金额，确保前置动作的计算结果不被无意义覆盖</description></item>
///   <item><description>金额取整不在本执行器处理，最终由 PricingEngine 统一保留 2 位小数、四舍五入</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class AmountFloorExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码，对应规则动作中的金额保底动作。
    /// </summary>
    public string ActionType => "APPLY_MIN_AMOUNT";

    /// <summary>
    /// 执行金额保底处理。仅在当前 FinalAmount 低于下限时抬高。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段（二选一）：
    /// <list type="bullet">
    ///   <item><description>MinAmount — 推荐字段，最终金额不能低于该值</description></item>
    ///   <item><description>FloorAmount — 兼容字段，语义同 MinAmount</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文。本执行器读取并修改 FinalAmount。
    /// </param>
    /// <returns>已完成的异步任务。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：解析保底金额 ==========
        // MinAmount 是当前推荐字段，FloorAmount 用于兼容历史配置字段。
        // 两个字段都为 null 时静默跳过——缺失参数应在发布校验阶段发现。
        var param = DeserializeParams(action.ParamsJson);
        var minAmount = param?.MinAmount ?? param?.FloorAmount;
        if (minAmount is null)
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：仅在低于下限时抬高金额 ==========
        // 使用 Math.Max 实现保底，语义清晰：FinalAmount = max(当前金额, 下限金额)。
        // 高于下限时不改金额，确保前置动作（如公式计算、折价）的计算结果不被无意义覆盖。
        if (context.FinalAmount < minAmount.Value)
        {
            context.FinalAmount = minAmount.Value;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 解析金额保底参数。
    /// </summary>
    /// <param name="json">动作参数 JSON 字符串。</param>
    /// <returns>解析后的参数对象；JSON 为空时返回 <c>null</c>。</returns>
    private static AmountFloorParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return RuleCenterJsonSerializer.Deserialize<AmountFloorParams>(json);
    }

    /// <summary>
    /// 金额保底参数模型。
    /// </summary>
    private sealed class AmountFloorParams
    {
        /// <summary>
        /// 推荐字段，表示最终金额不能低于该值。
        /// 使用 decimal? 而非 decimal，以便区分"未配置"和"配置为 0"。
        /// NULL 表示未配置（不校验），0 表示保底为 0（等同于不保底）。
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// 兼容字段，语义同 MinAmount。用于旧配置系统迁移。
        /// </summary>
        public decimal? FloorAmount { get; set; }
    }
}
