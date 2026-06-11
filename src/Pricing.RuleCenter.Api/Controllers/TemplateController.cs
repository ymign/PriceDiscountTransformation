using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Templates;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 模板平台管理控制器，负责模板主档和模板版本的查询与维护接口。
/// </summary>
[ApiController]
[Authorize(Policy = "RuleAdmin")]
[Route("api/pricing/templates")]
public sealed class TemplateController : ControllerBase
{
    private readonly TemplateAppService _templateAppService;
    private readonly TemplateVersionAppService _templateVersionAppService;

    /// <summary>
    /// 初始化模板平台管理控制器。
    /// </summary>
    /// <param name="templateAppService">模板主档应用服务。</param>
    /// <param name="templateVersionAppService">模板版本应用服务。</param>
    public TemplateController(
        TemplateAppService templateAppService,
        TemplateVersionAppService templateVersionAppService)
    {
        _templateAppService = templateAppService;
        _templateVersionAppService = templateVersionAppService;
    }

    /// <summary>
    /// 查询全部模板主档列表。
    /// </summary>
    /// <returns>模板概要列表。</returns>
    [HttpGet]
    public async Task<ApiResult<IReadOnlyList<TemplateResponse>>> GetAllAsync()
    {
        return ApiResult<IReadOnlyList<TemplateResponse>>.Ok(await _templateAppService.GetAllAsync());
    }

    /// <summary>
    /// 按主键查询模板详情。
    /// </summary>
    /// <param name="templateId">模板主键。</param>
    /// <returns>命中时返回模板详情；不存在时返回 404。</returns>
    [HttpGet("{templateId:long}")]
    public async Task<ActionResult<ApiResult<TemplateDetailResponse>>> GetByIdAsync(long templateId)
    {
        var item = await _templateAppService.GetByIdAsync(templateId);
        if (item is null)
        {
            return NotFound(ApiResult.Fail(404, $"模板不存在: {templateId}"));
        }

        return ApiResult<TemplateDetailResponse>.Ok(item);
    }

    /// <summary>
    /// 创建模板主档。
    /// </summary>
    /// <param name="request">模板创建请求。</param>
    /// <returns>新模板主键。</returns>
    [HttpPost]
    public async Task<ApiResult<long>> CreateAsync([FromBody] TemplateCreateRequest request)
    {
        return ApiResult<long>.Ok(await _templateAppService.CreateAsync(request));
    }

    /// <summary>
    /// 更新模板主档基础信息。
    /// </summary>
    /// <param name="templateId">模板主键。</param>
    /// <param name="request">模板更新请求。</param>
    /// <returns>统一成功响应。</returns>
    [HttpPut("{templateId:long}")]
    public async Task<ApiResult> UpdateAsync(long templateId, [FromBody] TemplateUpdateRequest request)
    {
        await _templateAppService.UpdateAsync(templateId, request);
        return ApiResult.Ok();
    }

    /// <summary>
    /// 查询指定模板下的单个版本详情。
    /// </summary>
    /// <param name="templateId">模板主键。</param>
    /// <param name="templateVersionId">模板版本主键。</param>
    /// <returns>命中时返回模板版本详情；不存在或模板不匹配时返回 404。</returns>
    [HttpGet("{templateId:long}/versions/{templateVersionId:long}")]
    public async Task<ActionResult<ApiResult<TemplateVersionResponse>>> GetVersionAsync(long templateId, long templateVersionId)
    {
        var item = await _templateVersionAppService.GetByIdAsync(templateVersionId);
        if (item is null || item.TemplateId != templateId)
        {
            return NotFound(ApiResult.Fail(404, $"模板版本不存在: {templateVersionId}"));
        }

        return ApiResult<TemplateVersionResponse>.Ok(item);
    }

    /// <summary>
    /// 保存模板版本草稿。
    /// </summary>
    /// <param name="templateId">模板主键。</param>
    /// <param name="request">模板版本保存请求。</param>
    /// <returns>模板版本主键。</returns>
    [HttpPost("{templateId:long}/versions")]
    public async Task<ApiResult<long>> SaveVersionAsync(long templateId, [FromBody] TemplateVersionSaveRequest request)
    {
        return ApiResult<long>.Ok(await _templateVersionAppService.SaveAsync(templateId, request));
    }
}
