using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PricingApiServiceTests
{
    [Fact]
    public async Task SimulateAsync_CalculatesEveryChargeItem()
    {
        var engine = new CapturingPricingEngine();
        var requestLogRepository = new InMemoryChargeRequestLogRepository();
        var service = new PricingApiService(
            engine,
            new EmptyRuleHeaderRepository(),
            requestLogRepository,
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            db: null!,
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

        var request = new PricingCalculateRequest
        {
            RequestNo = "REQ-001",
            SourceSystem = "HIS",
            PatientId = "P001",
            VisitType = "OUTPATIENT",
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
                Assert.Equal("OUTPATIENT", first.VisitType);
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
            });
        Assert.Equal(2, response.Items.Count);
        Assert.Equal(4200m, response.FinalAmount);
        Assert.Null(requestLogRepository.Inserted[0].ItemCode);
    }

    [Fact]
    public async Task SimulateAsync_PassesPriorDayAndWindowOccupiesToLaterItems()
    {
        var engine = new LimitCacheCapturingPricingEngine();
        var service = new PricingApiService(
            engine,
            new EmptyRuleHeaderRepository(),
            new InMemoryChargeRequestLogRepository(),
            new EmptyChargeDiscountDetailRepository(),
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            db: null!,
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
            secondContext.InRequestOccupiedQtyByLimitDimension["DAY_QTY:P001:ITEM001:20260510"]);
        Assert.Equal(
            2m,
            secondContext.InRequestOccupiedQtyByLimitDimension["TIME_WINDOW:P001:ITEM001"]);
    }

    [Fact]
    public async Task ConfirmAsync_ReturnsAndPersistsReplacementItem()
    {
        var discountRepository = new CapturingChargeDiscountDetailRepository();
        var service = new PricingApiService(
            new ReplacementPricingEngine(),
            new EmptyRuleHeaderRepository(),
            new InMemoryChargeRequestLogRepository(),
            discountRepository,
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            db: null!,
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
    }

    [Fact]
    public async Task ConfirmAsync_ReturnsAndPersistsChildItems()
    {
        var discountRepository = new CapturingChargeDiscountDetailRepository();
        var service = new PricingApiService(
            new ChildItemPricingEngine(),
            new EmptyRuleHeaderRepository(),
            new InMemoryChargeRequestLogRepository(),
            discountRepository,
            new EmptyChargeTraceStepRepository(),
            new EmptyLimitOccupyRepository(),
            new EmptyChargeReverseLogRepository(),
            new EmptyPriceMasterRepository(),
            db: null!,
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

    private sealed class CapturingPricingEngine : IPricingEngine
    {
        public List<PricingContext> Contexts { get; } = new();

        public Task<PricingResult> CalculateAsync(PricingContext context, BatchPricingContext? batchContext = null)
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

        public Task<PricingResult> CalculateAsync(PricingContext context, BatchPricingContext? batchContext = null)
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
        public Task<PricingResult> CalculateAsync(PricingContext context, BatchPricingContext? batchContext = null)
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
        public Task<PricingResult> CalculateAsync(PricingContext context, BatchPricingContext? batchContext = null)
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

    private sealed class InMemoryChargeRequestLogRepository : IChargeRequestLogRepository
    {
        private long _nextId = 100;

        public List<ChargeRequestLog> Inserted { get; } = new();

        public Task<ChargeRequestLog?> GetByIdAsync(long requestId) => Task.FromResult<ChargeRequestLog?>(null);

        public Task<ChargeRequestLog?> GetByBusinessKeyAsync(
            string sourceSystem, string businessRequestNo, string callType) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<long> InsertAsync(ChargeRequestLog entity)
        {
            entity.RequestId = ++_nextId;
            Inserted.Add(entity);
            return Task.FromResult(entity.RequestId);
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

    private sealed class EmptyRuleHeaderRepository : IRuleHeaderRepository
    {
        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(null);
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleHeader entity) => Task.FromResult(false);
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

        public Task UpdateStatusByRequestIdAsync(long requestId, string status) => Task.CompletedTask;
    }

    private sealed class EmptyChargeTraceStepRepository : IChargeTraceStepRepository
    {
        public Task<IReadOnlyList<ChargeTraceStep>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<ChargeTraceStep>)Array.Empty<ChargeTraceStep>());
        public Task InsertBatchAsync(IReadOnlyList<ChargeTraceStep> entities) => Task.CompletedTask;
    }

    private sealed class EmptyLimitOccupyRepository : ILimitOccupyRepository
    {
        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedQtyAsync(string limitType, string limitDimensionCode, DateTime startTime, DateTime endTime, IReadOnlyCollection<string> statuses) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status) => Task.FromResult(0m);
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
