using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Rules.Guards;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则发布用例。
/// </summary>
public sealed class PublishRuleUseCase : RulePublishUseCaseBase
{
    /// <summary>
    /// 初始化规则发布用例。
    /// </summary>
    public PublishRuleUseCase(
        RulePublishLifecycleRepositories lifecycleRepositories,
        RulePublishDefinitionRepositories definitionRepositories,
        RulePublishTransactionWriter transactionWriter,
        RulePublishCacheInvalidator cacheInvalidator,
        IRuleCacheInvalidationOutboxRepository cacheInvalidationOutboxRepository,
        RuleCacheInvalidationOutboxProcessor cacheInvalidationOutboxProcessor,
        IClock clock,
        RuleApprovalGate approvalGate,
        RulePublishGuard publishGuard,
        ILogger<PublishRuleUseCase> logger)
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
    /// 执行规则发布。
    /// </summary>
    public Task ExecuteAsync(long ruleId, RulePublishRequest request)
    {
        return ExecutePublishAsync(ruleId, request);
    }
}
