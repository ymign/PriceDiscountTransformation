using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions/{versionNo:int}/actions")]
/// <summary>
/// 规则动作控制器，暴露指定规则版本下动作链的查询和保存接口。
/// </summary>
public sealed class RuleActionController : ControllerBase
{
    /// <summary>
    /// _service 服务依赖，用于复用已经封装好的业务编排或领域处理能力。
    /// </summary>
    private readonly RuleActionService _service;

    /// <summary>
    /// 初始化规则动作控制器。
    /// </summary>
    /// <param name="service">规则动作应用服务。</param>
    public RuleActionController(RuleActionService service)
    {
        _service = service;
    }

    [HttpGet]
    /// <summary>
    /// 查询指定规则版本下的动作链。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <returns>规则动作列表。</returns>
    public async Task<ApiResponse<IReadOnlyList<RuleActionResponse>>> GetAsync(
        long ruleId, int versionNo)
    {
        var items = await _service.GetAsync(ruleId, versionNo);
        return ApiResponse<IReadOnlyList<RuleActionResponse>>.Ok(items);
    }

    [HttpPut]
    /// <summary>
    /// 整体保存指定草稿版本的动作链。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">动作保存请求。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> SaveAsync(
        long ruleId, int versionNo, [FromBody] RuleActionSaveRequest request)
    {
        await _service.SaveAsync(ruleId, versionNo, request);
        return ApiResponse.Ok();
    }
}
