using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Api.Application.Rules;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuleHeaderServiceTests
{
    [Fact]
    public async Task GetEffectiveAsync_UsesRequestedChargeTimeAndCachesByItemAndTime()
    {
        var repository = new CapturingRuleHeaderRepository();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            repository,
            new EmptyRuleChangeLogRepository(),
            cache,
            new NoopCacheVersionSynchronizer(),
            NullLogger<RuleHeaderService>.Instance);

        var firstTime = new DateTime(2026, 5, 10, 9, 0, 0);
        var secondTime = new DateTime(2026, 5, 11, 9, 0, 0);

        await service.GetEffectiveAsync("ITEM001", firstTime);
        await service.GetEffectiveAsync("ITEM001", secondTime);

        Assert.Equal(new[] { firstTime, secondTime }, repository.BusinessTimes);
    }

    [Fact]
    public async Task ClearEffectiveCache_RemovesItemSpecificCachedRules()
    {
        var repository = new CapturingRuleHeaderRepository();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            repository,
            new EmptyRuleChangeLogRepository(),
            cache,
            new NoopCacheVersionSynchronizer(),
            NullLogger<RuleHeaderService>.Instance);

        await service.GetEffectiveAsync("ITEM001", new DateTime(2026, 5, 10, 9, 0, 0));
        service.ClearEffectiveCache();
        await service.GetEffectiveAsync("ITEM001", new DateTime(2026, 5, 10, 9, 0, 0));

        Assert.Equal(2, repository.BusinessTimes.Count);
    }

    [Fact]
    public async Task CreateAsync_ReturnsRuleCodeDuplicateBizCodeWhenRuleCodeAlreadyExists()
    {
        var repository = new CapturingRuleHeaderRepository
        {
            ExistsResult = true
        };
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            repository,
            new EmptyRuleChangeLogRepository(),
            cache,
            new NoopCacheVersionSynchronizer(),
            NullLogger<RuleHeaderService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.CreateAsync(new RuleHeaderCreateRequest
            {
                RuleCode = "RULE001",
                RuleName = "重复规则",
                RuleCategory = "MIXED",
                RuleScope = "ITEM",
                ItemCode = "ITEM001"
            }));

        Assert.Equal(BizErrorCode.RuleCodeDuplicate, ex.Code);
    }

    [Fact]
    public async Task UpdateAsync_RejectsPublishedRuleMatchingFieldChanges()
    {
        var repository = new CapturingRuleHeaderRepository
        {
            Entity = new RuleHeader
            {
                RuleId = 100,
                RuleCode = "RULE001",
                RuleName = "旧名称",
                RuleCategory = "MIXED",
                RuleScope = "ITEM",
                ItemCode = "ITEM001",
                ItemName = "旧项目",
                GroupCode = "GRP001",
                Priority = 10,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                CurrentVersion = 1,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                RollbackMode = "STOP_CHARGE",
                CreatedAt = new DateTime(2026, 5, 1),
                UpdatedAt = new DateTime(2026, 5, 1)
            }
        };
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            repository,
            new EmptyRuleChangeLogRepository(),
            cache,
            new NoopCacheVersionSynchronizer(),
            NullLogger<RuleHeaderService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.UpdateAsync(100, new RuleHeaderUpdateRequest
            {
                RuleName = "新名称",
                RuleCategory = "MIXED",
                RuleScope = "ITEM",
                ItemCode = "ITEM002",
                ItemName = "新项目",
                GroupCode = "GRP001",
                Priority = 10,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                RollbackMode = "STOP_CHARGE",
                Remark = "尝试修改已发布规则项目"
            }));

        Assert.Equal(BizErrorCode.VersionStatusNotAllowed, ex.Code);
        Assert.False(repository.WasUpdated);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsRuleNotFoundBizCodeWhenHeaderIsMissing()
    {
        var repository = new CapturingRuleHeaderRepository();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            repository,
            new EmptyRuleChangeLogRepository(),
            cache,
            new NoopCacheVersionSynchronizer(),
            NullLogger<RuleHeaderService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.UpdateAsync(999, new RuleHeaderUpdateRequest
            {
                RuleName = "不存在",
                RuleCategory = "MIXED",
                RuleScope = "ITEM",
                ItemCode = "ITEM999"
            }));

        Assert.Equal(BizErrorCode.RuleNotFound, ex.Code);
    }

    [Fact]
    public async Task UpdateAsync_RejectsWhenPublishApprovalIsPending()
    {
        var repository = new CapturingRuleHeaderRepository
        {
            Entity = new RuleHeader
            {
                RuleId = 102,
                RuleCode = "RULE102",
                RuleName = "待审规则",
                RuleCategory = "MIXED",
                RuleScope = "ITEM",
                ItemCode = "ITEM102",
                Status = "DRAFT",
                IsEnabled = "Y"
            }
        };
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            repository,
            new ApprovalAwareRuleChangeLogRepository(new RuleApproval
            {
                ApprovalId = 5001,
                RuleId = 102,
                VersionNo = 1,
                ActionType = "PUBLISH",
                ApprovalStatus = "PENDING",
                SubmittedBy = "maker",
                SubmittedAt = new DateTime(2026, 5, 22, 9, 0, 0)
            }),
            cache,
            new NoopCacheVersionSynchronizer(),
            NullLogger<RuleHeaderService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            service.UpdateAsync(102, new RuleHeaderUpdateRequest
            {
                RuleName = "待审规则-修改",
                RuleCategory = "MIXED",
                RuleScope = "ITEM",
                ItemCode = "ITEM102"
            }));

        Assert.Equal(BizErrorCode.ApprovalPendingEditNotAllowed, ex.Code);
        Assert.False(repository.WasUpdated);
    }

    [Fact]
    public async Task UpdateAsync_AllowsPublishedRuleDisplayFieldChanges()
    {
        var repository = new CapturingRuleHeaderRepository
        {
            Entity = new RuleHeader
            {
                RuleId = 101,
                RuleCode = "RULE002",
                RuleName = "旧名称",
                RuleCategory = "MIXED",
                RuleScope = "ITEM",
                ItemCode = "ITEM001",
                ItemName = "旧项目",
                GroupCode = "GRP001",
                Priority = 10,
                Status = "PUBLISHED",
                IsEnabled = "Y",
                CurrentVersion = 1,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                RollbackMode = "STOP_CHARGE",
                CreatedAt = new DateTime(2026, 5, 1),
                UpdatedAt = new DateTime(2026, 5, 1)
            }
        };
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new RuleHeaderService(
            repository,
            new EmptyRuleChangeLogRepository(),
            cache,
            new NoopCacheVersionSynchronizer(),
            NullLogger<RuleHeaderService>.Instance);

        await service.UpdateAsync(101, new RuleHeaderUpdateRequest
        {
            RuleName = "新名称",
            RuleCategory = "MIXED",
            RuleScope = "ITEM",
            ItemCode = "ITEM001",
            ItemName = "新项目展示名",
            GroupCode = "GRP001",
            Priority = 10,
            EffectiveFrom = new DateTime(2026, 5, 1),
            EffectiveTo = new DateTime(2026, 5, 31),
            RollbackMode = "STOP_CHARGE",
            Remark = "只改展示字段",
            UpdatedBy = "tester"
        });

        Assert.True(repository.WasUpdated);
        Assert.Equal("新名称", repository.Entity.RuleName);
        Assert.Equal("新项目展示名", repository.Entity.ItemName);
    }

    private sealed class CapturingRuleHeaderRepository : IRuleHeaderRepository
    {
        public List<DateTime> BusinessTimes { get; } = new();
        public RuleHeader? Entity { get; set; }
        public bool WasUpdated { get; private set; }
        public bool ExistsResult { get; set; }

        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult(Entity?.RuleId == ruleId ? Entity : null);
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult(Entity?.RuleId == ruleId ? Entity : null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(null);
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));

        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime)
        {
            BusinessTimes.Add(businessTime);
            return Task.FromResult((IReadOnlyList<RuleHeader>)new[]
            {
                new RuleHeader
                {
                    RuleId = businessTime.Day,
                    RuleCode = $"RULE-{businessTime:yyyyMMdd}",
                    RuleName = "测试规则",
                    RuleCategory = "MIXED",
                    RuleScope = "ITEM",
                    ItemCode = "ITEM001",
                    Priority = 10,
                    CurrentVersion = 1,
                    Status = "PUBLISHED",
                    IsEnabled = "Y",
                    CreatedAt = businessTime,
                    UpdatedAt = businessTime
                }
            });
        }

        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleHeader entity, string? expectedCurrentStatus = null)
        {
            Entity = entity;
            WasUpdated = true;
            return Task.FromResult(true);
        }
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(ExistsResult);
    }

    private sealed class EmptyRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());
        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
    }

    private sealed class ApprovalAwareRuleChangeLogRepository : IRuleChangeLogRepository
    {
        private readonly RuleApproval _pendingApproval;

        public ApprovalAwareRuleChangeLogRepository(RuleApproval pendingApproval)
        {
            _pendingApproval = pendingApproval;
        }

        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId)
        {
            if (ruleId != _pendingApproval.RuleId)
            {
                return Task.FromResult((IReadOnlyList<RuleChangeLog>)Array.Empty<RuleChangeLog>());
            }

            var logs = new List<RuleChangeLog>
            {
                new RuleChangeLog
                {
                    RuleId = _pendingApproval.RuleId,
                    VersionNo = _pendingApproval.VersionNo,
                    ChangeType = "SUBMIT_APPROVAL",
                    ChangeSummary = _pendingApproval.ActionType,
                    ChangedAt = _pendingApproval.SubmittedAt
                }
            };
            return Task.FromResult((IReadOnlyList<RuleChangeLog>)logs);
        }

        public Task<long> InsertAsync(RuleChangeLog entity) => Task.FromResult(0L);
    }

    private sealed class NoopCacheVersionSynchronizer : ICacheVersionSynchronizer
    {
        public Task SyncAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<long> IncreaseVersionAsync(string cacheScope, CancellationToken cancellationToken = default) =>
            Task.FromResult(0L);
    }
}
