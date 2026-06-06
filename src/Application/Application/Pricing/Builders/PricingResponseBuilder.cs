using Pricing.RuleCenter.Application.Dto;
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
        IReadOnlyList<ItemPricingCalculation> calculations,
        DateTime now,
        DateTime? expireAt = null)
    {
        var itemResponses = calculations
            .Select(c => BuildItemResponse(requestId, c.Item, c.Result))
            .ToList();
        var first = itemResponses.FirstOrDefault();
        var expireSeconds = expireAt.HasValue
            ? Math.Max(0, (int)Math.Ceiling((expireAt.Value - now).TotalSeconds))
            : (int?)null;

        return new PricingCalculateResponse
        {
            RequestId = requestId,
            Items = itemResponses,
            IsSpecialItem = itemResponses.Any(i => i.IsSpecialItem),
            InputQty = itemResponses.Sum(i => i.InputQty),
            FinalQty = itemResponses.Sum(i => i.FinalQty),
            UnitPrice = itemResponses.Count == 1 ? first?.UnitPrice ?? 0 : 0,
            FinalAmount = itemResponses.Sum(i => i.FinalAmount),
            DiscountAmount = itemResponses.Sum(i => i.DiscountAmount),
            ExpireAt = expireAt,
            ExpireSeconds = expireSeconds,
            TraceSteps = itemResponses.Count == 1 ? first?.TraceSteps ?? Array.Empty<PricingTraceStepResponse>() : Array.Empty<PricingTraceStepResponse>(),
            MatchedRuleIds = itemResponses.SelectMany(i => i.MatchedRuleIds).Distinct().ToList()
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
        PricingCalculateItemRequest item,
        PricingResult result)
    {
        return new PricingCalculateItemResponse
        {
            ItemRequestNo = NormalizeString(item.ItemRequestNo),
            ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
            RequestId = requestId,
            ItemCode = item.ItemCode.Trim(),
            ItemName = NormalizeString(item.ItemName),
            IsSpecialItem = result.IsSpecialItem,
            InputQty = result.InputQty,
            FinalQty = result.FinalQty,
            ConvertedQty = result.ConvertedQty,
            UnitPrice = result.UnitPrice,
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
                OutputValue = s.OutputValue
            }).ToList(),
            MatchedRuleIds = result.MatchedRuleIds
        };
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
