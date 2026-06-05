using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Catalog;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Api.Controllers;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ControllerNotFoundTests
{
    [Fact]
    public async Task RuleHeaderController_GetByIdAsync_ReturnsNotFoundResultWhenMissing()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            new EmptyRuleHeaderRepository(),
            new EmptyRuleChangeLogRepository(),
            cache,
            new NoopCacheVersionSynchronizer(),
            NullLogger<RuleHeaderService>.Instance);
        var controller = new RuleHeaderController(service);

        var result = await controller.GetByIdAsync(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.Equal(404, response.Code);
    }

    [Fact]
    public async Task RuleVersionController_GetByIdAsync_ReturnsNotFoundResultWhenMissing()
    {
        var service = new RuleVersionAppService(
            new EmptyRuleVersionRepository(),
            new EmptyRuleHeaderRepository(),
            NullLogger<RuleVersionAppService>.Instance);
        var controller = new RuleVersionController(service);

        var result = await controller.GetByIdAsync(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.Equal(404, response.Code);
    }

    [Fact]
    public async Task FormulaDefController_GetByIdAsync_ReturnsNotFoundResultWhenMissing()
    {
        var service = new FormulaDefAppService(
            new EmptyFormulaDefRepository(),
            new EmptyRuleChangeLogRepository(),
            NullLogger<FormulaDefAppService>.Instance);
        var controller = new FormulaDefController(service);

        var result = await controller.GetByIdAsync(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.Equal(404, response.Code);
    }

    [Fact]
    public async Task DictController_GetByIdAsync_ReturnsNotFoundResultWhenMissing()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new DictAppService(
            new EmptyDictRepository(),
            new EmptyRuleChangeLogRepository(),
            cache,
            new NoopCacheVersionSynchronizer(),
            new EmptyRuleRuntimeCacheInvalidator(),
            NullLogger<DictAppService>.Instance);
        var controller = new DictController(service);

        var result = await controller.GetByIdAsync(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.Equal(404, response.Code);
    }

    private sealed class EmptyRuleHeaderRepository : IRuleHeaderRepository
    {
        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(null);
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleHeader entity, string? expectedCurrentStatus = null) => Task.FromResult(true);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }

    private sealed class EmptyRuleVersionRepository : IRuleVersionRepository
    {
        public Task<RuleVersion?> GetByIdAsync(long versionId) => Task.FromResult<RuleVersion?>(null);
        public Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo) => Task.FromResult<RuleVersion?>(null);
        public Task<RuleVersion?> GetByRuleAndVersionForUpdateAsync(long ruleId, int versionNo) => Task.FromResult<RuleVersion?>(null);
        public Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleVersion>)Array.Empty<RuleVersion>());
        public Task<long> InsertAsync(RuleVersion entity) => Task.FromResult(0L);
        public Task<bool> UpdateStatusAsync(long versionId, string status, string? expectedCurrentStatus = null) => Task.FromResult(true);
    }

    private sealed class EmptyFormulaDefRepository : IFormulaDefRepository
    {
        public Task<IReadOnlyList<FormulaDef>> GetAllAsync() => Task.FromResult((IReadOnlyList<FormulaDef>)Array.Empty<FormulaDef>());
        public Task<FormulaDef?> GetByIdAsync(long formulaId) => Task.FromResult<FormulaDef?>(null);
        public Task<FormulaDef?> GetByCodeAsync(string formulaCode) => Task.FromResult<FormulaDef?>(null);
        public Task<long> InsertAsync(FormulaDef entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(FormulaDef entity) => Task.FromResult(true);
        public Task<bool> SetEnabledAsync(long formulaId, string isEnabled) => Task.FromResult(true);
    }

    private sealed class EmptyDictRepository : IDictRepository
    {
        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) => Task.FromResult((IReadOnlyList<Dict>)Array.Empty<Dict>());
        public Task<Dict?> GetByIdAsync(long dictId) => Task.FromResult<Dict?>(null);
        public Task<IReadOnlyList<string>> GetAllTypesAsync() => Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());
        public Task<long> InsertAsync(Dict entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(Dict entity) => Task.FromResult(true);
        public Task<bool> SetEnabledAsync(long dictId, string isEnabled) => Task.FromResult(true);
        public Task<bool> ExistsAsync(string dictType, string dictCode) => Task.FromResult(false);
    }

    private sealed class EmptyRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());
        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
    }

    private sealed class NoopCacheVersionSynchronizer : ICacheVersionSynchronizer
    {
        public Task SyncAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<long> IncreaseVersionAsync(string cacheScope, CancellationToken cancellationToken = default) => Task.FromResult(0L);
    }

    private sealed class EmptyRuleRuntimeCacheInvalidator : IRuleRuntimeCacheInvalidator
    {
        public void ClearRuntimeCache()
        {
        }
    }
}


