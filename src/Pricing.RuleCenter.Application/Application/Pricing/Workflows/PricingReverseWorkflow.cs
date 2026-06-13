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
    public async Task<PricingReverseResponse> ExecuteAsync(PricingReverseRequest request)
    {
        PricingRequestGuard.EnsureReverseRequest(request);

        _logger.LogInformation(
            "退费冲正开始 原请求ID={OriginalRequestId}, 项目编码={ItemCode}, 退费数量={ReverseQty}",
            request.OriginalRequestId, request.ItemCode, request.ReverseQty);

        return await ExecuteInTransactionAsync(async () =>
        {
            // 锁定原请求和退费流水
            await LockReverseRequestAsync(request);

            var log = await GetOriginalRequestAsync(request.OriginalRequestId);

            var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
            var idempotentResponse = TryHandleIdempotentReverse(request, reverseLogs);
            if (idempotentResponse is not null)
            {
                return idempotentResponse;
            }

            EnsureReverseAllowed(log);
            var details = await _discountRepository.GetByRequestIdAsync(request.OriginalRequestId);
            var reverseContext = await BuildReverseContextAsync(request, details);
            ValidateResultGroupIntegrity(
                reverseContext.MatchedDetails,
                reverseLogs,
                reverseContext.ReverseQty,
                reverseContext.ReverseAmt,
                reverseContext.OriginalQty,
                reverseContext.OriginalAmt);

            if (reverseContext.IsFullReverse)
            {
                await MarkOriginalRequestReversedAsync(log, request.OriginalRequestId);
            }

            var reverseTime = request.ReverseTime ?? _clock.Now;
            var reverseRequestId = await _reverseLogWriter.SaveRequestLogAsync(new ReverseRequestLogSaveInput
            {
                Request = request,
                OriginalLog = log,
                MatchedDetails = reverseContext.MatchedDetails,
                ReverseQty = reverseContext.ReverseQty,
                ReverseAmt = reverseContext.ReverseAmt,
                ReverseTime = reverseTime
            });

            if (!reverseContext.IsFullReverse)
            {
                await _limitOccupyWriter.InsertNegativeAsync(new NegativeLimitOccupyInput
                {
                    Request = request,
                    MatchedDetails = reverseContext.MatchedDetails,
                    ReverseRequestId = reverseRequestId,
                    TraceId = log.TraceId,
                    ReverseQty = reverseContext.ReverseQty,
                    ReverseAmt = reverseContext.ReverseAmt,
                    ReverseTime = reverseTime
                });
            }

            await _reverseLogWriter.SaveReverseLogAsync(new ReverseLogSaveInput
            {
                Request = request,
                OriginalLog = log,
                MatchedDetails = reverseContext.MatchedDetails,
                ReverseRequestId = reverseRequestId,
                ReverseQty = reverseContext.ReverseQty,
                ReverseAmt = reverseContext.ReverseAmt,
                ReverseTime = reverseTime,
                IsFullReverse = reverseContext.IsFullReverse
            });

            _logger.LogInformation(
                "退费冲正成功 原请求ID={OriginalRequestId}, 退费数量={ReverseQty}, 退费金额={ReverseAmt}, 是否全退={IsFullReverse}",
                request.OriginalRequestId,
                reverseContext.ReverseQty,
                reverseContext.ReverseAmt,
                reverseContext.IsFullReverse);

            return BuildResponse(request, reverseRequestId, reverseContext.IsFullReverse);
        });
    }

    private async Task LockReverseRequestAsync(PricingReverseRequest request)
    {
        await _limitRepository.EnsureAndLockAsync(new[]
        {
            PricingLockKeyBuilder.BuildRequestLockKey(request.OriginalRequestId),
            PricingLockKeyBuilder.BuildReverseLockKey(request.OriginalRequestId, request.ReverseNo!)
        });
    }

    private async Task<ChargeRequest> GetOriginalRequestAsync(long originalRequestId)
    {
        return await _requestLogRepository.GetByIdAsync(originalRequestId)
            ?? throw new BizException(BizErrorCode.RequestNotFound, 404, $"原请求不存在: {originalRequestId}");
    }

    private PricingReverseResponse? TryHandleIdempotentReverse(
        PricingReverseRequest request,
        IReadOnlyList<ChargeReverseLog> reverseLogs)
    {
        var sameReverseNo = reverseLogs.FirstOrDefault(reverseLog =>
            string.Equals(reverseLog.ReverseNo, request.ReverseNo, StringComparison.OrdinalIgnoreCase));
        if (sameReverseNo is null)
        {
            return null;
        }

        if (!PricingReverseDetailSelector.IsSameReverseRequest(sameReverseNo, request))
        {
            throw new BizException(
                BizErrorCode.IdempotencyConflict,
                409,
                $"ReverseNo={request.ReverseNo} 已存在，但本次冲正参数与首次请求不一致");
        }

        _logger.LogInformation(
            "退费冲正幂等命中 原请求ID={OriginalRequestId}, 冲正流水号={ReverseNo}",
            request.OriginalRequestId,
            request.ReverseNo);
        return BuildResponse(
            request,
            sameReverseNo.ReverseRequestId ?? 0,
            string.Equals(sameReverseNo.ReverseType, "FULL", StringComparison.OrdinalIgnoreCase));
    }

    private static void EnsureReverseAllowed(ChargeRequest log)
    {
        if (!IsCommittedBusinessStatus(log.BusinessStatus))
        {
            throw new BizException(
                BizErrorCode.ReverseNotAllowed,
                409,
                $"只有CONFIRMED或COMMITTED状态可以REVERSE, 当前: {log.BusinessStatus}");
        }
    }

    private async Task<ReverseExecutionContext> BuildReverseContextAsync(
        PricingReverseRequest request,
        IReadOnlyList<ChargeDiscountDetail> details)
    {
        var matchedDetails = PricingReverseDetailSelector.FilterReverseDetails(details, request);
        if (matchedDetails.Count == 0)
        {
            throw new BizException(BizErrorCode.ReverseNotAllowed, 409, "未找到可退费的原收费明细");
        }

        var allOriginalQty = details
            .Where(detail => detail.Status == BusinessStatusCodes.Confirmed || detail.Status == BusinessStatusCodes.Committed)
            .Sum(detail => detail.FinalQty ?? 0);
        var originalQty = matchedDetails.Sum(detail => detail.FinalQty ?? 0);
        var originalAmt = matchedDetails.Sum(detail => detail.FinalAmt ?? 0);
        var reverseQty = request.ReverseQty ?? originalQty;
        if (reverseQty <= 0)
        {
            throw new BizException(BizErrorCode.ReverseNotAllowed, 409, "退费数量必须大于0");
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

        return new ReverseExecutionContext
        {
            MatchedDetails = matchedDetails,
            OriginalQty = originalQty,
            OriginalAmt = originalAmt,
            ReverseQty = reverseQty,
            ReverseAmt = reverseAmt,
            IsFullReverse =
                allHistoricalReversedQty + reverseQty == allOriginalQty &&
                historicalReversedAmt + reverseAmt == originalAmt
        };
    }

    private async Task MarkOriginalRequestReversedAsync(ChargeRequest log, long originalRequestId)
    {
        log.MarkReversed(_clock.Now);
        await _requestLogRepository.UpdateAsync(log);
        await _discountRepository.UpdateStatusByRequestIdAsync(originalRequestId, OccupyStatusCodes.Reversed);
        await _limitRepository.UpdateStatusByRequestIdAsync(originalRequestId, OccupyStatusCodes.Reversed);
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

    private static PricingReverseResponse BuildResponse(
        PricingReverseRequest request,
        long reverseRequestId,
        bool isFullReverse)
    {
        return new PricingReverseResponse
        {
            OriginalRequestId = request.OriginalRequestId,
            ReverseNo = request.ReverseNo!.Trim(),
            ReverseRequestId = reverseRequestId,
            IsFullReverse = isFullReverse,
            BusinessStatus = BusinessStatusCodes.Reversed,
            NextAction = PricingNextActionCodes.NoFurtherAction
        };
    }

    private async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        try
        {
            await _unitOfWork.BeginAsync();
            var result = await action();
            await _unitOfWork.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事务执行异常，已回滚");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private sealed record ReverseExecutionContext
    {
        public IReadOnlyList<ChargeDiscountDetail> MatchedDetails { get; init; } =
            Array.Empty<ChargeDiscountDetail>();

        public decimal OriginalQty { get; init; }

        public decimal OriginalAmt { get; init; }

        public decimal ReverseQty { get; init; }

        public decimal ReverseAmt { get; init; }

        public bool IsFullReverse { get; init; }
    }
}
