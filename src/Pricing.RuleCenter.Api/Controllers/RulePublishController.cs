using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules/{ruleId:long}")]
/// <summary>
/// 规则发布控制器，暴露发布历史、变更日志、发布、停用和回滚接口。
/// </summary>
/// <remarks>
/// 发布相关接口会改变规则是否参与计价匹配，控制器只做路由转发，状态机约束由
/// <see cref="RulePublishService"/> 统一执行。
/// </remarks>
public sealed class RulePublishController : ControllerBase
{
    /// <summary>
    /// _service 服务依赖，用于复用已经封装好的业务编排或领域处理能力。
    /// </summary>
    private readonly RulePublishService _service;

    /// <summary>
    /// 初始化规则发布控制器。
    /// </summary>
    /// <param name="service">规则发布应用服务。</param>
    public RulePublishController(RulePublishService service)
    {
        _service = service;
    }

    [HttpGet("publish-history")]
    /// <summary>
    /// 查询规则发布历史。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>发布、停用和回滚流水。</returns>
    public async Task<ApiResponse<IReadOnlyList<RulePublishResponse>>> GetPublishHistoryAsync(
        long ruleId)
    {
        var items = await _service.GetPublishHistoryAsync(ruleId);
        return ApiResponse<IReadOnlyList<RulePublishResponse>>.Ok(items);
    }

    [HttpGet("change-logs")]
    /// <summary>
    /// 查询规则变更日志。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>规则变更日志列表。</returns>
    public async Task<ApiResponse<IReadOnlyList<RuleChangeLogResponse>>> GetChangeLogsAsync(
        long ruleId)
    {
        var items = await _service.GetChangeLogsAsync(ruleId);
        return ApiResponse<IReadOnlyList<RuleChangeLogResponse>>.Ok(items);
    }

    [HttpPost("publish")]
    /// <summary>
    /// 发布草稿版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">发布请求。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> PublishAsync(
        long ruleId, [FromBody] RulePublishRequest request)
    {
        await _service.PublishAsync(ruleId, request);
        return ApiResponse.Ok();
    }

    [HttpPost("disable")]
    /// <summary>
    /// 停用已发布规则。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">停用请求。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> DisableAsync(
        long ruleId, [FromBody] RuleDisableRequest request)
    {
        await _service.DisableAsync(ruleId, request);
        return ApiResponse.Ok();
    }

    [HttpPost("rollback")]
    /// <summary>
    /// 回滚到最近一个历史发布版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">回滚请求。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> RollbackAsync(
        long ruleId, [FromBody] RuleRollbackRequest request)
    {
        await _service.RollbackAsync(ruleId, request);
        return ApiResponse.Ok();
    }
}
