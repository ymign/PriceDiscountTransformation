using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class OnceQtyLimitExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_UsesChargingActionDimensionAndInRequestOccupiedQty()
    {
        var repository = new InMemoryLimitOccupyRepository
        {
            OccupiedByStatus =
            {
                ["OQ:HIS:BIZ-001:ITEM001:PENDING"] = 1m,
                ["OQ:HIS:BIZ-001:ITEM001:CONFIRMED"] = 2m
            }
        };
        var executor = new OnceQtyLimitExecutor(repository);
        var context = new PricingContext
        {
            SourceSystem = "HIS",
            BusinessRequestNo = "BIZ-001",
            PatientId = "P001",
            ItemCode = "ITEM001",
            InputQty = 4m,
            FinalQty = 4m,
            UnitPrice = 10m,
            FinalAmount = 40m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0),
            ShouldLockLimits = true,
            InRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>
            {
                ["ONCE_QTY:HIS:BIZ-001:ITEM001"] = 1m
            }
        };
        var action = new RuleAction
        {
            ActionType = "APPLY_ONCE_LIMIT_QTY",
            ParamsJson = JsonConvert.SerializeObject(new { MaxOnceQty = 5m })
        };

        await executor.ExecuteAsync(action, context);

        Assert.Equal(1m, context.FinalQty);
        Assert.Equal(10m, context.FinalAmount);
        Assert.Equal(new[] { "OQ:HIS:BIZ-001:ITEM001" }, repository.LockedKeys);
        var occupy = Assert.Single(context.PendingLimitOccupies);
        Assert.Equal("ONCE_QTY", occupy.LimitType);
        Assert.Equal("HIS:BIZ-001:ITEM001", occupy.LimitDimensionCode);
        Assert.Equal("OQ:HIS:BIZ-001:ITEM001", occupy.LimitKey);
    }

    [Fact]
    public async Task ExecuteAsync_MaxOnceQtyZero_MeansNoChargeableQty()
    {
        var repository = new InMemoryLimitOccupyRepository();
        var executor = new OnceQtyLimitExecutor(repository);
        var context = new PricingContext
        {
            SourceSystem = "HIS",
            BusinessRequestNo = "BIZ-ZERO",
            PatientId = "P001",
            ItemCode = "ITEM001",
            InputQty = 3m,
            FinalQty = 3m,
            UnitPrice = 10m,
            FinalAmount = 30m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0)
        };
        var action = new RuleAction
        {
            ActionType = "APPLY_ONCE_LIMIT_QTY",
            ParamsJson = JsonConvert.SerializeObject(new { MaxOnceQty = 0m })
        };

        await executor.ExecuteAsync(action, context);

        Assert.Equal(0m, context.FinalQty);
        Assert.Equal(0m, context.FinalAmount);
        var occupy = Assert.Single(context.PendingLimitOccupies);
        Assert.Equal("ONCE_QTY", occupy.LimitType);
    }

    private sealed class InMemoryLimitOccupyRepository : ILimitOccupyRepository
    {
        public Dictionary<string, decimal> OccupiedByStatus { get; } = new(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyList<string> LockedKeys { get; private set; } = Array.Empty<string>();

        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status)
        {
            OccupiedByStatus.TryGetValue($"{limitKey}:{status}", out var qty);
            return Task.FromResult(qty);
        }

        public Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query) =>
            Task.FromResult(0m);

        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtByDimensionAsync(string dimensionCode, string status) => Task.FromResult(0m);
        public Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<LimitOccupy>)Array.Empty<LimitOccupy>());
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
