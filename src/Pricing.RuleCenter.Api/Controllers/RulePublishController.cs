using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Application.Pricing;
using Pricing.RuleCenter.Api.Application.Rules;
using Pricing.RuleCenter.Api.Application.Catalog;
using Pricing.RuleCenter.Api.Application.Trace;
using Pricing.RuleCenter.Api.Application.Background;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【规则发布控制器】
/// <para>
/// 职责范围：规则版本的发布、停用、回滚操作，以及发布历史和变更日志的查询。
/// </para>
/// <para>
/// 路由前缀：<c>api/pricing/rules/{ruleId:long}</c>
/// </para>
/// <para>
/// 规则生命周期状态机：
/// <code>
/// [草稿 DRAFT] ──发布──→ [已发布 PUBLISHED] ──停用──→ [已停用 DISABLED]
///       ↑                      │                        │
///       │                      └──回滚──→ [已发布]       │
///       │                                               │
///       └──────────────── 重新编辑/新版本 ───────────────┘
/// </code>
/// </para>
/// <para>
/// 发布前阻断校验（由 <see cref="RulePublishAppService"/> 执行）：
/// <list type="bullet">
///   <item>规则重叠校验：同一项目、同一场景、同一生效期不允许重复规则</item>
///   <item>动作组冲突校验：同一规则不允许同时存在冲突的动作配置</item>
///   <item>重复子项目校验：子项加收不允许重复配置</item>
///   <item>缺少测试用例校验：建议发布前至少有一条测试用例</item>
/// </list>
/// </para>
/// <para>
/// 缓存失效：规则发布、停用、回滚操作完成后必须立即失效计价引擎的规则缓存，
/// 确保下次计价请求使用最新规则配置。
/// </para>
/// </summary>
/// <remarks>
/// 发布相关接口会改变规则是否参与计价匹配，控制器只做路由转发，
/// 状态机约束和缓存失效由 <see cref="RulePublishAppService"/> 统一执行。
/// </remarks>
[ApiController]
[Route("api/pricing/rules/{ruleId:long}")]
public sealed class RulePublishController : ControllerBase
{
    /// <summary>
    /// 规则发布应用服务实例，封装发布/停用/回滚的状态机逻辑和缓存失效处理。
    /// </summary>
    private readonly RulePublishAppService _service;

    /// <summary>
    /// 构造函数，通过依赖注入获取规则发布应用服务。
    /// </summary>
    /// <param name="service">规则发布应用服务（<see cref="RulePublishAppService"/>）。</param>
    public RulePublishController(RulePublishAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 【查询发布历史】— 获取指定规则的所有发布、停用和回滚操作流水。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/publish-history</c>
    /// </para>
    /// <para>
    /// 用途：审计追溯场景，查看规则的完整发布生命周期记录，
    /// 包含每次操作的时间、操作人、操作类型和关联版本号。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <returns>
    /// 发布历史列表（<see cref="RulePublishResponse"/>），按操作时间倒序排列，
    /// 包含发布/停用/回滚的操作时间、操作人、操作原因等。
    /// </returns>
    [HttpGet("publish-history")]
    public async Task<ApiResponse<IReadOnlyList<RulePublishResponse>>> GetPublishHistoryAsync(
        long ruleId)
    {
        var items = await _service.GetPublishHistoryAsync(ruleId);
        return ApiResponse<IReadOnlyList<RulePublishResponse>>.Ok(items);
    }

    /// <summary>
    /// 【查询变更日志】— 获取指定规则的所有配置变更记录。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/change-logs</c>
    /// </para>
    /// <para>
    /// 用途：审计追溯场景，查看规则条件、动作、版本等配置的变更历史，
    /// 支持"规则变更链"追溯——即某条计价结果是由哪个版本的规则计算得出的。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <returns>
    /// 变更日志列表（<see cref="RuleChangeLogResponse"/>），包含变更时间、变更类型、
    /// 变更前后的值等详细信息。
    /// </returns>
    [HttpGet("change-logs")]
    public async Task<ApiResponse<IReadOnlyList<RuleChangeLogResponse>>> GetChangeLogsAsync(
        long ruleId)
    {
        var items = await _service.GetChangeLogsAsync(ruleId);
        return ApiResponse<IReadOnlyList<RuleChangeLogResponse>>.Ok(items);
    }

    /// <summary>
    /// 【发布规则】— 将草稿版本发布为生效版本，使其参与计价匹配。
    /// <para>
    /// HTTP 方法：POST &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/publish</c>
    /// </para>
    /// <para>
    /// 用途：规则配置完成后，管理员审核通过后执行发布操作。
    /// 发布后规则立即（或按配置的生效时间）参与计价匹配。
    /// </para>
    /// <para>
    /// 发布流程：
    /// <list type="number">
    ///   <item>执行发布前阻断校验（规则重叠、动作组冲突、重复子项目、缺少测试用例等）</item>
    ///   <item>将规则版本状态从 DRAFT 变更为 PUBLISHED</item>
    ///   <item>写入 PR_RULE_PUBLISH 发布记录</item>
    ///   <item>写入 PR_RULE_CHANGE_LOG 变更日志</item>
    ///   <item>立即失效计价引擎的规则缓存</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <param name="request">
    /// 发布请求（<see cref="RulePublishRequest"/>），包含：
    /// <list type="bullet">
    ///   <item><c>VersionNo</c> — 要发布的版本号（必填）</item>
    ///   <item><c>PublishNote</c> — 发布说明（可选，用于审计记录）</item>
    /// </list>
    /// </param>
    /// <returns>统一成功响应。</returns>
    [HttpPost("publish")]
    public async Task<ApiResponse> PublishAsync(
        long ruleId, [FromBody] RulePublishRequest request)
    {
        await _service.PublishAsync(ruleId, request);
        return ApiResponse.Ok();
    }

    /// <summary>
    /// 【停用规则】— 将已发布规则设为停用状态，使其不再参与计价匹配。
    /// <para>
    /// HTTP 方法：POST &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/disable</c>
    /// </para>
    /// <para>
    /// 用途：规则过期、业务调整等场景下停用规则。
    /// 停用后立即失效计价引擎的规则缓存，确保不会继续使用已停用规则进行计价。
    /// </para>
    /// <para>
    /// 状态变更：PR_RULE_VERSION.Status: PUBLISHED → DISABLED。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <param name="request">
    /// 停用请求（<see cref="RuleDisableRequest"/>），包含：
    /// <list type="bullet">
    ///   <item><c>DisableReason</c> — 停用原因（建议填写，用于审计追溯）</item>
    /// </list>
    /// </param>
    /// <returns>统一成功响应。</returns>
    [HttpPost("disable")]
    public async Task<ApiResponse> DisableAsync(
        long ruleId, [FromBody] RuleDisableRequest request)
    {
        await _service.DisableAsync(ruleId, request);
        return ApiResponse.Ok();
    }

    /// <summary>
    /// 【回滚规则】— 将规则回滚到最近一个历史发布版本。
    /// <para>
    /// HTTP 方法：POST &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/rollback</c>
    /// </para>
    /// <para>
    /// 用途：发现当前发布版本存在问题（如计价异常）时，快速回退到上一个已验证的版本。
    /// 回滚操作等同于重新发布历史版本，会触发缓存失效。
    /// </para>
    /// <para>
    /// 注意：回滚只能回到"最近一个历史发布版本"，不支持回滚到任意历史版本。
    /// 如需回到更早版本，应创建新版本并重新配置后发布。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <param name="request">
    /// 回滚请求（<see cref="RuleRollbackRequest"/>），包含：
    /// <list type="bullet">
    ///   <item><c>RollbackReason</c> — 回滚原因（建议填写，用于审计追溯）</item>
    /// </list>
    /// </param>
    /// <returns>统一成功响应。</returns>
    [HttpPost("rollback")]
    public async Task<ApiResponse> RollbackAsync(
        long ruleId, [FromBody] RuleRollbackRequest request)
    {
        await _service.RollbackAsync(ruleId, request);
        return ApiResponse.Ok();
    }
}


