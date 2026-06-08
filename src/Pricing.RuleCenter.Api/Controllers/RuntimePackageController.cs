using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Application.RuntimePackages;

namespace Pricing.RuleCenter.Api.Controllers;

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

    [HttpPost("publish")]
    public async Task<ApiResult<long>> PublishAsync([FromBody] RuntimePackagePublishRequest request)
    {
        var result = await _publishService.PublishAsync(request.PolicyVersionIds, request.PublishedBy);
        return ApiResult<long>.Ok(result.Package.PackageId);
    }

    [HttpGet("{packageId:long}/diff")]
    public async Task<ApiResult<PolicyPackageDiffResult>> DiffAsync(long packageId)
    {
        return ApiResult<PolicyPackageDiffResult>.Ok(await _diffService.DiffAgainstActiveAsync(packageId));
    }

    [HttpPost("{packageId:long}/activate")]
    public async Task<ApiResult<long>> ActivateAsync(long packageId, [FromBody] RuntimePackageOperationRequest request)
    {
        var package = await _activationService.ActivateAsync(packageId, request.OperatedBy);
        return ApiResult<long>.Ok(package.PackageId);
    }

    [HttpPost("{packageId:long}/rollback")]
    public async Task<ApiResult<long>> RollbackAsync(long packageId, [FromBody] RuntimePackageOperationRequest request)
    {
        var package = await _rollbackService.RollbackAsync(packageId, request.OperatedBy);
        return ApiResult<long>.Ok(package.PackageId);
    }

    [HttpGet("history")]
    public async Task<ApiResult<IReadOnlyList<RuntimePackageHistoryResponse>>> HistoryAsync([FromQuery] int take = 20)
    {
        return ApiResult<IReadOnlyList<RuntimePackageHistoryResponse>>.Ok(await _queryAppService.GetHistoryAsync(take));
    }
}
