using System.ComponentModel.DataAnnotations;

namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 规则发布流水响应 DTO。
/// </summary>
public sealed class RulePublishResponse
{
    /// <summary>
    /// 发布流水主键。
    /// </summary>
    public long PublishId { get; init; }
    /// <summary>
    /// 发布流水号，用于审计和日志交叉定位。
    /// </summary>
    public string PublishNo { get; init; } = string.Empty;
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; init; }
    /// <summary>
    /// 操作前版本号；首次发布时可以为空。
    /// </summary>
    public int? FromVersion { get; init; }
    /// <summary>
    /// 操作后版本号。
    /// </summary>
    public int ToVersion { get; init; }
    /// <summary>
    /// 动作类型，决定由哪个动作执行器处理
    /// </summary>
    public string ActionType { get; init; } = string.Empty;
    /// <summary>
    /// 发布、停用或回滚操作人
    /// </summary>
    public string? PublishedBy { get; init; }
    /// <summary>
    /// 发布、停用或回滚发生时间
    /// </summary>
    public DateTime PublishedAt { get; init; }
    /// <summary>
    /// 发布、停用或回滚备注。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 规则发布请求 DTO。
/// </summary>
public sealed class RulePublishRequest
{
    [Required(ErrorMessage = "版本号不能为空")]
    /// <summary>
    /// 规则版本号，与规则条件和动作的 VERSION_NO 对齐
    /// </summary>
    public int VersionNo { get; init; }

    /// <summary>
    /// 发布、停用或回滚操作人
    /// </summary>
    public string? PublishedBy { get; init; }
    /// <summary>
    /// 发布备注。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 规则停用请求 DTO。
/// </summary>
public sealed class RuleDisableRequest
{
    /// <summary>
    /// 发布、停用或回滚操作人
    /// </summary>
    public string? PublishedBy { get; init; }
    /// <summary>
    /// 停用备注。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 规则回滚请求 DTO。
/// </summary>
public sealed class RuleRollbackRequest
{
    /// <summary>
    /// 发布、停用或回滚操作人
    /// </summary>
    public string? PublishedBy { get; init; }
    /// <summary>
    /// 回滚备注。
    /// </summary>
    public string? Remark { get; init; }
}

/// <summary>
/// 规则变更日志响应 DTO。
/// </summary>
public sealed class RuleChangeLogResponse
{
    /// <summary>
    /// 变更日志主键。
    /// </summary>
    public long ChangeId { get; init; }
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; init; }
    /// <summary>
    /// 规则版本号，与规则条件和动作的 VERSION_NO 对齐
    /// </summary>
    public int? VersionNo { get; init; }
    /// <summary>
    /// 变更类型，例如 PUBLISH、DISABLE 或 ROLLBACK。
    /// </summary>
    public string ChangeType { get; init; } = string.Empty;
    /// <summary>
    /// 变更摘要，面向配置人员展示。
    /// </summary>
    public string? ChangeSummary { get; init; }
    /// <summary>
    /// 变更操作人。
    /// </summary>
    public string? ChangedBy { get; init; }
    /// <summary>
    /// 变更发生时间。
    /// </summary>
    public DateTime ChangedAt { get; init; }
    /// <summary>
    /// 来源系统编码，例如 HIS、自助机或微信公众号
    /// </summary>
    public string? SourceSystem { get; init; }
}
