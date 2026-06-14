using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Application.Engine;
using Pricing.RuleCenter.Application.Engine.EffectiveRules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuleMatchServiceTests
{
    [Fact]
    public async Task MatchAsync_KeepsDefaultActionOrderWhenDictIsPartial()
    {
        var rule = new RuleHeader
        {
            RuleId = 1,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1
        };
        var actions = new[]
        {
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "FORMULA_CALC", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "CONVERT_QTY", SortNo = 10, IsEnabled = "Y" }
        };
        var dictItems = new[]
        {
            new Dict
            {
                DictType = "ACTION_TYPE_ORDER",
                DictCode = "FORMULA_CALC",
                SortNo = 20,
                IsEnabled = "Y"
            }
        };
        var service = CreateRuleMatchService(
            new FixedRuleHeaderRepository(rule),
            new EmptyRuleConditionRepository(),
            new FixedRuleActionRepository(actions),
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()),
            new FixedDictRepository(dictItems),
            NullLogger<RuleMatchService>.Instance);

        var (_, orderedActions) = await service.MatchAsync(new PricingContext
        {
            ItemCode = "ITEM001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        });

        Assert.Collection(
            orderedActions,
            first => Assert.Equal("CONVERT_QTY", first.ActionType),
            second => Assert.Equal("FORMULA_CALC", second.ActionType));
    }

    [Fact]
    public async Task MatchAsync_DefaultOrderPlacesSameOperationBeforeChildAndExceed()
    {
        var rule = new RuleHeader
        {
            RuleId = 1,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1
        };
        var actions = new[]
        {
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "ADD_CHILD_ITEM", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "SAME_OPERATION_CEILING", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "DISCOUNT_EXCEED_TO_ZERO", SortNo = 10, IsEnabled = "Y" }
        };
        var service = CreateRuleMatchService(
            new FixedRuleHeaderRepository(rule),
            new EmptyRuleConditionRepository(),
            new FixedRuleActionRepository(actions),
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()),
            new FixedDictRepository(Array.Empty<Dict>()),
            NullLogger<RuleMatchService>.Instance);

        var (_, orderedActions) = await service.MatchAsync(new PricingContext
        {
            ItemCode = "ITEM001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        });

        Assert.Collection(
            orderedActions,
            first => Assert.Equal("SAME_OPERATION_CEILING", first.ActionType),
            second => Assert.Equal("ADD_CHILD_ITEM", second.ActionType),
            third => Assert.Equal("DISCOUNT_EXCEED_TO_ZERO", third.ActionType));
    }

    [Fact]
    public async Task MatchAsync_DefaultOrderMatchesHisLimitThenDiscountThenTopPrice()
    {
        var rule = new RuleHeader
        {
            RuleId = 1,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1
        };
        var actions = new[]
        {
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "APPLY_MAX_AMOUNT", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "FORMULA_CALC", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "APPLY_TIME_WINDOW_LIMIT", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "DISCOUNT_EXCEED_TO_ZERO", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "CONVERT_QTY", SortNo = 10, IsEnabled = "Y" }
        };
        var service = CreateRuleMatchService(
            new FixedRuleHeaderRepository(rule),
            new EmptyRuleConditionRepository(),
            new FixedRuleActionRepository(actions),
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()),
            new FixedDictRepository(Array.Empty<Dict>()),
            NullLogger<RuleMatchService>.Instance);

        var (_, orderedActions) = await service.MatchAsync(new PricingContext
        {
            ItemCode = "ITEM001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        });

        Assert.Collection(
            orderedActions,
            first => Assert.Equal("CONVERT_QTY", first.ActionType),
            second => Assert.Equal("APPLY_TIME_WINDOW_LIMIT", second.ActionType),
            third => Assert.Equal("FORMULA_CALC", third.ActionType),
            fourth => Assert.Equal("APPLY_MAX_AMOUNT", fourth.ActionType),
            fifth => Assert.Equal("DISCOUNT_EXCEED_TO_ZERO", fifth.ActionType));
    }

    [Fact]
    public async Task MatchAsync_DictOrderMatchesHisLimitThenDiscountThenTopPrice()
    {
        var rule = new RuleHeader
        {
            RuleId = 1,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1
        };
        var actions = new[]
        {
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "APPLY_MAX_AMOUNT", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "FORMULA_CALC", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "APPLY_TIME_WINDOW_LIMIT", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "DISCOUNT_EXCEED_TO_ZERO", SortNo = 10, IsEnabled = "Y" },
            new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "CONVERT_QTY", SortNo = 10, IsEnabled = "Y" }
        };
        var dictItems = new[]
        {
            new Dict { DictType = "ACTION_TYPE_ORDER", DictCode = "CONVERT_QTY", SortNo = 10, IsEnabled = "Y" },
            new Dict { DictType = "ACTION_TYPE_ORDER", DictCode = "APPLY_TIME_WINDOW_LIMIT", SortNo = 20, IsEnabled = "Y" },
            new Dict { DictType = "ACTION_TYPE_ORDER", DictCode = "FORMULA_CALC", SortNo = 30, IsEnabled = "Y" },
            new Dict { DictType = "ACTION_TYPE_ORDER", DictCode = "APPLY_MAX_AMOUNT", SortNo = 40, IsEnabled = "Y" },
            new Dict { DictType = "ACTION_TYPE_ORDER", DictCode = "DISCOUNT_EXCEED_TO_ZERO", SortNo = 50, IsEnabled = "Y" }
        };
        var service = CreateRuleMatchService(
            new FixedRuleHeaderRepository(rule),
            new EmptyRuleConditionRepository(),
            new FixedRuleActionRepository(actions),
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()),
            new FixedDictRepository(dictItems),
            NullLogger<RuleMatchService>.Instance);

        var (_, orderedActions) = await service.MatchAsync(new PricingContext
        {
            ItemCode = "ITEM001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        });

        Assert.Collection(
            orderedActions,
            first => Assert.Equal("CONVERT_QTY", first.ActionType),
            second => Assert.Equal("APPLY_TIME_WINDOW_LIMIT", second.ActionType),
            third => Assert.Equal("FORMULA_CALC", third.ActionType),
            fourth => Assert.Equal("APPLY_MAX_AMOUNT", fourth.ActionType),
            fifth => Assert.Equal("DISCOUNT_EXCEED_TO_ZERO", fifth.ActionType));
    }

    [Fact]
    public async Task MatchAsync_ExecutesOnlyHighestPriorityActionInExclusiveGroup()
    {
        var highPriorityRule = new RuleHeader
        {
            RuleId = 1,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1,
            Priority = 1
        };
        var lowPriorityRule = new RuleHeader
        {
            RuleId = 2,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1,
            Priority = 9
        };
        var actionsByRule = new Dictionary<long, IReadOnlyList<RuleAction>>
        {
            [1] = new[]
            {
                new RuleAction
                {
                    RuleId = 1,
                    VersionNo = 1,
                    ActionId = 10,
                    ActionType = "FORMULA_CALC",
                    ExclusiveGroup = "FORMULA",
                    SortNo = 10,
                    IsEnabled = "Y"
                }
            },
            [2] = new[]
            {
                new RuleAction
                {
                    RuleId = 2,
                    VersionNo = 1,
                    ActionId = 20,
                    ActionType = "FORMULA_CALC",
                    ExclusiveGroup = "FORMULA",
                    SortNo = 1,
                    IsEnabled = "Y"
                }
            }
        };
        var service = CreateRuleMatchService(
            new FixedRuleHeaderRepository(highPriorityRule, lowPriorityRule),
            new EmptyRuleConditionRepository(),
            new FixedRuleActionRepository(actionsByRule),
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()),
            new FixedDictRepository(Array.Empty<Dict>()),
            NullLogger<RuleMatchService>.Instance);

        var (_, orderedActions) = await service.MatchAsync(new PricingContext
        {
            ItemCode = "ITEM001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        });

        var action = Assert.Single(orderedActions);
        Assert.Equal(1, action.RuleId);
    }

    [Fact]
    public async Task MatchAsync_OrdersSameActionTypeByRulePriorityBeforeSortNo()
    {
        var highPriorityRule = new RuleHeader
        {
            RuleId = 1,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1,
            Priority = 1
        };
        var lowPriorityRule = new RuleHeader
        {
            RuleId = 2,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1,
            Priority = 9
        };
        var actionsByRule = new Dictionary<long, IReadOnlyList<RuleAction>>
        {
            [1] = new[]
            {
                new RuleAction { RuleId = 1, VersionNo = 1, ActionType = "APPLY_MAX_AMOUNT", SortNo = 99, IsEnabled = "Y" }
            },
            [2] = new[]
            {
                new RuleAction { RuleId = 2, VersionNo = 1, ActionType = "APPLY_MAX_AMOUNT", SortNo = 1, IsEnabled = "Y" }
            }
        };
        var service = CreateRuleMatchService(
            new FixedRuleHeaderRepository(highPriorityRule, lowPriorityRule),
            new EmptyRuleConditionRepository(),
            new FixedRuleActionRepository(actionsByRule),
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()),
            new FixedDictRepository(Array.Empty<Dict>()),
            NullLogger<RuleMatchService>.Instance);

        var (_, orderedActions) = await service.MatchAsync(new PricingContext
        {
            ItemCode = "ITEM001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        });

        Assert.Collection(
            orderedActions,
            first => Assert.Equal(1, first.RuleId),
            second => Assert.Equal(2, second.RuleId));
    }

    [Fact]
    public async Task MatchAsync_ThrowsWhenActionTypeIsNotRegisteredInOrderDictionary()
    {
        var rule = new RuleHeader
        {
            RuleId = 1,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1
        };
        var service = CreateRuleMatchService(
            new FixedRuleHeaderRepository(rule),
            new EmptyRuleConditionRepository(),
            new FixedRuleActionRepository(new[]
            {
                new RuleAction
                {
                    RuleId = 1,
                    VersionNo = 1,
                    ActionType = "NEW_UNREGISTERED_ACTION",
                    SortNo = 10,
                    IsEnabled = "Y"
                }
            }),
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()),
            new FixedDictRepository(Array.Empty<Dict>()),
            NullLogger<RuleMatchService>.Instance);
        service.ClearActionTypeOrderCache();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.MatchAsync(new PricingContext
            {
                ItemCode = "ITEM001",
                BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
            }));

        Assert.Contains("NEW_UNREGISTERED_ACTION", ex.Message);
        Assert.Contains("ACTION_TYPE_ORDER", ex.Message);
    }

    [Fact]
    public async Task MatchAsync_ThrowsWhenActionTypeOrderDictionaryCannotBeLoaded()
    {
        var rule = new RuleHeader
        {
            RuleId = 1,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1
        };
        var service = CreateRuleMatchService(
            new FixedRuleHeaderRepository(rule),
            new EmptyRuleConditionRepository(),
            new FixedRuleActionRepository(new[]
            {
                new RuleAction
                {
                    RuleId = 1,
                    VersionNo = 1,
                    ActionType = "FORMULA_CALC",
                    SortNo = 10,
                    IsEnabled = "Y"
                }
            }),
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()),
            new ThrowingDictRepository(),
            NullLogger<RuleMatchService>.Instance);
        service.ClearActionTypeOrderCache();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.MatchAsync(new PricingContext
            {
                ItemCode = "ITEM001",
                BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
            }));

        Assert.Contains("ACTION_TYPE_ORDER", ex.Message);
    }

    private static RuleMatchService CreateRuleMatchService(
        IRuleHeaderRepository headerRepository,
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        ConditionEvaluatorFactory evaluatorFactory,
        IDictRepository dictRepository,
        ILogger<RuleMatchService> logger) =>
        new(
            new EffectiveRuleViewCache(
                new MemoryCache(new MemoryCacheOptions()),
                new EffectiveRuleReader(
                    new RuleMatchRepositories(
                        headerRepository,
                        conditionRepository,
                        actionRepository,
                        dictRepository))),
            new RuleConditionGroupMatcher(
                evaluatorFactory,
                NullLogger<RuleConditionGroupMatcher>.Instance),
            new RuleActionPlanBuilder(
                dictRepository,
                NullLogger<RuleActionPlanBuilder>.Instance),
            logger);

    private sealed class FixedRuleHeaderRepository : IRuleHeaderRepository
    {
        private readonly IReadOnlyList<RuleHeader> _rules;

        public FixedRuleHeaderRepository(params RuleHeader[] rules)
        {
            _rules = rules;
        }

        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(null);
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) =>
            Task.FromResult((IReadOnlyList<RuleHeader>)_rules.Where(r => r.ItemCode == itemCode).ToList());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(
            string? itemCode, string? status, string? category, int pageIndex, int pageSize) =>
            Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) =>
            Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
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

    private sealed class FixedRuleActionRepository : IRuleActionRepository
    {
        private readonly IReadOnlyDictionary<long, IReadOnlyList<RuleAction>> _actionsByRule;

        public FixedRuleActionRepository(IReadOnlyList<RuleAction> actions)
        {
            _actionsByRule = new Dictionary<long, IReadOnlyList<RuleAction>>
            {
                [actions.FirstOrDefault()?.RuleId ?? 0] = actions
            };
        }

        public FixedRuleActionRepository(IReadOnlyDictionary<long, IReadOnlyList<RuleAction>> actionsByRule)
        {
            _actionsByRule = actionsByRule;
        }

        public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult(_actionsByRule.TryGetValue(ruleId, out var actions)
                ? actions
                : Array.Empty<RuleAction>());
        public Task InsertBatchAsync(IReadOnlyList<RuleAction> entities) => Task.CompletedTask;
        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
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

    private sealed class ThrowingDictRepository : IDictRepository
    {
        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) =>
            throw new InvalidOperationException($"无法读取 {dictType}");

        public Task<Dict?> GetByIdAsync(long dictId) => Task.FromResult<Dict?>(null);
        public Task<IReadOnlyList<string>> GetAllTypesAsync() =>
            Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());
        public Task<long> InsertAsync(Dict entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(Dict entity) => Task.FromResult(false);
        public Task<bool> SetEnabledAsync(long dictId, string isEnabled) => Task.FromResult(false);
        public Task<bool> ExistsAsync(string dictType, string dictCode) => Task.FromResult(false);
    }
}
