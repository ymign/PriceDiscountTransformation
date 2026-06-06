using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Application.Pricing.UseCases;

/// <summary>
/// 取消确认计价用例。
/// </summary>
public sealed class CancelPricingUseCase : PricingUseCaseBase
{
    /// <summary>
    /// 初始化取消确认计价用例。
    /// </summary>
    public CancelPricingUseCase(
        PricingAppCalculationDependencies calculationDependencies,
        PricingAppPersistenceRepositories repositories,
        AuthorityPriceChecker authorityPriceChecker,
        PricingIdempotencyService idempotencyService,
        PricingRequestLogWriter requestLogWriter,
        PricingTraceStepWriter traceStepWriter,
        PricingDiscountDetailWriter discountDetailWriter,
        PricingLimitOccupyWriter limitOccupyWriter,
        PricingReverseLogWriter reverseLogWriter,
        IUnitOfWork unitOfWork,
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger<CancelPricingUseCase> logger)
        : base(
            calculationDependencies,
            repositories,
            authorityPriceChecker,
            idempotencyService,
            requestLogWriter,
            traceStepWriter,
            discountDetailWriter,
            limitOccupyWriter,
            reverseLogWriter,
            unitOfWork,
            options,
            clock,
            logger)
    {
    }

    /// <summary>
    /// 执行取消确认计价。
    /// </summary>
    public Task ExecuteAsync(PricingCancelRequest request)
    {
        return ExecuteCancelAsync(request);
    }
}
