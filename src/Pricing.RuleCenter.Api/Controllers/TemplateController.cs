using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Templates;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Authorize(Policy = "RuleAdmin")]
[Route("api/pricing/templates")]
public sealed class TemplateController : ControllerBase
{
    private readonly TemplateAppService _templateAppService;
    private readonly TemplateVersionAppService _templateVersionAppService;

    public TemplateController(
        TemplateAppService templateAppService,
        TemplateVersionAppService templateVersionAppService)
    {
        _templateAppService = templateAppService;
        _templateVersionAppService = templateVersionAppService;
    }

    [HttpGet]
    public async Task<ApiResult<IReadOnlyList<TemplateResponse>>> GetAllAsync()
    {
        return ApiResult<IReadOnlyList<TemplateResponse>>.Ok(await _templateAppService.GetAllAsync());
    }

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

    [HttpPost]
    public async Task<ApiResult<long>> CreateAsync([FromBody] TemplateCreateRequest request)
    {
        return ApiResult<long>.Ok(await _templateAppService.CreateAsync(request));
    }

    [HttpPut("{templateId:long}")]
    public async Task<ApiResult> UpdateAsync(long templateId, [FromBody] TemplateUpdateRequest request)
    {
        await _templateAppService.UpdateAsync(templateId, request);
        return ApiResult.Ok();
    }

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

    [HttpPost("{templateId:long}/versions")]
    public async Task<ApiResult<long>> SaveVersionAsync(long templateId, [FromBody] TemplateVersionSaveRequest request)
    {
        return ApiResult<long>.Ok(await _templateVersionAppService.SaveAsync(templateId, request));
    }
}
