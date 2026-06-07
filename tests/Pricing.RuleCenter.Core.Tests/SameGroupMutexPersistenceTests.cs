using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Infrastructure;
using Xunit;

namespace Pricing.RuleCenter.Core.Tests;

public sealed class SameGroupMutexPersistenceTests
{
    [Fact]
    public async Task CalculateAsync_should_apply_item_window_limit_and_cp_shared_group_limit_together()
    {
        var limitRepository = new InMemoryLimitOccupyRepository();
        var headerRepository = new InMemoryRuleHeaderRepository(
            CreateRule(1, "RF_ITEM_A", itemCode: "ITEM_A"),
            CreateRule(2, "CP_ITEM_A", itemCode: "ITEM_A"),
            CreateRule(3, "RF_ITEM_B", itemCode: "ITEM_B"),
            CreateRule(4, "CP_ITEM_B", itemCode: "ITEM_B"));

        var actionRepository = new StubRuleActionRepository(
            CreateTimeWindowAction(1),
            CreateExceedToZeroAction(1),
            CreateSameGroupAction(2, "CP01"),
            CreateExceedToZeroAction(2),
            CreateTimeWindowAction(3),
            CreateExceedToZeroAction(3),
            CreateSameGroupAction(4, "CP01"),
            CreateExceedToZeroAction(4));

        var engine = new PricingEngine(
            new RuleMatchService(
                new RuleMatchRepositories(
                    headerRepository,
                    new StubRuleConditionRepository(),
                    actionRepository,
                    new StubDictRepository()),
                new ConditionEvaluatorFactory(Array.Empty<Pricing.RuleCenter.Core.Interfaces.IRuleConditionEvaluator>()),
                NullLogger<RuleMatchService>.Instance),
            new ActionExecutionPipeline(
                new ActionExecutorFactory(new Pricing.RuleCenter.Core.Interfaces.IRuleActionExecutor[]
                {
                    new TimeWindowLimitExecutor(limitRepository),
                    new SameGroupMutexExecutor(limitRepository),
                    new ExceedToZeroExecutor()
                }),
                NullLogger<ActionExecutionPipeline>.Instance),
            new SystemClock(),
            NullLogger<PricingEngine>.Instance);

        var first = await engine.CalculateAsync(CreateContext("ITEM_A"));
        Assert.Equal(1m, first.FinalQty);
        Assert.Equal(100m, first.FinalAmount);
        Assert.Contains(first.LimitOccupies, o => o.LimitType == "TIME_WINDOW");
        Assert.Contains(first.LimitOccupies, o => o.LimitType == "SAME_GROUP" && o.LimitDimensionCode == "P001:CP01");

        limitRepository.Seed(first.LimitOccupies
            .Select(o => Clone(o, status: "CONFIRMED"))
            .ToArray());

        var second = await engine.CalculateAsync(CreateContext("ITEM_B"));

        Assert.Equal(0m, second.FinalQty);
        Assert.Equal(0m, second.FinalAmount);
        Assert.Empty(second.LimitOccupies);
    }

    [Fact]
    public async Task CalculateAsync_should_block_later_confirm_when_same_group_was_already_occupied()
    {
        var limitRepository = new InMemoryLimitOccupyRepository();
        var headerRepository = new InMemoryRuleHeaderRepository(
            CreateRule(1, "RULE_ITEM_A", itemCode: "ITEM_A"),
            CreateRule(2, "RULE_ITEM_B", itemCode: "ITEM_B"));

        var actionRepository = new StubRuleActionRepository(
            CreateSameGroupAction(1, "ZT_01"),
            CreateSameGroupAction(2, "ZT_01"),
            CreateExceedToZeroAction(1),
            CreateExceedToZeroAction(2));

        var engine = new PricingEngine(
            new RuleMatchService(
                new RuleMatchRepositories(
                    headerRepository,
                    new StubRuleConditionRepository(),
                    actionRepository,
                    new StubDictRepository()),
                new ConditionEvaluatorFactory(Array.Empty<Pricing.RuleCenter.Core.Interfaces.IRuleConditionEvaluator>()),
                NullLogger<RuleMatchService>.Instance),
            new ActionExecutionPipeline(
                new ActionExecutorFactory(new Pricing.RuleCenter.Core.Interfaces.IRuleActionExecutor[]
                {
                    new SameGroupMutexExecutor(limitRepository),
                    new ExceedToZeroExecutor()
                }),
                NullLogger<ActionExecutionPipeline>.Instance),
            new SystemClock(),
            NullLogger<PricingEngine>.Instance);

        var first = await engine.CalculateAsync(CreateContext("ITEM_A"));
        Assert.Single(first.LimitOccupies);
        Assert.Equal("SAME_GROUP", first.LimitOccupies[0].LimitType);
        Assert.Equal(1m, first.LimitOccupies[0].OccupyQty);

        limitRepository.Seed(Clone(first.LimitOccupies[0], status: "CONFIRMED"));

        var second = await engine.CalculateAsync(CreateContext("ITEM_B"));

        Assert.Equal(0m, second.FinalQty);
        Assert.Equal(0m, second.FinalAmount);
    }

    [Fact]
    public async Task CalculateAsync_should_not_block_same_group_when_previous_occupy_is_outside_two_hour_window()
    {
        var limitRepository = new InMemoryLimitOccupyRepository();
        var headerRepository = new InMemoryRuleHeaderRepository(
            CreateRule(1, "RULE_ITEM_A", itemCode: "ITEM_A"),
            CreateRule(2, "RULE_ITEM_B", itemCode: "ITEM_B"));

        var actionRepository = new StubRuleActionRepository(
            CreateSameGroupAction(1, "ZT_01"),
            CreateSameGroupAction(2, "ZT_01"),
            CreateExceedToZeroAction(1),
            CreateExceedToZeroAction(2));

        var engine = new PricingEngine(
            new RuleMatchService(
                new RuleMatchRepositories(
                    headerRepository,
                    new StubRuleConditionRepository(),
                    actionRepository,
                    new StubDictRepository()),
                new ConditionEvaluatorFactory(Array.Empty<Pricing.RuleCenter.Core.Interfaces.IRuleConditionEvaluator>()),
                NullLogger<RuleMatchService>.Instance),
            new ActionExecutionPipeline(
                new ActionExecutorFactory(new Pricing.RuleCenter.Core.Interfaces.IRuleActionExecutor[]
                {
                    new SameGroupMutexExecutor(limitRepository),
                    new ExceedToZeroExecutor()
                }),
                NullLogger<ActionExecutionPipeline>.Instance),
            new SystemClock(),
            NullLogger<PricingEngine>.Instance);

        var first = await engine.CalculateAsync(CreateContext("ITEM_A", new DateTime(2026, 5, 14, 6, 59, 0)));
        limitRepository.Seed(Clone(first.LimitOccupies[0], status: "CONFIRMED"));

        var second = await engine.CalculateAsync(CreateContext("ITEM_B", new DateTime(2026, 5, 14, 9, 0, 0)));

        Assert.Equal(1m, second.FinalQty);
        Assert.Equal(100m, second.FinalAmount);
    }

    private static PricingContext CreateContext(string itemCode, DateTime? businessChargeTime = null)
    {
        return new PricingContext
        {
            PatientId = "P001",
            ItemCode = itemCode,
            InputQty = 1,
            UnitPrice = 100,
            SourceSystem = "HIS",
            BusinessRequestNo = $"REQ_{itemCode}",
            BusinessChargeTime = businessChargeTime ?? new DateTime(2026, 5, 14, 9, 0, 0)
        };
    }

    private static LimitOccupy Clone(LimitOccupy source, string status)
    {
        return new LimitOccupy
        {
            OccupyId = source.OccupyId,
            RequestId = source.RequestId,
            TraceId = source.TraceId,
            PatientId = source.PatientId,
            ItemCode = source.ItemCode,
            RuleId = source.RuleId,
            RuleVersionNo = source.RuleVersionNo,
            LimitType = source.LimitType,
            LimitKey = source.LimitKey,
            OccupyQty = source.OccupyQty,
            OccupyAmt = source.OccupyAmt,
            OccupyType = source.OccupyType,
            OriginalOccupyId = source.OriginalOccupyId,
            BusinessChargeTime = source.BusinessChargeTime,
            LimitDimensionCode = source.LimitDimensionCode,
            PartSeq = source.PartSeq,
            Status = status,
            OccupiedAt = source.OccupiedAt,
            ConfirmedAt = source.ConfirmedAt,
            ExpireAt = source.ExpireAt
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

    private static RuleAction CreateTimeWindowAction(long ruleId)
    {
        return new RuleAction
        {
            ActionId = ruleId * 10,
            RuleId = ruleId,
            VersionNo = 1,
            ActionType = "APPLY_TIME_WINDOW_LIMIT",
            ExecutorCode = "TimeWindowLimitExecutor",
            ParamsJson = "{\"WindowMinutes\":120,\"LimitQty\":1}",
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
