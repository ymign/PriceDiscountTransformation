using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

public sealed class RulePublishResponse
{
    public long PublishId { get; init; }
    public string PublishNo { get; init; } = string.Empty;
    public long RuleId { get; init; }
    public int? FromVersion { get; init; }
    public int ToVersion { get; init; }
    public string ActionType { get; init; } = string.Empty;
    public string? PublishedBy { get; init; }
    public DateTime PublishedAt { get; init; }
    public string? Remark { get; init; }
}

public sealed class RulePublishRequest
{
    [Required(ErrorMessage = "版本号不能为空")]
    public int VersionNo { get; init; }

    public string? PublishedBy { get; init; }
    public string? Remark { get; init; }
}

public sealed class RuleDisableRequest
{
    public string? PublishedBy { get; init; }
    public string? Remark { get; init; }
}

public sealed class RuleRollbackRequest
{
    public string? PublishedBy { get; init; }
    public string? Remark { get; init; }
}

public sealed class RuleChangeLogResponse
{
    public long ChangeId { get; init; }
    public long RuleId { get; init; }
    public int? VersionNo { get; init; }
    public string ChangeType { get; init; } = string.Empty;
    public string? ChangeSummary { get; init; }
    public string? ChangedBy { get; init; }
    public DateTime ChangedAt { get; init; }
    public string? SourceSystem { get; init; }
}
