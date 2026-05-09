using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

/// <summary>
/// 规则主档应用服务，负责维护规则的基础信息和当前发布状态。
/// </summary>
/// <remarks>
/// 规则主档描述“这条规则面向什么项目、属于什么类别、当前版本是多少”。条件和动作属于版本明细，
/// 发布服务负责推进版本状态，因此本服务只处理主档信息的增改查，避免把版本状态机混入基础维护入口。
/// </remarks>
public sealed class RuleHeaderService
{
    /// <summary>
    /// 规则主档仓储，负责分页、按项目查询、编码唯一性校验和主档写入。
    /// </summary>
    private readonly IRuleHeaderRepository _repository;
    /// <summary>
    /// 服务日志，用于记录规则主档新增等配置变更。
    /// </summary>
    private readonly ILogger<RuleHeaderService> _logger;

    /// <summary>
    /// 初始化规则主档服务。
    /// </summary>
    /// <param name="repository">规则主档仓储。</param>
    /// <param name="logger">日志对象。</param>
    public RuleHeaderService(IRuleHeaderRepository repository, ILogger<RuleHeaderService> logger)
    {
        _repository = repository;
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
    /// 创建规则主档。
    /// </summary>
    /// <param name="request">规则主档新增请求。</param>
    /// <returns>新增规则主键。</returns>
    /// <exception cref="InvalidOperationException">规则编码已存在时抛出。</exception>
    public async Task<long> CreateAsync(RuleHeaderCreateRequest request)
    {
        // ========== 第一阶段：校验稳定业务编码 ==========
        // RuleCode 是规则在配置、审计和外部沟通中的稳定标识，重复会导致发布历史和追踪记录难以解释。
        if (await _repository.ExistsAsync(request.RuleCode))
        {
            throw new InvalidOperationException($"规则编码已存在: {request.RuleCode}");
        }

        // ========== 第二阶段：创建草稿主档 ==========
        // 主档创建后还没有版本、条件和动作，因此 CurrentVersion 为 0，状态保持 DRAFT。
        var now = DateTime.Now;
        var entity = new RuleHeader
        {
            RuleCode = request.RuleCode,
            RuleName = request.RuleName,
            RuleCategory = request.RuleCategory,
            RuleScope = request.RuleScope,
            ItemCode = request.ItemCode,
            ItemName = request.ItemName,
            GroupCode = request.GroupCode,
            Priority = request.Priority,
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
        // 版本草稿由 RuleVersionService 单独创建，保持“主档”和“版本内容”的职责边界清晰。
        var id = await _repository.InsertAsync(entity);
        _logger.LogInformation("新增规则 {RuleCode}, ID={RuleId}", request.RuleCode, id);
        return id;
    }

    /// <summary>
    /// 更新规则主档基础信息。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">主档更新请求；不包含当前版本字段，避免绕过发布状态机。</param>
    /// <exception cref="KeyNotFoundException">规则不存在时抛出。</exception>
    public async Task UpdateAsync(long ruleId, RuleHeaderUpdateRequest request)
    {
        // ========== 第一阶段：读取现有主档 ==========
        // 不存在时直接返回业务错误，避免后续 Update 影响 0 行造成调用方误以为成功。
        var entity = await _repository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");

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
        entity.Remark = request.Remark;
        entity.UpdatedBy = request.UpdatedBy;
        entity.UpdatedAt = DateTime.Now;

        await _repository.UpdateAsync(entity);
    }

    /// <summary>
    /// 将规则主档实体映射为接口响应。
    /// </summary>
    /// <param name="entity">规则主档实体。</param>
    /// <returns>规则主档响应 DTO。</returns>
    private static RuleHeaderResponse MapToResponse(RuleHeader entity)
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
            Remark = entity.Remark,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
