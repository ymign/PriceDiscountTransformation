using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Api.Application.Catalog;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class DictAppServiceTests
{
    [Fact]
    public async Task UpdateAsync_ClearsRuntimeCacheWhenActionTypeOrderChanges()
    {
        var repository = new InMemoryDictRepository(new Dict
        {
            DictId = 1,
            DictType = "ACTION_TYPE_ORDER",
            DictCode = "FORMULA_CALC",
            DictName = "公式计算",
            SortNo = 60,
            IsEnabled = "Y"
        });
        var runtimeCache = new CapturingRuleRuntimeCacheInvalidator();
        var service = new DictAppService(
            repository,
            new EmptyRuleChangeLogRepository(),
            new MemoryCache(new MemoryCacheOptions()),
            runtimeCache,
            NullLogger<DictAppService>.Instance);

        await service.UpdateAsync(1, new DictUpdateRequest
        {
            DictName = "公式计算",
            SortNo = 65
        });

        Assert.Equal(1, runtimeCache.ClearCount);
    }

    private sealed class CapturingRuleRuntimeCacheInvalidator : IRuleRuntimeCacheInvalidator
    {
        public int ClearCount { get; private set; }

        public void ClearRuntimeCache()
        {
            ClearCount++;
        }
    }

    private sealed class InMemoryDictRepository : IDictRepository
    {
        private readonly List<Dict> _items;

        public InMemoryDictRepository(params Dict[] items)
        {
            _items = items.ToList();
        }

        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) =>
            Task.FromResult((IReadOnlyList<Dict>)_items
                .Where(d => string.Equals(d.DictType, dictType, StringComparison.OrdinalIgnoreCase))
                .Where(d => d.IsEnabled == "Y")
                .ToList());

        public Task<Dict?> GetByIdAsync(long dictId) =>
            Task.FromResult(_items.FirstOrDefault(d => d.DictId == dictId));

        public Task<IReadOnlyList<string>> GetAllTypesAsync() =>
            Task.FromResult((IReadOnlyList<string>)_items.Select(d => d.DictType).Distinct().ToList());

        public Task<long> InsertAsync(Dict entity)
        {
            entity.DictId = _items.Count + 1;
            _items.Add(entity);
            return Task.FromResult(entity.DictId);
        }

        public Task<bool> UpdateAsync(Dict entity) => Task.FromResult(true);

        public Task<bool> SetEnabledAsync(long dictId, string isEnabled)
        {
            var entity = _items.First(d => d.DictId == dictId);
            entity.IsEnabled = isEnabled;
            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(string dictType, string dictCode) =>
            Task.FromResult(_items.Any(d =>
                string.Equals(d.DictType, dictType, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(d.DictCode, dictCode, StringComparison.OrdinalIgnoreCase)));
    }

    private sealed class EmptyRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());

        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
    }
}
