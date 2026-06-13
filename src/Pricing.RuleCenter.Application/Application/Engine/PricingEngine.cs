using Microsoft.Extensions.Logging;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Services;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Application.Engine;

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
    private readonly IReadOnlyList<ILimitOccupyValueFinalizer> _limitOccupyValueFinalizers;
    /// <summary>
    /// 引擎日志，用于记录计价完成的关键金额。
    /// </summary>
    private readonly ILogger<PricingEngine> _logger;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化计价引擎。
    /// </summary>
    /// <param name="ruleMatchService">规则匹配服务。</param>
    /// <param name="pipeline">动作执行管线。</param>
    /// <param name="limitOccupyValueFinalizers">限额占额草稿结算策略集合。</param>
    /// <param name="clock">技术时间提供者。</param>
    /// <param name="logger">日志对象。</param>
    public PricingEngine(
        RuleMatchService ruleMatchService,
        ActionExecutionPipeline pipeline,
        IEnumerable<ILimitOccupyValueFinalizer> limitOccupyValueFinalizers,
        IClock clock,
        ILogger<PricingEngine> logger)
    {
        _ruleMatchService = ruleMatchService;
        _pipeline = pipeline;
        _limitOccupyValueFinalizers = limitOccupyValueFinalizers.ToList();
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行一次计价计算。
    /// </summary>
    /// <param name="context">已经从接口 DTO 标准化出来的计价上下文。</param>
    /// <returns>包含最终数量、金额、折价、命中规则、追溯步骤和占额草稿的计价结果。</returns>
    public async Task<PricingResult> CalculateAsync(PricingContext context)
    {
        context.InitializeForCalculation();

        var (matchedRules, orderedActions) = await _ruleMatchService.MatchAsync(context);
        if (matchedRules.Count == 0)
        {
            return BuildResult(context, false);
        }

        ApplyMatchResult(context, matchedRules, orderedActions);
        await _pipeline.ExecuteAsync(orderedActions, context);

        var originalAmount = FinalizeResult(context);
        var result = BuildResult(context, true);
        _logger.LogInformation(
            "计价完成 项目编码={ItemCode}, 原金额={OriginalAmount}, 最终金额={FinalAmount}, 折扣金额={DiscountAmount}",
            context.ItemCode, originalAmount, context.FinalAmount, context.DiscountAmount);

        return result;
    }

    private static void ApplyMatchResult(
        PricingContext context,
        IReadOnlyList<RuleAggregate> matchedRules,
        IReadOnlyList<RuleAction> orderedActions)
    {
        context.MatchedRules = matchedRules;
        context.OrderedActions = orderedActions;
        var singleRule = matchedRules.Count == 1 ? matchedRules[0] : null;
        context.TraceSteps.Add(new TraceStep
        {
            StepNo = 1,
            StepType = "MATCH",
            StepDesc = BuildMatchStepDesc(matchedRules, orderedActions.Count),
            InputValue = context.InputQty,
            OutputValue = orderedActions.Count,
            RuntimeRuleId = singleRule?.RuleId,
            RuleCode = NormalizeString(singleRule?.RuleCode),
            RuleName = NormalizeString(singleRule?.RuleName),
            ValueType = "MATCH_RESULT",
            InputName = "输入数量",
            OutputName = "动作数量"
        });
    }

    private decimal FinalizeResult(PricingContext context)
    {
        var originalAmount = context.GetOriginalAmount();
        context.DiscountAmount = originalAmount - context.FinalAmount;

        foreach (var occupy in context.PendingLimitOccupies)
        {
            ApplyFinalOccupyValues(occupy, context);
            occupy.Status = "PENDING";
            occupy.OccupiedAt = _clock.Now;
        }

        return originalAmount;
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
            MatchedRuleInfos = context.MatchedRules.Select(ToRuleTraceInfo).ToList(),
            LimitOccupies = context.PendingLimitOccupies
                .Where(o => o.OccupyQty != 0 || o.OccupyAmt != 0)
                .ToList()
        };
    }

    private static PricingRuleTraceInfo ToRuleTraceInfo(RuleAggregate rule)
    {
        return new PricingRuleTraceInfo
        {
            RuleId = rule.RuleId,
            RuleCode = NormalizeString(rule.RuleCode),
            RuleName = NormalizeString(rule.RuleName)
        };
    }

    private static string BuildMatchStepDesc(
        IReadOnlyList<RuleAggregate> matchedRules,
        int actionCount)
    {
        var ruleNames = matchedRules
            .Select(rule => NormalizeString(rule.RuleName) ?? NormalizeString(rule.RuleCode))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        if (ruleNames.Count == 0)
        {
            return $"命中 {matchedRules.Count} 条规则，待执行 {actionCount} 个动作";
        }

        return $"命中规则：{string.Join("、", ruleNames)}；待执行 {actionCount} 个动作";
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }

    /// <summary>
    /// 按限额类型把最终计价结果物化到占额草稿。
    /// </summary>
    /// <remarks>
    /// 不同限额类型的“占用口径”并不相同：
    /// <list type="bullet">
    /// <item><description>DAY_QTY/TIME_WINDOW/ONCE_QTY 等数量限额，记录最终可收费数量</description></item>
    /// <item><description>SAME_OPERATION 封顶按金额累计，记录最终金额即可</description></item>
    /// <item><description>SAME_GROUP 互斥按“已收费项目个数”累计，单条通过即占 1 个名额</description></item>
    /// </list>
    /// 因此不能把所有占额都简单写成 FinalQty/FinalAmount。
    /// </remarks>
    private void ApplyFinalOccupyValues(LimitOccupy occupy, PricingContext context)
    {
        var matchedFinalizers = _limitOccupyValueFinalizers
            .Where(finalizer => finalizer.CanHandle(occupy))
            .ToArray();
        var selectedFinalizers = matchedFinalizers
            .Where(finalizer => !finalizer.IsFallback)
            .ToArray();

        if (selectedFinalizers.Length == 0)
        {
            selectedFinalizers = matchedFinalizers;
        }

        if (selectedFinalizers.Length != 1)
        {
            throw new InvalidOperationException(
                $"未找到唯一的限额占额结算器: LimitType={occupy.LimitType}, Count={selectedFinalizers.Length}");
        }

        selectedFinalizers[0].Apply(occupy, context);
    }
}
