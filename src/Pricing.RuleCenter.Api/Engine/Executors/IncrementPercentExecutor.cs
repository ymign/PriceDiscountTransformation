using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

/// <summary>
/// 按增量比例计算金额的公式动作执行器。
/// </summary>
/// <remarks>
/// 该执行器用于兼容“数量中只有一部分按比例加收/折算”的公式。它只处理 ExecutorCode 为
/// INCREMENT_PERCENT 或历史类名 IncrementPercentExecutor 的动作，其他 FORMULA_CALC 动作会被跳过。
/// </remarks>
public sealed class IncrementPercentExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码。多个公式执行器可以共享 FORMULA_CALC，再通过 ExecutorCode 做二级分派。
    /// </summary>
    public string ActionType => "FORMULA_CALC";

    /// <summary>
    /// 执行增量比例公式，并把结果写回计价上下文。
    /// </summary>
    /// <param name="action">规则动作配置，ParamsJson 中支持 Rate 或 Percent。</param>
    /// <param name="context">计价上下文，提供单价、转换后数量和当前金额。</param>
    /// <returns>已完成的异步任务。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：二级执行器编码过滤 ==========
        // ActionType 只是大类，ExecutorCode 才决定当前公式是否由本执行器处理；不匹配时静默跳过。
        if (!string.Equals(action.ExecutorCode, "INCREMENT_PERCENT", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(action.ExecutorCode, "IncrementPercentExecutor", StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：解析动作参数 ==========
        // 历史配置可能没有参数或参数为空，运行期保持跳过，避免单条脏规则阻断整个计价请求。
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return Task.CompletedTask;
        }

        // ========== 第三阶段：兼容 Rate 和 Percent 两种口径 ==========
        // Rate 直接表示 0-1 比例；Percent 表示百分数。优先 Rate，避免双字段同时存在时出现二次除以 100。
        var rate = param.Rate != 0 ? param.Rate : param.Percent / 100m;

        // ========== 第四阶段：计算公式金额 ==========
        // 公式含义：转换数量中按比例计算的部分加上未按比例折算的 1 个基础量，再乘以单价。
        // 计算结果同时写入 FormulaAmount 和 FinalAmount，后续封顶/保底等动作可以继续在 FinalAmount 上处理。
        context.FormulaAmount = context.UnitPrice *
            (context.ConvertedQty * rate + (1m - rate));
        context.FinalAmount = context.FormulaAmount;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 解析增量比例公式参数。
    /// </summary>
    /// <param name="json">动作参数 JSON。</param>
    /// <returns>解析后的参数；参数为空时返回 <c>null</c>。</returns>
    private static IncrementPercentParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<IncrementPercentParams>(json);
    }

    /// <summary>
    /// 增量比例公式参数。
    /// </summary>
    private sealed class IncrementPercentParams
    {
        /// <summary>
        /// 直接比例值，取值通常为 0 到 1；配置后优先于 Percent。
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 百分比值，例如 30 表示 30%，用于兼容配置人员更容易理解的输入方式。
        /// </summary>
        public decimal Percent { get; set; }
    }
}
