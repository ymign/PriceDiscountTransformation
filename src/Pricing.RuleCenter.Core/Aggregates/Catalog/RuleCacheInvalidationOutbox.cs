namespace Pricing.RuleCenter.Core.Aggregates.Catalog;

/// <summary>
/// 规则缓存失效 outbox 任务。
/// </summary>
public sealed class RuleCacheInvalidationOutbox
{
    /// <summary>任务主键。</summary>
    public long OutboxId { get; set; }

    /// <summary>缓存作用域，例如 EFFECTIVE_RULES、ACTION_TYPE_ORDER。</summary>
    public string CacheScope { get; set; } = string.Empty;

    /// <summary>触发失效的业务操作类型，例如 PUBLISH、DISABLE、ROLLBACK。</summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>关联规则主键。</summary>
    public long RuleId { get; set; }

    /// <summary>关联版本号。</summary>
    public int? VersionNo { get; set; }

    /// <summary>任务状态。</summary>
    public string Status { get; set; } = CacheInvalidationOutboxStatusCodes.Pending;

    /// <summary>已重试次数。</summary>
    public int RetryCount { get; set; }

    /// <summary>下次可重试时间。</summary>
    public DateTime? NextRetryAt { get; set; }

    /// <summary>最后一次错误信息。</summary>
    public string? LastError { get; set; }

    /// <summary>创建时间。</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>处理完成时间。</summary>
    public DateTime? ProcessedAt { get; set; }
}

/// <summary>
/// 规则缓存失效 outbox 状态编码。
/// </summary>
public static class CacheInvalidationOutboxStatusCodes
{
    /// <summary>待处理。</summary>
    public const string Pending = "PENDING";

    /// <summary>已处理。</summary>
    public const string Processed = "PROCESSED";

    /// <summary>处理失败，等待重试。</summary>
    public const string Failed = "FAILED";
}
