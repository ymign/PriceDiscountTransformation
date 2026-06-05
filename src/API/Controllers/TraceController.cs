using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Catalog;
using Pricing.RuleCenter.Application.Trace;
using Pricing.RuleCenter.Application.Background;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【计价追溯查询控制器】
/// <para>
/// 职责范围：计价请求日志、执行步骤和折价明细的查询，支撑三条追溯链路：
/// <list type="bullet">
///   <item><b>规则变更链</b> — 某条计价结果是由哪个版本的规则计算得出的</item>
///   <item><b>计算过程链</b> — 计价引擎每一步的输入、输出和中间值</item>
///   <item><b>最终结果链</b> — 折价金额的明细构成和状态流转</item>
/// </list>
/// </para>
/// <para>
/// 路由前缀：<c>api/pricing/trace</c>
/// </para>
/// <para>
/// 追溯数据来源：
/// <list type="bullet">
///   <item><b>PR_CHARGE_REQUEST_LOG</b> — 计价请求日志，记录每次 confirm/commit/cancel/reverse 的请求和响应</item>
///   <item><b>PR_CHARGE_TRACE_STEP</b> — 计算步骤日志，记录计价引擎每一步的执行详情（规则匹配、公式计算、数量校验等）</item>
///   <item><b>PR_CHARGE_DISCOUNT_DETAIL</b> — 折价结果明细，记录最终的折价金额、原始金额、折扣率等</item>
/// </list>
/// </para>
/// <para>
/// 可追溯性是架构级原则：每笔折价必须可沿上述三条链路完整追溯，
/// 支撑问题排查、审计合规和资金对账。
/// </para>
/// </summary>
[ApiController]
[Route("api/pricing/trace")]
public sealed class TraceController : ControllerBase
{
    /// <summary>
    /// 计价追溯查询服务实例，封装请求日志、执行步骤和折价明细的查询业务逻辑。
    /// </summary>
    private readonly TraceQueryAppService _service;

    /// <summary>
    /// 构造函数，通过依赖注入获取计价追溯查询服务。
    /// </summary>
    /// <param name="service">计价追溯查询服务（<see cref="TraceQueryAppService"/>）。</param>
    public TraceController(TraceQueryAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 【追溯查询接口】— 按条件分页查询计价追溯记录。
    /// <para>
    /// HTTP 方法：POST  |  路由：<c>/api/pricing/trace/query</c>
    /// </para>
    /// <para>
    /// 用途：
    /// <list type="bullet">
    ///   <item>运营人员排查计价异常（如折价金额不符预期）</item>
    ///   <item>审计人员追溯某笔收费的规则依据和计算过程</item>
    ///   <item>财务人员对账时核对折价明细</item>
    ///   <item>开发人员调试计价引擎的执行路径</item>
    /// </list>
    /// </para>
    /// <para>
    /// 查询结果包含请求日志摘要和关联的计算步骤明细，支持按项目编码、
    /// 时间范围、请求状态、来源系统等条件筛选。
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 追溯查询请求（<see cref="TraceQueryRequest"/>），包含：
    /// <list type="bullet">
    ///   <item><c>ItemCode</c> — 收费项目编码筛选（可选）</item>
    ///   <item><c>StartTime</c> — 查询起始时间（可选）</item>
    ///   <item><c>EndTime</c> — 查询结束时间（可选）</item>
    ///   <item><c>RequestStatus</c> — 请求状态筛选（可选，如 CONFIRM_PENDING/CONFIRMED/CANCELLED 等）</item>
    ///   <item><c>SourceSystem</c> — 来源系统筛选（可选）</item>
    ///   <item><c>BusinessRequestNo</c> — 业务请求号精确查询（可选）</item>
    ///   <item><c>PageNo</c> — 页码（从 1 开始）</item>
    ///   <item><c>PageSize</c> — 每页条数</item>
    /// </list>
    /// </param>
    /// <returns>
    /// 追溯查询分页结果（<see cref="PagedResponse{TraceQueryResponse}"/>），
    /// 每条记录包含请求摘要（项目、金额、状态、时间）和关联的计算步骤列表。
    /// </returns>
    [HttpPost("query")]
    public async Task<ApiResult<PagedResponse<TraceQueryResponse>>> QueryAsync(
        [FromBody] TraceQueryRequest request)
    {
        var result = await _service.QueryAsync(request);
        return ApiResult<PagedResponse<TraceQueryResponse>>.Ok(result);
    }
}
