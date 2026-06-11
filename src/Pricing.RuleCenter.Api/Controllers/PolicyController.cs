using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 策略平台管理控制器，负责策略主档、草稿版本、预览校验和评审流转接口。
/// </summary>
[ApiController]
[Authorize(Policy = "RuleAdmin")]
[Route("api/pricing/policies")]
public sealed class PolicyController : ControllerBase
{
    private readonly PolicyAppService _policyAppService;
    private readonly PolicyVersionAppService _policyVersionAppService;
    private readonly PolicyPreviewAppService _policyPreviewAppService;
    private readonly PolicyReviewAppService _policyReviewAppService;
    private readonly PolicyImportService _policyImportService;

    /// <summary>
    /// 初始化策略平台管理控制器。
    /// </summary>
    /// <param name="policyAppService">策略主档应用服务。</param>
    /// <param name="policyVersionAppService">策略版本应用服务。</param>
    /// <param name="policyPreviewAppService">策略预览应用服务。</param>
    /// <param name="policyReviewAppService">策略评审应用服务。</param>
    /// <param name="policyImportService">历史规则导入应用服务。</param>
    public PolicyController(
        PolicyAppService policyAppService,
        PolicyVersionAppService policyVersionAppService,
        PolicyPreviewAppService policyPreviewAppService,
        PolicyReviewAppService policyReviewAppService,
        PolicyImportService policyImportService)
    {
        _policyAppService = policyAppService;
        _policyVersionAppService = policyVersionAppService;
        _policyPreviewAppService = policyPreviewAppService;
        _policyReviewAppService = policyReviewAppService;
        _policyImportService = policyImportService;
    }

    /// <summary>
    /// 查询全部策略主档列表。
    /// </summary>
    /// <returns>策略概要列表。</returns>
    [HttpGet]
    public async Task<ApiResult<IReadOnlyList<PolicyResponse>>> GetAllAsync()
    {
        return ApiResult<IReadOnlyList<PolicyResponse>>.Ok(await _policyAppService.GetAllAsync());
    }

    /// <summary>
    /// 按主键查询策略详情。
    /// </summary>
    /// <param name="policyId">策略主键。</param>
    /// <returns>命中时返回策略详情；不存在时返回 404。</returns>
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

    /// <summary>
    /// 创建策略主档。
    /// </summary>
    /// <param name="request">策略创建请求。</param>
    /// <returns>新策略主键。</returns>
    [HttpPost]
    public async Task<ApiResult<long>> CreateAsync([FromBody] PolicyCreateRequest request)
    {
        return ApiResult<long>.Ok(await _policyAppService.CreateAsync(request));
    }

    /// <summary>
    /// 更新策略主档基础信息。
    /// </summary>
    /// <param name="policyId">策略主键。</param>
    /// <param name="request">策略更新请求。</param>
    /// <returns>统一成功响应。</returns>
    [HttpPut("{policyId:long}")]
    public async Task<ApiResult> UpdateAsync(long policyId, [FromBody] PolicyUpdateRequest request)
    {
        await _policyAppService.UpdateAsync(policyId, request);
        return ApiResult.Ok();
    }

    /// <summary>
    /// 查询单个策略版本详情。
    /// </summary>
    /// <param name="policyVersionId">策略版本主键。</param>
    /// <returns>命中时返回策略版本详情；不存在时返回 404。</returns>
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

    /// <summary>
    /// 保存策略草稿版本。
    /// </summary>
    /// <param name="policyId">策略主键。</param>
    /// <param name="request">草稿版本保存请求。</param>
    /// <returns>新建或更新后的策略版本主键。</returns>
    [HttpPost("{policyId:long}/versions")]
    public async Task<ApiResult<long>> SaveDraftAsync(long policyId, [FromBody] PolicyVersionSaveRequest request)
    {
        return ApiResult<long>.Ok(await _policyVersionAppService.SaveDraftAsync(policyId, request));
    }

    /// <summary>
    /// 预览策略版本编译结果和绑定效果。
    /// </summary>
    /// <param name="policyVersionId">策略版本主键。</param>
    /// <returns>策略预览结果。</returns>
    [HttpPost("versions/{policyVersionId:long}/preview")]
    public async Task<ApiResult<PolicyPreviewResponse>> PreviewAsync(long policyVersionId)
    {
        return ApiResult<PolicyPreviewResponse>.Ok(await _policyPreviewAppService.PreviewAsync(policyVersionId));
    }

    /// <summary>
    /// 校验策略版本的表达式、绑定和发布前约束。
    /// </summary>
    /// <param name="policyVersionId">策略版本主键。</param>
    /// <returns>策略校验结果。</returns>
    [HttpPost("versions/{policyVersionId:long}/validate")]
    public async Task<ApiResult<PolicyValidateResponse>> ValidateAsync(long policyVersionId)
    {
        return ApiResult<PolicyValidateResponse>.Ok(await _policyVersionAppService.ValidateAsync(policyVersionId));
    }

    /// <summary>
    /// 提交策略版本进入评审流。
    /// </summary>
    /// <param name="policyVersionId">策略版本主键。</param>
    /// <param name="request">评审提交流水信息。</param>
    /// <returns>新评审记录主键。</returns>
    [HttpPost("versions/{policyVersionId:long}/review/submit")]
    public async Task<ApiResult<long>> SubmitReviewAsync(long policyVersionId, [FromBody] PolicyReviewSubmitRequest request)
    {
        return ApiResult<long>.Ok(await _policyReviewAppService.SubmitAsync(policyVersionId, request.SubmittedBy, request.ReviewStage));
    }

    /// <summary>
    /// 审批通过策略版本评审。
    /// </summary>
    /// <param name="policyVersionId">策略版本主键。</param>
    /// <param name="request">评审决定请求。</param>
    /// <returns>统一成功响应。</returns>
    [HttpPost("versions/{policyVersionId:long}/review/approve")]
    public async Task<ApiResult> ApproveReviewAsync(long policyVersionId, [FromBody] PolicyReviewDecisionRequest request)
    {
        await _policyReviewAppService.ApproveAsync(policyVersionId, request.ReviewedBy, request.ReviewComment);
        return ApiResult.Ok();
    }

    /// <summary>
    /// 驳回策略版本评审。
    /// </summary>
    /// <param name="policyVersionId">策略版本主键。</param>
    /// <param name="request">评审决定请求。</param>
    /// <returns>统一成功响应。</returns>
    [HttpPost("versions/{policyVersionId:long}/review/reject")]
    public async Task<ApiResult> RejectReviewAsync(long policyVersionId, [FromBody] PolicyReviewDecisionRequest request)
    {
        await _policyReviewAppService.RejectAsync(policyVersionId, request.ReviewedBy, request.ReviewComment);
        return ApiResult.Ok();
    }

    /// <summary>
    /// 将旧规则批量导入策略平台。
    /// </summary>
    /// <param name="request">历史规则导入请求。</param>
    /// <returns>导入生成的策略主键集合。</returns>
    [HttpPost("import")]
    public async Task<ApiResult<IReadOnlyList<long>>> ImportLegacyRulesAsync([FromBody] PolicyImportRequest request)
    {
        return ApiResult<IReadOnlyList<long>>.Ok(await _policyImportService.ImportAsync(request.RuleIds, request.ImportedBy));
    }
}
