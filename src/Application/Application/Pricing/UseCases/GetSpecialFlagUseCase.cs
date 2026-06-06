using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Pricing.Idempotency;
using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Application.Pricing.UseCases;

/// <summary>
/// 特殊项目标识查询用例。
/// </summary>
public sealed class GetSpecialFlagUseCase : PricingUseCaseBase
{
    /// <summary>
    /// 初始化特殊项目标识查询用例。
    /// </summary>
    public GetSpecialFlagUseCase(
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
        ILogger<GetSpecialFlagUseCase> logger)
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
    /// 执行特殊项目标识查询。
    /// </summary>
    public Task<SpecialFlagResponse> ExecuteAsync(string itemCode)
    {
        return ExecuteGetSpecialFlagAsync(itemCode);
    }
}
