using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 退费冲正 workflow。
/// </summary>
public sealed class PricingReverseWorkflow
{
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IChargeDiscountDetailRepository _discountRepository;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly IChargeReverseLogRepository _reverseLogRepository;
    private readonly PricingReverseLogWriter _reverseLogWriter;
    private readonly PricingLimitOccupyWriter _limitOccupyWriter;
    private readonly PricingTransactionExecutor _transactionExecutor;
    private readonly PricingReverseHistoryReader _reverseHistoryReader;
    private readonly IClock _clock;
    private readonly ILogger _logger;

    /// <summary>
    /// 初始化退费冲正 workflow。
    /// </summary>
    /// <param name="requestLogRepository">请求日志仓储。</param>
    /// <param name="discountRepository">折价明细仓储。</param>
    /// <param name="limitRepository">限额占用仓储。</param>
    /// <param name="reverseLogRepository">冲正日志仓储。</param>
    /// <param name="reverseLogWriter">冲正日志写入器。</param>
    /// <param name="limitOccupyWriter">负向占额写入器。</param>
    /// <param name="transactionExecutor">事务执行器。</param>
    /// <param name="reverseHistoryReader">历史冲正累计读取器。</param>
    /// <param name="clock">技术时间提供者。</param>
    /// <param name="logger">日志组件。</param>
    public PricingReverseWorkflow(
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository,
        IChargeReverseLogRepository reverseLogRepository,
        PricingReverseLogWriter reverseLogWriter,
        PricingLimitOccupyWriter limitOccupyWriter,
        PricingTransactionExecutor transactionExecutor,
        PricingReverseHistoryReader reverseHistoryReader,
        IClock clock,
        ILogger logger)
    {
        _requestLogRepository = requestLogRepository;
        _discountRepository = discountRepository;
        _limitRepository = limitRepository;
        _reverseLogRepository = reverseLogRepository;
        _reverseLogWriter = reverseLogWriter;
        _limitOccupyWriter = limitOccupyWriter;
        _transactionExecutor = transactionExecutor;
        _reverseHistoryReader = reverseHistoryReader;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行退费冲正。
    /// </summary>
    /// <param name="request">冲正请求。</param>
    public async Task ExecuteAsync(PricingReverseRequest request)
    {
        PricingRequestGuard.EnsureReverseRequest(request);
        var reverseNo = request.ReverseNo!;

        _logger.LogInformation(
            "REVERSE 开始 OriginalRequestId={OriginalRequestId}, ItemCode={ItemCode}, ReverseQty={ReverseQty}",
            request.OriginalRequestId, request.ItemCode, request.ReverseQty);

        await _transactionExecutor.ExecuteAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[]
            {
                PricingLockKeyBuilder.BuildRequestLockKey(request.OriginalRequestId),
                PricingLockKeyBuilder.BuildReverseLockKey(request.OriginalRequestId, reverseNo)
            });

            var log = await _requestLogRepository.GetByIdAsync(request.OriginalRequestId)
                ?? throw new BizException(
                    BizErrorCode.RequestNotFound,
                    404,
                    $"原请求不存在: {request.OriginalRequestId}");

            var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
            var sameReverseNo = reverseLogs.FirstOrDefault(r =>
                string.Equals(r.ReverseNo, request.ReverseNo, StringComparison.OrdinalIgnoreCase));
            if (sameReverseNo is not null)
            {
                if (!PricingReverseDetailSelector.IsSameReverseRequest(sameReverseNo, request))
                {
                    throw new BizException(
                        BizErrorCode.IdempotencyConflict,
                        409,
                        $"ReverseNo={request.ReverseNo} 已存在，但本次冲正参数与首次请求不一致");
                }

                _logger.LogInformation(
                    "REVERSE 幂等命中 OriginalRequestId={OriginalRequestId}, ReverseNo={ReverseNo}",
                    request.OriginalRequestId, request.ReverseNo);
                return;
            }

            if (!IsCommittedBusinessStatus(log.BusinessStatus))
            {
                _logger.LogWarning(
                    "REVERSE 状态校验失败 OriginalRequestId={OriginalRequestId}, 当前状态={Status}, 期望=CONFIRMED/COMMITTED",
                    request.OriginalRequestId, log.BusinessStatus);
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    $"只有CONFIRMED或COMMITTED状态可以REVERSE, 当前: {log.BusinessStatus}");
            }

            var details = await _discountRepository.GetByRequestIdAsync(request.OriginalRequestId);
            var matchedDetails = PricingReverseDetailSelector.FilterReverseDetails(details, request);
            if (matchedDetails.Count == 0)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    "未找到可退费的原收费明细");
            }

            var allOriginalQty = details
                .Where(d => d.Status == BusinessStatusCodes.Confirmed || d.Status == BusinessStatusCodes.Committed)
                .Sum(d => d.FinalQty ?? 0);
            var originalQty = matchedDetails.Sum(d => d.FinalQty ?? 0);
            var originalAmt = matchedDetails.Sum(d => d.FinalAmt ?? 0);
            var reverseQty = request.ReverseQty ?? originalQty;
            if (reverseQty <= 0)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    "退费数量必须大于0");
            }

            var historicalReversedQty = await _reverseHistoryReader.GetHistoricalReversedQtyAsync(request);
            var allHistoricalReversedQty = await _reverseHistoryReader.GetHistoricalReversedQtyAsync(request.OriginalRequestId);
            if (historicalReversedQty + reverseQty > originalQty)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    $"原有效数量={originalQty}, 历史已退={historicalReversedQty}, 本次退费={reverseQty}");
            }

            var reverseAmt = request.ReverseAmt ??
                (originalQty == 0 ? 0 : originalAmt * reverseQty / originalQty);
            reverseAmt = PricingAmountRounder.RoundFinal(reverseAmt);
            var historicalReversedAmt = await _reverseHistoryReader.GetHistoricalReversedAmtAsync(request);
            if (historicalReversedAmt + reverseAmt > originalAmt)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    $"原有效金额={originalAmt}, 历史已退={historicalReversedAmt}, 本次退费={reverseAmt}");
            }

            var isFullReverse =
                allHistoricalReversedQty + reverseQty == allOriginalQty &&
                historicalReversedAmt + reverseAmt == originalAmt;

            var groupedDetails = matchedDetails
                .Where(d => !string.IsNullOrWhiteSpace(d.ResultGroupNo))
                .GroupBy(d => d.ResultGroupNo)
                .ToList();
            foreach (var group in groupedDetails)
            {
                var groupOriginalQty = group.Sum(d => d.FinalQty ?? 0);
                var groupOriginalAmt = group.Sum(d => d.FinalAmt ?? 0);

                var groupHistoricalQty = reverseLogs
                    .Where(r => group.Any(d =>
                        string.Equals(d.ItemCode, r.ItemCode, StringComparison.OrdinalIgnoreCase) &&
                        d.ChargeDetailNo == r.ChargeDetailNo))
                    .Sum(r => r.ReverseQty ?? 0);
                var groupHistoricalAmt = reverseLogs
                    .Where(r => group.Any(d =>
                        string.Equals(d.ItemCode, r.ItemCode, StringComparison.OrdinalIgnoreCase) &&
                        d.ChargeDetailNo == r.ChargeDetailNo))
                    .Sum(r => r.ReverseAmt ?? 0);

                decimal groupReverseQty;
                decimal groupReverseAmt;
                if (group.Key == groupedDetails.Last().Key)
                {
                    var allocatedQty = groupedDetails.Where(g => g.Key != group.Key)
                        .Sum(g => originalQty == 0 ? 0 : reverseQty * g.Sum(d => d.FinalQty ?? 0) / originalQty);
                    var allocatedAmt = groupedDetails.Where(g => g.Key != group.Key)
                        .Sum(g => originalAmt == 0 ? 0 : reverseAmt * g.Sum(d => d.FinalAmt ?? 0) / originalAmt);
                    groupReverseQty = reverseQty - allocatedQty;
                    groupReverseAmt = reverseAmt - allocatedAmt;
                }
                else
                {
                    var groupRatio = originalQty == 0 ? 0 : groupOriginalQty / originalQty;
                    groupReverseQty = reverseQty * groupRatio;
                    groupReverseAmt = originalAmt == 0 ? 0 : reverseAmt * groupOriginalAmt / originalAmt;
                }

                if (groupHistoricalQty + groupReverseQty > groupOriginalQty)
                {
                    throw new BizException(
                        BizErrorCode.ReverseNotAllowed,
                        409,
                        $"ResultGroupNo={group.Key}, 组原有效数量={groupOriginalQty}, 组历史已退={groupHistoricalQty}, 组本次退费={groupReverseQty}");
                }

                if (groupHistoricalAmt + groupReverseAmt > groupOriginalAmt)
                {
                    throw new BizException(
                        BizErrorCode.ReverseNotAllowed,
                        409,
                        $"ResultGroupNo={group.Key}, 组原有效金额={groupOriginalAmt}, 组历史已退={groupHistoricalAmt}, 组本次退费={groupReverseAmt}");
                }
            }

            if (isFullReverse)
            {
                log.BusinessStatus = BusinessStatusCodes.Reversed;
                log.ResponseAt = _clock.Now;
                await _requestLogRepository.UpdateAsync(log);

                await _discountRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, OccupyStatusCodes.Reversed);
                await _limitRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, OccupyStatusCodes.Reversed);
            }

            var reverseTime = request.ReverseTime ?? _clock.Now;
            var reverseRequestId = await _reverseLogWriter.SaveRequestLogAsync(new ReverseRequestLogSaveInput
            {
                Request = request,
                OriginalLog = log,
                MatchedDetails = matchedDetails,
                ReverseQty = reverseQty,
                ReverseAmt = reverseAmt,
                ReverseTime = reverseTime
            });
            if (!isFullReverse)
            {
                await _limitOccupyWriter.InsertNegativeAsync(new NegativeLimitOccupyInput
                {
                    Request = request,
                    MatchedDetails = matchedDetails,
                    ReverseRequestId = reverseRequestId,
                    TraceId = log.TraceId,
                    ReverseQty = reverseQty,
                    ReverseAmt = reverseAmt,
                    ReverseTime = reverseTime
                });
            }

            await _reverseLogWriter.SaveReverseLogAsync(new ReverseLogSaveInput
            {
                Request = request,
                OriginalLog = log,
                MatchedDetails = matchedDetails,
                ReverseRequestId = reverseRequestId,
                ReverseQty = reverseQty,
                ReverseAmt = reverseAmt,
                ReverseTime = reverseTime
            });

            _logger.LogInformation(
                "REVERSE 成功 OriginalRequestId={OriginalRequestId}, ItemCode={ItemCode}, ReverseQty={ReverseQty}, ReverseAmt={ReverseAmt}, 全退={IsFullReverse}",
                request.OriginalRequestId, matchedDetails.FirstOrDefault()?.ItemCode,
                reverseQty, reverseAmt, isFullReverse);
        });
    }

    private static bool IsCommittedBusinessStatus(string? businessStatus)
    {
        return string.Equals(businessStatus, BusinessStatusCodes.Confirmed, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(businessStatus, BusinessStatusCodes.Committed, StringComparison.OrdinalIgnoreCase);
    }
}
