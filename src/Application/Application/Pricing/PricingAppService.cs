using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.UseCases;
using Pricing.RuleCenter.Core.Constants;

namespace Pricing.RuleCenter.Application.Pricing;

/// <summary>
/// 计价应用服务兼容门面，保留原 public API，实际业务流程委托给各 use case。
/// </summary>
public sealed class PricingAppService
{
    private readonly SimulatePricingUseCase _simulateUseCase;
    private readonly ConfirmPricingUseCase _confirmUseCase;
    private readonly CommitPricingUseCase _commitUseCase;
    private readonly CancelPricingUseCase _cancelUseCase;
    private readonly ReversePricingUseCase _reverseUseCase;
    private readonly GetSpecialFlagUseCase _specialFlagUseCase;

    /// <summary>
    /// 初始化计价应用服务门面。
    /// </summary>
    /// <param name="simulateUseCase">试算用例。</param>
    /// <param name="confirmUseCase">确认计价用例。</param>
    /// <param name="commitUseCase">落账提交用例。</param>
    /// <param name="cancelUseCase">取消确认用例。</param>
    /// <param name="reverseUseCase">退费冲正用例。</param>
    /// <param name="specialFlagUseCase">特殊项目标识查询用例。</param>
    public PricingAppService(
        SimulatePricingUseCase simulateUseCase,
        ConfirmPricingUseCase confirmUseCase,
        CommitPricingUseCase commitUseCase,
        CancelPricingUseCase cancelUseCase,
        ReversePricingUseCase reverseUseCase,
        GetSpecialFlagUseCase specialFlagUseCase)
    {
        _simulateUseCase = simulateUseCase;
        _confirmUseCase = confirmUseCase;
        _commitUseCase = commitUseCase;
        _cancelUseCase = cancelUseCase;
        _reverseUseCase = reverseUseCase;
        _specialFlagUseCase = specialFlagUseCase;
    }

    /// <summary>
    /// 执行试算计价。
    /// </summary>
    /// <param name="request">统一计价请求。</param>
    /// <returns>计价响应。</returns>
    public Task<PricingCalculateResponse> SimulateAsync(PricingCalculateRequest request)
    {
        return _simulateUseCase.ExecuteAsync(request);
    }

    /// <summary>
    /// 执行正式确认计价。
    /// </summary>
    /// <param name="request">统一计价请求。</param>
    /// <returns>计价响应。</returns>
    public Task<PricingCalculateResponse> ConfirmAsync(PricingCalculateRequest request)
    {
        return _confirmUseCase.ExecuteAsync(request);
    }

    /// <summary>
    /// HIS 落账成功后提交计价结果。
    /// </summary>
    /// <param name="request">commit 请求。</param>
    public Task CommitAsync(PricingCommitRequest request)
    {
        return _commitUseCase.ExecuteAsync(request);
    }

    /// <summary>
    /// HIS 落账失败、支付失败或用户取消时释放 confirm 保护状态。
    /// </summary>
    /// <param name="request">cancel 请求。</param>
    public Task CancelAsync(PricingCancelRequest request)
    {
        return _cancelUseCase.ExecuteAsync(request);
    }

    /// <summary>
    /// 对已经落账确认的计价结果执行冲正。
    /// </summary>
    /// <param name="request">冲正请求。</param>
    public Task ReverseAsync(PricingReverseRequest request)
    {
        return _reverseUseCase.ExecuteAsync(request);
    }

    /// <summary>
    /// 查询项目是否属于必须调用统一计价中心的特殊项目。
    /// </summary>
    /// <param name="itemCode">项目编码。</param>
    /// <returns>特殊项目标识响应。</returns>
    public Task<SpecialFlagResponse> GetSpecialFlagAsync(string itemCode)
    {
        return _specialFlagUseCase.ExecuteAsync(itemCode);
    }

    internal static string BuildRequestLockKey(long requestId)
    {
        return PricingLockKeyBuilder.BuildRequestLockKey(requestId);
    }
}
