using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 计价核心控制器：试算 → 确认 → 提交 → 取消 → 冲正完整生命周期。
/// </summary>
[ApiController]
[Authorize(Policy = "PricingService")]
[Route("api/pricing")]
public sealed class PricingController : ControllerBase
{
    /// <summary>
    /// 试算：模拟计价，不占用额度。
    /// </summary>
    /// <remarks>
    /// 计算顺序：匹配规则 → 双单位换算 → 日数量限制 → 时间窗数量限制 → 同组互斥 → 公式计算 → 金额下限 → 金额上限 → 子项加收 → 超出归零。
    /// business_request_no 可选；如果传入，必须保证本次试算唯一，重复会返回 409 BUSINESS_REQUEST_NO_DUPLICATED。
    /// </remarks>
    [HttpPost("calculate/simulate")]
    public async Task<ApiResult<PricingCalculateResponse>> SimulateAsync(
        [FromServices] PricingSimulateWorkflow simulateWorkflow,
        [FromBody] PricingCalculateRequest request)
    {
        var result = await simulateWorkflow.ExecuteAsync(request);
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>确认计价：正式计价并占用额度，返回 RequestId 供后续 commit/cancel 引用。</summary>
    /// <remarks>business_request_no 必填且必须稳定，confirm 超时重试必须复用同一个值。</remarks>
    [HttpPost("calculate/confirm")]
    public async Task<ApiResult<PricingCalculateResponse>> ConfirmAsync(
        [FromServices] PricingConfirmWorkflow confirmWorkflow,
        [FromBody] PricingCalculateRequest request)
    {
        var result = await confirmWorkflow.ExecuteAsync(request);
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>落账提交：HIS 成功落账后通知，将 CONFIRM_PENDING 推进为 CONFIRMED。</summary>
    /// <remarks>commit 使用 confirm 响应返回的 request_id，不使用 business_request_no 定位。</remarks>
    [HttpPost("calculate/commit")]
    public async Task<ApiResult<PricingCommitResponse>> CommitAsync(
        [FromServices] PricingCommitWorkflow commitWorkflow,
        [FromBody] PricingCommitRequest request)
    {
        var result = await commitWorkflow.ExecuteAsync(request);
        return ApiResult<PricingCommitResponse>.Ok(result);
    }

    /// <summary>取消确认：HIS 未落账时释放待确认额度。</summary>
    /// <remarks>cancel 使用 confirm 响应返回的 request_id，不使用 business_request_no 定位。</remarks>
    [HttpPost("calculate/cancel")]
    public async Task<ApiResult<PricingCancelResponse>> CancelAsync(
        [FromServices] PricingCancelWorkflow cancelWorkflow,
        [FromBody] PricingCancelRequest request)
    {
        var result = await cancelWorkflow.ExecuteAsync(request);
        return ApiResult<PricingCancelResponse>.Ok(result);
    }

    /// <summary>冲正退费：针对已落账记录做退费/冲销处理。</summary>
    /// <remarks>reverse 的幂等边界是 original_request_id + reverse_no，同一退费流水重试必须复用 reverse_no。</remarks>
    [HttpPost("calculate/reverse")]
    public async Task<ApiResult<PricingReverseResponse>> ReverseAsync(
        [FromServices] PricingReverseWorkflow reverseWorkflow,
        [FromBody] PricingReverseRequest request)
    {
        var result = await reverseWorkflow.ExecuteAsync(request);
        return ApiResult<PricingReverseResponse>.Ok(result);
    }

    /// <summary>查询特殊项目标识：判断收费项目是否需要走折价计价逻辑。</summary>
    /// <remarks>路径参数是项目主键，Query 参数用于提前模拟部分规则条件，减少只按 itemCode 粗判的误弹窗。</remarks>
    [HttpGet("items/{itemCode}/special-flag")]
    public async Task<ApiResult<SpecialFlagResponse>> GetSpecialFlagAsync(
        [FromServices] PricingSpecialFlagResolver specialFlagResolver,
        string itemCode,
        [FromQuery] SpecialFlagQueryRequest? query)
    {
        var request = SpecialFlagRequest.From(itemCode, query);
        var result = await specialFlagResolver.ResolveAsync(request);
        return ApiResult<SpecialFlagResponse>.Ok(result);
    }

    /// <summary>批量查询特殊项目标识：一次收费动作下多条费用明细一起判断是否需要走折价计价逻辑。</summary>
    /// <remarks>
    /// 请求体包含收费动作级上下文和明细级覆盖字段。响应逐行返回最终参与匹配的场景、时间、就诊类型、部位和科室，便于调用方排查误弹窗或漏弹窗。
    /// business_request_no 可选，仅用于诊断，不做幂等校验。
    /// </remarks>
    [HttpPost("items/special-flags")]
    public async Task<ApiResult<SpecialFlagBatchResponse>> BatchSpecialFlagsAsync(
        [FromServices] PricingSpecialFlagResolver specialFlagResolver,
        [FromBody] SpecialFlagBatchRequest request)
    {
        var result = await specialFlagResolver.ResolveBatchAsync(request);
        return ApiResult<SpecialFlagBatchResponse>.Ok(result);
    }

    /// <summary>快速判断本次收费动作中是否存在任一特殊项目。</summary>
    /// <remarks>
    /// 该接口是 special-flags 的轻量闸门版本，只回答是否需要进入统一计价。
    /// 一旦发现首条特殊项目即可返回，不提供整批逐行诊断结果。
    /// </remarks>
    [HttpPost("items/special-flags/any")]
    public async Task<ApiResult<SpecialFlagAnyResponse>> AnySpecialFlagAsync(
        [FromServices] PricingSpecialFlagResolver specialFlagResolver,
        [FromBody] SpecialFlagBatchRequest request)
    {
        var result = await specialFlagResolver.ResolveAnyAsync(request);
        return ApiResult<SpecialFlagAnyResponse>.Ok(result);
    }
}
