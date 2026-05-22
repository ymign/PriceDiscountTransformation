using Microsoft.Extensions.Logging.Abstractions;
using Pricing.RuleCenter.Api.Application.Rules;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuleVersionAppServiceTests
{
    [Fact]
    public async Task CreateDraftAsync_ReturnsRuleNotFoundBizCodeWhenHeaderIsMissing()
    {
        var service = new RuleVersionAppService(
            new InMemoryRuleVersionRepository(),
            new InMemoryRuleHeaderRepository(),
            NullLogger<RuleVersionAppService>.Instance);

        var ex = await Assert.ThrowsAsync<BizException>(() => service.CreateDraftAsync(999));

        Assert.Equal(BizErrorCode.RuleNotFound, ex.Code);
    }

    [Fact]
    public async Task CreateDraftAsync_RejectsWhenDraftVersionAlreadyExists()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var service = new RuleVersionAppService(
            versionRepository,
            headerRepository,
            NullLogger<RuleVersionAppService>.Instance);

        headerRepository.Items.Add(new RuleHeader
        {
            RuleId = 10,
            RuleCode = "R10",
            Status = "DRAFT",
            IsEnabled = "Y"
        });
        versionRepository.Items.AddRange(new[]
        {
            new RuleVersion { VersionId = 1, RuleId = 10, VersionNo = 1, VersionStatus = "PUBLISHED" },
            new RuleVersion { VersionId = 2, RuleId = 10, VersionNo = 2, VersionStatus = "DRAFT" }
        });

        var ex = await Assert.ThrowsAsync<BizException>(() => service.CreateDraftAsync(10));

        Assert.Equal(BizErrorCode.DraftVersionAlreadyExists, ex.Code);
    }

    [Fact]
    public async Task CreateDraftAsync_CreatesNextDraftVersionWhenNoDraftExists()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var versionRepository = new InMemoryRuleVersionRepository();
        var service = new RuleVersionAppService(
            versionRepository,
            headerRepository,
            NullLogger<RuleVersionAppService>.Instance);

        headerRepository.Items.Add(new RuleHeader
        {
            RuleId = 11,
            RuleCode = "R11",
            Status = "PUBLISHED",
            IsEnabled = "Y",
            EffectiveFrom = new DateTime(2026, 5, 1),
            EffectiveTo = new DateTime(2026, 5, 31)
        });
        versionRepository.Items.Add(new RuleVersion
        {
            VersionId = 101,
            RuleId = 11,
            VersionNo = 1,
            VersionStatus = "PUBLISHED"
        });

        var versionId = await service.CreateDraftAsync(11);

        var created = Assert.Single(versionRepository.Inserted);
        Assert.Equal(versionId, created.VersionId);
        Assert.Equal(2, created.VersionNo);
        Assert.Equal("DRAFT", created.VersionStatus);
        Assert.Equal(new DateTime(2026, 5, 1), created.EffectiveFrom);
        Assert.Equal(new DateTime(2026, 5, 31), created.EffectiveTo);
    }

    private sealed class InMemoryRuleHeaderRepository : IRuleHeaderRepository
    {
        public List<RuleHeader> Items { get; } = new();

        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult(Items.SingleOrDefault(i => i.RuleId == ruleId));
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult(Items.SingleOrDefault(i => i.RuleId == ruleId));
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult(Items.SingleOrDefault(i => i.RuleCode == ruleCode));
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(entity.RuleId);
        public Task<bool> UpdateAsync(RuleHeader entity, string? expectedCurrentStatus = null) => Task.FromResult(true);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }

    private sealed class InMemoryRuleVersionRepository : IRuleVersionRepository
    {
        public List<RuleVersion> Items { get; } = new();
        public List<RuleVersion> Inserted { get; } = new();

        public Task<RuleVersion?> GetByIdAsync(long versionId) => Task.FromResult(Items.SingleOrDefault(i => i.VersionId == versionId));
        public Task<RuleVersion?> GetByRuleAndVersionAsync(long ruleId, int versionNo) => Task.FromResult(Items.SingleOrDefault(i => i.RuleId == ruleId && i.VersionNo == versionNo));
        public Task<RuleVersion?> GetByRuleAndVersionForUpdateAsync(long ruleId, int versionNo) => GetByRuleAndVersionAsync(ruleId, versionNo);
        public Task<IReadOnlyList<RuleVersion>> GetByRuleIdAsync(long ruleId) => Task.FromResult((IReadOnlyList<RuleVersion>)Items.Where(i => i.RuleId == ruleId).OrderBy(i => i.VersionNo).ToList());

        public Task<long> InsertAsync(RuleVersion entity)
        {
            var toInsert = new RuleVersion
            {
                VersionId = entity.VersionId == 0 ? (Items.Count == 0 ? 1 : Items.Max(i => i.VersionId) + 1) : entity.VersionId,
                RuleId = entity.RuleId,
                VersionNo = entity.VersionNo,
                VersionStatus = entity.VersionStatus,
                EffectiveFrom = entity.EffectiveFrom,
                EffectiveTo = entity.EffectiveTo,
                RuleSnapshot = entity.RuleSnapshot,
                PublishedBy = entity.PublishedBy,
                PublishedAt = entity.PublishedAt,
                PublishRemark = entity.PublishRemark
            };
            Items.Add(toInsert);
            Inserted.Add(toInsert);
            return Task.FromResult(toInsert.VersionId);
        }

        public Task<bool> UpdateStatusAsync(long versionId, string status, string? expectedCurrentStatus = null) => Task.FromResult(true);
    }
}
