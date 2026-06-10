using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

/// <summary>
/// 保存主项目折价明细所需输入。
/// </summary>
/// <remarks>
/// 折价明细保存 confirm 结果中的可落账事实，commit/cancel/reverse 会同步推进这些明细状态。
/// </remarks>
internal sealed record DiscountDetailSaveInput
{
    /// <summary>请求日志主键。</summary>
    public long RequestId { get; init; }

    /// <summary>追溯流水号。</summary>
    public string? TraceId { get; init; }

    /// <summary>原始计价请求。</summary>
    public PricingCalculateRequest Request { get; init; } = null!;

    /// <summary>当前费用明细请求。</summary>
    public PricingCalculateItemRequest Item { get; init; } = null!;

    /// <summary>当前费用明细的计价结果。</summary>
    public PricingResult Result { get; init; } = null!;

    /// <summary>明细状态，confirm 阶段通常为 PENDING。</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>运行包追溯解析结果。</summary>
    public RuntimePackageTraceResolution? RuntimeTrace { get; init; }
}

/// <summary>
/// 保存替换子项或加收子项折价明细所需输入。
/// </summary>
internal sealed record ChildDiscountDetailSaveInput
{
    /// <summary>请求日志主键。</summary>
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

    public RuntimePackageTraceResolution? RuntimeTrace { get; init; }
}

internal sealed record MainDiscountDetailContext
{
    public long FirstRuleId { get; init; }

    public DateTime OccurredAt { get; init; }

    public string? ResultGroupNo { get; init; }

    public decimal ReplacementAmount { get; init; }

    public decimal MainFinalAmount { get; init; }

    public decimal MainDiscountAmount { get; init; }

    public RuntimeRule? FirstRuntimeRule { get; init; }
}

/// <summary>
/// 计价折价明细写入器，负责主项目、替换子项和加收子项明细落库。
/// </summary>
public sealed class PricingDiscountDetailWriter
{
    /// <summary>
    /// 折价明细仓储。
    /// </summary>
    private readonly IChargeDiscountDetailRepository _discountRepository;
    /// <summary>
    /// 统一时钟。
    /// </summary>
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
        var context = BuildMainContext(input);
        var mainDetail = BuildMainDiscountDetail(input, context);
        var mainDiscountId = await _discountRepository.InsertAsync(mainDetail);

        await SaveReplacementDiscountDetailAsync(input, context, mainDiscountId);
        await SaveChildDiscountDetailsAsync(BuildChildDiscountInput(input, context, mainDiscountId));
    }

    private MainDiscountDetailContext BuildMainContext(DiscountDetailSaveInput input)
    {
        var firstRuleId = input.Result.MatchedRuleIds.FirstOrDefault();
        var replacementAmount = input.Result.ReplaceChildResult is null
            ? 0m
            : PricingAmountRounder.RoundFinal(input.Result.ReplaceChildResult.Amount);
        var mainFinalAmount = input.Result.ReplaceChildResult is null
            ? input.Result.FinalAmount
            : Math.Max(input.Result.FinalAmount - replacementAmount, 0m);

        return new MainDiscountDetailContext
        {
            FirstRuleId = firstRuleId,
            OccurredAt = _clock.Now,
            ResultGroupNo = PricingResultGroupNoGenerator.Resolve(input.RequestId, input.Item, input.Result),
            ReplacementAmount = replacementAmount,
            MainFinalAmount = mainFinalAmount,
            MainDiscountAmount = PricingAmountRounder.RoundFinal(
                input.Item.UnitPrice * input.Item.InputQty - mainFinalAmount),
            FirstRuntimeRule = input.RuntimeTrace?.FindRule(firstRuleId == 0 ? null : firstRuleId)
        };
    }

    private ChargeDiscountDetail BuildMainDiscountDetail(
        DiscountDetailSaveInput input,
        MainDiscountDetailContext context)
    {
        long? firstRuleId = context.FirstRuleId == 0 ? null : context.FirstRuleId;
        var hasReplacement = input.Result.ReplaceChildResult is not null;

        return new ChargeDiscountDetail
        {
            RequestId = input.RequestId,
            TraceId = input.TraceId,
            ChargeNo = NormalizeString(input.Request.ChargeNo),
            ChargeDetailNo = NormalizeString(input.Item.ChargeDetailNo),
            PatientId = input.Request.PatientId,
            VisitId = input.Request.VisitId,
            ItemCode = input.Item.ItemCode,
            ItemName = input.Item.ItemName,
            RuleId = firstRuleId,
            RuntimePackageId = input.RuntimeTrace?.RuntimePackageId,
            RuntimeRuleId = firstRuleId,
            SourcePolicyVersionId = context.FirstRuntimeRule?.SourcePolicyVersionId,
            SourceTemplateVersionId = context.FirstRuntimeRule?.SourceTemplateVersionId,
            ResultGroupNo = context.ResultGroupNo,
            OriginalQty = input.Item.InputQty,
            ConvertedQty = input.Result.ConvertedQty,
            FinalQty = input.Result.FinalQty,
            UnitPrice = input.Result.UnitPrice,
            OriginalAmt = PricingAmountRounder.RoundFinal(input.Item.UnitPrice * input.Item.InputQty),
            CalculatedAmt = PricingAmountRounder.RoundFinal(context.MainFinalAmount),
            FinalAmt = PricingAmountRounder.RoundFinal(context.MainFinalAmount),
            DiscountAmt = context.MainDiscountAmount,
            DiscountType = hasReplacement ? "EXCESS_REPLACE" : null,
            ReasonCode = hasReplacement ? "EXCESS_REPLACE" : null,
            ReasonDesc = hasReplacement ? PricingResponseBuilder.BuildReasonDesc(input.Result) : null,
            Status = input.Status,
            OccurredAt = context.OccurredAt
        };
    }

    private async Task SaveReplacementDiscountDetailAsync(
        DiscountDetailSaveInput input,
        MainDiscountDetailContext context,
        long mainDiscountId)
    {
        var replacement = input.Result.ReplaceChildResult;
        if (replacement is null)
        {
            return;
        }

        await _discountRepository.InsertAsync(BuildReplacementDiscountDetail(input, context, mainDiscountId, replacement));
    }

    private ChargeDiscountDetail BuildReplacementDiscountDetail(
        DiscountDetailSaveInput input,
        MainDiscountDetailContext context,
        long mainDiscountId,
        ReplaceChildResult replacement)
    {
        long? firstRuleId = context.FirstRuleId == 0 ? null : context.FirstRuleId;

        return new ChargeDiscountDetail
        {
            RequestId = input.RequestId,
            TraceId = input.TraceId,
            ChargeNo = NormalizeString(input.Request.ChargeNo),
            ChargeDetailNo = NormalizeString(input.Item.ChargeDetailNo),
            PatientId = input.Request.PatientId,
            VisitId = input.Request.VisitId,
            ItemCode = replacement.ItemCode,
            ItemName = replacement.ItemName,
            RuleId = firstRuleId,
            RuntimePackageId = input.RuntimeTrace?.RuntimePackageId,
            RuntimeRuleId = firstRuleId,
            SourcePolicyVersionId = context.FirstRuntimeRule?.SourcePolicyVersionId,
            SourceTemplateVersionId = context.FirstRuntimeRule?.SourceTemplateVersionId,
            ResultGroupNo = context.ResultGroupNo,
            ParentDiscountId = mainDiscountId,
            ConvertedQty = replacement.Qty,
            FinalQty = replacement.Qty,
            UnitPrice = replacement.UnitPrice,
            OriginalAmt = 0m,
            CalculatedAmt = context.ReplacementAmount,
            FinalAmt = context.ReplacementAmount,
            DiscountAmt = -context.ReplacementAmount,
            DiscountType = "EXCESS_REPLACE",
            ReasonCode = "EXCESS_REPLACE",
            ReasonDesc = PricingResponseBuilder.BuildReplacementReasonDesc(input.Item, replacement),
            Status = input.Status,
            OccurredAt = context.OccurredAt
        };
    }

    private static ChildDiscountDetailSaveInput BuildChildDiscountInput(
        DiscountDetailSaveInput input,
        MainDiscountDetailContext context,
        long mainDiscountId)
    {
        return new ChildDiscountDetailSaveInput
        {
            RequestId = input.RequestId,
            TraceId = input.TraceId,
            Request = input.Request,
            Item = input.Item,
            ChildPricingResults = input.Result.ChildPricingResults,
            ResultGroupNo = context.ResultGroupNo,
            MainDiscountId = mainDiscountId,
            FirstRuleId = context.FirstRuleId,
            Status = input.Status,
            Now = context.OccurredAt,
            RuntimeTrace = input.RuntimeTrace
        };
    }

    private async Task SaveChildDiscountDetailsAsync(ChildDiscountDetailSaveInput input)
    {
        foreach (var child in input.ChildPricingResults)
        {
            await _discountRepository.InsertAsync(BuildChildDiscountDetail(input, child));
        }
    }

    private static ChargeDiscountDetail BuildChildDiscountDetail(
        ChildDiscountDetailSaveInput input,
        ChildPricingResult child)
    {
        var childAmount = PricingAmountRounder.RoundFinal(child.Amount);
        long? firstRuleId = input.FirstRuleId == 0 ? null : input.FirstRuleId;
        var runtimeRule = input.RuntimeTrace?.FindRule(firstRuleId);

        return new ChargeDiscountDetail
        {
            RequestId = input.RequestId,
            TraceId = input.TraceId,
            ChargeNo = NormalizeString(input.Request.ChargeNo),
            ChargeDetailNo = NormalizeString(input.Item.ChargeDetailNo),
            PatientId = input.Request.PatientId,
            VisitId = input.Request.VisitId,
            ItemCode = child.ItemCode,
            ItemName = child.ItemName,
            RuleId = firstRuleId,
            RuntimePackageId = input.RuntimeTrace?.RuntimePackageId,
            RuntimeRuleId = firstRuleId,
            SourcePolicyVersionId = runtimeRule?.SourcePolicyVersionId,
            SourceTemplateVersionId = runtimeRule?.SourceTemplateVersionId,
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
            ReasonDesc = PricingResponseBuilder.BuildChildReasonDesc(input.Item, child),
            Status = input.Status,
            OccurredAt = input.Now
        };
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
