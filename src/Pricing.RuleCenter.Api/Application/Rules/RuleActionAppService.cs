using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Api.Application.Rules;

/// <summary>
/// 规则动作应用服务，负责维护指定规则版本下的动作执行链。
/// </summary>
/// <remarks>
/// <para>
/// 职责边界：动作是规则真正改变数量、金额或限额占用的执行单元。与条件一样，
/// 动作只允许保存到 DRAFT 版本，已发布版本必须保持稳定，
/// 确保计价追踪中的动作链能够还原当时的规则配置。
/// </para>
/// <para>
/// 保存策略：与条件服务相同，采用"先删除后重建"模式整体替换。
/// 动作链的排序、互斥组和错误策略共同决定执行结果，逐项补丁容易产生不一致状态。
/// </para>
/// <para>
/// 关键字段说明：
/// - ExecutorCode：决定运行时使用哪个 IRuleActionExecutor 执行器
/// - ExclusiveGroup：互斥组编码，同组动作只执行第一个匹配的（如金额折扣和数量折扣互斥）
/// - OnError：执行失败策略（STOP/CONTINUE/SKIP），控制动作链是否继续
/// - SortNo：动作执行顺序，数值小的先执行
/// </para>
/// </remarks>
public sealed class RuleActionAppService
{
    /// <summary>
    /// 工作单元，负责把“删除旧动作 + 插入新动作”放进同一数据库事务。
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// 规则动作仓储，负责 PR_RULE_ACTION 表的读取、按版本清空和批量写入。
    /// </summary>
    private readonly IRuleActionRepository _actionRepository;

    /// <summary>
    /// 规则版本仓储，用于保存动作前确认版本仍处于 DRAFT 状态，
    /// 防止对已发布版本做意外修改。
    /// </summary>
    private readonly IRuleVersionRepository _versionRepository;

    /// <summary>
    /// 变更日志仓储，用于在动作集合保存时写入审计记录。
    /// 动作决定了规则的计价行为（公式计算、数量限制、金额限制等），
    /// 每次保存都必须记录变更日志，确保动作链的修改可追溯。
    /// </summary>
    private readonly IRuleChangeLogRepository _changeLogRepository;

    /// <summary>
    /// 服务日志，用于记录动作集合保存数量，便于追溯某次保存是否清空了所有动作。
    /// </summary>
    private readonly ILogger<RuleActionAppService> _logger;

    /// <summary>
    /// 初始化规则动作服务。
    /// </summary>
    /// <param name="unitOfWork">工作单元，用于保证整体替换保存的事务一致性。</param>
    /// <param name="actionRepository">规则动作仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="changeLogRepository">变更日志仓储，用于写入动作保存的审计记录。</param>
    /// <param name="logger">日志对象。</param>
    public RuleActionAppService(
        IUnitOfWork unitOfWork,
        IRuleActionRepository actionRepository,
        IRuleVersionRepository versionRepository,
        IRuleChangeLogRepository changeLogRepository,
        ILogger<RuleActionAppService> logger)
    {
        _unitOfWork = unitOfWork;
        _actionRepository = actionRepository;
        _versionRepository = versionRepository;
        _changeLogRepository = changeLogRepository;
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

        // ========== 第二阶段：映射动作实体 ==========
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

        // ========== 第三阶段：事务性整体替换 ==========
        // 删除和重建必须处于同一事务中，否则批量插入失败时，会把原草稿动作链整批清空。
        await _unitOfWork.BeginAsync();
        try
        {
            await _actionRepository.DeleteByRuleAndVersionAsync(ruleId, versionNo);

            // 空动作集合允许保存，表示当前草稿只做条件匹配但不改变价格；发布前可由业务流程再行校验。
            if (entities.Count > 0)
            {
                await _actionRepository.InsertBatchAsync(entities);
            }

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        _logger.LogInformation("保存规则动作 RuleId={RuleId}, VersionNo={VersionNo}, Count={Count}",
            ruleId, versionNo, entities.Count);

        // ========== 第四阶段：写入变更日志 ==========
        // 动作决定了规则的计价行为（公式计算、数量限制、金额限制、换算等），
        // 每次整体替换都必须记录审计日志，确保动作链的修改可追溯到具体保存操作。
        // 变更日志保持 best-effort 旁路语义，不反向影响已经提交成功的主配置保存。
        await TryWriteChangeLogAsync(ruleId, versionNo, "SAVE_ACTIONS",
            $"保存规则动作：共 {entities.Count} 个动作，RuleId={ruleId}，VersionNo={versionNo}");
    }

    /// <summary>
    /// 安全写入变更日志，失败时仅记录警告日志，不阻断主业务流程。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 动作保存是规则配置的核心操作，审计日志写入失败不应导致动作保存回滚。
    /// try-catch 确保即使 PR_RULE_CHANGE_LOG 表不可用，动作配置仍可正常进行。
    /// </para>
    /// </remarks>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="changeType">变更类型编码，如 SAVE_ACTIONS。</param>
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
                ChangedAt = DateTime.Now,
                SourceSystem = "API"
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "写入变更日志失败 RuleId={RuleId}, ChangeType={ChangeType}", ruleId, changeType);
        }
    }

    /// <summary>
    /// 将规则动作实体映射为接口响应。
    /// </summary>
    /// <remarks>
    /// ParamsJson 原样返回，不在此处解析。该字段保存执行器私有参数（如公式参数、限额值等），
    /// 结构由 ExecutorCode 对应的 IRuleActionExecutor 定义。
    /// ExclusiveGroup 为 null 表示该动作不属于任何互斥组，总是执行。
    /// </remarks>
    /// <param name="entity">规则动作实体，来自 PR_RULE_ACTION 表。</param>
    /// <returns>规则动作响应 DTO，包含执行器编码、参数、互斥组和错误策略。</returns>
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


