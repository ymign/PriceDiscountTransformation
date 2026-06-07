using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing;
using Pricing.RuleCenter.Core.Aggregates.Charging;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PricingWorkflowSupportTests
{
    [Fact]
    public async Task IdempotentResponseReader_ReturnsDeserializedSnapshot()
    {
        var reader = new PricingIdempotentResponseReader(NullLogger<PricingIdempotentResponseReader>.Instance);
        var response = new PricingCalculateResponse
        {
            RequestId = 100,
            FinalAmount = 12.34m,
            FinalQty = 2m
        };
        var log = new ChargeRequest
        {
            RequestId = 100,
            ResponseJson = "{\"requestId\":100,\"finalAmount\":12.34,\"finalQty\":2.0}"
        };

        var result = await reader.ReadAsync(log);

        Assert.Equal(response.RequestId, result.RequestId);
        Assert.Equal(response.FinalAmount, result.FinalAmount);
        Assert.Equal(response.FinalQty, result.FinalQty);
    }

    [Fact]
    public async Task IdempotentResponseReader_RejectsMissingSnapshot()
    {
        var reader = new PricingIdempotentResponseReader(NullLogger<PricingIdempotentResponseReader>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => reader.ReadAsync(new ChargeRequest
        {
            RequestId = 101,
            ResponseJson = null
        }));

        Assert.Equal(BizErrorCode.IdempotencyResponseSnapshotInvalid, ex.Code);
    }

    [Fact]
    public async Task IdempotentResponseReader_RejectsInvalidSnapshot()
    {
        var reader = new PricingIdempotentResponseReader(NullLogger<PricingIdempotentResponseReader>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => reader.ReadAsync(new ChargeRequest
        {
            RequestId = 102,
            ResponseJson = "{not-json"
        }));

        Assert.Equal(BizErrorCode.IdempotencyResponseSnapshotInvalid, ex.Code);
    }

    [Fact]
    public async Task TransactionExecutor_CommitsOnSuccessAndRollsBackOnFailure()
    {
        var unitOfWork = new RecordingUnitOfWork();
        var executor = new PricingTransactionExecutor(unitOfWork, NullLogger<PricingTransactionExecutor>.Instance);

        var result = await executor.ExecuteAsync(async () =>
        {
            await Task.Yield();
            return 123;
        });

        Assert.Equal(123, result);
        Assert.Equal(1, unitOfWork.BeginCount);
        Assert.Equal(1, unitOfWork.CommitCount);
        Assert.Equal(0, unitOfWork.RollbackCount);

        await Assert.ThrowsAsync<InvalidOperationException>(() => executor.ExecuteAsync<int>(() =>
            throw new InvalidOperationException("boom")));

        Assert.Equal(2, unitOfWork.BeginCount);
        Assert.Equal(1, unitOfWork.CommitCount);
        Assert.Equal(1, unitOfWork.RollbackCount);
    }

    [Fact]
    public async Task SpecialFlagResolver_UsesMostConservativeRollbackModeWithinEffectivePublishedRules()
    {
        var resolver = new PricingSpecialFlagResolver(
            new StubRuleHeaderRepository(new[]
            {
                new RuleAggregate
                {
                    RuleId = 1,
                    ItemCode = "ITEM001",
                    Status = RuleStatusCodes.Published,
                    IsEnabled = EnableFlag.Yes,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    RollbackMode = "LEGACY_EQUIVALENT"
                },
                new RuleAggregate
                {
                    RuleId = 2,
                    ItemCode = "ITEM001",
                    Status = RuleStatusCodes.Published,
                    IsEnabled = EnableFlag.Yes,
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    EffectiveTo = new DateTime(2026, 12, 31),
                    RollbackMode = "STOP_CHARGE"
                },
                new RuleAggregate
                {
                    RuleId = 3,
                    ItemCode = "ITEM001",
                    Status = RuleStatusCodes.Published,
                    IsEnabled = EnableFlag.Yes,
                    EffectiveFrom = new DateTime(2027, 1, 1),
                    EffectiveTo = new DateTime(2027, 12, 31),
                    RollbackMode = "MANUAL_REVIEW"
                }
            }),
            new FixedClock(new DateTime(2026, 5, 10, 10, 0, 0)));

        var result = await resolver.ResolveAsync(" ITEM001 ");

        Assert.True(result.IsSpecial);
        Assert.Equal(2, result.RuleCount);
        Assert.Equal("STOP_CHARGE", result.RollbackMode);
    }

    private sealed class RecordingUnitOfWork : IUnitOfWork
    {
        public int BeginCount { get; private set; }
        public int CommitCount { get; private set; }
        public int RollbackCount { get; private set; }

        public Task BeginAsync()
        {
            BeginCount++;
            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            CommitCount++;
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            RollbackCount++;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private sealed class StubRuleHeaderRepository : IRuleHeaderRepository
    {
        private readonly IReadOnlyList<RuleAggregate> _items;

        public StubRuleHeaderRepository(IReadOnlyList<RuleAggregate> items)
        {
            _items = items;
        }

        public Task<RuleAggregate?> GetByIdAsync(long ruleId) => Task.FromResult<RuleAggregate?>(null);
        public Task<RuleAggregate?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult<RuleAggregate?>(null);
        public Task<RuleAggregate?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleAggregate?>(null);
        public Task<IReadOnlyList<RuleAggregate>> GetByItemCodeAsync(string itemCode) =>
            Task.FromResult((IReadOnlyList<RuleAggregate>)_items.Where(item => item.ItemCode == itemCode.Trim()).ToList());
        public Task<(IReadOnlyList<RuleAggregate> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) =>
            Task.FromResult(((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>(), 0));
        public Task<IReadOnlyList<RuleAggregate>> GetEffectiveAsync(DateTime businessTime) =>
            Task.FromResult((IReadOnlyList<RuleAggregate>)Array.Empty<RuleAggregate>());
        public Task<long> InsertAsync(RuleAggregate entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleAggregate entity, string? expectedCurrentStatus = null) => Task.FromResult(true);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }
}
