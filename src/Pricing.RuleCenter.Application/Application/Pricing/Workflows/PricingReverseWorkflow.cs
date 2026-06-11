using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 退费冲正工作流：对已落账的计价请求执行退费、冲销和额度释放。
/// </summary>
/// <remarks>
/// reverse 只处理已 commit/confirmed 的记录，基于原收费明细和历史退费记录计算可退数量/金额。
/// 部分退费通过负向占用抵扣额度；全退直接标记原请求为 REVERSED。
/// 以 OriginalRequestId + ReverseNo 作为退费幂等边界。
/// </remarks>
public sealed class PricingReverseWorkflow
{
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IChargeDiscountDetailRepository _discountRepository;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly IChargeReverseLogRepository _reverseLogRepository;
    private readonly PricingReverseLogWriter _reverseLogWriter;
    private readonly PricingLimitOccupyWriter _limitOccupyWriter;
    private readonly PricingReverseHistoryReader _reverseHistoryReader;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly ILogger<PricingReverseWorkflow> _logger;

    public PricingReverseWorkflow(
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository,
        IChargeReverseLogRepository reverseLogRepository,
        PricingReverseLogWriter reverseLogWriter,
        PricingLimitOccupyWriter limitOccupyWriter,
        PricingReverseHistoryReader reverseHistoryReader,
        IUnitOfWork unitOfWork,
        IClock clock,
        ILogger<PricingReverseWorkflow> logger)
    {
        _requestLogRepository = requestLogRepository;
        _discountRepository = discountRepository;
        _limitRepository = limitRepository;
        _reverseLogRepository = reverseLogRepository;
        _reverseLogWriter = reverseLogWriter;
        _limitOccupyWriter = limitOccupyWriter;
        _reverseHistoryReader = reverseHistoryReader;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行退费冲正：校验 → 锁定 → 幂等 → 可退校验 → 计算可退 → 写冲正日志。
    /// </summary>
    public async Task ExecuteAsync(PricingReverseRequest request)
    {
        PricingRequestGuard.EnsureReverseRequest(request);
        var reverseNo = request.ReverseNo!;

        _logger.LogInformation(
            "退费冲正开始 原请求ID={OriginalRequestId}, 项目编码={ItemCode}, 退费数量={ReverseQty}",
            request.OriginalRequestId, request.ItemCode, request.ReverseQty);

        await ExecuteInTransactionAsync(async () =>
        {
            // 锁定原请求和退费流水
            await _limitRepository.EnsureAndLockAsync(new[]
            {
                PricingLockKeyBuilder.BuildRequestLockKey(request.OriginalRequestId),
                PricingLockKeyBuilder.BuildReverseLockKey(request.OriginalRequestId, reverseNo)
            });

            var log = await _requestLogRepository.GetByIdAsync(request.OriginalRequestId)
                ?? throw new BizException(BizErrorCode.RequestNotFound, 404, $"原请求不存在: {request.OriginalRequestId}");

            // 幂等检查：相同 ReverseNo 参数必须一致
            var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
            var sameReverseNo = reverseLogs.FirstOrDefault(r =>
                string.Equals(r.ReverseNo, request.ReverseNo, StringComparison.OrdinalIgnoreCase));
            if (sameReverseNo is not null)
            {
                if (!PricingReverseDetailSelector.IsSameReverseRequest(sameReverseNo, request))
                {
                    throw new BizException(BizErrorCode.IdempotencyConflict, 409,
                        $"ReverseNo={request.ReverseNo} 已存在，但本次冲正参数与首次请求不一致");
                }
                _logger.LogInformation("退费冲正幂等命中 原请求ID={OriginalRequestId}, 冲正流水号={ReverseNo}",
                    request.OriginalRequestId, request.ReverseNo);
                return;
            }

            // 只有已落账记录才能 reverse
            if (!IsCommittedBusinessStatus(log.BusinessStatus))
            {
                throw new BizException(BizErrorCode.ReverseNotAllowed, 409,
                    $"只有CONFIRMED或COMMITTED状态可以REVERSE, 当前: {log.BusinessStatus}");
            }

            var details = await _discountRepository.GetByRequestIdAsync(request.OriginalRequestId);
            var matchedDetails = PricingReverseDetailSelector.FilterReverseDetails(details, request);
            if (matchedDetails.Count == 0)
            {
                throw new BizException(BizErrorCode.ReverseNotAllowed, 409, "未找到可退费的原收费明细");
            }

            // 计算本次可退数量和金额
            // allOriginalQty 是原请求全部有效明细的总量（不限于本次匹配范围），用于全退判断。
            // originalQty/Amt 仅是本次匹配到的明细，用于部分退费的比例分摊和累计校验。
            var allOriginalQty = details
                .Where(d => d.Status == BusinessStatusCodes.Confirmed || d.Status == BusinessStatusCodes.Committed)
                .Sum(d => d.FinalQty ?? 0);
            var originalQty = matchedDetails.Sum(d => d.FinalQty ?? 0);
            var originalAmt = matchedDetails.Sum(d => d.FinalAmt ?? 0);
            var reverseQty = request.ReverseQty ?? originalQty;
            if (reverseQty <= 0)
            {
                throw new BizException(BizErrorCode.ReverseNotAllowed, 409, "退费数量必须大于0");
            }

            var historicalReversedQty = await _reverseHistoryReader.GetHistoricalReversedQtyAsync(request);
            var allHistoricalReversedQty = await _reverseHistoryReader.GetHistoricalReversedQtyAsync(request.OriginalRequestId);
            if (historicalReversedQty + reverseQty > originalQty)
            {
                throw new BizException(BizErrorCode.ReverseNotAllowed, 409,
                    $"原有效数量={originalQty}, 历史已退={historicalReversedQty}, 本次退费={reverseQty}");
            }

            var reverseAmt = request.ReverseAmt ??
                (originalQty == 0 ? 0 : originalAmt * reverseQty / originalQty);
            reverseAmt = PricingAmountRounder.RoundFinal(reverseAmt);
            var historicalReversedAmt = await _reverseHistoryReader.GetHistoricalReversedAmtAsync(request);
            if (historicalReversedAmt + reverseAmt > originalAmt)
            {
                throw new BizException(BizErrorCode.ReverseNotAllowed, 409,
                    $"原有效金额={originalAmt}, 历史已退={historicalReversedAmt}, 本次退费={reverseAmt}");
            }

            // 全退必须同时满足数量和金额两个维度。多明细部分退费可能数量相等但金额因四舍五入有微差，
            // 只看数量会误判为全退，必须两个维度都闭合才标记原请求为 REVERSED。
            var isFullReverse =
                allHistoricalReversedQty + reverseQty == allOriginalQty &&
                historicalReversedAmt + reverseAmt == originalAmt;

            // 主子项目结果组保护
            ValidateResultGroupIntegrity(matchedDetails, reverseLogs, reverseQty, reverseAmt, originalQty, originalAmt);

            // 推进状态或写负向占用
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
                "退费冲正成功 原请求ID={OriginalRequestId}, 退费数量={ReverseQty}, 退费金额={ReverseAmt}, 是否全退={IsFullReverse}",
                request.OriginalRequestId, reverseQty, reverseAmt, isFullReverse);
        });
    }

    /// <summary>
    /// 校验主子项目结果组内退费不超出原有效数量/金额。
    /// </summary>
    private void ValidateResultGroupIntegrity(
        IReadOnlyList<ChargeDiscountDetail> matchedDetails,
        IReadOnlyList<ChargeReverseLog> reverseLogs,
        decimal reverseQty,
        decimal reverseAmt,
        decimal originalQty,
        decimal originalAmt)
    {
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

            // 最后一组承接前面各组按比例分摊后的差额，避免 decimal 截断导致各组退费之和 != 本次总退费。
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
                throw new BizException(BizErrorCode.ReverseNotAllowed, 409,
                    $"ResultGroupNo={group.Key}, 组原有效数量={groupOriginalQty}, 组历史已退={groupHistoricalQty}, 组本次退费={groupReverseQty}");
            }

            if (groupHistoricalAmt + groupReverseAmt > groupOriginalAmt)
            {
                throw new BizException(BizErrorCode.ReverseNotAllowed, 409,
                    $"ResultGroupNo={group.Key}, 组原有效金额={groupOriginalAmt}, 组历史已退={groupHistoricalAmt}, 组本次退费={groupReverseAmt}");
            }
        }
    }

    private static bool IsCommittedBusinessStatus(string? businessStatus)
    {
        return string.Equals(businessStatus, BusinessStatusCodes.Confirmed, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(businessStatus, BusinessStatusCodes.Committed, StringComparison.OrdinalIgnoreCase);
    }

    private async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        try
        {
            await _unitOfWork.BeginAsync();
            await action();
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事务执行异常，已回滚");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
