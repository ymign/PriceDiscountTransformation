using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Rules.Guards;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则停用用例。
/// </summary>
public sealed class DisableRuleUseCase : RulePublishUseCaseBase
{
    /// <summary>
    /// 初始化规则停用用例。
    /// </summary>
    public DisableRuleUseCase(
        RulePublishLifecycleRepositories lifecycleRepositories,
        RulePublishDefinitionRepositories definitionRepositories,
        RulePublishTransactionWriter transactionWriter,
        RulePublishCacheInvalidator cacheInvalidator,
        IRuleCacheInvalidationOutboxRepository cacheInvalidationOutboxRepository,
        RuleCacheInvalidationOutboxProcessor cacheInvalidationOutboxProcessor,
        IClock clock,
        RuleApprovalGate approvalGate,
        RulePublishGuard publishGuard,
        ILogger<DisableRuleUseCase> logger)
        : base(
            lifecycleRepositories,
            definitionRepositories,
            transactionWriter,
            cacheInvalidator,
            cacheInvalidationOutboxRepository,
            cacheInvalidationOutboxProcessor,
            clock,
            approvalGate,
            publishGuard,
            logger)
    {
    }

    /// <summary>
    /// 执行规则停用。
    /// </summary>
    public Task ExecuteAsync(long ruleId, RuleDisableRequest request)
    {
        return ExecuteDisableAsync(ruleId, request);
    }
}
