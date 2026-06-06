using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

internal sealed record DiscountDetailSaveInput
{
    public long RequestId { get; init; }

    public string? TraceId { get; init; }

    public PricingCalculateRequest Request { get; init; } = null!;

    public PricingCalculateItemRequest Item { get; init; } = null!;

    public PricingResult Result { get; init; } = null!;

    public string Status { get; init; } = string.Empty;
}

internal sealed record ChildDiscountDetailSaveInput
{
    public long RequestId { get; init; }

    public string? TraceId { get; init; }

    public PricingCalculateRequest Request { get; init; } = null!;

    public PricingCalculateItemRequest Item { get; init; } = null!;

    public IReadOnlyList<ChildPricingResult> ChildPricingResults { get; init; } =
        Array.Empty<ChildPricingResult>();

    public string? ResultGroupNo { get; init; }

    public long MainDiscountId { get; init; }

    public long FirstRuleId { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime Now { get; init; }
}

/// <summary>
/// 计价折价明细写入器，负责主项目、替换子项和加收子项明细落库。
/// </summary>
public sealed class PricingDiscountDetailWriter
{
    private readonly IChargeDiscountDetailRepository _discountRepository;
    private readonly IClock _clock;

    /// <summary>
    /// 初始化计价折价明细写入器。
    /// </summary>
    /// <param name="discountRepository">折价明细仓储。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    public PricingDiscountDetailWriter(
        IChargeDiscountDetailRepository discountRepository,
        IClock clock)
    {
        _discountRepository = discountRepository;
        _clock = clock;
    }

    internal async Task SaveAsync(DiscountDetailSaveInput input)
    {
        var requestId = input.RequestId;
        var request = input.Request;
        var item = input.Item;
        var result = input.Result;
        var status = input.Status;
        var firstRuleId = result.MatchedRuleIds.FirstOrDefault();
        var now = _clock.Now;
        var resultGroupNo = PricingResultGroupNoGenerator.Resolve(requestId, item, result);
        var replacementAmt = result.ReplaceChildResult is null
            ? 0m
            : PricingAmountRounder.RoundFinal(result.ReplaceChildResult.Amount);
        var mainFinalAmt = result.ReplaceChildResult is null
            ? result.FinalAmount
            : Math.Max(result.FinalAmount - replacementAmt, 0m);
        var mainDiscountAmt = PricingAmountRounder.RoundFinal(
            item.UnitPrice * item.InputQty - mainFinalAmt);

        var detail = new ChargeDiscountDetail
        {
            RequestId = requestId,
            TraceId = input.TraceId,
            ChargeNo = NormalizeString(request.ChargeNo),
            ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
            PatientId = request.PatientId,
            VisitId = request.VisitId,
            ItemCode = item.ItemCode,
            ItemName = item.ItemName,
            RuleId = firstRuleId == 0 ? null : firstRuleId,
            ResultGroupNo = resultGroupNo,
            OriginalQty = item.InputQty,
            ConvertedQty = result.ConvertedQty,
            FinalQty = result.FinalQty,
            UnitPrice = result.UnitPrice,
            OriginalAmt = PricingAmountRounder.RoundFinal(item.UnitPrice * item.InputQty),
            CalculatedAmt = PricingAmountRounder.RoundFinal(mainFinalAmt),
            FinalAmt = PricingAmountRounder.RoundFinal(mainFinalAmt),
            DiscountAmt = mainDiscountAmt,
            DiscountType = result.ReplaceChildResult is null ? null : "EXCESS_REPLACE",
            ReasonCode = result.ReplaceChildResult is null ? null : "EXCESS_REPLACE",
            ReasonDesc = result.ReplaceChildResult is null ? null : PricingResponseBuilder.BuildReasonDesc(result),
            Status = status,
            OccurredAt = now
        };

        var mainDiscountId = await _discountRepository.InsertAsync(detail);

        if (result.ReplaceChildResult is null)
        {
            await SaveChildDiscountDetailsAsync(new ChildDiscountDetailSaveInput
            {
                RequestId = requestId,
                TraceId = input.TraceId,
                Request = request,
                Item = item,
                ChildPricingResults = result.ChildPricingResults,
                ResultGroupNo = resultGroupNo,
                MainDiscountId = mainDiscountId,
                FirstRuleId = firstRuleId,
                Status = status,
                Now = now
            });
            return;
        }

        var replacement = result.ReplaceChildResult;
        var replacementDetail = new ChargeDiscountDetail
        {
            RequestId = requestId,
            TraceId = input.TraceId,
            ChargeNo = NormalizeString(request.ChargeNo),
            ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
            PatientId = request.PatientId,
            VisitId = request.VisitId,
            ItemCode = replacement.ItemCode,
            ItemName = replacement.ItemName,
            RuleId = firstRuleId == 0 ? null : firstRuleId,
            ResultGroupNo = resultGroupNo,
            ParentDiscountId = mainDiscountId,
            ConvertedQty = replacement.Qty,
            FinalQty = replacement.Qty,
            UnitPrice = replacement.UnitPrice,
            OriginalAmt = 0m,
            CalculatedAmt = replacementAmt,
            FinalAmt = replacementAmt,
            DiscountAmt = -replacementAmt,
            DiscountType = "EXCESS_REPLACE",
            ReasonCode = "EXCESS_REPLACE",
            ReasonDesc = PricingResponseBuilder.BuildReplacementReasonDesc(item, replacement),
            Status = status,
            OccurredAt = now
        };

        await _discountRepository.InsertAsync(replacementDetail);

        await SaveChildDiscountDetailsAsync(new ChildDiscountDetailSaveInput
        {
            RequestId = requestId,
            TraceId = input.TraceId,
            Request = request,
            Item = item,
            ChildPricingResults = result.ChildPricingResults,
            ResultGroupNo = resultGroupNo,
            MainDiscountId = mainDiscountId,
            FirstRuleId = firstRuleId,
            Status = status,
            Now = now
        });
    }

    private async Task SaveChildDiscountDetailsAsync(ChildDiscountDetailSaveInput input)
    {
        var request = input.Request;
        var item = input.Item;
        foreach (var child in input.ChildPricingResults)
        {
            var childAmount = PricingAmountRounder.RoundFinal(child.Amount);
            var childDetail = new ChargeDiscountDetail
            {
                RequestId = input.RequestId,
                TraceId = input.TraceId,
                ChargeNo = NormalizeString(request.ChargeNo),
                ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
                PatientId = request.PatientId,
                VisitId = request.VisitId,
                ItemCode = child.ItemCode,
                ItemName = child.ItemName,
                RuleId = input.FirstRuleId == 0 ? null : input.FirstRuleId,
                ResultGroupNo = input.ResultGroupNo,
                ParentDiscountId = input.MainDiscountId,
                ConvertedQty = child.Qty,
                FinalQty = child.Qty,
                UnitPrice = child.UnitPrice,
                OriginalAmt = 0m,
                CalculatedAmt = childAmount,
                FinalAmt = childAmount,
                DiscountAmt = -childAmount,
                DiscountType = "ADD_CHILD_ITEM",
                ReasonCode = "ADD_CHILD_ITEM",
                ReasonDesc = PricingResponseBuilder.BuildChildReasonDesc(item, child),
                Status = input.Status,
                OccurredAt = input.Now
            };

            await _discountRepository.InsertAsync(childDetail);
        }
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
