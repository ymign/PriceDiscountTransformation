using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Catalog;
using Pricing.RuleCenter.Application.Trace;
using Pricing.RuleCenter.Application.Background;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【规则版本控制器】
/// <para>
/// 职责范围：规则版本（PR_RULE_VERSION 表）的查询和草稿版本创建，
/// 管理同一规则在不同时间点的配置快照。
/// </para>
/// <para>
/// 路由前缀：<c>api/pricing/rules/{ruleId:long}/versions</c>
/// </para>
/// <para>
/// 版本管理模型：
/// <list type="bullet">
///   <item>每个规则可有多个版本，同一时间只有一个版本处于 PUBLISHED 状态</item>
///   <item>版本号（versionNo）从 1 开始递增</item>
///   <item>版本状态：DRAFT（草稿）→ PUBLISHED（已发布）→ DISABLED（已停用）</item>
///   <item>新版本基于当前已发布版本的配置创建，编辑后发布</item>
///   <item>已发布版本的条件和动作不允许直接修改，必须创建新版本</item>
/// </list>
/// </para>
/// <para>
/// 与规则发布的关系：
/// <list type="number">
///   <item>在本控制器创建草稿版本</item>
///   <item>在条件/动作控制器配置草稿版本的条件和动作</item>
///   <item>在发布控制器发布草稿版本</item>
///   <item>发布后如有问题，在发布控制器执行停用或回滚</item>
/// </list>
/// </para>
/// </summary>
[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions")]
public sealed class RuleVersionController : ControllerBase
{
    /// <summary>
    /// 规则版本应用服务实例，封装版本查询和草稿创建的业务逻辑。
    /// </summary>
    private readonly RuleVersionAppService _service;

    /// <summary>
    /// 构造函数，通过依赖注入获取规则版本应用服务。
    /// </summary>
    /// <param name="service">规则版本应用服务（<see cref="RuleVersionAppService"/>）。</param>
    public RuleVersionController(RuleVersionAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 【查询规则全部版本】— 获取指定规则的所有版本列表。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/versions</c>
    /// </para>
    /// <para>
    /// 用途：规则详情页面展示版本历史时调用，显示每个版本的状态（草稿/已发布/已停用）、
    /// 创建时间、发布时间等信息，便于用户选择版本进行查看或编辑。
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <returns>
    /// 规则版本列表（<see cref="RuleVersionResponse"/>），按版本号升序排列，
    /// 每个版本包含版本号、状态、创建时间、发布时间、操作人等。
    /// </returns>
    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<RuleVersionResponse>>> GetByRuleIdAsync(
        long ruleId)
    {
        var items = await _service.GetByRuleIdAsync(ruleId);
        return ApiResponse<IReadOnlyList<RuleVersionResponse>>.Ok(items);
    }

    /// <summary>
    /// 【按版本主键查询版本详情】— 查询单个版本的详细信息。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/versions/{versionId}</c>
    /// </para>
    /// <para>
    /// 用途：版本详情页面加载特定版本的完整信息时调用。
    /// </para>
    /// </summary>
    /// <param name="versionId">版本主键（路径参数）。</param>
    /// <returns>规则版本详情（<see cref="RuleVersionResponse"/>）。不存在时返回 404 统一响应。</returns>
    [HttpGet("{versionId:long}")]
    public async Task<ActionResult<ApiResponse<RuleVersionResponse>>> GetByIdAsync(long versionId)
    {
        var item = await _service.GetByIdAsync(versionId);
        if (item is null)
        {
            return NotFound(ApiResponse.Fail(404, $"规则版本不存在: {versionId}"));
        }

        return ApiResponse<RuleVersionResponse>.Ok(item);
    }

    /// <summary>
    /// 【创建草稿版本】— 基于当前已发布版本创建下一个草稿版本。
    /// <para>
    /// HTTP 方法：POST &nbsp;|&nbsp; 路由：<c>/api/pricing/rules/{ruleId}/versions</c>
    /// </para>
    /// <para>
    /// 用途：需要修改已发布规则的配置时，先创建草稿版本，再在草稿版本上编辑条件和动作，
    /// 编辑完成后通过发布接口发布新版本。
    /// </para>
    /// <para>
    /// 创建逻辑：
    /// <list type="number">
    ///   <item>检查是否存在未发布的草稿版本（同一时间只能有一个草稿版本）</item>
    ///   <item>基于当前已发布版本的条件和动作复制配置到新版本</item>
    ///   <item>新版本号 = 当前最大版本号 + 1</item>
    ///   <item>新版本状态为 DRAFT</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="ruleId">规则主键（路径参数）。</param>
    /// <returns>新增草稿版本的主键 ID。</returns>
    [HttpPost]
    public async Task<ApiResponse<long>> CreateDraftAsync(long ruleId)
    {
        var id = await _service.CreateDraftAsync(ruleId);
        return ApiResponse<long>.Ok(id);
    }
}




