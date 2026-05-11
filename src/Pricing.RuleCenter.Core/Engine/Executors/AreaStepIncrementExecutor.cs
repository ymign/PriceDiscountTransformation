using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 面积分段递增公式执行器（B-5 类规则）。
/// </summary>
/// <remarks>
/// <para>
/// 【业务场景】带蒂皮瓣转移、游离皮瓣移植等手术，以面积为计费基础：
/// 面积 ≤ 基础面积（15cm²）时，按单价收费；超过后每增加一个基础面积段（或不足一段），
/// 追加一档递增比例。公式来源：12-历史规则分类与公式定义.md B-5 类。
/// </para>
/// <para>
/// 【公式定义】
/// <code>
/// steps = max(0, ceil((area - baseArea) / baseArea))
/// amount = unitPrice × (1 + steps × stepRate)
/// </code>
/// 示例（baseArea=15, stepRate=0.15）：
///   area=10 → steps=0 → amount=unitPrice
///   area=15 → steps=0 → amount=unitPrice
///   area=16 → steps=1 → amount=unitPrice × 1.15
///   area=30 → steps=1 → amount=unitPrice × 1.15
///   area=31 → steps=2 → amount=unitPrice × 1.30
/// </para>
/// <para>
/// 【面积来源】面积从 context.PricingParts 各明细的 Area 字段求和。
/// PricingParts 为空时面积视为 0，amount = unitPrice（0 步，基础收费）。
/// </para>
/// <para>
/// 【执行器编码二级分派】ActionType 为 FORMULA_CALC，ExecutorCode 为 AREA_STEP_INCREMENT。
/// 其他 FORMULA_CALC 动作不匹配时静默跳过。
/// </para>
/// </remarks>
public sealed class AreaStepIncrementExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码。与其他公式执行器共享 FORMULA_CALC，通过 ExecutorCode 做二级分派。
    /// </summary>
    public string ActionType => "FORMULA_CALC";

    /// <summary>
    /// 执行面积分段递增公式，把结果写回计价上下文。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段：
    /// <list type="bullet">
    ///   <item><description>BaseArea — 基础面积（cm²），默认 15</description></item>
    ///   <item><description>StepRate — 每段递增比例（如 0.15 表示每段加 15%），默认 0.15</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文，读取 UnitPrice 和 PricingParts，写入 FormulaAmount 和 FinalAmount。
    /// </param>
    /// <returns>已完成的异步任务（计算为纯内存操作，无 IO）。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：二级执行器编码过滤 ==========
        if (!string.Equals(action.ExecutorCode, "AREA_STEP_INCREMENT", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(action.ExecutorCode, "AreaStepIncrementExecutor", StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：解析动作参数 ==========
        var param = DeserializeParams(action.ParamsJson);
        var baseArea = param?.BaseArea > 0 ? param.BaseArea : 15m;
        var stepRate = param?.StepRate > 0 ? param.StepRate : 0.15m;

        // ========== 第三阶段：汇总面积 ==========
        // 多片段时求和，如双侧皮瓣各自有面积。
        // PricingParts 为空时面积 = 0，不加收。
        var totalArea = 0m;
        if (context.PricingParts is { Count: > 0 })
        {
            foreach (var part in context.PricingParts)
            {
                totalArea += part.Area ?? 0m;
            }
        }

        // ========== 第四阶段：计算阶梯步数 ==========
        // steps = max(0, ceil((area - baseArea) / baseArea))
        // 面积 ≤ baseArea 时 steps=0；超出时每超过 baseArea 一次，steps 加 1（不足一段向上取整）。
        var steps = 0m;
        if (totalArea > baseArea)
        {
            steps = Math.Ceiling((totalArea - baseArea) / baseArea);
        }

        // ========== 第五阶段：套用公式 ==========
        // amount = unitPrice × (1 + steps × stepRate)
        // steps=0 时：amount = unitPrice（基础一档，不加收）
        // steps=1 时：amount = unitPrice × 1.15（超出基础面积一段）
        var formulaAmount = context.UnitPrice * (1m + steps * stepRate);
        context.FormulaAmount = formulaAmount;
        context.FinalAmount = formulaAmount;

        // ========== 第六阶段：记录追踪步骤 ==========
        context.TraceSteps.Add(new TraceStep
        {
            StepNo = context.TraceSteps.Count + 1,
            StepType = "FORMULA",
            StepDesc = $"面积递增公式：面积 {totalArea}cm²，基础面积 {baseArea}cm²，" +
                       $"步数 {steps}，递增比例 {stepRate:P0}，" +
                       $"单价 {context.UnitPrice} × (1 + {steps} × {stepRate}) = {formulaAmount}",
            InputValue = totalArea,
            OutputValue = formulaAmount,
            ParamsJson = action.ParamsJson
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// 解析面积递增公式参数。
    /// </summary>
    private static AreaStepIncrementParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<AreaStepIncrementParams>(json);
    }

    /// <summary>
    /// 面积递增公式参数模型。
    /// </summary>
    private sealed class AreaStepIncrementParams
    {
        /// <summary>
        /// 基础面积（cm²）。面积在此以内按单价收费，超出后每段递增一档。
        /// 默认 15，对应五院 B-5 类规则业务口径。
        /// </summary>
        public decimal BaseArea { get; set; }

        /// <summary>
        /// 每段递增比例，取值范围通常为 0 到 1（如 0.15 表示每段加 15%）。
        /// 默认 0.15，对应五院 B-5 类规则业务口径。
        /// </summary>
        public decimal StepRate { get; set; }
    }
}
