namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则缓存失效 outbox 处理结果。
/// </summary>
public sealed record RuleCacheInvalidationProcessResult(
    int ProcessedCount,
    int FailedCount);
