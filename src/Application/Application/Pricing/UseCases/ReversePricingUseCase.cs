using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Application.Pricing.UseCases;

/// <summary>
/// 退费冲正用例。
/// </summary>
public sealed class ReversePricingUseCase : PricingUseCaseBase
{
    /// <summary>
    /// 初始化退费冲正用例。
    /// </summary>
    public ReversePricingUseCase(
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
        ILogger<ReversePricingUseCase> logger)
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
    /// 执行退费冲正。
    /// </summary>
    public Task ExecuteAsync(PricingReverseRequest request)
    {
        return ExecuteReverseAsync(request);
    }
}
