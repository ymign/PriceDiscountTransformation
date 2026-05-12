using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RulePublishConflictTests
{
    [Fact]
    public async Task PublishAsync_BlocksFormulaConflictForSameItemSceneAndEffectiveRange()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var service = CreateService(
            headerRepository,
            versionRepository,
            conditionRepository,
            actionRepository,
            new FixedDictRepository(new[]
            {
                new Dict
                {
                    DictType = "MUTUALLY_EXCLUSIVE_ACTION_TYPE",
                    DictCode = "APPLY_MAX_AMOUNT",
                    SortNo = 10,
                    IsEnabled = "Y"
                }
            }));

        headerRepository.Headers.AddRange(new[]
        {
            new RuleHeader
            {
                RuleId = 1,
                RuleCode = "R-OLD",
                ItemCode = "ITEM001",
                CurrentVersion = 1,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            },
            new RuleHeader
            {
                RuleId = 2,
                RuleCode = "R-NEW",
                ItemCode = "ITEM001",
                CurrentVersion = 0,
                Status = "DRAFT",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 6, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            }
        });
        versionRepository.Versions.AddRange(new[]
        {
            new RuleVersion { VersionId = 11, RuleId = 1, VersionNo = 1, VersionStatus = "PUBLISHED" },
            new RuleVersion { VersionId = 21, RuleId = 2, VersionNo = 1, VersionStatus = "DRAFT" }
        });
        conditionRepository.Add(1, 1, new RuleCondition
        {
            RuleId = 1,
            VersionNo = 1,
            ConditionType = "CHARGE_SCENE",
            RightValue = "OUTPATIENT",
            IsEnabled = "Y"
        });
        conditionRepository.Add(2, 1, new RuleCondition
        {
            RuleId = 2,
            VersionNo = 1,
            ConditionType = "CHARGE_SCENE",
            RightValue = "OUTPATIENT",
            IsEnabled = "Y"
        });
        actionRepository.Add(1, 1, new RuleAction
        {
            RuleId = 1,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            ExecutorCode = "INCREMENT_PERCENT",
            IsEnabled = "Y"
        });
        actionRepository.Add(2, 1, new RuleAction
        {
            RuleId = 2,
            VersionNo = 1,
            ActionType = "FORMULA_CALC",
            ExecutorCode = "INCREMENT_PERCENT",
            IsEnabled = "Y"
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.PublishAsync(2, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" }));
        Assert.Contains("RULE_CONFLICT", ex.Message);
    }

    [Fact]
    public async Task PublishAsync_AllowsConvertQtyRulesWithDifferentBodyParts()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var service = CreateService(headerRepository, versionRepository, conditionRepository, actionRepository);

        headerRepository.Headers.AddRange(new[]
        {
            new RuleHeader
            {
                RuleId = 1,
                RuleCode = "R-HEAD",
                ItemCode = "ITEM001",
                CurrentVersion = 1,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            },
            new RuleHeader
            {
                RuleId = 2,
                RuleCode = "R-BODY",
                ItemCode = "ITEM001",
                CurrentVersion = 0,
                Status = "DRAFT",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            }
        });
        versionRepository.Versions.AddRange(new[]
        {
            new RuleVersion { VersionId = 11, RuleId = 1, VersionNo = 1, VersionStatus = "PUBLISHED" },
            new RuleVersion { VersionId = 21, RuleId = 2, VersionNo = 1, VersionStatus = "DRAFT" }
        });
        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionType = "BODY_PART", RightValue = "HEAD", IsEnabled = "Y" });
        conditionRepository.Add(2, 1, new RuleCondition { RuleId = 2, VersionNo = 1, ConditionType = "BODY_PART", RightValue = "BODY", IsEnabled = "Y" });
        actionRepository.Add(1, 1, new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "CONVERT_QTY", IsEnabled = "Y" });
        actionRepository.Add(2, 1, new RuleAction { RuleId = 2, VersionNo = 1, ActionType = "CONVERT_QTY", IsEnabled = "Y" });

        await service.PublishAsync(2, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" });

        Assert.Equal("PUBLISHED", headerRepository.Headers.Single(h => h.RuleId == 2).Status);
    }

    [Fact]
    public async Task PublishAsync_AllowsConvertQtyWhenSceneAndBodyOnlyOverlapAcrossDifferentConditionGroups()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var service = CreateService(headerRepository, versionRepository, conditionRepository, actionRepository);

        headerRepository.Headers.AddRange(new[]
        {
            new RuleHeader
            {
                RuleId = 1,
                RuleCode = "R-OLD",
                ItemCode = "ITEM001",
                CurrentVersion = 1,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            },
            new RuleHeader
            {
                RuleId = 2,
                RuleCode = "R-NEW",
                ItemCode = "ITEM001",
                CurrentVersion = 0,
                Status = "DRAFT",
                IsEnabled = "Y",
                EffectiveFrom = new DateTime(2026, 1, 1),
                EffectiveTo = new DateTime(2026, 12, 31)
            }
        });
        versionRepository.Versions.AddRange(new[]
        {
            new RuleVersion { VersionId = 11, RuleId = 1, VersionNo = 1, VersionStatus = "PUBLISHED" },
            new RuleVersion { VersionId = 21, RuleId = 2, VersionNo = 1, VersionStatus = "DRAFT" }
        });

        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionGroup = "G1", ConditionType = "CHARGE_SCENE", RightValue = "SCENE_A", IsEnabled = "Y" });
        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionGroup = "G1", ConditionType = "BODY_PART", RightValue = "HEAD", IsEnabled = "Y" });
        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionGroup = "G2", ConditionType = "CHARGE_SCENE", RightValue = "SCENE_B", IsEnabled = "Y" });
        conditionRepository.Add(1, 1, new RuleCondition { RuleId = 1, VersionNo = 1, ConditionGroup = "G2", ConditionType = "BODY_PART", RightValue = "TRUNK", IsEnabled = "Y" });

        conditionRepository.Add(2, 1, new RuleCondition { RuleId = 2, VersionNo = 1, ConditionGroup = "G1", ConditionType = "CHARGE_SCENE", RightValue = "SCENE_B", IsEnabled = "Y" });
        conditionRepository.Add(2, 1, new RuleCondition { RuleId = 2, VersionNo = 1, ConditionGroup = "G1", ConditionType = "BODY_PART", RightValue = "HEAD", IsEnabled = "Y" });

        actionRepository.Add(1, 1, new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "CONVERT_QTY", IsEnabled = "Y" });
        actionRepository.Add(2, 1, new RuleAction { RuleId = 2, VersionNo = 1, ActionType = "CONVERT_QTY", IsEnabled = "Y" });

        await service.PublishAsync(2, new RulePublishRequest { VersionNo = 1, PublishedBy = "tester" });

        Assert.Equal("PUBLISHED", headerRepository.Headers.Single(h => h.RuleId == 2).Status);
    }

    private static RulePublishService CreateService(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        IDictRepository? dictRepository = null) =>
        new(
            new RulePublishLifecycleRepositories(
                headerRepository,
                versionRepository,
                new EmptyRulePublishRepository(),
                new EmptyRuleChangeLogRepository()),
            new RulePublishDefinitionRepositories(
                conditionRepository,
                actionRepository,
                dictRepository ?? new EmptyDictRepository()),
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<RulePublishService>.Instance);

    private sealed class InMemoryRuleHeaderRepository : IRuleHeaderRepository
    {
        public List<RuleHeader> Headers { get; } = new();
        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult(Headers.SingleOrDefault(h => h.RuleId == ruleId));
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult(Headers.SingleOrDefault(h => h.RuleCode == ruleCode));
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Headers.Where(h => h.ItemCode == itemCode).ToList());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Headers.ToList(), Headers.Count));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) => Task.FromResult((IReadOnlyList<RuleHeader>)Headers.Where(h => h.IsEnabled == "Y" && h.Status == "PUBLISHED").ToList());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(entity.RuleId);
        public Task<bool> UpdateAsync(RuleHeader entity) => Task.FromResult(true);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(Headers.Any(h => h.RuleCode == ruleCode));
    }

    private sealed class InMemoryRuleVersionRepository : IRuleVersionRepository
    {
        public List<RuleVersion> Versions { get; } = new();
        public Task<RuleVersion?> GetByIdAsync(long versionId) => Task.FromResult(Versions.SingleOrDefault(v => v.VersionId == versionId));
        public Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo) => Task.FromResult(Versions.SingleOrDefault(v => v.RuleId == ruleId && v.VersionNo == versionNo));
        public Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleVersion>)Versions.Where(v => v.RuleId == ruleId).ToList());
        public Task<long> InsertAsync(RuleVersion entity) => Task.FromResult(entity.VersionId);
        public Task<bool> UpdateStatusAsync(long versionId, string status)
        {
            var version = Versions.Single(v => v.VersionId == versionId);
            version.VersionStatus = status;
            return Task.FromResult(true);
        }
    }

    private sealed class InMemoryRuleConditionRepository : IRuleConditionRepository
    {
        private readonly Dictionary<(long RuleId, int VersionNo), List<RuleCondition>> _items = new();

        public void Add(long ruleId, int versionNo, RuleCondition condition)
        {
            var key = (ruleId, versionNo);
            if (!_items.TryGetValue(key, out var items))
            {
                items = new List<RuleCondition>();
                _items[key] = items;
            }

            items.Add(condition);
        }

        public Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleCondition>)(_items.TryGetValue((ruleId, versionNo), out var items) ? items : new List<RuleCondition>()));

        public Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities) => Task.CompletedTask;
        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class InMemoryRuleActionRepository : IRuleActionRepository
    {
        private readonly Dictionary<(long RuleId, int VersionNo), List<RuleAction>> _items = new();

        public void Add(long ruleId, int versionNo, RuleAction action)
        {
            var key = (ruleId, versionNo);
            if (!_items.TryGetValue(key, out var items))
            {
                items = new List<RuleAction>();
                _items[key] = items;
            }

            items.Add(action);
        }

        public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleAction>)(_items.TryGetValue((ruleId, versionNo), out var items) ? items : new List<RuleAction>()));

        public Task InsertBatchAsync(IReadOnlyList<RuleAction> entities) => Task.CompletedTask;
        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class EmptyRulePublishRepository : IRulePublishRepository
    {
        public Task<IReadOnlyList<RulePublish>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RulePublish>)Array.Empty<RulePublish>());
        public Task<long> InsertAsync(RulePublish entity) => Task.FromResult(0L);
    }

    private sealed class EmptyRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());
        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
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

    private sealed class FixedDictRepository : IDictRepository
    {
        private readonly IReadOnlyList<Dict> _items;

        public FixedDictRepository(IReadOnlyList<Dict> items)
        {
            _items = items;
        }

        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) =>
            Task.FromResult((IReadOnlyList<Dict>)_items
                .Where(d => d.DictType == dictType && d.IsEnabled == "Y")
                .OrderBy(d => d.SortNo)
                .ToList());
        public Task<Dict?> GetByIdAsync(long dictId) => Task.FromResult<Dict?>(null);
        public Task<IReadOnlyList<string>> GetAllTypesAsync() =>
            Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());
        public Task<long> InsertAsync(Dict entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(Dict entity) => Task.FromResult(false);
        public Task<bool> SetEnabledAsync(long dictId, string isEnabled) => Task.FromResult(false);
        public Task<bool> ExistsAsync(string dictType, string dictCode) => Task.FromResult(false);
    }
}
