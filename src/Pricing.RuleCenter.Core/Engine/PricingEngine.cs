using Microsoft.Extensions.Logging;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Services;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Engine;

/// <summary>
/// 计价引擎，负责把一次标准化计价上下文转换为最终计价结果。
/// </summary>
/// <remarks>
/// 该类型不直接写数据库，也不处理 HTTP 幂等。它的边界是：初始化计价上下文、匹配规则、
/// 执行动作链、计算折价金额，并把执行器生成的占额草稿随结果返回给应用服务。
/// </remarks>
public sealed class PricingEngine : IPricingEngine
{
    /// <summary>
    /// 规则匹配服务，负责找出当前计价上下文命中的规则和动作链。
    /// </summary>
    private readonly RuleMatchService _ruleMatchService;
    /// <summary>
    /// 动作执行管线，负责按顺序执行已经排序好的动作。
    /// </summary>
    private readonly ActionExecutionPipeline _pipeline;
    /// <summary>
    /// 引擎日志，用于记录计价完成的关键金额。
    /// </summary>
    private readonly ILogger<PricingEngine> _logger;

    /// <summary>
    /// 初始化计价引擎。
    /// </summary>
    /// <param name="ruleMatchService">规则匹配服务。</param>
    /// <param name="pipeline">动作执行管线。</param>
    /// <param name="logger">日志对象。</param>
    public PricingEngine(
        RuleMatchService ruleMatchService,
        ActionExecutionPipeline pipeline,
        ILogger<PricingEngine> logger)
    {
        _ruleMatchService = ruleMatchService;
        _pipeline = pipeline;
        _logger = logger;
    }

    /// <summary>
    /// 执行一次计价计算。
    /// </summary>
    /// <param name="context">已经从接口 DTO 标准化出来的计价上下文。</param>
    /// <param name="batchContext">
    /// 批量计价共享上下文，可选。传入时引擎会在计算前从批量上下文注入同批已占用的限额数据，
    /// 计算完成后将本项目结果回写到批量上下文，确保后续项目能正确看到本项目的占用。
    /// </param>
    /// <returns>包含最终数量、金额、折价、命中规则、追溯步骤和占额草稿的计价结果。</returns>
    public async Task<PricingResult> CalculateAsync(PricingContext context, BatchPricingContext? batchContext = null)
    {
        // ========== 第零阶段：从批量上下文注入同批已占用数据 ==========
        // 如果传入了批量上下文，说明本项目是批量请求中的一个。前面的项目可能已经产生了
        // 限额占用，需要把这些占用注入到当前上下文，让限额执行器能正确判断"同批内已占用多少"。
        // 注入时机在引擎计算之前，确保动作链执行时能读到完整的同批累计数据。
        if (batchContext is not null)
        {
            InjectBatchContext(context, batchContext);
        }

        // ========== 第一阶段：初始化默认计价状态 ==========
        // 默认结果是"普通计价"：数量不变，金额为单价乘数量。
        // 后续规则动作会在这个基础上做换算、公式、封顶或限额截断。
        context.ConvertedQty = context.InputQty;
        context.FinalQty = context.InputQty;
        context.FinalAmount = context.UnitPrice * context.InputQty;
        context.FormulaAmount = 0;
        context.LimitedAmount = 0;
        context.DiscountAmount = 0;

        // ========== 第二阶段：匹配当前业务时间下可用的已发布规则 ==========
        // 规则匹配返回两部分：命中的规则头，以及已经按全局动作顺序排序的动作链。
        var (matchedRules, orderedActions) = await _ruleMatchService.MatchAsync(context);

        if (matchedRules.Count == 0)
        {
            // 未命中特殊规则时直接返回普通计价结果。应用服务会保存请求日志，
            // 但不会写折价明细或限额占用。
            return BuildResult(context, false);
        }

        // ========== 第三阶段：记录匹配结果并写追溯步骤 ==========
        // MATCH 步骤是追溯链的起点，便于后续解释"为什么这个项目被当作特殊项目处理"。
        context.MatchedRules = matchedRules;
        context.OrderedActions = orderedActions;
        context.TraceSteps.Add(new TraceStep
        {
            StepNo = 1,
            StepType = "MATCH",
            StepDesc = $"命中 {matchedRules.Count} 条规则",
            InputValue = context.InputQty,
            OutputValue = orderedActions.Count
        });

        // ========== 第四阶段：执行规则动作链 ==========
        // 动作链内部按"换算 -> 公式 -> 金额上下限 -> 数量/窗口限制 -> 折价"顺序执行。
        await _pipeline.ExecuteAsync(orderedActions, context);

        // ========== 第五阶段：计算折价金额并补齐占额草稿 ==========
        // 占额草稿在执行器中只记录维度，最终占用数量和金额要等动作链全部执行完成后才能确定。
        var originalAmount = context.UnitPrice * context.InputQty;
        context.FinalAmount = PricingAmountRounder.RoundFinal(context.FinalAmount);
        context.DiscountAmount = PricingAmountRounder.RoundFinal(originalAmount - context.FinalAmount);
        foreach (var occupy in context.PendingLimitOccupies)
        {
            occupy.OccupyQty = context.FinalQty;
            occupy.OccupyAmt = context.FinalAmount;
            occupy.Status = "PENDING";
            occupy.OccupiedAt = DateTime.Now;
        }

        // ========== 第六阶段：将本项目结果回写到批量上下文 ==========
        // 回写后，后续项目在进入引擎计算时能读到本项目的限额占用、同组互斥和同手术封顶数据。
        var result = BuildResult(context, true);

        if (batchContext is not null)
        {
            batchContext.AccumulateToBatch(result, context);
        }

        // ========== 第七阶段：输出计价完成日志 ==========
        // 日志记录关键金额，不记录完整请求，完整快照由请求日志保存。
        _logger.LogInformation(
            "计价完成 ItemCode={ItemCode}, 原金额={OriginalAmount}, 最终金额={FinalAmount}, 折扣={DiscountAmount}",
            context.ItemCode, originalAmount, context.FinalAmount, context.DiscountAmount);

        return result;
    }

    /// <summary>
    /// 将批量上下文中的同批已占用数据注入到当前项目的计价上下文。
    /// </summary>
    /// <param name="context">当前项目的计价上下文，注入后限额执行器可读到同批已占用数据。</param>
    /// <param name="batchContext">批量共享上下文，包含前面项目已经产生的限额占用和互斥计数。</param>
    /// <remarks>
    /// <para>
    /// 注入内容包括两部分：
    /// 1. InRequestOccupiedQtyByLimitDimension — 同批内按限额维度累计的数量，供限额执行器快速判断。
    /// 2. InRequestLimitOccupies — 同批内占额草稿列表，供时间窗执行器按 BusinessChargeTime 精确过滤。
    /// </para>
    /// <para>
    /// 注入方式为"追加而非覆盖"，因为 PricingContext 上可能已经有应用服务注入的
    /// InRequest 级别数据（同一请求内前序项目的累计），批量上下文的数据应叠加而非替换。
    /// </para>
    /// </remarks>
    private static void InjectBatchContext(PricingContext context, BatchPricingContext batchContext)
    {
        // ========== 阶段一：合并限额维度累计数量 ==========
        // 将批量上下文中前面项目已经占用的数量合并到上下文的 InRequest 字典。
        // 如果同一维度在两处都有值（理论上不应出现），取较大值以确保安全。
        var mergedQtyDict = new Dictionary<string, decimal>(
            context.InRequestOccupiedQtyByLimitDimension,
            StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in batchContext.InBatchOccupiedQtyByDimension)
        {
            mergedQtyDict.TryGetValue(kvp.Key, out var existing);
            if (kvp.Value > existing)
            {
                mergedQtyDict[kvp.Key] = kvp.Value;
            }
        }

        foreach (var kvp in batchContext.InBatchItemCountByGroup)
        {
            var mutexKey = $"MUTEX:{kvp.Key}".ToUpperInvariant();
            mergedQtyDict.TryGetValue(mutexKey, out var existing);
            if (kvp.Value > existing)
            {
                mergedQtyDict[mutexKey] = kvp.Value;
            }
        }

        foreach (var kvp in batchContext.InBatchOccupiedAmtByOperation)
        {
            var opCeilingKey = $"OP_CEILING:{kvp.Key}".ToUpperInvariant();
            mergedQtyDict.TryGetValue(opCeilingKey, out var existing);
            if (kvp.Value > existing)
            {
                mergedQtyDict[opCeilingKey] = kvp.Value;
            }
        }

        context.InRequestOccupiedQtyByLimitDimension = mergedQtyDict;

        // ========== 阶段二：合并占额草稿列表 ==========
        // 时间窗执行器需要完整的占额草稿来做时间过滤，不能只看累计数量。
        // 将批量上下文的占额草稿追加到上下文的 InRequest 列表。
        var mergedOccupies = new List<LimitOccupy>(context.InRequestLimitOccupies);
        foreach (var occupy in batchContext.InBatchLimitOccupies)
        {
            if (!mergedOccupies.Contains(occupy))
            {
                mergedOccupies.Add(occupy);
            }
        }
        context.InRequestLimitOccupies = mergedOccupies;
    }

    /// <summary>
    /// 根据计价上下文组装引擎输出结果。
    /// </summary>
    /// <param name="context">计价上下文。</param>
    /// <param name="isSpecial">是否命中特殊计价规则。</param>
    /// <returns>计价结果。</returns>
    private static PricingResult BuildResult(PricingContext context, bool isSpecial)
    {
        // 结果对象是对外响应和后续持久化的共同来源。这里过滤掉 0 占用草稿，
        // 避免没有实际收费数量/金额的规则仍然写入占额表。
        return new PricingResult
        {
            IsSpecialItem = isSpecial,
            InputQty = context.InputQty,
            ConvertedQty = context.ConvertedQty,
            FinalQty = context.FinalQty,
            UnitPrice = context.UnitPrice,
            FinalAmount = PricingAmountRounder.RoundFinal(context.FinalAmount),
            DiscountAmount = PricingAmountRounder.RoundFinal(context.DiscountAmount),
            ExceedQty = context.ExceedQty,
            ReplaceChildResult = context.ReplaceChildResult,
            ChildPricingResults = context.ChildPricingResults.ToList(),
            TraceSteps = context.TraceSteps.ToList(),
            MatchedRuleIds = context.MatchedRules.Select(r => r.RuleId).ToList(),
            LimitOccupies = context.PendingLimitOccupies
                .Where(o => o.OccupyQty != 0 || o.OccupyAmt != 0)
                .ToList()
        };
    }
}
