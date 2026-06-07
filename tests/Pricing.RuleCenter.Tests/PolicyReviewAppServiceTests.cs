using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PolicyReviewAppServiceTests
{
    [Fact]
    public async Task SubmitAsync_MarksVersionReviewPendingAndCreatesReview()
    {
        var policyRepository = new InMemoryPolicyRepository();
        policyRepository.Versions[101] = new PolicyVersion
        {
            PolicyVersionId = 101,
            PolicyId = 1,
            VersionNo = 1,
            PolicyStatus = PolicyLifecycleCodes.Validated,
            Checksum = "CHK-101"
        };
        var reviewRepository = new InMemoryPolicyReviewRepository();
        var service = new PolicyReviewAppService(
            policyRepository,
            reviewRepository,
            new NoopUnitOfWork(),
            new FixedClock(new DateTime(2026, 6, 7, 9, 0, 0)));

        var reviewId = await service.SubmitAsync(101, "maker", "NORMAL");

        Assert.True(reviewId > 0);
        Assert.Equal(PolicyLifecycleCodes.ReviewPending, policyRepository.Versions[101].PolicyStatus);
        var review = Assert.Single(reviewRepository.Items);
        Assert.Equal(PolicyReviewStatusCodes.Pending, review.ReviewStatus);
        Assert.Equal("CHK-101", review.SourceChecksum);
    }

    [Fact]
    public async Task ApproveAsync_MarksReviewOutdatedWhenChecksumChanged()
    {
        var policyRepository = new InMemoryPolicyRepository();
        policyRepository.Versions[102] = new PolicyVersion
        {
            PolicyVersionId = 102,
            PolicyId = 1,
            VersionNo = 1,
            PolicyStatus = PolicyLifecycleCodes.ReviewPending,
            Checksum = "CHK-NEW"
        };
        var reviewRepository = new InMemoryPolicyReviewRepository();
        reviewRepository.Items.Add(new PolicyReview
        {
            ReviewId = 1,
            PolicyVersionId = 102,
            ReviewStatus = PolicyReviewStatusCodes.Pending,
            ReviewStage = "NORMAL",
            SourceChecksum = "CHK-OLD"
        });
        var service = new PolicyReviewAppService(
            policyRepository,
            reviewRepository,
            new NoopUnitOfWork(),
            new FixedClock(new DateTime(2026, 6, 7, 9, 30, 0)));

        var ex = await Assert.ThrowsAsync<BizException>(() => service.ApproveAsync(102, "checker", "通过"));

        Assert.Equal(BizErrorCode.PolicyReviewOutdated, ex.Code);
        Assert.Equal(PolicyReviewStatusCodes.Outdated, reviewRepository.Items.Single().ReviewStatus);
    }

    [Fact]
    public async Task ApproveAsync_MarksReviewApprovedAndVersionApproved()
    {
        var policyRepository = new InMemoryPolicyRepository();
        policyRepository.Versions[103] = new PolicyVersion
        {
            PolicyVersionId = 103,
            PolicyId = 1,
            VersionNo = 1,
            PolicyStatus = PolicyLifecycleCodes.ReviewPending,
            Checksum = "CHK-103"
        };
        var reviewRepository = new InMemoryPolicyReviewRepository();
        reviewRepository.Items.Add(new PolicyReview
        {
            ReviewId = 2,
            PolicyVersionId = 103,
            ReviewStatus = PolicyReviewStatusCodes.Pending,
            ReviewStage = "NORMAL",
            SourceChecksum = "CHK-103"
        });
        var service = new PolicyReviewAppService(
            policyRepository,
            reviewRepository,
            new NoopUnitOfWork(),
            new FixedClock(new DateTime(2026, 6, 7, 10, 0, 0)));

        await service.ApproveAsync(103, "checker", "通过");

        Assert.Equal(PolicyLifecycleCodes.Approved, policyRepository.Versions[103].PolicyStatus);
        var review = reviewRepository.Items.Single();
        Assert.Equal(PolicyReviewStatusCodes.Approved, review.ReviewStatus);
        Assert.Equal("checker", review.ReviewedBy);
    }

    [Fact]
    public async Task RejectAsync_MarksReviewRejectedAndVersionDraft()
    {
        var policyRepository = new InMemoryPolicyRepository();
        policyRepository.Versions[104] = new PolicyVersion
        {
            PolicyVersionId = 104,
            PolicyId = 1,
            VersionNo = 1,
            PolicyStatus = PolicyLifecycleCodes.ReviewPending,
            Checksum = "CHK-104"
        };
        var reviewRepository = new InMemoryPolicyReviewRepository();
        reviewRepository.Items.Add(new PolicyReview
        {
            ReviewId = 3,
            PolicyVersionId = 104,
            ReviewStatus = PolicyReviewStatusCodes.Pending,
            ReviewStage = "NORMAL",
            SourceChecksum = "CHK-104"
        });
        var service = new PolicyReviewAppService(
            policyRepository,
            reviewRepository,
            new NoopUnitOfWork(),
            new FixedClock(new DateTime(2026, 6, 7, 10, 30, 0)));

        await service.RejectAsync(104, "checker", "驳回");

        Assert.Equal(PolicyLifecycleCodes.Draft, policyRepository.Versions[104].PolicyStatus);
        Assert.Equal(PolicyReviewStatusCodes.Rejected, reviewRepository.Items.Single().ReviewStatus);
    }

    private sealed class InMemoryPolicyRepository : IPolicyRepository
    {
        public Dictionary<long, PolicyVersion> Versions { get; } = new();

        public Task<IReadOnlyList<PolicyAggregate>> GetAllAsync() =>
            Task.FromResult((IReadOnlyList<PolicyAggregate>)Array.Empty<PolicyAggregate>());

        public Task<PolicyAggregate?> GetByIdAsync(long policyId) =>
            Task.FromResult<PolicyAggregate?>(new PolicyAggregate { PolicyId = policyId, PolicyCode = $"POL{policyId:000}" });

        public Task<PolicyAggregate?> GetByCodeAsync(string policyCode) => Task.FromResult<PolicyAggregate?>(null);

        public Task<IReadOnlyList<PolicyVersion>> GetVersionsByPolicyIdAsync(long policyId) =>
            Task.FromResult((IReadOnlyList<PolicyVersion>)Versions.Values
                .Where(version => version.PolicyId == policyId)
                .ToList());

        public Task<PolicyVersion?> GetVersionAsync(long policyVersionId) =>
            Task.FromResult(Versions.TryGetValue(policyVersionId, out var version) ? version : null);

        public Task<IReadOnlyList<PolicyBinding>> GetBindingsAsync(long policyVersionId) =>
            Task.FromResult((IReadOnlyList<PolicyBinding>)Array.Empty<PolicyBinding>());

        public Task<IReadOnlyList<PolicyScope>> GetScopesAsync(long policyVersionId) =>
            Task.FromResult((IReadOnlyList<PolicyScope>)Array.Empty<PolicyScope>());

        public Task<IReadOnlyList<PolicyParam>> GetParamsAsync(long policyVersionId) =>
            Task.FromResult((IReadOnlyList<PolicyParam>)Array.Empty<PolicyParam>());

        public Task<IReadOnlyList<PolicyVersion>> GetPublishReadyVersionsAsync() =>
            Task.FromResult((IReadOnlyList<PolicyVersion>)Versions.Values
                .Where(version => string.Equals(version.PolicyStatus, PolicyLifecycleCodes.PublishReady, StringComparison.OrdinalIgnoreCase))
                .ToList());

        public Task<long> InsertAsync(PolicyAggregate entity) => Task.FromResult(0L);

        public Task UpdateAsync(PolicyAggregate entity) => Task.CompletedTask;

        public Task<long> InsertVersionAsync(PolicyVersion entity)
        {
            Versions[entity.PolicyVersionId] = entity;
            return Task.FromResult(entity.PolicyVersionId);
        }

        public Task UpdateVersionAsync(PolicyVersion entity)
        {
            Versions[entity.PolicyVersionId] = entity;
            return Task.CompletedTask;
        }

        public Task ReplaceBindingsAsync(long policyVersionId, IReadOnlyList<PolicyBinding> entities) => Task.CompletedTask;

        public Task ReplaceScopesAsync(long policyVersionId, IReadOnlyList<PolicyScope> entities) => Task.CompletedTask;

        public Task ReplaceParamsAsync(long policyVersionId, IReadOnlyList<PolicyParam> entities) => Task.CompletedTask;
    }

    private sealed class InMemoryPolicyReviewRepository : IPolicyReviewRepository
    {
        public List<PolicyReview> Items { get; } = new();
        private long _nextId = 100;

        public Task<PolicyReview?> GetLatestByPolicyVersionIdAsync(long policyVersionId) =>
            Task.FromResult(Items
                .Where(item => item.PolicyVersionId == policyVersionId)
                .OrderByDescending(item => item.SubmittedAt ?? DateTime.MinValue)
                .ThenByDescending(item => item.ReviewId)
                .FirstOrDefault());

        public Task<IReadOnlyList<PolicyReview>> GetByPolicyVersionIdAsync(long policyVersionId) =>
            Task.FromResult((IReadOnlyList<PolicyReview>)Items
                .Where(item => item.PolicyVersionId == policyVersionId)
                .OrderBy(item => item.ReviewId)
                .ToList());

        public Task<long> InsertAsync(PolicyReview entity)
        {
            entity.ReviewId = _nextId++;
            Items.Add(entity);
            return Task.FromResult(entity.ReviewId);
        }

        public Task UpdateAsync(PolicyReview entity)
        {
            var index = Items.FindIndex(item => item.ReviewId == entity.ReviewId);
            if (index >= 0)
            {
                Items[index] = entity;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class NoopUnitOfWork : IUnitOfWork
    {
        public Task BeginAsync() => Task.CompletedTask;
        public Task CommitAsync() => Task.CompletedTask;
        public Task RollbackAsync() => Task.CompletedTask;
        public void Dispose() { }
    }
}
