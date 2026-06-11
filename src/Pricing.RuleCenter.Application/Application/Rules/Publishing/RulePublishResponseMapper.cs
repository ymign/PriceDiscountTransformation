using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

/// <summary>
/// 规则发布相关 DTO 映射器。
/// </summary>
internal static class RulePublishResponseMapper
{
    /// <summary>
    /// 将发布流水实体映射为接口响应。
    /// </summary>
    public static RulePublishResponse ToResponse(RulePublish entity)
    {
        return new RulePublishResponse
        {
            PublishId = entity.PublishId,
            PublishNo = entity.PublishNo,
            RuleId = entity.RuleId,
            FromVersion = entity.FromVersion,
            ToVersion = entity.ToVersion,
            ActionType = entity.ActionType,
            PublishedBy = entity.PublishedBy,
            PublishedAt = entity.PublishedAt,
            Remark = entity.Remark
        };
    }

    /// <summary>
    /// 将规则变更日志实体映射为接口响应。
    /// </summary>
    public static RuleChangeLogResponse ToResponse(RuleChangeLog entity)
    {
        return new RuleChangeLogResponse
        {
            ChangeId = entity.ChangeId,
            RuleId = entity.RuleId,
            VersionNo = entity.VersionNo,
            ChangeType = entity.ChangeType,
            ChangeSummary = entity.ChangeSummary,
            ChangedBy = entity.ChangedBy,
            ChangedAt = entity.ChangedAt,
            SourceSystem = entity.SourceSystem
        };
    }
}
