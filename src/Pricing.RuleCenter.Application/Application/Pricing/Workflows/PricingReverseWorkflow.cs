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
/// 退费冲正工作流，负责对已落账的计价请求执行退费、冲销和额度释放。
/// </summary>
/// <remarks>
/// <para>
/// reverse 只处理已经 commit/confirmed 的记录。它不重新执行原始计价规则，而是基于原收费明细、
/// 历史退费记录和本次退费请求计算可退数量/金额，并写入冲正日志。
/// </para>
/// <para>
/// 部分退费必须保证“本次退费 + 历史已退”不超过原有效收费，且主子项目同组结果需要按 resultGroupNo
/// 做组内保护，避免只退主项或只退子项造成金额和额度口径不一致。
/// </para>
/// <para>
/// reverse 与 cancel 的边界非常严格：cancel 处理未落账的保护占用；reverse 处理已落账的账务事实。
/// reverse 释放额度时必须保留冲正流水和负向占用，不能简单把原占用删除，否则后续对账无法解释额度变化。
/// </para>
/// <para>
/// 本 workflow 以 <c>OriginalRequestId + ReverseNo</c> 作为退费幂等边界。HIS 重试同一笔退费时返回成功；
/// 如果同一个 ReverseNo 携带不同项目、数量、金额或片段条件，则视为幂等冲突。
/// </para>
/// </remarks>
public sealed class PricingReverseWorkflow
{
    /// <summary>
    /// 请求日志仓储，用于读取原始已落账请求并在全退时推进状态。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;

    /// <summary>
    /// 折价明细仓储，用于定位可退费明细和原计价结果。
    /// </summary>
    private readonly IChargeDiscountDetailRepository _discountRepository;

    /// <summary>
    /// 限额占用仓储，用于锁定原请求和退费流水维度。
    /// </summary>
    private readonly ILimitOccupyRepository _limitRepository;

    /// <summary>
    /// 冲正日志仓储，用于读取历史退费记录并做 ReverseNo 幂等判断。
    /// </summary>
    private readonly IChargeReverseLogRepository _reverseLogRepository;

    /// <summary>
    /// 冲正请求和明细日志写入器。
    /// </summary>
    private readonly PricingReverseLogWriter _reverseLogWriter;

    /// <summary>
    /// 限额占用写入器，用于部分退费时写入负向占用释放额度。
    /// </summary>
    private readonly PricingLimitOccupyWriter _limitOccupyWriter;

    /// <summary>
    /// 事务执行器，保证冲正日志和额度释放原子提交。
    /// </summary>
    private readonly PricingTransactionExecutor _transactionExecutor;

    /// <summary>
    /// 历史冲正读取器，用于汇总已退数量和已退金额。
    /// </summary>
    private readonly PricingReverseHistoryReader _reverseHistoryReader;

    /// <summary>
    /// 统一时钟，用于默认退费时间和状态更新时间。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// reverse 工作流日志对象。
    /// </summary>
    private readonly ILogger<PricingReverseWorkflow> _logger;

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
        ILogger<PricingReverseWorkflow> logger)
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
    /// <remarks>
    /// 该方法对应 <c>/api/pricing/calculate/reverse</c>。调用方可指定收费明细、项目编码、partSeq 和退费数量；
    /// 如果未指定数量，服务层按匹配到的原明细全退处理。部分退费场景必须依赖历史冲正累计防止超退。
    /// </remarks>
    public async Task ExecuteAsync(PricingReverseRequest request)
    {
        // ========== 第一阶段：请求结构校验 ==========
        // ReverseNo 是退费幂等键，缺失时无法区分“重试同一次退费”和“新的退费动作”。
        // OriginalRequestId 则把退费限定在某一次已确认计价结果内，避免跨收费单误退。
        PricingRequestGuard.EnsureReverseRequest(request);
        var reverseNo = request.ReverseNo!;

        _logger.LogInformation(
            "退费冲正开始 原请求ID={OriginalRequestId}, 项目编码={ItemCode}, 退费数量={ReverseQty}",
            request.OriginalRequestId, request.ItemCode, request.ReverseQty);

        await _transactionExecutor.ExecuteAsync(async () =>
        {
            // ========== 第二阶段：锁定原请求和退费流水 ==========
            // 原请求锁防止 commit/cancel/reverse 并发推进；退费流水锁防止相同 ReverseNo 并发重复写入。
            // 两把锁都需要：原请求锁保护状态机，退费流水锁保护同一笔退费的幂等事实。
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
                // 相同 ReverseNo 代表 HIS 重试同一笔退费；参数必须一致，否则按幂等冲突处理。
                // 不能用“后到请求覆盖前一次退费”，否则会让历史已退数量和金额失去审计可信度。
                if (!PricingReverseDetailSelector.IsSameReverseRequest(sameReverseNo, request))
                {
                    throw new BizException(
                        BizErrorCode.IdempotencyConflict,
                        409,
                        $"ReverseNo={request.ReverseNo} 已存在，但本次冲正参数与首次请求不一致");
                }

                _logger.LogInformation(
                    "退费冲正幂等命中 原请求ID={OriginalRequestId}, 冲正流水号={ReverseNo}",
                    request.OriginalRequestId, request.ReverseNo);
                return;
            }

            // ========== 第三阶段：状态和可退明细校验 ==========
            // 只有已落账记录才允许 reverse；未 commit 的请求应通过 cancel 或过期清理释放额度。
            // 这条边界保证规则中心的额度释放与 HIS 账务事实一致。
            if (!IsCommittedBusinessStatus(log.BusinessStatus))
            {
                _logger.LogWarning(
                    "退费冲正状态校验失败 原请求ID={OriginalRequestId}, 当前状态={Status}, 期望状态=CONFIRMED/COMMITTED",
                    request.OriginalRequestId, log.BusinessStatus);
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    $"只有CONFIRMED或COMMITTED状态可以REVERSE, 当前: {log.BusinessStatus}");
            }

            var details = await _discountRepository.GetByRequestIdAsync(request.OriginalRequestId);
            // 允许 HIS 按收费明细、项目编码或 partSeq 定位部分退费；没有筛选条件时视为对匹配范围做全退。
            // 过滤结果必须来自原 confirm/commit 保存的折价明细，不能根据当前规则重新推导可退项目。
            var matchedDetails = PricingReverseDetailSelector.FilterReverseDetails(details, request);
            if (matchedDetails.Count == 0)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    "未找到可退费的原收费明细");
            }

            // ========== 第四阶段：计算本次可退数量和金额 ==========
            // 默认退费数量为匹配明细全退；显式传 ReverseQty 时按原金额比例折算退费金额。
            // 退费金额最终仍按统一金额规则保留 2 位小数，避免 HIS 和规则中心出现分位差。
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
            // 部分退费必须累计历史已退数量。只校验本次退费小于原数量是不够的，多次部分退费会突破原有效收费。
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
            // 金额也要做历史累计校验，避免按数量比例四舍五入后多次退费超过原有效金额。
            if (historicalReversedAmt + reverseAmt > originalAmt)
            {
                throw new BizException(
                    BizErrorCode.ReverseNotAllowed,
                    409,
                    $"原有效金额={originalAmt}, 历史已退={historicalReversedAmt}, 本次退费={reverseAmt}");
            }

            // 全退判断同时看原请求整体数量和匹配明细金额，避免多明细部分退费误判为整单冲正。
            var isFullReverse =
                allHistoricalReversedQty + reverseQty == allOriginalQty &&
                historicalReversedAmt + reverseAmt == originalAmt;

            // ========== 第五阶段：主子项目结果组保护 ==========
            // 同一 resultGroupNo 通常代表主项目和子项加收的原子结果，退费分摊不能突破组内原数量/金额。
            // 这能防止只退主项目但保留子项，或只退子项导致主子项目组金额不闭合。
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
                    // 最后一组承接前面比例分摊后的差额，避免 decimal 除法造成总退费数量/金额不闭合。
                    // 这样所有组的退费数量/金额加总仍等于本次请求的 reverseQty/reverseAmt。
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

            // ========== 第六阶段：推进原请求状态或写负向占用 ==========
            // 全退直接把原请求及占用标记为 REVERSED；部分退费通过负向占用抵扣额度，保留原请求有效状态。
            // 保留原记录再插入负向占用，比直接修改原占用更利于追溯净占用是如何变化的。
            if (isFullReverse)
            {
                log.BusinessStatus = BusinessStatusCodes.Reversed;
                log.ResponseAt = _clock.Now;
                await _requestLogRepository.UpdateAsync(log);

                await _discountRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, OccupyStatusCodes.Reversed);
                await _limitRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, OccupyStatusCodes.Reversed);
            }

            var reverseTime = request.ReverseTime ?? _clock.Now;
            // 冲正请求日志先落库，后续负向占用和冲正明细都引用该 reverseRequestId，形成完整审计链。
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

            // ========== 第七阶段：落冲正明细日志 ==========
            // 冲正日志是后续幂等、累计已退数量/金额和审计追溯的事实来源。
            // 后续再收到同一 ReverseNo 或新的部分退费时，都以这里保存的明细作为历史事实。
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
                "退费冲正成功 原请求ID={OriginalRequestId}, 项目编码={ItemCode}, 退费数量={ReverseQty}, 退费金额={ReverseAmt}, 是否全退={IsFullReverse}",
                request.OriginalRequestId, matchedDetails.FirstOrDefault()?.ItemCode,
                reverseQty, reverseAmt, isFullReverse);
        });
    }

    /// <summary>
    /// 判断原请求状态是否已经代表 HIS 落账成功。
    /// </summary>
    /// <param name="businessStatus">请求日志中的业务状态。</param>
    /// <returns>状态为 CONFIRMED 或兼容旧值 COMMITTED 时返回 true。</returns>
    /// <remarks>
    /// 退费只允许针对已落账事实执行。这里保留 <c>COMMITTED</c> 兼容，是为了历史数据或旧版本状态命名
    /// 不阻断正常退费；新增状态仍应统一使用 <c>CONFIRMED</c>。
    /// </remarks>
    private static bool IsCommittedBusinessStatus(string? businessStatus)
    {
        // 兼容旧命名 COMMITTED 和当前 CONFIRMED，避免历史数据状态差异阻断正常退费。
        return string.Equals(businessStatus, BusinessStatusCodes.Confirmed, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(businessStatus, BusinessStatusCodes.Committed, StringComparison.OrdinalIgnoreCase);
    }
}
