using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Commands;
using Pricing.RuleCenter.Application.Pricing.Queries;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 计价核心控制器：试算 → 确认 → 提交 → 取消 → 冲正完整生命周期。
/// </summary>
[ApiController]
[Authorize(Policy = "PricingService")]
[Route("api/pricing")]
public sealed class PricingController : ControllerBase
{
    private readonly IMediator _mediator;

    public PricingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 试算：模拟计价，不占用额度。
    /// </summary>
    /// <remarks>
    /// 计算顺序：匹配规则 → 双单位换算 → 日数量限制 → 时间窗数量限制 → 同组互斥 → 公式计算 → 金额下限 → 金额上限 → 子项加收 → 超出归零。
    /// </remarks>
    [HttpPost("calculate/simulate")]
    public async Task<ApiResult<PricingCalculateResponse>> SimulateAsync(
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _mediator.Send(new SimulatePricingCommand(request));
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
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _mediator.Send(new SimulatePricingCommand(request));
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>确认计价：正式计价并占用额度，返回 RequestId 供后续 commit/cancel 引用。</summary>
    [HttpPost("calculate/confirm")]
    public async Task<ApiResult<PricingCalculateResponse>> ConfirmAsync(
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _mediator.Send(new ConfirmPricingCommand(request));
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>落账提交：HIS 成功落账后通知，将 CONFIRM_PENDING 推进为 CONFIRMED。</summary>
    [HttpPost("calculate/commit")]
    public async Task<ApiResult> CommitAsync([FromBody] PricingCommitRequest request)
    {
        await _mediator.Send(new CommitPricingCommand(request));
        return ApiResult.Ok();
    }

    /// <summary>取消确认：HIS 未落账时释放待确认额度。</summary>
    [HttpPost("calculate/cancel")]
    public async Task<ApiResult> CancelAsync([FromBody] PricingCancelRequest request)
    {
        await _mediator.Send(new CancelPricingCommand(request));
        return ApiResult.Ok();
    }

    /// <summary>冲正退费：针对已落账记录做退费/冲销处理。</summary>
    [HttpPost("calculate/reverse")]
    public async Task<ApiResult> ReverseAsync([FromBody] PricingReverseRequest request)
    {
        await _mediator.Send(new ReversePricingCommand(request));
        return ApiResult.Ok();
    }

    /// <summary>查询特殊项目标识：判断收费项目是否需要走折价计价逻辑。</summary>
    /// <remarks>路径参数是项目主键，Query 参数用于提前模拟部分规则条件，减少只按 itemCode 粗判的误弹窗。</remarks>
    [HttpGet("items/{itemCode}/special-flag")]
    public async Task<ApiResult<SpecialFlagResponse>> GetSpecialFlagAsync(
        string itemCode,
        [FromQuery] SpecialFlagQueryRequest? query)
    {
        var request = SpecialFlagRequest.From(itemCode, query);
        var result = await _mediator.Send(new GetSpecialFlagQuery(
            request.ItemCode,
            request.ChargeScene,
            request.BusinessChargeTime,
            request.VisitType,
            request.BodyPartCode,
            request.ChargeDeptCode));
        return ApiResult<SpecialFlagResponse>.Ok(result);
    }
}
