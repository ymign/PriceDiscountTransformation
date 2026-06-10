using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Engine.RuleRuntimeSnapshot;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Infrastructure;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class SameGroupMutexBatchTests
{
    [Fact]
    public async Task CalculateAsync_should_block_second_item_in_same_batch_for_same_exclusive_group()
    {
        var headerRepository = new InMemoryRuleHeaderRepository(
            CreateRule(1, "RULE_ITEM_A", itemCode: "ITEM_A"),
            CreateRule(2, "RULE_ITEM_B", itemCode: "ITEM_B"));

        var actionRepository = new StubRuleActionRepository(
            CreateSameGroupAction(1, "ZT_01"),
            CreateSameGroupAction(2, "ZT_01"),
            CreateExceedToZeroAction(1),
            CreateExceedToZeroAction(2));

        var repositories = new RuleMatchRepositories(
            headerRepository,
            new StubRuleConditionRepository(),
            actionRepository,
            new StubDictRepository());
        var engine = new PricingEngine(
            CreateRuleMatchService(
                repositories,
                new ConditionEvaluatorFactory(Array.Empty<Pricing.RuleCenter.Core.Interfaces.IRuleConditionEvaluator>())),
            new ActionExecutionPipeline(
                new ActionExecutorFactory(new Pricing.RuleCenter.Core.Interfaces.IRuleActionExecutor[]
                {
                    new SameGroupMutexExecutor(),
                    new ExceedToZeroExecutor()
                }),
                NullLogger<ActionExecutionPipeline>.Instance),
            CreateLimitOccupyValueFinalizers(),
            new SystemClock(),
            NullLogger<PricingEngine>.Instance);

        var sharedState = new RequestSharedPricingState();
        var firstContext = CreateContext("ITEM_A", sharedState);
        var first = await engine.CalculateAsync(firstContext);
        sharedState.Accumulate(first, firstContext);

        var second = await engine.CalculateAsync(CreateContext("ITEM_B", sharedState));

        Assert.Equal(1m, first.FinalQty);
        Assert.Equal(0m, second.FinalQty);
        Assert.Equal(0m, second.FinalAmount);
    }

    private static PricingContext CreateContext(string itemCode, RequestSharedPricingState sharedState)
    {
        return new PricingContext
        {
            PatientId = "P001",
            ItemCode = itemCode,
            InputQty = 1,
            UnitPrice = 100,
            SourceSystem = "HIS",
            BusinessRequestNo = $"REQ_{itemCode}",
            BusinessChargeTime = new DateTime(2026, 5, 14, 9, 0, 0),
            RequestSharedState = sharedState
        };
    }

    private static RuleMatchService CreateRuleMatchService(
        RuleMatchRepositories repositories,
        ConditionEvaluatorFactory evaluatorFactory)
    {
        var snapshotCache = new EffectiveRuleSnapshotCache(
            new MemoryCache(new MemoryCacheOptions()),
            new EffectiveRuleSnapshotLoader(repositories),
            repositories.RuntimePackageTraceContextAccessor);
        var matcher = new RuleConditionGroupMatcher(
            evaluatorFactory,
            NullLogger<RuleConditionGroupMatcher>.Instance);
        var actionPlanBuilder = new RuleActionPlanBuilder(
            repositories.DictRepository,
            NullLogger<RuleActionPlanBuilder>.Instance);
        return new RuleMatchService(
            snapshotCache,
            matcher,
            actionPlanBuilder,
            NullLogger<RuleMatchService>.Instance);
    }

    private static ILimitOccupyValueFinalizer[] CreateLimitOccupyValueFinalizers()
    {
        return new ILimitOccupyValueFinalizer[]
        {
            new SameGroupLimitOccupyValueFinalizer(),
            new SameOperationLimitOccupyValueFinalizer(),
            new DefaultLimitOccupyValueFinalizer()
        };
    }

    private static RuleAggregate CreateRule(long ruleId, string ruleCode, string itemCode)
    {
        return new RuleAggregate
        {
            RuleId = ruleId,
            RuleCode = ruleCode,
            RuleName = ruleCode,
            RuleCategory = "MIXED",
            RuleScope = "ITEM",
            ItemCode = itemCode,
            Priority = 10,
            CurrentVersion = 1,
            Status = "PUBLISHED",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 1, 1),
            EffectiveTo = new DateTime(2026, 12, 31)
        };
    }

    private static RuleAction CreateSameGroupAction(long ruleId, string exclusiveGroup)
    {
        return new RuleAction
        {
            ActionId = ruleId * 10 + 1,
            RuleId = ruleId,
            VersionNo = 1,
            ActionType = "SAME_GROUP_MUTEX",
            ExecutorCode = "SameGroupMutexExecutor",
            ParamsJson = "{\"GroupDimension\":\"EXCLUSIVE_GROUP\",\"MaxCountPerGroup\":1}",
            ExclusiveGroup = exclusiveGroup,
            SortNo = 10,
            OnError = "STOP",
            IsEnabled = "Y"
        };
    }

    private static RuleAction CreateExceedToZeroAction(long ruleId)
    {
        return new RuleAction
        {
            ActionId = ruleId * 10 + 2,
            RuleId = ruleId,
            VersionNo = 1,
            ActionType = "DISCOUNT_EXCEED_TO_ZERO",
            ExecutorCode = "ExceedToZeroExecutor",
            SortNo = 20,
            OnError = "STOP",
            IsEnabled = "Y"
        };
    }
}
