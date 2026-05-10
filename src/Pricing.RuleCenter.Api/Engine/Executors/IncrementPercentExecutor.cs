using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

/// <summary>
/// 按增量比例计算金额的公式动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务场景】某些收费项目（如病理检查）的数量中，只有一部分按比例加收/折算。
/// 例如：3 个肿物中，第 1 个按全价，第 2-3 个按 50% 加收。公式为：
/// <c>金额 = 单价 × (转换数量 × 比例 + (1 - 比例))</c>
/// </para>
/// <para>
/// 【执行器编码二级分派】本执行器的 ActionType 为 FORMULA_CALC（与其他公式执行器共享），
/// 通过 ExecutorCode 做二级分派。只有 ExecutorCode 为 INCREMENT_PERCENT 或历史类名
/// IncrementPercentExecutor 时才处理，其他 FORMULA_CALC 动作静默跳过。
/// </para>
/// <para>
/// 【公式推导】
/// 设转换数量为 Q，比例为 R（0 ≤ R ≤ 1），单价为 P：
/// <code>
/// 金额 = P × (Q × R + (1 - R))
///      = P × (1 + (Q - 1) × R)
/// </code>
/// 当 R=1 时，金额 = P × Q（全量计价）；当 R=0 时，金额 = P（固定 1 个基础量）。
/// </para>
/// <para>
/// 【约束引用】
/// <list type="bullet">
///   <item><description>换算数量固定为1：公式使用换算后数量（ConvertedQty），非输入数量</description></item>
///   <item><description>公式优先于限制：本执行器写入金额后，后续 LIMIT 类动作再做比较</description></item>
///   <item><description>公式不自动跳过限制：是否执行数量/时间窗限制以规则动作配置为准</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class IncrementPercentExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码。多个公式执行器共享 FORMULA_CALC，再通过 ExecutorCode 做二级分派。
    /// </summary>
    /// <remarks>
    /// 这是"一级分派 + 二级分派"设计：ActionExecutionPipeline 按 ActionType 找到本执行器，
    /// 本执行器内部再按 ExecutorCode 判断是否由自己处理。好处是新增公式类型时只需新增执行器类，
    /// 无需在 ActionTypeOrder 中新增条目。
    /// </remarks>
    public string ActionType => "FORMULA_CALC";

    /// <summary>
    /// 执行增量比例公式，并把结果写回计价上下文。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段：
    /// <list type="bullet">
    ///   <item><description>Rate — 直接比例值（0-1），优先使用</description></item>
    ///   <item><description>Percent — 百分比值（如 30 表示 30%），兼容旧配置</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文，提供：
    /// <list type="bullet">
    ///   <item><description>UnitPrice — 单价（权威来源，非渠道传入）</description></item>
    ///   <item><description>ConvertedQty — 换算后数量（经过双单位换算）</description></item>
    ///   <item><description>FormulaAmount — 公式计算结果（输出）</description></item>
    ///   <item><description>FinalAmount — 最终金额（输出，后续动作继续修改）</description></item>
    /// </list>
    /// </param>
    /// <returns>已完成的异步任务（计算为纯内存操作，无 IO）。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：二级执行器编码过滤 ==========
        // ActionType 只是大类（FORMULA_CALC），ExecutorCode 才决定当前公式是否由本执行器处理。
        // 不匹配时静默跳过，不抛异常——其他公式执行器（如 LINEAR_DISCOUNT）会处理各自编码。
        if (!string.Equals(action.ExecutorCode, "INCREMENT_PERCENT", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(action.ExecutorCode, "IncrementPercentExecutor", StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：解析动作参数 ==========
        // 历史配置可能没有参数或参数为空，运行期保持跳过，避免单条脏规则阻断整个计价请求。
        // 参数缺失应在规则发布校验阶段发现，运行期做防御性处理。
        var param = DeserializeParams(action.ParamsJson);
        if (param is null)
        {
            return Task.CompletedTask;
        }

        // ========== 第三阶段：兼容 Rate 和 Percent 两种口径 ==========
        // Rate 直接表示 0-1 比例（如 0.5 表示 50%）；Percent 表示百分数（如 50 表示 50%）。
        // 优先使用 Rate，避免双字段同时存在时出现二次除以 100 的错误。
        // 两者都为 0 时按 0 处理（无增量），不会除零。
        var rate = param.Rate != 0 ? param.Rate : param.Percent / 100m;

        // ========== 第四阶段：计算公式金额 ==========
        // 公式含义：金额 = 单价 × (转换数量 × 比例 + (1 - 比例))
        //   - 转换数量中按比例计算的部分：ConvertedQty × Rate
        //   - 未按比例折算的 1 个基础量：(1 - Rate)
        //   - 两者相加后乘以单价
        //
        // 示例：单价=100, 转换数量=3, Rate=0.5
        //   金额 = 100 × (3 × 0.5 + 0.5) = 100 × 2 = 200
        //   而非 100 × 3 × 0.5 = 150（那是纯比例折算，缺少基础量）
        //
        // 中间计算保留全精度（decimal），不提前取整。
        // 计算结果同时写入 FormulaAmount 和 FinalAmount：
        //   - FormulaAmount：记录公式计算结果，供追溯使用
        //   - FinalAmount：后续封顶/保底等动作继续在此基础上处理
        context.FormulaAmount = context.UnitPrice *
            (context.ConvertedQty * rate + (1m - rate));
        context.FinalAmount = context.FormulaAmount;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 解析增量比例公式参数。
    /// </summary>
    /// <param name="json">动作参数 JSON 字符串。</param>
    /// <returns>解析后的参数对象；JSON 为空或解析失败时返回 <c>null</c>。</returns>
    private static IncrementPercentParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<IncrementPercentParams>(json);
    }

    /// <summary>
    /// 增量比例公式参数模型。
    /// </summary>
    /// <remarks>
    /// 支持两种比例输入方式，优先级：Rate > Percent。
    /// 配置人员可根据习惯选择，系统自动兼容。
    /// </remarks>
    private sealed class IncrementPercentParams
    {
        /// <summary>
        /// 直接比例值，取值范围通常为 0 到 1（如 0.5 表示 50%）。
        /// 非零时优先于 Percent 使用。
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 百分比值，例如 30 表示 30%。用于兼容配置人员更容易理解的输入方式。
        /// 当 Rate 为 0 时，实际比例 = Percent / 100。
        /// </summary>
        public decimal Percent { get; set; }
    }
}
