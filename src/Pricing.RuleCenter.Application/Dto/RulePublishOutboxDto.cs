using System.Text.Json.Serialization;

namespace Pricing.RuleCenter.Application.Dto;

/// <summary>
/// 规则缓存失效 outbox 运维汇总响应 DTO。
/// </summary>
public sealed class RuleCacheOutboxSummaryResponse
{
    /// <summary>
    /// 待处理任务数。
    /// </summary>
    [JsonPropertyName("pending_count")]
    public int PendingCount { get; init; }

    /// <summary>
    /// 失败待重试任务数。
    /// </summary>
    [JsonPropertyName("failed_count")]
    public int FailedCount { get; init; }

    /// <summary>
    /// 当前最早的未完成任务创建时间。
    /// </summary>
    [JsonPropertyName("oldest_unprocessed_created_at")]
    public DateTime? OldestUnprocessedCreatedAt { get; init; }

    /// <summary>
    /// 最近失败任务明细。
    /// </summary>
    [JsonPropertyName("failed_items")]
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
    [JsonPropertyName("outbox_id")]
    public long OutboxId { get; init; }

    /// <summary>
    /// 缓存作用域。
    /// </summary>
    [JsonPropertyName("cache_scope")]
    public string CacheScope { get; init; } = string.Empty;

    /// <summary>
    /// 触发操作类型。
    /// </summary>
    [JsonPropertyName("operation_type")]
    public string OperationType { get; init; } = string.Empty;

    /// <summary>
    /// 关联规则主键。
    /// </summary>
    [JsonPropertyName("rule_id")]
    public long RuleId { get; init; }

    /// <summary>
    /// 关联版本号。
    /// </summary>
    [JsonPropertyName("version_no")]
    public int? VersionNo { get; init; }

    /// <summary>
    /// 当前状态。
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// 已重试次数。
    /// </summary>
    [JsonPropertyName("retry_count")]
    public int RetryCount { get; init; }

    /// <summary>
    /// 下次重试时间。
    /// </summary>
    [JsonPropertyName("next_retry_at")]
    public DateTime? NextRetryAt { get; init; }

    /// <summary>
    /// 最后错误信息。
    /// </summary>
    [JsonPropertyName("last_error")]
    public string? LastError { get; init; }

    /// <summary>
    /// 创建时间。
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }
}
