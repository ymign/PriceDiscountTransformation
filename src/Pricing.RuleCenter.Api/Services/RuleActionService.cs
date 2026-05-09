using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

/// <summary>
/// 规则动作应用服务，负责维护指定规则版本下的动作执行链。
/// </summary>
/// <remarks>
/// 动作是规则真正改变数量、金额或限额占用的执行单元。与条件一样，动作只允许保存到 DRAFT 版本，
/// 已发布版本必须保持稳定，确保计价追踪中的动作链能够还原当时的规则配置。
/// </remarks>
public sealed class RuleActionService
{
    /// <summary>
    /// 规则动作仓储，负责读取、清空和批量写入某个版本下的动作明细。
    /// </summary>
    private readonly IRuleActionRepository _actionRepository;
    /// <summary>
    /// 规则版本仓储，用于保存动作前确认版本仍可编辑。
    /// </summary>
    private readonly IRuleVersionRepository _versionRepository;
    /// <summary>
    /// 服务日志，用于记录动作集合保存数量。
    /// </summary>
    private readonly ILogger<RuleActionService> _logger;

    /// <summary>
    /// 初始化规则动作服务。
    /// </summary>
    /// <param name="actionRepository">规则动作仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="logger">日志对象。</param>
    public RuleActionService(
        IRuleActionRepository actionRepository,
        IRuleVersionRepository versionRepository,
        ILogger<RuleActionService> logger)
    {
        _actionRepository = actionRepository;
        _versionRepository = versionRepository;
        _logger = logger;
    }

    /// <summary>
    /// 读取指定规则版本下的动作集合。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <returns>动作响应列表，按仓储层定义的排序返回。</returns>
    public async Task<IReadOnlyList<RuleActionResponse>> GetAsync(long ruleId, int versionNo)
    {
        var items = await _actionRepository.GetByRuleAndVersionAsync(ruleId, versionNo);
        return items.Select(MapToResponse).ToList();
    }

    /// <summary>
    /// 整体保存指定草稿版本的动作集合。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">动作保存请求，包含完整动作执行链。</param>
    /// <exception cref="KeyNotFoundException">规则版本不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">规则版本不是草稿状态时抛出。</exception>
    public async Task SaveAsync(long ruleId, int versionNo, RuleActionSaveRequest request)
    {
        // ========== 第一阶段：校验版本是否可编辑 ==========
        // 动作会直接影响计价输出，已发布版本必须冻结；需要调整时应创建新草稿版本再发布。
        var version = await _versionRepository.GetByRuleAndVersionAsync(ruleId, versionNo)
            ?? throw new KeyNotFoundException($"规则版本不存在: RuleId={ruleId}, VersionNo={versionNo}");

        if (version.VersionStatus != "DRAFT")
        {
            throw new InvalidOperationException($"只有草稿版本可以编辑动作, 当前状态: {version.VersionStatus}");
        }

        // ========== 第二阶段：删除旧动作 ==========
        // 动作链的排序、互斥组和错误策略共同决定执行结果。整体替换比逐项补丁更容易保证最终状态与请求一致。
        await _actionRepository.DeleteByRuleAndVersionAsync(ruleId, versionNo);

        // ========== 第三阶段：映射动作实体 ==========
        // ExecutorCode 决定运行时使用哪个 IRuleActionExecutor；ParamsJson 作为执行器私有参数保存。
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

        // ========== 第四阶段：批量写入动作集合 ==========
        // 空动作集合允许保存，表示当前草稿只做条件匹配但不改变价格；发布前可由业务流程再行校验。
        if (entities.Count > 0)
        {
            await _actionRepository.InsertBatchAsync(entities);
        }

        _logger.LogInformation("保存规则动作 RuleId={RuleId}, VersionNo={VersionNo}, Count={Count}",
            ruleId, versionNo, entities.Count);
    }

    /// <summary>
    /// 将规则动作实体映射为接口响应。
    /// </summary>
    /// <param name="entity">规则动作实体。</param>
    /// <returns>规则动作响应 DTO。</returns>
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
