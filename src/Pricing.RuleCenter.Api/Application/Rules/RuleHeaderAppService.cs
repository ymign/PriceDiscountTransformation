using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Api.Application.Background;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Api.Application.Rules;

/// <summary>
/// 规则主档应用服务，负责维护规则的基础信息和当前发布状态。
/// </summary>
/// <remarks>
/// <para>
/// 职责边界：规则主档描述"这条规则面向什么项目、属于什么类别、当前版本是多少"。
/// 条件和动作属于版本明细（由 RuleConditionAppService 和 RuleActionAppService 维护），
/// 发布状态推进由 RulePublishAppService 负责。本服务只处理主档信息的增改查，
/// 避免把版本状态机混入基础维护入口。
/// </para>
/// <para>
/// 状态字段：Status（DRAFT/PUBLISHED/DISABLED）和 CurrentVersion 由发布服务推进，
/// UpdateAsync 不允许修改这两个字段，防止绕过发布状态机。
/// </para>
/// <para>
/// 生效时间：主档的 EffectiveFrom/EffectiveTo 是规则的全局生效范围，
/// 版本创建时会继承主档的生效时间，但版本可以单独调整。
/// </para>
/// <para>
/// 缓存策略：生效规则使用 IMemoryCache 缓存，TTL 5 分钟。
/// 规则发布、停用、回滚时主动清除缓存，确保计价引擎读到最新规则集。
/// </para>
/// </remarks>
public sealed class RuleHeaderAppService
{
    /// <summary>
    /// 规则主档仓储，负责 PR_RULE_HEADER 表的分页查询、按项目查询、编码唯一性校验和主档写入。
    /// </summary>
    private readonly IRuleHeaderRepository _repository;

    /// <summary>
    /// 变更日志仓储，用于在规则主档创建和更新时写入审计记录。
    /// 每次写操作完成后，通过 InsertAsync 向 PR_RULE_CHANGE_LOG 表追加一条变更摘要，
    /// 保证规则主档的配置变更可沿"规则变更链"追溯。
    /// </summary>
    private readonly IRuleChangeLogRepository _changeLogRepository;

    /// <summary>
    /// 内存缓存，用于缓存生效规则查询结果，减少数据库访问频率。
    /// 生效规则变化频率低（仅发布/停用/回滚时变化），适合应用层缓存。
    /// </summary>
    private readonly IMemoryCache _cache;
    /// <summary>
    /// 跨实例缓存版本同步器，用于在读取生效规则前校验本机缓存是否过期。
    /// </summary>
    private readonly ICacheVersionSynchronizer _cacheVersionSynchronizer;

    /// <summary>
    /// 服务日志，用于记录规则主档新增和更新等配置变更。
    /// 主档变更会影响规则匹配结果，保留操作线索便于审计追踪。
    /// </summary>
    private readonly ILogger<RuleHeaderAppService> _logger;

    /// <summary>
    /// 生效规则缓存的统一前缀，用于登记和批量失效。
    /// </summary>
    private const string EffectiveCachePrefix = "rules:effective:";

    /// <summary>
    /// 生效规则缓存过期时间（5 分钟）。
    /// 平衡实时性和数据库压力：规则发布/停用/回滚时主动清除，
    /// 正常情况下 5 分钟 TTL 作为兜底过期保障。
    /// </summary>
    private static readonly TimeSpan EffectiveCacheDuration = TimeSpan.FromMinutes(5);

    private sealed record RuleChangeLogInput
    {
        public long RuleId { get; init; }

        public int? VersionNo { get; init; }

        public string ChangeType { get; init; } = string.Empty;

        public string ChangeSummary { get; init; } = string.Empty;

        public string? ChangedBy { get; init; }
    }

    /// <summary>
    /// 初始化规则主档服务。
    /// </summary>
    /// <param name="repository">规则主档仓储。</param>
    /// <param name="changeLogRepository">变更日志仓储，用于写入主档创建和更新的审计记录。</param>
    /// <param name="cache">内存缓存，用于缓存生效规则查询结果。</param>
    /// <param name="logger">日志对象。</param>
    public RuleHeaderAppService(
        IRuleHeaderRepository repository,
        IRuleChangeLogRepository changeLogRepository,
        IMemoryCache cache,
        ICacheVersionSynchronizer cacheVersionSynchronizer,
        ILogger<RuleHeaderAppService> logger)
    {
        _repository = repository;
        _changeLogRepository = changeLogRepository;
        _cache = cache;
        _cacheVersionSynchronizer = cacheVersionSynchronizer;
        _logger = logger;
    }

    /// <summary>
    /// 分页查询规则主档。
    /// </summary>
    /// <param name="request">分页与筛选条件，支持按项目、状态和规则分类过滤。</param>
    /// <returns>规则主档分页结果。</returns>
    public async Task<PagedResponse<RuleHeaderResponse>> GetPagedAsync(RuleHeaderPagedRequest request)
    {
        // 查询条件保持透传，具体 SQL 拼装和分页计算由仓储层负责，服务层只负责 DTO 边界。
        var (items, total) = await _repository.GetPagedAsync(
            request.ItemCode, request.Status, request.Category,
            request.PageIndex, request.PageSize);

        return new PagedResponse<RuleHeaderResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            Total = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// 按主键读取规则主档。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>找到时返回规则主档；不存在时返回 <c>null</c>。</returns>
    public async Task<RuleHeaderResponse?> GetByIdAsync(long ruleId)
    {
        var entity = await _repository.GetByIdAsync(ruleId);
        return entity is null ? null : MapToResponse(entity);
    }

    /// <summary>
    /// 按项目编码读取关联规则。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>关联到该项目的规则主档列表。</returns>
    public async Task<IReadOnlyList<RuleHeaderResponse>> GetByItemCodeAsync(string itemCode)
    {
        var items = await _repository.GetByItemCodeAsync(itemCode);
        return items.Select(MapToResponse).ToList();
    }

    /// <summary>
    /// 查询当前时间点所有已发布且在有效期内的规则。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 用于 GET <c>/api/pricing/rules/effective</c> 端点，返回计价引擎当前可参与匹配的规则集合。
    /// 结果按优先级升序排列，与计价引擎匹配顺序一致。
    /// </para>
    /// <para>
    /// 缓存策略：使用 IMemoryCache 缓存，TTL 5 分钟。
    /// 缓存 key 格式为 <c>rules:effective:{itemCode}:{timeKey}</c>（itemCode 为空时用 "all"）。
    /// 规则发布、停用、回滚时通过 <see cref="ClearEffectiveCache"/> 主动清除。
    /// </para>
    /// </remarks>
    /// <param name="itemCode">
    /// 项目编码（选填）。为空时返回所有生效规则；非空时按项目编码进一步过滤。
    /// </param>
    /// <returns>当前时间点生效的规则主档列表，按优先级升序排列。</returns>
    public async Task<IReadOnlyList<RuleHeaderResponse>> GetEffectiveAsync(
        string? itemCode = null,
        DateTime? chargeTime = null)
    {
        await _cacheVersionSynchronizer.SyncAsync();
        var businessTime = chargeTime ?? DateTime.Now;

        // 构造缓存 key：itemCode 为空时用 "all"，避免 null 参与 key 拼接产生歧义。
        var itemKey = string.IsNullOrWhiteSpace(itemCode) ? "all" : itemCode.Trim().ToUpperInvariant();
        var timeKey = chargeTime.HasValue ? businessTime.ToString("yyyyMMddHHmmss") : "current";
        var cacheKey = $"{EffectiveCachePrefix}{itemKey}:{timeKey}";

        // 尝试从缓存读取；缓存未命中时查询数据库并写入缓存。
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<RuleHeaderResponse>? cached) && cached is not null)
        {
            return cached;
        }

        // 使用请求指定的业务时间查询；未指定时才回退当前时间，避免历史回放误用系统当前时间。
        var entities = await _repository.GetEffectiveAsync(businessTime);

        // 如果指定了 itemCode，进一步过滤。
        var filtered = string.IsNullOrWhiteSpace(itemCode)
            ? entities
            : entities.Where(r => string.Equals(r.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase)).ToList();

        var result = filtered.Select(MapToResponse).ToList();

        // 写入缓存，TTL 5 分钟。
        _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = EffectiveCacheDuration
        });
        EffectiveRuleCacheKeys.Track(cacheKey);

        return result;
    }

    /// <summary>
    /// 清除所有生效规则缓存。
    /// </summary>
    /// <remarks>
    /// 在规则发布、停用、回滚时由 RulePublishAppService 调用，确保计价引擎读到最新规则集。
    /// IMemoryCache 不提供按前缀批量删除，因此这里通过移除已知 key 实现。
    /// 如果后续规则项自增长，可考虑使用 CacheEntryRegistration 或改用 IDistributedCache。
    /// </remarks>
    public void ClearEffectiveCache()
    {
        var removed = EffectiveRuleCacheKeys.Clear(_cache);
        _logger.LogInformation("已清除生效规则缓存，共清理 {Count} 个缓存键", removed);
    }

    /// <summary>
    /// 创建规则主档。
    /// </summary>
    /// <param name="request">规则主档新增请求。</param>
    /// <returns>新增规则主键。</returns>
    /// <exception cref="BizException">规则编码已存在时抛出结构化业务错误。</exception>
    public async Task<long> CreateAsync(RuleHeaderCreateRequest request)
    {
        // ========== 第一阶段：校验稳定业务编码 ==========
        // RuleCode 是规则在配置、审计和外部沟通中的稳定标识，重复会导致发布历史和追踪记录难以解释。
        if (await _repository.ExistsAsync(request.RuleCode))
        {
            throw new BizException(
                BizErrorCode.RuleCodeDuplicate,
                409,
                $"规则编码已存在: {request.RuleCode}");
        }

        // ========== 第二阶段：创建草稿主档 ==========
        // 主档创建后还没有版本、条件和动作，因此 CurrentVersion 为 0，状态保持 DRAFT。
        var now = DateTime.Now;
        var entity = new RuleAggregate
        {
            RuleCode = request.RuleCode,
            RuleName = request.RuleName,
            RuleCategory = request.RuleCategory,
            RuleScope = request.RuleScope,
            ItemCode = request.ItemCode,
            ItemName = request.ItemName,
            GroupCode = request.GroupCode,
            Priority = request.Priority,
            RollbackMode = request.RollbackMode,
            CurrentVersion = 0,
            Status = "DRAFT",
            IsEnabled = "Y",
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            Remark = request.Remark,
            CreatedBy = request.CreatedBy,
            CreatedAt = now,
            UpdatedAt = now
        };

        // ========== 第三阶段：写入主档并记录日志 ==========
        // 版本草稿由 RuleVersionAppService 单独创建，保持"主档"和"版本内容"的职责边界清晰。
        var id = await _repository.InsertAsync(entity);
        _logger.LogInformation("新增规则 {RuleCode}, ID={RuleId}", request.RuleCode, id);

        // ========== 第四阶段：写入变更日志 ==========
        // 主档创建是规则生命周期的起点，必须记录审计日志，便于后续追溯规则来源。
        // 变更日志写入失败不应阻断主业务流程，因此使用 try-catch 包裹。
        await TryWriteChangeLogAsync(new RuleChangeLogInput
        {
            RuleId = id,
            ChangeType = "CREATE_RULE",
            ChangeSummary = $"创建规则主档：编码={request.RuleCode}，名称={request.RuleName}",
            ChangedBy = request.CreatedBy
        });

        return id;
    }

    /// <summary>
    /// 更新规则主档基础信息。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">主档更新请求；不包含当前版本字段，避免绕过发布状态机。</param>
    /// <exception cref="BizException">规则不存在或更新边界不允许时抛出结构化业务错误。</exception>
    public async Task UpdateAsync(long ruleId, RuleHeaderUpdateRequest request)
    {
        // ========== 第一阶段：读取现有主档 ==========
        // 不存在时直接返回业务错误，避免后续 Update 影响 0 行造成调用方误以为成功。
        var entity = await _repository.GetByIdAsync(ruleId)
            ?? throw new BizException(
                BizErrorCode.RuleNotFound,
                404,
                $"规则不存在: {ruleId}");

        await EnsureNoPendingPublishApprovalAsync(ruleId, entity.CurrentVersion);
        ValidatePublishedRuleUpdate(entity, request);

        // ========== 第二阶段：只更新主档可维护字段 ==========
        // CurrentVersion、Status、IsEnabled 由发布/停用服务推进，不能在普通编辑入口里直接改。
        entity.RuleName = request.RuleName;
        entity.RuleCategory = request.RuleCategory;
        entity.RuleScope = request.RuleScope;
        entity.ItemCode = request.ItemCode;
        entity.ItemName = request.ItemName;
        entity.GroupCode = request.GroupCode;
        entity.Priority = request.Priority;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.RollbackMode = request.RollbackMode;
        entity.Remark = request.Remark;
        entity.UpdatedBy = request.UpdatedBy;
        entity.UpdatedAt = DateTime.Now;

        await _repository.UpdateAsync(entity);
        _logger.LogInformation("更新规则主档 RuleId={RuleId}", ruleId);
        ClearEffectiveCache();

        // ========== 第三阶段：写入变更日志 ==========
        // 主档更新可能影响规则匹配条件（如项目编码、生效时间、优先级等），
        // 必须记录审计日志便于追溯哪次修改导致了匹配结果变化。
        await TryWriteChangeLogAsync(new RuleChangeLogInput
        {
            RuleId = ruleId,
            ChangeType = "UPDATE_RULE",
            ChangeSummary = $"更新规则主档：名称={request.RuleName}，项目={request.ItemCode}，优先级={request.Priority}",
            ChangedBy = request.UpdatedBy
        });
    }

    private static void ValidatePublishedRuleUpdate(RuleAggregate entity, RuleHeaderUpdateRequest request)
    {
        if (!string.Equals(entity.Status, RuleStatusCodes.Published, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // 已发布规则已经进入计价匹配链路。普通更新入口只允许修改展示和说明字段；
        // 会改变匹配范围、冲突校验或降级策略的字段必须通过新版本发布，避免绕过发布审查。
        var immutableFieldChanged =
            !StringEquals(entity.RuleCategory, request.RuleCategory) ||
            !StringEquals(entity.RuleScope, request.RuleScope) ||
            !StringEquals(entity.ItemCode, request.ItemCode) ||
            !StringEquals(entity.GroupCode, request.GroupCode) ||
            entity.Priority != request.Priority ||
            entity.EffectiveFrom != request.EffectiveFrom ||
            entity.EffectiveTo != request.EffectiveTo ||
            !StringEquals(entity.RollbackMode, request.RollbackMode);

        if (immutableFieldChanged)
        {
            throw new BizException(
                BizErrorCode.VersionStatusNotAllowed,
                409,
                "已发布规则不允许在主档更新入口修改匹配关键字段，请创建新版本后发布");
        }
    }

    private static bool StringEquals(string? left, string? right)
    {
        return string.Equals(
            NormalizeString(left),
            NormalizeString(right),
            StringComparison.OrdinalIgnoreCase);
    }

    private static string? NormalizeString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private async Task EnsureNoPendingPublishApprovalAsync(long ruleId, int versionNo)
    {
        if (versionNo <= 0)
        {
            return;
        }

        var logs = await _changeLogRepository.GetByRuleIdAsync(ruleId);
        var latestApprovalSubmit = logs
            .Where(log => log.VersionNo == versionNo)
            .Where(log => string.Equals(log.ChangeType, "SUBMIT_APPROVAL", StringComparison.OrdinalIgnoreCase))
            .Where(log => log.ChangeSummary?.Contains("PUBLISH", StringComparison.OrdinalIgnoreCase) == true)
            .OrderByDescending(log => log.ChangedAt)
            .FirstOrDefault();

        if (latestApprovalSubmit is not null)
        {
            throw new BizException(
                BizErrorCode.ApprovalPendingEditNotAllowed,
                409,
                $"RuleId={ruleId}, VersionNo={versionNo} 已提交发布审批，审批完成前不允许继续编辑");
        }
    }

    /// <summary>
    /// 安全写入变更日志，失败时仅记录警告日志，不阻断主业务流程。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 审计日志是"尽力而为"的辅助功能。如果数据库写入失败（如连接超时、序列耗尽），
    /// 不应导致规则主档的创建或更新操作回滚，否则会因审计功能的故障影响核心配置流程。
    /// </para>
    /// <para>
    /// 失败后通过 ILogger 输出 WARN 级别日志，运维人员可通过日志监控发现审计丢失，
    /// 并通过数据库层面的补偿脚本恢复缺失的变更记录。
    /// </para>
    /// </remarks>
    /// <param name="input">规则变更日志写入上下文。</param>
    private async Task TryWriteChangeLogAsync(RuleChangeLogInput input)
    {
        try
        {
            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = input.RuleId,
                VersionNo = input.VersionNo,
                ChangeType = input.ChangeType,
                ChangeSummary = input.ChangeSummary,
                ChangedBy = input.ChangedBy ?? "SYSTEM",
                ChangedAt = DateTime.Now,
                SourceSystem = "API"
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "写入变更日志失败 RuleId={RuleId}, ChangeType={ChangeType}",
                input.RuleId, input.ChangeType);
        }
    }

    /// <summary>
    /// 将规则主档实体映射为接口响应。
    /// </summary>
    /// <remarks>
    /// 映射包含全部主档字段，包括创建/更新人和时间等审计字段，
    /// 供前端规则详情页完整展示。Status 和 IsEnabled 保留原始编码值。
    /// </remarks>
    /// <param name="entity">规则主档实体，来自 PR_RULE_HEADER 表。</param>
    /// <returns>规则主档响应 DTO，包含完整的主档信息和审计字段。</returns>
    private static RuleHeaderResponse MapToResponse(RuleAggregate entity)
    {
        return new RuleHeaderResponse
        {
            RuleId = entity.RuleId,
            RuleCode = entity.RuleCode,
            RuleName = entity.RuleName,
            RuleCategory = entity.RuleCategory,
            RuleScope = entity.RuleScope,
            ItemCode = entity.ItemCode,
            ItemName = entity.ItemName,
            GroupCode = entity.GroupCode,
            Priority = entity.Priority,
            CurrentVersion = entity.CurrentVersion,
            Status = entity.Status,
            IsEnabled = entity.IsEnabled,
            EffectiveFrom = entity.EffectiveFrom,
            EffectiveTo = entity.EffectiveTo,
            RollbackMode = entity.RollbackMode,
            Remark = entity.Remark,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}


