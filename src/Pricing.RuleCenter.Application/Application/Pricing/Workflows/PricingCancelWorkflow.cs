using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 取消确认工作流：释放未落账 confirm 产生的待确认额度。
/// </summary>
/// <remarks>
/// 只允许 CONFIRM_PENDING 状态。重复取消或已过期清理的请求按幂等成功处理。
/// </remarks>
public sealed class PricingCancelWorkflow
{
    private readonly IChargeRequestLogRepository _requestLogRepository;
    private readonly IChargeDiscountDetailRepository _discountRepository;
    private readonly ILimitOccupyRepository _limitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly ILogger<PricingCancelWorkflow> _logger;

    public PricingCancelWorkflow(
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository,
        IUnitOfWork unitOfWork,
        IClock clock,
        ILogger<PricingCancelWorkflow> logger)
    {
        _requestLogRepository = requestLogRepository;
        _discountRepository = discountRepository;
        _limitRepository = limitRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行取消确认：校验 → 锁定 → 状态校验 → 释放占用。
    /// </summary>
    public async Task<PricingCancelResponse> ExecuteAsync(PricingCancelRequest request)
    {
        PricingRequestGuard.EnsureCancelRequest(request);

        _logger.LogInformation(
            "取消确认开始 请求ID={RequestId}, 取消流水号={CancelNo}, 取消人={CancelledBy}",
            request.RequestId, request.CancelNo, request.CancelledBy);

        return await ExecuteInTransactionAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[] { PricingLockKeyBuilder.BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new BizException(BizErrorCode.RequestNotFound, 404, $"请求不存在: {request.RequestId}");

            // 幂等：重复取消和已过期清理后不再释放额度
            if (log.BusinessStatus == BusinessStatusCodes.Cancelled || log.BusinessStatus == BusinessStatusCodes.Expired)
            {
                _logger.LogInformation("取消确认幂等命中 请求ID={RequestId}, 当前状态={Status}", request.RequestId, log.BusinessStatus);
                return BuildResponse(log);
            }

            if (log.BusinessStatus != BusinessStatusCodes.ConfirmPending)
            {
                throw new BizException(BizErrorCode.RequestStatusNotAllowed, 409,
                    $"只有CONFIRM_PENDING状态可以CANCEL, 当前: {log.BusinessStatus}");
            }

            // 请求日志、折价明细、限额占用三表状态必须一起变更，保证追溯和额度口径一致。
            log.MarkCancelled(_clock.Now);
            await _requestLogRepository.UpdateAsync(log);
            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Cancelled);
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Cancelled);

            _logger.LogInformation("取消确认成功 请求ID={RequestId}, 取消流水号={CancelNo}", request.RequestId, request.CancelNo);
            return BuildResponse(log);
        });
    }

    private static PricingCancelResponse BuildResponse(ChargeRequest log)
    {
        return new PricingCancelResponse
        {
            RequestId = log.RequestId,
            BusinessStatus = log.BusinessStatus,
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
}
