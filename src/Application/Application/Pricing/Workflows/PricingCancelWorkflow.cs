using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 取消确认计价 workflow。
/// </summary>
public sealed class PricingCancelWorkflow
{
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IChargeDiscountDetailRepository _discountRepository;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly PricingTransactionExecutor _transactionExecutor;
    private readonly IClock _clock;
    private readonly ILogger _logger;

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
        ILogger logger)
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
    public async Task ExecuteAsync(PricingCancelRequest request)
    {
        PricingRequestGuard.EnsureCancelRequest(request);

        _logger.LogInformation(
            "CANCEL 开始 RequestId={RequestId}",
            request.RequestId);

        await _transactionExecutor.ExecuteAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[] { PricingLockKeyBuilder.BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new BizException(
                    BizErrorCode.RequestNotFound,
                    404,
                    $"请求不存在: {request.RequestId}");

            if (log.BusinessStatus == BusinessStatusCodes.Cancelled || log.BusinessStatus == BusinessStatusCodes.Expired)
            {
                _logger.LogInformation(
                    "CANCEL 幂等命中 RequestId={RequestId}, 当前状态={Status}",
                    request.RequestId, log.BusinessStatus);
                return;
            }

            if (log.BusinessStatus != BusinessStatusCodes.ConfirmPending)
            {
                _logger.LogWarning(
                    "CANCEL 状态校验失败 RequestId={RequestId}, 当前状态={Status}, 期望=CONFIRM_PENDING",
                    request.RequestId, log.BusinessStatus);
                throw new BizException(
                    BizErrorCode.RequestStatusNotAllowed,
                    409,
                    $"只有CONFIRM_PENDING状态可以CANCEL, 当前: {log.BusinessStatus}");
            }

            log.BusinessStatus = BusinessStatusCodes.Cancelled;
            log.ResponseAt = _clock.Now;
            await _requestLogRepository.UpdateAsync(log);

            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Cancelled);
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Cancelled);

            _logger.LogInformation(
                "CANCEL 成功 RequestId={RequestId}, SourceSystem={SourceSystem}, ItemCode={ItemCode}, 限额已释放",
                request.RequestId, log.SourceSystem, log.ItemCode);
        });
    }
}
