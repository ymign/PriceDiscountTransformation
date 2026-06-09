using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Builders;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing.Persistence;

/// <summary>
/// 保存 reverse 负向占用所需输入。
/// </summary>
/// <remarks>
/// 部分退费不直接修改原占用，而是插入负向占用，保证额度变化可追溯、可对账。
/// </remarks>
internal sealed record NegativeLimitOccupyInput
{
    /// <summary>退费请求。</summary>
    public PricingReverseRequest Request { get; init; } = null!;

    public IReadOnlyList<ChargeDiscountDetail> MatchedDetails { get; init; } =
        Array.Empty<ChargeDiscountDetail>();

    public long ReverseRequestId { get; init; }

    public string? TraceId { get; init; }

    public decimal ReverseQty { get; init; }

    public decimal ReverseAmt { get; init; }

    public DateTime ReverseTime { get; init; }
}

/// <summary>
/// 计价限额占用写入器，负责 confirm 正向占额和 reverse 负向释放。
/// </summary>
public sealed class PricingLimitOccupyWriter
{
    /// <summary>
    /// 限额占用仓储。
    /// </summary>
    private readonly ILimitOccupyRepository _limitRepository;
    /// <summary>
    /// 计价配置，主要使用 confirm 保护占用过期时间。
    /// </summary>
    private readonly PricingOptions _options;
    /// <summary>
    /// 限额占用日志。
    /// </summary>
    private readonly ILogger<PricingLimitOccupyWriter> _logger;
    /// <summary>
    /// 统一时钟。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// 初始化计价限额占用写入器。
    /// </summary>
    /// <param name="limitRepository">限额占用仓储。</param>
    /// <param name="options">计价配置项。</param>
    /// <param name="logger">日志组件。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    public PricingLimitOccupyWriter(
        ILimitOccupyRepository limitRepository,
        IOptions<PricingOptions> options,
        ILogger<PricingLimitOccupyWriter> logger,
        IClock clock)
    {
        _limitRepository = limitRepository;
        _options = options.Value;
        _logger = logger;
        _clock = clock;
    }

    internal async Task SaveAsync(
        long requestId,
        string? traceId,
        PricingCalculateItemRequest item,
        PricingResult result)
    {
        // 正向占用只在 confirm 阶段写入，状态为 PENDING。
        // commit 后推进为 CONFIRMED，cancel/expire 后释放为 CANCELLED/EXPIRED。
        var now = _clock.Now;
        var expireAt = now.AddMinutes(_options.ConfirmExpireMinutes);
        var resultGroupNo = PricingResultGroupNoGenerator.Resolve(requestId, item, result);
        foreach (var occupy in result.LimitOccupies)
        {
            // 执行器只生成占用草稿，RequestId、TraceId、过期时间和状态需要在应用层统一补齐。
            occupy.RequestId = requestId;
            occupy.TraceId = traceId;
            occupy.ChargeDetailNo = NormalizeString(item.ChargeDetailNo);
            occupy.ResultGroupNo = resultGroupNo;
            occupy.Status = OccupyStatusCodes.Pending;
            occupy.ExpireAt = expireAt;
            occupy.OccupiedAt = now;
            await _limitRepository.InsertAsync(occupy);

            _logger.LogDebug(
                "限额占用 请求ID={RequestId}, 限额类型={LimitType}, 限额维度编码={LimitDimensionCode}, 占用数量={OccupyQty}, 项目编码={ItemCode}",
                requestId, occupy.LimitType, occupy.LimitDimensionCode, occupy.OccupyQty, occupy.ItemCode);
        }
    }

    internal async Task InsertNegativeAsync(NegativeLimitOccupyInput input)
    {
        // 负向占用只用于部分退费。全额退费会直接把原占用状态推进为 REVERSED。
        var request = input.Request;
        var originalOccupies = await _limitRepository.GetByRequestIdAsync(request.OriginalRequestId);
        var matchedChargeDetailNos = input.MatchedDetails
            .Select(d => NormalizeString(d.ChargeDetailNo))
            .Where(chargeDetailNo => chargeDetailNo is not null)
            .Select(chargeDetailNo => chargeDetailNo!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var matchedResultGroupNos = input.MatchedDetails
            .Select(d => NormalizeString(d.ResultGroupNo))
            .Where(resultGroupNo => resultGroupNo is not null)
            .Select(resultGroupNo => resultGroupNo!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var matchedItemCodes = input.MatchedDetails
            .Select(d => NormalizeString(d.ItemCode))
            .Where(itemCode => itemCode is not null)
            .Select(itemCode => itemCode!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        // 候选原占用必须与本次退费命中的明细或结果组相关，且仅当日退费释放当日额度。
        var candidates = originalOccupies
            .Where(o => o.Status == BusinessStatusCodes.Confirmed)
            .Where(o => o.OccupyType == "CHARGE")
            .Where(o => MatchReverseOccupyCandidate(
                o,
                matchedChargeDetailNos,
                matchedResultGroupNos,
                matchedItemCodes))
            .Where(o => !request.PartSeq.HasValue || o.PartSeq == request.PartSeq)
            .Where(o => IsSameBusinessDay(o.BusinessChargeTime, input.ReverseTime))
            .ToList();

        var totalOriginalQty = candidates.Sum(o => Math.Abs(o.OccupyQty));
        if (totalOriginalQty <= 0)
        {
            return;
        }

        foreach (var occupy in candidates)
        {
            // 按原占用数量比例分摊本次退费数量和金额，避免多限额维度下释放总量不闭合。
            var now = _clock.Now;
            var ratio = Math.Abs(occupy.OccupyQty) / totalOriginalQty;
            var releaseQty = input.ReverseQty * ratio;
            var releaseAmt = input.ReverseAmt * ratio;
            await _limitRepository.InsertAsync(new LimitOccupy
            {
                RequestId = input.ReverseRequestId,
                TraceId = input.TraceId ?? occupy.TraceId,
                PatientId = occupy.PatientId,
                ItemCode = occupy.ItemCode,
                ChargeDetailNo = occupy.ChargeDetailNo,
                RuleId = occupy.RuleId,
                RuleVersionNo = occupy.RuleVersionNo,
                ResultGroupNo = occupy.ResultGroupNo,
                LimitType = occupy.LimitType,
                LimitKey = occupy.LimitKey,
                LimitDimensionCode = occupy.LimitDimensionCode,
                OccupyQty = -releaseQty,
                OccupyAmt = -PricingAmountRounder.RoundFinal(releaseAmt),
                OccupyType = PricingCallTypeCodes.Reverse,
                OriginalOccupyId = occupy.OccupyId,
                BusinessChargeTime = occupy.BusinessChargeTime,
                PartSeq = occupy.PartSeq,
                Status = BusinessStatusCodes.Confirmed,
                OccupiedAt = now,
                ConfirmedAt = now
            });

            _logger.LogInformation(
                "限额释放 原请求ID={OriginalRequestId}, 限额类型={LimitType}, 限额维度编码={LimitDimensionCode}, 释放数量={ReleaseQty}, 释放金额={ReleaseAmt}",
                request.OriginalRequestId, occupy.LimitType, occupy.LimitDimensionCode, releaseQty, releaseAmt);
        }
    }

    private static bool IsSameBusinessDay(DateTime? originalBusinessTime, DateTime reverseTime)
    {
        // 当前业务口径：当日退费释放额度；隔日退费重收后按重收当天重新校验。
        return originalBusinessTime.HasValue &&
               originalBusinessTime.Value.Date == reverseTime.Date;
    }

    private static bool MatchReverseOccupyCandidate(
        LimitOccupy occupy,
        IReadOnlySet<string> matchedChargeDetailNos,
        IReadOnlySet<string> matchedResultGroupNos,
        IReadOnlySet<string> matchedItemCodes)
    {
        // 优先按 resultGroupNo 匹配主子项目组；没有组号时退化到 chargeDetailNo；再没有时按 itemCode 匹配。
        var occupyChargeDetailNo = NormalizeString(occupy.ChargeDetailNo);
        var occupyResultGroupNo = NormalizeString(occupy.ResultGroupNo);
        var occupyItemCode = NormalizeString(occupy.ItemCode);

        if (matchedResultGroupNos.Count > 0)
        {
            return (occupyResultGroupNo is not null && matchedResultGroupNos.Contains(occupyResultGroupNo)) ||
                   (occupyChargeDetailNo is not null && matchedChargeDetailNos.Contains(occupyChargeDetailNo)) ||
                   (occupyResultGroupNo is null &&
                    occupyChargeDetailNo is null &&
                    occupyItemCode is not null &&
                    matchedItemCodes.Contains(occupyItemCode));
        }

        if (matchedChargeDetailNos.Count > 0)
        {
            return (occupyChargeDetailNo is not null && matchedChargeDetailNos.Contains(occupyChargeDetailNo)) ||
                   (occupyChargeDetailNo is null &&
                    occupyItemCode is not null &&
                    matchedItemCodes.Contains(occupyItemCode));
        }

        return occupyItemCode is not null && matchedItemCodes.Contains(occupyItemCode);
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
