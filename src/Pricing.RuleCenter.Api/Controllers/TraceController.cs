using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/trace")]
/// <summary>
/// 计价追踪控制器，暴露请求日志、执行步骤和折价明细的查询接口。
/// </summary>
public sealed class TraceController : ControllerBase
{
    /// <summary>
    /// _service 服务依赖，用于复用已经封装好的业务编排或领域处理能力。
    /// </summary>
    private readonly TraceQueryService _service;

    /// <summary>
    /// 初始化计价追踪控制器。
    /// </summary>
    /// <param name="service">计价追踪查询服务。</param>
    public TraceController(TraceQueryService service)
    {
        _service = service;
    }

    [HttpPost("query")]
    /// <summary>
    /// 查询计价追踪记录。
    /// </summary>
    /// <param name="request">追踪查询条件。</param>
    /// <returns>追踪查询分页结果。</returns>
    public async Task<ApiResponse<PagedResponse<TraceQueryResponse>>> QueryAsync(
        [FromBody] TraceQueryRequest request)
    {
        var result = await _service.QueryAsync(request);
        return ApiResponse<PagedResponse<TraceQueryResponse>>.Ok(result);
    }
}
