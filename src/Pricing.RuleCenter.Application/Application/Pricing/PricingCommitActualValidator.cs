using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// commit 阶段 HIS 实际落账明细对账校验器。
/// </summary>
/// <remarks>
/// <para>
/// confirm 返回的是规则中心允许落账的事实，commit 回传的是 HIS 最终写库成功的事实。
/// 两者必须逐项闭合，才能把 <c>CONFIRM_PENDING</c> 推进为 <c>CONFIRMED</c>。
/// </para>
/// <para>
/// 普通主项目要求收费明细号严格匹配；替换子项和加收子项允许 HIS 落账时生成新明细号，
/// 但必须按项目编码、partSeq、数量和金额匹配。这样既兼容 HIS 子项新行号，又防止漏落账或多落账。
/// </para>
/// </remarks>
internal static class PricingCommitActualValidator
{
    /// <summary>
    /// 校验 HIS 实际落账明细是否覆盖 confirm 保存的全部有效收费明细。
    /// </summary>
    /// <param name="request">commit 请求。</param>
    /// <param name="details">confirm 阶段保存的折价明细。</param>
    /// <param name="requireActualItems">是否强制要求回传 ActualItems；首次 commit 为 true，重复 commit 可为 false。</param>
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

        // expected 来自 confirm 保存的折价明细，是规则中心认可的可落账结果。
        // 这里过滤掉数量和金额都为 0 的明细，避免“超出部分归零”的说明性明细阻断 commit。
        var expected = BuildExpectedCommitDetails(details);
        var expectedTotal = PricingAmountRounder.RoundFinal(expected.Sum(v => v.Amount));
        var actualItems = (request.ActualItems ?? Array.Empty<PricingCommitActualItemRequest>())
            .Where(IsBillableActualItem)
            .ToList();
        if (actualItems.Count == 0)
        {
            // 重复 commit 或旧渠道可能只回传总金额。首次 commit 仍要求 ActualItems，
            // 因为只有明细级对账才能覆盖主子项目和替换项目。
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

        // actual 做同样的规范化，后续匹配只比较稳定业务键和最终数量金额。
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
            // 严格明细号的普通主项目优先匹配，避免被允许新行号的子项候选抢占。
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
            // 如果身份匹配但数量或金额不一致，返回更精确的错误码，方便 HIS 定位是数量错还是金额错。
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

        // HIS 多回传任何有效明细都视为不一致。否则规则中心会确认一笔自己没有计算过的收费事实。
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
        // 主项目必须严格匹配 HIS 原收费明细号；子项/替换项有 ParentDiscountId，
        // HIS 可能在落账时生成新明细号，因此不能要求 chargeDetailNo 完全相同。
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
        /// <summary>
        /// confirm 阶段保存的收费明细号；普通主项目需要严格匹配。
        /// </summary>
        public string? ChargeDetailNo { get; init; }

        /// <summary>
        /// 项目编码。
        /// </summary>
        public string ItemCode { get; init; } = string.Empty;

        /// <summary>
        /// 多片段序号。
        /// </summary>
        public int? PartSeq { get; init; }

        /// <summary>
        /// 规则中心确认的最终数量。
        /// </summary>
        public decimal Qty { get; init; }

        /// <summary>
        /// 规则中心确认的最终金额。
        /// </summary>
        public decimal Amount { get; init; }

        /// <summary>
        /// 是否要求 HIS 实际落账明细号与 confirm 明细号严格一致。
        /// </summary>
        public bool RequireChargeDetailNo { get; init; }
    }

    private sealed record CommitActualDetail
    {
        /// <summary>
        /// HIS 实际落账明细号。
        /// </summary>
        public string? ChargeDetailNo { get; init; }

        /// <summary>
        /// HIS 实际落账项目编码。
        /// </summary>
        public string ItemCode { get; init; } = string.Empty;

        /// <summary>
        /// HIS 实际落账片段序号。
        /// </summary>
        public int? PartSeq { get; init; }

        /// <summary>
        /// HIS 实际落账数量。
        /// </summary>
        public decimal Qty { get; init; }

        /// <summary>
        /// HIS 实际落账金额。
        /// </summary>
        public decimal Amount { get; init; }
    }
}


