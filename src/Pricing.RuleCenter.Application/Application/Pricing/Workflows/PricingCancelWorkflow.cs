using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 取消确认计价工作流，负责释放未落账 confirm 产生的待确认额度。
/// </summary>
/// <remarks>
/// <para>
/// cancel 只允许处理 <c>CONFIRM_PENDING</c> 状态的请求。HIS 已经落账的记录不能 cancel，
/// 必须通过 reverse 走退费/冲销链路，避免账务成功但规则中心释放额度。
/// </para>
/// <para>
/// 重复 cancel 或后台已过期清理的请求按幂等成功处理。
/// </para>
/// <para>
/// cancel 的业务含义是“确认计价未被 HIS 采用”。常见来源包括用户取消收费、HIS 写收费明细失败、
/// 支付失败或渠道超时放弃。它释放的是 confirm 阶段的保护占用，不会产生退费流水。
/// </para>
/// </remarks>
public sealed class PricingCancelWorkflow
{
    /// <summary>
    /// 请求日志仓储，用于定位待取消的 confirm 请求。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;

    /// <summary>
    /// 折价明细仓储，用于同步取消折价结果状态。
    /// </summary>
    private readonly IChargeDiscountDetailRepository _discountRepository;

    /// <summary>
    /// 限额占用仓储，用于锁定请求维度并释放待确认占用。
    /// </summary>
    private readonly ILimitOccupyRepository _limitRepository;

    /// <summary>
    /// 事务执行器，保证请求日志、折价明细和限额占用一起回滚或提交。
    /// </summary>
    private readonly PricingTransactionExecutor _transactionExecutor;

    /// <summary>
    /// 统一时钟，用于记录取消响应时间。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// cancel 工作流日志对象。
    /// </summary>
    private readonly ILogger<PricingCancelWorkflow> _logger;

    /// <summary>
    /// 初始化取消确认计价 workflow。
    /// </summary>
    /// <param name="requestLogRepository">请求日志仓储。</param>
    /// <param name="discountRepository">折价明细仓储。</param>
    /// <param name="limitRepository">限额占用仓储。</param>
    /// <param name="transactionExecutor">事务执行器。</param>
    /// <param name="clock">技术时间提供者。</param>
    /// <param name="logger">日志组件。</param>
    public PricingCancelWorkflow(
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository,
        PricingTransactionExecutor transactionExecutor,
        IClock clock,
        ILogger<PricingCancelWorkflow> logger)
    {
        _requestLogRepository = requestLogRepository;
        _discountRepository = discountRepository;
        _limitRepository = limitRepository;
        _transactionExecutor = transactionExecutor;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行取消确认计价。
    /// </summary>
    /// <param name="request">取消请求。</param>
    /// <remarks>
    /// 该方法对应 <c>/api/pricing/calculate/cancel</c>。调用方应只在 HIS 未成功落账时调用。
    /// 如果 HIS 已经写入收费明细，即使后续要撤销，也必须走 reverse，以便保留退费审计和额度返还依据。
    /// </remarks>
    public async Task ExecuteAsync(PricingCancelRequest request)
    {
        // ========== 第一阶段：请求结构校验 ==========
        // cancel 必须携带 requestId，不能仅凭业务号取消，避免误释放其他收费动作的额度。
        // RequestId 来自 confirm 响应，是请求日志、折价明细和限额占用之间最稳定的串联键。
        PricingRequestGuard.EnsureCancelRequest(request);

        _logger.LogInformation(
            "取消确认开始 请求ID={RequestId}, 取消流水号={CancelNo}, 取消人={CancelledBy}, 取消时间={CancelledAt}, 取消原因={CancelReason}",
            request.RequestId, request.CancelNo, request.CancelledBy, request.CancelledAt, request.CancelReason);

        await _transactionExecutor.ExecuteAsync(async () =>
        {
            // ========== 第二阶段：锁定请求维度 ==========
            // 与 commit 使用相同 requestId 锁，防止 HIS 成功落账和取消操作并发竞争。
            // 如果不加同一把锁，可能出现 HIS 已落账但规则中心又释放额度的资金口径错误。
            await _limitRepository.EnsureAndLockAsync(new[] { PricingLockKeyBuilder.BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new BizException(
                    BizErrorCode.RequestNotFound,
                    404,
                    $"请求不存在: {request.RequestId}");

            if (log.BusinessStatus == BusinessStatusCodes.Cancelled || log.BusinessStatus == BusinessStatusCodes.Expired)
            {
                // 重复取消和后台过期清理后的取消都不会再次释放额度，直接按幂等成功返回。
                // 对渠道来说，重试 cancel 的目标是确认保护占用不再有效，而不是重复写释放记录。
                _logger.LogInformation(
                    "取消确认幂等命中 请求ID={RequestId}, 当前状态={Status}",
                    request.RequestId, log.BusinessStatus);
                return;
            }

            // ========== 第三阶段：状态校验 ==========
            // 已确认落账的记录必须 reverse，不能 cancel，否则会破坏 HIS 账务和规则中心额度一致性。
            // 其他状态也不能取消：SIMULATED 没有正式占用，REVERSED 已经进入退费链路。
            if (log.BusinessStatus != BusinessStatusCodes.ConfirmPending)
            {
                _logger.LogWarning(
                    "取消确认状态校验失败 请求ID={RequestId}, 当前状态={Status}, 期望状态=CONFIRM_PENDING",
                    request.RequestId, log.BusinessStatus);
                throw new BizException(
                    BizErrorCode.RequestStatusNotAllowed,
                    409,
                    $"只有CONFIRM_PENDING状态可以CANCEL, 当前: {log.BusinessStatus}");
            }

            // ========== 第四阶段：同步释放待确认占用 ==========
            // 三张表状态一起变更，保证追溯、折价明细和额度口径一致。
            // 这里不是物理删除占用，而是推进业务状态，方便后续对账识别“曾经确认但未落账”的收费动作。
            log.BusinessStatus = BusinessStatusCodes.Cancelled;
            log.ResponseAt = _clock.Now;
            await _requestLogRepository.UpdateAsync(log);

            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Cancelled);
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Cancelled);

            _logger.LogInformation(
                "取消确认成功 请求ID={RequestId}, 来源系统={SourceSystem}, 项目编码={ItemCode}, 取消流水号={CancelNo}, 取消人={CancelledBy}, 取消时间={CancelledAt}, 限额已释放",
                request.RequestId, log.SourceSystem, log.ItemCode,
                request.CancelNo, request.CancelledBy, request.CancelledAt);
        });
    }
}
