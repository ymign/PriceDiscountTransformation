using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions/{versionNo:int}/actions")]
public sealed class RuleActionController : ControllerBase
{
    private readonly RuleActionService _service;

    public RuleActionController(RuleActionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<RuleActionResponse>>> GetAsync(
        long ruleId, int versionNo)
    {
        var items = await _service.GetAsync(ruleId, versionNo);
        return ApiResponse<IReadOnlyList<RuleActionResponse>>.Ok(items);
    }

    [HttpPut]
    public async Task<ApiResponse> SaveAsync(
        long ruleId, int versionNo, [FromBody] RuleActionSaveRequest request)
    {
        await _service.SaveAsync(ruleId, versionNo, request);
        return ApiResponse.Ok();
    }
}
