using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Application.Pricing.UseCases;

/// <summary>
/// 试算计价用例。
/// </summary>
public sealed class SimulatePricingUseCase : PricingUseCaseBase
{
    /// <summary>
    /// 初始化试算计价用例。
    /// </summary>
    public SimulatePricingUseCase(
        PricingAppCalculationDependencies calculationDependencies,
        PricingAppPersistenceRepositories repositories,
        AuthorityPriceChecker authorityPriceChecker,
        PricingIdempotencyService idempotencyService,
        PricingRequestLogWriter requestLogWriter,
        PricingTraceStepWriter traceStepWriter,
        PricingDiscountDetailWriter discountDetailWriter,
        PricingLimitOccupyWriter limitOccupyWriter,
        PricingReverseLogWriter reverseLogWriter,
        RuntimePackageTraceResolver runtimePackageTraceResolver,
        IUnitOfWork unitOfWork,
        IOptions<PricingOptions> options,
        IClock clock,
        ILogger<SimulatePricingUseCase> logger)
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
            runtimePackageTraceResolver,
            unitOfWork,
            options,
            clock,
            logger)
    {
    }

    /// <summary>
    /// 执行试算计价。
    /// </summary>
    public Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request)
    {
        return ExecuteSimulateAsync(request);
    }
}
