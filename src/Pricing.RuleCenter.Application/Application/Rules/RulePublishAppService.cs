using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Rules.Publishing;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules;

/// <summary>
/// 规则发布应用服务兼容门面，保留原 public API，生命周期状态机委托给 use case。
/// </summary>
public sealed class RulePublishAppService
{
    private readonly IRulePublishRepository _publishRepository;
    private readonly IRuleChangeLogRepository _changeLogRepository;
    private readonly PublishRuleUseCase _publishUseCase;
    private readonly DisableRuleUseCase _disableUseCase;
    private readonly RollbackRuleUseCase _rollbackUseCase;

    /// <summary>
    /// 初始化规则发布应用服务门面。
    /// </summary>
    public RulePublishAppService(
        RulePublishLifecycleRepositories lifecycleRepositories,
        PublishRuleUseCase publishUseCase,
        DisableRuleUseCase disableUseCase,
        RollbackRuleUseCase rollbackUseCase)
    {
        _publishRepository = lifecycleRepositories.PublishRepository;
        _changeLogRepository = lifecycleRepositories.ChangeLogRepository;
        _publishUseCase = publishUseCase;
        _disableUseCase = disableUseCase;
        _rollbackUseCase = rollbackUseCase;
    }

    /// <summary>
    /// 读取规则的发布历史。
    /// </summary>
    public async Task<IReadOnlyList<RulePublishResponse>> GetPublishHistoryAsync(long ruleId)
    {
        var items = await _publishRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapPublishToResponse).ToList();
    }

    /// <summary>
    /// 读取规则变更日志。
    /// </summary>
    public async Task<IReadOnlyList<RuleChangeLogResponse>> GetChangeLogsAsync(long ruleId)
    {
        var items = await _changeLogRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapChangeLogToResponse).ToList();
    }

    /// <summary>
    /// 发布指定草稿版本，让其成为规则当前生效版本。
    /// </summary>
    public Task PublishAsync(long ruleId, RulePublishRequest request)
    {
        return _publishUseCase.ExecuteAsync(ruleId, request);
    }

    /// <summary>
    /// 停用当前已发布规则。
    /// </summary>
    public Task DisableAsync(long ruleId, RuleDisableRequest request)
    {
        return _disableUseCase.ExecuteAsync(ruleId, request);
    }

    /// <summary>
    /// 回滚到最近一个历史发布版本。
    /// </summary>
    public Task RollbackAsync(long ruleId, RuleRollbackRequest request)
    {
        return _rollbackUseCase.ExecuteAsync(ruleId, request);
    }

    private static RulePublishResponse MapPublishToResponse(RulePublish entity)
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

    private static RuleChangeLogResponse MapChangeLogToResponse(RuleChangeLog entity)
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
