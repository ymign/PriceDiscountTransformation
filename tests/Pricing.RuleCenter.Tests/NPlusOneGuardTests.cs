using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Application.Rules.Guards;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Options;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class NPlusOneGuardTests
{
    [Fact]
    public async Task EffectiveRuleSnapshotLoader_ShouldUseBatchConditionAndActionQueries()
    {
        var headers = new[]
        {
            CreateHeader(1, 1, "ITEM001"),
            CreateHeader(2, 2, "ITEM001")
        };
        var conditionRepository = new BatchOnlyRuleConditionRepository();
        var actionRepository = new BatchOnlyRuleActionRepository();
        conditionRepository.Seed((1, 1), new RuleCondition { RuleId = 1, VersionNo = 1, ConditionType = RuleConditionTypeCodes.ItemMatch, IsEnabled = EnableFlag.Yes });
        conditionRepository.Seed((2, 2), new RuleCondition { RuleId = 2, VersionNo = 2, ConditionType = RuleConditionTypeCodes.ItemMatch, IsEnabled = EnableFlag.Yes });
        actionRepository.Seed((1, 1), new RuleAction { RuleId = 1, VersionNo = 1, ActionType = RuleActionTypeCodes.FormulaCalc, IsEnabled = EnableFlag.Yes });
        actionRepository.Seed((2, 2), new RuleAction { RuleId = 2, VersionNo = 2, ActionType = RuleActionTypeCodes.ApplyMaxAmount, IsEnabled = EnableFlag.Yes });

        var loader = new EffectiveRuleSnapshotLoader(new RuleMatchRepositories(
            new FixedRuleHeaderRepository(headers),
            conditionRepository,
            actionRepository,
            new EmptyDictRepository()));

        var snapshots = await loader.LoadByItemCodeAsync("ITEM001");

        Assert.Equal(2, snapshots.Count);
        Assert.Equal(1, conditionRepository.BatchCallCount);
        Assert.Equal(1, actionRepository.BatchCallCount);
    }

    [Fact]
    public async Task RuleConflictDetector_ShouldUseBatchConditionAndActionQueries()
    {
        var target = CreateHeader(1, 1, "ITEM001");
        var existing = CreateHeader(2, 2, "ITEM001");
        var headerRepository = new FixedRuleHeaderRepository(new[] { target, existing });
        var conditionRepository = new BatchOnlyRuleConditionRepository();
        var actionRepository = new BatchOnlyRuleActionRepository();
        conditionRepository.Seed((1, 1), new RuleCondition
        {
            RuleId = 1,
            VersionNo = 1,
            ConditionType = RuleConditionTypeCodes.ChargeScene,
            RightValue = "OUTPATIENT",
            IsEnabled = EnableFlag.Yes
        });
        conditionRepository.Seed((2, 2), new RuleCondition
        {
            RuleId = 2,
            VersionNo = 2,
            ConditionType = RuleConditionTypeCodes.ChargeScene,
            RightValue = "INPATIENT",
            IsEnabled = EnableFlag.Yes
        });

        var detector = new RuleConflictDetector(
            headerRepository,
            conditionRepository,
            actionRepository,
            new EmptyDictRepository(),
            NullLogger<RuleConflictDetector>.Instance);

        await detector.EnsureNoConflictAsync(target, 1);

        Assert.Equal(1, conditionRepository.BatchCallCount);
        Assert.Equal(1, actionRepository.BatchCallCount);
    }

    [Fact]
    public async Task AuthorityPriceChecker_ShouldUseBatchUnitPriceQuery()
    {
        var repository = new BatchOnlyPriceMasterRepository(new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            ["ITEM001"] = 10m,
            ["ITEM002"] = 20m
        });
        var checker = new AuthorityPriceChecker(
            repository,
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = true }),
            NullLogger<AuthorityPriceChecker>.Instance);

        var request = new PricingCalculateRequest
        {
            PatientId = "P001",
            SourceSystem = "HIS",
            BusinessChargeTime = new DateTime(2026, 6, 8, 10, 0, 0),
            Items = new[]
            {
                new PricingCalculateItemRequest { ItemCode = "ITEM001", UnitPrice = 10m, InputQty = 1m },
                new PricingCalculateItemRequest { ItemCode = "ITEM002", UnitPrice = 20m, InputQty = 1m }
            }
        };

        await checker.CheckAsync(request, request.Items);

        Assert.Equal(1, repository.BatchCallCount);
    }

    private static RuleAggregate CreateHeader(long ruleId, int versionNo, string itemCode)
    {
        return new RuleAggregate
        {
            RuleId = ruleId,
            RuleCode = $"RULE_{ruleId}",
            RuleName = $"Rule {ruleId}",
            ItemCode = itemCode,
            Status = RuleStatusCodes.Published,
            IsEnabled = EnableFlag.Yes,
            CurrentVersion = versionNo,
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        };
    }

    private sealed class FixedRuleHeaderRepository : IRuleHeaderRepository
    {
        private readonly IReadOnlyList<RuleAggregate> _headers;

        public FixedRuleHeaderRepository(IReadOnlyList<RuleAggregate> headers)
        {
            _headers = headers;
        }

        public Task<RuleAggregate?> GetByIdAsync(long ruleId) =>
            Task.FromResult(_headers.SingleOrDefault(header => header.RuleId == ruleId));

        public Task<RuleAggregate?> GetByIdForUpdateAsync(long ruleId) => GetByIdAsync(ruleId);

        public Task<RuleAggregate?> GetByCodeAsync(string ruleCode) =>
            Task.FromResult(_headers.SingleOrDefault(header => header.RuleCode == ruleCode));

        public Task<IReadOnlyList<RuleAggregate>> GetByItemCodeAsync(string itemCode) =>
            Task.FromResult((IReadOnlyList<RuleAggregate>)_headers.Where(header => header.ItemCode == itemCode).ToList());

        public Task<(IReadOnlyList<RuleAggregate> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) =>
            Task.FromResult(((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>(), 0));

        public Task<IReadOnlyList<RuleAggregate>> GetEffectiveAsync(DateTime businessTime) =>
            Task.FromResult((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>());

        public Task<long> InsertAsync(RuleAggregate entity) => Task.FromResult(0L);

        public Task<bool> UpdateAsync(RuleAggregate entity, string? expectedCurrentStatus = null) => Task.FromResult(true);

        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }

    private sealed class BatchOnlyRuleConditionRepository : IRuleConditionRepository
    {
        private readonly Dictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleCondition>> _items = new();

        public int BatchCallCount { get; private set; }

        public void Seed((long RuleId, int VersionNo) key, params RuleCondition[] conditions)
        {
            _items[key] = conditions;
        }

        public Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            throw new NotSupportedException("当前测试要求走批量条件查询，而不是逐条查询。");

        public Task<IReadOnlyDictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleCondition>>> GetByRuleVersionsAsync(
            IReadOnlyCollection<(long RuleId, int VersionNo)> ruleVersions)
        {
            BatchCallCount++;
            var result = new Dictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleCondition>>();
            foreach (var ruleVersion in ruleVersions)
            {
                result[ruleVersion] = _items.TryGetValue(ruleVersion, out var items)
                    ? items
                    : Array.Empty<RuleCondition>();
            }

            return Task.FromResult((IReadOnlyDictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleCondition>>)result);
        }

        public Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities) => Task.CompletedTask;

        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class BatchOnlyRuleActionRepository : IRuleActionRepository
    {
        private readonly Dictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>> _items = new();

        public int BatchCallCount { get; private set; }

        public void Seed((long RuleId, int VersionNo) key, params RuleAction[] actions)
        {
            _items[key] = actions;
        }

        public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            throw new NotSupportedException("当前测试要求走批量动作查询，而不是逐条查询。");

        public Task<IReadOnlyDictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>>> GetByRuleVersionsAsync(
            IReadOnlyCollection<(long RuleId, int VersionNo)> ruleVersions)
        {
            BatchCallCount++;
            var result = new Dictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>>();
            foreach (var ruleVersion in ruleVersions)
            {
                result[ruleVersion] = _items.TryGetValue(ruleVersion, out var items)
                    ? items
                    : Array.Empty<RuleAction>();
            }

            return Task.FromResult((IReadOnlyDictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>>)result);
        }

        public Task InsertBatchAsync(IReadOnlyList<RuleAction> entities) => Task.CompletedTask;

        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class BatchOnlyPriceMasterRepository : IPriceMasterRepository
    {
        private readonly IReadOnlyDictionary<string, decimal> _prices;

        public BatchOnlyPriceMasterRepository(IReadOnlyDictionary<string, decimal> prices)
        {
            _prices = prices;
        }

        public int BatchCallCount { get; private set; }

        public Task<decimal?> GetUnitPriceAsync(string itemCode) =>
            throw new NotSupportedException("当前测试要求走批量单价查询，而不是逐条查询。");

        public Task<IReadOnlyDictionary<string, decimal?>> GetUnitPricesAsync(IReadOnlyCollection<string> itemCodes)
        {
            BatchCallCount++;
            var result = new Dictionary<string, decimal?>(StringComparer.OrdinalIgnoreCase);
            foreach (var itemCode in itemCodes)
            {
                result[itemCode] = _prices.TryGetValue(itemCode, out var price) ? price : null;
            }

            return Task.FromResult((IReadOnlyDictionary<string, decimal?>)result);
        }
    }

    private sealed class EmptyDictRepository : IDictRepository
    {
        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) =>
            Task.FromResult((IReadOnlyList<Dict>)Array.Empty<Dict>());

        public Task<Dict?> GetByIdAsync(long dictId) => Task.FromResult<Dict?>(null);

        public Task<IReadOnlyList<string>> GetAllTypesAsync() =>
            Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());

        public Task<long> InsertAsync(Dict entity) => Task.FromResult(0L);

        public Task<bool> UpdateAsync(Dict entity) => Task.FromResult(false);

        public Task<bool> SetEnabledAsync(long dictId, string isEnabled) => Task.FromResult(false);

        public Task<bool> ExistsAsync(string dictType, string dictCode) => Task.FromResult(false);
    }
}
