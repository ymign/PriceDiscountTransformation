using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

public sealed class RuleActionService
{
    private readonly IRuleActionRepository _actionRepository;
    private readonly IRuleVersionRepository _versionRepository;
    private readonly ILogger<RuleActionService> _logger;

    public RuleActionService(
        IRuleActionRepository actionRepository,
        IRuleVersionRepository versionRepository,
        ILogger<RuleActionService> logger)
    {
        _actionRepository = actionRepository;
        _versionRepository = versionRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RuleActionResponse>> GetAsync(long ruleId, int versionNo)
    {
        var items = await _actionRepository.GetByRuleAndVersionAsync(ruleId, versionNo);
        return items.Select(MapToResponse).ToList();
    }

    public async Task SaveAsync(long ruleId, int versionNo, RuleActionSaveRequest request)
    {
        var version = await _versionRepository.GetByRuleAndVersionAsync(ruleId, versionNo)
            ?? throw new KeyNotFoundException($"规则版本不存在: RuleId={ruleId}, VersionNo={versionNo}");

        if (version.VersionStatus != "DRAFT")
        {
            throw new InvalidOperationException($"只有草稿版本可以编辑动作, 当前状态: {version.VersionStatus}");
        }

        await _actionRepository.DeleteByRuleAndVersionAsync(ruleId, versionNo);

        var entities = request.Actions.Select(a => new RuleAction
        {
            RuleId = ruleId,
            VersionNo = versionNo,
            ActionType = a.ActionType,
            ExecutorCode = a.ExecutorCode,
            ParamsJson = a.ParamsJson,
            ExclusiveGroup = a.ExclusiveGroup,
            SortNo = a.SortNo,
            OnError = a.OnError,
            IsEnabled = a.IsEnabled
        }).ToList();

        if (entities.Count > 0)
        {
            await _actionRepository.InsertBatchAsync(entities);
        }

        _logger.LogInformation("保存规则动作 RuleId={RuleId}, VersionNo={VersionNo}, Count={Count}",
            ruleId, versionNo, entities.Count);
    }

    private static RuleActionResponse MapToResponse(RuleAction entity)
    {
        return new RuleActionResponse
        {
            ActionId = entity.ActionId,
            RuleId = entity.RuleId,
            VersionNo = entity.VersionNo,
            ActionType = entity.ActionType,
            ExecutorCode = entity.ExecutorCode,
            ParamsJson = entity.ParamsJson,
            ExclusiveGroup = entity.ExclusiveGroup,
            SortNo = entity.SortNo,
            OnError = entity.OnError,
            IsEnabled = entity.IsEnabled
        };
    }
}
