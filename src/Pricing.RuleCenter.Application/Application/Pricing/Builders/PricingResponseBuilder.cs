using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing.Builders;

internal sealed record ItemPricingCalculation(
    PricingCalculateItemRequest Item,
    PricingResult Result);

internal static class PricingResponseBuilder
{
    public static PricingCalculateResponse Build(
        long requestId,
        string? traceId,
        IReadOnlyList<ItemPricingCalculation> calculations,
        DateTime now,
        RuntimePackageTraceResolution? runtimeTrace = null,
        DateTime? expireAt = null)
    {
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

    public static string BuildReasonDesc(PricingResult result)
    {
        if (result.ReplaceChildResult is null)
        {
            return $"超出限额数量 {result.ExceedQty}，超出部分归零";
        }

        return $"超出限额数量 {result.ExceedQty}，替换为 {result.ReplaceChildResult.ItemCode} " +
               $"{result.ReplaceChildResult.ItemName}，数量 {result.ReplaceChildResult.Qty}，金额 {result.ReplaceChildResult.Amount}";
    }

    public static string BuildReplacementReasonDesc(
        PricingCalculateItemRequest item,
        ReplaceChildResult replacement)
    {
        return $"主项目 {item.ItemCode} 超限后替换为 {replacement.ItemCode} " +
               $"{replacement.ItemName}，数量 {replacement.Qty}，金额 {replacement.Amount}";
    }

    public static string BuildChildReasonDesc(
        PricingCalculateItemRequest item,
        ChildPricingResult child)
    {
        return $"主项目 {item.ItemCode} 自动加收子项目 {child.ItemCode} " +
               $"{child.ItemName}，数量 {child.Qty}，金额 {child.Amount}";
    }

    private static PricingCalculateItemResponse BuildItemResponse(
        long requestId,
        string? traceId,
        PricingCalculateItemRequest item,
        PricingResult result,
        RuntimePackageTraceResolution? runtimeTrace)
    {
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
            FinalAmount = PricingAmountRounder.RoundFinal(
                result.FinalAmount + result.ChildPricingResults.Sum(c => c.Amount)),
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
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
