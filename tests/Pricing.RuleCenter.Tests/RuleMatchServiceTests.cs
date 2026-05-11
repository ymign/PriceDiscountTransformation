using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Core.Engine;
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
        var service = new RuleMatchService(
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

    private sealed class FixedRuleHeaderRepository : IRuleHeaderRepository
    {
        private readonly RuleHeader _rule;

        public FixedRuleHeaderRepository(RuleHeader rule)
        {
            _rule = rule;
        }

        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(null);
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) =>
            Task.FromResult((IReadOnlyList<RuleHeader>)new[] { _rule });
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(
            string? itemCode, string? status, string? category, int pageIndex, int pageSize) =>
            Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) =>
            Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleHeader entity) => Task.FromResult(false);
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
        private readonly IReadOnlyList<RuleAction> _actions;

        public FixedRuleActionRepository(IReadOnlyList<RuleAction> actions)
        {
            _actions = actions;
        }

        public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult(_actions);
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
}
