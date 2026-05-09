using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

/// <summary>
/// 规则条件应用服务，负责维护指定规则版本下的匹配条件集合。
/// </summary>
/// <remarks>
/// 条件是版本内容的一部分，只允许在 DRAFT 版本中整体保存。已发布版本必须保持可追溯快照，
/// 因此这里不会允许对 PUBLISHED、DISABLED 或 ROLLED_BACK 版本做局部修改。
/// </remarks>
public sealed class RuleConditionService
{
    /// <summary>
    /// 规则条件仓储，负责读取、清空和批量写入某个版本下的条件明细。
    /// </summary>
    private readonly IRuleConditionRepository _conditionRepository;
    /// <summary>
    /// 规则版本仓储，用于保存前校验版本存在且仍处于草稿状态。
    /// </summary>
    private readonly IRuleVersionRepository _versionRepository;
    /// <summary>
    /// 服务日志，用于记录条件集合保存数量。
    /// </summary>
    private readonly ILogger<RuleConditionService> _logger;

    /// <summary>
    /// 初始化规则条件服务。
    /// </summary>
    /// <param name="conditionRepository">规则条件仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="logger">日志对象。</param>
    public RuleConditionService(
        IRuleConditionRepository conditionRepository,
        IRuleVersionRepository versionRepository,
        ILogger<RuleConditionService> logger)
    {
        _conditionRepository = conditionRepository;
        _versionRepository = versionRepository;
        _logger = logger;
    }

    /// <summary>
    /// 读取指定规则版本下的条件集合。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <returns>条件响应列表，按仓储层定义的排序返回。</returns>
    public async Task<IReadOnlyList<RuleConditionResponse>> GetAsync(long ruleId, int versionNo)
    {
        var items = await _conditionRepository.GetByRuleAndVersionAsync(ruleId, versionNo);
        return items.Select(MapToResponse).ToList();
    }

    /// <summary>
    /// 整体保存指定草稿版本的条件集合。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">条件保存请求，包含完整条件集合。</param>
    /// <exception cref="KeyNotFoundException">规则版本不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">规则版本不是草稿状态时抛出。</exception>
    public async Task SaveAsync(long ruleId, int versionNo, RuleConditionSaveRequest request)
    {
        // ========== 第一阶段：校验版本状态 ==========
        // 条件属于版本快照内容；已发布或历史版本如果允许编辑，会导致追踪记录无法解释当时使用了哪套条件。
        var version = await _versionRepository.GetByRuleAndVersionAsync(ruleId, versionNo)
            ?? throw new KeyNotFoundException($"规则版本不存在: RuleId={ruleId}, VersionNo={versionNo}");

        if (version.VersionStatus != "DRAFT")
        {
            throw new InvalidOperationException($"只有草稿版本可以编辑条件, 当前状态: {version.VersionStatus}");
        }

        // ========== 第二阶段：先清空旧条件 ==========
        // 前端通常以完整条件树提交。采用“删除后重建”能避免复杂的增删改合并逻辑，
        // 同时保证排序号、分组和启用状态完全以本次提交为准。
        await _conditionRepository.DeleteByRuleAndVersionAsync(ruleId, versionNo);

        // ========== 第三阶段：把请求项映射为实体 ==========
        // ConditionGroup 用于表达同组条件关系；具体如何求值由 RuleMatchService 和 Evaluator 执行。
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

        // ========== 第四阶段：批量写入非空集合 ==========
        // 空集合也是合法保存结果，表示该版本暂时没有额外条件，因此不能强行报错。
        if (entities.Count > 0)
        {
            await _conditionRepository.InsertBatchAsync(entities);
        }

        _logger.LogInformation("保存规则条件 RuleId={RuleId}, VersionNo={VersionNo}, Count={Count}",
            ruleId, versionNo, entities.Count);
    }

    /// <summary>
    /// 将规则条件实体映射为接口响应。
    /// </summary>
    /// <param name="entity">规则条件实体。</param>
    /// <returns>规则条件响应 DTO。</returns>
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
