using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Dto;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 规则审批控制器。
/// </summary>
[ApiController]
[Authorize(Policy = "RuleAdmin")]
[Route("api/pricing/rules/{ruleId:long}/versions/{versionNo:int}")]
public sealed class RuleApprovalController : ControllerBase
{
    private readonly RuleApprovalAppService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleApprovalController"/> class.
    /// </summary>
    /// <param name="service">规则审批应用服务。</param>
    public RuleApprovalController(RuleApprovalAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 查询指定规则版本的审批记录。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <returns>指定版本的审批记录列表。</returns>
    [HttpGet("approvals")]
    public async Task<ApiResult<IReadOnlyList<RuleApprovalResponse>>> GetApprovalsAsync(long ruleId, int versionNo)
    {
        var items = await _service.GetByRuleIdAsync(ruleId);
        return ApiResult<IReadOnlyList<RuleApprovalResponse>>.Ok(
            items.Where(i => i.VersionNo == versionNo).ToList());
    }

    /// <summary>
    /// 提交规则版本审批申请。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">审批提交请求。</param>
    /// <returns>新建审批记录主键。</returns>
    [HttpPost("submit-approval")]
    public async Task<ApiResult<long>> SubmitApprovalAsync(
        long ruleId,
        int versionNo,
        [FromBody] RuleApprovalSubmitRequest request)
    {
        var approvalId = await _service.SubmitAsync(ruleId, versionNo, request);
        return ApiResult<long>.Ok(approvalId);
    }

    /// <summary>
    /// 审批通过规则版本申请。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">审批决策请求。</param>
    /// <returns>统一响应结果。</returns>
    [HttpPost("approve")]
    public async Task<ApiResult> ApproveAsync(
        long ruleId,
        int versionNo,
        [FromBody] RuleApprovalDecisionRequest request)
    {
        await _service.ApproveAsync(ruleId, versionNo, request);
        return ApiResult.Ok();
    }

    /// <summary>
    /// 驳回规则版本审批申请。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="versionNo">规则版本号。</param>
    /// <param name="request">审批决策请求。</param>
    /// <returns>统一响应结果。</returns>
    [HttpPost("reject")]
    public async Task<ApiResult> RejectAsync(
        long ruleId,
        int versionNo,
        [FromBody] RuleApprovalDecisionRequest request)
    {
        await _service.RejectAsync(ruleId, versionNo, request);
        return ApiResult.Ok();
    }
}
