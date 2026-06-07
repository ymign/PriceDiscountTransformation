using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Application.Pricing.Validation;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 落账提交 workflow。
/// </summary>
public sealed class PricingCommitWorkflow
{
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IChargeDiscountDetailRepository _discountRepository;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly PricingTransactionExecutor _transactionExecutor;
    private readonly PricingOptions _options;
    private readonly IClock _clock;
    private readonly ILogger _logger;

    /// <summary>
    /// 初始化落账提交 workflow。
    /// </summary>
    /// <param name="requestLogRepository">请求日志仓储。</param>
    /// <param name="discountRepository">折价明细仓储。</param>
    /// <param name="limitRepository">限额占用仓储。</param>
    /// <param name="transactionExecutor">事务执行器。</param>
    /// <param name="options">计价配置。</param>
    /// <param name="clock">技术时间提供者。</param>
    /// <param name="logger">日志组件。</param>
    public PricingCommitWorkflow(
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository,
        PricingTransactionExecutor transactionExecutor,
        PricingOptions options,
        IClock clock,
        ILogger logger)
    {
        _requestLogRepository = requestLogRepository;
        _discountRepository = discountRepository;
        _limitRepository = limitRepository;
        _transactionExecutor = transactionExecutor;
        _options = options;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行落账提交。
    /// </summary>
    /// <param name="request">提交请求。</param>
    public async Task ExecuteAsync(PricingCommitRequest request)
    {
        PricingRequestGuard.EnsureCommitRequest(request);

        _logger.LogInformation(
            "COMMIT 开始 RequestId={RequestId}, ChargeNo={ChargeNo}",
            request.RequestId, request.ChargeNo);

        await _transactionExecutor.ExecuteAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[] { PricingLockKeyBuilder.BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new BizException(
                    BizErrorCode.RequestNotFound,
                    404,
                    $"请求不存在: {request.RequestId}");

            if (log.BusinessStatus == BusinessStatusCodes.Confirmed || log.BusinessStatus == BusinessStatusCodes.Committed)
            {
                if ((request.ActualItems?.Count ?? 0) > 0 || request.ActualTotalAmount.HasValue)
                {
                    var confirmedDetails = await _discountRepository.GetByRequestIdAsync(request.RequestId);
                    PricingCommitActualValidator.Validate(request, confirmedDetails, requireActualItems: false);
                }

                _logger.LogInformation(
                    "COMMIT 幂等命中 RequestId={RequestId}, 当前状态={Status}",
                    request.RequestId, log.BusinessStatus);
                return;
            }

            if (log.BusinessStatus != BusinessStatusCodes.ConfirmPending)
            {
                _logger.LogWarning(
                    "COMMIT 状态校验失败 RequestId={RequestId}, 当前状态={Status}, 期望=CONFIRM_PENDING",
                    request.RequestId, log.BusinessStatus);
                throw new BizException(
                    BizErrorCode.RequestStatusNotAllowed,
                    409,
                    $"只有CONFIRM_PENDING状态可以COMMIT, 当前: {log.BusinessStatus}");
            }

            if (_clock.Now > log.RequestAt.AddMinutes(_options.ConfirmExpireMinutes))
            {
                _logger.LogWarning(
                    "COMMIT 已过期 RequestId={RequestId}, RequestAt={RequestAt}, 过期分钟数={ExpireMinutes}",
                    request.RequestId, log.RequestAt, _options.ConfirmExpireMinutes);
                throw new BizException(
                    BizErrorCode.RequestStatusNotAllowed,
                    409,
                    "确认计价结果已过期，请重新 confirm");
            }

            var details = await _discountRepository.GetByRequestIdAsync(request.RequestId);
            PricingCommitActualValidator.Validate(request, details, requireActualItems: true);

            log.BusinessStatus = BusinessStatusCodes.Confirmed;
            log.ChargeNo = request.ChargeNo ?? log.ChargeNo;
            log.ResponseAt = _clock.Now;
            await _requestLogRepository.UpdateAsync(log);

            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Confirmed);
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Confirmed);

            _logger.LogInformation(
                "COMMIT 成功 RequestId={RequestId}, SourceSystem={SourceSystem}, ItemCode={ItemCode}, ChargeNo={ChargeNo}",
                request.RequestId, log.SourceSystem, log.ItemCode, log.ChargeNo);
        });
    }
}
