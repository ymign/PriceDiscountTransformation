using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules")]
public sealed class RuleHeaderController : ControllerBase
{
    private readonly RuleHeaderService _service;

    public RuleHeaderController(RuleHeaderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ApiResponse<PagedResponse<RuleHeaderResponse>>> GetPagedAsync(
        [FromQuery] RuleHeaderPagedRequest request)
    {
        var result = await _service.GetPagedAsync(request);
        return ApiResponse<PagedResponse<RuleHeaderResponse>>.Ok(result);
    }

    [HttpGet("{ruleId:long}")]
    public async Task<ApiResponse<RuleHeaderResponse>> GetByIdAsync(long ruleId)
    {
        var item = await _service.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");
        return ApiResponse<RuleHeaderResponse>.Ok(item);
    }

    [HttpGet("by-item/{itemCode}")]
    public async Task<ApiResponse<IReadOnlyList<RuleHeaderResponse>>> GetByItemCodeAsync(
        string itemCode)
    {
        var items = await _service.GetByItemCodeAsync(itemCode);
        return ApiResponse<IReadOnlyList<RuleHeaderResponse>>.Ok(items);
    }

    [HttpPost]
    public async Task<ApiResponse<long>> CreateAsync(
        [FromBody] RuleHeaderCreateRequest request)
    {
        var id = await _service.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    [HttpPut("{ruleId:long}")]
    public async Task<ApiResponse> UpdateAsync(
        long ruleId, [FromBody] RuleHeaderUpdateRequest request)
    {
        await _service.UpdateAsync(ruleId, request);
        return ApiResponse.Ok();
    }
}
