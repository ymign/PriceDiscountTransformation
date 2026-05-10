using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【字典配置控制器】
/// <para>
/// 职责范围：规则中心基础字典的增删改查，字典数据用于规则维护工作台的下拉选项、
/// 计价类型的枚举值、单位映射等场景。
/// </para>
/// <para>
/// 路由前缀：<c>api/pricing/dicts</c>
/// </para>
/// <para>
/// 字典类型包括但不限于：计价类型、收费单位、公式类型、规则场景、部位编码等。
/// 所有字典数据存储在 PR_DICT 表中，通过 DICT_TYPE 字段区分不同类型。
/// </para>
/// <para>
/// 设计原则：
/// <list type="bullet">
///   <item>字典项采用软删除（启用/停用状态），不物理删除。</item>
///   <item>字典类型编码全局唯一，字典项编码在同类型内唯一。</item>
///   <item>前端空值必须存为 NULL（NULL 表示"不校验"，0 表示"限制为零"）。</item>
/// </list>
/// </para>
/// </summary>
[ApiController]
[Route("api/pricing/dicts")]
public sealed class DictController : ControllerBase
{
    /// <summary>
    /// 字典应用服务实例，封装字典数据的 CRUD 业务逻辑。
    /// </summary>
    private readonly DictService _service;

    /// <summary>
    /// 构造函数，通过依赖注入获取字典应用服务。
    /// </summary>
    /// <param name="service">字典应用服务（<see cref="DictService"/>）。</param>
    public DictController(DictService service)
    {
        _service = service;
    }

    /// <summary>
    /// 【按类型查询字典项】— 查询指定字典类型下所有启用的字典项。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/dicts?dictType={dictType}</c>
    /// </para>
    /// <para>
    /// 用途：前端维护工作台加载下拉选项时调用，例如查询"计价类型"、"收费单位"等字典。
    /// 仅返回启用状态的字典项，已停用的不返回。
    /// </para>
    /// </summary>
    /// <param name="dictType">
    /// 字典类型编码（查询参数），例如 <c>"PRICING_TYPE"</c>、<c>"UNIT"</c>、<c>"FORMULA_TYPE"</c> 等。
    /// </param>
    /// <returns>
    /// 字典项列表（<see cref="DictResponse"/>），每个字典项包含编码、名称、排序号、启用状态等信息。
    /// </returns>
    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<DictResponse>>> GetByTypeAsync(
        [FromQuery] string dictType)
    {
        var items = await _service.GetByTypeAsync(dictType);
        return ApiResponse<IReadOnlyList<DictResponse>>.Ok(items);
    }

    /// <summary>
    /// 【按主键查询字典项】— 查询单个字典项的详细信息。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/dicts/{dictId}</c>
    /// </para>
    /// <para>
    /// 用途：字典项编辑页面加载详情时调用。
    /// </para>
    /// </summary>
    /// <param name="dictId">字典项主键（路径参数，数据库自增序列生成）。</param>
    /// <returns>字典项详情（<see cref="DictResponse"/>）。</returns>
    /// <exception cref="KeyNotFoundException">当指定 dictId 的字典项不存在时抛出。</exception>
    [HttpGet("{dictId:long}")]
    public async Task<ApiResponse<DictResponse>> GetByIdAsync(long dictId)
    {
        var item = await _service.GetByIdAsync(dictId)
            ?? throw new KeyNotFoundException($"字典项不存在: {dictId}");
        return ApiResponse<DictResponse>.Ok(item);
    }

    /// <summary>
    /// 【查询所有字典类型】— 获取系统中所有已使用的字典类型编码列表。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/dicts/types</c>
    /// </para>
    /// <para>
    /// 用途：字典管理页面展示所有字典类型分类时调用，便于按类型筛选字典项。
    /// </para>
    /// </summary>
    /// <returns>字典类型编码字符串列表，去重后的唯一类型集合。</returns>
    [HttpGet("types")]
    public async Task<ApiResponse<IReadOnlyList<string>>> GetAllTypesAsync()
    {
        var types = await _service.GetAllTypesAsync();
        return ApiResponse<IReadOnlyList<string>>.Ok(types);
    }

    /// <summary>
    /// 【新增字典项】— 向指定字典类型下新增一个字典项。
    /// <para>
    /// HTTP 方法：POST &nbsp;|&nbsp; 路由：<c>/api/pricing/dicts</c>
    /// </para>
    /// <para>
    /// 用途：字典维护工作台新增字典项时调用。新增后默认为启用状态。
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 字典新增请求（<see cref="DictCreateRequest"/>），包含：
    /// <list type="bullet">
    ///   <item><c>DictType</c> — 字典类型编码（必填）</item>
    ///   <item><c>DictCode</c> — 字典项编码（同类型内唯一，必填）</item>
    ///   <item><c>DictName</c> — 字典项名称（必填）</item>
    ///   <item><c>SortOrder</c> — 排序号（可选）</item>
    /// </list>
    /// </param>
    /// <returns>新增字典项的主键 ID。</returns>
    [HttpPost]
    public async Task<ApiResponse<long>> CreateAsync([FromBody] DictCreateRequest request)
    {
        var id = await _service.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    /// <summary>
    /// 【更新字典项】— 更新指定字典项的展示信息（名称、排序等）。
    /// <para>
    /// HTTP 方法：PUT &nbsp;|&nbsp; 路由：<c>/api/pricing/dicts/{dictId}</c>
    /// </para>
    /// <para>
    /// 用途：字典维护工作台编辑字典项时调用。
    /// 注意：字典类型编码和字典项编码通常不允许修改，仅允许修改名称、排序等展示信息。
    /// </para>
    /// </summary>
    /// <param name="dictId">字典项主键（路径参数）。</param>
    /// <param name="request">字典更新请求（<see cref="DictUpdateRequest"/>）。</param>
    /// <returns>统一成功响应。</returns>
    [HttpPut("{dictId:long}")]
    public async Task<ApiResponse> UpdateAsync(
        long dictId, [FromBody] DictUpdateRequest request)
    {
        await _service.UpdateAsync(dictId, request);
        return ApiResponse.Ok();
    }

    /// <summary>
    /// 【停用字典项】— 软删除字典项，将其状态设为停用。
    /// <para>
    /// HTTP 方法：DELETE &nbsp;|&nbsp; 路由：<c>/api/pricing/dicts/{dictId}</c>
    /// </para>
    /// <para>
    /// 用途：字典维护工作台停用字典项时调用。停用后该字典项不再出现在下拉选项中，
    /// 但不会影响已引用该字典项的历史规则数据。
    /// </para>
    /// <para>
    /// 注意：采用软删除策略，物理记录保留用于审计追溯，仅更新启用状态标志。
    /// </para>
    /// </summary>
    /// <param name="dictId">字典项主键（路径参数）。</param>
    /// <returns>统一成功响应。</returns>
    [HttpDelete("{dictId:long}")]
    public async Task<ApiResponse> DeleteAsync(long dictId)
    {
        await _service.DeleteAsync(dictId);
        return ApiResponse.Ok();
    }
}
