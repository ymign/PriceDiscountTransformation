using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Application.Pricing;
using Pricing.RuleCenter.Api.Application.Rules;
using Pricing.RuleCenter.Api.Application.Catalog;
using Pricing.RuleCenter.Api.Application.Trace;
using Pricing.RuleCenter.Api.Application.Background;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 【公式定义控制器】
/// <para>
/// 职责范围：计价公式元数据的增删改查，用于管理计价引擎中可复用的公式模板。
/// </para>
/// <para>
/// 路由前缀：<c>api/pricing/formulas</c>
/// </para>
/// <para>
/// 公式在计价流程中的位置：
/// <list type="number">
///   <item>规则匹配成功后，检查规则动作（PR_RULE_ACTION）是否配置了公式</item>
///   <item>若配置了公式，从 PR_FORMULA_DEF 表读取公式定义（本控制器管理的数据）</item>
///   <item>计价引擎使用换算后数量（非输入数量）执行公式计算</item>
///   <item>公式计算结果再与金额上下限比较（公式优先于限制，先算公式再与限制比较）</item>
/// </list>
/// </para>
/// <para>
/// 关键业务约束：
/// <list type="bullet">
///   <item>同一项目不允许维护不同折价公式（冲突校验在发布时执行）。</item>
///   <item>公式项目是否执行数量、时间窗、同组、同手术限制，以规则动作配置为准；
///         禁止代码层一见公式就跳过全部数量类限制。</item>
///   <item>换算数量固定为 1，公式使用换算后数量。</item>
///   <item>中间计算保留全部精度，最终金额保留 2 位小数、四舍五入。</item>
/// </list>
/// </para>
/// </summary>
[ApiController]
[Route("api/pricing/formulas")]
public sealed class FormulaDefController : ControllerBase
{
    /// <summary>
    /// 公式定义应用服务实例，封装公式元数据的 CRUD 业务逻辑。
    /// </summary>
    private readonly FormulaDefAppService _service;

    /// <summary>
    /// 构造函数，通过依赖注入获取公式定义应用服务。
    /// </summary>
    /// <param name="service">公式定义应用服务（<see cref="FormulaDefAppService"/>）。</param>
    public FormulaDefController(FormulaDefAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 【查询全部公式定义】— 获取系统中所有公式定义列表。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/formulas</c>
    /// </para>
    /// <para>
    /// 用途：规则维护工作台配置规则动作时，展示可选的公式列表供用户选择。
    /// 返回所有公式（包括停用的），前端根据启用状态区分显示。
    /// </para>
    /// </summary>
    /// <returns>
    /// 公式定义列表（<see cref="FormulaDefResponse"/>），每个公式包含：
    /// 公式 ID、公式名称、公式表达式、公式类型、启用状态等。
    /// </returns>
    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<FormulaDefResponse>>> GetAllAsync()
    {
        var items = await _service.GetAllAsync();
        return ApiResponse<IReadOnlyList<FormulaDefResponse>>.Ok(items);
    }

    /// <summary>
    /// 【按主键查询公式定义】— 查询单个公式的详细信息。
    /// <para>
    /// HTTP 方法：GET &nbsp;|&nbsp; 路由：<c>/api/pricing/formulas/{formulaId}</c>
    /// </para>
    /// <para>
    /// 用途：公式编辑页面加载详情、或规则动作配置页展示公式的完整表达式时调用。
    /// </para>
    /// </summary>
    /// <param name="formulaId">公式定义主键（路径参数）。</param>
    /// <returns>公式定义详情（<see cref="FormulaDefResponse"/>）。</returns>
    /// <exception cref="KeyNotFoundException">当指定 formulaId 的公式不存在时抛出。</exception>
    [HttpGet("{formulaId:long}")]
    public async Task<ApiResponse<FormulaDefResponse>> GetByIdAsync(long formulaId)
    {
        var item = await _service.GetByIdAsync(formulaId)
            ?? throw new KeyNotFoundException($"公式定义不存在: {formulaId}");
        return ApiResponse<FormulaDefResponse>.Ok(item);
    }

    /// <summary>
    /// 【新增公式定义】— 创建一个新的计价公式模板。
    /// <para>
    /// HTTP 方法：POST &nbsp;|&nbsp; 路由：<c>/api/pricing/formulas</c>
    /// </para>
    /// <para>
    /// 用途：管理员在规则维护工作台定义新公式时调用。
    /// 公式表达式支持变量引用（如数量、单价、换算系数等），具体语法由计价引擎定义。
    /// </para>
    /// </summary>
    /// <param name="request">
    /// 公式新增请求（<see cref="FormulaDefCreateRequest"/>），包含：
    /// <list type="bullet">
    ///   <item><c>FormulaName</c> — 公式名称（必填）</item>
    ///   <item><c>FormulaExpr</c> — 公式表达式（必填，如 "unitPrice * quantity * 0.8"）</item>
    ///   <item><c>FormulaType</c> — 公式类型（必填，如固定折扣、阶梯折扣等）</item>
    ///   <item><c>Description</c> — 公式说明（可选）</item>
    /// </list>
    /// </param>
    /// <returns>新增公式的主键 ID。</returns>
    [HttpPost]
    public async Task<ApiResponse<long>> CreateAsync(
        [FromBody] FormulaDefCreateRequest request)
    {
        var id = await _service.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    /// <summary>
    /// 【更新公式定义】— 修改已有公式的名称、表达式、类型等信息。
    /// <para>
    /// HTTP 方法：PUT &nbsp;|&nbsp; 路由：<c>/api/pricing/formulas/{formulaId}</c>
    /// </para>
    /// <para>
    /// 注意：若该公式已被已发布的规则引用，修改公式可能影响线上计价结果。
    /// 建议先停用旧公式，新增公式后更新规则配置并重新发布。
    /// </para>
    /// </summary>
    /// <param name="formulaId">公式定义主键（路径参数）。</param>
    /// <param name="request">公式更新请求（<see cref="FormulaDefUpdateRequest"/>）。</param>
    /// <returns>统一成功响应。</returns>
    [HttpPut("{formulaId:long}")]
    public async Task<ApiResponse> UpdateAsync(
        long formulaId, [FromBody] FormulaDefUpdateRequest request)
    {
        await _service.UpdateAsync(formulaId, request);
        return ApiResponse.Ok();
    }

    /// <summary>
    /// 【切换公式启用状态】— 在启用/停用之间切换指定公式的状态。
    /// <para>
    /// HTTP 方法：PATCH &nbsp;|&nbsp; 路由：<c>/api/pricing/formulas/{formulaId}/toggle</c>
    /// </para>
    /// <para>
    /// 用途：快捷切换公式状态，无需传入完整更新数据。
    /// 停用公式后，已引用该公式的规则在计价匹配时将跳过公式执行（但不影响规则本身匹配）。
    /// </para>
    /// </summary>
    /// <param name="formulaId">公式定义主键（路径参数）。</param>
    /// <returns>统一成功响应。</returns>
    [HttpPatch("{formulaId:long}/toggle")]
    public async Task<ApiResponse> ToggleAsync(long formulaId)
    {
        await _service.ToggleAsync(formulaId);
        return ApiResponse.Ok();
    }
}


