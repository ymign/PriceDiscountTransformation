namespace Pricing.RuleCenter.Api.Dto;

/// <summary>
/// 规则版本响应 DTO。
/// </summary>
/// <remarks>
/// 版本响应用于展示规则版本状态和发布信息。条件、动作明细通过各自接口读取。
/// </remarks>
public sealed class RuleVersionResponse
{
    /// <summary>
    /// 规则版本主键
    /// </summary>
    public long VersionId { get; init; }
    /// <summary>
    /// 规则主键，用于关联规则头、版本、条件、动作和追溯结果
    /// </summary>
    public long RuleId { get; init; }
    /// <summary>
    /// 规则版本号，与规则条件和动作的 VERSION_NO 对齐
    /// </summary>
    public int VersionNo { get; init; }
    /// <summary>
    /// 版本状态，例如 DRAFT、PUBLISHED、DISABLED 或 ROLLED_BACK
    /// </summary>
    public string VersionStatus { get; init; } = string.Empty;
    /// <summary>
    /// 规则或版本的生效开始时间
    /// </summary>
    public DateTime? EffectiveFrom { get; init; }
    /// <summary>
    /// 规则或版本的生效结束时间，空值表示未设失效时间
    /// </summary>
    public DateTime? EffectiveTo { get; init; }
    /// <summary>
    /// 规则发布时的完整快照 JSON，用于历史计价追溯
    /// </summary>
    public string? RuleSnapshot { get; init; }
    /// <summary>
    /// 发布、停用或回滚操作人
    /// </summary>
    public string? PublishedBy { get; init; }
    /// <summary>
    /// 发布、停用或回滚发生时间
    /// </summary>
    public DateTime? PublishedAt { get; init; }
    /// <summary>
    /// 发布说明或审批备注
    /// </summary>
    public string? PublishRemark { get; init; }
}
