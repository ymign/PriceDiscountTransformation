using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Pricing.RuleCenter.Api.Serialization;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;

using Pricing.RuleCenter.Application.Pricing.Persistence;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Application.Engine;
using Pricing.RuleCenter.Application.Engine.Evaluators;
using Pricing.RuleCenter.Application.Engine.EffectiveRules;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PricingApiServiceTests
{
    [Fact]
    public async Task SimulateAsync_RejectsMissingSourceSystem()
    {
        var service = CreateValidationService();
        var request = CreateValidCalculateRequest(sourceSystem: " ");

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.SimulateAsync(request));

        Assert.Contains("来源系统", ex.Message);
    }

    [Fact]
    public async Task SimulateAsync_RejectsNonPositiveInputQty()
    {
        var service = CreateValidationService();
        var request = CreateValidCalculateRequest(items: new[]
        {
            new PricingCalculateItemRequest
            {
                ItemCode = "ITEM001",
                InputQty = 0m,
                UnitPrice = 10m
            }
        });

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.SimulateAsync(request));

        Assert.Contains("数量必须大于0", ex.Message);
    }

    [Fact]
    public async Task SimulateAsync_RejectsTooManyItems()
    {
        var service = CreateValidationService();
        var request = CreateValidCalculateRequest(items: Enumerable.Range(1, 51)
            .Select(index => new PricingCalculateItemRequest
            {
                ItemCode = $"ITEM{index:000}",
                InputQty = 1m,
                UnitPrice = 10m
            })
            .ToList());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.SimulateAsync(request));

        Assert.Contains("50", ex.Message);
    }

    [Fact]
    public async Task SimulateAsync_RejectsNonPositivePricingPartQty()
    {
        var service = CreateValidationService();
        var request = CreateValidCalculateRequest(items: new[]
        {
            new PricingCalculateItemRequest
            {
                ItemCode = "ITEM001",
                InputQty = 1m,
                UnitPrice = 10m,
                PricingParts = new[]
                {
                    new PricingPartItemRequest
                    {
                        PartSeq = 1,
                        BodyPartCode = "HEAD",
                        Qty = 0m
                    }
                }
            }
        });

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.SimulateAsync(request));

        Assert.Contains("PricingParts", ex.Message);
        Assert.Contains("数量必须大于0", ex.Message);
    }

    [Fact]
    public async Task CommitAsync_RejectsInvalidRequestIdBeforeLookup()
    {
        var service = CreateValidationService();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CommitAsync(new PricingCommitRequest { RequestId = 0 }));

        Assert.Contains("RequestId", ex.Message);
    }

    [Fact]
    public async Task CancelAsync_RejectsInvalidRequestIdBeforeLookup()
    {
        var service = CreateValidationService();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CancelAsync(new PricingCancelRequest { RequestId = 0 }));

        Assert.Contains("RequestId", ex.Message);
    }

    [Fact]
    public async Task CommitAsync_ReturnsRequestNotFoundBizCodeWhenRequestIsMissing()
    {
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            new SpecialFlagRuleHeaderRepository(Array.Empty<RuleHeader>()),
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.CommitAsync(new PricingCommitRequest
            {
                RequestId = 999,
                ChargeNo = "C999",
                ActualItems = new[]
                {
                    new PricingCommitActualItemRequest
                    {
                        ItemCode = "ITEM999",
                        FinalQty = 1m,
                        FinalAmount = 10m
                    }
                }
            }));

        Assert.Equal(BizErrorCode.RequestNotFound, ex.Code);
    }

    [Fact]
    public async Task CancelAsync_ReturnsRequestNotFoundBizCodeWhenRequestIsMissing()
    {
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            new SpecialFlagRuleHeaderRepository(Array.Empty<RuleHeader>()),
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.CancelAsync(new PricingCancelRequest
            {
                RequestId = 999
            }));

        Assert.Equal(BizErrorCode.RequestNotFound, ex.Code);
    }

    [Fact]
    public async Task ReverseAsync_RejectsInvalidRequestBeforeLookup()
    {
        var service = CreateValidationService();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ReverseAsync(new PricingReverseRequest
            {
                OriginalRequestId = 1,
                ReverseNo = "R001",
                ReverseAmt = -1m
            }));

        Assert.Contains("退费金额", ex.Message);
    }

    [Fact]
    public async Task GetSpecialFlagAsync_RejectsEmptyItemCode()
    {
        var service = CreateValidationService();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.GetSpecialFlagAsync(" "));

        Assert.Contains("项目编码", ex.Message);
    }

    [Fact]
    public async Task GetSpecialFlagAsync_IgnoresRulesOutsideEffectiveRange()
    {
        var now = new DateTime(2026, 5, 10, 10, 0, 0);
        var repository = new SpecialFlagRuleHeaderRepository(new[]
        {
            new RuleHeader
            {
                RuleId = 1,
                RuleCode = "RULE-EXPIRED",
                RuleName = "已过期规则",
                ItemCode = "ITEM001",
                Status = "PUBLISHED",
                IsEnabled = "Y",
                EffectiveTo = now.AddDays(-1),
                CreatedAt = now.AddDays(-10),
                UpdatedAt = now.AddDays(-10)
            },
            new RuleHeader
            {
                RuleId = 2,
                RuleCode = "RULE-FUTURE",
                RuleName = "未来规则",
                ItemCode = "ITEM001",
                Status = "PUBLISHED",
                IsEnabled = "Y",
                EffectiveFrom = now.AddDays(1),
                CreatedAt = now,
                UpdatedAt = now
            }
        });
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            repository,
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var result = await service.GetSpecialFlagAsync("ITEM001");

        Assert.False(result.IsSpecial);
        Assert.Equal(0, result.RuleCount);
    }

    [Fact]
    public async Task GetSpecialFlagAsync_ReturnsMostConservativeRollbackMode()
    {
        var repository = new SpecialFlagRuleHeaderRepository(new[]
        {
            new RuleHeader
            {
                RuleId = 3,
                RuleCode = "RULE-LEGACY",
                RuleName = "旧逻辑等价规则",
                ItemCode = "ITEM001",
                Status = "PUBLISHED",
                IsEnabled = "Y",
                RollbackMode = "LEGACY_EQUIVALENT",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new RuleHeader
            {
                RuleId = 4,
                RuleCode = "RULE-STOP",
                RuleName = "停收规则",
                ItemCode = "ITEM001",
                Status = "PUBLISHED",
                IsEnabled = "Y",
                RollbackMode = "STOP_CHARGE",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        });
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            repository,
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var result = await service.GetSpecialFlagAsync("ITEM001");

        Assert.True(result.IsSpecial);
        Assert.Equal("STOP_CHARGE", result.RollbackMode);
    }

    [Fact]
    public void PricingResponseDtos_DoNotExposeRuntimePackageFields()
    {
        Assert.Null(typeof(PricingCalculateResponse).GetProperty("RuntimePackageId"));
        Assert.Null(typeof(PricingCalculateResponse).GetProperty("RuntimePackageVersion"));
        Assert.Null(typeof(PricingCalculateResponse).GetProperty("MatchedRuntimeRuleIds"));
        Assert.Null(typeof(PricingCalculateResponse).GetProperty("MatchedPolicyVersionIds"));
        Assert.Null(typeof(PricingCalculateResponse).GetProperty("MatchedTemplateVersionIds"));

        Assert.Null(typeof(PricingCalculateItemResponse).GetProperty("RuntimePackageId"));
        Assert.Null(typeof(PricingCalculateItemResponse).GetProperty("RuntimePackageVersion"));
        Assert.Null(typeof(PricingCalculateItemResponse).GetProperty("MatchedRuntimeRuleIds"));
        Assert.Null(typeof(PricingCalculateItemResponse).GetProperty("MatchedPolicyVersionIds"));
        Assert.Null(typeof(PricingCalculateItemResponse).GetProperty("MatchedTemplateVersionIds"));

        Assert.Null(typeof(SpecialFlagResponse).GetProperty("RuntimePackageId"));
        Assert.Null(typeof(SpecialFlagResponse).GetProperty("RuntimePackageVersion"));
        Assert.Null(typeof(SpecialFlagResponse).GetProperty("MatchedRuntimeRuleIds"));
        Assert.Null(typeof(SpecialFlagResponse).GetProperty("MatchedPolicyVersionIds"));

        Assert.Null(typeof(PricingTraceStepResponse).GetProperty("RuntimeRuleId"));
        Assert.Null(typeof(PricingTraceStepResponse).GetProperty("SourcePolicyVersionId"));
        Assert.Null(typeof(PricingTraceStepResponse).GetProperty("SourceTemplateVersionId"));
        Assert.NotNull(typeof(PricingTraceStepResponse).GetProperty("RuleId"));
    }

    [Fact]
    public async Task GetSpecialFlagAsync_UsesDirectRuleSnapshotAndQueryDimensions()
    {
        var directRule = new RuleHeader
        {
            RuleId = 901,
            RuleCode = "RULE-DIRECT-SPECIAL-FLAG",
            RuleName = "直接规则特殊项目",
            ItemCode = "ITEM001",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1,
            RollbackMode = "STOP_CHARGE",
            EffectiveFrom = new DateTime(2026, 5, 1),
            EffectiveTo = new DateTime(2026, 5, 31, 23, 59, 59),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        var conditions = new[]
        {
            new RuleCondition
            {
                ConditionId = 1,
                RuleId = 901,
                VersionNo = 1,
                ConditionGroup = "DEFAULT",
                ConditionType = RuleConditionTypeCodes.ChargeScene,
                RightValue = "OUTPATIENT",
                IsEnabled = "Y",
                SortNo = 10
            }
        };
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            new SpecialFlagRuleHeaderRepository(new[]
            {
                directRule
            }),
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance,
            new FixedRuleConditionRepository(conditions),
            new EmptyRuleActionRepository(),
            new ConditionEvaluatorFactory(new IRuleConditionEvaluator[]
            {
                new ChargeSceneMatchEvaluator()
            }));

        var matched = await service.GetSpecialFlagAsync(new SpecialFlagRequest
        {
            ItemCode = "ITEM001",
            ChargeScene = "OUTPATIENT",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0)
        });
        var notMatched = await service.GetSpecialFlagAsync(new SpecialFlagRequest
        {
            ItemCode = "ITEM001",
            ChargeScene = "INPATIENT",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0)
        });

        Assert.True(matched.IsSpecial);
        Assert.Equal(new[] { 901L }, matched.MatchedRuleIds);
        Assert.False(notMatched.IsSpecial);
        Assert.Equal(0, notMatched.RuleCount);
    }

    [Fact]
    public async Task GetSpecialFlagsAsync_ReturnsBatchSummaryAndEffectiveContext()
    {
        var directRule = new RuleHeader
        {
            RuleId = 902,
            RuleCode = "RULE-BATCH-SPECIAL-FLAG",
            RuleName = "批量特殊项目规则",
            ItemCode = "ITEM_SPECIAL",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            CurrentVersion = 1,
            RollbackMode = "STOP_CHARGE",
            EffectiveFrom = new DateTime(2026, 5, 1),
            EffectiveTo = new DateTime(2026, 5, 31, 23, 59, 59),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        var conditions = new[]
        {
            new RuleCondition
            {
                ConditionId = 1,
                RuleId = 902,
                VersionNo = 1,
                ConditionGroup = "DEFAULT",
                ConditionType = RuleConditionTypeCodes.ChargeScene,
                RightValue = "OUTPATIENT",
                IsEnabled = "Y",
                SortNo = 10
            },
            new RuleCondition
            {
                ConditionId = 2,
                RuleId = 902,
                VersionNo = 1,
                ConditionGroup = "DEFAULT",
                ConditionType = RuleConditionTypeCodes.BodyPart,
                RightValue = "HEAD",
                IsEnabled = "Y",
                SortNo = 20
            }
        };
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            new SpecialFlagRuleHeaderRepository(new[]
            {
                directRule
            }),
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance,
            new FixedRuleConditionRepository(conditions),
            new EmptyRuleActionRepository(),
            new ConditionEvaluatorFactory(new IRuleConditionEvaluator[]
            {
                new ChargeSceneMatchEvaluator(),
                new BodyPartMatchEvaluator()
            }));
        var request = new SpecialFlagBatchRequest
        {
            RequestNo = "REQ-SPECIAL-FLAG-BATCH-001",
            SourceSystem = "HIS",
            SourceTerminal = "SWAGGER",
            PatientId = "P001",
            VisitId = "V001",
            VisitType = "OUTPATIENT",
            EncounterNo = "OP20260510001",
            ChargeScene = "OUTPATIENT",
            ChargeDeptCode = "998",
            ChargeNo = "C001",
            BusinessRequestNo = "BR-SPECIAL-FLAG-BATCH-001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            OperatorId = "tester",
            OperatorName = "测试员",
            ExtraParams = new Dictionary<string, object?>
            {
                ["operationNo"] = "OP001"
            },
            Items = new[]
            {
                new SpecialFlagBatchItemRequest
                {
                    ItemRequestNo = "1",
                    ChargeDetailNo = "CD001",
                    ItemCode = "ITEM_SPECIAL",
                    ItemName = "特殊测试项目",
                    ItemGroupCode = "GROUP_A",
                    InputQty = 2m,
                    Unit = "PART",
                    UnitPrice = 100m,
                    BodyPartCode = "HEAD",
                    ExtraParams = new Dictionary<string, object?>
                    {
                        ["rowNo"] = "1"
                    }
                },
                new SpecialFlagBatchItemRequest
                {
                    ItemRequestNo = "2",
                    ChargeDetailNo = "CD002",
                    ItemCode = "ITEM_NORMAL",
                    ItemName = "普通测试项目",
                    ItemGroupCode = "GROUP_B",
                    InputQty = 1m,
                    Unit = "EACH",
                    UnitPrice = 20m,
                    ChargeScene = "INPATIENT",
                    BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0),
                    VisitType = "INPATIENT",
                    BodyPartCode = "CHEST",
                    ChargeDeptCode = "997"
                }
            }
        };

        var result = await service.GetSpecialFlagsAsync(request);

        Assert.True(result.IsSpecial);
        Assert.Equal(2, result.ItemCount);
        Assert.Equal(1, result.SpecialItemCount);
        Assert.Equal("CALL_SIMULATE", result.NextAction);
        Assert.True(result.Blocking);
        Assert.Contains("1 条特殊项目", result.DecisionReason);
        Assert.Equal(new DateTime(2026, 5, 10, 10, 0, 0), result.RuleReadTime);
        Assert.Collection(
            result.Items,
            first =>
            {
                Assert.Equal("1", first.ItemRequestNo);
                Assert.Equal("CD001", first.ChargeDetailNo);
                Assert.Equal("ITEM_SPECIAL", first.ItemCode);
                Assert.Equal("特殊测试项目", first.ItemName);
                Assert.True(first.IsSpecial);
                Assert.Equal(1, first.RuleCount);
                Assert.Equal("STOP_CHARGE", first.RollbackMode);
                Assert.Equal(new[] { 902L }, first.MatchedRuleIds);
                Assert.Equal("CALL_SIMULATE", first.NextAction);
                Assert.True(first.Blocking);
                Assert.Contains("批量特殊项目规则", first.DecisionReason);
                var matchedRule = Assert.Single(first.MatchedRules);
                Assert.Equal(902L, matchedRule.RuleId);
                Assert.Equal("RULE-BATCH-SPECIAL-FLAG", matchedRule.RuleCode);
                Assert.Equal("批量特殊项目规则", matchedRule.RuleName);
                Assert.Equal("STOP_CHARGE", matchedRule.RollbackMode);
                Assert.Equal(new DateTime(2026, 5, 10, 10, 0, 0), first.RuleReadTime);
                Assert.Equal("OUTPATIENT", first.EffectiveChargeScene);
                Assert.Equal(new DateTime(2026, 5, 10, 9, 30, 0), first.EffectiveBusinessChargeTime);
                Assert.Equal("OUTPATIENT", first.EffectiveVisitType);
                Assert.Equal("HEAD", first.EffectiveBodyPartCode);
                Assert.Equal("998", first.EffectiveChargeDeptCode);
            },
            second =>
            {
                Assert.Equal("2", second.ItemRequestNo);
                Assert.Equal("CD002", second.ChargeDetailNo);
                Assert.Equal("ITEM_NORMAL", second.ItemCode);
                Assert.Equal("普通测试项目", second.ItemName);
                Assert.False(second.IsSpecial);
                Assert.Equal(0, second.RuleCount);
                Assert.Empty(second.MatchedRuleIds);
                Assert.Empty(second.MatchedRules);
                Assert.Equal("NORMAL_PRICING", second.NextAction);
                Assert.False(second.Blocking);
                Assert.Contains("未命中特殊计价规则", second.DecisionReason);
                Assert.Equal(new DateTime(2026, 5, 10, 10, 0, 0), second.RuleReadTime);
                Assert.Equal("INPATIENT", second.EffectiveChargeScene);
                Assert.Equal(new DateTime(2026, 5, 10, 10, 0, 0), second.EffectiveBusinessChargeTime);
                Assert.Equal("INPATIENT", second.EffectiveVisitType);
                Assert.Equal("CHEST", second.EffectiveBodyPartCode);
                Assert.Equal("997", second.EffectiveChargeDeptCode);
            });
    }

    [Fact]
    public async Task GetSpecialFlagsAsync_RejectsInvalidBatchItems()
    {
        var service = CreateValidationService();

        var emptyItems = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetSpecialFlagsAsync(new SpecialFlagBatchRequest
            {
                Items = Array.Empty<SpecialFlagBatchItemRequest>()
            }));
        var tooManyItems = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetSpecialFlagsAsync(new SpecialFlagBatchRequest
            {
                Items = Enumerable.Range(1, 51)
                    .Select(index => new SpecialFlagBatchItemRequest { ItemCode = $"ITEM{index:000}" })
                    .ToArray()
            }));
        var blankItemCode = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetSpecialFlagsAsync(new SpecialFlagBatchRequest
            {
                Items = new[]
                {
                    new SpecialFlagBatchItemRequest { ItemCode = "ITEM001" },
                    new SpecialFlagBatchItemRequest { ItemCode = " " }
                }
            }));

        Assert.Contains("Items", emptyItems.Message);
        Assert.Contains("50", tooManyItems.Message);
        Assert.Contains("第 2 行", blankItemCode.Message);
        Assert.Contains("项目编码", blankItemCode.Message);
    }

    [Fact]
    public async Task SimulateAsync_CalculatesEveryChargeItem()
    {
        var engine = new CapturingPricingEngine();
        var requestLogRepository = new InMemoryChargeRequestLogRepository();
        var service = CreatePricingApiService(
            engine,
            new EmptyRuleHeaderRepository(),
            requestLogRepository,
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var request = new PricingCalculateRequest
        {
            RequestNo = "REQ-001",
            SourceSystem = "HIS",
            PatientId = "P001",
            VisitType = "OUTPATIENT",
            ChargeScene = "REQUEST_SCENE",
            ChargeDeptCode = "REQ_DEPT",
            PatientAge = 32,
            ChargeNo = "C001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            ExtraParams = new Dictionary<string, object?>
            {
                ["operationNo"] = "OP001"
            },
            Items = new List<PricingCalculateItemRequest>
            {
                new()
                {
                    ChargeDetailNo = "CD001",
                    ItemCode = "CT001",
                    ItemName = "CT平扫",
                    ItemGroupCode = "CT_GROUP",
                    InputQty = 2m,
                    Unit = "PART",
                    UnitPrice = 300m,
                    ChargeScene = "ITEM_SCENE",
                    VisitType = "ITEM_VISIT",
                    ChargeDeptCode = "ITEM_DEPT",
                    ExtraParams = new Dictionary<string, object?>
                    {
                        ["pregnancyNo"] = "PREG001"
                    }
                },
                new()
                {
                    ChargeDetailNo = "CD002",
                    ItemCode = "SKIN001",
                    ItemName = "皮肤治疗",
                    InputQty = 18m,
                    Unit = "CM2",
                    UnitPrice = 200m
                }
            }
        };

        var response = await service.SimulateAsync(request);

        Assert.Collection(
            engine.Contexts,
            first =>
            {
                Assert.Equal("CT001", first.ItemCode);
                Assert.Equal("CD001", response.Items[0].ChargeDetailNo);
                Assert.Equal("CT_GROUP", first.ItemGroupCode);
                Assert.Equal("ITEM_SCENE", first.ChargeScene);
                Assert.Equal("ITEM_VISIT", first.VisitType);
                Assert.Equal("ITEM_DEPT", first.ChargeDeptCode);
                Assert.Equal(32, first.PatientAge);
                Assert.Equal(2m, first.InputQty);
                Assert.Equal("OP001", first.ExtraParams?["operationNo"]);
                Assert.Equal("PREG001", first.ExtraParams?["pregnancyNo"]);
            },
            second =>
            {
                Assert.Equal("SKIN001", second.ItemCode);
                Assert.Equal("CD002", response.Items[1].ChargeDetailNo);
                Assert.Equal(18m, second.InputQty);
                Assert.Equal("REQUEST_SCENE", second.ChargeScene);
                Assert.Equal("OUTPATIENT", second.VisitType);
                Assert.Equal("REQ_DEPT", second.ChargeDeptCode);
            });
        Assert.Equal(2, response.Items.Count);
        Assert.Equal(4200m, response.FinalAmount);
        Assert.Equal("CONFIRM_BEFORE_CHARGE", response.NextAction);
        Assert.Equal("SIMULATED", response.BusinessStatus);
        Assert.Equal(new DateTime(2026, 5, 10, 10, 0, 0), response.RuleReadTime);
        Assert.Null(requestLogRepository.Inserted[0].ItemCode);
    }

    [Fact]
    public async Task SimulateAsync_RejectsDuplicateBusinessRequestNo()
    {
        var requestLogRepository = new InMemoryChargeRequestLogRepository();
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            new EmptyRuleHeaderRepository(),
            requestLogRepository,
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);
        var request = CreateConfirmRequest("REQ-SIM-DUPLICATE", "BR-SIM-DUPLICATE");

        await service.SimulateAsync(request);
        var ex = await Assert.ThrowsAsync<BizException>(() => service.SimulateAsync(request));

        Assert.Equal(BizErrorCode.BusinessRequestNoDuplicated, ex.Code);
        Assert.Equal(409, ex.HttpStatusCode);
        Assert.Contains("业务请求号重复", ex.Message);
        Assert.Single(requestLogRepository.Inserted);
    }

    [Fact]
    public async Task SimulateAsync_MapsUniqueConstraintDuplicateBusinessRequestNoToConflict()
    {
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            new EmptyRuleHeaderRepository(),
            new DuplicateOnInsertChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.SimulateAsync(CreateConfirmRequest("REQ-SIM-DUPLICATE-DB", "BR-SIM-DUPLICATE-DB")));

        Assert.Equal(BizErrorCode.BusinessRequestNoDuplicated, ex.Code);
        Assert.Equal(409, ex.HttpStatusCode);
        Assert.Contains("业务请求号重复", ex.Message);
    }

    [Fact]
    public async Task SimulateAsync_PassesPriorDayAndWindowOccupiesToLaterItems()
    {
        var engine = new LimitCacheCapturingPricingEngine();
        var service = CreatePricingApiService(
            engine,
            new EmptyRuleHeaderRepository(),
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var request = new PricingCalculateRequest
        {
            RequestNo = "REQ-002",
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = "C002",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new List<PricingCalculateItemRequest>
            {
                new()
                {
                    ChargeDetailNo = "CD001",
                    ItemCode = "ITEM001",
                    InputQty = 2m,
                    UnitPrice = 10m
                },
                new()
                {
                    ChargeDetailNo = "CD002",
                    ItemCode = "ITEM001",
                    InputQty = 3m,
                    UnitPrice = 10m
                }
            }
        };

        await service.SimulateAsync(request);

        var secondContext = engine.Contexts[1];
        Assert.Equal(
            2m,
            secondContext.RequestSharedState.GetLimitQty("DAY_QTY", "P001:ITEM001:20260510"));
        Assert.Equal(
            2m,
            secondContext.RequestSharedState.GetLimitQty("TIME_WINDOW", "P001:ITEM001"));
    }

    [Fact]
    public async Task ConfirmAsync_ReturnsAndPersistsReplacementItem()
    {
        var discountRepository = new CapturingChargeDiscountDetailRepository();
        var service = CreatePricingApiService(
            new ReplacementPricingEngine(),
            new EmptyRuleHeaderRepository(),
            new InMemoryChargeRequestLogRepository(),
            discountRepository,
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var request = new PricingCalculateRequest
        {
            RequestNo = "REQ-003",
            BusinessRequestNo = "BR-003",
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = "C003",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new List<PricingCalculateItemRequest>
            {
                new()
                {
                    ChargeDetailNo = "CD003",
                    ItemCode = "ITEM001",
                    ItemName = "原项目",
                    InputQty = 4m,
                    UnitPrice = 10m
                }
            }
        };

        var response = await service.ConfirmAsync(request);

        var itemResponse = Assert.Single(response.Items);
        var replacementItem = Assert.IsType<PricingReplacementItemResponse>(itemResponse.ReplacementItem);
        Assert.Equal("ITEM_ADD", replacementItem.ItemCode);
        Assert.Equal(4m, itemResponse.ExceedQty);
        Assert.Equal(20m, itemResponse.FinalAmount);
        Assert.Equal(20m, replacementItem.Amount);

        Assert.Equal(2, discountRepository.Inserted.Count);
        var mainDetail = discountRepository.Inserted.Single(d => d.ParentDiscountId is null);
        var replacementDetail = discountRepository.Inserted.Single(d => d.ParentDiscountId == mainDetail.DiscountId);
        Assert.Equal(0m, mainDetail.FinalAmt);
        Assert.Equal("ITEM_ADD", replacementDetail.ItemCode);
        Assert.Equal(4m, replacementDetail.FinalQty);
        Assert.Equal(20m, replacementDetail.FinalAmt);
        Assert.Equal(mainDetail.ResultGroupNo, replacementDetail.ResultGroupNo);
        Assert.Equal("COMMIT_OR_CANCEL", response.NextAction);
        Assert.Equal("CONFIRM_PENDING", response.BusinessStatus);
        Assert.Equal(new DateTime(2026, 5, 10, 10, 0, 0), response.RuleReadTime);
    }

    [Fact]
    public async Task ConfirmAsync_ReturnsAndPersistsChildItems()
    {
        var discountRepository = new CapturingChargeDiscountDetailRepository();
        var service = CreatePricingApiService(
            new ChildItemPricingEngine(),
            new EmptyRuleHeaderRepository(),
            new InMemoryChargeRequestLogRepository(),
            discountRepository,
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var request = new PricingCalculateRequest
        {
            RequestNo = "REQ-004",
            BusinessRequestNo = "BR-004",
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = "C004",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new List<PricingCalculateItemRequest>
            {
                new()
                {
                    ChargeDetailNo = "CD004",
                    ItemCode = "ITEM_MAIN",
                    ItemName = "主项目",
                    InputQty = 2m,
                    UnitPrice = 100m
                }
            }
        };

        var response = await service.ConfirmAsync(request);

        var itemResponse = Assert.Single(response.Items);
        var childItem = Assert.Single(itemResponse.ChildItems);
        Assert.Equal("ITEM_CHILD", childItem.ItemCode);
        Assert.Equal(1m, childItem.Qty);
        Assert.Equal(30m, childItem.Amount);
        Assert.Equal(230m, itemResponse.FinalAmount);
        Assert.Equal(230m, response.FinalAmount);

        Assert.Equal(2, discountRepository.Inserted.Count);
        var mainDetail = discountRepository.Inserted.Single(d => d.ParentDiscountId is null);
        var childDetail = discountRepository.Inserted.Single(d => d.ParentDiscountId == mainDetail.DiscountId);
        Assert.Equal("ITEM_MAIN", mainDetail.ItemCode);
        Assert.Equal("ITEM_CHILD", childDetail.ItemCode);
        Assert.Equal(30m, childDetail.FinalAmt);
        Assert.Equal(mainDetail.ResultGroupNo, childDetail.ResultGroupNo);
        Assert.StartsWith("CHILD:", mainDetail.ResultGroupNo);
    }

    [Fact]
    public async Task ConfirmAsync_PersistsMixedDetailsAndCommitAcceptsFullHisActuals()
    {
        var requestRepository = new InMemoryChargeRequestLogRepository();
        var discountRepository = new CapturingChargeDiscountDetailRepository();
        var limitRepository = new CapturingLimitOccupyRepository();
        var service = CreatePricingApiService(
            new MixedSpecialPricingEngine(),
            new EmptyRuleHeaderRepository(),
            requestRepository,
            discountRepository,
            new EmptyChargeTraceStepRepository(),
            limitRepository,
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var response = await service.ConfirmAsync(new PricingCalculateRequest
        {
            RequestNo = "REQ-005",
            BusinessRequestNo = "BR-005",
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = "C005",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new List<PricingCalculateItemRequest>
            {
                new()
                {
                    ChargeDetailNo = "CD005-S",
                    ItemCode = "ITEM_SPECIAL",
                    ItemName = "特殊项目",
                    InputQty = 1m,
                    UnitPrice = 10m
                },
                new()
                {
                    ChargeDetailNo = "CD005-N",
                    ItemCode = "ITEM_NORMAL",
                    ItemName = "普通项目",
                    InputQty = 2m,
                    UnitPrice = 10m
                }
            }
        });

        Assert.Equal(2, discountRepository.Inserted.Count);
        Assert.Contains(discountRepository.Inserted, d => d.ItemCode == "ITEM_SPECIAL" && d.Status == "PENDING");
        Assert.Contains(discountRepository.Inserted, d => d.ItemCode == "ITEM_NORMAL" && d.Status == "PENDING");

        var occupy = Assert.Single(limitRepository.Inserted);
        Assert.Equal("ITEM_SPECIAL", occupy.ItemCode);
        Assert.Equal("PENDING", occupy.Status);

        await service.CommitAsync(new PricingCommitRequest
        {
            RequestId = response.RequestId,
            ChargeNo = "C005-HIS",
            ActualTotalAmount = 30m,
            ActualItems = new[]
            {
                new PricingCommitActualItemRequest
                {
                    ChargeDetailNo = "CD005-S",
                    ItemCode = "ITEM_SPECIAL",
                    FinalQty = 1m,
                    FinalAmount = 10m
                },
                new PricingCommitActualItemRequest
                {
                    ChargeDetailNo = "CD005-N",
                    ItemCode = "ITEM_NORMAL",
                    FinalQty = 2m,
                    FinalAmount = 20m
                }
            }
        });

        var requestLog = Assert.Single(requestRepository.Inserted);
        Assert.Equal("CONFIRMED", requestLog.BusinessStatus);
        Assert.Equal("C005-HIS", requestLog.ChargeNo);
        Assert.All(discountRepository.Inserted, detail => Assert.Equal("CONFIRMED", detail.Status));
        Assert.Equal((response.RequestId, "CONFIRMED"), limitRepository.LastStatusUpdate);
    }

    [Fact]
    public async Task ConfirmAsync_PersistsSameTraceIdAcrossRequestStepsDetailsAndOccupies()
    {
        var requestRepository = new InMemoryChargeRequestLogRepository();
        var discountRepository = new CapturingChargeDiscountDetailRepository();
        var traceRepository = new CapturingChargeTraceStepRepository();
        var limitRepository = new CapturingLimitOccupyRepository();
        var service = CreatePricingApiService(
            new TraceableSpecialPricingEngine(),
            new EmptyRuleHeaderRepository(),
            requestRepository,
            discountRepository,
            traceRepository,
            limitRepository,
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var response = await service.ConfirmAsync(new PricingCalculateRequest
        {
            RequestNo = "REQ-TRACE",
            BusinessRequestNo = "BR-TRACE",
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = "C-TRACE",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new[]
            {
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = "CD-TRACE",
                    ItemCode = "ITEM_TRACE",
                    ItemName = "追溯项目",
                    InputQty = 1m,
                    UnitPrice = 10m
                }
            }
        });

        var log = Assert.Single(requestRepository.Inserted);
        Assert.Equal(response.RequestId, log.RequestId);
        Assert.False(string.IsNullOrWhiteSpace(log.TraceId));
        var traceId = log.TraceId;
        Assert.All(traceRepository.Inserted, step => Assert.Equal(traceId, step.TraceId));
        Assert.All(discountRepository.Inserted, detail => Assert.Equal(traceId, detail.TraceId));
        Assert.All(limitRepository.Inserted, occupy => Assert.Equal(traceId, occupy.TraceId));
    }

    [Fact]
    public async Task ConfirmAsync_DoesNotPersistRuntimePackageMetadata()
    {
        var requestRepository = new InMemoryChargeRequestLogRepository();
        var discountRepository = new CapturingChargeDiscountDetailRepository();
        var traceRepository = new CapturingChargeTraceStepRepository();
        var limitRepository = new CapturingLimitOccupyRepository();
        var service = CreatePricingApiService(
            new RuleTracePricingEngine(),
            new EmptyRuleHeaderRepository(),
            requestRepository,
            discountRepository,
            traceRepository,
            limitRepository,
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var response = await service.ConfirmAsync(new PricingCalculateRequest
        {
            RequestNo = "REQ-RUNTIME-TRACE",
            BusinessRequestNo = "BR-RUNTIME-TRACE",
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = "C-RUNTIME-TRACE",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new[]
            {
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = "CD-RUNTIME-TRACE",
                    ItemCode = "ITEM_TRACE",
                    ItemName = "追溯项目",
                    InputQty = 1m,
                    UnitPrice = 10m
                }
            }
        });

        Assert.Equal(new[] { 501L }, response.MatchedRuleIds);
        Assert.Equal(10m, response.TotalOriginalAmount);
        Assert.Equal(response.FinalAmount, response.TotalFinalAmount);
        Assert.Equal(response.DiscountAmount, response.TotalDiscountAmount);
        Assert.Equal(new[] { 501L }, response.Items[0].MatchedRuleIds);
        Assert.Equal(501, response.Items[0].TraceSteps[0].RuleId);

        var requestLog = Assert.Single(requestRepository.Inserted);
        Assert.Equal(2, traceRepository.Inserted.Count);
        Assert.All(traceRepository.Inserted, traceStep =>
        {
            Assert.Equal(501, traceStep.RuleId);
            Assert.Null(traceStep.RuntimeRuleId);
            Assert.Null(traceStep.SourcePolicyVersionId);
            Assert.Null(traceStep.SourceTemplateVersionId);
        });
        var discountDetail = Assert.Single(discountRepository.Inserted);
        Assert.Equal(501, discountDetail.RuleId);
        Assert.Null(discountDetail.RuntimeRuleId);
        Assert.Null(discountDetail.SourcePolicyVersionId);
        Assert.Null(discountDetail.SourceTemplateVersionId);
    }

    [Fact]
    public async Task SimulateAsync_ReturnsReadableTraceNodesOnItemOnly()
    {
        var service = CreatePricingApiService(
            new RuleTracePricingEngine(),
            new EmptyRuleHeaderRepository(),
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var response = await service.SimulateAsync(new PricingCalculateRequest
        {
            RequestNo = "REQ-TRACE-NODE",
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = "C-TRACE-NODE",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new[]
            {
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = "CD-TRACE-NODE",
                    ItemCode = "ITEM_TRACE",
                    ItemName = "追溯项目",
                    InputQty = 2m,
                    UnitPrice = 100m
                }
            }
        });

        Assert.Null(response.TraceSteps);

        var item = Assert.Single(response.Items);
        Assert.Collection(
            item.TraceSteps,
            match =>
            {
                Assert.False(string.IsNullOrWhiteSpace(match.NodeKey));
                Assert.Equal("规则匹配", match.NodeTitle);
                Assert.Equal("RULE-TIME-WINDOW", match.RuleCode);
                Assert.Equal("测试时间窗上限规则", match.RuleName);
                Assert.Contains("测试时间窗上限规则", match.NodeDesc);
                Assert.Equal("MATCH_RESULT", match.ValueType);
                Assert.Equal("输入数量", match.InputName);
                Assert.Equal("动作数量", match.OutputName);
            },
            limit =>
            {
                Assert.False(string.IsNullOrWhiteSpace(limit.NodeKey));
                Assert.Equal("限额处理", limit.NodeTitle);
                Assert.Equal("RULE-TIME-WINDOW", limit.RuleCode);
                Assert.Equal("测试时间窗上限规则", limit.RuleName);
                Assert.Equal("APPLY_TIME_WINDOW_LIMIT", limit.ActionCode);
                Assert.Equal("时间窗口限额", limit.ActionName);
                Assert.Contains("测试时间窗上限规则", limit.NodeDesc);
                Assert.Contains("时间窗口限额", limit.NodeDesc);
                Assert.Equal("AMOUNT", limit.ValueType);
                Assert.Equal("元", limit.ValueUnit);
                Assert.Equal("处理前金额", limit.InputName);
                Assert.Equal("处理后金额", limit.OutputName);
            });

        var json = JsonSerializer.Serialize(response, ApiJsonSerializerOptions.Create());
        using var document = JsonDocument.Parse(json);
        Assert.False(document.RootElement.TryGetProperty("trace_steps", out _));
        Assert.True(document.RootElement.GetProperty("items")[0].TryGetProperty("trace_steps", out var itemTraceSteps));
        Assert.True(itemTraceSteps[0].TryGetProperty("node_title", out _));
    }

    [Fact]
    public async Task SimulateAsync_DoesNotPersistRuntimePackageMetadata_WhenEngineStateChangesDuringExecution()
    {
        var requestRepository = new InMemoryChargeRequestLogRepository();
        var engine = new RuleStateChangingPricingEngine();
        var service = CreatePricingApiService(
            engine,
            new EmptyRuleHeaderRepository(),
            requestRepository,
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var response = await service.SimulateAsync(new PricingCalculateRequest
        {
            RequestNo = "REQ-RUNTIME-SWITCH",
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = "C-RUNTIME-SWITCH",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new[]
            {
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = "CD-RULE-SWITCH",
                    ItemCode = "ITEM_SWITCH",
                    ItemName = "规则切换项目",
                    InputQty = 1m,
                    UnitPrice = 10m
                }
            }
        });

        Assert.Equal(new[] { 601L }, response.MatchedRuleIds);
        Assert.True(engine.WasSwitched);

        var requestLog = Assert.Single(requestRepository.Inserted);
    }

    [Fact]
    public async Task ConfirmAsync_PersistsChargeDetailNoIntoLimitOccupies()
    {
        var requestRepository = new InMemoryChargeRequestLogRepository();
        var discountRepository = new CapturingChargeDiscountDetailRepository();
        var limitRepository = new CapturingLimitOccupyRepository();
        var service = CreatePricingApiService(
            new TraceableSpecialPricingEngine(),
            new EmptyRuleHeaderRepository(),
            requestRepository,
            discountRepository,
            new EmptyChargeTraceStepRepository(),
            limitRepository,
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var response = await service.ConfirmAsync(new PricingCalculateRequest
        {
            RequestNo = "REQ-TRACE-DETAIL",
            BusinessRequestNo = "BR-TRACE-DETAIL",
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = "C-TRACE-DETAIL",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new[]
            {
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = "CD-TRACE-DETAIL",
                    ItemCode = "ITEM_TRACE",
                    ItemName = "追溯项目",
                    InputQty = 1m,
                    UnitPrice = 10m
                }
            }
        });

        Assert.True(response.IsSpecialItem);
        var occupy = Assert.Single(limitRepository.Inserted);
        Assert.Equal("CD-TRACE-DETAIL", occupy.ChargeDetailNo);
        Assert.Null(occupy.ResultGroupNo);
    }

    [Fact]
    public async Task ConfirmAsync_RejectsIdempotentRetryWhenResponseSnapshotIsMissing()
    {
        var requestRepository = new InMemoryChargeRequestLogRepository();
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            new EmptyRuleHeaderRepository(),
            requestRepository,
            new CapturingChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);
        var request = CreateConfirmRequest("REQ-IDEMPOTENT-MISSING", "BR-IDEMPOTENT-MISSING");

        await service.ConfirmAsync(request);
        requestRepository.Inserted.Single().ResponseJson = null;

        var ex = await Assert.ThrowsAsync<BizException>(() => service.ConfirmAsync(request));

        Assert.Equal(2014, ex.Code);
    }

    [Fact]
    public async Task ConfirmAsync_RejectsIdempotentRetryWhenResponseSnapshotIsInvalid()
    {
        var requestRepository = new InMemoryChargeRequestLogRepository();
        var service = CreatePricingApiService(
            new CapturingPricingEngine(),
            new EmptyRuleHeaderRepository(),
            requestRepository,
            new CapturingChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);
        var request = CreateConfirmRequest("REQ-IDEMPOTENT-INVALID", "BR-IDEMPOTENT-INVALID");

        await service.ConfirmAsync(request);
        requestRepository.Inserted.Single().ResponseJson = "{not-json";

        var ex = await Assert.ThrowsAsync<BizException>(() => service.ConfirmAsync(request));

        Assert.Equal(2014, ex.Code);
    }

    [Fact]
    public async Task CommitAsync_ConfirmsWhenHisActualsMatchConfirmDetails()
    {
        var requestRepository = new CommitRequestLogRepository(new ChargeRequestLog
        {
            RequestId = 900,
            BusinessStatus = "CONFIRM_PENDING",
            SourceSystem = "HIS",
            ChargeNo = "C900",
            RequestAt = DateTime.Now
        });
        var discountRepository = new CommitDiscountDetailRepository(new[]
        {
            new ChargeDiscountDetail
            {
                RequestId = 900,
                ChargeDetailNo = "CD900",
                ItemCode = "ITEM900",
                FinalQty = 2m,
                FinalAmt = 20m,
                Status = "PENDING"
            }
        });
        var limitRepository = new CommitLimitOccupyRepository();
        var service = CreateCommitService(requestRepository, discountRepository, limitRepository);

        var commitResponse = await service.CommitAsync(new PricingCommitRequest
        {
            RequestId = 900,
            ChargeNo = "C900-HIS",
            ActualTotalAmount = 20m,
            ActualItems = new[]
            {
                new PricingCommitActualItemRequest
                {
                    ChargeDetailNo = "CD900",
                    ItemCode = "ITEM900",
                    FinalQty = 2m,
                    FinalAmount = 20m
                }
            }
        });

        Assert.Equal("CONFIRMED", requestRepository.Log.BusinessStatus);
        Assert.Equal("C900-HIS", requestRepository.Log.ChargeNo);
        Assert.Equal("CONFIRMED", Assert.Single(discountRepository.Details).Status);
        Assert.NotNull(limitRepository.LastStatusUpdate);
        Assert.Equal(900L, limitRepository.LastStatusUpdate.Value.RequestId);
        Assert.Equal("CONFIRMED", limitRepository.LastStatusUpdate.Value.Status);
        Assert.Equal(900L, commitResponse.RequestId);
        Assert.Equal("CONFIRMED", commitResponse.BusinessStatus);
        Assert.Equal("C900-HIS", commitResponse.ChargeNo);
        Assert.Equal("NO_FURTHER_ACTION", commitResponse.NextAction);
        Assert.NotNull(commitResponse.CommittedAt);
    }

    [Fact]
    public async Task CancelAsync_ReturnsCancelledResponse()
    {
        var requestRepository = new CommitRequestLogRepository(new ChargeRequestLog
        {
            RequestId = 910,
            BusinessStatus = "CONFIRM_PENDING",
            SourceSystem = "HIS",
            RequestAt = DateTime.Now
        });
        var discountRepository = new CommitDiscountDetailRepository(new[]
        {
            new ChargeDiscountDetail
            {
                RequestId = 910,
                ChargeDetailNo = "CD910",
                ItemCode = "ITEM910",
                FinalQty = 1m,
                FinalAmt = 10m,
                Status = "PENDING"
            }
        });
        var limitRepository = new CommitLimitOccupyRepository();
        var service = CreateCommitService(requestRepository, discountRepository, limitRepository);

        var cancelResponse = await service.CancelAsync(new PricingCancelRequest
        {
            RequestId = 910,
            CancelNo = "CN910",
            CancelReason = "HIS 落账失败"
        });

        Assert.Equal("CANCELLED", requestRepository.Log.BusinessStatus);
        Assert.Equal("CANCELLED", Assert.Single(discountRepository.Details).Status);
        Assert.Equal(910L, cancelResponse.RequestId);
        Assert.Equal("CANCELLED", cancelResponse.BusinessStatus);
        Assert.Equal("NO_FURTHER_ACTION", cancelResponse.NextAction);
    }

    [Fact]
    public async Task CommitAsync_RejectsHisActualAmountMismatchAndKeepsPending()
    {
        var requestRepository = new CommitRequestLogRepository(new ChargeRequestLog
        {
            RequestId = 901,
            BusinessStatus = "CONFIRM_PENDING",
            SourceSystem = "HIS",
            RequestAt = DateTime.Now
        });
        var discountRepository = new CommitDiscountDetailRepository(new[]
        {
            new ChargeDiscountDetail
            {
                RequestId = 901,
                ChargeDetailNo = "CD901",
                ItemCode = "ITEM901",
                FinalQty = 2m,
                FinalAmt = 20m,
                Status = "PENDING"
            }
        });
        var limitRepository = new CommitLimitOccupyRepository();
        var service = CreateCommitService(requestRepository, discountRepository, limitRepository);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.CommitAsync(new PricingCommitRequest
            {
                RequestId = 901,
                ChargeNo = "C901",
                ActualItems = new[]
                {
                    new PricingCommitActualItemRequest
                    {
                        ChargeDetailNo = "CD901",
                        ItemCode = "ITEM901",
                        FinalQty = 2m,
                        FinalAmount = 21m
                    }
                }
            }));

        Assert.Equal(BizErrorCode.CommitAmountMismatch, ex.Code);
        Assert.Equal("CONFIRM_PENDING", requestRepository.Log.BusinessStatus);
        Assert.Equal("PENDING", Assert.Single(discountRepository.Details).Status);
        Assert.Null(limitRepository.LastStatusUpdate);
    }

    [Fact]
    public async Task CommitAsync_RejectsHisActualQtyMismatchAndKeepsPending()
    {
        var requestRepository = new CommitRequestLogRepository(new ChargeRequestLog
        {
            RequestId = 902,
            BusinessStatus = "CONFIRM_PENDING",
            SourceSystem = "HIS",
            RequestAt = DateTime.Now
        });
        var discountRepository = new CommitDiscountDetailRepository(new[]
        {
            new ChargeDiscountDetail
            {
                RequestId = 902,
                ChargeDetailNo = "CD902",
                ItemCode = "ITEM902",
                FinalQty = 2m,
                FinalAmt = 20m,
                Status = "PENDING"
            }
        });
        var limitRepository = new CommitLimitOccupyRepository();
        var service = CreateCommitService(requestRepository, discountRepository, limitRepository);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.CommitAsync(new PricingCommitRequest
            {
                RequestId = 902,
                ChargeNo = "C902",
                ActualItems = new[]
                {
                    new PricingCommitActualItemRequest
                    {
                        ChargeDetailNo = "CD902",
                        ItemCode = "ITEM902",
                        FinalQty = 3m,
                        FinalAmount = 20m
                    }
                }
            }));

        Assert.Equal(BizErrorCode.CommitQtyMismatch, ex.Code);
        Assert.Equal("CONFIRM_PENDING", requestRepository.Log.BusinessStatus);
        Assert.Equal("PENDING", Assert.Single(discountRepository.Details).Status);
        Assert.Null(limitRepository.LastStatusUpdate);
    }

    [Fact]
    public async Task CommitAsync_AllowsChildActualsWithNewHisChargeDetailNo()
    {
        var requestRepository = new CommitRequestLogRepository(new ChargeRequestLog
        {
            RequestId = 903,
            BusinessStatus = "CONFIRM_PENDING",
            SourceSystem = "HIS",
            RequestAt = DateTime.Now
        });
        var discountRepository = new CommitDiscountDetailRepository(new[]
        {
            new ChargeDiscountDetail
            {
                RequestId = 903,
                DiscountId = 1001,
                ChargeDetailNo = "CD903",
                ItemCode = "ITEM_MAIN",
                FinalQty = 2m,
                FinalAmt = 200m,
                Status = "PENDING"
            },
            new ChargeDiscountDetail
            {
                RequestId = 903,
                DiscountId = 1002,
                ParentDiscountId = 1001,
                ChargeDetailNo = "CD903",
                ItemCode = "ITEM_CHILD",
                FinalQty = 1m,
                FinalAmt = 30m,
                Status = "PENDING"
            }
        });
        var limitRepository = new CommitLimitOccupyRepository();
        var service = CreateCommitService(requestRepository, discountRepository, limitRepository);

        await service.CommitAsync(new PricingCommitRequest
        {
            RequestId = 903,
            ChargeNo = "C903",
            ActualTotalAmount = 230m,
            ActualItems = new[]
            {
                new PricingCommitActualItemRequest
                {
                    ChargeDetailNo = "CD903",
                    ItemCode = "ITEM_MAIN",
                    FinalQty = 2m,
                    FinalAmount = 200m
                },
                new PricingCommitActualItemRequest
                {
                    ChargeDetailNo = "HIS-CD903-CHILD",
                    ItemCode = "ITEM_CHILD",
                    FinalQty = 1m,
                    FinalAmount = 30m
                }
            }
        });

        Assert.Equal("CONFIRMED", requestRepository.Log.BusinessStatus);
        Assert.All(discountRepository.Details, detail => Assert.Equal("CONFIRMED", detail.Status));
    }

    [Fact]
    public async Task CommitAsync_RejectsChildActualAmountMismatchEvenWithNewHisChargeDetailNo()
    {
        var requestRepository = new CommitRequestLogRepository(new ChargeRequestLog
        {
            RequestId = 904,
            BusinessStatus = "CONFIRM_PENDING",
            SourceSystem = "HIS",
            RequestAt = DateTime.Now
        });
        var discountRepository = new CommitDiscountDetailRepository(new[]
        {
            new ChargeDiscountDetail
            {
                RequestId = 904,
                DiscountId = 2001,
                ChargeDetailNo = "CD904",
                ItemCode = "ITEM_MAIN",
                FinalQty = 2m,
                FinalAmt = 200m,
                Status = "PENDING"
            },
            new ChargeDiscountDetail
            {
                RequestId = 904,
                DiscountId = 2002,
                ParentDiscountId = 2001,
                ChargeDetailNo = "CD904",
                ItemCode = "ITEM_CHILD",
                FinalQty = 1m,
                FinalAmt = 30m,
                Status = "PENDING"
            }
        });
        var limitRepository = new CommitLimitOccupyRepository();
        var service = CreateCommitService(requestRepository, discountRepository, limitRepository);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.CommitAsync(new PricingCommitRequest
            {
                RequestId = 904,
                ChargeNo = "C904",
                ActualItems = new[]
                {
                    new PricingCommitActualItemRequest
                    {
                        ChargeDetailNo = "CD904",
                        ItemCode = "ITEM_MAIN",
                        FinalQty = 2m,
                        FinalAmount = 200m
                    },
                    new PricingCommitActualItemRequest
                    {
                        ChargeDetailNo = "HIS-CD904-CHILD",
                        ItemCode = "ITEM_CHILD",
                        FinalQty = 1m,
                        FinalAmount = 31m
                    }
                }
            }));

        Assert.Equal(BizErrorCode.CommitAmountMismatch, ex.Code);
        Assert.Equal("CONFIRM_PENDING", requestRepository.Log.BusinessStatus);
        Assert.All(discountRepository.Details, detail => Assert.Equal("PENDING", detail.Status));
    }

    private static PricingApiService CreateCommitService(
        IChargeRequestLogRepository requestRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository) =>
        CreatePricingApiService(
            new CapturingPricingEngine(),
            new EmptyRuleHeaderRepository(),
            requestRepository,
            discountRepository,
            new EmptyChargeTraceStepRepository(),
            limitRepository,
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

    private static PricingApiService CreateValidationService() =>
        CreatePricingApiService(
            new CapturingPricingEngine(),
            new EmptyRuleHeaderRepository(),
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            new NoopUnitOfWork(),
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

    private static PricingApiService CreatePricingApiService(
        IPricingEngine engine,
        IRuleHeaderRepository headerRepository,
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        IChargeTraceStepRepository traceStepRepository,
        ILimitOccupyRepository limitRepository,
        IChargeReverseLogRepository reverseLogRepository,
        IPriceMasterRepository priceMasterRepository,
        IUnitOfWork unitOfWork,
        IOptions<PricingOptions> options,
        ILogger<PricingApiService> logger,
        IRuleConditionRepository? ruleConditionRepository = null,
        IRuleActionRepository? ruleActionRepository = null,
        ConditionEvaluatorFactory? conditionEvaluatorFactory = null)
    {
        var authorityPriceChecker = new AuthorityPriceChecker(
            priceMasterRepository,
            options,
            NullLogger<AuthorityPriceChecker>.Instance);
        var clock = new FixedClock(new DateTime(2026, 5, 10, 10, 0, 0));
        var requestLogWriter = new PricingRequestLogWriter(requestLogRepository, clock);
        var traceStepWriter = new PricingTraceStepWriter(traceStepRepository, clock);
        var discountDetailWriter = new PricingDiscountDetailWriter(discountRepository, clock);
        var limitOccupyWriter = new PricingLimitOccupyWriter(
            limitRepository,
            options,
            NullLogger<PricingLimitOccupyWriter>.Instance,
            clock);
        var simulationPersistenceService = new PricingSimulationPersistenceService(
            requestLogWriter,
            traceStepWriter,
            clock);
        var confirmationPersistenceService = new PricingConfirmationPersistenceService(
            requestLogWriter,
            traceStepWriter,
            discountDetailWriter,
            limitOccupyWriter,
            options,
            clock);
        var reverseLogWriter = new PricingReverseLogWriter(requestLogRepository, reverseLogRepository, clock);
        var reverseHistoryReader = new PricingReverseHistoryReader(reverseLogRepository);
        IRuleConditionGroupMatcher? conditionMatcher = conditionEvaluatorFactory is null
            ? null
            : new RuleConditionGroupMatcher(
                conditionEvaluatorFactory,
                NullLogger<RuleConditionGroupMatcher>.Instance);
        var effectiveRuleSnapshotLoader = ruleConditionRepository is null
            ? null
            : new EffectiveRuleReader(new RuleMatchRepositories(
                headerRepository,
                ruleConditionRepository,
                ruleActionRepository ?? new EmptyRuleActionRepository(),
                new EmptyDictRepository()));
        var specialFlagResolver = new PricingSpecialFlagResolver(
            headerRepository,
            clock,
            conditionMatcher,
            effectiveRuleSnapshotLoader);

        return new PricingApiService(
            new PricingSimulateWorkflow(
                engine,
                requestLogRepository,
                authorityPriceChecker,
                simulationPersistenceService,
                clock,
                NullLogger<PricingSimulateWorkflow>.Instance),
            new PricingConfirmWorkflow(
                engine,
                requestLogRepository,
                authorityPriceChecker,
                confirmationPersistenceService,
                limitRepository,
                unitOfWork,
                options,
                clock,
                NullLogger<PricingConfirmWorkflow>.Instance),
            new PricingCommitWorkflow(
                requestLogRepository,
                discountRepository,
                limitRepository,
                unitOfWork,
                options,
                clock,
                NullLogger<PricingCommitWorkflow>.Instance),
            new PricingCancelWorkflow(
                requestLogRepository,
                discountRepository,
                limitRepository,
                unitOfWork,
                clock,
                NullLogger<PricingCancelWorkflow>.Instance),
            new PricingReverseWorkflow(
                requestLogRepository,
                discountRepository,
                limitRepository,
                reverseLogRepository,
                reverseLogWriter,
                limitOccupyWriter,
                reverseHistoryReader,
                unitOfWork,
                clock,
                NullLogger<PricingReverseWorkflow>.Instance),
            specialFlagResolver);
    }

    private sealed class NoopUnitOfWork : IUnitOfWork
    {
        public Task BeginAsync() => Task.CompletedTask;

        public Task CommitAsync() => Task.CompletedTask;

        public Task RollbackAsync() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    private sealed class FixedRuleConditionRepository : IRuleConditionRepository
    {
        private readonly IReadOnlyList<RuleCondition> _conditions;

        public FixedRuleConditionRepository(IReadOnlyList<RuleCondition> conditions)
        {
            _conditions = conditions;
        }

        public Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult((IReadOnlyList<RuleCondition>)_conditions
                .Where(condition => condition.RuleId == ruleId && condition.VersionNo == versionNo)
                .ToList());

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
        public Task<Dict?> GetByIdAsync(long dictId) => Task.FromResult<Dict?>(null);

        public Task<IReadOnlyList<Dict>> GetByTypeAsync(string dictType) =>
            Task.FromResult((IReadOnlyList<Dict>)Array.Empty<Dict>());

        public Task<IReadOnlyList<string>> GetAllTypesAsync() =>
            Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());

        public Task<long> InsertAsync(Dict entity) => Task.FromResult(0L);

        public Task<bool> UpdateAsync(Dict entity) => Task.FromResult(false);

        public Task<bool> SetEnabledAsync(long dictId, string isEnabled) => Task.FromResult(false);

        public Task<bool> ExistsAsync(string dictType, string dictCode) => Task.FromResult(false);
    }

    private static PricingCalculateRequest CreateValidCalculateRequest(
        string sourceSystem = "HIS",
        string patientId = "P001",
        DateTime? businessChargeTime = null,
        IReadOnlyList<PricingCalculateItemRequest>? items = null) =>
        new()
        {
            RequestNo = "REQ-VALID",
            SourceSystem = sourceSystem,
            PatientId = patientId,
            BusinessChargeTime = businessChargeTime ?? new DateTime(2026, 5, 10, 9, 30, 0),
            Items = items ?? new[]
            {
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = "CD001",
                    ItemCode = "ITEM001",
                    InputQty = 1m,
                    UnitPrice = 10m
                }
            }
        };

    private static PricingCalculateRequest CreateConfirmRequest(
        string requestNo,
        string businessRequestNo) =>
        new()
        {
            RequestNo = requestNo,
            BusinessRequestNo = businessRequestNo,
            SourceSystem = "HIS",
            PatientId = "P001",
            ChargeNo = $"C-{businessRequestNo}",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0),
            Items = new[]
            {
                new PricingCalculateItemRequest
                {
                    ChargeDetailNo = $"CD-{businessRequestNo}",
                    ItemCode = "ITEM001",
                    ItemName = "测试项目",
                    InputQty = 1m,
                    UnitPrice = 10m
                }
            }
        };

    private sealed class CapturingPricingEngine : IPricingEngine
    {
        public List<PricingContext> Contexts { get; } = new();

        public Task<PricingResult> CalculateAsync(PricingContext context)
        {
            Contexts.Add(context);
            return Task.FromResult(new PricingResult
            {
                IsSpecialItem = false,
                InputQty = context.InputQty,
                FinalQty = context.InputQty,
                UnitPrice = context.UnitPrice,
                FinalAmount = context.InputQty * context.UnitPrice,
                DiscountAmount = 0
            });
        }
    }

    private sealed class LimitCacheCapturingPricingEngine : IPricingEngine
    {
        public List<PricingContext> Contexts { get; } = new();

        public Task<PricingResult> CalculateAsync(PricingContext context)
        {
            Contexts.Add(context);
            var dayDimensionCode = $"{context.PatientId}:{context.ItemCode}:{context.BusinessChargeTime:yyyyMMdd}".ToUpperInvariant();
            var windowDimensionCode = $"{context.PatientId}:{context.ItemCode}".ToUpperInvariant();

            return Task.FromResult(new PricingResult
            {
                IsSpecialItem = true,
                InputQty = context.InputQty,
                FinalQty = context.InputQty,
                UnitPrice = context.UnitPrice,
                FinalAmount = context.InputQty * context.UnitPrice,
                LimitOccupies = new[]
                {
                    new LimitOccupy
                    {
                        LimitType = "DAY_QTY",
                        LimitDimensionCode = dayDimensionCode,
                        OccupyQty = context.InputQty
                    },
                    new LimitOccupy
                    {
                        LimitType = "TIME_WINDOW",
                        LimitDimensionCode = windowDimensionCode,
                        OccupyQty = context.InputQty
                    }
                }
            });
        }
    }

    private sealed class ReplacementPricingEngine : IPricingEngine
    {
        public Task<PricingResult> CalculateAsync(PricingContext context)
        {
            return Task.FromResult(new PricingResult
            {
                IsSpecialItem = true,
                InputQty = context.InputQty,
                ConvertedQty = context.InputQty,
                FinalQty = 0m,
                UnitPrice = context.UnitPrice,
                FinalAmount = 20m,
                DiscountAmount = 20m,
                ExceedQty = context.InputQty,
                MatchedRuleIds = new[] { 101L },
                ReplaceChildResult = new ReplaceChildResult
                {
                    ItemCode = "ITEM_ADD",
                    ItemName = "替换加收",
                    Qty = context.InputQty,
                    UnitPrice = 5m,
                    Amount = 20m
                }
            });
        }
    }

    private sealed class ChildItemPricingEngine : IPricingEngine
    {
        public Task<PricingResult> CalculateAsync(PricingContext context)
        {
            return Task.FromResult(new PricingResult
            {
                IsSpecialItem = true,
                InputQty = context.InputQty,
                ConvertedQty = context.InputQty,
                FinalQty = context.InputQty,
                UnitPrice = context.UnitPrice,
                FinalAmount = context.InputQty * context.UnitPrice,
                DiscountAmount = 0m,
                MatchedRuleIds = new[] { 102L },
                ChildPricingResults = new[]
                {
                    new ChildPricingResult
                    {
                        ItemCode = "ITEM_CHILD",
                        ItemName = "加收子项",
                        Qty = 1m,
                        UnitPrice = 30m,
                        Amount = 30m,
                        ShareParentLimit = true,
                        ParentItemCode = context.ItemCode
                    }
                }
            });
        }
    }

    private sealed class MixedSpecialPricingEngine : IPricingEngine
    {
        public Task<PricingResult> CalculateAsync(PricingContext context)
        {
            var amount = context.InputQty * context.UnitPrice;
            var isSpecial = string.Equals(context.ItemCode, "ITEM_SPECIAL", StringComparison.OrdinalIgnoreCase);
            return Task.FromResult(new PricingResult
            {
                IsSpecialItem = isSpecial,
                InputQty = context.InputQty,
                ConvertedQty = context.InputQty,
                FinalQty = context.InputQty,
                UnitPrice = context.UnitPrice,
                FinalAmount = amount,
                DiscountAmount = 0m,
                LimitOccupies = isSpecial
                    ? new[]
                    {
                        new LimitOccupy
                        {
                            PatientId = context.PatientId,
                            ItemCode = context.ItemCode,
                            LimitType = "DAY_QTY",
                            LimitKey = $"DAY_QTY:{context.PatientId}:{context.ItemCode}:20260510",
                            LimitDimensionCode = $"{context.PatientId}:{context.ItemCode}:20260510",
                            OccupyQty = context.InputQty,
                            OccupyAmt = amount,
                            OccupyType = "CHARGE",
                            BusinessChargeTime = context.BusinessChargeTime
                        }
                    }
                    : Array.Empty<LimitOccupy>()
            });
        }
    }

    private sealed class TraceableSpecialPricingEngine : IPricingEngine
    {
        public Task<PricingResult> CalculateAsync(PricingContext context)
        {
            return Task.FromResult(new PricingResult
            {
                IsSpecialItem = true,
                InputQty = context.InputQty,
                ConvertedQty = context.InputQty,
                FinalQty = context.InputQty,
                UnitPrice = context.UnitPrice,
                FinalAmount = context.InputQty * context.UnitPrice,
                DiscountAmount = 0m,
                TraceSteps = new[]
                {
                    new TraceStep
                    {
                        StepNo = 1,
                        StepType = "MATCH",
                        StepDesc = "命中特殊规则",
                        InputValue = context.InputQty,
                        OutputValue = context.InputQty
                    }
                },
                MatchedRuleIds = new[] { 201L },
                LimitOccupies = new[]
                {
                    new LimitOccupy
                    {
                        PatientId = context.PatientId,
                        ItemCode = context.ItemCode,
                        LimitType = "DAY_QTY",
                        LimitKey = $"DAY_QTY:{context.PatientId}:{context.ItemCode}:20260510",
                        LimitDimensionCode = $"{context.PatientId}:{context.ItemCode}:20260510",
                        OccupyQty = context.InputQty,
                        OccupyAmt = context.InputQty * context.UnitPrice,
                        OccupyType = "CHARGE",
                        BusinessChargeTime = context.BusinessChargeTime
                    }
                }
            });
        }
    }

    private sealed class RuleTracePricingEngine : IPricingEngine
    {
        public Task<PricingResult> CalculateAsync(PricingContext context)
        {
            return Task.FromResult(new PricingResult
            {
                IsSpecialItem = true,
                InputQty = context.InputQty,
                ConvertedQty = context.InputQty,
                FinalQty = context.InputQty,
                UnitPrice = context.UnitPrice,
                FinalAmount = context.InputQty * context.UnitPrice,
                DiscountAmount = 0m,
                TraceSteps = new[]
                {
                    new TraceStep
                    {
                        StepNo = 1,
                        StepType = "MATCH",
                        StepDesc = "命中规则",
                        InputValue = context.InputQty,
                        OutputValue = context.InputQty,
                        RuntimeRuleId = 501
                    },
                    new TraceStep
                    {
                        StepNo = 2,
                        StepType = "LIMIT",
                        StepDesc = "执行时间窗口限额",
                        InputValue = context.InputQty * context.UnitPrice,
                        OutputValue = context.UnitPrice,
                        RuntimeRuleId = 501,
                        ActionCode = "APPLY_TIME_WINDOW_LIMIT"
                    }
                },
                MatchedRuleIds = new[] { 501L },
                MatchedRuleInfos = new[]
                {
                    new PricingRuleTraceInfo
                    {
                        RuleId = 501,
                        RuleCode = "RULE-TIME-WINDOW",
                        RuleName = "测试时间窗上限规则"
                    }
                }
            });
        }
    }

    private sealed class RuleStateChangingPricingEngine : IPricingEngine
    {
        private bool _switched;

        public bool WasSwitched => _switched;

        public Task<PricingResult> CalculateAsync(PricingContext context)
        {
            if (!_switched)
            {
                _switched = true;
            }

            return Task.FromResult(new PricingResult
            {
                IsSpecialItem = true,
                InputQty = context.InputQty,
                ConvertedQty = context.InputQty,
                FinalQty = context.InputQty,
                UnitPrice = context.UnitPrice,
                FinalAmount = context.InputQty * context.UnitPrice,
                DiscountAmount = 0m,
                MatchedRuleIds = new[] { 601L }
            });
        }
    }

    private sealed class InMemoryChargeRequestLogRepository : IChargeRequestLogRepository
    {
        private long _nextId = 100;

        public List<ChargeRequestLog> Inserted { get; } = new();

        public Task<ChargeRequestLog?> GetByIdAsync(long requestId) =>
            Task.FromResult<ChargeRequestLog?>(Inserted.FirstOrDefault(log => log.RequestId == requestId));

        public Task<ChargeRequestLog?> GetByBusinessKeyAsync(
            string sourceSystem, string businessRequestNo, string callType) =>
            Task.FromResult<ChargeRequestLog?>(Inserted.FirstOrDefault(log =>
                string.Equals(log.SourceSystem, sourceSystem, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(log.BusinessRequestNo, businessRequestNo, StringComparison.Ordinal) &&
                string.Equals(log.CallType, callType, StringComparison.OrdinalIgnoreCase)));

        public Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<long> InsertAsync(ChargeRequestLog entity)
        {
            entity.RequestId = ++_nextId;
            Inserted.Add(entity);
            return Task.FromResult(entity.RequestId);
        }

        public Task UpdateAsync(ChargeRequestLog entity)
        {
            var index = Inserted.FindIndex(log => log.RequestId == entity.RequestId);
            if (index >= 0)
            {
                Inserted[index] = entity;
            }

            return Task.CompletedTask;
        }

        public Task<(IReadOnlyList<ChargeRequestLog> Items, int Total)> GetPagedAsync(
            string? patientId,
            string? itemCode,
            string? chargeNo,
            DateTime? startTime,
            DateTime? endTime,
            int pageIndex,
            int pageSize) =>
            Task.FromResult(((IReadOnlyList<ChargeRequestLog>)Array.Empty<ChargeRequestLog>(), 0));

        public Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore) =>
            Task.FromResult((IReadOnlyList<ChargeRequestLog>)Array.Empty<ChargeRequestLog>());
    }

    private sealed class DuplicateOnInsertChargeRequestLogRepository : IChargeRequestLogRepository
    {
        public Task<ChargeRequestLog?> GetByIdAsync(long requestId) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<ChargeRequestLog?> GetByBusinessKeyAsync(
            string sourceSystem,
            string businessRequestNo,
            string callType) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<long> InsertAsync(ChargeRequestLog entity)
        {
            throw new InvalidOperationException(
                "ORA-00001: unique constraint (PRICING.UK_PR_CRL_BIZ) violated");
        }

        public Task UpdateAsync(ChargeRequestLog entity) => Task.CompletedTask;

        public Task<(IReadOnlyList<ChargeRequestLog> Items, int Total)> GetPagedAsync(
            string? patientId,
            string? itemCode,
            string? chargeNo,
            DateTime? startTime,
            DateTime? endTime,
            int pageIndex,
            int pageSize) =>
            Task.FromResult(((IReadOnlyList<ChargeRequestLog>)Array.Empty<ChargeRequestLog>(), 0));

        public Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore) =>
            Task.FromResult((IReadOnlyList<ChargeRequestLog>)Array.Empty<ChargeRequestLog>());
    }

    private sealed class CommitRequestLogRepository : IChargeRequestLogRepository
    {
        public CommitRequestLogRepository(ChargeRequestLog log)
        {
            Log = log;
        }

        public ChargeRequestLog Log { get; private set; }

        public Task<ChargeRequestLog?> GetByIdAsync(long requestId) =>
            Task.FromResult<ChargeRequestLog?>(Log.RequestId == requestId ? Log : null);

        public Task<ChargeRequestLog?> GetByBusinessKeyAsync(
            string sourceSystem,
            string businessRequestNo,
            string callType) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<long> InsertAsync(ChargeRequestLog entity) => Task.FromResult(0L);

        public Task UpdateAsync(ChargeRequestLog entity)
        {
            Log = entity;
            return Task.CompletedTask;
        }

        public Task<(IReadOnlyList<ChargeRequestLog> Items, int Total)> GetPagedAsync(
            string? patientId,
            string? itemCode,
            string? chargeNo,
            DateTime? startTime,
            DateTime? endTime,
            int pageIndex,
            int pageSize) =>
            Task.FromResult(((IReadOnlyList<ChargeRequestLog>)Array.Empty<ChargeRequestLog>(), 0));

        public Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore) =>
            Task.FromResult((IReadOnlyList<ChargeRequestLog>)Array.Empty<ChargeRequestLog>());
    }

    private sealed class CommitDiscountDetailRepository : IChargeDiscountDetailRepository
    {
        public CommitDiscountDetailRepository(IEnumerable<ChargeDiscountDetail> details)
        {
            Details.AddRange(details);
        }

        public List<ChargeDiscountDetail> Details { get; } = new();

        public Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId) =>
            Task.FromResult((IReadOnlyList<ChargeDiscountDetail>)Details
                .Where(d => d.RequestId == requestId)
                .ToList());

        public Task<long> InsertAsync(ChargeDiscountDetail entity) => Task.FromResult(0L);

        public Task UpdateStatusByRequestIdAsync(long requestId, string status)
        {
            foreach (var detail in Details.Where(d => d.RequestId == requestId))
            {
                detail.Status = status;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class CommitLimitOccupyRepository : ILimitOccupyRepository
    {
        public (long RequestId, string Status)? LastStatusUpdate { get; private set; }

        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtByDimensionAsync(string dimensionCode, string status) => Task.FromResult(0m);
        public Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<LimitOccupy>)Array.Empty<LimitOccupy>());
        public Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys) => Task.CompletedTask;
        public Task<long> InsertAsync(LimitOccupy entity) => Task.FromResult(0L);
        public Task UpdateStatusAsync(long occupyId, string status) => Task.CompletedTask;

        public Task UpdateStatusByRequestIdAsync(long requestId, string status)
        {
            LastStatusUpdate = (requestId, status);
            return Task.CompletedTask;
        }
    }

    private sealed class EmptyRuleHeaderRepository : IRuleHeaderRepository
    {
        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(null);
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleHeader entity, string? expectedCurrentStatus = null) => Task.FromResult(false);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }

    private sealed class SpecialFlagRuleHeaderRepository : IRuleHeaderRepository
    {
        private readonly IReadOnlyList<RuleHeader> _rules;

        public SpecialFlagRuleHeaderRepository(IReadOnlyList<RuleHeader> rules)
        {
            _rules = rules;
        }

        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(null);
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) =>
            Task.FromResult((IReadOnlyList<RuleHeader>)_rules
                .Where(r => string.Equals(r.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase))
                .ToList());

        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) =>
            Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));

        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) =>
            Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());

        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleHeader entity, string? expectedCurrentStatus = null) => Task.FromResult(false);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }

    private sealed class EmptyChargeDiscountDetailRepository : IChargeDiscountDetailRepository
    {
        public Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<ChargeDiscountDetail>)Array.Empty<ChargeDiscountDetail>());
        public Task<long> InsertAsync(ChargeDiscountDetail entity) => Task.FromResult(0L);
        public Task UpdateStatusByRequestIdAsync(long requestId, string status) => Task.CompletedTask;
    }

    private sealed class CapturingChargeDiscountDetailRepository : IChargeDiscountDetailRepository
    {
        private long _nextId = 200;

        public List<ChargeDiscountDetail> Inserted { get; } = new();

        public Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId) =>
            Task.FromResult((IReadOnlyList<ChargeDiscountDetail>)Inserted
                .Where(d => d.RequestId == requestId)
                .ToList());

        public Task<long> InsertAsync(ChargeDiscountDetail entity)
        {
            entity.DiscountId = ++_nextId;
            Inserted.Add(entity);
            return Task.FromResult(entity.DiscountId);
        }

        public Task UpdateStatusByRequestIdAsync(long requestId, string status)
        {
            foreach (var detail in Inserted.Where(d => d.RequestId == requestId))
            {
                detail.Status = status;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class CapturingLimitOccupyRepository : ILimitOccupyRepository
    {
        public List<LimitOccupy> Inserted { get; } = new();
        public (long RequestId, string Status)? LastStatusUpdate { get; private set; }

        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtByDimensionAsync(string dimensionCode, string status) => Task.FromResult(0m);
        public Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<LimitOccupy>)Inserted.Where(o => o.RequestId == requestId).ToList());
        public Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys) => Task.CompletedTask;

        public Task<long> InsertAsync(LimitOccupy entity)
        {
            Inserted.Add(entity);
            return Task.FromResult(0L);
        }

        public Task UpdateStatusAsync(long occupyId, string status) => Task.CompletedTask;

        public Task UpdateStatusByRequestIdAsync(long requestId, string status)
        {
            LastStatusUpdate = (requestId, status);
            foreach (var occupy in Inserted.Where(o => o.RequestId == requestId))
            {
                occupy.Status = status;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class EmptyChargeTraceStepRepository : IChargeTraceStepRepository
    {
        public Task<IReadOnlyList<ChargeTraceStep>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<ChargeTraceStep>)Array.Empty<ChargeTraceStep>());
        public Task InsertBatchAsync(IReadOnlyList<ChargeTraceStep> entities) => Task.CompletedTask;
    }

    private sealed class CapturingChargeTraceStepRepository : IChargeTraceStepRepository
    {
        public List<ChargeTraceStep> Inserted { get; } = new();

        public Task<IReadOnlyList<ChargeTraceStep>> GetByRequestIdAsync(long requestId) =>
            Task.FromResult((IReadOnlyList<ChargeTraceStep>)Inserted
                .Where(s => s.RequestId == requestId)
                .ToList());

        public Task InsertBatchAsync(IReadOnlyList<ChargeTraceStep> entities)
        {
            Inserted.AddRange(entities);
            return Task.CompletedTask;
        }
    }

    private sealed class EmptyLimitOccupyRepository : ILimitOccupyRepository
    {
        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtByDimensionAsync(string dimensionCode, string status) => Task.FromResult(0m);
        public Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<LimitOccupy>)Array.Empty<LimitOccupy>());
        public Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys) => Task.CompletedTask;
        public Task<long> InsertAsync(LimitOccupy entity) => Task.FromResult(0L);
        public Task UpdateStatusAsync(long occupyId, string status) => Task.CompletedTask;
        public Task UpdateStatusByRequestIdAsync(long requestId, string status) => Task.CompletedTask;
    }

    private sealed class EmptyChargeReverseLogRepository : IChargeReverseLogRepository
    {
        public Task<IReadOnlyList<ChargeReverseLog>> GetByOriginalRequestIdAsync(long originalRequestId) => Task.FromResult((IReadOnlyList<ChargeReverseLog>)Array.Empty<ChargeReverseLog>());
        public Task<long> InsertAsync(ChargeReverseLog entity) => Task.FromResult(0L);
    }

    private sealed class EmptyPriceMasterRepository : IPriceMasterRepository
    {
        public Task<decimal?> GetUnitPriceAsync(string itemCode) => Task.FromResult<decimal?>(null);
    }
}


