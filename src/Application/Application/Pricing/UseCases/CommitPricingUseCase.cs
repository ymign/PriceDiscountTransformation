using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Application.Pricing.UseCases;

/// <summary>
/// 落账提交用例。
/// </summary>
public sealed class CommitPricingUseCase : PricingUseCaseBase
{
    /// <summary>
    /// 初始化落账提交用例。
    /// </summary>
    public CommitPricingUseCase(
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
        ILogger<CommitPricingUseCase> logger)
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
    /// 执行落账提交。
    /// </summary>
    public Task ExecuteAsync(PricingCommitRequest request)
    {
        return ExecuteCommitAsync(request);
    }
}
