using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.RuntimePackages;
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
/// 替换子项、加收子项、运行包追溯 ID 和根层汇总金额。
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
    /// <param name="runtimeTrace">运行包追溯解析结果，用于把运行时规则映射回策略版本和模板版本。</param>
    /// <param name="expireAt">confirm 结果过期时间；simulate 为空。</param>
    /// <returns>对外计价响应 DTO。</returns>
    public static PricingCalculateResponse Build(
        long requestId,
        string? traceId,
        IReadOnlyList<ItemPricingCalculation> calculations,
        DateTime now,
        RuntimePackageTraceResolution? runtimeTrace = null,
        DateTime? expireAt = null)
    {
        // 先构造明细层响应，再从明细层汇总根层金额和命中规则。
        // 这样单明细和多明细的汇总口径一致，避免根层和 Items 层金额不闭合。
        var itemResponses = calculations
            .Select(c => BuildItemResponse(requestId, traceId, c.Item, c.Result, runtimeTrace))
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
            RuntimePackageId = runtimeTrace?.RuntimePackageId,
            RuntimePackageVersion = runtimeTrace?.RuntimePackageVersion,
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
            TraceSteps = itemResponses.Count == 1 ? first?.TraceSteps ?? Array.Empty<PricingTraceStepResponse>() : Array.Empty<PricingTraceStepResponse>(),
            MatchedRuleIds = itemResponses.SelectMany(i => i.MatchedRuleIds).Distinct().ToList(),
            MatchedRuntimeRuleIds = itemResponses.SelectMany(i => i.MatchedRuntimeRuleIds).Distinct().ToList(),
            MatchedPolicyVersionIds = itemResponses.SelectMany(i => i.MatchedPolicyVersionIds).Distinct().ToList(),
            MatchedTemplateVersionIds = itemResponses.SelectMany(i => i.MatchedTemplateVersionIds).Distinct().ToList()
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
    /// <param name="runtimeTrace">运行包追溯解析结果。</param>
    /// <returns>单条费用明细响应。</returns>
    private static PricingCalculateItemResponse BuildItemResponse(
        long requestId,
        string? traceId,
        PricingCalculateItemRequest item,
        PricingResult result,
        RuntimePackageTraceResolution? runtimeTrace)
    {
        // 运行包启用时，核心引擎中的 RuleId 实际是运行时规则 ID。
        // 这里同时返回运行时规则、来源策略版本和模板版本，保证追溯页面能从收费结果反查配置来源。
        var matchedRuleIds = result.MatchedRuleIds
            .Where(ruleId => ruleId > 0)
            .Distinct()
            .ToList();
        var matchedRuntimeRuleIds = runtimeTrace is not null && runtimeTrace.RuntimePackageId.HasValue
            ? matchedRuleIds
            : new List<long>();
        var matchedPolicyVersionIds = matchedRuleIds
            .Select(ruleId => runtimeTrace?.FindRule(ruleId)?.SourcePolicyVersionId)
            .Where(id => id.HasValue && id.Value > 0)
            .Select(id => id.GetValueOrDefault())
            .Distinct()
            .ToList();
        var matchedTemplateVersionIds = matchedRuleIds
            .Select(ruleId => runtimeTrace?.FindRule(ruleId)?.SourceTemplateVersionId)
            .Where(id => id.HasValue && id.Value > 0)
            .Select(id => id.GetValueOrDefault())
            .Distinct()
            .ToList();

        return new PricingCalculateItemResponse
        {
            ItemRequestNo = NormalizeString(item.ItemRequestNo),
            ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
            TraceId = NormalizeString(traceId),
            RequestId = requestId,
            RuntimePackageId = runtimeTrace?.RuntimePackageId,
            RuntimePackageVersion = runtimeTrace?.RuntimePackageVersion,
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
            TraceSteps = result.TraceSteps.Select(s => new PricingTraceStepResponse
            {
                StepNo = s.StepNo,
                StepType = s.StepType,
                StepDesc = s.StepDesc,
                InputValue = s.InputValue,
                OutputValue = s.OutputValue,
                RuntimeRuleId = s.RuntimeRuleId,
                SourcePolicyVersionId = runtimeTrace?.FindRule(s.RuntimeRuleId)?.SourcePolicyVersionId,
                SourceTemplateVersionId = runtimeTrace?.FindRule(s.RuntimeRuleId)?.SourceTemplateVersionId
            }).ToList(),
            MatchedRuleIds = matchedRuleIds,
            MatchedRuntimeRuleIds = matchedRuntimeRuleIds,
            MatchedPolicyVersionIds = matchedPolicyVersionIds,
            MatchedTemplateVersionIds = matchedTemplateVersionIds
        };
    }

    private static string? NormalizeString(string? value)
    {
        // 响应里统一把空白字符串转为 null，避免调用方把空串误当成有效业务编码。
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
