using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;

namespace Pricing.RuleCenter.Api.Controllers;

[ApiController]
[Route("api/pricing/formulas")]
/// <summary>
/// 公式定义控制器，暴露公式元数据的查询和维护接口。
/// </summary>
public sealed class FormulaDefController : ControllerBase
{
    /// <summary>
    /// _service 服务依赖，用于复用已经封装好的业务编排或领域处理能力。
    /// </summary>
    private readonly FormulaDefService _service;

    /// <summary>
    /// 初始化公式定义控制器。
    /// </summary>
    /// <param name="service">公式定义应用服务。</param>
    public FormulaDefController(FormulaDefService service)
    {
        _service = service;
    }

    [HttpGet]
    /// <summary>
    /// 查询全部公式定义。
    /// </summary>
    /// <returns>公式定义列表。</returns>
    public async Task<ApiResponse<IReadOnlyList<FormulaDefResponse>>> GetAllAsync()
    {
        var items = await _service.GetAllAsync();
        return ApiResponse<IReadOnlyList<FormulaDefResponse>>.Ok(items);
    }

    [HttpGet("{formulaId:long}")]
    /// <summary>
    /// 按主键查询公式定义。
    /// </summary>
    /// <param name="formulaId">公式定义主键。</param>
    /// <returns>公式定义详情。</returns>
    public async Task<ApiResponse<FormulaDefResponse>> GetByIdAsync(long formulaId)
    {
        var item = await _service.GetByIdAsync(formulaId)
            ?? throw new KeyNotFoundException($"公式定义不存在: {formulaId}");
        return ApiResponse<FormulaDefResponse>.Ok(item);
    }

    [HttpPost]
    /// <summary>
    /// 新增公式定义。
    /// </summary>
    /// <param name="request">公式定义新增请求。</param>
    /// <returns>新增公式定义主键。</returns>
    public async Task<ApiResponse<long>> CreateAsync(
        [FromBody] FormulaDefCreateRequest request)
    {
        var id = await _service.CreateAsync(request);
        return ApiResponse<long>.Ok(id);
    }

    [HttpPut("{formulaId:long}")]
    /// <summary>
    /// 更新公式定义。
    /// </summary>
    /// <param name="formulaId">公式定义主键。</param>
    /// <param name="request">公式定义更新请求。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> UpdateAsync(
        long formulaId, [FromBody] FormulaDefUpdateRequest request)
    {
        await _service.UpdateAsync(formulaId, request);
        return ApiResponse.Ok();
    }

    [HttpPatch("{formulaId:long}/toggle")]
    /// <summary>
    /// 切换公式定义启用状态。
    /// </summary>
    /// <param name="formulaId">公式定义主键。</param>
    /// <returns>统一成功响应。</returns>
    public async Task<ApiResponse> ToggleAsync(long formulaId)
    {
        await _service.ToggleAsync(formulaId);
        return ApiResponse.Ok();
    }
}
