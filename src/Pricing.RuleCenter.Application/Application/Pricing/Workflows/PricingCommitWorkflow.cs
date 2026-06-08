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
/// 落账提交工作流，负责把 confirm 阶段的待确认占用转为正式生效。
/// </summary>
/// <remarks>
/// <para>
/// commit 只处理 HIS 已经成功落账的记录。它不会重新计价，也不会重新占额，
/// 只推进请求日志、折价明细和限额占用状态，确保规则中心和 HIS 账务状态一致。
/// </para>
/// <para>
/// 当前允许 <c>CONFIRM_PENDING -> CONFIRMED</c>，重复提交已确认记录按幂等成功处理。
/// </para>
/// </remarks>
public sealed class PricingCommitWorkflow
{
    /// <summary>
    /// 请求日志仓储，用于定位 confirm 阶段生成的请求记录。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;

    /// <summary>
    /// 折价明细仓储，用于校验 HIS 实际落账明细并推进明细状态。
    /// </summary>
    private readonly IChargeDiscountDetailRepository _discountRepository;

    /// <summary>
    /// 限额占用仓储，用于加锁并将占用从待确认推进为正式确认。
    /// </summary>
    private readonly ILimitOccupyRepository _limitRepository;

    /// <summary>
    /// 事务执行器，保证请求日志、折价明细和限额占用状态一致提交。
    /// </summary>
    private readonly PricingTransactionExecutor _transactionExecutor;

    /// <summary>
    /// 计价配置，主要用于判断 confirm 保护期是否已过期。
    /// </summary>
    private readonly PricingOptions _options;

    /// <summary>
    /// 统一时钟，用于过期判断和响应时间写入。
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// commit 工作流日志对象。
    /// </summary>
    private readonly ILogger<PricingCommitWorkflow> _logger;

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
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger<PricingCommitWorkflow> logger)
    {
        _requestLogRepository = requestLogRepository;
        _discountRepository = discountRepository;
        _limitRepository = limitRepository;
        _transactionExecutor = transactionExecutor;
        _options = options.Value;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// 执行落账提交。
    /// </summary>
    /// <param name="request">提交请求。</param>
    public async Task ExecuteAsync(PricingCommitRequest request)
    {
        // ========== 第一阶段：请求结构校验 ==========
        // commit 必须引用 confirm 返回的 requestId，否则无法保证与已占用额度一一对应。
        PricingRequestGuard.EnsureCommitRequest(request);

        _logger.LogInformation(
            "落账提交开始 请求ID={RequestId}, 收费单号={ChargeNo}, 提交流水号={CommitNo}, 提交人={CommittedBy}, 提交时间={CommittedAt}",
            request.RequestId, request.ChargeNo, request.CommitNo, request.CommittedBy, request.CommittedAt);

        await _transactionExecutor.ExecuteAsync(async () =>
        {
            // ========== 第二阶段：锁定请求维度 ==========
            // commit/cancel/reverse 都以 requestId 为并发边界，避免同一确认记录被不同操作同时推进状态。
            await _limitRepository.EnsureAndLockAsync(new[] { PricingLockKeyBuilder.BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new BizException(
                    BizErrorCode.RequestNotFound,
                    404,
                    $"请求不存在: {request.RequestId}");

            if (log.BusinessStatus == BusinessStatusCodes.Confirmed || log.BusinessStatus == BusinessStatusCodes.Committed)
            {
                // 重复 commit 允许幂等返回；如果渠道补传实际落账明细，仍做一次轻量对账校验。
                if ((request.ActualItems?.Count ?? 0) > 0 || request.ActualTotalAmount.HasValue)
                {
                    var confirmedDetails = await _discountRepository.GetByRequestIdAsync(request.RequestId);
                    PricingCommitActualValidator.Validate(request, confirmedDetails, requireActualItems: false);
                }

                _logger.LogInformation(
                    "落账提交幂等命中 请求ID={RequestId}, 当前状态={Status}",
                    request.RequestId, log.BusinessStatus);
                return;
            }

            // ========== 第三阶段：状态和过期校验 ==========
            // 只有 confirm 保护期内的 CONFIRM_PENDING 才能转为正式确认；过期后必须重新 confirm。
            if (log.BusinessStatus != BusinessStatusCodes.ConfirmPending)
            {
                _logger.LogWarning(
                    "落账提交状态校验失败 请求ID={RequestId}, 当前状态={Status}, 期望状态=CONFIRM_PENDING",
                    request.RequestId, log.BusinessStatus);
                throw new BizException(
                    BizErrorCode.RequestStatusNotAllowed,
                    409,
                    $"只有CONFIRM_PENDING状态可以COMMIT, 当前: {log.BusinessStatus}");
            }

            if (_clock.Now > log.RequestAt.AddMinutes(_options.ConfirmExpireMinutes))
            {
                _logger.LogWarning(
                    "落账提交已过期 请求ID={RequestId}, 请求时间={RequestAt}, 过期分钟数={ExpireMinutes}",
                    request.RequestId, log.RequestAt, _options.ConfirmExpireMinutes);
                throw new BizException(
                    BizErrorCode.RequestStatusNotAllowed,
                    409,
                    "确认计价结果已过期，请重新 confirm");
            }

            var details = await _discountRepository.GetByRequestIdAsync(request.RequestId);
            PricingCommitActualValidator.Validate(request, details, requireActualItems: true);

            // ========== 第四阶段：推进正式落账状态 ==========
            // 请求日志、折价明细、限额占用必须一起变更，避免审计和额度口径不一致。
            log.BusinessStatus = BusinessStatusCodes.Confirmed;
            log.ChargeNo = request.ChargeNo ?? log.ChargeNo;
            log.ResponseAt = _clock.Now;
            await _requestLogRepository.UpdateAsync(log);

            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Confirmed);
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, BusinessStatusCodes.Confirmed);

            _logger.LogInformation(
                "落账提交成功 请求ID={RequestId}, 来源系统={SourceSystem}, 项目编码={ItemCode}, 收费单号={ChargeNo}, 提交流水号={CommitNo}, 提交人={CommittedBy}, 提交时间={CommittedAt}",
                request.RequestId, log.SourceSystem, log.ItemCode, log.ChargeNo,
                request.CommitNo, request.CommittedBy, request.CommittedAt);
        });
    }
}
