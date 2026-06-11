using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Application.RuntimePackages;

namespace Pricing.RuleCenter.Api.Controllers;

/// <summary>
/// 运行时包管理控制器，负责规则运行包的发布、差异比对、激活、回滚和历史查询。
/// </summary>
[ApiController]
[Authorize(Policy = "RuleAdmin")]
[Route("api/pricing/runtime-packages")]
public sealed class RuntimePackageController : ControllerBase
{
    private readonly RuntimePackagePublishService _publishService;
    private readonly RuntimePackageActivationService _activationService;
    private readonly RuntimePackageRollbackService _rollbackService;
    private readonly PolicyPackageDiffService _diffService;
    private readonly RuntimePackageQueryAppService _queryAppService;

    /// <summary>
    /// 初始化运行时包管理控制器。
    /// </summary>
    /// <param name="publishService">运行时包发布服务。</param>
    /// <param name="activationService">运行时包激活服务。</param>
    /// <param name="rollbackService">运行时包回滚服务。</param>
    /// <param name="diffService">运行时包差异分析服务。</param>
    /// <param name="queryAppService">运行时包查询服务。</param>
    public RuntimePackageController(
        RuntimePackagePublishService publishService,
        RuntimePackageActivationService activationService,
        RuntimePackageRollbackService rollbackService,
        PolicyPackageDiffService diffService,
        RuntimePackageQueryAppService queryAppService)
    {
        _publishService = publishService;
        _activationService = activationService;
        _rollbackService = rollbackService;
        _diffService = diffService;
        _queryAppService = queryAppService;
    }

    /// <summary>
    /// 根据策略版本集合构建新的运行时包。
    /// </summary>
    /// <param name="request">运行时包发布请求。</param>
    /// <returns>新运行时包主键。</returns>
    [HttpPost("publish")]
    public async Task<ApiResult<long>> PublishAsync([FromBody] RuntimePackagePublishRequest request)
    {
        var result = await _publishService.PublishAsync(request.PolicyVersionIds, request.PublishedBy);
        return ApiResult<long>.Ok(result.Package.PackageId);
    }

    /// <summary>
    /// 查看指定运行时包与当前激活包的差异。
    /// </summary>
    /// <param name="packageId">运行时包主键。</param>
    /// <returns>策略差异结果。</returns>
    [HttpGet("{packageId:long}/diff")]
    public async Task<ApiResult<PolicyPackageDiffResult>> DiffAsync(long packageId)
    {
        return ApiResult<PolicyPackageDiffResult>.Ok(await _diffService.DiffAgainstActiveAsync(packageId));
    }

    /// <summary>
    /// 激活指定运行时包。
    /// </summary>
    /// <param name="packageId">运行时包主键。</param>
    /// <param name="request">运行时包操作请求。</param>
    /// <returns>已激活的运行时包主键。</returns>
    [HttpPost("{packageId:long}/activate")]
    public async Task<ApiResult<long>> ActivateAsync(long packageId, [FromBody] RuntimePackageOperationRequest request)
    {
        var package = await _activationService.ActivateAsync(packageId, request.OperatedBy);
        return ApiResult<long>.Ok(package.PackageId);
    }

    /// <summary>
    /// 将当前激活包回滚到指定历史运行时包。
    /// </summary>
    /// <param name="packageId">回滚目标运行时包主键。</param>
    /// <param name="request">运行时包操作请求。</param>
    /// <returns>回滚后重新激活的运行时包主键。</returns>
    [HttpPost("{packageId:long}/rollback")]
    public async Task<ApiResult<long>> RollbackAsync(long packageId, [FromBody] RuntimePackageOperationRequest request)
    {
        var package = await _rollbackService.RollbackAsync(packageId, request.OperatedBy);
        return ApiResult<long>.Ok(package.PackageId);
    }

    /// <summary>
    /// 查询最近发布或激活过的运行时包历史。
    /// </summary>
    /// <param name="take">返回条数，默认 20。</param>
    /// <returns>运行时包历史记录。</returns>
    [HttpGet("history")]
    public async Task<ApiResult<IReadOnlyList<RuntimePackageHistoryResponse>>> HistoryAsync([FromQuery] int take = 20)
    {
        return ApiResult<IReadOnlyList<RuntimePackageHistoryResponse>>.Ok(await _queryAppService.GetHistoryAsync(take));
    }
}
