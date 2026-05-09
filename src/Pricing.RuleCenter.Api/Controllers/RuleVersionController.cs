using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/rules/{ruleId:long}/versions")]
/// <summary>
/// 规则版本控制器，暴露规则版本查询和草稿版本创建接口。
/// </summary>
public sealed class RuleVersionController : ControllerBase
{
    /// <summary>
    /// _service 服务依赖，用于复用已经封装好的业务编排或领域处理能力。
    /// </summary>
    private readonly RuleVersionService _service;

    /// <summary>
    /// 初始化规则版本控制器。
    /// </summary>
    /// <param name="service">规则版本应用服务。</param>
    public RuleVersionController(RuleVersionService service)
    {
        _service = service;
    }

    [HttpGet]
    /// <summary>
    /// 查询指定规则的全部版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>规则版本列表。</returns>
    public async Task<ApiResponse<IReadOnlyList<RuleVersionResponse>>> GetByRuleIdAsync(
        long ruleId)
    {
        var items = await _service.GetByRuleIdAsync(ruleId);
        return ApiResponse<IReadOnlyList<RuleVersionResponse>>.Ok(items);
    }

    [HttpGet("{versionId:long}")]
    /// <summary>
    /// 按版本主键查询规则版本。
    /// </summary>
    /// <param name="versionId">版本主键。</param>
    /// <returns>规则版本详情。</returns>
    public async Task<ApiResponse<RuleVersionResponse>> GetByIdAsync(long versionId)
    {
        var item = await _service.GetByIdAsync(versionId)
            ?? throw new KeyNotFoundException($"规则版本不存在: {versionId}");
        return ApiResponse<RuleVersionResponse>.Ok(item);
    }

    [HttpPost]
    /// <summary>
    /// 创建下一个草稿版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>新增草稿版本主键。</returns>
    public async Task<ApiResponse<long>> CreateDraftAsync(long ruleId)
    {
        var id = await _service.CreateDraftAsync(ruleId);
        return ApiResponse<long>.Ok(id);
    }
}
