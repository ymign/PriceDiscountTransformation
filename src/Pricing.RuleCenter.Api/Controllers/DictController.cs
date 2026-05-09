using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/dicts")]
/// <summary>
/// 字典配置控制器，暴露规则中心基础字典的查询和维护接口。
/// </summary>
public sealed class DictController : ControllerBase
{
    /// <summary>
    /// _service 服务依赖，用于复用已经封装好的业务编排或领域处理能力。
    /// </summary>
    private readonly DictService _service;

    /// <summary>
    /// 初始化字典配置控制器。
    /// </summary>
    /// <param name="service">字典应用服务。</param>
    public DictController(DictService service)
    {
        _service = service;
    }

    [HttpGet]
    /// <summary>
    /// 按字典类型查询启用字典项。
    /// </summary>
    /// <param name="dictType">字典类型编码。</param>
    /// <returns>字典项列表。</returns>
    public async Task<ApiResponse<IReadOnlyList<DictResponse>>> GetByTypeAsync(
        [FromQuery] string dictType)
    {
        var items = await _service.GetByTypeAsync(dictType);
        return ApiResponse<IReadOnlyList<DictResponse>>.Ok(items);
    }

    [HttpGet("{dictId:long}")]
    /// <summary>
    /// 按主键查询字典项。
    /// </summary>
    /// <param name="dictId">字典项主键。</param>
    /// <returns>字典项详情。</returns>
    public async Task<ApiResponse<DictResponse>> GetByIdAsync(long dictId)
    {
        var item = await _service.GetByIdAsync(dictId)
            ?? throw new KeyNotFoundException($"字典项不存在: {dictId}");
        return ApiResponse<DictResponse>.Ok(item);
    }

    [HttpGet("types")]
    /// <summary>
    /// 查询所有启用字典类型。
    /// </summary>
    /// <returns>字典类型编码列表。</returns>
    public async Task<ApiResponse<IReadOnlyList<string>>> GetAllTypesAsync()
    {
        var types = await _service.GetAllTypesAsync();
        return ApiResponse<IReadOnlyList<string>>.Ok(types);
    }

    [HttpPost]
    /// <summary>
    /// 新增字典项。
    /// </summary>
    /// <param name="request">字典新增请求。</param>
    /// <returns>新增字典项主键。</returns>
    public async Task<ApiResponse<long>> CreateAsync([FromBody] DictCreateRequest request)
    {
        var id = await _service.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    [HttpPut("{dictId:long}")]
    /// <summary>
    /// 更新字典项展示信息。
    /// </summary>
    /// <param name="dictId">字典项主键。</param>
    /// <param name="request">字典更新请求。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> UpdateAsync(
        long dictId, [FromBody] DictUpdateRequest request)
    {
        await _service.UpdateAsync(dictId, request);
        return ApiResponse.Ok();
    }

    [HttpDelete("{dictId:long}")]
    /// <summary>
    /// 停用字典项。
    /// </summary>
    /// <param name="dictId">字典项主键。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> DeleteAsync(long dictId)
    {
        await _service.DeleteAsync(dictId);
        return ApiResponse.Ok();
    }
}
