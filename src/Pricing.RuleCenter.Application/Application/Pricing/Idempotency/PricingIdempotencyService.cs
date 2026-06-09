using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Interfaces.Charging;

namespace Pricing.RuleCenter.Application.Pricing.Idempotency;

/// <summary>
/// 计价幂等服务，负责生成 confirm 指纹并校验同业务号重试参数一致性。
/// </summary>
public sealed class PricingIdempotencyService
{
    /// <summary>
    /// 请求日志仓储，用于按业务键查询首次 confirm 记录。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;

    /// <summary>
    /// 初始化计价幂等服务。
    /// </summary>
    /// <param name="requestLogRepository">计价请求日志仓储。</param>
    public PricingIdempotencyService(IChargeRequestLogRepository requestLogRepository)
    {
        _requestLogRepository = requestLogRepository;
    }

    /// <summary>
    /// 检查 confirm 幂等状态并生成本次请求指纹。
    /// </summary>
    /// <param name="request">确认计价请求。</param>
    /// <param name="items">已校验费用明细集合。</param>
    /// <param name="callType">调用类型。</param>
    /// <returns>包含已有请求和本次指纹的幂等检查结果。</returns>
    /// <remarks>
    /// 该方法只做事务外快路径检查。并发请求仍可能同时查不到已有记录，因此 confirm workflow
    /// 必须在幂等锁内做事务内二次检查。
    /// </remarks>
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
            // 指纹一致说明是同一业务动作重试，可以复用首次响应快照，不重复占额。
            return;
        }

        // 指纹不一致说明同一 BusinessRequestNo 被用于不同业务事实，必须拒绝，不能覆盖首次结果。
        throw new BizException(
            BizErrorCode.IdempotencyConflict,
            409,
            $"BusinessRequestNo={businessRequestNo} 已存在，但本次参数与首次请求不一致");
    }
}
