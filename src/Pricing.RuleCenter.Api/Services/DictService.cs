using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

/// <summary>
/// 字典应用服务，负责维护规则中心内部使用的枚举型基础数据。
/// </summary>
/// <remarks>
/// 字典数据通常被前端配置页用于下拉框、状态名称和分类名称展示。该服务只处理规则中心自有字典，
/// 不直接读取 HIS 项目主数据，避免把业务主数据同步逻辑混入配置维护入口。
/// </remarks>
public sealed class DictService
{
    /// <summary>
    /// 字典仓储，负责字典项查询、唯一性判断、写入和软停用。
    /// </summary>
    private readonly IDictRepository _repository;
    /// <summary>
    /// 服务日志，用于记录新增和停用等会影响配置展示的操作。
    /// </summary>
    private readonly ILogger<DictService> _logger;

    /// <summary>
    /// 初始化字典服务。
    /// </summary>
    /// <param name="repository">字典仓储，用于隔离 PR 字典表的持久化实现。</param>
    /// <param name="logger">日志对象，用于输出配置变更审计线索。</param>
    public DictService(IDictRepository repository, ILogger<DictService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// 按字典类型读取启用中的字典项。
    /// </summary>
    /// <param name="dictType">字典类型编码，例如规则分类、动作类型或条件类型。</param>
    /// <returns>指定类型下的字典项集合，通常按仓储层排序规则返回。</returns>
    public async Task<IReadOnlyList<DictResponse>> GetByTypeAsync(string dictType)
    {
        var items = await _repository.GetByTypeAsync(dictType);
        return items.Select(MapToResponse).ToList();
    }

    /// <summary>
    /// 按主键读取单个字典项。
    /// </summary>
    /// <param name="dictId">字典项主键。</param>
    /// <returns>找到时返回字典 DTO；不存在时返回 <c>null</c>，由控制器转换为 404。</returns>
    public async Task<DictResponse?> GetByIdAsync(long dictId)
    {
        var entity = await _repository.GetByIdAsync(dictId);
        return entity is null ? null : MapToResponse(entity);
    }

    /// <summary>
    /// 读取当前系统已存在的字典类型编码。
    /// </summary>
    /// <returns>去重后的字典类型列表，用于配置页快速定位已有分类。</returns>
    public async Task<IReadOnlyList<string>> GetAllTypesAsync()
    {
        return await _repository.GetAllTypesAsync();
    }

    /// <summary>
    /// 新增一个字典项。
    /// </summary>
    /// <param name="request">新增字典项请求，包含类型、编码、名称、排序和备注。</param>
    /// <returns>新增字典项的数据库主键。</returns>
    /// <exception cref="InvalidOperationException">同一字典类型下已存在相同字典编码时抛出。</exception>
    public async Task<long> CreateAsync(DictCreateRequest request)
    {
        // ========== 第一阶段：做业务唯一性校验 ==========
        // 字典项的唯一键由“类型 + 编码”组成。先在服务层给出清晰业务错误，
        // 而不是依赖数据库唯一索引异常向外泄漏存储细节。
        if (await _repository.ExistsAsync(request.DictType, request.DictCode))
        {
            throw new InvalidOperationException(
                $"字典项已存在: {request.DictType}/{request.DictCode}");
        }

        // ========== 第二阶段：组装持久化实体 ==========
        // 新增字典默认启用；停用通过 DeleteAsync 的软删除实现，保留历史配置引用的可读性。
        var entity = new Dict
        {
            DictType = request.DictType,
            DictCode = request.DictCode,
            DictName = request.DictName,
            ParentCode = request.ParentCode,
            SortNo = request.SortNo,
            IsEnabled = "Y",
            Remark = request.Remark
        };

        // ========== 第三阶段：写库并记录操作线索 ==========
        // 字典变更会影响前端配置可选项，保留日志便于定位“页面选项为什么变化”。
        var id = await _repository.InsertAsync(entity);
        _logger.LogInformation("新增字典项 {DictType}/{DictCode}, ID={DictId}",
            request.DictType, request.DictCode, id);
        return id;
    }

    /// <summary>
    /// 更新字典项的展示信息。
    /// </summary>
    /// <param name="dictId">要更新的字典项主键。</param>
    /// <param name="request">更新请求；不包含字典类型和字典编码，避免外部修改稳定业务键。</param>
    /// <exception cref="KeyNotFoundException">字典项不存在时抛出。</exception>
    public async Task UpdateAsync(long dictId, DictUpdateRequest request)
    {
        // 字典编码是规则配置引用的稳定键，这里只允许改名称、父级、排序和备注。
        var entity = await _repository.GetByIdAsync(dictId)
            ?? throw new KeyNotFoundException($"字典项不存在: {dictId}");

        entity.DictName = request.DictName;
        entity.ParentCode = request.ParentCode;
        entity.SortNo = request.SortNo;
        entity.Remark = request.Remark;

        await _repository.UpdateAsync(entity);
    }

    /// <summary>
    /// 停用字典项。
    /// </summary>
    /// <param name="dictId">要停用的字典项主键。</param>
    /// <exception cref="KeyNotFoundException">字典项不存在时抛出。</exception>
    public async Task DeleteAsync(long dictId)
    {
        // ========== 第一阶段：确认目标存在 ==========
        // 即使停用动作最终只是写 IsEnabled，也要先区分“不存在”和“已存在但停用”，便于接口返回明确错误。
        _ = await _repository.GetByIdAsync(dictId)
            ?? throw new KeyNotFoundException($"字典项不存在: {dictId}");

        // ========== 第二阶段：软停用 ==========
        // 不物理删除字典项，是为了保留历史规则配置中已保存编码的可解释性。
        await _repository.SetEnabledAsync(dictId, "N");
        _logger.LogInformation("停用字典项 {DictId}", dictId);
    }

    /// <summary>
    /// 将字典实体映射为接口返回对象。
    /// </summary>
    /// <param name="entity">数据库中的字典实体。</param>
    /// <returns>面向接口层的字典响应对象。</returns>
    private static DictResponse MapToResponse(Dict entity)
    {
        return new DictResponse
        {
            DictId = entity.DictId,
            DictType = entity.DictType,
            DictCode = entity.DictCode,
            DictName = entity.DictName,
            ParentCode = entity.ParentCode,
            SortNo = entity.SortNo,
            IsEnabled = entity.IsEnabled,
            Remark = entity.Remark
        };
    }
}
