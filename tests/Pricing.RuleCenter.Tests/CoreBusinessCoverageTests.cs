using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.Commands;
using Pricing.RuleCenter.Application.Pricing.Queries;
using Pricing.RuleCenter.Application.Pricing.Validation;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Aggregates.Quota;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Engine;
using Pricing.RuleCenter.Core.Engine.Evaluators;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Events;
using Pricing.RuleCenter.Core.Exceptions;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.ValueObjects;
using Xunit;

namespace Pricing.RuleCenter.Tests;

/// <summary>
/// 核心业务对象、评估器、执行器和 CQRS 验证器的补充覆盖测试。
/// </summary>
public sealed class CoreBusinessCoverageTests
{
    [Fact]
    public void Money_EncapsulatesDecimalArithmeticRoundingAndFormatting()
    {
        var amount = new Money(1.005m);
        var rounded = amount.Round();
        var sum = new Money(10m) + new Money(2.5m);
        var difference = new Money(10m) - new Money(2.5m);
        var product = new Money(2.5m) * 4m;

        Assert.Equal(1.01m, rounded.Value);
        Assert.Equal(12.5m, sum.Value);
        Assert.Equal(7.5m, difference.Value);
        Assert.Equal(10m, product.Value);
        Assert.True(new Money(11m) > new Money(10m));
        Assert.True(new Money(9m) < new Money(10m));
        Assert.True(new Money(10m) >= new Money(10m));
        Assert.True(new Money(10m) <= new Money(10m));
        Assert.True(Money.Zero.IsZero);
        Assert.Equal(3.4567m, (decimal)(Money)3.4567m);
        Assert.Equal("3.4567", ((Money)3.4567m).ToString());
    }

    [Fact]
    public void EffectivePeriod_EvaluatesOpenClosedRangesAndOverlaps()
    {
        var period = new EffectivePeriod(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.True(period.IsEffectiveAt(new DateTime(2026, 1, 1)));
        Assert.True(period.IsEffectiveAt(new DateTime(2026, 1, 31)));
        Assert.False(period.IsEffectiveAt(new DateTime(2025, 12, 31)));
        Assert.False(period.IsEffectiveAt(new DateTime(2026, 2, 1)));
        Assert.True(period.Overlaps(new EffectivePeriod(new DateTime(2026, 1, 15), null)));
        Assert.False(period.Overlaps(new EffectivePeriod(new DateTime(2026, 2, 1), null)));
        Assert.Equal("[2026-01-01 ~ 2026-01-31]", period.ToString());
        Assert.Equal("[∞ ~ ∞]", new EffectivePeriod(null, null).ToString());
    }

    [Fact]
    public void IdempotencyKey_UsesStableTupleStringAndValueEquality()
    {
        var left = new IdempotencyKey("HIS", "BIZ-001", PricingCallTypeCodes.Confirm);
        var right = new IdempotencyKey("HIS", "BIZ-001", PricingCallTypeCodes.Confirm);

        Assert.Equal(left, right);
        Assert.Equal("HIS|BIZ-001|CONFIRM", left.ToString());
        Assert.Equal("HIS", left.SourceSystem);
        Assert.Equal("BIZ-001", left.BusinessRequestNo);
        Assert.Equal(PricingCallTypeCodes.Confirm, left.CallType);
    }

    [Fact]
    public void DomainEventsAndDomainExceptions_ExposeConstructorValues()
    {
        var occurredAt = new DateTime(2026, 5, 10, 8, 30, 0);

        var confirmed = new PricingConfirmedEvent(10, "TRACE-1", "P001", "ITEM001", occurredAt);
        var published = new RulePublishedEvent(20, 2, occurredAt);
        var disabled = new RuleDisabledEvent(30, "重复配置", occurredAt);
        var notFound = new NotFoundException("规则不存在");

        Assert.Equal(10, confirmed.RequestId);
        Assert.Equal("TRACE-1", confirmed.TraceId);
        Assert.Equal("P001", confirmed.PatientId);
        Assert.Equal("ITEM001", confirmed.ItemCode);
        Assert.Equal(occurredAt, confirmed.OccurredAt);
        Assert.Equal(20, published.RuleHeaderId);
        Assert.Equal(2, published.VersionNo);
        Assert.Equal(occurredAt, published.OccurredAt);
        Assert.Equal(30, disabled.RuleHeaderId);
        Assert.Equal("重复配置", disabled.Reason);
        Assert.Equal(occurredAt, disabled.OccurredAt);
        Assert.Equal(404, notFound.Code);
        Assert.Equal("规则不存在", notFound.Message);
    }

    [Fact]
    public void RuleAggregate_LifecycleMethodsMaintainInvariantsAndEvents()
    {
        var publishedAt = new DateTime(2026, 5, 10, 8, 30, 0);
        var republishedAt = publishedAt.AddHours(1);
        var disabledAt = publishedAt.AddHours(2);
        var rollbackAt = publishedAt.AddHours(3);
        var rule = new RuleAggregate
        {
            RuleId = 1,
            RuleCode = "RULE-001",
            Status = RuleStatusCodes.Draft,
            IsEnabled = EnableFlag.Yes
        };

        rule.Publish(1, publishedAt);
        Assert.Equal(RuleStatusCodes.Published, rule.Status);
        Assert.Equal(1, rule.CurrentVersion);
        var published = Assert.IsType<RulePublishedEvent>(Assert.Single(rule.DomainEvents));
        Assert.Equal(publishedAt, rule.UpdatedAt);
        Assert.Equal(publishedAt, published.OccurredAt);

        rule.ClearDomainEvents();
        Assert.Empty(rule.DomainEvents);

        rule.Publish(2, republishedAt);
        Assert.Equal(2, rule.CurrentVersion);
        Assert.Equal(RuleStatusCodes.Published, rule.Status);
        Assert.Equal(republishedAt, rule.UpdatedAt);

        rule.Disable("配置冲突", disabledAt);
        Assert.Equal(RuleStatusCodes.Disabled, rule.Status);
        Assert.Equal(EnableFlag.No, rule.IsEnabled);
        var disabled = Assert.IsType<RuleDisabledEvent>(rule.DomainEvents.Last());
        Assert.Equal(disabledAt, rule.UpdatedAt);
        Assert.Equal(disabledAt, disabled.OccurredAt);
        Assert.Throws<InvalidOperationException>(() => rule.Disable(null, disabledAt.AddMinutes(1)));
        Assert.Throws<InvalidOperationException>(() => rule.Publish(3, republishedAt.AddMinutes(1)));

        rule.Rollback(1, rollbackAt);
        Assert.Equal(RuleStatusCodes.Published, rule.Status);
        Assert.Equal(1, rule.CurrentVersion);
        Assert.Equal(rollbackAt, rule.UpdatedAt);
        var rolledBack = Assert.IsType<RulePublishedEvent>(rule.DomainEvents.Last());
        Assert.Equal(rollbackAt, rolledBack.OccurredAt);
    }

    [Fact]
    public void LimitOccupy_StateMachineAllowsOnlyLegalTransitions()
    {
        var confirmedAt = new DateTime(2026, 5, 10, 9, 0, 0);
        var confirmed = new LimitOccupy { OccupyId = 1, Status = OccupyStatusCodes.Pending };
        confirmed.Confirm(confirmedAt);
        Assert.Equal(OccupyStatusCodes.Confirmed, confirmed.Status);
        Assert.Equal(confirmedAt, confirmed.ConfirmedAt);
        Assert.Throws<InvalidOperationException>(() => confirmed.Cancel());

        var cancelled = new LimitOccupy { OccupyId = 2, Status = OccupyStatusCodes.Pending };
        cancelled.Cancel();
        Assert.Equal(OccupyStatusCodes.Cancelled, cancelled.Status);
        Assert.Throws<InvalidOperationException>(() => cancelled.Confirm(confirmedAt.AddMinutes(1)));

        var reversed = new LimitOccupy { OccupyId = 3, Status = OccupyStatusCodes.Confirmed };
        reversed.Reverse();
        Assert.Equal(OccupyStatusCodes.Reversed, reversed.Status);
        Assert.Throws<InvalidOperationException>(() => new LimitOccupy { OccupyId = 4, Status = OccupyStatusCodes.Pending }.Reverse());

        var expired = new LimitOccupy { OccupyId = 5, Status = OccupyStatusCodes.Pending };
        expired.Expire();
        Assert.Equal(OccupyStatusCodes.Expired, expired.Status);
        Assert.Throws<InvalidOperationException>(() => new LimitOccupy { OccupyId = 6, Status = OccupyStatusCodes.Confirmed }.Expire());

        expired.DomainEvents.Add(new RuleDisabledEvent(1, "test", DateTime.Now));
        expired.ClearDomainEvents();
        Assert.Empty(expired.DomainEvents);
    }

    [Fact]
    public void ChargeRequest_StateMachineCoversCommitCancelExpireReverseAndInvalidBranches()
    {
        var baseTime = new DateTime(2026, 5, 10, 10, 0, 0);
        var pending = new ChargeRequest { RequestNo = "REQ-001" };
        pending.MarkConfirmPending(baseTime);
        Assert.Equal(BusinessStatusCodes.ConfirmPending, pending.BusinessStatus);
        Assert.Equal(EnableFlag.Yes, pending.IsSuccess);
        Assert.Equal(baseTime, pending.ResponseAt);

        var committed = new ChargeRequest { RequestNo = "REQ-002", BusinessStatus = BusinessStatusCodes.ConfirmPending };
        committed.MarkCommitted(baseTime.AddMinutes(1));
        Assert.Equal(BusinessStatusCodes.Confirmed, committed.BusinessStatus);
        Assert.Equal(baseTime.AddMinutes(1), committed.ResponseAt);
        Assert.Throws<InvalidOperationException>(() => committed.MarkCancelled(baseTime.AddMinutes(1)));

        var cancelled = new ChargeRequest { RequestNo = "REQ-003", BusinessStatus = BusinessStatusCodes.ConfirmPending };
        cancelled.MarkCancelled(baseTime.AddMinutes(2));
        Assert.Equal(BusinessStatusCodes.Cancelled, cancelled.BusinessStatus);
        Assert.Equal(baseTime.AddMinutes(2), cancelled.ResponseAt);

        var expired = new ChargeRequest { RequestNo = "REQ-004", BusinessStatus = BusinessStatusCodes.ConfirmPending };
        expired.MarkExpired(baseTime.AddMinutes(3));
        Assert.Equal(BusinessStatusCodes.Expired, expired.BusinessStatus);
        Assert.Equal(baseTime.AddMinutes(3), expired.ResponseAt);

        var reversed = new ChargeRequest { RequestNo = "REQ-005", BusinessStatus = BusinessStatusCodes.Committed };
        reversed.MarkReversed(baseTime.AddMinutes(4));
        Assert.Equal(BusinessStatusCodes.Reversed, reversed.BusinessStatus);
        Assert.Equal(baseTime.AddMinutes(4), reversed.ResponseAt);
        Assert.Throws<InvalidOperationException>(() => new ChargeRequest { RequestNo = "REQ-006", BusinessStatus = BusinessStatusCodes.Cancelled }.MarkReversed(baseTime.AddMinutes(5)));
        Assert.Throws<InvalidOperationException>(() => new ChargeRequest { RequestNo = "REQ-007", BusinessStatus = BusinessStatusCodes.Cancelled }.MarkCommitted(baseTime.AddMinutes(6)));
        Assert.Throws<InvalidOperationException>(() => new ChargeRequest { RequestNo = "REQ-008", BusinessStatus = BusinessStatusCodes.Confirmed }.MarkExpired(baseTime.AddMinutes(7)));

        pending.DomainEvents.Add(new PricingConfirmedEvent(1, null, null, null, DateTime.Now));
        pending.ClearDomainEvents();
        Assert.Empty(pending.DomainEvents);
    }

    [Theory]
    [InlineData(null, 10, true)]
    [InlineData(">=0,<14", null, true)]
    [InlineData(">=0,<14", 13, true)]
    [InlineData(">=0,<14", 14, false)]
    [InlineData("<=60", 60, true)]
    [InlineData(">60", 60, false)]
    [InlineData("=7", 7, true)]
    [InlineData("bad", 7, false)]
    public async Task AgeMatchEvaluator_EvaluatesSupportedRangeExpressions(string? rightValue, int? age, bool expected)
    {
        var evaluator = new AgeMatchEvaluator();
        var condition = new RuleCondition { RightValue = rightValue };
        var context = new PricingContext { PatientAge = age };

        Assert.Equal(expected, evaluator.Evaluate(condition, context));
        Assert.Equal(expected, await evaluator.EvaluateAsync(condition, context));
        Assert.Equal("AGE_MATCH", evaluator.ConditionType);
    }

    [Fact]
    public async Task SimpleEvaluators_ApplyWildcardCaseInsensitiveAndConservativeRules()
    {
        var bodyPart = new BodyPartMatchEvaluator();
        var scene = new ChargeSceneMatchEvaluator();
        var item = new ItemMatchEvaluator();
        var visitType = new VisitTypeMatchEvaluator();
        var pregnancy = new PregnancyMatchEvaluator();
        var timeRange = new TimeRangeEvaluator();
        var context = new PricingContext
        {
            BodyPartCode = "head",
            ChargeScene = "outpatient",
            ItemCode = "item001",
            VisitType = "emergency",
            ExtraParams = new Dictionary<string, string> { ["pregnancyId"] = "PREG-1" },
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        };

        Assert.True(bodyPart.Evaluate(new RuleCondition { RightValue = null }, context));
        Assert.True(bodyPart.Evaluate(new RuleCondition { RightValue = "HEAD" }, context));
        Assert.False(bodyPart.Evaluate(new RuleCondition { RightValue = "LEG" }, context));
        Assert.True(scene.Evaluate(new RuleCondition { RightValue = null }, context));
        Assert.True(scene.Evaluate(new RuleCondition { RightValue = "OUTPATIENT" }, context));
        Assert.False(scene.Evaluate(new RuleCondition { RightValue = "INPATIENT" }, context));
        Assert.False(item.Evaluate(new RuleCondition { RightValue = null }, context));
        Assert.True(item.Evaluate(new RuleCondition { RightValue = "ITEM001" }, context));
        Assert.False(item.Evaluate(new RuleCondition { RightValue = "ITEM002" }, context));
        Assert.True(visitType.Evaluate(new RuleCondition { RightValue = null }, context));
        Assert.True(visitType.Evaluate(new RuleCondition { RightValue = "OUTPATIENT, EMERGENCY" }, context));
        Assert.False(visitType.Evaluate(new RuleCondition { RightValue = "INPATIENT" }, context));
        Assert.True(visitType.Evaluate(new RuleCondition { RightValue = "INPATIENT" }, new PricingContext()));
        Assert.True(pregnancy.Evaluate(new RuleCondition { RightValue = null }, context));
        Assert.True(pregnancy.Evaluate(new RuleCondition { RightValue = "preg-1" }, context));
        Assert.False(pregnancy.Evaluate(new RuleCondition { RightValue = "preg-2" }, context));
        Assert.True(pregnancy.Evaluate(new RuleCondition { RightValue = "preg-2" }, new PricingContext()));
        Assert.True(timeRange.Evaluate(new RuleCondition { RightValue = null }, context));
        Assert.True(timeRange.Evaluate(new RuleCondition { RightValue = "2026-05-10 09:00:00~2026-05-10 11:00:00" }, context));
        Assert.False(timeRange.Evaluate(new RuleCondition { RightValue = "2026-05-10 11:00:01~2026-05-10 12:00:00" }, context));
        Assert.False(timeRange.Evaluate(new RuleCondition { RightValue = "bad-format" }, context));
        Assert.False(timeRange.Evaluate(new RuleCondition { RightValue = "bad~2026-05-10" }, context));

        Assert.True(await bodyPart.EvaluateAsync(new RuleCondition { RightValue = "HEAD" }, context));
        Assert.True(await scene.EvaluateAsync(new RuleCondition { RightValue = "OUTPATIENT" }, context));
        Assert.True(await item.EvaluateAsync(new RuleCondition { RightValue = "ITEM001" }, context));
        Assert.True(await visitType.EvaluateAsync(new RuleCondition { RightValue = "EMERGENCY" }, context));
        Assert.True(await pregnancy.EvaluateAsync(new RuleCondition { RightValue = "PREG-1" }, context));
        Assert.True(await timeRange.EvaluateAsync(new RuleCondition { RightValue = "2026-05-10 09:00:00~2026-05-10 11:00:00" }, context));
    }

    [Fact]
    public async Task GroupMatchEvaluator_UsesFastPathRepositoryPathAndExceptionFallback()
    {
        var group = new ItemGroup { GroupId = 10, GroupCode = "GROUP-A" };
        var details = new[] { new ItemGroupDetail { GroupId = 10, ItemCode = "ITEM001" } };
        var evaluator = new GroupMatchEvaluator(
            new InMemoryItemGroupRepository(group),
            new InMemoryItemGroupDetailRepository(details));

        Assert.True(await evaluator.EvaluateAsync(new RuleCondition { RightValue = null }, new PricingContext()));
        Assert.False(await evaluator.EvaluateAsync(new RuleCondition { RightValue = "GROUP-A" }, new PricingContext()));
        Assert.True(await evaluator.EvaluateAsync(
            new RuleCondition { RightValue = "GROUP-A" },
            new PricingContext { ItemCode = "ITEM002", ItemGroupCode = "group-a" }));

        var context = new PricingContext { ItemCode = "item001" };
        Assert.True(await evaluator.EvaluateAsync(new RuleCondition { RightValue = "GROUP-A" }, context));
        Assert.Equal("GROUP-A", context.ItemGroupCode);
        Assert.False(await evaluator.EvaluateAsync(new RuleCondition { RightValue = "GROUP-MISSING" }, new PricingContext { ItemCode = "ITEM001" }));

        var throwing = new GroupMatchEvaluator(new ThrowingItemGroupRepository(), new InMemoryItemGroupDetailRepository(details));
        Assert.False(await throwing.EvaluateAsync(new RuleCondition { RightValue = "GROUP-A" }, new PricingContext { ItemCode = "ITEM001" }));
        Assert.Equal("GROUP_MATCH", evaluator.ConditionType);
    }

    [Fact]
    public async Task AmountLimitExecutors_ClampOnlyWhenConfiguredAndThresholdIsCrossed()
    {
        var ceiling = new AmountCeilingExecutor();
        var floor = new AmountFloorExecutor();

        var aboveCeiling = new PricingContext { FinalAmount = 120m, FinalQty = 3m };
        await ceiling.ExecuteAsync(new RuleAction { ParamsJson = JsonConvert.SerializeObject(new { MaxAmount = 100m }) }, aboveCeiling);
        Assert.Equal(100m, aboveCeiling.FinalAmount);
        Assert.Equal(3m, aboveCeiling.FinalQty);

        var belowCeiling = new PricingContext { FinalAmount = 80m };
        await ceiling.ExecuteAsync(new RuleAction { ParamsJson = JsonConvert.SerializeObject(new { CeilingAmount = 100m }) }, belowCeiling);
        Assert.Equal(80m, belowCeiling.FinalAmount);

        var aboveFloor = new PricingContext { FinalAmount = 120m };
        await floor.ExecuteAsync(new RuleAction { ParamsJson = JsonConvert.SerializeObject(new { MinAmount = 100m }) }, aboveFloor);
        Assert.Equal(120m, aboveFloor.FinalAmount);

        var belowFloor = new PricingContext { FinalAmount = 80m };
        await floor.ExecuteAsync(new RuleAction { ParamsJson = JsonConvert.SerializeObject(new { FloorAmount = 100m }) }, belowFloor);
        Assert.Equal(100m, belowFloor.FinalAmount);

        var missingCeiling = new PricingContext { FinalAmount = 120m };
        var missingFloor = new PricingContext { FinalAmount = 80m };
        await ceiling.ExecuteAsync(new RuleAction { ParamsJson = null }, missingCeiling);
        await floor.ExecuteAsync(new RuleAction { ParamsJson = "" }, missingFloor);
        Assert.Equal(120m, missingCeiling.FinalAmount);
        Assert.Equal(80m, missingFloor.FinalAmount);
        Assert.Equal("APPLY_MAX_AMOUNT", ceiling.ActionType);
        Assert.Equal("APPLY_MIN_AMOUNT", floor.ActionType);
    }

    [Fact]
    public async Task Factories_ReturnRegisteredStrategiesAndFallbacks()
    {
        var itemEvaluator = new ItemMatchEvaluator();
        var chargeSceneEvaluator = new ChargeSceneMatchEvaluator();
        var bodyPartEvaluator = new BodyPartMatchEvaluator();
        var evaluatorFactory = new ConditionEvaluatorFactory(new IRuleConditionEvaluator[]
        {
            itemEvaluator,
            chargeSceneEvaluator,
            bodyPartEvaluator
        });
        var ceiling = new AmountCeilingExecutor();
        var floor = new AmountFloorExecutor();
        var executorFactory = new ActionExecutorFactory(new IRuleActionExecutor[] { ceiling, floor });

        Assert.Same(itemEvaluator, evaluatorFactory.GetEvaluator(RuleConditionTypeCodes.ItemMatch.ToLowerInvariant()));
        Assert.Same(itemEvaluator, evaluatorFactory.GetEvaluator(RuleConditionTypeCodes.ItemCode));
        Assert.Same(chargeSceneEvaluator, evaluatorFactory.GetEvaluator(RuleConditionTypeCodes.ChargeSceneMatch));
        Assert.Same(bodyPartEvaluator, evaluatorFactory.GetEvaluator(RuleConditionTypeCodes.BodyPartMatch));
        Assert.Null(evaluatorFactory.GetEvaluator("missing"));
        Assert.Contains(ceiling, executorFactory.GetExecutors("apply_max_amount"));
        Assert.Empty(executorFactory.GetExecutors("missing"));
        Assert.Same(floor, executorFactory.GetExecutor("APPLY_MIN_AMOUNT"));
        Assert.Null(executorFactory.GetExecutor("missing"));

        var itemCodeEvaluator = evaluatorFactory.GetEvaluator(RuleConditionTypeCodes.ItemCode);
        var itemCodeCondition = new RuleCondition { ConditionType = RuleConditionTypeCodes.ItemCode, RightValue = "ITEM001" };
        Assert.True(await itemCodeEvaluator!.EvaluateAsync(itemCodeCondition, new PricingContext { ItemCode = "ITEM001" }));
    }

    [Fact]
    public void BatchPricingContext_AccumulatesLimitGroupOperationAndParentAmountState()
    {
        var batch = new BatchPricingContext();
        var result = new PricingResult
        {
            FinalQty = 2m,
            FinalAmount = 88m,
            LimitOccupies = new[]
            {
                new LimitOccupy
                {
                    LimitType = "DAY_QTY",
                    LimitDimensionCode = "patient:item:20260510",
                    OccupyQty = 2m,
                    OccupyAmt = 88m
                },
                new LimitOccupy { LimitType = "", LimitDimensionCode = "", OccupyQty = 99m, OccupyAmt = 99m },
                new LimitOccupy
                {
                    LimitType = "TIME_WINDOW",
                    LimitDimensionCode = "patient:item:window",
                    OccupyQty = 0m,
                    OccupyAmt = 0m
                }
            }
        };
        var context = new PricingContext
        {
            ItemCode = " item001 ",
            ItemGroupCode = " group-a ",
            ExtraParams = new Dictionary<string, string> { ["operationNo"] = " OP-1 " }
        };

        batch.AccumulateToBatch(result, context);

        Assert.Equal(2, batch.InBatchLimitOccupies.Count);
        Assert.Equal(2m, batch.InBatchOccupiedQtyByDimension["DAY_QTY:PATIENT:ITEM:20260510"]);
        Assert.Equal(88m, batch.InBatchOccupiedAmtByDimension["DAY_QTY:PATIENT:ITEM:20260510"]);
        Assert.Equal(1, batch.InBatchItemCountByGroup["GROUP-A"]);
        Assert.Equal(88m, batch.InBatchOccupiedAmtByOperation["OP-1:GROUP-A"]);
        Assert.Equal(88m, batch.InBatchOccupiedQtyByDimension["ITEM_AMT:ITEM001"]);
        Assert.Same(result, Assert.Single(batch.ProcessedResults));

        batch.AccumulateToBatch(
            new PricingResult { FinalQty = 0m, FinalAmount = 10m, LimitOccupies = Array.Empty<LimitOccupy>() },
            new PricingContext
            {
                ItemCode = "ITEM002",
                ItemGroupCode = "GROUP-B",
                ExtraParams = new Dictionary<string, string> { ["operationId"] = " " }
            });
        Assert.False(batch.InBatchItemCountByGroup.ContainsKey("GROUP-B"));
    }

    [Fact]
    public void PricingValidators_ReturnFieldLevelFailuresForEveryCommandAndQuery()
    {
        AssertInvalid(new SimulatePricingCommandValidator(), new SimulatePricingCommand(new PricingCalculateRequest()), "SourceSystem", "PatientId", "Items");
        AssertInvalid(new ConfirmPricingCommandValidator(), new ConfirmPricingCommand(CreateCalculateRequest(businessRequestNo: "")), "BusinessRequestNo");
        AssertInvalid(new CommitPricingCommandValidator(), new CommitPricingCommand(new PricingCommitRequest { RequestId = 0 }), "Request.RequestId");
        AssertInvalid(new CancelPricingCommandValidator(), new CancelPricingCommand(new PricingCancelRequest { RequestId = 0 }), "Request.RequestId");
        AssertInvalid(new ReversePricingCommandValidator(), new ReversePricingCommand(new PricingReverseRequest { OriginalRequestId = 0, ReverseNo = "" }), "Request.OriginalRequestId", "Request.ReverseNo");
        AssertInvalid(new GetSpecialFlagQueryValidator(), new GetSpecialFlagQuery(" "), "ItemCode");

        Assert.True(new SimulatePricingCommandValidator().Validate(new SimulatePricingCommand(CreateCalculateRequest())).IsValid);
        Assert.True(new ConfirmPricingCommandValidator().Validate(new ConfirmPricingCommand(CreateCalculateRequest())).IsValid);
        Assert.True(new CommitPricingCommandValidator().Validate(new CommitPricingCommand(new PricingCommitRequest { RequestId = 1 })).IsValid);
        Assert.True(new CancelPricingCommandValidator().Validate(new CancelPricingCommand(new PricingCancelRequest { RequestId = 1 })).IsValid);
        Assert.True(new ReversePricingCommandValidator().Validate(new ReversePricingCommand(new PricingReverseRequest { OriginalRequestId = 1, ReverseNo = "REV-1" })).IsValid);
        Assert.True(new GetSpecialFlagQueryValidator().Validate(new GetSpecialFlagQuery("ITEM001")).IsValid);
    }

    [Fact]
    public async Task SpecialFlagCacheKeysAndQueryHandler_ReturnCachedValueAndClearRegisteredKeys()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        SpecialFlagCacheKeys.Clear(cache);
        var key = SpecialFlagCacheKeys.Register(" item001 ");
        cache.Set(key, new SpecialFlagResponse
        {
            ItemCode = "ITEM001",
            IsSpecial = true,
            RuleCount = 2,
            RollbackMode = "STOP_CHARGE"
        });
        var handler = new GetSpecialFlagQueryHandler(null!, cache);

        var cached = await handler.Handle(new GetSpecialFlagQuery("ITEM001"), CancellationToken.None);

        Assert.True(cached.IsSpecial);
        Assert.Equal("ITEM001", cached.ItemCode);
        Assert.Equal(2, cached.RuleCount);
        var removed = SpecialFlagCacheKeys.Clear(cache);
        Assert.True(removed >= 0);
    }

    [Fact]
    public void PricingRequestFingerprintBuilder_NormalizesOrderPartsAndExtraValues()
    {
        var request = new PricingCalculateRequest
        {
            RequestNo = "REQ-001",
            BusinessRequestNo = "BIZ-001",
            SourceSystem = "HIS",
            PatientId = "P001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 0, 0),
            ExtraParams = new Dictionary<string, object?>
            {
                [" operationNo "] = " OP-1 ",
                ["pregnancyNo"] = 2.123456d,
                ["mainChargeDetailNo"] = new JValue(" MAIN-1 "),
                ["json"] = JObject.Parse("{\"b\":2,\"a\":1}"),
                ["none"] = null
            },
            Items = new[]
            {
                new PricingCalculateItemRequest
                {
                    ItemRequestNo = "2",
                    ChargeDetailNo = "CD002",
                    ItemCode = "ITEM002",
                    ItemName = " 项目2 ",
                    ItemGroupCode = " GROUP ",
                    InputQty = 2.123456m,
                    Unit = " 次 ",
                    UnitPrice = 3.456789m,
                    BusinessChargeTime = new DateTime(2026, 5, 10, 9, 1, 0),
                    BodyPartCode = " BODY ",
                    ExtraParams = new Dictionary<string, object?> { ["operationNo"] = 1.23456f },
                    PricingParts = new[]
                    {
                        new PricingPartItemRequest
                        {
                            PartSeq = 2,
                            PartCode = " P2 ",
                            PartName = " 片段2 ",
                            BodyPartCode = " BP2 ",
                            Qty = 1.23456m,
                            Area = 2.34567m,
                            MeasureType = " AREA ",
                            MeasureValue = 3.45678m,
                            MeasureUnit = " CM2 ",
                            LesionCount = 2
                        },
                        new PricingPartItemRequest { PartSeq = 1, PartCode = "P1", Qty = 1m }
                    }
                },
                new PricingCalculateItemRequest
                {
                    ItemRequestNo = "1",
                    ChargeDetailNo = "CD001",
                    ItemCode = "ITEM001",
                    InputQty = 1m,
                    UnitPrice = 10m
                }
            }
        };
        var reversedItems = request.Items.Reverse().ToList();

        var first = BuildConfirmFingerprint(request, request.Items);
        var second = BuildConfirmFingerprint(request, reversedItems);
        var reverseFingerprint = BuildReverseFingerprint(
            new PricingReverseRequest
            {
                OriginalRequestId = 9,
                ReverseNo = " REV-1 ",
                ChargeDetailNo = " CD001 ",
                ItemCode = " ITEM001 ",
                PartSeq = 1,
                ReverseQty = 1.23456m,
                ReverseAmt = 2.345m,
                ReversedBy = " OP ",
                Reason = " 退费 "
            },
            new ChargeRequest
            {
                SourceSystem = " HIS ",
                ChargeNo = " CHG-1 "
            },
            new DateTime(2026, 5, 11, 10, 0, 0));

        Assert.Equal(first, second);
        Assert.Equal(64, first.Length);
        Assert.Equal(64, reverseFingerprint.Length);
        Assert.Null(NormalizeExtraValue(null));
        Assert.Equal("text", NormalizeExtraValue(" text "));
        Assert.Equal(1.2346m, NormalizeExtraValue(1.23456m));
        Assert.Equal(1.2346m, NormalizeExtraValue(1.23456d));
        Assert.Equal(1.2346m, NormalizeExtraValue(1.23456f));
        Assert.Equal("json", NormalizeExtraValue(new JValue(" json ")));
        Assert.Equal("123", NormalizeExtraValue(new JValue(123)));
        Assert.Equal("{\"x\":1}", NormalizeExtraValue(JObject.Parse("{\"x\":1}")));
    }

    [Fact]
    public void PricingCommitActualValidator_RejectsAllMismatchShapesAndAcceptsValidDetails()
    {
        Assert.Equal(
            BizErrorCode.CommitDetailNotFound,
            InvokeCommitValidateExpectingBizException(new PricingCommitRequest { RequestId = 1 }, Array.Empty<ChargeDiscountDetail>(), false).Code);

        var details = new[]
        {
            CreateDiscountDetail("CD001", "ITEM001", finalQty: 1m, finalAmt: 10m)
        };

        Assert.Equal(
            BizErrorCode.CommitAmountMismatch,
            InvokeCommitValidateExpectingBizException(new PricingCommitRequest { RequestId = 1, ActualTotalAmount = 11m }, details, false).Code);
        Assert.Equal(
            BizErrorCode.CommitActualItemsRequired,
            InvokeCommitValidateExpectingBizException(new PricingCommitRequest { RequestId = 1 }, details, true).Code);
        Assert.Equal(
            BizErrorCode.CommitActualItemsRequired,
            InvokeCommitValidateExpectingBizException(new PricingCommitRequest
            {
                RequestId = 1,
                ActualItems = new[] { new PricingCommitActualItemRequest { ItemCode = " ", FinalQty = 1m, FinalAmount = 10m } }
            }, details, true).Code);
        Assert.Equal(
            BizErrorCode.CommitDetailMismatch,
            InvokeCommitValidateExpectingBizException(new PricingCommitRequest
            {
                RequestId = 1,
                ActualItems = new[] { new PricingCommitActualItemRequest { ChargeDetailNo = "CD404", ItemCode = "ITEM001", FinalQty = 1m, FinalAmount = 10m } }
            }, details, true).Code);
        Assert.Equal(
            BizErrorCode.CommitQtyMismatch,
            InvokeCommitValidateExpectingBizException(new PricingCommitRequest
            {
                RequestId = 1,
                ActualItems = new[] { new PricingCommitActualItemRequest { ChargeDetailNo = "CD001", ItemCode = "ITEM001", FinalQty = 2m, FinalAmount = 10m } }
            }, details, true).Code);
        Assert.Equal(
            BizErrorCode.CommitAmountMismatch,
            InvokeCommitValidateExpectingBizException(new PricingCommitRequest
            {
                RequestId = 1,
                ActualItems = new[] { new PricingCommitActualItemRequest { ChargeDetailNo = "CD001", ItemCode = "ITEM001", FinalQty = 1m, FinalAmount = 9m } }
            }, details, true).Code);

        var extra = InvokeCommitValidateExpectingBizException(new PricingCommitRequest
        {
            RequestId = 1,
            ActualItems = new[]
            {
                new PricingCommitActualItemRequest { ChargeDetailNo = "CD001", ItemCode = "ITEM001", FinalQty = 1m, FinalAmount = 10m },
                new PricingCommitActualItemRequest { ChargeDetailNo = "", ItemCode = "ITEM999", FinalQty = 1m, FinalAmount = 1m }
            }
        }, details, true);
        Assert.Equal(BizErrorCode.CommitDetailMismatch, extra.Code);
        Assert.Contains("ChargeDetailNo=-, ItemCode=ITEM999, PartSeq=-", extra.Message);

        var cumulativeRounding = InvokeCommitValidateExpectingBizException(new PricingCommitRequest
        {
            RequestId = 1,
            ActualItems = new[]
            {
                new PricingCommitActualItemRequest { ChargeDetailNo = "CD001", ItemCode = "ITEM001", FinalQty = 1m, FinalAmount = 0.01m },
                new PricingCommitActualItemRequest { ChargeDetailNo = "CD002", ItemCode = "ITEM002", FinalQty = 1m, FinalAmount = 0.01m }
            }
        }, new[]
        {
            CreateDiscountDetail("CD001", "ITEM001", finalQty: 1m, finalAmt: 0.005m),
            CreateDiscountDetail("CD002", "ITEM002", finalQty: 1m, finalAmt: 0.005m)
        }, true);
        Assert.Equal(BizErrorCode.CommitAmountMismatch, cumulativeRounding.Code);

        InvokeCommitValidate(new PricingCommitRequest
        {
            RequestId = 1,
            ActualTotalAmount = 10m,
            ActualItems = new[] { new PricingCommitActualItemRequest { ChargeDetailNo = "CD001", ItemCode = "ITEM001", FinalQty = 1m, FinalAmount = 10m } }
        }, details, true);
    }

    private static PricingCalculateRequest CreateCalculateRequest(string? businessRequestNo = "BIZ-001")
    {
        return new PricingCalculateRequest
        {
            RequestNo = "REQ-001",
            BusinessRequestNo = businessRequestNo,
            SourceSystem = "HIS",
            PatientId = "P001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 0, 0),
            Items = new[]
            {
                new PricingCalculateItemRequest
                {
                    ItemCode = "ITEM001",
                    InputQty = 1m,
                    UnitPrice = 10m
                }
            }
        };
    }

    private static void AssertInvalid<T>(IValidator<T> validator, T instance, params string[] propertyNames)
    {
        var result = validator.Validate(instance);

        Assert.False(result.IsValid);
        foreach (var propertyName in propertyNames)
        {
            Assert.Contains(result.Errors, error =>
                string.Equals(error.PropertyName, propertyName, StringComparison.Ordinal) ||
                error.PropertyName.EndsWith("." + propertyName, StringComparison.Ordinal));
        }
    }

    private static string BuildConfirmFingerprint(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items)
    {
        var type = GetApplicationInternalType("Pricing.RuleCenter.Application.Pricing.PricingRequestFingerprintBuilder");
        var method = type.GetMethod("BuildConfirmFingerprint", BindingFlags.Public | BindingFlags.Static)!;
        return (string)method.Invoke(null, new object?[] { request, items, PricingCallTypeCodes.Confirm })!;
    }

    private static string BuildReverseFingerprint(PricingReverseRequest request, ChargeRequest originalLog, DateTime reverseTime)
    {
        var type = GetApplicationInternalType("Pricing.RuleCenter.Application.Pricing.PricingRequestFingerprintBuilder");
        var method = type.GetMethod("BuildReverseFingerprint", BindingFlags.Public | BindingFlags.Static)!;
        return (string)method.Invoke(null, new object?[] { request, originalLog, reverseTime })!;
    }

    private static object? NormalizeExtraValue(object? value)
    {
        var type = GetApplicationInternalType("Pricing.RuleCenter.Application.Pricing.PricingRequestFingerprintBuilder");
        var method = type.GetMethod("NormalizeExtraValue", BindingFlags.Public | BindingFlags.Static)!;
        return method.Invoke(null, new[] { value });
    }

    private static void InvokeCommitValidate(
        PricingCommitRequest request,
        IReadOnlyList<ChargeDiscountDetail> details,
        bool requireActualItems)
    {
        var type = GetApplicationInternalType("Pricing.RuleCenter.Application.Pricing.PricingCommitActualValidator");
        var method = type.GetMethod("Validate", BindingFlags.Public | BindingFlags.Static)!;
        method.Invoke(null, new object?[] { request, details, requireActualItems });
    }

    private static BizException InvokeCommitValidateExpectingBizException(
        PricingCommitRequest request,
        IReadOnlyList<ChargeDiscountDetail> details,
        bool requireActualItems)
    {
        try
        {
            InvokeCommitValidate(request, details, requireActualItems);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is BizException bizException)
        {
            return bizException;
        }

        throw new InvalidOperationException("Expected BizException was not thrown.");
    }

    private static Type GetApplicationInternalType(string fullName)
    {
        return Type.GetType($"{fullName}, Pricing.RuleCenter.Application", throwOnError: true)!;
    }

    private static ChargeDiscountDetail CreateDiscountDetail(
        string? chargeDetailNo,
        string itemCode,
        decimal finalQty,
        decimal finalAmt)
    {
        return new ChargeDiscountDetail
        {
            ChargeDetailNo = chargeDetailNo,
            ItemCode = itemCode,
            FinalQty = finalQty,
            FinalAmt = finalAmt
        };
    }

    private sealed class InMemoryItemGroupRepository : IItemGroupRepository
    {
        private readonly ItemGroup _group;

        public InMemoryItemGroupRepository(ItemGroup group)
        {
            _group = group;
        }

        public Task<ItemGroup?> GetByIdAsync(long groupId) => Task.FromResult(groupId == _group.GroupId ? _group : null);

        public Task<IReadOnlyList<ItemGroup>> GetByTypeAsync(string groupType) => Task.FromResult((IReadOnlyList<ItemGroup>)Array.Empty<ItemGroup>());

        public Task<long> InsertAsync(ItemGroup entity) => Task.FromResult(entity.GroupId);

        public Task<ItemGroup?> GetByCodeAsync(string groupCode) =>
            Task.FromResult(string.Equals(groupCode, _group.GroupCode, StringComparison.OrdinalIgnoreCase) ? _group : null);

        public Task UpdateAsync(ItemGroup entity) => Task.CompletedTask;
    }

    private sealed class ThrowingItemGroupRepository : IItemGroupRepository
    {
        public Task<ItemGroup?> GetByIdAsync(long groupId) => throw new InvalidOperationException("boom");

        public Task<IReadOnlyList<ItemGroup>> GetByTypeAsync(string groupType) => throw new InvalidOperationException("boom");

        public Task<long> InsertAsync(ItemGroup entity) => throw new InvalidOperationException("boom");

        public Task<ItemGroup?> GetByCodeAsync(string groupCode) => throw new InvalidOperationException("boom");

        public Task UpdateAsync(ItemGroup entity) => throw new InvalidOperationException("boom");
    }

    private sealed class InMemoryItemGroupDetailRepository : IItemGroupDetailRepository
    {
        private readonly IReadOnlyList<ItemGroupDetail> _details;

        public InMemoryItemGroupDetailRepository(IReadOnlyList<ItemGroupDetail> details)
        {
            _details = details;
        }

        public Task<IReadOnlyList<ItemGroupDetail>> GetByGroupIdAsync(long groupId) =>
            Task.FromResult((IReadOnlyList<ItemGroupDetail>)_details.Where(d => d.GroupId == groupId).ToList());

        public Task<IReadOnlyList<ItemGroupDetail>> GetByItemCodeAsync(string itemCode) =>
            Task.FromResult((IReadOnlyList<ItemGroupDetail>)_details.Where(d => d.ItemCode == itemCode).ToList());

        public Task InsertBatchAsync(IReadOnlyList<ItemGroupDetail> entities) => Task.CompletedTask;

        public Task DeleteByGroupIdAsync(long groupId) => Task.CompletedTask;
    }
}
