using System.Globalization;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing.Builders;

/// <summary>
/// 单条费用明细与核心引擎结果的配对对象。
/// </summary>
/// <param name="Item">原始费用明细请求，用于保留 HIS 行号、收费明细号和展示名称。</param>
/// <param name="Result">核心引擎输出的计价结果，用于构造响应和持久化折价明细。</param>
/// <remarks>
/// 应用层 workflow 以“多条费用明细逐条计算、最终统一汇总”的方式工作，该记录把每条明细的输入和输出绑定起来，
/// 避免响应构建、步骤日志、折价明细写入时只拿到结果而丢失原请求行信息。
/// </remarks>
internal sealed record ItemPricingCalculation(
    PricingCalculateItemRequest Item,
    PricingResult Result);

/// <summary>
/// 计价响应构建器，负责把每条费用的引擎结果转换成对外 HTTP 响应 DTO。
/// </summary>
/// <remarks>
/// <para>
/// 该构建器是“核心引擎结果”和“接口契约”之间的边界。核心引擎只关心 FinalQty、FinalAmount、
/// TraceSteps 和占额草稿；接口响应还需要按 HIS 行关联返回 ItemRequestNo/ChargeDetailNo、
/// 替换子项、加收子项和根层汇总金额。
/// </para>
/// <para>
/// 根层字段只用于兼容单明细场景和汇总展示。多明细落账必须优先使用 <c>Items</c> 集合，
/// 因为项目编码、收费明细号、partSeq、子项和替换项都在明细层表达。
/// </para>
/// </remarks>
internal static class PricingResponseBuilder
{
    /// <summary>
    /// 构建完整计价响应。
    /// </summary>
    /// <param name="requestId">请求日志主键，confirm 后续 commit/cancel/reverse 必须引用。</param>
    /// <param name="traceId">追溯流水号，用于串联请求日志、步骤日志和折价明细。</param>
    /// <param name="calculations">本次请求中所有费用明细的计算结果。</param>
    /// <param name="now">当前技术时间，用于计算 confirm 剩余有效秒数。</param>
    /// <param name="expireAt">confirm 结果过期时间；simulate 为空。</param>
    /// <returns>对外计价响应 DTO。</returns>
    public static PricingCalculateResponse Build(
        long requestId,
        string? traceId,
        IReadOnlyList<ItemPricingCalculation> calculations,
        DateTime now,
        DateTime? expireAt = null)
    {
        // 先构造明细层响应，再从明细层汇总根层金额和命中规则。
        // 这样单明细和多明细的汇总口径一致，避免根层和 Items 层金额不闭合。
        var itemResponses = calculations
            .Select(c => BuildItemResponse(requestId, traceId, c.Item, c.Result))
            .ToList();
        var first = itemResponses.FirstOrDefault();
        var expireSeconds = expireAt.HasValue
            ? Math.Max(0, (int)Math.Ceiling((expireAt.Value - now).TotalSeconds))
            : (int?)null;
        var finalAmount = itemResponses.Sum(i => i.FinalAmount);
        var discountAmount = itemResponses.Sum(i => i.DiscountAmount);

        return new PricingCalculateResponse
        {
            RequestId = requestId,
            TraceId = NormalizeString(traceId),
            NextAction = expireAt.HasValue
                ? PricingNextActionCodes.CommitOrCancel
                : PricingNextActionCodes.ConfirmBeforeCharge,
            BusinessStatus = expireAt.HasValue
                ? BusinessStatusCodes.ConfirmPending
                : BusinessStatusCodes.Simulated,
            RuleSnapshotTime = now,
            Items = itemResponses,
            IsSpecialItem = itemResponses.Any(i => i.IsSpecialItem),
            InputQty = itemResponses.Sum(i => i.InputQty),
            FinalQty = itemResponses.Sum(i => i.FinalQty),
            UnitPrice = itemResponses.Count == 1 ? first?.UnitPrice ?? 0 : 0,
            TotalOriginalAmount = itemResponses.Sum(i => i.OriginalAmount),
            TotalFinalAmount = finalAmount,
            TotalDiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            DiscountAmount = discountAmount,
            ExpireAt = expireAt,
            ExpireSeconds = expireSeconds,
            TraceSteps = null,
            MatchedRuleIds = itemResponses.SelectMany(i => i.MatchedRuleIds).Distinct().ToList()
        };
    }

    /// <summary>
    /// 构建主项目超限后替换子项的原因说明。
    /// </summary>
    /// <param name="result">计价结果。</param>
    /// <returns>用于折价明细和追溯展示的原因文本。</returns>
    public static string BuildReasonDesc(PricingResult result)
    {
        if (result.ReplaceChildResult is null)
        {
            return $"超出限额数量 {result.ExceedQty}，超出部分归零";
        }

        return $"超出限额数量 {result.ExceedQty}，替换为 {result.ReplaceChildResult.ItemCode} " +
               $"{result.ReplaceChildResult.ItemName}，数量 {result.ReplaceChildResult.Qty}，金额 {result.ReplaceChildResult.Amount}";
    }

    /// <summary>
    /// 构建替换子项的原因说明。
    /// </summary>
    /// <param name="item">原主项目请求明细。</param>
    /// <param name="replacement">替换子项结果。</param>
    /// <returns>用于替换子项折价明细的原因文本。</returns>
    public static string BuildReplacementReasonDesc(
        PricingCalculateItemRequest item,
        ReplaceChildResult replacement)
    {
        return $"主项目 {item.ItemCode} 超限后替换为 {replacement.ItemCode} " +
               $"{replacement.ItemName}，数量 {replacement.Qty}，金额 {replacement.Amount}";
    }

    /// <summary>
    /// 构建自动加收子项的原因说明。
    /// </summary>
    /// <param name="item">原主项目请求明细。</param>
    /// <param name="child">加收子项结果。</param>
    /// <returns>用于子项折价明细的原因文本。</returns>
    public static string BuildChildReasonDesc(
        PricingCalculateItemRequest item,
        ChildPricingResult child)
    {
        return $"主项目 {item.ItemCode} 自动加收子项目 {child.ItemCode} " +
               $"{child.ItemName}，数量 {child.Qty}，金额 {child.Amount}";
    }

    /// <summary>
    /// 构建单条费用明细响应。
    /// </summary>
    /// <param name="requestId">请求日志主键。</param>
    /// <param name="traceId">追溯流水号。</param>
    /// <param name="item">原费用明细请求。</param>
    /// <param name="result">核心引擎结果。</param>
    /// <returns>单条费用明细响应。</returns>
    private static PricingCalculateItemResponse BuildItemResponse(
        long requestId,
        string? traceId,
        PricingCalculateItemRequest item,
        PricingResult result)
    {
        var matchedRuleIds = result.MatchedRuleIds
            .Where(ruleId => ruleId > 0)
            .Distinct()
            .ToList();

        return new PricingCalculateItemResponse
        {
            ItemRequestNo = NormalizeString(item.ItemRequestNo),
            ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
            TraceId = NormalizeString(traceId),
            RequestId = requestId,
            ItemCode = item.ItemCode.Trim(),
            ItemName = NormalizeString(item.ItemName),
            IsSpecialItem = result.IsSpecialItem,
            InputQty = result.InputQty,
            FinalQty = result.FinalQty,
            ConvertedQty = result.ConvertedQty,
            UnitPrice = result.UnitPrice,
            OriginalAmount = PricingAmountRounder.RoundFinal(item.UnitPrice * item.InputQty),
            // FinalAmount 包含主项目正常金额 + 子项加收金额；替换子项金额已在 result.FinalAmount 中体现。
            FinalAmount = PricingAmountRounder.RoundFinal(
                result.FinalAmount + result.ChildPricingResults.Sum(c => c.Amount)),
            // DiscountAmount 表示原金额与最终应收之间的差额。子项加收是“负折扣”，因此要从折扣金额中扣除。
            DiscountAmount = PricingAmountRounder.RoundFinal(
                result.DiscountAmount - result.ChildPricingResults.Sum(c => c.Amount)),
            ExceedQty = result.ExceedQty,
            ReplacementItem = result.ReplaceChildResult is null
                ? null
                : new PricingReplacementItemResponse
                {
                    ItemCode = result.ReplaceChildResult.ItemCode,
                    ItemName = NormalizeString(result.ReplaceChildResult.ItemName),
                    Qty = result.ReplaceChildResult.Qty,
                    UnitPrice = result.ReplaceChildResult.UnitPrice,
                    Amount = PricingAmountRounder.RoundFinal(result.ReplaceChildResult.Amount)
                },
            ChildItems = result.ChildPricingResults.Select(c => new PricingChildItemResponse
            {
                ItemCode = c.ItemCode,
                ItemName = NormalizeString(c.ItemName),
                Qty = c.Qty,
                UnitPrice = c.UnitPrice,
                Amount = PricingAmountRounder.RoundFinal(c.Amount),
                ShareParentLimit = c.ShareParentLimit
            }).ToList(),
            TraceSteps = BuildTraceSteps(result),
            MatchedRuleIds = matchedRuleIds
        };
    }

    private static IReadOnlyList<PricingTraceStepResponse> BuildTraceSteps(PricingResult result)
    {
        var ruleLookup = result.MatchedRuleInfos
            .Where(rule => rule.RuleId > 0)
            .GroupBy(rule => rule.RuleId)
            .ToDictionary(group => group.Key, group => group.First());

        return result.TraceSteps
            .Select(step => BuildTraceStep(step, ruleLookup))
            .ToList();
    }

    private static PricingTraceStepResponse BuildTraceStep(
        TraceStep step,
        IReadOnlyDictionary<long, PricingRuleTraceInfo> ruleLookup)
    {
        var ruleInfo = ResolveRuleInfo(step, ruleLookup);
        var actionCode = NormalizeString(step.ActionCode);
        var actionName = ResolveActionName(actionCode);
        var valueInfo = ResolveValueInfo(step);
        var nodeTitle = ResolveNodeTitle(step.StepType);
        var nodeDesc = BuildNodeDesc(step, ruleInfo, actionName, valueInfo);

        return new PricingTraceStepResponse
        {
            NodeKey = BuildNodeKey(step, ruleInfo.RuleId, actionCode),
            NodeTitle = nodeTitle,
            NodeDesc = nodeDesc,
            StepNo = step.StepNo,
            StepType = step.StepType,
            StepDesc = nodeDesc,
            InputValue = step.InputValue,
            OutputValue = step.OutputValue,
            RuleId = step.RuntimeRuleId,
            RuleCode = ruleInfo.RuleCode,
            RuleName = ruleInfo.RuleName,
            ActionCode = actionCode,
            ActionName = actionName,
            ExecutorCode = NormalizeString(step.ExecutorCode),
            ValueType = valueInfo.ValueType,
            ValueUnit = valueInfo.ValueUnit,
            InputName = valueInfo.InputName,
            OutputName = valueInfo.OutputName
        };
    }

    private static TraceRuleInfo ResolveRuleInfo(
        TraceStep step,
        IReadOnlyDictionary<long, PricingRuleTraceInfo> ruleLookup)
    {
        PricingRuleTraceInfo? matchedRule = null;
        if (step.RuntimeRuleId.HasValue &&
            ruleLookup.TryGetValue(step.RuntimeRuleId.Value, out var lookupRule))
        {
            matchedRule = lookupRule;
        }

        return new TraceRuleInfo(
            step.RuntimeRuleId,
            NormalizeString(step.RuleCode) ?? NormalizeString(matchedRule?.RuleCode),
            NormalizeString(step.RuleName) ?? NormalizeString(matchedRule?.RuleName));
    }

    private static TraceValueInfo ResolveValueInfo(TraceStep step)
    {
        var valueType = NormalizeString(step.ValueType);
        var valueUnit = NormalizeString(step.ValueUnit);
        var inputName = NormalizeString(step.InputName);
        var outputName = NormalizeString(step.OutputName);

        if (valueType is not null || inputName is not null || outputName is not null)
        {
            return new TraceValueInfo(valueType, valueUnit, inputName, outputName);
        }

        return NormalizeString(step.StepType)?.ToUpperInvariant() switch
        {
            "MATCH" => new TraceValueInfo("MATCH_RESULT", null, "输入数量", "动作数量"),
            "CONVERT" => new TraceValueInfo("QTY", null, "换算前数量", "换算后数量"),
            "FORMULA" => new TraceValueInfo("AMOUNT", "元", "公式输入金额", "公式输出金额"),
            "LIMIT" => new TraceValueInfo("AMOUNT", "元", "处理前金额", "处理后金额"),
            "DISCOUNT" => new TraceValueInfo("AMOUNT", "元", "折价前金额", "折价后金额"),
            _ => new TraceValueInfo(null, null, null, null)
        };
    }

    private static string ResolveNodeTitle(string? stepType)
    {
        return NormalizeString(stepType)?.ToUpperInvariant() switch
        {
            "MATCH" => "规则匹配",
            "CONVERT" => "双单位换算",
            "FORMULA" => "公式计算",
            "LIMIT" => "限额处理",
            "DISCOUNT" => "折价处理",
            "VALIDATE" => "校验处理",
            "ERROR" => "异常处理",
            _ => "计价步骤"
        };
    }

    private static string? ResolveActionName(string? actionCode)
    {
        return NormalizeString(actionCode)?.ToUpperInvariant() switch
        {
            "CONVERT_QTY" => "双单位换算",
            "FORMULA_CALC" => "公式计价",
            "APPLY_MIN_AMOUNT" => "金额下限",
            "APPLY_MAX_AMOUNT" => "金额上限",
            "APPLY_DAY_LIMIT_QTY" => "日数量限额",
            "APPLY_TIME_WINDOW_LIMIT" => "时间窗口限额",
            "APPLY_ONCE_LIMIT_QTY" => "单次数量限额",
            "SAME_GROUP_MUTEX" => "同组互斥",
            "SAME_OPERATION_CEILING" => "同手术封顶",
            "DISCOUNT_EXCEED_TO_ZERO" => "超出部分归零",
            "ADD_CHILD_ITEM" => "子项加收",
            null => null,
            _ => actionCode
        };
    }

    private static string BuildNodeDesc(
        TraceStep step,
        TraceRuleInfo ruleInfo,
        string? actionName,
        TraceValueInfo valueInfo)
    {
        var parts = new List<string>();
        var stepType = NormalizeString(step.StepType)?.ToUpperInvariant();
        var ruleDisplay = ruleInfo.RuleName ?? ruleInfo.RuleCode;

        if (stepType == "MATCH")
        {
            parts.Add(ruleDisplay is null ? "完成规则匹配" : $"命中规则：{ruleDisplay}");
        }
        else
        {
            if (ruleDisplay is not null)
            {
                parts.Add($"规则：{ruleDisplay}");
            }

            if (actionName is not null)
            {
                parts.Add($"动作：{actionName}");
            }
        }

        var originalDesc = NormalizeString(step.StepDesc);
        if (originalDesc is not null && !parts.Contains(originalDesc, StringComparer.Ordinal))
        {
            parts.Add(originalDesc);
        }

        var valueDesc = BuildValueDesc(step, valueInfo);
        if (valueDesc is not null)
        {
            parts.Add(valueDesc);
        }

        return string.Join("；", parts);
    }

    private static string? BuildValueDesc(TraceStep step, TraceValueInfo valueInfo)
    {
        if (!step.InputValue.HasValue && !step.OutputValue.HasValue)
        {
            return null;
        }

        var inputName = valueInfo.InputName ?? "输入值";
        var outputName = valueInfo.OutputName ?? "输出值";
        var unit = valueInfo.ValueUnit;
        var input = step.InputValue.HasValue
            ? $"{inputName} {FormatDecimal(step.InputValue.Value)}{unit}"
            : null;
        var output = step.OutputValue.HasValue
            ? $"{outputName} {FormatDecimal(step.OutputValue.Value)}{unit}"
            : null;

        return (input, output) switch
        {
            (not null, not null) => $"{input}，{output}",
            (not null, null) => input,
            (null, not null) => output,
            _ => null
        };
    }

    private static string BuildNodeKey(
        TraceStep step,
        long? ruleId,
        string? actionCode)
    {
        var stepType = NormalizeString(step.StepType)?.ToUpperInvariant() ?? "STEP";
        var rulePart = ruleId.HasValue && ruleId.Value > 0
            ? ruleId.Value.ToString(CultureInfo.InvariantCulture)
            : "NONE";
        var actionPart = NormalizeString(actionCode)?.ToUpperInvariant() ?? "NONE";
        return $"{stepType}:{rulePart}:{actionPart}:{step.StepNo.ToString(CultureInfo.InvariantCulture)}";
    }

    private static string FormatDecimal(decimal value)
    {
        return value.ToString("0.####", CultureInfo.InvariantCulture);
    }

    private static string? NormalizeString(string? value)
    {
        // 响应里统一把空白字符串转为 null，避免调用方把空串误当成有效业务编码。
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }

    private sealed record TraceRuleInfo(
        long? RuleId,
        string? RuleCode,
        string? RuleName);

    private sealed record TraceValueInfo(
        string? ValueType,
        string? ValueUnit,
        string? InputName,
        string? OutputName);
}
