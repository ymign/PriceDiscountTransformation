using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 规则审批控制器。
/// </summary>
[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions/{versionNo:int}")]
public sealed class RuleApprovalController : ControllerBase
{
    private readonly RuleApprovalAppService _service;

    public RuleApprovalController(RuleApprovalAppService service)
    {
        _service = service;
    }

    [HttpGet("approvals")]
    public async Task<ApiResponse<IReadOnlyList<RuleApprovalResponse>>> GetApprovalsAsync(long ruleId, int versionNo)
    {
        var items = await _service.GetByRuleIdAsync(ruleId);
        return ApiResponse<IReadOnlyList<RuleApprovalResponse>>.Ok(
            items.Where(i => i.VersionNo == versionNo).ToList());
    }

    [HttpPost("submit-approval")]
    public async Task<ApiResponse<long>> SubmitApprovalAsync(
        long ruleId,
        int versionNo,
        [FromBody] RuleApprovalSubmitRequest request)
    {
        var approvalId = await _service.SubmitAsync(ruleId, versionNo, request);
        return ApiResponse<long>.Ok(approvalId);
    }

    [HttpPost("approve")]
    public async Task<ApiResponse> ApproveAsync(
        long ruleId,
        int versionNo,
        [FromBody] RuleApprovalDecisionRequest request)
    {
        await _service.ApproveAsync(ruleId, versionNo, request);
        return ApiResponse.Ok();
    }

    [HttpPost("reject")]
    public async Task<ApiResponse> RejectAsync(
        long ruleId,
        int versionNo,
        [FromBody] RuleApprovalDecisionRequest request)
    {
        await _service.RejectAsync(ruleId, versionNo, request);
        return ApiResponse.Ok();
    }
}


