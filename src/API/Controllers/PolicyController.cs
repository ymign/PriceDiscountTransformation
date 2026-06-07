using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Authorize(Policy = "RuleAdmin")]
[Route("api/pricing/policies")]
public sealed class PolicyController : ControllerBase
{
    private readonly PolicyAppService _policyAppService;
    private readonly PolicyVersionAppService _policyVersionAppService;
    private readonly PolicyPreviewAppService _policyPreviewAppService;
    private readonly PolicyReviewAppService _policyReviewAppService;

    public PolicyController(
        PolicyAppService policyAppService,
        PolicyVersionAppService policyVersionAppService,
        PolicyPreviewAppService policyPreviewAppService,
        PolicyReviewAppService policyReviewAppService)
    {
        _policyAppService = policyAppService;
        _policyVersionAppService = policyVersionAppService;
        _policyPreviewAppService = policyPreviewAppService;
        _policyReviewAppService = policyReviewAppService;
    }

    [HttpGet]
    public async Task<ApiResult<IReadOnlyList<PolicyResponse>>> GetAllAsync()
    {
        return ApiResult<IReadOnlyList<PolicyResponse>>.Ok(await _policyAppService.GetAllAsync());
    }

    [HttpGet("{policyId:long}")]
    public async Task<ActionResult<ApiResult<PolicyDetailResponse>>> GetByIdAsync(long policyId)
    {
        var item = await _policyAppService.GetByIdAsync(policyId);
        if (item is null)
        {
            return NotFound(ApiResult.Fail(404, $"策略不存在: {policyId}"));
        }

        return ApiResult<PolicyDetailResponse>.Ok(item);
    }

    [HttpPost]
    public async Task<ApiResult<long>> CreateAsync([FromBody] PolicyCreateRequest request)
    {
        return ApiResult<long>.Ok(await _policyAppService.CreateAsync(request));
    }

    [HttpPut("{policyId:long}")]
    public async Task<ApiResult> UpdateAsync(long policyId, [FromBody] PolicyUpdateRequest request)
    {
        await _policyAppService.UpdateAsync(policyId, request);
        return ApiResult.Ok();
    }

    [HttpGet("versions/{policyVersionId:long}")]
    public async Task<ActionResult<ApiResult<PolicyVersionResponse>>> GetVersionAsync(long policyVersionId)
    {
        var item = await _policyVersionAppService.GetByIdAsync(policyVersionId);
        if (item is null)
        {
            return NotFound(ApiResult.Fail(404, $"策略版本不存在: {policyVersionId}"));
        }

        return ApiResult<PolicyVersionResponse>.Ok(item);
    }

    [HttpPost("{policyId:long}/versions")]
    public async Task<ApiResult<long>> SaveDraftAsync(long policyId, [FromBody] PolicyVersionSaveRequest request)
    {
        return ApiResult<long>.Ok(await _policyVersionAppService.SaveDraftAsync(policyId, request));
    }

    [HttpPost("versions/{policyVersionId:long}/preview")]
    public async Task<ApiResult<PolicyPreviewResponse>> PreviewAsync(long policyVersionId)
    {
        return ApiResult<PolicyPreviewResponse>.Ok(await _policyPreviewAppService.PreviewAsync(policyVersionId));
    }

    [HttpPost("versions/{policyVersionId:long}/validate")]
    public async Task<ApiResult<PolicyValidateResponse>> ValidateAsync(long policyVersionId)
    {
        return ApiResult<PolicyValidateResponse>.Ok(await _policyVersionAppService.ValidateAsync(policyVersionId));
    }

    [HttpPost("versions/{policyVersionId:long}/review/submit")]
    public async Task<ApiResult<long>> SubmitReviewAsync(long policyVersionId, [FromBody] PolicyReviewSubmitRequest request)
    {
        return ApiResult<long>.Ok(await _policyReviewAppService.SubmitAsync(policyVersionId, request.SubmittedBy, request.ReviewStage));
    }

    [HttpPost("versions/{policyVersionId:long}/review/approve")]
    public async Task<ApiResult> ApproveReviewAsync(long policyVersionId, [FromBody] PolicyReviewDecisionRequest request)
    {
        await _policyReviewAppService.ApproveAsync(policyVersionId, request.ReviewedBy, request.ReviewComment);
        return ApiResult.Ok();
    }

    [HttpPost("versions/{policyVersionId:long}/review/reject")]
    public async Task<ApiResult> RejectReviewAsync(long policyVersionId, [FromBody] PolicyReviewDecisionRequest request)
    {
        await _policyReviewAppService.RejectAsync(policyVersionId, request.ReviewedBy, request.ReviewComment);
        return ApiResult.Ok();
    }
}
