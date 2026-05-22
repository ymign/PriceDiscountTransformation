using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Api.Application.Rules;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuleApprovalAppServiceTests
{
    [Fact]
    public async Task SubmitAsync_CreatesPendingApprovalAndWritesChangeLog()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var approvalRepository = new InMemoryRuleApprovalRepository();
        var changeLogRepository = new InMemoryRuleChangeLogRepository();
        var service = CreateService(headerRepository, versionRepository, approvalRepository, changeLogRepository);

        headerRepository.Headers.Add(new RuleHeader { RuleId = 1, RuleCode = "R1", Status = "DRAFT", IsEnabled = "Y" });
        versionRepository.Versions.Add(new RuleVersion { VersionId = 11, RuleId = 1, VersionNo = 2, VersionStatus = "DRAFT" });

        var approvalId = await service.SubmitAsync(1, 2, new RuleApprovalSubmitRequest
        {
            ActionType = "publish",
            SubmittedBy = "maker",
            Remark = "提交发布审批"
        });

        var approval = Assert.Single(approvalRepository.Items);
        Assert.Equal(approvalId, approval.ApprovalId);
        Assert.Equal("PUBLISH", approval.ActionType);
        Assert.Equal("PENDING", approval.ApprovalStatus);
        Assert.Equal("maker", approval.SubmittedBy);
        Assert.Contains(changeLogRepository.Items, i => i.ChangeType == "SUBMIT_APPROVAL");
    }

    [Fact]
    public async Task ApproveAsync_UpdatesLatestPendingApproval()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var approvalRepository = new InMemoryRuleApprovalRepository();
        var changeLogRepository = new InMemoryRuleChangeLogRepository();
        var service = CreateService(headerRepository, versionRepository, approvalRepository, changeLogRepository);

        headerRepository.Headers.Add(new RuleHeader { RuleId = 2, RuleCode = "R2", Status = "DRAFT", IsEnabled = "Y" });
        versionRepository.Versions.Add(new RuleVersion { VersionId = 12, RuleId = 2, VersionNo = 1, VersionStatus = "DRAFT" });
        approvalRepository.Items.Add(new RuleApproval
        {
            ApprovalId = 100,
            RuleId = 2,
            VersionNo = 1,
            ActionType = "PUBLISH",
            ApprovalStatus = "PENDING",
            SubmittedBy = "maker",
            SubmittedAt = new DateTime(2026, 5, 22, 9, 0, 0)
        });

        await service.ApproveAsync(2, 1, new RuleApprovalDecisionRequest
        {
            ReviewedBy = "checker",
            ReviewComment = "通过"
        });

        var approval = Assert.Single(approvalRepository.Items);
        Assert.Equal("APPROVED", approval.ApprovalStatus);
        Assert.Equal("checker", approval.ReviewedBy);
        Assert.NotNull(approval.ReviewedAt);
        Assert.Contains(changeLogRepository.Items, i => i.ChangeType == "APPROVE");
    }

    [Fact]
    public async Task RejectAsync_UpdatesLatestPendingApproval()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var approvalRepository = new InMemoryRuleApprovalRepository();
        var changeLogRepository = new InMemoryRuleChangeLogRepository();
        var service = CreateService(headerRepository, versionRepository, approvalRepository, changeLogRepository);

        headerRepository.Headers.Add(new RuleHeader { RuleId = 3, RuleCode = "R3", Status = "DRAFT", IsEnabled = "Y" });
        versionRepository.Versions.Add(new RuleVersion { VersionId = 13, RuleId = 3, VersionNo = 1, VersionStatus = "DRAFT" });
        approvalRepository.Items.Add(new RuleApproval
        {
            ApprovalId = 101,
            RuleId = 3,
            VersionNo = 1,
            ActionType = "DISABLE",
            ApprovalStatus = "PENDING",
            SubmittedBy = "maker",
            SubmittedAt = new DateTime(2026, 5, 22, 9, 0, 0)
        });

        await service.RejectAsync(3, 1, new RuleApprovalDecisionRequest
        {
            ReviewedBy = "checker",
            ReviewComment = "不通过"
        });

        var approval = Assert.Single(approvalRepository.Items);
        Assert.Equal("REJECTED", approval.ApprovalStatus);
        Assert.Equal("checker", approval.ReviewedBy);
        Assert.NotNull(approval.ReviewedAt);
        Assert.Contains(changeLogRepository.Items, i => i.ChangeType == "REJECT");
    }

    private static RuleApprovalAppService CreateService(
        InMemoryRuleHeaderRepository headerRepository,
        InMemoryRuleVersionRepository versionRepository,
        InMemoryRuleApprovalRepository approvalRepository,
        InMemoryRuleChangeLogRepository changeLogRepository) =>
        new(
            headerRepository,
            versionRepository,
            approvalRepository,
            changeLogRepository,
            NullLogger<RuleApprovalAppService>.Instance);

    private sealed class InMemoryRuleHeaderRepository : IRuleHeaderRepository
    {
        public List<RuleHeader> Headers { get; } = new();
        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult(Headers.SingleOrDefault(h => h.RuleId == ruleId));
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult(Headers.SingleOrDefault(h => h.RuleId == ruleId));
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult(Headers.SingleOrDefault(h => h.RuleCode == ruleCode));
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Headers.ToList(), Headers.Count));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(entity.RuleId);
        public Task<bool> UpdateAsync(RuleHeader entity, string? expectedCurrentStatus = null) => Task.FromResult(true);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(Headers.Any(h => h.RuleCode == ruleCode));
    }

    private sealed class InMemoryRuleVersionRepository : IRuleVersionRepository
    {
        public List<RuleVersion> Versions { get; } = new();
        public Task<RuleVersion?> GetByIdAsync(long versionId) => Task.FromResult(Versions.SingleOrDefault(v => v.VersionId == versionId));
        public Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo) => Task.FromResult(Versions.SingleOrDefault(v => v.RuleId == ruleId && v.VersionNo == versionNo));
        public Task<RuleVersion?> GetByRuleAndVersionForUpdateAsync(long ruleId, int versionNo) => GetByRuleAndVersionAsync(ruleId, versionNo);
        public Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleVersion>)Versions.Where(v => v.RuleId == ruleId).ToList());
        public Task<long> InsertAsync(RuleVersion entity) => Task.FromResult(entity.VersionId);
        public Task<bool> UpdateStatusAsync(long versionId, string status, string? expectedCurrentStatus = null) => Task.FromResult(true);
    }

    private sealed class InMemoryRuleApprovalRepository : IRuleApprovalRepository
    {
        public List<RuleApproval> Items { get; } = new();

        public Task<IReadOnlyList<RuleApproval>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleApproval>)Items.Where(i => i.RuleId == ruleId).OrderByDescending(i => i.SubmittedAt).ToList());

        public Task<IReadOnlyList<RuleApproval>> GetPendingAsync() =>
            Task.FromResult((IReadOnlyList<RuleApproval>)Items.Where(i => i.ApprovalStatus == "PENDING").OrderBy(i => i.SubmittedAt).ToList());

        public Task<long> InsertAsync(RuleApproval entity)
        {
            entity.ApprovalId = entity.ApprovalId == 0 ? (Items.Count == 0 ? 1 : Items.Max(i => i.ApprovalId) + 1) : entity.ApprovalId;
            Items.Add(entity);
            return Task.FromResult(entity.ApprovalId);
        }

        public Task UpdateStatusAsync(long approvalId, string status, string reviewedBy, string reviewComment)
        {
            var item = Items.Single(i => i.ApprovalId == approvalId);
            item.ApprovalStatus = status;
            item.ReviewedBy = reviewedBy;
            item.ReviewComment = reviewComment;
            item.ReviewedAt = DateTime.Now;
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryRuleChangeLogRepository : IRuleChangeLogRepository
    {
        public List<RuleChangeLog> Items { get; } = new();

        public Task<IReadOnlyList<RuleChangeLog>> GetByRuleIdAsync(long ruleId) =>
            Task.FromResult((IReadOnlyList<RuleChangeLog>)Items.Where(i => i.RuleId == ruleId).ToList());

        public Task<long> InsertAsync(RuleChangeLog entity)
        {
            Items.Add(entity);
            return Task.FromResult(entity.ChangeId);
        }
    }
}
