using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing")]
/// <summary>
/// 计价主接口控制器，暴露试算、确认、提交、取消、冲正和特殊项目识别接口。
/// </summary>
/// <remarks>
/// 控制器只承担 HTTP 路由和统一响应包装，幂等、事务、限额占用和状态流转全部委托给
/// <see cref="PricingApiService"/>，避免接口层散落业务规则。
/// </remarks>
public sealed class PricingController : ControllerBase
{
    /// <summary>
    /// _service 服务依赖，用于复用已经封装好的业务编排或领域处理能力。
    /// </summary>
    private readonly PricingApiService _service;

    /// <summary>
    /// 初始化计价主接口控制器。
    /// </summary>
    /// <param name="service">计价应用服务。</param>
    public PricingController(PricingApiService service)
    {
        _service = service;
    }

    [HttpPost("calculate/simulate")]
    /// <summary>
    /// 执行试算，不写保护占用。
    /// </summary>
    /// <param name="request">计价请求。</param>
    /// <returns>计价结果和追踪步骤。</returns>
    public async Task<ApiResponse<PricingCalculateResponse>> SimulateAsync(
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _service.SimulateAsync(request);
        return ApiResponse<PricingCalculateResponse>.Ok(result);
    }

    [HttpPost("calculate/confirm")]
    /// <summary>
    /// 执行确认计价，生成待提交保护占用。
    /// </summary>
    /// <param name="request">计价请求，建议包含稳定 BusinessRequestNo 以支持幂等。</param>
    /// <returns>确认计价结果，包含后续 commit/cancel 所需 RequestId。</returns>
    public async Task<ApiResponse<PricingCalculateResponse>> ConfirmAsync(
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _service.ConfirmAsync(request);
        return ApiResponse<PricingCalculateResponse>.Ok(result);
    }

    [HttpPost("calculate/commit")]
    /// <summary>
    /// 提交已确认计价结果，表示 HIS 已成功落账。
    /// </summary>
    /// <param name="request">提交请求，必须包含 confirm 返回的 RequestId。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> CommitAsync([FromBody] PricingCommitRequest request)
    {
        await _service.CommitAsync(request);
        return ApiResponse.Ok();
    }

    [HttpPost("calculate/cancel")]
    /// <summary>
    /// 取消待提交确认结果，释放保护占用。
    /// </summary>
    /// <param name="request">取消请求，必须包含 confirm 返回的 RequestId。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> CancelAsync([FromBody] PricingCancelRequest request)
    {
        await _service.CancelAsync(request);
        return ApiResponse.Ok();
    }

    [HttpPost("calculate/reverse")]
    /// <summary>
    /// 冲正已提交计价结果。
    /// </summary>
    /// <param name="request">冲正请求，必须包含原始已提交 RequestId。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> ReverseAsync([FromBody] PricingReverseRequest request)
    {
        await _service.ReverseAsync(request);
        return ApiResponse.Ok();
    }

    [HttpGet("items/{itemCode}/special-flag")]
    /// <summary>
    /// 查询项目是否存在特殊计价规则。
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码。</param>
    /// <returns>特殊项目标识和有效规则数量。</returns>
    public async Task<ApiResponse<SpecialFlagResponse>> GetSpecialFlagAsync(string itemCode)
    {
        var result = await _service.GetSpecialFlagAsync(itemCode);
        return ApiResponse<SpecialFlagResponse>.Ok(result);
    }
}
