using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Application.Pricing;
using Pricing.RuleCenter.Api.Application.Rules;
using Pricing.RuleCenter.Api.Application.Catalog;
using Pricing.RuleCenter.Api.Application.Trace;
using Pricing.RuleCenter.Api.Application.Background;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【规则主档控制器】
/// <para>
/// 职责范围：规则基础信息（PR_RULE_HEADER 表）的增删改查，
/// 是规则维护工作台的主入口，管理规则的生命周期元数据。
/// </para>
/// <para>
/// 路由前缀：<c>api/pricing/rules</c>
/// </para>
/// <para>
/// 规则数据模型概述（条件-动作分离模式）：
/// <list type="bullet">
///   <item><b>PR_RULE_HEADER</b>（本控制器管理）— 规则主档，包含规则名称、适用项目、场景等基础信息</item>
///   <item><b>PR_RULE_VERSION</b> — 规则版本，同一规则可有多个草稿/发布版本</item>
///   <item><b>PR_RULE_CONDITION</b> — 规则条件，定义"何时触发"（如项目编码、部位、时间段等）</item>
///   <item><b>PR_RULE_ACTION</b> — 规则动作，定义"触发后做什么"（如折价公式、金额上下限、数量限制等）</item>
/// </list>
/// </para>
/// <para>
/// 规则冲突校验（发布时执行）：
/// <list type="bullet">
///   <item>同一项目、同一场景、同一生效期内不允许多套折价公式同时生效</item>
///   <item>同一项目、同一场景、同一生效期内不允许多套金额上下限或额度规则同时生效</item>
///   <item>同一项目允许按不同部位维护不同换算规则</item>
/// </list>
/// </para>
/// </summary>
[ApiController]
[Route("api/pricing/rules")]
public sealed class RuleHeaderController : ControllerBase
{
    /// <summary>
    /// 规则主档应用服务实例，封装规则主档数据的 CRUD 业务逻辑。
    /// </summary>
    private readonly RuleHeaderAppService _service;

    /// <summary>
    /// 构造函数，通过依赖注入获取规则主档应用服务。
    /// </summary>
    /// <param name="service">规则主档应用服务（<see cref="RuleHeaderAppService"/>）。</param>
    public RuleHeaderController(RuleHeaderAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 【分页查询规则主档】— 按条件分页查询规则列表。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules?pageNo=1&amp;pageSize=20&amp;...</c>
    /// </para>
    /// <para>
    /// 用途：规则维护工作台的规则列表页面，支持按规则名称、项目编码、规则状态等条件筛选。
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 分页查询请求（<see cref="RuleHeaderPagedRequest"/>），包含：
    /// <list type="bullet">
    ///   <item><c>PageNo</c> — 页码（从 1 开始）</item>
    ///   <item><c>PageSize</c> — 每页条数</item>
    ///   <item><c>RuleName</c> — 规则名称模糊搜索（可选）</item>
    ///   <item><c>ItemCode</c> — 适用项目编码筛选（可选）</item>
    ///   <item><c>Status</c> — 规则状态筛选（可选）</item>
    /// </list>
    /// </param>
    /// <returns>规则主档分页结果（<see cref="PagedResponse{RuleHeaderResponse}"/>）。</returns>
    [HttpGet]
    public async Task<ApiResponse<PagedResponse<RuleHeaderResponse>>> GetPagedAsync(
        [FromQuery] RuleHeaderPagedRequest request)
    {
        var result = await _service.GetPagedAsync(request);
        return ApiResponse<PagedResponse<RuleHeaderResponse>>.Ok(result);
    }

    /// <summary>
    /// 【按主键查询规则主档】— 查询单个规则的详细信息。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}</c>
    /// </para>
    /// <para>
    /// 用途：规则详情页面加载基础信息时调用，后续还需调用版本、条件、动作接口获取完整配置。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <returns>规则主档详情（<see cref="RuleHeaderResponse"/>）。不存在时返回 404 统一响应。</returns>
    [HttpGet("{ruleId:long}")]
    public async Task<ActionResult<ApiResponse<RuleHeaderResponse>>> GetByIdAsync(long ruleId)
    {
        var item = await _service.GetByIdAsync(ruleId);
        if (item is null)
        {
            return NotFound(ApiResponse.Fail(404, $"规则不存在: {ruleId}"));
        }

        return ApiResponse<RuleHeaderResponse>.Ok(item);
    }

    /// <summary>
    /// 【按项目编码查询关联规则】— 查询与指定收费项目关联的所有规则。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/by-item/{itemCode}</c>
    /// </para>
    /// <para>
    /// 用途：查看某个收费项目已配置了哪些规则，辅助规则冲突排查和配置检查。
    /// </para>
    /// </summary>
    /// <param name="itemCode">HIS 收费项目编码（路径参数）。</param>
    /// <returns>与该项目关联的规则主档列表。</returns>
    [HttpGet("by-item/{itemCode}")]
    public async Task<ApiResponse<IReadOnlyList<RuleHeaderResponse>>> GetByItemCodeAsync(
        string itemCode)
    {
        var items = await _service.GetByItemCodeAsync(itemCode);
        return ApiResponse<IReadOnlyList<RuleHeaderResponse>>.Ok(items);
    }

    /// <summary>
    /// 【查询当前生效规则】— 查询当前时间点所有已发布且在有效期内的规则。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/effective?itemCode=xxx</c>
    /// </para>
    /// <para>
    /// 用途：
    /// <list type="bullet">
    ///   <item>计价引擎预览：查看当前哪些规则参与匹配</item>
    ///   <item>规则配置验证：确认规则发布后是否正确出现在生效列表中</item>
    ///   <item>运维排查：快速定位某项目的生效规则集合</item>
    /// </list>
    /// </para>
    /// <para>
    /// 缓存说明：结果使用 IMemoryCache 缓存 5 分钟，规则发布/停用/回滚时主动清除。
    /// </para>
    /// </summary>
    /// <param name="itemCode">
    /// 项目编码（选填查询参数）。为空时返回所有生效规则；非空时按项目编码过滤。
    /// </param>
    /// <param name="chargeTime">
    /// 业务收费发生时间（选填查询参数）。为空时使用当前时间。
    /// </param>
    /// <returns>当前时间点生效的规则主档列表，按优先级升序排列。</returns>
    [HttpGet("effective")]
    public async Task<ApiResponse<IReadOnlyList<RuleHeaderResponse>>> GetEffectiveRules(
        [FromQuery] string? itemCode = null,
        [FromQuery] DateTime? chargeTime = null)
    {
        var result = await _service.GetEffectiveAsync(itemCode, chargeTime);
        return ApiResponse<IReadOnlyList<RuleHeaderResponse>>.Ok(result);
    }

    /// <summary>
    /// 【创建规则主档】— 新建一条规则的基础信息。
    /// <para>
    /// HTTP 方法：POST &nbsp;|&nbsp; 路由：<c>/api/pricing/rules</c>
    /// </para>
    /// <para>
    /// 用途：规则维护工作台新建规则时调用。创建后自动生成第一个草稿版本（versionNo = 1），
    /// 后续需分别调用条件和动作接口配置规则细节。
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 规则新增请求（<see cref="RuleHeaderCreateRequest"/>），包含：
    /// <list type="bullet">
    ///   <item><c>RuleName</c> — 规则名称（必填）</item>
    ///   <item><c>ItemCode</c> — 适用收费项目编码（必填）</item>
    ///   <item><c>SceneType</c> — 适用场景类型（门诊/住院/手术等）</item>
    ///   <item><c>Description</c> — 规则说明（可选）</item>
    /// </list>
    /// </param>
    /// <returns>新增规则的主键 ID。</returns>
    [HttpPost]
    public async Task<ApiResponse<long>> CreateAsync(
        [FromBody] RuleHeaderCreateRequest request)
    {
        var id = await _service.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    /// <summary>
    /// 【更新规则主档】— 修改规则的基础信息。
    /// <para>
    /// HTTP 方法：PUT &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}</c>
    /// </para>
    /// <para>
    /// 注意：仅允许修改基础信息（名称、说明等），适用项目编码等关键字段通常不允许变更。
    /// 若规则已有发布版本，修改基础信息可能需要创建新版本。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <param name="request">规则更新请求（<see cref="RuleHeaderUpdateRequest"/>）。</param>
    /// <returns>统一成功响应。</returns>
    [HttpPut("{ruleId:long}")]
    public async Task<ApiResponse> UpdateAsync(
        long ruleId, [FromBody] RuleHeaderUpdateRequest request)
    {
        await _service.UpdateAsync(ruleId, request);
        return ApiResponse.Ok();
    }
}


