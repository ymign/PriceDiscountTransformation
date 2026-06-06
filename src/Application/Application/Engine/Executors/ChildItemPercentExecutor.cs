using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine.Executors;

/// <summary>
/// 子项金额百分比公式执行器（B-4 儿童加收规则）。
/// </summary>
/// <remarks>
/// <para>
/// 【业务场景】巨痣、血管瘤等手术有儿童加收子项目（如"浅表肿物去除费-儿童(加收)"），
/// 其金额 = 主项目经公式和封顶后的最终金额 × 加收比例（通常 30%）。
/// 基准是主项目的最终金额而非单价×数量，因此不能用 AddChildItemExecutor 直接处理。
/// 公式来源：12-历史规则分类与公式定义.md B-4 儿童加收。
/// </para>
/// <para>
/// 【批量依赖】本执行器需要在批量请求中找到父项目的最终金额。
/// 通过 context.InRequestOccupiedQtyByLimitDimension["ITEM_AMT:{parentItemCode}"] 获取，
/// 该值由 BatchPricingContext.AccumulateToBatch 在父项目计算完成后写入。
/// <strong>父项目必须在批量请求中先于子项目处理，此为调用方责任。</strong>
/// </para>
/// <para>
/// 【单批约束】如果父项目和子项目在同一批次中，父项必须排在子项前面。
/// 如果两者在不同批次（先收父项、后收子项），父项的 ITEM_AMT 不会注入当前上下文，
/// 则应改用 AddChildItemExecutor 并由 HIS 端传入子项单价。
/// </para>
/// <para>
/// 【执行器编码二级分派】ActionType 为 FORMULA_CALC，ExecutorCode 为 CHILD_ITEM_PERCENT。
/// 其他 FORMULA_CALC 动作不匹配时静默跳过。
/// </para>
/// </remarks>
public sealed class ChildItemPercentExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码。与其他公式执行器共享 FORMULA_CALC，通过 ExecutorCode 做二级分派。
    /// </summary>
    public string ActionType => "FORMULA_CALC";

    /// <summary>
    /// 判断当前公式动作是否应由子项金额百分比公式执行器处理。
    /// </summary>
    /// <param name="action">待执行的规则动作，ExecutorCode 必须是子项百分比公式编码或历史类名。</param>
    /// <returns>匹配子项百分比公式时返回 <c>true</c>，否则返回 <c>false</c>。</returns>
    public bool CanHandle(RuleAction action)
    {
        return FormulaExecutorCodes.IsChildItemPercent(action.ExecutorCode);
    }

    /// <summary>
    /// 执行子项金额百分比公式，把结果写回计价上下文。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段：
    /// <list type="bullet">
    ///   <item><description>ParentItemCode — 父项目编码（必填），用于查找父项目最终金额</description></item>
    ///   <item><description>ChildRate — 加收比例（0-1），如 0.30 表示加收 30%</description></item>
    ///   <item><description>ChildPercent — 加收百分数，如 30 表示 30%；ChildRate 非零时优先</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文，从 InRequestOccupiedQtyByLimitDimension 获取父项目金额，
    /// 写入 FormulaAmount 和 FinalAmount。
    /// </param>
    /// <returns>已完成的异步任务（计算为纯内存操作，无 IO）。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：二级执行器编码过滤 ==========
        if (!CanHandle(action))
        {
            return Task.CompletedTask;
        }

        // ========== 第二阶段：解析动作参数 ==========
        var param = DeserializeParams(action.ParamsJson);
        if (param is null || string.IsNullOrEmpty(param.ParentItemCode))
        {
            return Task.CompletedTask;
        }

        // ========== 第三阶段：兼容 ChildRate 和 ChildPercent 两种口径 ==========
        var rate = param.ChildRate != 0 ? param.ChildRate : param.ChildPercent / 100m;
        if (rate <= 0m)
        {
            return Task.CompletedTask;
        }

        // ========== 第四阶段：查找父项目最终金额 ==========
        // 父项目金额由 BatchPricingContext.AccumulateToBatch 以 "ITEM_AMT:{itemCode}" 为键写入
        // InBatchOccupiedQtyByDimension，再由 PricingEngine.InjectBatchContext 注入到当前上下文。
        // 父项不在同一批次时查不到，静默跳过（此场景应改用 AddChildItemExecutor）。
        var parentAmtKey = $"ITEM_AMT:{param.ParentItemCode.Trim().ToUpperInvariant()}";
        if (!context.InRequestOccupiedQtyByLimitDimension.TryGetValue(parentAmtKey, out var parentFinalAmount))
        {
            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "FORMULA",
                StepDesc = $"子项金额百分比：未找到父项目 {param.ParentItemCode} 的最终金额（父项不在同一批次，跳过）",
                InputValue = 0,
                OutputValue = context.FinalAmount,
                ParamsJson = action.ParamsJson
            });
            return Task.CompletedTask;
        }

        // ========== 第五阶段：计算子项金额 ==========
        // childAmount = parentFinalAmount × rate
        // 中间计算保留全精度，最终取整由 PricingEngine 统一处理。
        var childAmount = parentFinalAmount * rate;
        context.FormulaAmount = childAmount;
        context.FinalAmount = childAmount;

        // ========== 第六阶段：记录追踪步骤 ==========
        context.TraceSteps.Add(new TraceStep
        {
            StepNo = context.TraceSteps.Count + 1,
            StepType = "FORMULA",
            StepDesc = $"子项金额百分比：父项目 {param.ParentItemCode} 最终金额 {parentFinalAmount}，" +
                       $"加收比例 {rate:P0}，子项金额 {childAmount}",
            InputValue = parentFinalAmount,
            OutputValue = childAmount,
            ParamsJson = action.ParamsJson
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// 解析子项金额百分比参数。
    /// </summary>
    private static ChildItemPercentParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<ChildItemPercentParams>(json);
    }

    /// <summary>
    /// 子项金额百分比参数模型。
    /// </summary>
    private sealed class ChildItemPercentParams
    {
        /// <summary>
        /// 父项目编码。子项金额基于此项目的最终金额计算。
        /// 不可为空；父项目必须在同一批次中先于子项目被计价。
        /// </summary>
        public string? ParentItemCode { get; set; }

        /// <summary>
        /// 加收比例，取值范围通常为 0 到 1（如 0.30 表示 30%）。
        /// 非零时优先于 ChildPercent 使用。
        /// </summary>
        public decimal ChildRate { get; set; }

        /// <summary>
        /// 加收百分数，例如 30 表示 30%。用于兼容习惯百分数输入的配置人员。
        /// 当 ChildRate 为 0 时，实际比例 = ChildPercent / 100。
        /// </summary>
        public decimal ChildPercent { get; set; }
    }
}
