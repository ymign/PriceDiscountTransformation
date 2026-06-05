using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Application.Catalog;

/// <summary>
/// 字典应用服务，负责维护规则中心内部使用的枚举型基础数据。
/// </summary>
/// <remarks>
/// <para>
/// 职责边界：该服务只处理规则中心自有字典（计价类型、动作类型、条件类型等），
/// 不直接读取 HIS 项目主数据，避免把业务主数据同步逻辑混入配置维护入口。
/// </para>
/// <para>
/// 使用场景：字典数据被前端配置页用于下拉框、状态名称和分类名称展示。
/// 规则配置中引用的字典编码（如 ConditionType、ActionType）都通过本服务维护可选项。
/// </para>
/// <para>
/// 生命周期：字典项采用软删除（IsEnabled=EnableFlag.No），不物理删除，以保留历史规则配置中
/// 已保存编码的可解释性。前端下拉框只显示 IsEnabled=EnableFlag.Yes 的项。
/// </para>
/// <para>
/// 缓存策略：字典查询使用 IMemoryCache 缓存，TTL 30 分钟。
/// 字典增删改时主动清除缓存，确保前端下拉框读到最新数据。
/// </para>
/// </remarks>
public sealed class DictAppService
{
    /// <summary>
    /// 字典仓储，负责 PR_DICT 表的查询、唯一性判断、插入和软停用。
    /// 仓储层隔离了 SqlSugar 的具体 API，服务层只通过接口调用。
    /// </summary>
    private readonly IDictRepository _repository;

    /// <summary>
    /// 变更日志仓储，用于在字典项创建、更新和停用时写入审计记录。
    /// 字典编码被规则配置引用，任何字典变更都可能影响规则匹配和前端可选项，
    /// 因此需要通过变更日志保留完整的操作审计链。
    /// </summary>
    private readonly IRuleChangeLogRepository _changeLogRepository;

    /// <summary>
    /// 内存缓存，用于缓存字典查询结果，减少数据库访问频率。
    /// 字典数据变化频率极低（仅配置维护时变更），适合较长 TTL 的应用层缓存。
    /// </summary>
    private readonly IMemoryCache _cache;
    /// <summary>
    /// 规则运行期缓存失效器。部分字典（如动作执行顺序）会影响计价引擎跨请求缓存。
    /// </summary>
    private readonly IRuleRuntimeCacheInvalidator _runtimeCacheInvalidator;
    /// <summary>
    /// 跨实例缓存版本同步器，用于在字典变更后递增共享缓存版本。
    /// </summary>
    private readonly ICacheVersionSynchronizer _cacheVersionSynchronizer;

    /// <summary>
    /// 服务日志，用于记录新增、更新和停用等会影响配置展示的操作。
    /// 字典变更会直接影响前端可选项，保留操作线索便于定位"页面选项为什么变化"。
    /// </summary>
    private readonly ILogger<DictAppService> _logger;

    /// <summary>
    /// 字典缓存 key 前缀。
    /// </summary>
    private const string DictCachePrefix = "dicts:";

    /// <summary>
    /// 字典缓存过期时间（30 分钟）。
    /// 字典数据变化频率极低，较长 TTL 可以显著减少数据库访问。
    /// </summary>
    private static readonly TimeSpan DictCacheDuration = TimeSpan.FromMinutes(30);

    /// <summary>
    /// 初始化字典服务。
    /// </summary>
    /// <param name="repository">字典仓储，用于隔离 PR 字典表的持久化实现。</param>
    /// <param name="changeLogRepository">变更日志仓储，用于写入字典增删改的审计记录。</param>
    /// <param name="cache">内存缓存，用于缓存字典查询结果。</param>
    /// <param name="cacheVersionSynchronizer">跨实例缓存版本同步器，用于传播字典相关缓存失效。</param>
    /// <param name="runtimeCacheInvalidator">规则运行期缓存失效器，用于清理动作顺序等引擎缓存。</param>
    /// <param name="logger">日志对象，用于输出配置变更审计线索。</param>
    public DictAppService(
        IDictRepository repository,
        IRuleChangeLogRepository changeLogRepository,
        IMemoryCache cache,
        ICacheVersionSynchronizer cacheVersionSynchronizer,
        IRuleRuntimeCacheInvalidator runtimeCacheInvalidator,
        ILogger<DictAppService> logger)
    {
        _repository = repository;
        _changeLogRepository = changeLogRepository;
        _cache = cache;
        _cacheVersionSynchronizer = cacheVersionSynchronizer;
        _runtimeCacheInvalidator = runtimeCacheInvalidator;
        _logger = logger;
    }

    /// <summary>
    /// 按字典类型读取启用中的字典项。
    /// </summary>
    /// <remarks>
    /// 缓存策略：使用 IMemoryCache 缓存，key 为 <c>dicts:{dictType}</c>，TTL 30 分钟。
    /// 字典增删改时通过 <see cref="ClearDictCache"/> 主动清除。
    /// </remarks>
    /// <param name="dictType">字典类型编码，例如规则分类、动作类型或条件类型。</param>
    /// <returns>指定类型下的字典项集合，通常按仓储层排序规则返回。</returns>
    public async Task<IReadOnlyList<DictResponse>> GetByTypeAsync(string dictType)
    {
        var cacheKey = $"{DictCachePrefix}{dictType}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<DictResponse>? cached) && cached is not null)
        {
            return cached;
        }

        var items = await _repository.GetByTypeAsync(dictType);
        var result = items.Select(MapToResponse).ToList();

        _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = DictCacheDuration
        });

        return result;
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
    /// <remarks>
    /// 缓存策略：使用 IMemoryCache 缓存，key 为 <c>dicts:__all_types__</c>，TTL 30 分钟。
    /// 字典增删改时通过 <see cref="ClearDictCache"/> 主动清除。
    /// </remarks>
    /// <returns>去重后的字典类型列表，用于配置页快速定位已有分类。</returns>
    public async Task<IReadOnlyList<string>> GetAllTypesAsync()
    {
        var cacheKey = $"{DictCachePrefix}__all_types__";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<string>? cached) && cached is not null)
        {
            return cached;
        }

        var result = await _repository.GetAllTypesAsync();

        _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = DictCacheDuration
        });

        return result;
    }

    /// <summary>
    /// 新增一个字典项。
    /// </summary>
    /// <param name="request">新增字典项请求，包含类型、编码、名称、排序和备注。</param>
    /// <returns>新增字典项的数据库主键。</returns>
    /// <exception cref="BizException">字典项重复或字典项不存在时抛出结构化业务错误。</exception>
    public async Task<long> CreateAsync(DictCreateRequest request)
    {
        // ========== 第一阶段：做业务唯一性校验 ==========
        // 字典项的唯一键由"类型 + 编码"组成。先在服务层给出清晰业务错误，
        // 而不是依赖数据库唯一索引异常向外泄漏存储细节。
        if (await _repository.ExistsAsync(request.DictType, request.DictCode))
        {
            throw new BizException(
                BizErrorCode.ResourceAlreadyExists,
                409,
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
            IsEnabled = EnableFlag.Yes,
            Remark = request.Remark
        };

        // ========== 第三阶段：写库、清缓存并记录操作线索 ==========
        // 字典变更会影响前端配置可选项，写库后必须清除缓存，否则前端下拉框读到旧数据。
        var id = await _repository.InsertAsync(entity);
        ClearDictCache(request.DictType);
        _logger.LogInformation("新增字典项 {DictType}/{DictCode}, ID={DictId}",
            request.DictType, request.DictCode, id);

        // ========== 第四阶段：写入变更日志 ==========
        // 字典编码被规则条件和动作引用，新增字典项会扩大配置页面的可选范围，
        // 必须记录审计日志便于追溯"某个选项是什么时候加入的"。
        await TryWriteChangeLogAsync("CREATE_DICT",
            $"新增字典项：类型={request.DictType}，编码={request.DictCode}，名称={request.DictName}");

        return id;
    }

    /// <summary>
    /// 更新字典项的展示信息。
    /// </summary>
    /// <param name="dictId">要更新的字典项主键。</param>
    /// <param name="request">更新请求；不包含字典类型和字典编码，避免外部修改稳定业务键。</param>
    /// <exception cref="BizException">字典项不存在时抛出结构化业务错误。</exception>
    public async Task UpdateAsync(long dictId, DictUpdateRequest request)
    {
        // 字典编码是规则配置引用的稳定键，这里只允许改名称、父级、排序和备注。
        var entity = await _repository.GetByIdAsync(dictId)
            ?? throw new BizException(
                BizErrorCode.DictNotFound,
                404,
                $"字典项不存在: {dictId}");

        entity.DictName = request.DictName;
        entity.ParentCode = request.ParentCode;
        entity.SortNo = request.SortNo;
        entity.Remark = request.Remark;

        await _repository.UpdateAsync(entity);
        ClearDictCache(entity.DictType);
        _logger.LogInformation("更新字典项 {DictId}, 类型={DictType}, 编码={DictCode}",
            dictId, entity.DictType, entity.DictCode);

        // ========== 写入变更日志 ==========
        // 字典名称变更会影响前端显示，排序变更会影响下拉框顺序，
        // 必须记录审计日志便于追溯配置变更历史。
        await TryWriteChangeLogAsync("UPDATE_DICT",
            $"更新字典项：类型={entity.DictType}，编码={entity.DictCode}，新名称={request.DictName}");
    }

    /// <summary>
    /// 停用字典项。
    /// </summary>
    /// <param name="dictId">要停用的字典项主键。</param>
    /// <exception cref="BizException">字典项不存在时抛出结构化业务错误。</exception>
    public async Task DeleteAsync(long dictId)
    {
        // ========== 第一阶段：确认目标存在并记录类型 ==========
        // 即使停用动作最终只是写 IsEnabled，也要先区分"不存在"和"已存在但停用"，便于接口返回明确错误。
        // 同时记录 DictType 用于后续清除缓存。
        var entity = await _repository.GetByIdAsync(dictId)
            ?? throw new BizException(
                BizErrorCode.DictNotFound,
                404,
                $"字典项不存在: {dictId}");

        // ========== 第二阶段：软停用并清缓存 ==========
        // 不物理删除字典项，是为了保留历史规则配置中已保存编码的可解释性。
        await _repository.SetEnabledAsync(dictId, EnableFlag.No);
        ClearDictCache(entity.DictType);
        _logger.LogInformation("停用字典项 {DictId}", dictId);

        // ========== 第三阶段：写入变更日志 ==========
        // 停用字典项会使前端下拉框不再显示该选项，可能影响规则配置人员的操作，
        // 必须记录审计日志便于追溯"某个选项是什么时候被移除的"。
        await TryWriteChangeLogAsync("DELETE_DICT",
            $"停用字典项：类型={entity.DictType}，编码={entity.DictCode}，名称={entity.DictName}");
    }

    /// <summary>
    /// 安全写入变更日志，失败时仅记录警告日志，不阻断主业务流程。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 字典变更是低频配置操作，审计日志写入失败不应导致字典保存或停用回滚。
    /// try-catch 确保即使 PR_RULE_CHANGE_LOG 表不可用，字典维护仍可正常进行。
    /// </para>
    /// <para>
    /// 字典操作不关联特定规则，因此 RuleId 使用 0、VersionNo 使用 null。
    /// </para>
    /// </remarks>
    /// <param name="changeType">变更类型编码，如 CREATE_DICT、UPDATE_DICT、DELETE_DICT。</param>
    /// <param name="changeSummary">人可读的变更摘要，含字典类型和编码等关键参数。</param>
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
    /// 清除指定字典类型及其全类型列表的缓存。
    /// </summary>
    /// <remarks>
    /// 字典增删改时调用，确保前端下拉框读到最新数据。
    /// 同时清除全类型列表缓存，因为新增/停用字典项可能改变类型列表。
    /// </remarks>
    /// <param name="dictType">需要清除缓存的字典类型编码。</param>
    private void ClearDictCache(string dictType)
    {
        _cache.Remove($"{DictCachePrefix}{dictType}");
        _cache.Remove($"{DictCachePrefix}__all_types__");
        if (string.Equals(dictType, "ACTION_TYPE_ORDER", StringComparison.OrdinalIgnoreCase))
        {
            _runtimeCacheInvalidator.ClearRuntimeCache();
            _cacheVersionSynchronizer.IncreaseVersionAsync(CacheVersionSynchronizer.ActionTypeOrderScope).GetAwaiter().GetResult();
        }

        _logger.LogDebug("已清除字典缓存: {DictType}", dictType);
    }

    /// <summary>
    /// 将字典实体映射为接口返回对象。
    /// </summary>
    /// <remarks>
    /// 映射逻辑采用逐字段赋值而非 AutoMapper，原因：字典字段数量少且稳定，
    /// 逐字段赋值可以在编译期发现字段遗漏，避免运行时因映射缺失产生空值。
    /// </remarks>
    /// <param name="entity">数据库中的字典实体，来自 PR_DICT 表。</param>
    /// <returns>面向接口层的字典响应对象，IsEnabled 字段保留原始 EnableFlag.Yes/EnableFlag.No 值供前端判断显示样式。</returns>
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
