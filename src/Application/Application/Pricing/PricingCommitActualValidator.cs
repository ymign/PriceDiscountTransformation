using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing;

internal static class PricingCommitActualValidator
{
    public static void Validate(
        PricingCommitRequest request,
        IReadOnlyList<ChargeDiscountDetail> details,
        bool requireActualItems)
    {
        if (details.Count == 0)
        {
            throw new BizException(
                BizErrorCode.CommitDetailNotFound,
                409,
                $"RequestId={request.RequestId} 未找到 confirm 折价明细");
        }

        var expected = BuildExpectedCommitDetails(details);
        var expectedTotal = PricingAmountRounder.RoundFinal(expected.Sum(v => v.Amount));
        var actualItems = (request.ActualItems ?? Array.Empty<PricingCommitActualItemRequest>())
            .Where(IsBillableActualItem)
            .ToList();
        if (actualItems.Count == 0)
        {
            if (request.ActualTotalAmount.HasValue &&
                PricingAmountRounder.RoundFinal(request.ActualTotalAmount.Value) != expectedTotal)
            {
                throw new BizException(
                    BizErrorCode.CommitAmountMismatch,
                    409,
                    $"confirm总金额={expectedTotal}, HIS实际总金额={PricingAmountRounder.RoundFinal(request.ActualTotalAmount.Value)}");
            }

            if (requireActualItems)
            {
                throw new BizException(
                    BizErrorCode.CommitActualItemsRequired,
                    409,
                    "commit 必须传入 HIS 实际落账明细");
            }

            return;
        }

        foreach (var item in actualItems)
        {
            if (string.IsNullOrWhiteSpace(item.ItemCode))
            {
                throw new BizException(
                    BizErrorCode.CommitActualItemsRequired,
                    409,
                    "实际落账明细 ItemCode 不能为空");
            }
        }

        var actual = actualItems
            .Select(i => new CommitActualDetail
            {
                ChargeDetailNo = NormalizeString(i.ChargeDetailNo)?.ToUpperInvariant(),
                ItemCode = NormalizeString(i.ItemCode)?.ToUpperInvariant() ?? string.Empty,
                PartSeq = i.PartSeq,
                Qty = i.FinalQty,
                Amount = i.FinalAmount
            })
            .ToList();

        var usedActualIndexes = new HashSet<int>();
        foreach (var expectedDetail in expected.OrderByDescending(e => e.RequireChargeDetailNo))
        {
            var candidateIndexes = actual
                .Select((item, index) => new { item, index })
                .Where(x => !usedActualIndexes.Contains(x.index))
                .Where(x => IsSameCommitIdentity(expectedDetail, x.item))
                .Select(x => x.index)
                .ToList();
            if (candidateIndexes.Count == 0)
            {
                throw new BizException(
                    BizErrorCode.CommitDetailMismatch,
                    409,
                    $"HIS 未回传实际落账明细 {FormatCommitDetail(expectedDetail)}");
            }

            var matchedIndex = candidateIndexes
                .Cast<int?>()
                .FirstOrDefault(index =>
                    Math.Round(actual[index!.Value].Qty, 4) == Math.Round(expectedDetail.Qty, 4) &&
                    PricingAmountRounder.RoundFinal(actual[index.Value].Amount) ==
                    PricingAmountRounder.RoundFinal(expectedDetail.Amount))
                ?? candidateIndexes[0];

            var actualValue = actual[matchedIndex];
            if (Math.Round(actualValue.Qty, 4) != Math.Round(expectedDetail.Qty, 4))
            {
                throw new BizException(
                    BizErrorCode.CommitQtyMismatch,
                    409,
                    $"{FormatCommitDetail(expectedDetail)} confirm数量={Math.Round(expectedDetail.Qty, 4)}, HIS实际数量={Math.Round(actualValue.Qty, 4)}");
            }

            var expectedAmount = PricingAmountRounder.RoundFinal(expectedDetail.Amount);
            var actualAmount = PricingAmountRounder.RoundFinal(actualValue.Amount);
            if (actualAmount != expectedAmount)
            {
                throw new BizException(
                    BizErrorCode.CommitAmountMismatch,
                    409,
                    $"{FormatCommitDetail(expectedDetail)} confirm金额={expectedAmount}, HIS实际金额={actualAmount}");
            }

            usedActualIndexes.Add(matchedIndex);
        }

        var extraActuals = actual
            .Select((item, index) => new { item, index })
            .Where(x => !usedActualIndexes.Contains(x.index))
            .Select(x => x.item)
            .ToList();
        if (extraActuals.Count > 0)
        {
            throw new BizException(
                BizErrorCode.CommitDetailMismatch,
                409,
                $"HIS 回传了 confirm 未产生的落账明细 {string.Join(", ", extraActuals.Select(FormatCommitActual))}");
        }

        var actualTotal = PricingAmountRounder.RoundFinal(actual.Sum(v => v.Amount));
        if (actualTotal != expectedTotal)
        {
            throw new BizException(
                BizErrorCode.CommitAmountMismatch,
                409,
                $"confirm总金额={expectedTotal}, HIS实际明细合计={actualTotal}");
        }

        if (request.ActualTotalAmount.HasValue &&
            PricingAmountRounder.RoundFinal(request.ActualTotalAmount.Value) != expectedTotal)
        {
            throw new BizException(
                BizErrorCode.CommitAmountMismatch,
                409,
                $"confirm总金额={expectedTotal}, HIS实际总金额={PricingAmountRounder.RoundFinal(request.ActualTotalAmount.Value)}");
        }
    }

    private static IReadOnlyList<CommitExpectedDetail> BuildExpectedCommitDetails(
        IReadOnlyList<ChargeDiscountDetail> details)
    {
        return details
            .Where(IsBillableExpectedDetail)
            .Select(d => new CommitExpectedDetail
            {
                ChargeDetailNo = NormalizeString(d.ChargeDetailNo)?.ToUpperInvariant(),
                ItemCode = NormalizeString(d.ItemCode)?.ToUpperInvariant() ?? string.Empty,
                PartSeq = d.PartSeq,
                Qty = d.FinalQty ?? 0m,
                Amount = d.FinalAmt ?? 0m,
                RequireChargeDetailNo = RequiresCommitChargeDetailMatch(d)
            })
            .ToList();
    }

    private static bool IsBillableExpectedDetail(ChargeDiscountDetail detail)
    {
        return (detail.FinalQty ?? 0m) != 0m ||
               (detail.FinalAmt ?? 0m) != 0m;
    }

    private static bool IsBillableActualItem(PricingCommitActualItemRequest item)
    {
        return item.FinalQty != 0m || item.FinalAmount != 0m;
    }

    private static bool RequiresCommitChargeDetailMatch(ChargeDiscountDetail detail)
    {
        return !detail.ParentDiscountId.HasValue &&
               !string.IsNullOrWhiteSpace(detail.ChargeDetailNo);
    }

    private static bool IsSameCommitIdentity(
        CommitExpectedDetail expected,
        CommitActualDetail actual)
    {
        if (!string.Equals(expected.ItemCode, actual.ItemCode, StringComparison.OrdinalIgnoreCase) ||
            expected.PartSeq != actual.PartSeq)
        {
            return false;
        }

        return !expected.RequireChargeDetailNo ||
               string.Equals(expected.ChargeDetailNo, actual.ChargeDetailNo, StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatCommitDetail(CommitExpectedDetail detail)
    {
        var chargeDetailNo = string.IsNullOrWhiteSpace(detail.ChargeDetailNo) ? "-" : detail.ChargeDetailNo;
        var itemCode = string.IsNullOrWhiteSpace(detail.ItemCode) ? "-" : detail.ItemCode;
        var partSeq = detail.PartSeq.HasValue ? detail.PartSeq.Value.ToString() : "-";
        var matchMode = detail.RequireChargeDetailNo ? "严格明细号" : "HIS实落明细号";
        return $"ChargeDetailNo={chargeDetailNo}, ItemCode={itemCode}, PartSeq={partSeq}, MatchMode={matchMode}";
    }

    private static string FormatCommitActual(CommitActualDetail detail)
    {
        var chargeDetailNo = string.IsNullOrWhiteSpace(detail.ChargeDetailNo) ? "-" : detail.ChargeDetailNo;
        var itemCode = string.IsNullOrWhiteSpace(detail.ItemCode) ? "-" : detail.ItemCode;
        var partSeq = detail.PartSeq.HasValue ? detail.PartSeq.Value.ToString() : "-";
        return $"ChargeDetailNo={chargeDetailNo}, ItemCode={itemCode}, PartSeq={partSeq}";
    }

    private static string? NormalizeString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private sealed record CommitExpectedDetail
    {
        public string? ChargeDetailNo { get; init; }

        public string ItemCode { get; init; } = string.Empty;

        public int? PartSeq { get; init; }

        public decimal Qty { get; init; }

        public decimal Amount { get; init; }

        public bool RequireChargeDetailNo { get; init; }
    }

    private sealed record CommitActualDetail
    {
        public string? ChargeDetailNo { get; init; }

        public string ItemCode { get; init; } = string.Empty;

        public int? PartSeq { get; init; }

        public decimal Qty { get; init; }

        public decimal Amount { get; init; }
    }
}


