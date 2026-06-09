using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Charging;
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
        // 主项目、替换子项和加收子项使用同一个 resultGroupNo。
        // commit/cancel/reverse 以该组号保护主子项目原子性。
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
        var firstRuntimeRule = input.RuntimeTrace?.FindRule(firstRuleId == 0 ? null : firstRuleId);
        var mainFinalAmt = result.ReplaceChildResult is null
            ? result.FinalAmount
            : Math.Max(result.FinalAmount - replacementAmt, 0m);
        // 如果存在替换子项，主项目自身金额应扣除替换金额，替换部分另存一条子明细。
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
            RuntimePackageId = input.RuntimeTrace?.RuntimePackageId,
            RuntimeRuleId = firstRuleId == 0 ? null : firstRuleId,
            SourcePolicyVersionId = firstRuntimeRule?.SourcePolicyVersionId,
            SourceTemplateVersionId = firstRuntimeRule?.SourceTemplateVersionId,
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
            // 无替换子项时，只需要额外写普通加收子项。
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
                Now = now,
                RuntimeTrace = input.RuntimeTrace
            });
            return;
        }

        var replacement = result.ReplaceChildResult;
        // 替换子项是“超限部分换成另一个收费项目”的结果，ParentDiscountId 指向主项目明细。
        // DiscountAmt 为负数，表示它增加了最终收费，而不是减少收费。
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
            RuntimePackageId = input.RuntimeTrace?.RuntimePackageId,
            RuntimeRuleId = firstRuleId == 0 ? null : firstRuleId,
            SourcePolicyVersionId = firstRuntimeRule?.SourcePolicyVersionId,
            SourceTemplateVersionId = firstRuntimeRule?.SourceTemplateVersionId,
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
            Now = now,
            RuntimeTrace = input.RuntimeTrace
        });
    }

    private async Task SaveChildDiscountDetailsAsync(ChildDiscountDetailSaveInput input)
    {
        // 子项明细与主项目共享 ResultGroupNo，并通过 ParentDiscountId 指向主明细。
        // HIS commit 可以给子项生成新 chargeDetailNo，但必须按 itemCode + partSeq + 数量金额完成对账。
        var request = input.Request;
        var item = input.Item;
        foreach (var child in input.ChildPricingResults)
        {
            var childAmount = PricingAmountRounder.RoundFinal(child.Amount);
            var runtimeRule = input.RuntimeTrace?.FindRule(input.FirstRuleId == 0 ? null : input.FirstRuleId);
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
                RuntimePackageId = input.RuntimeTrace?.RuntimePackageId,
                RuntimeRuleId = input.FirstRuleId == 0 ? null : input.FirstRuleId,
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
