using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Application.Rules;

/// <summary>
/// 规则条件应用服务，负责维护指定规则版本下的匹配条件集合。
/// </summary>
/// <remarks>
/// <para>
/// 职责边界：条件是版本内容的一部分，只允许在 DRAFT 版本中整体保存。
/// 已发布版本必须保持可追溯快照，因此不允许对 PUBLISHED、DISABLED 或 ROLLED_BACK 版本做局部修改。
/// </para>
/// <para>
/// 保存策略：SaveAsync 采用"先删除后重建"模式。前端通常以完整条件树提交，
/// 逐项增删改合并逻辑复杂且容易遗漏排序号和分组关系。整体替换能保证最终状态与请求完全一致。
/// </para>
/// <para>
/// 条件分组：ConditionGroup 字段用于表达同组条件之间的 AND/OR 关系，
/// 具体求值逻辑由 RuleMatchService 中的 IRuleConditionEvaluator 执行器负责。
/// </para>
/// </remarks>
public sealed class RuleConditionAppService
{
    /// <summary>
    /// 工作单元，负责把“删除旧条件 + 插入新条件”放进同一数据库事务。
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// 规则条件仓储，负责 PR_RULE_CONDITION 表的读取、按版本清空和批量写入。
    /// </summary>
    private readonly IRuleConditionRepository _conditionRepository;

    /// <summary>
    /// 规则版本仓储，用于保存条件前校验版本存在且仍处于 DRAFT 状态，
    /// 防止对已发布版本做意外修改。
    /// </summary>
    private readonly IRuleVersionRepository _versionRepository;

    /// <summary>
    /// 变更日志仓储，用于在条件集合保存时写入审计记录。
    /// 条件决定了规则的匹配范围（项目、场景、部位、时间等），
    /// 每次保存都必须记录变更日志，确保条件维度的修改可追溯。
    /// </summary>
    private readonly IRuleChangeLogRepository _changeLogRepository;
    private readonly RuleEditGuard _editGuard;
    private readonly IClock _clock;

    /// <summary>
    /// 服务日志，用于记录条件集合保存数量，便于追溯某次保存是否清空了所有条件。
    /// </summary>
    private readonly ILogger<RuleConditionAppService> _logger;

    /// <summary>
    /// 初始化规则条件服务。
    /// </summary>
    /// <param name="unitOfWork">工作单元，用于保证整体替换保存的事务一致性。</param>
    /// <param name="conditionRepository">规则条件仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="changeLogRepository">变更日志仓储，用于写入条件保存的审计记录。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    /// <param name="logger">日志对象。</param>
    /// <param name="editGuard">规则编辑保护器；为空时基于变更日志仓储创建默认保护器。</param>
    public RuleConditionAppService(
        IUnitOfWork unitOfWork,
        IRuleConditionRepository conditionRepository,
        IRuleVersionRepository versionRepository,
        IRuleChangeLogRepository changeLogRepository,
        IClock clock,
        ILogger<RuleConditionAppService> logger,
        RuleEditGuard? editGuard = null)
    {
        _unitOfWork = unitOfWork;
        _conditionRepository = conditionRepository;
        _versionRepository = versionRepository;
        _changeLogRepository = changeLogRepository;
        _clock = clock;
        _logger = logger;
        _editGuard = editGuard ?? new RuleEditGuard(changeLogRepository);
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
    /// <exception cref="BizException">规则版本不存在或不是草稿状态时抛出结构化业务错误。</exception>
    public async Task SaveAsync(long ruleId, int versionNo, RuleConditionSaveRequest request)
    {
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

        // 版本状态必须在事务内重新读取并加锁。
        // 否则会出现：事务外看到 DRAFT，但正式删除/重建时该版本已被发布，导致已发布版本内容被篡改。
        await _unitOfWork.BeginAsync();
        try
        {
            var version = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, versionNo)
                ?? throw new BizException(
                    BizErrorCode.RuleVersionNotFound,
                    404,
                    $"规则版本不存在: RuleId={ruleId}, VersionNo={versionNo}");

            if (version.VersionStatus != VersionStatusCodes.Draft)
            {
                throw new BizException(
                    BizErrorCode.VersionStatusNotAllowed,
                    409,
                    $"只有草稿版本可以编辑条件, 当前状态: {version.VersionStatus}");
            }

            await _editGuard.EnsureNoPendingPublishApprovalAsync(ruleId, versionNo);
            await _conditionRepository.DeleteByRuleAndVersionAsync(ruleId, versionNo);

            // 空集合也是合法保存结果，表示该版本暂时没有额外条件，因此不能强行报错。
            if (entities.Count > 0)
            {
                await _conditionRepository.InsertBatchAsync(entities);
            }

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        _logger.LogInformation("保存规则条件 规则ID={RuleId}, 版本号={VersionNo}, 数量={Count}",
            ruleId, versionNo, entities.Count);

        // 条件决定了规则的匹配范围（项目、场景、部位、时间等），
        // 每次整体替换都必须记录审计日志，确保条件维度的修改可追溯到具体保存操作。
        // 变更日志保持 best-effort 旁路语义，不反向影响已经提交成功的主配置保存。
        await TryWriteChangeLogAsync(ruleId, versionNo, "SAVE_CONDITIONS",
            $"保存规则条件：共 {entities.Count} 个条件，RuleId={ruleId}，VersionNo={versionNo}");
    }

    /// <summary>
    /// 安全写入变更日志，失败时仅记录警告日志，不阻断主业务流程。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 条件保存是规则配置的核心操作，审计日志写入失败不应导致条件保存回滚。
    /// try-catch 确保即使 PR_RULE_CHANGE_LOG 表不可用，条件配置仍可正常进行。
    /// </para>
    /// </remarks>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="changeType">变更类型编码，如 SAVE_CONDITIONS。</param>
    /// <param name="changeSummary">人可读的变更摘要，含保存数量和规则/版本标识。</param>
    private async Task TryWriteChangeLogAsync(long ruleId, int versionNo, string changeType, string changeSummary)
    {
        try
        {
            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = ruleId,
                VersionNo = versionNo,
                ChangeType = changeType,
                ChangeSummary = changeSummary,
                ChangedBy = "SYSTEM",
                ChangedAt = _clock.Now,
                SourceSystem = "API"
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "写入变更日志失败 规则ID={RuleId}, 变更类型={ChangeType}", ruleId, changeType);
        }
    }

    /// <summary>
    /// 将规则条件实体映射为接口响应。
    /// </summary>
    /// <remarks>
    /// ParamsJson 字段原样返回，不在此处解析。该字段保存条件执行器的私有参数，
    /// 结构由 ConditionType 对应的 IRuleConditionEvaluator 定义。
    /// </remarks>
    /// <param name="entity">规则条件实体，来自 PR_RULE_CONDITION 表。</param>
    /// <returns>规则条件响应 DTO，包含条件类型、操作符、左键和右值。</returns>
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
