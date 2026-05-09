using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/formulas")]
public sealed class FormulaDefController : ControllerBase
{
    private readonly FormulaDefService _service;

    public FormulaDefController(FormulaDefService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<FormulaDefResponse>>> GetAllAsync()
    {
        var items = await _service.GetAllAsync();
        return ApiResponse<IReadOnlyList<FormulaDefResponse>>.Ok(items);
    }

    [HttpGet("{formulaId:long}")]
    public async Task<ApiResponse<FormulaDefResponse>> GetByIdAsync(long formulaId)
    {
        var item = await _service.GetByIdAsync(formulaId)
            ?? throw new KeyNotFoundException($"公式定义不存在: {formulaId}");
        return ApiResponse<FormulaDefResponse>.Ok(item);
    }

    [HttpPost]
    public async Task<ApiResponse<long>> CreateAsync(
        [FromBody] FormulaDefCreateRequest request)
    {
        var id = await _service.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    [HttpPut("{formulaId:long}")]
    public async Task<ApiResponse> UpdateAsync(
        long formulaId, [FromBody] FormulaDefUpdateRequest request)
    {
        await _service.UpdateAsync(formulaId, request);
        return ApiResponse.Ok();
    }

    [HttpPatch("{formulaId:long}/toggle")]
    public async Task<ApiResponse> ToggleAsync(long formulaId)
    {
        await _service.ToggleAsync(formulaId);
        return ApiResponse.Ok();
    }
}
