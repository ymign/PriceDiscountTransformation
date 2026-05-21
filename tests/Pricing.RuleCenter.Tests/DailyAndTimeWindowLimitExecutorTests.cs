using Newtonsoft.Json;
using Pricing.RuleCenter.Core.Engine.Executors;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class DailyAndTimeWindowLimitExecutorTests
{
    [Fact]
    public async Task DailyQtyLimitExecutor_UsesInRequestOccupiedQty()
    {
        var repository = new InMemoryLimitOccupyRepository { OccupiedQty = 1m };
        var executor = new DailyQtyLimitExecutor(repository);
        var context = new PricingContext
        {
            PatientId = "P001",
            ItemCode = "ITEM001",
            InputQty = 4m,
            FinalQty = 4m,
            UnitPrice = 10m,
            FinalAmount = 40m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0),
            InRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>
            {
                ["DAY_QTY:P001:ITEM001:20260510"] = 2m
            }
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "APPLY_DAY_LIMIT_QTY",
            ParamsJson = JsonConvert.SerializeObject(new { MaxDailyQty = 5m })
        }, context);

        Assert.Equal(2m, context.FinalQty);
        Assert.Equal(20m, context.FinalAmount);
    }

    [Fact]
    public async Task TimeWindowLimitExecutor_UsesInRequestOccupiedQty()
    {
        var repository = new InMemoryLimitOccupyRepository { OccupiedQty = 1m };
        var executor = new TimeWindowLimitExecutor(repository);
        var context = new PricingContext
        {
            PatientId = "P001",
            ItemCode = "ITEM001",
            InputQty = 4m,
            FinalQty = 4m,
            UnitPrice = 10m,
            FinalAmount = 40m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0),
            InRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>
            {
                ["TIME_WINDOW:P001:ITEM001"] = 2m
            }
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "APPLY_TIME_WINDOW_LIMIT",
            ParamsJson = JsonConvert.SerializeObject(new { WindowMinutes = 120, LimitQty = 5m })
        }, context);

        Assert.Equal(2m, context.FinalQty);
        Assert.Equal(20m, context.FinalAmount);
    }

    [Fact]
    public async Task TimeWindowLimitExecutor_LegacyOccupiedQty_计入窗口累计()
    {
        // DB返回1，InRequest返回1，LegacyOccupied=2，合计4，limit=5 → 剩余1，本次4个截断为1
        var repository = new InMemoryLimitOccupyRepository { OccupiedQty = 1m };
        var executor = new TimeWindowLimitExecutor(repository);
        var context = new PricingContext
        {
            PatientId = "P001",
            ItemCode = "ITEM001",
            InputQty = 4m,
            FinalQty = 4m,
            UnitPrice = 10m,
            FinalAmount = 40m,
            LegacyOccupiedQty = 2m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0),
            InRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>
            {
                ["TIME_WINDOW:P001:ITEM001"] = 1m
            }
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "APPLY_TIME_WINDOW_LIMIT",
            ParamsJson = JsonConvert.SerializeObject(new { WindowMinutes = 120, LimitQty = 5m })
        }, context);

        // 已占用 = 1(DB) + 1(InRequest) + 2(Legacy) = 4，剩余 = 5-4=1
        Assert.Equal(1m, context.FinalQty);
        Assert.Equal(10m, context.FinalAmount);
    }

    [Fact]
    public async Task TimeWindowLimitExecutor_LegacyOccupiedQty为零_行为不变()
    {
        var repository = new InMemoryLimitOccupyRepository { OccupiedQty = 1m };
        var executor = new TimeWindowLimitExecutor(repository);
        var context = new PricingContext
        {
            PatientId = "P001",
            ItemCode = "ITEM001",
            InputQty = 4m,
            FinalQty = 4m,
            UnitPrice = 10m,
            FinalAmount = 40m,
            LegacyOccupiedQty = 0m,
            BusinessChargeTime = new DateTime(2026, 5, 10, 10, 0, 0),
            InRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>
            {
                ["TIME_WINDOW:P001:ITEM001"] = 2m
            }
        };

        await executor.ExecuteAsync(new RuleAction
        {
            ActionType = "APPLY_TIME_WINDOW_LIMIT",
            ParamsJson = JsonConvert.SerializeObject(new { WindowMinutes = 120, LimitQty = 5m })
        }, context);

        // 已占用 = 1(DB) + 2(InRequest) + 0(Legacy) = 3，剩余 = 2，本次4个截断为2
        Assert.Equal(2m, context.FinalQty);
    }

    private sealed class InMemoryLimitOccupyRepository : ILimitOccupyRepository
    {
        public decimal OccupiedQty { get; init; }

        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status) => Task.FromResult(0m);

        public Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query) =>
            Task.FromResult(OccupiedQty);

        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status) => Task.FromResult(0m);

        public Task<decimal> GetOccupiedAmtByDimensionAsync(string dimensionCode, string status) => Task.FromResult(0m);

        public Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId) =>
            Task.FromResult((IReadOnlyList<LimitOccupy>)Array.Empty<LimitOccupy>());

        public Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys) => Task.CompletedTask;

        public Task<long> InsertAsync(LimitOccupy entity) => Task.FromResult(0L);

        public Task UpdateStatusAsync(long occupyId, string status) => Task.CompletedTask;

        public Task UpdateStatusByRequestIdAsync(long requestId, string status) => Task.CompletedTask;
    }
}
