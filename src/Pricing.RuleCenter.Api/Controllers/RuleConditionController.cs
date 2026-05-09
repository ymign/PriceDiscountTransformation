using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions/{versionNo:int}/conditions")]
public sealed class RuleConditionController : ControllerBase
{
    private readonly RuleConditionService _service;

    public RuleConditionController(RuleConditionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<RuleConditionResponse>>> GetAsync(
        long ruleId, int versionNo)
    {
        var items = await _service.GetAsync(ruleId, versionNo);
        return ApiResponse<IReadOnlyList<RuleConditionResponse>>.Ok(items);
    }

    [HttpPut]
    public async Task<ApiResponse> SaveAsync(
        long ruleId, int versionNo, [FromBody] RuleConditionSaveRequest request)
    {
        await _service.SaveAsync(ruleId, versionNo, request);
        return ApiResponse.Ok();
    }
}
