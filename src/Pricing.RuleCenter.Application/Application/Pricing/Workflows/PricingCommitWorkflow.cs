using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Microsoft.Extensions.Options;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 落账提交工作流：confirm 阶段的待确认占用转为正式生效。
/// </summary>
/// <remarks>
/// commit 不重新计价，只基于 confirm 保存的折价明细和 HIS 回传的 ActualItems 做对账后推进状态。
/// 重复提交已确认记录按幂等成功处理。
/// </remarks>
public sealed class PricingCommitWorkflow
{
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IChargeDiscountDetailRepository _discountRepository;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PricingOptions _options;
    private readonly IClock _clock;
    private readonly ILogger<PricingCommitWorkflow> _logger;

    public PricingCommitWorkflow(
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository,
        IUnitOfWork unitOfWork,
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger<PricingCommitWorkflow> logger)
    {
        _requestLogRepository = requestLogRepository;
        _discountRepository = discountRepository;
        _limitRepository = limitRepository;
        _unitOfWork = unitOfWork;
        _options = options.Value;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行落账提交：校验 → 锁定 → 状态/过期校验 → 对账 → 推进状态。
    /// </summary>
    public async Task ExecuteAsync(PricingCommitRequest request)
    {
        PricingRequestGuard.EnsureCommitRequest(request);

        _logger.LogInformation(
            "落账提交开始 请求ID={RequestId}, 收费单号={ChargeNo}, 提交流水号={CommitNo}",
            request.RequestId, request.ChargeNo, request.CommitNo);

        await ExecuteInTransactionAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[] { PricingLockKeyBuilder.BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new BizException(BizErrorCode.RequestNotFound, 404, $"请求不存在: {request.RequestId}");

            // 幂等：重复 commit 允许返回成功。但如果渠道补传了实际落账明细，仍做轻量对账，
            // 便于发现重复回调中携带的落账事实与首次结果不一致。
            if (log.BusinessStatus == BusinessStatusCodes.Confirmed || log.BusinessStatus == BusinessStatusCodes.Committed)
            {
                if ((request.ActualItems?.Count ?? 0) > 0 || request.ActualTotalAmount.HasValue)
                {
                    var confirmedDetails = await _discountRepository.GetByRequestIdAsync(request.RequestId);
                    PricingCommitActualValidator.Validate(request, confirmedDetails, requireActualItems: false);
                }
                _logger.LogInformation("落账提交幂等命中 请求ID={RequestId}, 当前状态={Status}", request.RequestId, log.BusinessStatus);
                return;
            }

            // 只有 CONFIRM_PENDING 且未过期才能提交
            if (log.BusinessStatus != BusinessStatusCodes.ConfirmPending)
            {
                throw new BizException(BizErrorCode.RequestStatusNotAllowed, 409,
                    $"只有CONFIRM_PENDING状态可以COMMIT, 当前: {log.BusinessStatus}");
            }

            if (_clock.Now > log.RequestAt.AddMinutes(_options.ConfirmExpireMinutes))
            {
                throw new BizException(BizErrorCode.RequestStatusNotAllowed, 409, "确认计价结果已过期，请重新 confirm");
            }

            var details = await _discountRepository.GetByRequestIdAsync(request.RequestId);
            PricingCommitActualValidator.Validate(request, details, requireActualItems: true);

            // 推进状态：请求日志 + 折价明细 + 限额占用一起变更
            log.ChargeNo = request.ChargeNo ?? log.ChargeNo;
            log.MarkCommitted(_clock.Now);
            await _requestLogRepository.UpdateAsync(log);
            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Confirmed);
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Confirmed);

            _logger.LogInformation(
                "落账提交成功 请求ID={RequestId}, 收费单号={ChargeNo}, 提交流水号={CommitNo}",
                request.RequestId, request.ChargeNo, request.CommitNo);
        });
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
