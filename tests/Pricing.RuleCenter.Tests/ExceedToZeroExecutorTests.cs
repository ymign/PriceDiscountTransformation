using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ExceedToZeroExecutorTests
{
    [Fact]
    public async Task ReplaceMode_ProcessesFullExceededQty()
    {
        var executor = new ExceedToZeroExecutor();
        var context = new PricingContext
        {
            InputQty = 4m,
            ConvertedQty = 4m,
            FinalQty = 0m,
            UnitPrice = 10m,
            FinalAmount = 0m
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "DISCOUNT_EXCEED_TO_ZERO",
            ParamsJson = JsonConvert.SerializeObject(new
            {
                exceedAction = "REPLACE",
                replaceItemCode = "CT_ADD",
                replaceItemName = "CT加收",
                replaceUnitPrice = 6m
            })
        }, context);

        Assert.Equal(4m, context.ExceedQty);
        Assert.Equal(24m, context.FinalAmount);
        Assert.NotNull(context.ReplaceChildResult);
        Assert.Equal("CT_ADD", context.ReplaceChildResult.ItemCode);
        Assert.Equal(4m, context.ReplaceChildResult.Qty);
        Assert.Equal(24m, context.ReplaceChildResult.Amount);
    }

    [Fact]
    public async Task ConvertQty_DoesNotTriggerExceedHandling()
    {
        var executor = new ExceedToZeroExecutor();
        var context = new PricingContext
        {
            InputQty = 10m,
            ConvertedQty = 3m,
            FinalQty = 3m,
            UnitPrice = 10m,
            FinalAmount = 30m
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "DISCOUNT_EXCEED_TO_ZERO",
            ParamsJson = JsonConvert.SerializeObject(new
            {
                exceedAction = "REPLACE",
                replaceItemCode = "SKIN_ADD",
                replaceUnitPrice = 5m
            })
        }, context);

        Assert.Equal(0m, context.ExceedQty);
        Assert.Equal(30m, context.FinalAmount);
        Assert.Null(context.ReplaceChildResult);
    }

    [Fact]
    public async Task ZeroMode_DoesNotScaleAmountAfterLimitExecutor()
    {
        var executor = new ExceedToZeroExecutor();
        var context = new PricingContext
        {
            InputQty = 4m,
            ConvertedQty = 4m,
            FinalQty = 2m,
            UnitPrice = 10m,
            FinalAmount = 20m
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "DISCOUNT_EXCEED_TO_ZERO",
            ParamsJson = JsonConvert.SerializeObject(new { exceedAction = "ZERO" })
        }, context);

        Assert.Equal(2m, context.ExceedQty);
        Assert.Equal(20m, context.FinalAmount);
    }

    [Fact]
    public async Task PricingEngine_CopiesReplaceChildResultToResult()
    {
        var rule = new RuleHeader
        {
            RuleId = 101,
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1
        };
        var actions = new[]
        {
            new RuleAction
            {
                RuleId = 101,
                VersionNo = 1,
                ActionType = "APPLY_ONCE_LIMIT_QTY",
                SortNo = 10
            },
            new RuleAction
            {
                RuleId = 101,
                VersionNo = 1,
                ActionType = "DISCOUNT_EXCEED_TO_ZERO",
                SortNo = 20,
                ParamsJson = JsonConvert.SerializeObject(new
                {
                    exceedAction = "REPLACE",
                    replaceItemCode = "ITEM_ADD",
                    replaceItemName = "替换加收",
                    replaceUnitPrice = 5m
                })
            }
        };
        var ruleMatchService = new RuleMatchService(
            new FixedRuleHeaderRepository(rule),
            new EmptyRuleConditionRepository(),
            new FixedRuleActionRepository(actions),
            new ConditionEvaluatorFactory(Array.Empty<IRuleConditionEvaluator>()),
            NullLogger<RuleMatchService>.Instance);
        var pipeline = new ActionExecutionPipeline(
            new ActionExecutorFactory(new IRuleActionExecutor[]
            {
                new FullLimitExecutor(),
                new ExceedToZeroExecutor()
            }),
            NullLogger<ActionExecutionPipeline>.Instance);
        var engine = new PricingEngine(
            ruleMatchService,
            pipeline,
            NullLogger<PricingEngine>.Instance);

        var result = await engine.CalculateAsync(new PricingContext
        {
            PatientId = "P001",
            ItemCode = "ITEM001",
            InputQty = 4m,
            UnitPrice = 10m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        });

        Assert.NotNull(result.ReplaceChildResult);
        Assert.Equal("ITEM_ADD", result.ReplaceChildResult.ItemCode);
        Assert.Equal(4m, result.ExceedQty);
        Assert.Equal(20m, result.FinalAmount);
    }

    private sealed class FullLimitExecutor : IRuleActionExecutor
    {
        public string ActionType => "APPLY_ONCE_LIMIT_QTY";

        public Task ExecuteAsync(RuleAction action, PricingContext context)
        {
            context.FinalQty = 0m;
            context.FinalAmount = 0m;
            return Task.CompletedTask;
        }
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
}
