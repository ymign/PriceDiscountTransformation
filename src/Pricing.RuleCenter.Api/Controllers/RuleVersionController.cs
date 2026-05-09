using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions")]
public sealed class RuleVersionController : ControllerBase
{
    private readonly RuleVersionService _service;

    public RuleVersionController(RuleVersionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<RuleVersionResponse>>> GetByRuleIdAsync(
        long ruleId)
    {
        var items = await _service.GetByRuleIdAsync(ruleId);
        return ApiResponse<IReadOnlyList<RuleVersionResponse>>.Ok(items);
    }

    [HttpGet("{versionId:long}")]
    public async Task<ApiResponse<RuleVersionResponse>> GetByIdAsync(long versionId)
    {
        var item = await _service.GetByIdAsync(versionId)
            ?? throw new KeyNotFoundException($"规则版本不存在: {versionId}");
        return ApiResponse<RuleVersionResponse>.Ok(item);
    }

    [HttpPost]
    public async Task<ApiResponse<long>> CreateDraftAsync(long ruleId)
    {
        var id = await _service.CreateDraftAsync(ruleId);
        return ApiResponse<long>.Ok(id);
    }
}
