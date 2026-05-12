using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Services;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PricingReverseTests
{
    [Fact]
    public async Task ReverseAsync_AllowsPartialSameDayRefundAndWritesNegativeLimitOccupy()
    {
        var requestRepository = new ReverseRequestLogRepository();
        var discountRepository = new ReverseDiscountDetailRepository();
        var limitRepository = new ReverseLimitOccupyRepository();
        var reverseRepository = new ReverseLogRepository();
        var service = CreateService(requestRepository, discountRepository, limitRepository, reverseRepository);

        requestRepository.Log = new ChargeRequestLog
        {
            RequestId = 100,
            BusinessStatus = "CONFIRMED",
            SourceSystem = "HIS",
            ChargeNo = "C001",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0)
        };
        discountRepository.Details.Add(new ChargeDiscountDetail
        {
            RequestId = 100,
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            FinalQty = 5m,
            FinalAmt = 50m,
            Status = "CONFIRMED"
        });
        limitRepository.Occupies.Add(new LimitOccupy
        {
            OccupyId = 1,
            RequestId = 100,
            PatientId = "P001",
            ItemCode = "ITEM001",
            LimitType = "DAY_QTY",
            LimitKey = "DQ:P001:ITEM001:20260510",
            LimitDimensionCode = "P001:ITEM001:20260510",
            OccupyQty = 5m,
            OccupyAmt = 50m,
            Status = "CONFIRMED",
            OccupyType = "CHARGE",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0)
        });

        await service.ReverseAsync(new PricingReverseRequest
        {
            OriginalRequestId = 100,
            ReverseNo = "R001",
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            ReverseQty = 2m,
            ReverseTime = new DateTime(2026, 5, 10, 11, 0, 0),
            ReversedBy = "tester"
        });

        var negativeOccupy = Assert.Single(limitRepository.Inserted);
        Assert.Equal("REVERSE", negativeOccupy.OccupyType);
        Assert.Equal(1, negativeOccupy.OriginalOccupyId);
        Assert.Equal(-2m, negativeOccupy.OccupyQty);
        Assert.Equal(-20m, negativeOccupy.OccupyAmt);
        Assert.Equal("CONFIRMED", negativeOccupy.Status);
        Assert.Equal(new DateTime(2026, 5, 10, 9, 30, 0), negativeOccupy.BusinessChargeTime);
        Assert.Equal("CONFIRMED", requestRepository.Log.BusinessStatus);

        var reverseRequestLog = Assert.Single(requestRepository.Inserted);
        Assert.Equal("REVERSE", reverseRequestLog.CallType);
        Assert.Equal("REVERSED", reverseRequestLog.BusinessStatus);
        Assert.Equal("R001", reverseRequestLog.BusinessRequestNo);
        Assert.Equal(2m, reverseRequestLog.InputQty);

        var reverseLog = Assert.Single(reverseRepository.Inserted);
        Assert.Equal("CD001", reverseLog.ChargeDetailNo);
        Assert.Equal(reverseRequestLog.RequestId, reverseLog.ReverseRequestId);
    }

    [Fact]
    public async Task ReverseAsync_DoesNotMarkWholeRequestReversedWhenOneOfMultipleDetailsIsFullyRefunded()
    {
        var requestRepository = new ReverseRequestLogRepository();
        var discountRepository = new ReverseDiscountDetailRepository();
        var limitRepository = new ReverseLimitOccupyRepository();
        var reverseRepository = new ReverseLogRepository();
        var service = CreateService(requestRepository, discountRepository, limitRepository, reverseRepository);

        requestRepository.Log = new ChargeRequestLog
        {
            RequestId = 200,
            BusinessStatus = "CONFIRMED",
            SourceSystem = "HIS",
            ChargeNo = "C002",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0)
        };
        discountRepository.Details.AddRange(new[]
        {
            new ChargeDiscountDetail
            {
                RequestId = 200,
                ChargeDetailNo = "CD001",
                ItemCode = "ITEM001",
                FinalQty = 2m,
                FinalAmt = 20m,
                Status = "CONFIRMED"
            },
            new ChargeDiscountDetail
            {
                RequestId = 200,
                ChargeDetailNo = "CD002",
                ItemCode = "ITEM002",
                FinalQty = 3m,
                FinalAmt = 30m,
                Status = "CONFIRMED"
            }
        });

        await service.ReverseAsync(new PricingReverseRequest
        {
            OriginalRequestId = 200,
            ReverseNo = "R002",
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            ReverseQty = 2m,
            ReverseTime = new DateTime(2026, 5, 10, 11, 0, 0)
        });

        Assert.Equal("CONFIRMED", requestRepository.Log.BusinessStatus);
        Assert.All(discountRepository.Details, detail => Assert.Equal("CONFIRMED", detail.Status));
        Assert.Equal(2m, Assert.Single(reverseRepository.Inserted).ReverseQty);
    }

    [Fact]
    public async Task ReverseAsync_IncludesResultGroupChildrenWhenOnlyParentDetailIsRequested()
    {
        var requestRepository = new ReverseRequestLogRepository();
        var discountRepository = new ReverseDiscountDetailRepository();
        var limitRepository = new ReverseLimitOccupyRepository();
        var reverseRepository = new ReverseLogRepository();
        var service = CreateService(requestRepository, discountRepository, limitRepository, reverseRepository);

        requestRepository.Log = new ChargeRequestLog
        {
            RequestId = 250,
            BusinessStatus = "CONFIRMED",
            SourceSystem = "HIS",
            ChargeNo = "C250",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0)
        };
        discountRepository.Details.AddRange(new[]
        {
            new ChargeDiscountDetail
            {
                RequestId = 250,
                DiscountId = 2501,
                ChargeDetailNo = "CD250-P",
                ItemCode = "ITEM_PARENT",
                ResultGroupNo = "CHILD:250",
                FinalQty = 2m,
                FinalAmt = 200m,
                Status = "CONFIRMED"
            },
            new ChargeDiscountDetail
            {
                RequestId = 250,
                DiscountId = 2502,
                ParentDiscountId = 2501,
                ChargeDetailNo = "CD250-C",
                ItemCode = "ITEM_CHILD",
                ResultGroupNo = "CHILD:250",
                FinalQty = 1m,
                FinalAmt = 30m,
                Status = "CONFIRMED"
            }
        });

        await service.ReverseAsync(new PricingReverseRequest
        {
            OriginalRequestId = 250,
            ReverseNo = "R250",
            ChargeDetailNo = "CD250-P",
            ItemCode = "ITEM_PARENT",
            ReverseTime = new DateTime(2026, 5, 10, 11, 0, 0)
        });

        Assert.Equal("REVERSED", requestRepository.Log.BusinessStatus);
        Assert.All(discountRepository.Details, detail => Assert.Equal("REVERSED", detail.Status));

        var reverseRequestLog = Assert.Single(requestRepository.Inserted);
        Assert.Equal(3m, reverseRequestLog.InputQty);
        Assert.Equal("CHILD:250", reverseRequestLog.ResultGroupNo);

        var reverseLog = Assert.Single(reverseRepository.Inserted);
        Assert.Equal(3m, reverseLog.ReverseQty);
        Assert.Equal(230m, reverseLog.ReverseAmt);
    }

    [Fact]
    public async Task ReverseAsync_RejectsReverseAmountGreaterThanOriginalAmount()
    {
        var requestRepository = new ReverseRequestLogRepository();
        var discountRepository = new ReverseDiscountDetailRepository();
        var limitRepository = new ReverseLimitOccupyRepository();
        var reverseRepository = new ReverseLogRepository();
        var service = CreateService(requestRepository, discountRepository, limitRepository, reverseRepository);

        requestRepository.Log = new ChargeRequestLog
        {
            RequestId = 300,
            BusinessStatus = "CONFIRMED",
            SourceSystem = "HIS",
            ChargeNo = "C003",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0)
        };
        discountRepository.Details.Add(new ChargeDiscountDetail
        {
            RequestId = 300,
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            FinalQty = 5m,
            FinalAmt = 50m,
            Status = "CONFIRMED"
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ReverseAsync(new PricingReverseRequest
            {
                OriginalRequestId = 300,
                ReverseNo = "R003",
                ChargeDetailNo = "CD001",
                ItemCode = "ITEM001",
                ReverseQty = 2m,
                ReverseAmt = 60m,
                ReverseTime = new DateTime(2026, 5, 10, 11, 0, 0)
            }));

        Assert.Contains("REVERSE_AMT_EXCEEDED", ex.Message);
        Assert.Empty(reverseRepository.Inserted);
        Assert.Empty(limitRepository.Inserted);
    }

    [Fact]
    public async Task ReverseAsync_UsesOriginalOccupyBusinessTimeForSameDayRelease()
    {
        var requestRepository = new ReverseRequestLogRepository();
        var discountRepository = new ReverseDiscountDetailRepository();
        var limitRepository = new ReverseLimitOccupyRepository();
        var reverseRepository = new ReverseLogRepository();
        var service = CreateService(requestRepository, discountRepository, limitRepository, reverseRepository);

        requestRepository.Log = new ChargeRequestLog
        {
            RequestId = 400,
            BusinessStatus = "CONFIRMED",
            SourceSystem = "HIS",
            ChargeNo = "C004",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0)
        };
        discountRepository.Details.Add(new ChargeDiscountDetail
        {
            RequestId = 400,
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            FinalQty = 5m,
            FinalAmt = 50m,
            Status = "CONFIRMED"
        });
        limitRepository.Occupies.Add(new LimitOccupy
        {
            OccupyId = 4,
            RequestId = 400,
            PatientId = "P001",
            ItemCode = "ITEM001",
            LimitType = "DAY_QTY",
            LimitKey = "DQ:P001:ITEM001:20260511",
            LimitDimensionCode = "P001:ITEM001:20260511",
            OccupyQty = 5m,
            OccupyAmt = 50m,
            Status = "CONFIRMED",
            OccupyType = "CHARGE",
            BusinessChargeTime = new DateTime(2026, 5, 11, 8, 30, 0)
        });

        await service.ReverseAsync(new PricingReverseRequest
        {
            OriginalRequestId = 400,
            ReverseNo = "R004",
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            ReverseQty = 2m,
            ReverseTime = new DateTime(2026, 5, 11, 11, 0, 0)
        });

        var negativeOccupy = Assert.Single(limitRepository.Inserted);
        Assert.Equal(-2m, negativeOccupy.OccupyQty);
        Assert.Equal(new DateTime(2026, 5, 11, 8, 30, 0), negativeOccupy.BusinessChargeTime);
    }

    [Fact]
    public async Task ReverseAsync_ReusesSameReverseNoIdempotently()
    {
        var requestRepository = new ReverseRequestLogRepository();
        var discountRepository = new ReverseDiscountDetailRepository();
        var limitRepository = new ReverseLimitOccupyRepository();
        var reverseRepository = new ReverseLogRepository();
        var service = CreateService(requestRepository, discountRepository, limitRepository, reverseRepository);

        requestRepository.Log = new ChargeRequestLog
        {
            RequestId = 500,
            BusinessStatus = "CONFIRMED",
            SourceSystem = "HIS",
            ChargeNo = "C005"
        };
        discountRepository.Details.Add(new ChargeDiscountDetail
        {
            RequestId = 500,
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            FinalQty = 5m,
            FinalAmt = 50m,
            Status = "CONFIRMED"
        });
        limitRepository.Occupies.Add(new LimitOccupy
        {
            OccupyId = 5,
            RequestId = 500,
            PatientId = "P001",
            ItemCode = "ITEM001",
            LimitType = "DAY_QTY",
            LimitKey = "DQ:P001:ITEM001:20260510",
            LimitDimensionCode = "P001:ITEM001:20260510",
            OccupyQty = 5m,
            OccupyAmt = 50m,
            Status = "CONFIRMED",
            OccupyType = "CHARGE",
            BusinessChargeTime = new DateTime(2026, 5, 10, 9, 30, 0)
        });

        var reverseRequest = new PricingReverseRequest
        {
            OriginalRequestId = 500,
            ReverseNo = "R005",
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            ReverseQty = 2m,
            ReverseTime = new DateTime(2026, 5, 10, 11, 0, 0)
        };

        await service.ReverseAsync(reverseRequest);
        await service.ReverseAsync(reverseRequest);

        Assert.Single(reverseRepository.Inserted);
        Assert.Single(requestRepository.Inserted);
        Assert.Single(limitRepository.Inserted);
    }

    [Fact]
    public async Task ReverseAsync_RejectsSameReverseNoWithDifferentParams()
    {
        var requestRepository = new ReverseRequestLogRepository();
        var discountRepository = new ReverseDiscountDetailRepository();
        var limitRepository = new ReverseLimitOccupyRepository();
        var reverseRepository = new ReverseLogRepository();
        var service = CreateService(requestRepository, discountRepository, limitRepository, reverseRepository);

        requestRepository.Log = new ChargeRequestLog
        {
            RequestId = 600,
            BusinessStatus = "CONFIRMED",
            SourceSystem = "HIS",
            ChargeNo = "C006"
        };
        discountRepository.Details.Add(new ChargeDiscountDetail
        {
            RequestId = 600,
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            FinalQty = 5m,
            FinalAmt = 50m,
            Status = "CONFIRMED"
        });

        await service.ReverseAsync(new PricingReverseRequest
        {
            OriginalRequestId = 600,
            ReverseNo = "R006",
            ChargeDetailNo = "CD001",
            ItemCode = "ITEM001",
            ReverseQty = 2m,
            ReverseTime = new DateTime(2026, 5, 10, 11, 0, 0)
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ReverseAsync(new PricingReverseRequest
            {
                OriginalRequestId = 600,
                ReverseNo = "R006",
                ChargeDetailNo = "CD001",
                ItemCode = "ITEM001",
                ReverseQty = 3m,
                ReverseTime = new DateTime(2026, 5, 10, 11, 1, 0)
            }));

        Assert.Contains("IDEMPOTENT_CONFLICT", ex.Message);
    }

    private static PricingApiService CreateService(
        IChargeRequestLogRepository requestRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository,
        IChargeReverseLogRepository reverseRepository) =>
        new(
            new PricingApiCalculationDependencies(
                new EmptyPricingEngine(),
                new EmptyRuleHeaderRepository(),
                new EmptyPriceMasterRepository()),
            new PricingApiPersistenceRepositories(
                requestRepository,
                discountRepository,
                new EmptyTraceStepRepository(),
                limitRepository,
                reverseRepository),
            db: null!,
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = false }),
            NullLogger<PricingApiService>.Instance);

    private sealed class ReverseRequestLogRepository : IChargeRequestLogRepository
    {
        private long _nextId = 9000;

        public ChargeRequestLog Log { get; set; } = new();
        public List<ChargeRequestLog> Inserted { get; } = new();

        public Task<ChargeRequestLog?> GetByIdAsync(long requestId) =>
            Task.FromResult<ChargeRequestLog?>(
                Log.RequestId == requestId
                    ? Log
                    : Inserted.FirstOrDefault(log => log.RequestId == requestId));

        public Task<ChargeRequestLog?> GetByBusinessKeyAsync(string sourceSystem, string businessRequestNo, string callType) => Task.FromResult<ChargeRequestLog?>(null);
        public Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint) => Task.FromResult<ChargeRequestLog?>(null);
        public Task<long> InsertAsync(ChargeRequestLog entity)
        {
            entity.RequestId = ++_nextId;
            Inserted.Add(entity);
            return Task.FromResult(entity.RequestId);
        }

        public Task UpdateAsync(ChargeRequestLog entity)
        {
            if (Log.RequestId == entity.RequestId)
            {
                Log = entity;
            }
            else
            {
                var index = Inserted.FindIndex(log => log.RequestId == entity.RequestId);
                if (index >= 0)
                {
                    Inserted[index] = entity;
                }
            }

            return Task.CompletedTask;
        }

        public Task<(IReadOnlyList<ChargeRequestLog> Items, int Total)> GetPagedAsync(string? patientId, string? itemCode, string? chargeNo, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize) =>
            Task.FromResult(((IReadOnlyList<ChargeRequestLog>)Array.Empty<ChargeRequestLog>(), 0));

        public Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore) =>
            Task.FromResult((IReadOnlyList<ChargeRequestLog>)Array.Empty<ChargeRequestLog>());
    }

    private sealed class ReverseDiscountDetailRepository : IChargeDiscountDetailRepository
    {
        public List<ChargeDiscountDetail> Details { get; } = new();
        public Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<ChargeDiscountDetail>)Details.Where(d => d.RequestId == requestId).ToList());
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

    private sealed class ReverseLimitOccupyRepository : ILimitOccupyRepository
    {
        public List<LimitOccupy> Occupies { get; } = new();
        public List<LimitOccupy> Inserted { get; } = new();
        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<LimitOccupy>)Occupies.Where(o => o.RequestId == requestId).ToList());
        public Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys) => Task.CompletedTask;
        public Task<long> InsertAsync(LimitOccupy entity)
        {
            Inserted.Add(entity);
            return Task.FromResult(0L);
        }

        public Task UpdateStatusAsync(long occupyId, string status) => Task.CompletedTask;
        public Task UpdateStatusByRequestIdAsync(long requestId, string status)
        {
            foreach (var occupy in Occupies.Where(o => o.RequestId == requestId))
            {
                occupy.Status = status;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class ReverseLogRepository : IChargeReverseLogRepository
    {
        public List<ChargeReverseLog> Inserted { get; } = new();
        public Task<IReadOnlyList<ChargeReverseLog>> GetByOriginalRequestIdAsync(long originalRequestId) =>
            Task.FromResult((IReadOnlyList<ChargeReverseLog>)Inserted.Where(r => r.OriginalRequestId == originalRequestId).ToList());

        public Task<long> InsertAsync(ChargeReverseLog entity)
        {
            Inserted.Add(entity);
            return Task.FromResult(0L);
        }
    }

    private sealed class EmptyPricingEngine : IPricingEngine
    {
        public Task<PricingResult> CalculateAsync(PricingContext context, BatchPricingContext? batchContext = null) => Task.FromResult(new PricingResult());
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

    private sealed class EmptyTraceStepRepository : IChargeTraceStepRepository
    {
        public Task<IReadOnlyList<ChargeTraceStep>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<ChargeTraceStep>)Array.Empty<ChargeTraceStep>());
        public Task InsertBatchAsync(IReadOnlyList<ChargeTraceStep> entities) => Task.CompletedTask;
    }

    private sealed class EmptyPriceMasterRepository : IPriceMasterRepository
    {
        public Task<decimal?> GetUnitPriceAsync(string itemCode) => Task.FromResult<decimal?>(null);
    }
}
