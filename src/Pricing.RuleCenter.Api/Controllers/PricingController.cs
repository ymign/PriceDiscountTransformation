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
    /// </remarks>
    [HttpPost("calculate/simulate")]
    public async Task<ApiResult<PricingCalculateResponse>> SimulateAsync(
        [FromServices] PricingSimulateWorkflow simulateWorkflow,
        [FromBody] PricingCalculateRequest request)
    {
        var result = await simulateWorkflow.ExecuteAsync(request);
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>
    /// 批量试算：多条费用明细共享批量上下文，不占用额度。
    /// </summary>
    /// <remarks>
    /// 与 simulate 使用同一工作流。下游根据 Items.Count 构造批量上下文，使多条明细参与同组互斥、同手术封顶和窗口额度的请求内虚拟占用。
    /// </remarks>
    [HttpPost("calculate/batch-simulate")]
    public async Task<ApiResult<PricingCalculateResponse>> BatchSimulateAsync(
        [FromServices] PricingSimulateWorkflow simulateWorkflow,
        [FromBody] PricingCalculateRequest request)
    {
        var result = await simulateWorkflow.ExecuteAsync(request);
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>确认计价：正式计价并占用额度，返回 RequestId 供后续 commit/cancel 引用。</summary>
    [HttpPost("calculate/confirm")]
    public async Task<ApiResult<PricingCalculateResponse>> ConfirmAsync(
        [FromServices] PricingConfirmWorkflow confirmWorkflow,
        [FromBody] PricingCalculateRequest request)
    {
        var result = await confirmWorkflow.ExecuteAsync(request);
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>落账提交：HIS 成功落账后通知，将 CONFIRM_PENDING 推进为 CONFIRMED。</summary>
    [HttpPost("calculate/commit")]
    public async Task<ApiResult> CommitAsync(
        [FromServices] PricingCommitWorkflow commitWorkflow,
        [FromBody] PricingCommitRequest request)
    {
        await commitWorkflow.ExecuteAsync(request);
        return ApiResult.Ok();
    }

    /// <summary>取消确认：HIS 未落账时释放待确认额度。</summary>
    [HttpPost("calculate/cancel")]
    public async Task<ApiResult> CancelAsync(
        [FromServices] PricingCancelWorkflow cancelWorkflow,
        [FromBody] PricingCancelRequest request)
    {
        await cancelWorkflow.ExecuteAsync(request);
        return ApiResult.Ok();
    }

    /// <summary>冲正退费：针对已落账记录做退费/冲销处理。</summary>
    [HttpPost("calculate/reverse")]
    public async Task<ApiResult> ReverseAsync(
        [FromServices] PricingReverseWorkflow reverseWorkflow,
        [FromBody] PricingReverseRequest request)
    {
        await reverseWorkflow.ExecuteAsync(request);
        return ApiResult.Ok();
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
}
