using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 历史冲正累计读取器。
/// </summary>
/// <remarks>
/// reverse 校验不能只看本次退费数量/金额，还必须累计历史已退。
/// 本类集中读取冲正日志，给 workflow 做“本次退费 + 历史已退 &lt;= 原有效收费”的资金安全校验。
/// </remarks>
public sealed class PricingReverseHistoryReader
{
    /// <summary>
    /// 冲正日志仓储，用于读取原请求下已经发生的退费记录。
    /// </summary>
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
        // 按本次退费的筛选维度累计历史已退数量。
        // 如果请求指定 chargeDetailNo/itemCode/partSeq，只校验同一范围；否则校验原请求全部范围。
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
        // 全请求范围累计用于判断是否已经整单全退。
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
        // 金额也必须累计历史已退，防止多次按比例四舍五入后超过原有效金额。
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
