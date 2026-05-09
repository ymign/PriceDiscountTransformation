using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules/{ruleId:long}")]
public sealed class RulePublishController : ControllerBase
{
    private readonly RulePublishService _service;

    public RulePublishController(RulePublishService service)
    {
        _service = service;
    }

    [HttpGet("publish-history")]
    public async Task<ApiResponse<IReadOnlyList<RulePublishResponse>>> GetPublishHistoryAsync(
        long ruleId)
    {
        var items = await _service.GetPublishHistoryAsync(ruleId);
        return ApiResponse<IReadOnlyList<RulePublishResponse>>.Ok(items);
    }

    [HttpGet("change-logs")]
    public async Task<ApiResponse<IReadOnlyList<RuleChangeLogResponse>>> GetChangeLogsAsync(
        long ruleId)
    {
        var items = await _service.GetChangeLogsAsync(ruleId);
        return ApiResponse<IReadOnlyList<RuleChangeLogResponse>>.Ok(items);
    }

    [HttpPost("publish")]
    public async Task<ApiResponse> PublishAsync(
        long ruleId, [FromBody] RulePublishRequest request)
    {
        await _service.PublishAsync(ruleId, request);
        return ApiResponse.Ok();
    }

    [HttpPost("disable")]
    public async Task<ApiResponse> DisableAsync(
        long ruleId, [FromBody] RuleDisableRequest request)
    {
        await _service.DisableAsync(ruleId, request);
        return ApiResponse.Ok();
    }

    [HttpPost("rollback")]
    public async Task<ApiResponse> RollbackAsync(
        long ruleId, [FromBody] RuleRollbackRequest request)
    {
        await _service.RollbackAsync(ruleId, request);
        return ApiResponse.Ok();
    }
}
