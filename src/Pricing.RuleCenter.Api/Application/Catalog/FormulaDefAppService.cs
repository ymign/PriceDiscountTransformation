using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Api.Application.Catalog;

/// <summary>
/// 公式定义应用服务，负责维护动作执行器与配置参数之间的注册关系。
/// </summary>
/// <remarks>
/// <para>
/// 职责边界：公式定义不是直接计算价格的地方。它保存"公式编码、执行器编码、参数结构"等元数据，
/// 供规则动作配置页面约束用户输入，并在计价引擎运行时通过 ExecutorCode 找到对应的
/// IFormulaExecutor 实现完成实际计算。
/// </para>
/// <para>
/// 与规则动作的关系：PR_RULE_ACTION 表通过 ExecutorCode 引用公式定义。
/// 公式定义的启用/停用影响配置页面的可选范围，但不影响已发布规则中已保存的动作。
/// </para>
/// <para>
/// 参数结构：ParamSchemaJson 字段保存 JSON Schema 或自由格式的参数描述，
/// 用于前端配置页面动态渲染参数输入框。计价引擎运行时按 ExecutorCode 分发，
/// 各执行器自行解析 ParamsJson。
/// </para>
/// </remarks>
public sealed class FormulaDefAppService
{
    /// <summary>
    /// 公式定义仓储，负责 PR_FORMULA_DEF 表的查询、唯一性判断和启停状态更新。
    /// </summary>
    private readonly IFormulaDefRepository _repository;

    /// <summary>
    /// 变更日志仓储，用于在公式定义创建、更新和状态切换时写入审计记录。
    /// 公式定义决定了规则动作的计算能力，任何变更都可能影响计价结果，
    /// 因此需要通过变更日志保留完整的操作审计链。
    /// </summary>
    private readonly IRuleChangeLogRepository _changeLogRepository;

    /// <summary>
    /// 服务日志，用于记录新增公式、更新定义和切换启停状态等配置变更。
    /// 公式新增会改变规则动作可选项，切换状态会限制或放开配置页面的选择范围。
    /// </summary>
    private readonly ILogger<FormulaDefAppService> _logger;

    /// <summary>
    /// 初始化公式定义服务。
    /// </summary>
    /// <param name="repository">公式定义仓储，用于隔离 PR_FORMULA_DEF 表的持久化实现。</param>
    /// <param name="changeLogRepository">变更日志仓储，用于写入公式增删改的审计记录。</param>
    /// <param name="logger">日志对象，用于输出公式新增和状态切换的审计线索。</param>
    public FormulaDefAppService(
        IFormulaDefRepository repository,
        IRuleChangeLogRepository changeLogRepository,
        ILogger<FormulaDefAppService> logger)
    {
        _repository = repository;
        _changeLogRepository = changeLogRepository;
        _logger = logger;
    }

    /// <summary>
    /// 读取全部公式定义。
    /// </summary>
    /// <returns>公式定义响应列表，通常供前端动作配置页面选择。</returns>
    public async Task<IReadOnlyList<FormulaDefResponse>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToResponse).ToList();
    }

    /// <summary>
    /// 按主键读取公式定义。
    /// </summary>
    /// <param name="formulaId">公式定义主键。</param>
    /// <returns>找到时返回公式定义；不存在时返回 <c>null</c>。</returns>
    public async Task<FormulaDefResponse?> GetByIdAsync(long formulaId)
    {
        var entity = await _repository.GetByIdAsync(formulaId);
        return entity is null ? null : MapToResponse(entity);
    }

    /// <summary>
    /// 新增公式定义。
    /// </summary>
    /// <param name="request">新增请求，包含公式编码、执行器编码、参数结构和说明。</param>
    /// <returns>新增公式定义主键。</returns>
    /// <exception cref="BizException">公式编码已存在或公式定义不存在时抛出结构化业务错误。</exception>
    public async Task<long> CreateAsync(FormulaDefCreateRequest request)
    {
        // ========== 第一阶段：校验公式编码唯一 ==========
        // 公式编码会被规则动作引用，重复编码会导致页面选择和运行时解释都产生歧义。
        var existing = await _repository.GetByCodeAsync(request.FormulaCode);
        if (existing is not null)
        {
            throw new BizException(
                BizErrorCode.ResourceAlreadyExists,
                409,
                $"公式编码已存在: {request.FormulaCode}");
        }

        // ========== 第二阶段：组装定义实体 ==========
        // ParamSchemaJson 只保存参数结构，不在这里解析执行；真正计算由执行器完成。
        var entity = new FormulaDef
        {
            FormulaCode = request.FormulaCode,
            FormulaName = request.FormulaName,
            FormulaDesc = request.FormulaDesc,
            ExecutorCode = request.ExecutorCode,
            ParamSchemaJson = request.ParamSchemaJson,
            IsEnabled = "Y",
            Remark = request.Remark
        };

        // ========== 第三阶段：写入并输出审计日志 ==========
        // 公式新增会改变规则动作可选能力，需要保留操作线索。
        var id = await _repository.InsertAsync(entity);
        _logger.LogInformation("新增公式定义 {FormulaCode}, ID={FormulaId}",
            request.FormulaCode, id);

        // ========== 第四阶段：写入变更日志 ==========
        // 公式定义决定了规则动作的计算能力，新增公式会扩大配置页面的可选范围，
        // 必须记录审计日志便于追溯"某个公式是什么时候加入系统的"。
        await TryWriteChangeLogAsync("CREATE_FORMULA",
            $"新增公式定义：编码={request.FormulaCode}，名称={request.FormulaName}，执行器={request.ExecutorCode}");

        return id;
    }

    /// <summary>
    /// 更新公式定义的展示信息、执行器编码或参数结构。
    /// </summary>
    /// <remarks>
    /// 公式编码（FormulaCode）是规则动作的外部引用键，更新时不允许修改，避免破坏已发布规则中的引用关系。
    /// 可调整的字段包括：名称、描述、执行器编码和参数结构。修改执行器编码会影响运行时的公式计算行为，
    /// 操作人员应确认新执行器已实现并注册。
    /// </remarks>
    /// <param name="formulaId">公式定义主键。</param>
    /// <param name="request">更新请求；不包含公式编码字段，避免外部修改稳定业务键。</param>
    /// <exception cref="BizException">公式定义不存在时抛出结构化业务错误。</exception>
    public async Task UpdateAsync(long formulaId, FormulaDefUpdateRequest request)
    {
        // 公式编码是外部引用键，更新时保持稳定；可调整名称、描述、执行器和参数结构。
        // 如果执行器编码变更，已发布规则中引用该公式的动作在下次发布时会使用新执行器。
        var entity = await _repository.GetByIdAsync(formulaId)
            ?? throw new BizException(
                BizErrorCode.FormulaNotFound,
                404,
                $"公式定义不存在: {formulaId}");

        entity.FormulaName = request.FormulaName;
        entity.FormulaDesc = request.FormulaDesc;
        entity.ExecutorCode = request.ExecutorCode;
        entity.ParamSchemaJson = request.ParamSchemaJson;
        entity.Remark = request.Remark;

        await _repository.UpdateAsync(entity);
        _logger.LogInformation("更新公式定义 {FormulaId}, 编码={FormulaCode}", formulaId, entity.FormulaCode);

        // ========== 写入变更日志 ==========
        // 修改执行器编码会改变运行时的公式计算行为，修改参数结构会影响前端配置页面，
        // 必须记录审计日志便于追溯公式定义的变更历史。
        await TryWriteChangeLogAsync("UPDATE_FORMULA",
            $"更新公式定义：编码={entity.FormulaCode}，名称={request.FormulaName}，执行器={request.ExecutorCode}");
    }

    /// <summary>
    /// 切换公式定义启用状态。
    /// </summary>
    /// <param name="formulaId">公式定义主键。</param>
    /// <exception cref="BizException">公式定义不存在时抛出结构化业务错误。</exception>
    public async Task ToggleAsync(long formulaId)
    {
        // ========== 第一阶段：读取当前状态 ==========
        // 这里采用显式查询后切换，便于返回"不存在"的业务错误，而不是静默影响 0 行。
        var entity = await _repository.GetByIdAsync(formulaId)
            ?? throw new BizException(
                BizErrorCode.FormulaNotFound,
                404,
                $"公式定义不存在: {formulaId}");

        // ========== 第二阶段：在 Y/N 之间翻转 ==========
        // 停用后历史规则中的动作仍保留，但配置页面可以据此限制新增选择。
        var newEnabled = entity.IsEnabled == "Y" ? "N" : "Y";
        await _repository.SetEnabledAsync(formulaId, newEnabled);

        _logger.LogInformation("切换公式 {FormulaId} 状态为 {Enabled}", formulaId, newEnabled);

        // ========== 第三阶段：写入变更日志 ==========
        // 状态切换会限制或放开配置页面的公式选择范围，停用可能导致已有规则在下次发布时无法引用该公式，
        // 必须记录审计日志便于追溯"公式何时被禁用或重新启用"。
        var stateLabel = newEnabled == "Y" ? "启用" : "停用";
        await TryWriteChangeLogAsync("TOGGLE_FORMULA",
            $"{stateLabel}公式定义：编码={entity.FormulaCode}，名称={entity.FormulaName}");
    }

    /// <summary>
    /// 安全写入变更日志，失败时仅记录警告日志，不阻断主业务流程。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 公式定义变更是低频配置操作，审计日志写入失败不应导致公式保存或状态切换回滚。
    /// try-catch 确保即使 PR_RULE_CHANGE_LOG 表不可用，公式维护仍可正常进行。
    /// </para>
    /// <para>
    /// 公式定义操作不关联特定规则，因此 RuleId 使用 0、VersionNo 使用 null。
    /// </para>
    /// </remarks>
    /// <param name="changeType">变更类型编码，如 CREATE_FORMULA、UPDATE_FORMULA、TOGGLE_FORMULA。</param>
    /// <param name="changeSummary">人可读的变更摘要，含公式编码、名称和执行器等关键参数。</param>
    private async Task TryWriteChangeLogAsync(string changeType, string changeSummary)
    {
        try
        {
            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = 0,
                VersionNo = null,
                ChangeType = changeType,
                ChangeSummary = changeSummary,
                ChangedBy = "SYSTEM",
                ChangedAt = DateTime.Now,
                SourceSystem = "API"
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "写入变更日志失败 ChangeType={ChangeType}", changeType);
        }
    }

    /// <summary>
    /// 将公式定义实体映射为接口返回对象。
    /// </summary>
    /// <remarks>
    /// 映射时保留 IsEnabled 的 "Y"/"N" 原始值，前端据此决定在下拉框中是否可选。
    /// ParamSchemaJson 原样返回，不在此处解析或校验。
    /// </remarks>
    /// <param name="entity">数据库中的公式定义实体，来自 PR_FORMULA_DEF 表。</param>
    /// <returns>公式定义响应 DTO，包含执行器编码和参数结构供前端配置页面使用。</returns>
    private static FormulaDefResponse MapToResponse(FormulaDef entity)
    {
        return new FormulaDefResponse
        {
            FormulaId = entity.FormulaId,
            FormulaCode = entity.FormulaCode,
            FormulaName = entity.FormulaName,
            FormulaDesc = entity.FormulaDesc,
            ExecutorCode = entity.ExecutorCode,
            ParamSchemaJson = entity.ParamSchemaJson,
            IsEnabled = entity.IsEnabled,
            Remark = entity.Remark
        };
    }
}


