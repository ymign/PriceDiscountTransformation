using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/dicts")]
public sealed class DictController : ControllerBase
{
    private readonly DictService _service;

    public DictController(DictService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<DictResponse>>> GetByTypeAsync(
        [FromQuery] string dictType)
    {
        var items = await _service.GetByTypeAsync(dictType);
        return ApiResponse<IReadOnlyList<DictResponse>>.Ok(items);
    }

    [HttpGet("{dictId:long}")]
    public async Task<ApiResponse<DictResponse>> GetByIdAsync(long dictId)
    {
        var item = await _service.GetByIdAsync(dictId)
            ?? throw new KeyNotFoundException($"字典项不存在: {dictId}");
        return ApiResponse<DictResponse>.Ok(item);
    }

    [HttpGet("types")]
    public async Task<ApiResponse<IReadOnlyList<string>>> GetAllTypesAsync()
    {
        var types = await _service.GetAllTypesAsync();
        return ApiResponse<IReadOnlyList<string>>.Ok(types);
    }

    [HttpPost]
    public async Task<ApiResponse<long>> CreateAsync([FromBody] DictCreateRequest request)
    {
        var id = await _service.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    [HttpPut("{dictId:long}")]
    public async Task<ApiResponse> UpdateAsync(
        long dictId, [FromBody] DictUpdateRequest request)
    {
        await _service.UpdateAsync(dictId, request);
        return ApiResponse.Ok();
    }

    [HttpDelete("{dictId:long}")]
    public async Task<ApiResponse> DeleteAsync(long dictId)
    {
        await _service.DeleteAsync(dictId);
        return ApiResponse.Ok();
    }
}
