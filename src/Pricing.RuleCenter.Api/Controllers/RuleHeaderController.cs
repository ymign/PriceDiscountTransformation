using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules")]
/// <summary>
/// 规则主档控制器，暴露规则基础信息的查询和维护接口。
/// </summary>
public sealed class RuleHeaderController : ControllerBase
{
    /// <summary>
    /// _service 服务依赖，用于复用已经封装好的业务编排或领域处理能力。
    /// </summary>
    private readonly RuleHeaderService _service;

    /// <summary>
    /// 初始化规则主档控制器。
    /// </summary>
    /// <param name="service">规则主档应用服务。</param>
    public RuleHeaderController(RuleHeaderService service)
    {
        _service = service;
    }

    [HttpGet]
    /// <summary>
    /// 分页查询规则主档。
    /// </summary>
    /// <param name="request">分页和筛选条件。</param>
    /// <returns>规则主档分页结果。</returns>
    public async Task<ApiResponse<PagedResponse<RuleHeaderResponse>>> GetPagedAsync(
        [FromQuery] RuleHeaderPagedRequest request)
    {
        var result = await _service.GetPagedAsync(request);
        return ApiResponse<PagedResponse<RuleHeaderResponse>>.Ok(result);
    }

    [HttpGet("{ruleId:long}")]
    /// <summary>
    /// 按主键查询规则主档。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>规则主档详情。</returns>
    public async Task<ApiResponse<RuleHeaderResponse>> GetByIdAsync(long ruleId)
    {
        var item = await _service.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");
        return ApiResponse<RuleHeaderResponse>.Ok(item);
    }

    [HttpGet("by-item/{itemCode}")]
    /// <summary>
    /// 按项目编码查询关联规则。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>规则主档列表。</returns>
    public async Task<ApiResponse<IReadOnlyList<RuleHeaderResponse>>> GetByItemCodeAsync(
        string itemCode)
    {
        var items = await _service.GetByItemCodeAsync(itemCode);
        return ApiResponse<IReadOnlyList<RuleHeaderResponse>>.Ok(items);
    }

    [HttpPost]
    /// <summary>
    /// 创建规则主档。
    /// </summary>
    /// <param name="request">规则主档新增请求。</param>
    /// <returns>新增规则主键。</returns>
    public async Task<ApiResponse<long>> CreateAsync(
        [FromBody] RuleHeaderCreateRequest request)
    {
        var id = await _service.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    [HttpPut("{ruleId:long}")]
    /// <summary>
    /// 更新规则主档基础信息。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">规则主档更新请求。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> UpdateAsync(
        long ruleId, [FromBody] RuleHeaderUpdateRequest request)
    {
        await _service.UpdateAsync(ruleId, request);
        return ApiResponse.Ok();
    }
}
