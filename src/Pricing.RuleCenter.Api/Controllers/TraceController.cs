using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/trace")]
public sealed class TraceController : ControllerBase
{
    private readonly TraceQueryService _service;

    public TraceController(TraceQueryService service)
    {
        _service = service;
    }

    [HttpPost("query")]
    public async Task<ApiResponse<PagedResponse<TraceQueryResponse>>> QueryAsync(
        [FromBody] TraceQueryRequest request)
    {
        var result = await _service.QueryAsync(request);
        return ApiResponse<PagedResponse<TraceQueryResponse>>.Ok(result);
    }
}
