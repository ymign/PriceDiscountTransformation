using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Idempotency;

/// <summary>
/// 计价幂等服务，负责生成 confirm 指纹并校验同业务号重试参数一致性。
/// </summary>
public sealed class PricingIdempotencyService
{
    private readonly IChargeRequestLogRepository _requestLogRepository;

    /// <summary>
    /// 初始化计价幂等服务。
    /// </summary>
    /// <param name="requestLogRepository">计价请求日志仓储。</param>
    public PricingIdempotencyService(IChargeRequestLogRepository requestLogRepository)
    {
        _requestLogRepository = requestLogRepository;
    }

    internal async Task<PricingIdempotencyResult> CheckConfirmAsync(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items,
        string callType)
    {
        var fingerprint = PricingRequestFingerprintBuilder.BuildConfirmFingerprint(request, items, callType);
        var existing = await _requestLogRepository.GetByBusinessKeyAsync(
            request.SourceSystem,
            request.BusinessRequestNo!,
            callType);

        return new PricingIdempotencyResult(
            existing is not null,
            existing,
            fingerprint);
    }

    /// <summary>
    /// 确保已有请求指纹与本次请求一致。
    /// </summary>
    /// <param name="existing">已存在的计价请求日志。</param>
    /// <param name="fingerprint">本次请求计算出的指纹。</param>
    /// <param name="businessRequestNo">业务幂等号。</param>
    public void EnsureSameFingerprint(
        ChargeRequest existing,
        string fingerprint,
        string businessRequestNo)
    {
        if (string.Equals(existing.RequestFingerprint, fingerprint, StringComparison.Ordinal))
        {
            return;
        }

        throw new BizException(
            BizErrorCode.IdempotencyConflict,
            409,
            $"BusinessRequestNo={businessRequestNo} 已存在，但本次参数与首次请求不一致");
    }
}
