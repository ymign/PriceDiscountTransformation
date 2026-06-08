using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Commands;
using Pricing.RuleCenter.Application.Pricing.Queries;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【计价核心控制器】
/// <para>
/// 职责范围：统一计价引擎的 HTTP 入口，对外暴露"试算 → 确认 → 提交 → 取消 → 冲正"完整生命周期，
/// 以及特殊项目标识查询接口。
/// </para>
/// <para>
/// 路由前缀：<c>api/pricing</c>
/// </para>
/// <para>
/// 设计原则：
/// <list type="bullet">
///   <item>控制器层仅做 HTTP 路由分发和统一响应格式包装，不含任何业务逻辑。</item>
///   <item>幂等控制、事务边界、限额占用/释放、状态流转等核心逻辑通过 MediatR 用例处理器编排，保持接口层极薄。</item>
///   <item>所有金额相关的请求/响应字段均使用 <c>decimal</c> 类型，禁止 float/double。</item>
/// </list>
/// </para>
/// <para>
/// 三阶段确认模型（核心业务流程）：
/// <list type="number">
///   <item><b>confirm（确认）</b> — 试算通过后调用，占用额度（PR_LIMIT_OCCUPY），写入请求日志（PR_CHARGE_REQUEST_LOG），
///         返回 RequestId 供后续 commit/cancel 引用。此阶段必须幂等（键：sourceSystem + businessRequestNo + callType）。</item>
///   <item><b>commit（提交）</b> — HIS 成功落账后通知计价中心，将请求日志状态从 CONFIRM_PENDING 变更为 CONFIRMED，
///         确认额度占用永久生效。若 HIS 结算失败则调用 cancel 释放。</item>
///   <item><b>cancel（取消）</b> — HIS 未成功落账时调用，释放 confirm 阶段占用的额度，
///         状态变更为 CANCELLED。cancel 不处理已落账退费；已 commit 的收费必须走 reverse。</item>
/// </list>
/// </para>
/// <para>
/// 冲正（reverse）流程：针对已 commit 的记录做退费/冲销处理。
/// 退费按退费数量释放额度；部分退费必须校验"本次退费 + 历史已退"不超过原有效收费数量。
/// 2 小时规则按收费时间（包含重收时间）向前查 2 小时。
/// </para>
/// </summary>
/// <remarks>
/// 安全与稳定性要求：
/// <list type="bullet">
///   <item>confirm 接口必须幂等，REQUEST_FINGERPRINT 用于防重放。</item>
///   <item>超时重试必须复用同一业务号（businessRequestNo），不得生成新 RequestId。</item>
///   <item>权威单价当前作为诊断日志使用，缺价或差异仅记录 Warning，不阻断 simulate/confirm 流程。</item>
///   <item>并发额度控制通过 PR_LIMIT_LOCK + SELECT FOR UPDATE 实现，TIME_WINDOW 必须锁定
///         业务时间窗口覆盖的全部小时桶。</item>
///   <item>计价服务不可用时，渠道不得回退为普通计价，必须转人工或暂停。</item>
/// </list>
/// </remarks>
[ApiController]
[Authorize(Policy = "PricingService")]
[Route("api/pricing")]
public sealed class PricingController : ControllerBase
{
    /// <summary>
    /// MediatR 调度器，用于发送计价 Command/Query。
    /// <para>
    /// 包含：规则匹配、公式计算、双单位换算、金额/数量上下限校验、日数量限制、
    /// 时间窗数量限制、同组互斥、同手术封顶、子项加收、超出归零等全部计价逻辑。
    /// </para>
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// 构造函数，通过依赖注入获取 MediatR 调度器。
    /// </summary>
    /// <param name="mediator">MediatR 调度器，用于发送计价 Command/Query。</param>
    public PricingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 【试算接口】— 模拟计价，不占用额度，不写入正式折价明细。
    /// <para>
    /// HTTP 方法：POST  |  路由：<c>/api/pricing/calculate/simulate</c>
    /// </para>
    /// <para>
    /// 用途：渠道在收费录入界面实时预览折价金额时调用。试算结果仅用于展示，
    /// 不产生任何额度占用或正式折价明细；服务端会保留请求日志和步骤日志，方便追溯页面解释当时为什么算出该价格。
    /// </para>
    /// <para>
    /// 计算顺序（强制）：
    /// <list type="number">
    ///   <item>按项目、场景、部位、时间匹配规则</item>
    ///   <item>双单位换算（如已配置）</item>
    ///   <item>日数量限制 → 时间窗数量限制（如 2 小时窗）</item>
    ///   <item>同组互斥 / 同手术封顶</item>
    ///   <item>公式计算（如已配置，使用限制后的 FinalQty）</item>
    ///   <item>金额下限 → 金额上限</item>
    ///   <item>子项加收 / 附加项目</item>
    ///   <item>超出部分 → 0 元（不是整单，不是拒单）</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 计价请求体（<see cref="PricingCalculateRequest"/>），包含：
    /// <list type="bullet">
    ///   <item><c>Items</c> — HIS 本次收费动作下的费用明细集合（必填）</item>
    ///   <item><c>Items[].ItemCode</c> — HIS 收费项目编码（必填）</item>
    ///   <item><c>Items[].InputQty</c> — 收费数量（必填）</item>
    ///   <item><c>Items[].Unit</c> — 计价单位编码</item>
    ///   <item><c>SourceSystem</c> — 来源系统标识（HIS/自助机/微信等）</item>
    ///   <item><c>BusinessChargeTime</c> — 业务收费发生时间（规则生效、2 小时窗口、单日累计的时间基准）</item>
    ///   <item><c>Items[].PricingParts</c> — 多肿物/多部位/多面积明细（复杂输入必须使用此字段，不能压成单个数量）</item>
    /// </list>
    /// </param>
    /// <returns>
    /// 试算结果（<see cref="PricingCalculateResponse"/>），包含：
    /// <list type="bullet">
    ///   <item><c>Items</c> — 每条费用明细的独立计价结果，多明细落账应优先使用该集合</item>
    ///   <item><c>FinalAmount</c> — 最终折价金额（decimal，2 位小数，四舍五入）</item>
    ///   <item><c>TraceSteps</c> — 计算步骤明细（用于前端展示和问题排查）</item>
    ///   <item><c>MatchedRuleIds</c> — 匹配到的规则 ID 集合（未匹配为空集合）</item>
    /// </list>
    /// </returns>
    [HttpPost("calculate/simulate")]
    public async Task<ApiResult<PricingCalculateResponse>> SimulateAsync(
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _mediator.Send(new SimulatePricingCommand(request));
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>
    /// 【批量试算接口】— 与 simulate 使用同一请求结构，要求多条 Items 共享批量上下文。
    /// </summary>
    [HttpPost("calculate/batch-simulate")]
    public async Task<ApiResult<PricingCalculateResponse>> BatchSimulateAsync(
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _mediator.Send(new SimulatePricingCommand(request));
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>
    /// 【确认计价接口】— 正式计价并占用额度，是三阶段确认模型的第一步。
    /// <para>
    /// HTTP 方法：POST  |  路由：<c>/api/pricing/calculate/confirm</c>
    /// </para>
    /// <para>
    /// 用途：渠道确定要收费时调用。此接口会：
    /// <list type="number">
    ///   <item>执行完整的计价计算（与 simulate 相同的计算逻辑）</item>
    ///   <item>在 PR_LIMIT_OCCUPY 表中占用额度（使用 SELECT FOR UPDATE 防并发超限）</item>
    ///   <item>在 PR_CHARGE_REQUEST_LOG 表中写入请求日志（状态：CONFIRM_PENDING）</item>
    ///   <item>在 PR_CHARGE_TRACE_STEP 表中写入计算步骤日志</item>
    ///   <item>在 PR_CHARGE_DISCOUNT_DETAIL 表中写入折价结果明细</item>
    /// </list>
    /// </para>
    /// <para>
    /// 幂等性保证：同一 sourceSystem + businessRequestNo + callType 的重复请求，
    /// 返回首次确认的相同结果（含相同 RequestId），不会重复占用额度。
    /// REQUEST_FINGERPRINT 用于防重放。
    /// </para>
    /// <para>
    /// 单价诊断：confirm 会读取权威物价主数据并记录差异日志，但不会因单价差异拦截业务流程。
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 计价请求体，除试算所需字段外，还应包含：
    /// <list type="bullet">
    ///   <item><c>BusinessRequestNo</c> — 业务请求号（幂等键组成部分，建议渠道保证唯一且稳定）</item>
    ///   <item><c>SourceSystem</c> — 来源系统标识（幂等键组成部分）</item>
    /// </list>
    /// </param>
    /// <returns>
    /// 确认计价结果，除试算结果字段外，还包含：
    /// <list type="bullet">
    ///   <item><c>RequestId</c> — 计价请求唯一标识，后续 commit/cancel 必须携带此 ID</item>
    ///   <item><c>ExpireAt</c> — 确认有效期，超时未 commit 的请求将被后台任务扫描清理</item>
    /// </list>
    /// </returns>
    [HttpPost("calculate/confirm")]
    public async Task<ApiResult<PricingCalculateResponse>> ConfirmAsync(
        [FromBody] PricingCalculateRequest request)
    {
        var result = await _mediator.Send(new ConfirmPricingCommand(request));
        return ApiResult<PricingCalculateResponse>.Ok(result);
    }

    /// <summary>
    /// 【提交接口】— HIS 成功落账后通知计价中心，是三阶段确认模型的第二步。
    /// <para>
    /// HTTP 方法：POST  |  路由：<c>/api/pricing/calculate/commit</c>
    /// </para>
    /// <para>
    /// 用途：HIS 结算成功后调用，将请求日志状态从 CONFIRM_PENDING 变更为 CONFIRMED，
    /// 确认额度占用永久生效。若 HIS 结算失败，应调用 cancel 而非 commit。
    /// </para>
    /// <para>
    /// 前置条件：必须存在已确认（CONFIRM_PENDING）的请求记录。
    /// </para>
    /// <para>
    /// 状态变更：PR_CHARGE_REQUEST_LOG.Status: CONFIRM_PENDING → CONFIRMED。
    /// 同时更新 PR_LIMIT_OCCUPY 中对应占用记录的状态。
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 提交请求（<see cref="PricingCommitRequest"/>），必须包含：
    /// <list type="bullet">
    ///   <item><c>RequestId</c> — confirm 阶段返回的请求 ID（必填，用于定位待提交记录）</item>
    ///   <item><c>ChargeNo</c> — HIS 实际收费单号（可选，用于关联 HIS 账务）</item>
    ///   <item><c>ActualItems</c> — HIS 真实落账明细（必填，用于和 confirm 结果对账）</item>
    /// </list>
    /// </param>
    /// <returns>统一成功响应，无额外业务数据。</returns>
    [HttpPost("calculate/commit")]
    public async Task<ApiResult> CommitAsync([FromBody] PricingCommitRequest request)
    {
        await _mediator.Send(new CommitPricingCommand(request));
        return ApiResult.Ok();
    }

    /// <summary>
    /// 【取消接口】— 取消已确认但未提交的计价结果，释放占用额度，是三阶段确认模型的第三步（异常分支）。
    /// <para>
    /// HTTP 方法：POST  |  路由：<c>/api/pricing/calculate/cancel</c>
    /// </para>
    /// <para>
    /// 用途：HIS 未成功落账（如用户取消收费、结算失败等）时调用。
    /// 释放 confirm 阶段占用的额度，将请求日志状态变更为 CANCELLED。
    /// </para>
    /// <para>
    /// 状态变更：
    /// <list type="bullet">
    ///   <item>PR_CHARGE_REQUEST_LOG.Status: CONFIRM_PENDING → CANCELLED</item>
    ///   <item>PR_LIMIT_OCCUPY：删除或释放对应占用记录</item>
    /// </list>
    /// </para>
    /// <para>
    /// 注意：cancel 必须同时更新请求日志、限额占用记录的状态，保证一致性。
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 取消请求（<see cref="PricingCancelRequest"/>），必须包含：
    /// <list type="bullet">
    ///   <item><c>RequestId</c> — confirm 阶段返回的请求 ID（必填，用于定位待取消记录）</item>
    /// </list>
    /// </param>
    /// <returns>统一成功响应，无额外业务数据。</returns>
    [HttpPost("calculate/cancel")]
    public async Task<ApiResult> CancelAsync([FromBody] PricingCancelRequest request)
    {
        await _mediator.Send(new CancelPricingCommand(request));
        return ApiResult.Ok();
    }

    /// <summary>
    /// 【冲正/退费接口】— 针对已 commit 成功并进入 CONFIRMED 的计价记录做退费或冲销处理。
    /// <para>
    /// HTTP 方法：POST  |  路由：<c>/api/pricing/calculate/reverse</c>
    /// </para>
    /// <para>
    /// 用途：患者退费、医嘱撤销等场景下调用。冲正会：
    /// <list type="number">
    ///   <item>校验原始记录状态必须为 CONFIRMED</item>
    ///   <item>按退费数量释放额度（当日退费释放数量，隔日退费重收后按重收当天重新校验额度）</item>
    ///   <item>部分退费时校验"本次退费 + 历史已退"不超过原有效收费数量</item>
    ///   <item>写入 PR_CHARGE_REVERSE_LOG 退费流水</item>
    ///   <item>全额退费时更新 PR_CHARGE_REQUEST_LOG 状态为 REVERSED；部分退费时保留 CONFIRMED 并写冲正日志</item>
    ///   <item>更新 PR_LIMIT_OCCUPY 释放对应额度</item>
    /// </list>
    /// </para>
    /// <para>
    /// 2 小时窗口规则：退费释放额度后，2 小时规则按收费时间（包含重收时间）向前查 2 小时。
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 冲正请求（<see cref="PricingReverseRequest"/>），必须包含：
    /// <list type="bullet">
    ///   <item><c>OriginalRequestId</c> — 原始已提交的请求 ID（必填，用于定位待冲正记录）</item>
    ///   <item><c>ReverseNo</c> — HIS 稳定退费流水号（必填，用于幂等）</item>
    ///   <item><c>ReverseQty</c> — 退费数量（可选；为空时按匹配明细全退）</item>
    ///   <item><c>ReverseTime</c> — 退费业务时间（可选，用于确定退费日期和额度释放计算）</item>
    ///   <item><c>Reason</c> — 退费原因（可选，用于审计追溯）</item>
    /// </list>
    /// </param>
    /// <returns>统一成功响应，无额外业务数据。</returns>
    [HttpPost("calculate/reverse")]
    public async Task<ApiResult> ReverseAsync([FromBody] PricingReverseRequest request)
    {
        await _mediator.Send(new ReversePricingCommand(request));
        return ApiResult.Ok();
    }

    /// <summary>
    /// 【特殊项目标识查询接口】— 查询指定收费项目是否存在特殊计价规则。
    /// <para>
    /// HTTP 方法：GET  |  路由：<c>/api/pricing/items/{itemCode}/special-flag</c>
    /// </para>
    /// <para>
    /// 用途：渠道收费录入界面判断某个收费项目是否需要走折价/特殊计价逻辑。
    /// 若为特殊项目，渠道应展示折价相关 UI（如部位选择、数量输入等）；
    /// 若非特殊项目，走普通计价流程。
    /// </para>
    /// <para>
    /// "折价项目"和"特殊项目"是同一标识，仅命名不同。
    /// </para>
    /// </summary>
    /// <param name="itemCode">
    /// HIS 收费项目编码（路径参数），例如 <c>"FW001"</c>。
    /// </param>
    /// <param name="query">
    /// 可选查询维度：chargeScene、businessChargeTime、visitType、bodyPartCode、chargeDeptCode。
    /// 传入后服务端会尽量按当前激活运行时包评估条件，减少只按 itemCode 粗判导致的误弹窗。
    /// </param>
    /// <returns>
    /// 特殊项目标识响应（<see cref="SpecialFlagResponse"/>），包含：
    /// <list type="bullet">
    ///   <item><c>IsSpecial</c> — 是否为特殊项目（true/false）</item>
    ///   <item><c>RuleCount</c> — 当前生效的有效规则数量</item>
    /// </list>
    /// </returns>
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
