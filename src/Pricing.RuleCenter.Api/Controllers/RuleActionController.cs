using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Catalog;
using Pricing.RuleCenter.Application.Trace;
using Pricing.RuleCenter.Application.Background;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【规则动作控制器】
/// <para>
/// 职责范围：管理指定规则版本下的动作链（PR_RULE_ACTION 表），
/// 定义规则触发后"做什么"（如执行折价公式、设置金额上下限、配置数量限制等）。
/// </para>
/// <para>
/// 路由前缀：<c>api/pricing/rules/{ruleId:long}/versions/{versionNo:int}/actions</c>
/// </para>
/// <para>
/// 条件-动作分离模型中的"动作"侧：
/// <list type="bullet">
///   <item>规则条件（PR_RULE_CONDITION）定义"何时触发"</item>
///   <item><b>规则动作（PR_RULE_ACTION）定义"触发后做什么"（本控制器管理）</b></item>
/// </list>
/// </para>
/// <para>
/// 动作类型包括但不限于：
/// <list type="bullet">
///   <item><b>折价公式</b> — 关联公式定义（PR_FORMULA_DEF），按公式计算折价金额</item>
///   <item><b>金额下限</b> — 计算结果不低于此值（NULL 表示不校验，0 表示限制为零）</item>
///   <item><b>金额上限</b> — 计算结果不高于此值</item>
///   <item><b>日数量限制</b> — 全院累计单日收费数量上限</item>
///   <item><b>时间窗数量限制</b> — 如 2 小时窗口内的收费数量上限</item>
///   <item><b>同组互斥</b> — 同一互斥组内不允许重复收费</item>
///   <item><b>同手术封顶</b> — 同一台手术中该收费项目的总金额上限</item>
///   <item><b>子项加收</b> — 主项目触发时自动加收子项目</item>
/// </list>
/// </para>
/// <para>
/// 关键约束：
/// <list type="bullet">
///   <item>同一项目不允许维护不同折价公式或不同折价额度规则（冲突校验在发布时执行）。</item>
///   <item>公式项目是否执行数量、时间窗、同组、同手术限制，以动作配置为准；
///         禁止代码层一见公式就跳过全部数量类限制。</item>
///   <item>NULL ≠ 0：NULL 表示"不校验"，0 表示"限制为零"。前端空值必须存为 NULL。</item>
/// </list>
/// </para>
/// </summary>
[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions/{versionNo:int}/actions")]
public sealed class RuleActionController : ControllerBase
{
    /// <summary>
    /// 规则动作应用服务实例，封装动作链的查询和保存业务逻辑。
    /// </summary>
    private readonly RuleActionAppService _service;

    /// <summary>
    /// 构造函数，通过依赖注入获取规则动作应用服务。
    /// </summary>
    /// <param name="service">规则动作应用服务（<see cref="RuleActionAppService"/>）。</param>
    public RuleActionController(RuleActionAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 【查询动作链】— 获取指定规则版本下的所有动作配置。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/versions/{versionNo}/actions</c>
    /// </para>
    /// <para>
    /// 用途：规则维护工作台的动作配置页面加载已有动作时调用。
    /// 动作按执行顺序排列，计价引擎按此顺序依次执行。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数），标识所属规则。</param>
    /// <param name="versionNo">
    /// 规则版本号（路径参数），标识所属版本。
    /// 仅草稿版本允许编辑动作，已发布版本的动作为只读。
    /// </param>
    /// <returns>
    /// 动作列表（<see cref="RuleActionResponse"/>），每个动作包含动作类型、参数配置、执行顺序等。
    /// </returns>
    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<RuleActionResponse>>> GetAsync(
        long ruleId, int versionNo)
    {
        var items = await _service.GetAsync(ruleId, versionNo);
        return ApiResponse<IReadOnlyList<RuleActionResponse>>.Ok(items);
    }

    /// <summary>
    /// 【整体保存动作链】— 替换指定草稿版本的全部动作配置。
    /// <para>
    /// HTTP 方法：PUT &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/versions/{versionNo}/actions</c>
    /// </para>
    /// <para>
    /// 用途：规则维护工作台的动作配置页面保存时调用。
    /// 采用整体替换策略（先删后插），确保动作链的完整性和顺序一致性。
    /// </para>
    /// <para>
    /// 前置条件：目标版本必须为草稿状态（DRAFT），已发布版本不允许直接修改动作。
    /// 如需修改已发布规则的动作，应创建新版本并在新版本上编辑。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <param name="versionNo">规则版本号（路径参数），必须为草稿版本。</param>
    /// <param name="request">
    /// 动作保存请求（<see cref="RuleActionSaveRequest"/>），包含完整的动作列表，
    /// 每个动作包含：
    /// <list type="bullet">
    ///   <item><c>ActionType</c> — 动作类型（折价公式/金额上限/日数量限制等）</item>
    ///   <item><c>ActionParams</c> — 动作参数（JSON 格式，具体结构因动作类型而异）</item>
    ///   <item><c>ExecuteOrder</c> — 执行顺序号</item>
    /// </list>
    /// </param>
    /// <returns>统一成功响应。</returns>
    [HttpPut]
    public async Task<ApiResponse> SaveAsync(
        long ruleId, int versionNo, [FromBody] RuleActionSaveRequest request)
    {
        await _service.SaveAsync(ruleId, versionNo, request);
        return ApiResponse.Ok();
    }
}




