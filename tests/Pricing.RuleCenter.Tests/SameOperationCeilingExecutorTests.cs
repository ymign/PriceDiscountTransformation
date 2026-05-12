using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class SameOperationCeilingExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_UsesDatabaseAndInRequestAmountWithOperationNo()
    {
        var repository = new InMemoryLimitOccupyRepository
        {
            OccupiedAmtByStatus =
            {
                ["SO:OP001:GRP001:PENDING"] = 20m,
                ["SO:OP001:GRP001:CONFIRMED"] = 50m
            }
        };
        var executor = new SameOperationCeilingExecutor(repository);
        var context = new PricingContext
        {
            PatientId = "P001",
            ItemCode = "ITEM001",
            ItemGroupCode = "GRP001",
            FinalQty = 1m,
            FinalAmount = 50m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0),
            ShouldLockLimits = true,
            ExtraParams = new Dictionary<string, string>
            {
                ["operationNo"] = "OP001"
            },
            InRequestLimitOccupies = new[]
            {
                new LimitOccupy
                {
                    LimitType = "SAME_OPERATION",
                    LimitDimensionCode = "OP001:GRP001",
                    OccupyAmt = 10m
                }
            }
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "SAME_OPERATION_CEILING",
            ParamsJson = JsonConvert.SerializeObject(new { CeilingPerOperation = 100m })
        }, context);

        Assert.Equal(20m, context.FinalAmount);
        Assert.Equal(new[] { "SO:OP001:GRP001" }, repository.LockedKeys);
        var occupy = Assert.Single(context.PendingLimitOccupies);
        Assert.Equal("SAME_OPERATION", occupy.LimitType);
        Assert.Equal("SO:OP001:GRP001", occupy.LimitKey);
        Assert.Equal("OP001:GRP001", occupy.LimitDimensionCode);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsWhenOperationNoMissing()
    {
        var executor = new SameOperationCeilingExecutor(new InMemoryLimitOccupyRepository());
        var context = new PricingContext
        {
            PatientId = "P001",
            ItemCode = "ITEM001",
            ItemGroupCode = "GRP001",
            FinalQty = 1m,
            FinalAmount = 50m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(new RuleAction
            {
                ActionType = "SAME_OPERATION_CEILING",
                ParamsJson = JsonConvert.SerializeObject(new { CeilingPerOperation = 100m })
            }, context));

        Assert.Contains("SAME_OPERATION_REQUIRED", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotRequireOperationNoForPerItemCeilingOnly()
    {
        var executor = new SameOperationCeilingExecutor(new InMemoryLimitOccupyRepository());
        var context = new PricingContext
        {
            PatientId = "P001",
            ItemCode = "ITEM001",
            FinalQty = 1m,
            FinalAmount = 120m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "SAME_OPERATION_CEILING",
            ParamsJson = JsonConvert.SerializeObject(new { CeilingPerItem = 100m })
        }, context);

        Assert.Equal(100m, context.FinalAmount);
        Assert.Empty(context.PendingLimitOccupies);
    }

    private sealed class InMemoryLimitOccupyRepository : ILimitOccupyRepository
    {
        public Dictionary<string, decimal> OccupiedAmtByStatus { get; } = new(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyList<string> LockedKeys { get; private set; } = Array.Empty<string>();

        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status) => Task.FromResult(0m);

        public Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query) =>
            Task.FromResult(0m);

        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status)
        {
            OccupiedAmtByStatus.TryGetValue($"{limitKey}:{status}", out var amount);
            return Task.FromResult(amount);
        }

        public Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId) =>
            Task.FromResult((IReadOnlyList<LimitOccupy>)Array.Empty<LimitOccupy>());

        public Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys)
        {
            LockedKeys = lockKeys.ToList();
            return Task.CompletedTask;
        }

        public Task<long> InsertAsync(LimitOccupy entity) => Task.FromResult(0L);
        public Task UpdateStatusAsync(long occupyId, string status) => Task.CompletedTask;
        public Task UpdateStatusByRequestIdAsync(long requestId, string status) => Task.CompletedTask;
    }
}
