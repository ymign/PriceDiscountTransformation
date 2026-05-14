using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Application.Pricing;
using Pricing.RuleCenter.Api.Application.Rules;
using Pricing.RuleCenter.Api.Application.Catalog;
using Pricing.RuleCenter.Api.Application.Trace;
using Pricing.RuleCenter.Api.Application.Background;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【规则条件控制器】
/// <para>
/// 职责范围：管理指定规则版本下的条件集合（PR_RULE_CONDITION 表），
/// 定义规则"何时触发"（如指定项目编码、适用场景、部位、时间段等）。
/// </para>
/// <para>
/// 路由前缀：<c>api/pricing/rules/{ruleId:long}/versions/{versionNo:int}/conditions</c>
/// </para>
/// <para>
/// 条件-动作分离模型中的"条件"侧：
/// <list type="bullet">
///   <item><b>规则条件（PR_RULE_CONDITION）定义"何时触发"（本控制器管理）</b></item>
///   <item>规则动作（PR_RULE_ACTION）定义"触发后做什么"</item>
/// </list>
/// </para>
/// <para>
/// 条件类型包括但不限于：
/// <list type="bullet">
///   <item><b>项目编码</b> — 匹配 HIS 收费项目编码（必填条件）</item>
///   <item><b>适用场景</b> — 门诊/住院/手术/医技等场景</item>
///   <item><b>部位编码</b> — 匹配收费部位（支持多部位项目）</item>
///   <item><b>时间段</b> — 规则生效的时间范围</item>
///   <item><b>收费途径</b> — HIS/自助机/微信等渠道</item>
/// </list>
/// </para>
/// <para>
/// 多规则边界说明：
/// <list type="bullet">
///   <item>同一项目允许按不同部位维护不同换算规则（条件中部位不同即可）</item>
///   <item>同一项目、同一场景、同一生效期内不允许多套折价公式或额度规则</item>
///   <item>多肿物、多部位、多面积项目必须使用 pricingParts 明细表达，不能压成单个数量粗算</item>
/// </list>
/// </para>
/// </summary>
[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions/{versionNo:int}/conditions")]
public sealed class RuleConditionController : ControllerBase
{
    /// <summary>
    /// 规则条件应用服务实例，封装条件集合的查询和保存业务逻辑。
    /// </summary>
    private readonly RuleConditionAppService _service;

    /// <summary>
    /// 构造函数，通过依赖注入获取规则条件应用服务。
    /// </summary>
    /// <param name="service">规则条件应用服务（<see cref="RuleConditionAppService"/>）。</param>
    public RuleConditionController(RuleConditionAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 【查询条件集合】— 获取指定规则版本下的所有条件配置。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/versions/{versionNo}/conditions</c>
    /// </para>
    /// <para>
    /// 用途：规则维护工作台的条件配置页面加载已有条件时调用。
    /// 条件之间为"且"关系（AND），所有条件同时满足时规则才触发。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数），标识所属规则。</param>
    /// <param name="versionNo">
    /// 规则版本号（路径参数），标识所属版本。
    /// 仅草稿版本允许编辑条件，已发布版本的条件为只读。
    /// </param>
    /// <returns>
    /// 条件列表（<see cref="RuleConditionResponse"/>），每个条件包含条件类型、匹配值、比较运算符等。
    /// </returns>
    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<RuleConditionResponse>>> GetAsync(
        long ruleId, int versionNo)
    {
        var items = await _service.GetAsync(ruleId, versionNo);
        return ApiResponse<IReadOnlyList<RuleConditionResponse>>.Ok(items);
    }

    /// <summary>
    /// 【整体保存条件集合】— 替换指定草稿版本的全部条件配置。
    /// <para>
    /// HTTP 方法：PUT &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/versions/{versionNo}/conditions</c>
    /// </para>
    /// <para>
    /// 用途：规则维护工作台的条件配置页面保存时调用。
    /// 采用整体替换策略（先删后插），确保条件集合的完整性。
    /// </para>
    /// <para>
    /// 前置条件：目标版本必须为草稿状态（DRAFT），已发布版本不允许直接修改条件。
    /// 如需修改已发布规则的条件，应创建新版本并在新版本上编辑。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <param name="versionNo">规则版本号（路径参数），必须为草稿版本。</param>
    /// <param name="request">
    /// 条件保存请求（<see cref="RuleConditionSaveRequest"/>），包含完整的条件列表，
    /// 每个条件包含：
    /// <list type="bullet">
    ///   <item><c>ConditionType</c> — 条件类型（项目编码/场景/部位/时间段等）</item>
    ///   <item><c>MatchValue</c> — 匹配值（支持精确匹配和模糊匹配）</item>
    ///   <item><c>Operator</c> — 比较运算符（等于/包含/范围等）</item>
    /// </list>
    /// </param>
    /// <returns>统一成功响应。</returns>
    [HttpPut]
    public async Task<ApiResponse> SaveAsync(
        long ruleId, int versionNo, [FromBody] RuleConditionSaveRequest request)
    {
        await _service.SaveAsync(ruleId, versionNo, request);
        return ApiResponse.Ok();
    }
}


