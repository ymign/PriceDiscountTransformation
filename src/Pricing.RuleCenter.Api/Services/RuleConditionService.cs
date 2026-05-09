using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

public sealed class RuleConditionService
{
    private readonly IRuleConditionRepository _conditionRepository;
    private readonly IRuleVersionRepository _versionRepository;
    private readonly ILogger<RuleConditionService> _logger;

    public RuleConditionService(
        IRuleConditionRepository conditionRepository,
        IRuleVersionRepository versionRepository,
        ILogger<RuleConditionService> logger)
    {
        _conditionRepository = conditionRepository;
        _versionRepository = versionRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RuleConditionResponse>> GetAsync(long ruleId, int versionNo)
    {
        var items = await _conditionRepository.GetByRuleAndVersionAsync(ruleId, versionNo);
        return items.Select(MapToResponse).ToList();
    }

    public async Task SaveAsync(long ruleId, int versionNo, RuleConditionSaveRequest request)
    {
        var version = await _versionRepository.GetByRuleAndVersionAsync(ruleId, versionNo)
            ?? throw new KeyNotFoundException($"规则版本不存在: RuleId={ruleId}, VersionNo={versionNo}");

        if (version.VersionStatus != "DRAFT")
        {
            throw new InvalidOperationException($"只有草稿版本可以编辑条件, 当前状态: {version.VersionStatus}");
        }

        await _conditionRepository.DeleteByRuleAndVersionAsync(ruleId, versionNo);

        var entities = request.Conditions.Select(c => new RuleCondition
        {
            RuleId = ruleId,
            VersionNo = versionNo,
            ConditionGroup = c.ConditionGroup,
            ConditionType = c.ConditionType,
            OperatorType = c.OperatorType,
            LeftKey = c.LeftKey,
            RightValue = c.RightValue,
            ParamsJson = c.ParamsJson,
            SortNo = c.SortNo,
            IsEnabled = c.IsEnabled
        }).ToList();

        if (entities.Count > 0)
        {
            await _conditionRepository.InsertBatchAsync(entities);
        }

        _logger.LogInformation("保存规则条件 RuleId={RuleId}, VersionNo={VersionNo}, Count={Count}",
            ruleId, versionNo, entities.Count);
    }

    private static RuleConditionResponse MapToResponse(RuleCondition entity)
    {
        return new RuleConditionResponse
        {
            ConditionId = entity.ConditionId,
            RuleId = entity.RuleId,
            VersionNo = entity.VersionNo,
            ConditionGroup = entity.ConditionGroup,
            ConditionType = entity.ConditionType,
            OperatorType = entity.OperatorType,
            LeftKey = entity.LeftKey,
            RightValue = entity.RightValue,
            ParamsJson = entity.ParamsJson,
            SortNo = entity.SortNo,
            IsEnabled = entity.IsEnabled
        };
    }
}
