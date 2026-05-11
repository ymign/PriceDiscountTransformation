using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Engine.Executors;

/// <summary>
/// 同组互斥动作执行器。
/// </summary>
/// <remarks>
/// <para>
/// 【业务语义】同组内同类项目只允许执行优先级最高的一条，超出部分金额归零。
/// 例如：皮肤科"激光治疗"项目组包含 CO2 激光和半导体激光两个项目，
/// 同次收费中只允许对优先级最高的那条计费，其余超出部分金额归零。
/// </para>
/// <para>
/// 【执行顺序】在全局动作排序中，SAME_GROUP_MUTEX 排在限额动作之后、
/// DISCOUNT_EXCEED_TO_ZERO 之前。先由互斥动作截断数量，再由超额归零同步金额。
/// </para>
/// <para>
/// 【互斥组维度】通过 groupDimension 参数指定互斥维度：
/// <list type="bullet">
///   <item><description>ITEM_GROUP — 按项目组编码互斥（默认）</description></item>
///   <item><description>EXCLUSIVE_GROUP — 按规则动作的 ExclusiveGroup 互斥</description></item>
/// </list>
/// </para>
/// <para>
/// 【约束引用】
/// <list type="bullet">
///   <item><description>超出 = 0元：不是拒单，不是整单归零，仅超出部分为 0 元</description></item>
///   <item><description>FinalQty 为 0 时金额归零（由后续 DISCOUNT_EXCEED_TO_ZERO 同步）</description></item>
///   <item><description>同组互斥只截断数量，金额归零由 ExceedToZeroExecutor 统一处理</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class SameGroupMutexExecutor : IRuleActionExecutor
{
    /// <summary>
    /// 获取动作类型编码，对应规则动作中的同组互斥动作。
    /// </summary>
    public string ActionType => "SAME_GROUP_MUTEX";

    /// <summary>
    /// 执行同组互斥判断，超出配额的项目数量归零。
    /// </summary>
    /// <param name="action">
    /// 规则动作配置。ParamsJson 中支持以下字段：
    /// <list type="bullet">
    ///   <item><description>groupDimension — 互斥维度，默认 ITEM_GROUP</description></item>
    ///   <item><description>maxCountPerGroup — 同组内允许计费的最大项目数，默认 1</description></item>
    /// </list>
    /// </param>
    /// <param name="context">
    /// 计价上下文，提供：
    /// <list type="bullet">
    ///   <item><description>ItemGroupCode — 当前项目所属的项目组编码</description></item>
    ///   <item><description>OrderedActions — 已排序的动作链，用于判断同组内已处理的项目</description></item>
    ///   <item><description>FinalQty — 最终数量（本执行器可能归零）</description></item>
    /// </list>
    /// </param>
    /// <returns>已完成的异步任务（互斥判断为纯内存操作，无 IO）。</returns>
    public Task ExecuteAsync(RuleAction action, PricingContext context)
    {
        // ========== 第一阶段：解析互斥参数 ==========
        var param = DeserializeParams(action.ParamsJson);
        var maxCount = param?.MaxCountPerGroup > 0 ? param.MaxCountPerGroup : 1;
        var groupDimension = !string.IsNullOrEmpty(param?.GroupDimension)
            ? param.GroupDimension
            : "ITEM_GROUP";

        // ========== 第二阶段：获取当前项目的互斥组标识 ==========
        // 按互斥维度确定当前项目属于哪个互斥组。
        // ITEM_GROUP 模式使用 PricingContext.ItemGroupCode；
        // EXCLUSIVE_GROUP 模式使用规则动作的 ExclusiveGroup 字段。
        var currentGroupKey = GetGroupKey(context, action, groupDimension);
        if (string.IsNullOrEmpty(currentGroupKey))
        {
            // 无法确定互斥组时静默跳过——可能该项目不属于任何互斥组。
            return Task.CompletedTask;
        }

        // ========== 第三阶段：检查同组内已处理的项目数量 ==========
        // 同组互斥的判断依赖于 batch-simulate 共享的 BatchPricingContext。
        // 这里通过 InRequestOccupiedQtyByLimitDimension 间接获取同组内已处理的项目数量。
        // 维度键格式：MUTEX:{groupKey}
        var mutexKey = $"MUTEX:{currentGroupKey}".ToUpperInvariant();
        var processedCount = 0;
        if (context.InRequestOccupiedQtyByLimitDimension.TryGetValue(mutexKey, out var countValue))
        {
            processedCount = (int)countValue;
        }

        // ========== 第四阶段：超出配额时归零数量 ==========
        // 当同组内已处理的项目数 >= maxCountPerGroup 时，当前项目数量归零。
        // 归零逻辑只改 FinalQty，不改 FinalAmount——金额归零由后续 DISCOUNT_EXCEED_TO_ZERO 统一处理。
        // 这是"只负责判断，不负责同步"的设计，与 ExceedToZeroExecutor 配合。
        if (processedCount >= maxCount)
        {
            context.FinalQty = 0;

            // 记录追踪步骤：解释为什么数量被归零。
            context.TraceSteps.Add(new TraceStep
            {
                StepNo = context.TraceSteps.Count + 1,
                StepType = "LIMIT",
                StepDesc = $"同组互斥：项目组 {currentGroupKey} 内已处理 {processedCount} 个项目，" +
                           $"超过上限 {maxCount}，当前项目数量归零",
                InputValue = context.FinalQty,
                OutputValue = 0,
                ParamsJson = action.ParamsJson
            });

            return Task.CompletedTask;
        }

        // ========== 第五阶段：当前项目通过互斥校验 ==========
        // 通过校验时不修改数量，仅记录追踪步骤说明互斥校验结果。
        context.TraceSteps.Add(new TraceStep
        {
            StepNo = context.TraceSteps.Count + 1,
            StepType = "LIMIT",
            StepDesc = $"同组互斥校验通过：项目组 {currentGroupKey} 内已处理 {processedCount} 个项目，" +
                       $"未超过上限 {maxCount}",
            InputValue = context.FinalQty,
            OutputValue = context.FinalQty,
            ParamsJson = action.ParamsJson
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据互斥维度获取当前项目的互斥组标识。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <param name="action">规则动作，用于获取 ExclusiveGroup。</param>
    /// <param name="groupDimension">互斥维度。</param>
    /// <returns>互斥组标识；无法确定时返回空字符串。</returns>
    private static string GetGroupKey(
        PricingContext context, RuleAction action, string groupDimension)
    {
        return groupDimension.ToUpperInvariant() switch
        {
            // 按项目组编码互斥：使用 PricingContext.ItemGroupCode。
            "ITEM_GROUP" => context.ItemGroupCode ?? string.Empty,
            // 按规则动作的 ExclusiveGroup 互斥：直接使用动作配置中的互斥组编码。
            "EXCLUSIVE_GROUP" => action.ExclusiveGroup ?? string.Empty,
            // 默认使用项目组编码。
            _ => context.ItemGroupCode ?? string.Empty
        };
    }

    /// <summary>
    /// 解析同组互斥参数。
    /// </summary>
    /// <param name="json">动作参数 JSON 字符串。</param>
    /// <returns>解析后的参数对象；JSON 为空时返回 <c>null</c>。</returns>
    private static SameGroupMutexParams? DeserializeParams(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<SameGroupMutexParams>(json);
    }

    /// <summary>
    /// 同组互斥参数模型。
    /// </summary>
    private sealed class SameGroupMutexParams
    {
        /// <summary>
        /// 互斥维度编码。
        /// ITEM_GROUP — 按项目组编码互斥（默认）。
        /// EXCLUSIVE_GROUP — 按规则动作的 ExclusiveGroup 字段互斥。
        /// </summary>
        public string? GroupDimension { get; set; }

        /// <summary>
        /// 同组内允许计费的最大项目数。
        /// 默认为 1，即同组只允许一条项目计费。
        /// NULL 或 0 时默认为 1。
        /// </summary>
        public int MaxCountPerGroup { get; set; }
    }
}
