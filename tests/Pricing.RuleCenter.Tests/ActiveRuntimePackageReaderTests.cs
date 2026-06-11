using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Application.Engine;
using Pricing.RuleCenter.Application.Engine.RuleRuntimeSnapshot;
using Pricing.RuleCenter.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Core.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ActiveRuntimePackageReaderTests
{
    [Fact]
    public async Task LoadByItemCodeAsync_ReturnsEmpty_WhenNoActivePackage()
    {
        var reader = new ActiveRuntimePackageReader(
            new FixedRuntimePackageStateRepository(null),
            new FixedRuntimeRuleReadRepository());

        var snapshots = await reader.LoadByItemCodeAsync("ITEM001");

        Assert.Empty(snapshots);
    }

    [Fact]
    public async Task LoadByItemCodeAsync_LoadsRuleConditionsAndActions_FromActivePackage()
    {
        var reader = new ActiveRuntimePackageReader(
            new FixedRuntimePackageStateRepository(new RuntimePackageState
            {
                StateCode = "ACTIVE",
                ActivePackageId = 88,
                ActivePackageVersion = 3
            }),
            new FixedRuntimeRuleReadRepository(
                new RuntimeRule
                {
                    RuntimeRuleId = 101,
                    PackageId = 88,
                    TargetItemCode = "ITEM001",
                    CapabilityFamily = "FORMULA_PRICING",
                    PriorityKey = "10|ITEM"
                },
                new RuntimeCondition
                {
                    RuntimeConditionId = 201,
                    RuntimeRuleId = 101,
                    ConditionType = "ITEM_MATCH"
                },
                new RuntimeAction
                {
                    RuntimeActionId = 301,
                    RuntimeRuleId = 101,
                    ActionType = "FORMULA_CALC",
                    ExecutorCode = "ExpressionFormulaExecutor"
                }));

        var snapshots = await reader.LoadByItemCodeAsync("ITEM001");

        var snapshot = Assert.Single(snapshots);
        Assert.Equal(101, snapshot.Rule.RuntimeRuleId);
        Assert.Single(snapshot.Conditions);
        Assert.Single(snapshot.Actions);
    }

    [Fact]
    public async Task EffectiveRuleSnapshotLoader_UsesRuntimeReadModel_WhenAvailable()
    {
        var repositories = new RuleMatchRepositories(
            new EmptyRuleHeaderRepository(),
            new EmptyRuleConditionRepository(),
            new EmptyRuleActionRepository(),
            new EmptyDictRepository(),
            new FixedRuntimePackageStateRepository(new RuntimePackageState
            {
                StateCode = "ACTIVE",
                ActivePackageId = 99,
                ActivePackageVersion = 7
            }),
            new FixedRuntimeRuleReadRepository(
                new RuntimeRule
                {
                    RuntimeRuleId = 111,
                    PackageId = 99,
                    TargetItemCode = "ITEM001",
                    CapabilityFamily = "QTY_LIMIT_TIME_WINDOW",
                    PriorityKey = "5|ITEM",
                    EffectiveFrom = new DateTime(2026, 6, 1),
                    EffectiveTo = new DateTime(2026, 6, 30)
                },
                new RuntimeCondition
                {
                    RuntimeConditionId = 211,
                    RuntimeRuleId = 111,
                    ConditionGroup = "DEFAULT",
                    ConditionType = "ITEM_MATCH",
                    SortNo = 1
                },
                new RuntimeAction
                {
                    RuntimeActionId = 311,
                    RuntimeRuleId = 111,
                    ActionType = "APPLY_TIME_WINDOW_LIMIT",
                    ExecutorCode = "TimeWindowLimitExecutor",
                    SortNo = 1,
                    OnError = "STOP"
                }));
        var loader = new EffectiveRuleSnapshotLoader(repositories);

        var snapshots = await loader.LoadByItemCodeAsync("ITEM001");

        var snapshot = Assert.Single(snapshots);
        Assert.Equal(111, snapshot.Header.RuleId);
        Assert.Equal("ITEM001", snapshot.Header.ItemCode);
        Assert.Equal("QTY_LIMIT_TIME_WINDOW", snapshot.Header.RuleCategory);
        Assert.Single(snapshot.Conditions);
        Assert.Single(snapshot.Actions);
        Assert.Equal("APPLY_TIME_WINDOW_LIMIT", snapshot.Actions[0].ActionType);
    }

    [Fact]
    public async Task RuleMatchService_UsesRuntimePackageRules_WhenRuntimeRepositoriesAreRegistered()
    {
        var repositories = new RuleMatchRepositories(
            new EmptyRuleHeaderRepository(),
            new EmptyRuleConditionRepository(),
            new EmptyRuleActionRepository(),
            new EmptyDictRepository(new[]
            {
                new Dict
                {
                    DictType = "ACTION_TYPE_ORDER",
                    DictCode = "FORMULA_CALC",
                    SortNo = 10,
                    IsEnabled = "Y"
                }
            }),
            new FixedRuntimePackageStateRepository(new RuntimePackageState
            {
                StateCode = "ACTIVE",
                ActivePackageId = 66,
                ActivePackageVersion = 2
            }),
            new FixedRuntimeRuleReadRepository(
                new[]
                {
                    new RuntimeRule
                    {
                        RuntimeRuleId = 1201,
                        PackageId = 66,
                        TargetItemCode = "ITEM001",
                        CapabilityFamily = "FORMULA_PRICING",
                        PriorityKey = "10|ITEM"
                    }
                },
                Array.Empty<RuntimeCondition>(),
                new[]
                {
                    new RuntimeAction
                    {
                        RuntimeActionId = 3201,
                        RuntimeRuleId = 1201,
                        ActionType = "FORMULA_CALC",
                        ExecutorCode = "ExpressionFormulaExecutor",
                        SortNo = 1,
                        OnError = "STOP"
                    }
                }));
        var service = CreateRuleMatchService(
            repositories,
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()));

        var (rules, actions) = await service.MatchAsync(new PricingContext
        {
            ItemCode = "ITEM001",
            BusinessChargeTime = new DateTime(2026, 6, 7, 10, 0, 0)
        });

        var matchedRule = Assert.Single(rules);
        Assert.Equal(1201, matchedRule.RuleId);
        var matchedAction = Assert.Single(actions);
        Assert.Equal(1201, matchedAction.RuleId);
        Assert.Equal("FORMULA_CALC", matchedAction.ActionType);
    }

    private static RuleMatchService CreateRuleMatchService(
        RuleMatchRepositories repositories,
        ConditionEvaluatorFactory evaluatorFactory)
    {
        var matcher = new RuleConditionGroupMatcher(
            evaluatorFactory,
            NullLogger<RuleConditionGroupMatcher>.Instance);
        var actionPlanBuilder = new RuleActionPlanBuilder(
            repositories.DictRepository,
            NullLogger<RuleActionPlanBuilder>.Instance);
        var snapshotCache = new EffectiveRuleSnapshotCache(
            new MemoryCache(new MemoryCacheOptions()),
            new EffectiveRuleSnapshotLoader(repositories),
            repositories.RuntimePackageTraceContextAccessor);
        return new RuleMatchService(
            snapshotCache,
            matcher,
            actionPlanBuilder,
            NullLogger<RuleMatchService>.Instance);
    }

    [Fact]
    public async Task RuntimePackageTraceScope_KeepsReaderAndResolverOnCapturedPackage()
    {
        var accessor = new RuntimePackageTraceContextAccessor();
        var stateRepository = new MutableRuntimePackageStateRepository(new RuntimePackageState
        {
            StateCode = "ACTIVE",
            ActivePackageId = 77,
            ActivePackageVersion = 5
        });
        var ruleRepository = new FixedRuntimeRuleReadRepository(
            new[]
            {
                new RuntimeRule
                {
                    RuntimeRuleId = 501,
                    PackageId = 77,
                    TargetItemCode = "ITEM001",
                    CapabilityFamily = "FORMULA_PRICING",
                    PriorityKey = "10|ITEM",
                    SourcePolicyVersionId = 7001,
                    SourceTemplateVersionId = 8001
                },
                new RuntimeRule
                {
                    RuntimeRuleId = 601,
                    PackageId = 88,
                    TargetItemCode = "ITEM001",
                    CapabilityFamily = "FORMULA_PRICING",
                    PriorityKey = "10|ITEM",
                    SourcePolicyVersionId = 7002,
                    SourceTemplateVersionId = 8002
                }
            },
            Array.Empty<RuntimeCondition>(),
            Array.Empty<RuntimeAction>());
        var resolver = new RuntimePackageTraceResolver(stateRepository, ruleRepository, accessor);

        var runtimePackageContext = await resolver.CaptureContextAsync();
        using var scope = resolver.BeginScope(runtimePackageContext);
        stateRepository.State = new RuntimePackageState
        {
            StateCode = "ACTIVE",
            ActivePackageId = 88,
            ActivePackageVersion = 6
        };

        var reader = new ActiveRuntimePackageReader(stateRepository, ruleRepository, accessor);
        var snapshots = await reader.LoadByItemCodeAsync("ITEM001");
        var resolution = await resolver.ResolveAsync(new[] { 501L });

        var snapshot = Assert.Single(snapshots);
        Assert.Equal(501, snapshot.Rule.RuntimeRuleId);
        Assert.Equal(77, resolution.RuntimePackageId);
        Assert.Equal(5, resolution.RuntimePackageVersion);
        Assert.Equal(7001, Assert.Single(resolution.RuntimeRulesById.Values).SourcePolicyVersionId);
    }

    private sealed class FixedRuntimePackageStateRepository : IRuntimePackageStateRepository
    {
        private readonly RuntimePackageState? _state;

        public FixedRuntimePackageStateRepository(RuntimePackageState? state)
        {
            _state = state;
        }

        public Task<RuntimePackageState?> GetActiveAsync() => Task.FromResult(_state);

        public Task<RuntimePackageState?> GetActiveForUpdateAsync() => Task.FromResult(_state);

        public Task UpsertAsync(RuntimePackageState entity) => Task.CompletedTask;
    }

    private sealed class MutableRuntimePackageStateRepository : IRuntimePackageStateRepository
    {
        public RuntimePackageState? State { get; set; }

        public MutableRuntimePackageStateRepository(RuntimePackageState? state)
        {
            State = state;
        }

        public Task<RuntimePackageState?> GetActiveAsync() => Task.FromResult(State);

        public Task<RuntimePackageState?> GetActiveForUpdateAsync() => Task.FromResult(State);

        public Task UpsertAsync(RuntimePackageState entity)
        {
            State = entity;
            return Task.CompletedTask;
        }
    }

    private sealed class FixedRuntimeRuleReadRepository : IRuntimeRuleReadRepository
    {
        private readonly IReadOnlyList<RuntimeRule> _rules;
        private readonly IReadOnlyDictionary<long, IReadOnlyList<RuntimeCondition>> _conditions;
        private readonly IReadOnlyDictionary<long, IReadOnlyList<RuntimeAction>> _actions;

        public FixedRuntimeRuleReadRepository()
            : this(Array.Empty<RuntimeRule>(), Array.Empty<RuntimeCondition>(), Array.Empty<RuntimeAction>())
        {
        }

        public FixedRuntimeRuleReadRepository(
            RuntimeRule rule,
            RuntimeCondition condition,
            RuntimeAction action)
            : this(new[] { rule }, new[] { condition }, new[] { action })
        {
        }

        public FixedRuntimeRuleReadRepository(
            IReadOnlyList<RuntimeRule> rules,
            IReadOnlyList<RuntimeCondition> conditions,
            IReadOnlyList<RuntimeAction> actions)
        {
            _rules = rules;
            _conditions = conditions
                .GroupBy(item => item.RuntimeRuleId)
                .ToDictionary(group => group.Key, group => (IReadOnlyList<RuntimeCondition>)group.ToList());
            _actions = actions
                .GroupBy(item => item.RuntimeRuleId)
                .ToDictionary(group => group.Key, group => (IReadOnlyList<RuntimeAction>)group.ToList());
        }

        public Task<IReadOnlyList<RuntimeRule>> GetRulesByItemCodeAsync(long packageId, string itemCode) =>
            Task.FromResult((IReadOnlyList<RuntimeRule>)_rules
                .Where(rule => rule.PackageId == packageId && string.Equals(rule.TargetItemCode, itemCode, StringComparison.Ordinal))
                .ToList());

        public Task<IReadOnlyList<RuntimeRule>> GetRulesByIdsAsync(IReadOnlyCollection<long> runtimeRuleIds) =>
            Task.FromResult((IReadOnlyList<RuntimeRule>)_rules
                .Where(rule => runtimeRuleIds.Contains(rule.RuntimeRuleId))
                .ToList());

        public Task<IReadOnlyDictionary<long, IReadOnlyList<RuntimeCondition>>> GetConditionsByRuleIdsAsync(IReadOnlyCollection<long> runtimeRuleIds) =>
            Task.FromResult((IReadOnlyDictionary<long, IReadOnlyList<RuntimeCondition>>)_conditions
                .Where(pair => runtimeRuleIds.Contains(pair.Key))
                .ToDictionary(pair => pair.Key, pair => pair.Value));

        public Task<IReadOnlyDictionary<long, IReadOnlyList<RuntimeAction>>> GetActionsByRuleIdsAsync(IReadOnlyCollection<long> runtimeRuleIds) =>
            Task.FromResult((IReadOnlyDictionary<long, IReadOnlyList<RuntimeAction>>)_actions
                .Where(pair => runtimeRuleIds.Contains(pair.Key))
                .ToDictionary(pair => pair.Key, pair => pair.Value));
    }

    private sealed class EmptyRuleHeaderRepository : IRuleHeaderRepository
    {
        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(null);
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) =>
            Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleHeader entity, string? expectedCurrentStatus = null) => Task.FromResult(false);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }

    private sealed class EmptyRuleConditionRepository : IRuleConditionRepository
    {
        public Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleCondition>)Array.Empty<RuleCondition>());

        public Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities) => Task.CompletedTask;

        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class EmptyRuleActionRepository : IRuleActionRepository
    {
        public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleAction>)Array.Empty<RuleAction>());

        public Task InsertBatchAsync(IReadOnlyList<RuleAction> entities) => Task.CompletedTask;

        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class EmptyDictRepository : IDictRepository
    {
        private readonly IReadOnlyList<Dict> _items;

        public EmptyDictRepository()
            : this(Array.Empty<Dict>())
        {
        }

        public EmptyDictRepository(IReadOnlyList<Dict> items)
        {
            _items = items;
        }

        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) =>
            Task.FromResult((IReadOnlyList<Dict>)_items
                .Where(item => item.DictType == dictType && item.IsEnabled == "Y")
                .OrderBy(item => item.SortNo)
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
