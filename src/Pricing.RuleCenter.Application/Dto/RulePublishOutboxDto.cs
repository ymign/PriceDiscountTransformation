namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 规则缓存失效 outbox 运维汇总响应 DTO。
/// </summary>
public sealed class RuleCacheOutboxSummaryResponse
{
    /// <summary>
    /// 待处理任务数。
    /// </summary>
    public int PendingCount { get; init; }

    /// <summary>
    /// 失败待重试任务数。
    /// </summary>
    public int FailedCount { get; init; }

    /// <summary>
    /// 当前最早的未完成任务创建时间。
    /// </summary>
    public DateTime? OldestUnprocessedCreatedAt { get; init; }

    /// <summary>
    /// 最近失败任务明细。
    /// </summary>
    public IReadOnlyList<RuleCacheOutboxItemResponse> FailedItems { get; init; } = Array.Empty<RuleCacheOutboxItemResponse>();
}

/// <summary>
/// 规则缓存失效 outbox 运维明细 DTO。
/// </summary>
public sealed class RuleCacheOutboxItemResponse
{
    /// <summary>
    /// outbox 主键。
    /// </summary>
    public long OutboxId { get; init; }

    /// <summary>
    /// 缓存作用域。
    /// </summary>
    public string CacheScope { get; init; } = string.Empty;

    /// <summary>
    /// 触发操作类型。
    /// </summary>
    public string OperationType { get; init; } = string.Empty;

    /// <summary>
    /// 关联规则主键。
    /// </summary>
    public long RuleId { get; init; }

    /// <summary>
    /// 关联版本号。
    /// </summary>
    public int? VersionNo { get; init; }

    /// <summary>
    /// 当前状态。
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// 已重试次数。
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    /// 下次重试时间。
    /// </summary>
    public DateTime? NextRetryAt { get; init; }

    /// <summary>
    /// 最后错误信息。
    /// </summary>
    public string? LastError { get; init; }

    /// <summary>
    /// 创建时间。
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
