using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ExpireCleanupAppServiceTests
{
    [Fact]
    public async Task CleanupExpiredAsync_UsesInjectedClockForThresholdAndResponseTime()
    {
        var now = new DateTime(2026, 5, 22, 10, 30, 0);
        var requestRepository = new CapturingChargeRequestLogRepository();
        var discountRepository = new CapturingChargeDiscountDetailRepository();
        var limitRepository = new CapturingLimitOccupyRepository();
        var unitOfWork = new CapturingUnitOfWork();
        var log = new ChargeRequestLog
        {
            RequestId = 301,
            RequestAt = now.AddHours(-1),
            BusinessStatus = BusinessStatusCodes.ConfirmPending
        };
        requestRepository.Items.Add(log);
        var service = CreateService(
            now,
            requestRepository,
            discountRepository,
            limitRepository,
            unitOfWork);

        await InvokeCleanupExpiredAsync(service);

        Assert.Equal(now.AddMinutes(-30), requestRepository.LastExpireBefore);
        Assert.Equal(BusinessStatusCodes.Expired, log.BusinessStatus);
        Assert.Equal(now, log.ResponseAt);
        Assert.Equal((301L, BusinessStatusCodes.Expired), discountRepository.LastStatusUpdate);
        Assert.Equal((301L, BusinessStatusCodes.Expired), limitRepository.LastStatusUpdate);
        Assert.Contains("REQ:301", limitRepository.LockedKeys);
        Assert.Equal(1, unitOfWork.BeginCount);
        Assert.Equal(1, unitOfWork.CommitCount);
        Assert.Equal(0, unitOfWork.RollbackCount);
    }

    private static ExpireCleanupAppService CreateService(
        DateTime now,
        IChargeRequestLogRepository requestRepository,
        IChargeDiscountDetailRepository discountRepository,
        ILimitOccupyRepository limitRepository,
        IUnitOfWork unitOfWork)
    {
        var services = new ServiceCollection();
        services.AddSingleton(requestRepository);
        services.AddSingleton(discountRepository);
        services.AddSingleton(limitRepository);
        services.AddSingleton(unitOfWork);
        services.AddSingleton<IClock>(new FixedClock(now));
        var provider = services.BuildServiceProvider();

        return new ExpireCleanupAppService(
            provider.GetRequiredService<IServiceScopeFactory>(),
            Options.Create(new PricingOptions
            {
                ConfirmExpireMinutes = 30,
                ExpireCleanupIntervalSeconds = 300
            }),
            NullLogger<ExpireCleanupAppService>.Instance);
    }

    private static async Task InvokeCleanupExpiredAsync(ExpireCleanupAppService service)
    {
        var method = typeof(ExpireCleanupAppService).GetMethod(
            "CleanupExpiredAsync",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        var task = (Task)method.Invoke(service, Array.Empty<object>())!;
        await task;
    }

    private sealed class FixedClock : IClock
    {
        public FixedClock(DateTime now)
        {
            Now = now;
        }

        public DateTime Now { get; }
    }

    private sealed class CapturingUnitOfWork : IUnitOfWork
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

    private sealed class CapturingChargeRequestLogRepository : IChargeRequestLogRepository
    {
        public List<ChargeRequestLog> Items { get; } = new();
        public DateTime? LastExpireBefore { get; private set; }

        public Task<ChargeRequestLog?> GetByIdAsync(long requestId) =>
            Task.FromResult<ChargeRequestLog?>(Items.SingleOrDefault(item => item.RequestId == requestId));

        public Task<ChargeRequestLog?> GetByBusinessKeyAsync(
            string sourceSystem,
            string businessRequestNo,
            string callType) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<ChargeRequestLog?> GetByFingerprintAsync(string fingerprint) =>
            Task.FromResult<ChargeRequestLog?>(null);

        public Task<long> InsertAsync(ChargeRequestLog entity) => Task.FromResult(0L);

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

        public Task<IReadOnlyList<ChargeRequestLog>> GetPendingExpiredAsync(DateTime expireBefore)
        {
            LastExpireBefore = expireBefore;
            return Task.FromResult((IReadOnlyList<ChargeRequestLog>)Items.ToList());
        }
    }

    private sealed class CapturingChargeDiscountDetailRepository : IChargeDiscountDetailRepository
    {
        public (long RequestId, string Status)? LastStatusUpdate { get; private set; }

        public Task<IReadOnlyList<ChargeDiscountDetail>> GetByRequestIdAsync(long requestId) =>
            Task.FromResult((IReadOnlyList<ChargeDiscountDetail>)Array.Empty<ChargeDiscountDetail>());

        public Task<long> InsertAsync(ChargeDiscountDetail entity) => Task.FromResult(0L);

        public Task UpdateStatusByRequestIdAsync(long requestId, string status)
        {
            LastStatusUpdate = (requestId, status);
            return Task.CompletedTask;
        }
    }

    private sealed class CapturingLimitOccupyRepository : ILimitOccupyRepository
    {
        public List<string> LockedKeys { get; } = new();
        public (long RequestId, string Status)? LastStatusUpdate { get; private set; }

        public Task<decimal> GetOccupiedQtyAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedQtyAsync(LimitOccupyRangeQuery query) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtAsync(string limitKey, string status) => Task.FromResult(0m);
        public Task<decimal> GetOccupiedAmtByDimensionAsync(string dimensionCode, string status) => Task.FromResult(0m);
        public Task<IReadOnlyList<LimitOccupy>> GetByRequestIdAsync(long requestId) => Task.FromResult((IReadOnlyList<LimitOccupy>)Array.Empty<LimitOccupy>());

        public Task EnsureAndLockAsync(IReadOnlyCollection<string> lockKeys)
        {
            LockedKeys.AddRange(lockKeys);
            return Task.CompletedTask;
        }

        public Task<long> InsertAsync(LimitOccupy entity) => Task.FromResult(0L);
        public Task UpdateStatusAsync(long occupyId, string status) => Task.CompletedTask;

        public Task UpdateStatusByRequestIdAsync(long requestId, string status)
        {
            LastStatusUpdate = (requestId, status);
            return Task.CompletedTask;
        }
    }
}
