using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions/{versionNo:int}/conditions")]
/// <summary>
/// 规则条件控制器，暴露指定规则版本下条件集合的查询和保存接口。
/// </summary>
public sealed class RuleConditionController : ControllerBase
{
    /// <summary>
    /// _service 服务依赖，用于复用已经封装好的业务编排或领域处理能力。
    /// </summary>
    private readonly RuleConditionService _service;

    /// <summary>
    /// 初始化规则条件控制器。
    /// </summary>
    /// <param name="service">规则条件应用服务。</param>
    public RuleConditionController(RuleConditionService service)
    {
        _service = service;
    }

    [HttpGet]
    /// <summary>
    /// 查询指定规则版本下的条件集合。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <returns>规则条件列表。</returns>
    public async Task<ApiResponse<IReadOnlyList<RuleConditionResponse>>> GetAsync(
        long ruleId, int versionNo)
    {
        var items = await _service.GetAsync(ruleId, versionNo);
        return ApiResponse<IReadOnlyList<RuleConditionResponse>>.Ok(items);
    }

    [HttpPut]
    /// <summary>
    /// 整体保存指定草稿版本的条件集合。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">条件保存请求。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> SaveAsync(
        long ruleId, int versionNo, [FromBody] RuleConditionSaveRequest request)
    {
        await _service.SaveAsync(ruleId, versionNo, request);
        return ApiResponse.Ok();
    }
}
