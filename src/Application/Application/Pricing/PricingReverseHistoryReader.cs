using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 历史冲正累计读取器。
/// </summary>
public sealed class PricingReverseHistoryReader
{
    private readonly IChargeReverseLogRepository _reverseLogRepository;

    /// <summary>
    /// 初始化历史冲正累计读取器。
    /// </summary>
    /// <param name="reverseLogRepository">冲正日志仓储。</param>
    public PricingReverseHistoryReader(IChargeReverseLogRepository reverseLogRepository)
    {
        _reverseLogRepository = reverseLogRepository;
    }

    /// <summary>
    /// 查询指定退费筛选条件下的历史已退数量。
    /// </summary>
    /// <param name="request">当前退费请求。</param>
    /// <returns>历史已退数量合计。</returns>
    public async Task<decimal> GetHistoricalReversedQtyAsync(PricingReverseRequest request)
    {
        var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
        var chargeDetailNo = NormalizeString(request.ChargeDetailNo);
        var itemCode = NormalizeString(request.ItemCode);

        return reverseLogs
            .Where(r => string.IsNullOrWhiteSpace(chargeDetailNo) ||
                        string.Equals(r.ChargeDetailNo, chargeDetailNo, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.IsNullOrWhiteSpace(itemCode) ||
                        string.Equals(r.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase))
            .Where(r => !request.PartSeq.HasValue || r.PartSeq == request.PartSeq)
            .Sum(r => r.ReverseQty ?? 0);
    }

    /// <summary>
    /// 查询原请求范围下的全部历史已退数量。
    /// </summary>
    /// <param name="originalRequestId">原请求主键。</param>
    /// <returns>历史已退数量合计。</returns>
    public async Task<decimal> GetHistoricalReversedQtyAsync(long originalRequestId)
    {
        var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(originalRequestId);
        return reverseLogs.Sum(r => r.ReverseQty ?? 0);
    }

    /// <summary>
    /// 查询指定退费筛选条件下的历史已退金额。
    /// </summary>
    /// <param name="request">当前退费请求。</param>
    /// <returns>历史已退金额合计。</returns>
    public async Task<decimal> GetHistoricalReversedAmtAsync(PricingReverseRequest request)
    {
        var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
        var chargeDetailNo = NormalizeString(request.ChargeDetailNo);
        var itemCode = NormalizeString(request.ItemCode);

        return reverseLogs
            .Where(r => string.IsNullOrWhiteSpace(chargeDetailNo) ||
                        string.Equals(r.ChargeDetailNo, chargeDetailNo, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.IsNullOrWhiteSpace(itemCode) ||
                        string.Equals(r.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase))
            .Where(r => !request.PartSeq.HasValue || r.PartSeq == request.PartSeq)
            .Sum(r => r.ReverseAmt ?? 0);
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
