using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing")]
public sealed class PricingController : ControllerBase
{
    private readonly PricingApiService _service;

    public PricingController(PricingApiService service)
    {
        _service = service;
    }

    [HttpPost("calculate/simulate")]
    public async Task<ApiResponse<PricingCalculateResponse>> SimulateAsync(
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _service.SimulateAsync(request);
        return ApiResponse<PricingCalculateResponse>.Ok(result);
    }

    [HttpPost("calculate/confirm")]
    public async Task<ApiResponse<PricingCalculateResponse>> ConfirmAsync(
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _service.ConfirmAsync(request);
        return ApiResponse<PricingCalculateResponse>.Ok(result);
    }

    [HttpPost("calculate/commit")]
    public async Task<ApiResponse> CommitAsync([FromBody] PricingCommitRequest request)
    {
        await _service.CommitAsync(request);
        return ApiResponse.Ok();
    }

    [HttpPost("calculate/cancel")]
    public async Task<ApiResponse> CancelAsync([FromBody] PricingCancelRequest request)
    {
        await _service.CancelAsync(request);
        return ApiResponse.Ok();
    }

    [HttpPost("calculate/reverse")]
    public async Task<ApiResponse> ReverseAsync([FromBody] PricingReverseRequest request)
    {
        await _service.ReverseAsync(request);
        return ApiResponse.Ok();
    }

    [HttpGet("items/{itemCode}/special-flag")]
    public async Task<ApiResponse<SpecialFlagResponse>> GetSpecialFlagAsync(string itemCode)
    {
        var result = await _service.GetSpecialFlagAsync(itemCode);
        return ApiResponse<SpecialFlagResponse>.Ok(result);
    }
}
