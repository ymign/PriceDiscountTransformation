using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Engine.Formula;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Templates;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuntimePackageActivationServiceTests
{
    [Fact]
    public async Task ActivateAsync_SwitchesActivePackageAndWritesOutbox()
    {
        var packageRepository = new RuntimePackagePublishServiceTests.InMemoryRuntimePackageRepository();
        packageRepository.Packages[1] = new RuntimePackage { PackageId = 1, PackageVersion = 1, PackageStatus = RuntimePackageStatusCodes.Active };
        packageRepository.Packages[2] = new RuntimePackage { PackageId = 2, PackageVersion = 2, PackageStatus = RuntimePackageStatusCodes.Built };
        var stateRepository = new RuntimePackagePublishServiceTests.InMemoryRuntimePackageStateRepository(new RuntimePackageState
        {
            StateCode = RuntimePackageStateCodes.Active,
            ActivePackageId = 1,
            ActivePackageVersion = 1
        });
        var cacheVersionRepository = new RuntimePackagePublishServiceTests.InMemoryCacheVersionRepository();
        var outboxRepository = new RuntimePackagePublishServiceTests.InMemoryOutboxRepository();
        var service = new RuntimePackageActivationService(
            packageRepository,
            stateRepository,
            cacheVersionRepository,
            outboxRepository,
            new RuntimePackagePublishServiceTests.NoopUnitOfWork(),
            new FixedClock(new DateTime(2026, 6, 7, 11, 0, 0)));

        var activated = await service.ActivateAsync(2, "ops");

        Assert.Equal(RuntimePackageStatusCodes.Active, activated.PackageStatus);
        Assert.Equal(RuntimePackageStatusCodes.Superseded, packageRepository.Packages[1].PackageStatus);
        Assert.Equal(2, stateRepository.State.ActivePackageId);
        Assert.Equal(2L, cacheVersionRepository.Items.Single(item => item.CacheScope == CacheVersionSynchronizer.EffectiveRulesScope).VersionNo);
        Assert.Equal(2, outboxRepository.Items.Count);
        Assert.All(outboxRepository.Items, item => Assert.Equal(2, item.RuleId));
    }
}

public sealed class RuntimePackageRollbackServiceTests
{
    [Fact]
    public async Task RollbackAsync_ReactivatesHistoricalPackage()
    {
        var packageRepository = new RuntimePackagePublishServiceTests.InMemoryRuntimePackageRepository();
        packageRepository.Packages[10] = new RuntimePackage { PackageId = 10, PackageVersion = 10, PackageStatus = RuntimePackageStatusCodes.Active };
        packageRepository.Packages[9] = new RuntimePackage { PackageId = 9, PackageVersion = 9, PackageStatus = RuntimePackageStatusCodes.Superseded };
        var stateRepository = new RuntimePackagePublishServiceTests.InMemoryRuntimePackageStateRepository(new RuntimePackageState
        {
            StateCode = RuntimePackageStateCodes.Active,
            ActivePackageId = 10,
            ActivePackageVersion = 10
        });
        var activationService = new RuntimePackageActivationService(
            packageRepository,
            stateRepository,
            new RuntimePackagePublishServiceTests.InMemoryCacheVersionRepository(),
            new RuntimePackagePublishServiceTests.InMemoryOutboxRepository(),
            new RuntimePackagePublishServiceTests.NoopUnitOfWork(),
            new FixedClock(new DateTime(2026, 6, 7, 11, 30, 0)));
        var rollbackService = new RuntimePackageRollbackService(activationService, packageRepository);

        var package = await rollbackService.RollbackAsync(9, "ops");

        Assert.Equal(9, package.PackageId);
        Assert.Equal(RuntimePackageStatusCodes.Active, packageRepository.Packages[9].PackageStatus);
        Assert.Equal(RuntimePackageStatusCodes.Superseded, packageRepository.Packages[10].PackageStatus);
        Assert.Equal(9, stateRepository.State.ActivePackageId);
    }
}

public sealed class PolicyPackageDiffServiceTests
{
    [Fact]
    public async Task DiffAgainstActiveAsync_ReturnsAddedRemovedAndUnchangedPolicies()
    {
        var packageRepository = new RuntimePackagePublishServiceTests.InMemoryRuntimePackageRepository();
        packageRepository.Packages[100] = new RuntimePackage { PackageId = 100, PackageVersion = 100, PackageStatus = RuntimePackageStatusCodes.Active };
        packageRepository.Packages[101] = new RuntimePackage { PackageId = 101, PackageVersion = 101, PackageStatus = RuntimePackageStatusCodes.Built };
        packageRepository.PackagePolicies[100] = new[]
        {
            new RuntimePackagePolicy { PackagePolicyId = 1, PackageId = 100, PolicyVersionId = 1 },
            new RuntimePackagePolicy { PackagePolicyId = 2, PackageId = 100, PolicyVersionId = 2 }
        };
        packageRepository.PackagePolicies[101] = new[]
        {
            new RuntimePackagePolicy { PackagePolicyId = 3, PackageId = 101, PolicyVersionId = 2 },
            new RuntimePackagePolicy { PackagePolicyId = 4, PackageId = 101, PolicyVersionId = 3 }
        };
        var service = new PolicyPackageDiffService(
            new RuntimePackagePublishServiceTests.InMemoryRuntimePackageStateRepository(new RuntimePackageState
            {
                StateCode = RuntimePackageStateCodes.Active,
                ActivePackageId = 100,
                ActivePackageVersion = 100
            }),
            packageRepository);

        var diff = await service.DiffAgainstActiveAsync(101);

        Assert.Equal(new long[] { 3 }, diff.AddedPolicyVersionIds);
        Assert.Equal(new long[] { 1 }, diff.RemovedPolicyVersionIds);
        Assert.Equal(new long[] { 2 }, diff.UnchangedPolicyVersionIds);
    }
}

public sealed class RuntimePackagePublishServiceTests
{
    [Fact]
    public async Task PublishAsync_DirectProfileBuildsAndActivatesPackage()
    {
        var fixture = PublishFixture.CreateDirect();
        var result = await fixture.Service.PublishAsync(new[] { 101L }, "publisher", new DateTime(2026, 6, 7, 12, 0, 0, DateTimeKind.Utc));

        Assert.Equal(RuntimePackageStatusCodes.Active, result.Package.PackageStatus);
        Assert.Equal(result.Package.PackageId, fixture.StateRepository.State.ActivePackageId);
        Assert.Equal(result.Package.PackageId, fixture.PolicyRepository.Versions[101].LastBuiltPackageId);
    }

    [Fact]
    public async Task PublishAsync_ReviewRequiredProfileRejectsWithoutApprovedReview()
    {
        var fixture = PublishFixture.CreateReviewRequired(withApprovedReview: false);

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            fixture.Service.PublishAsync(new[] { 201L }, "publisher", new DateTime(2026, 6, 7, 12, 10, 0, DateTimeKind.Utc)));

        Assert.Equal(BizErrorCode.PolicyReviewRequired, ex.Code);
    }

    [Fact]
    public async Task PublishAsync_ReviewRequiredProfilePublishesAfterApprovedReview()
    {
        var fixture = PublishFixture.CreateReviewRequired(withApprovedReview: true);
        var result = await fixture.Service.PublishAsync(new[] { 201L }, "publisher", new DateTime(2026, 6, 7, 12, 20, 0, DateTimeKind.Utc));

        Assert.Equal(result.Package.PackageId, fixture.StateRepository.State.ActivePackageId);
        Assert.Equal(result.Package.PackageId, fixture.PolicyRepository.Versions[201].LastBuiltPackageId);
    }

    private sealed class PublishFixture
    {
        public RuntimePackagePublishService Service { get; init; } = null!;
        public InMemoryPolicyRepository PolicyRepository { get; init; } = null!;
        public InMemoryRuntimePackageStateRepository StateRepository { get; init; } = null!;

        public static PublishFixture CreateDirect()
        {
            var policyRepository = new InMemoryPolicyRepository();
            var reviewRepository = new InMemoryPolicyReviewRepository();
            SeedPolicy(policyRepository, reviewRepository, direct: true, approvedReview: false);
            return Create(policyRepository, reviewRepository);
        }

        public static PublishFixture CreateReviewRequired(bool withApprovedReview)
        {
            var policyRepository = new InMemoryPolicyRepository();
            var reviewRepository = new InMemoryPolicyReviewRepository();
            SeedPolicy(policyRepository, reviewRepository, direct: false, approvedReview: withApprovedReview);
            return Create(policyRepository, reviewRepository);
        }

        private static PublishFixture Create(InMemoryPolicyRepository policyRepository, InMemoryPolicyReviewRepository reviewRepository)
        {
            var templateRepository = new InMemoryTemplateRepository();
            SeedTemplate(templateRepository);
            var packageRepository = new InMemoryRuntimePackageRepository();
            var buildRepository = new InMemoryRuntimeRuleBuildRepository();
            var stateRepository = new InMemoryRuntimePackageStateRepository(new RuntimePackageState
            {
                StateCode = RuntimePackageStateCodes.Active,
                ActivePackageId = 0,
                ActivePackageVersion = 0
            });
            var validationService = new PolicyValidationService(
                new FormulaExpressionValidator(
                    new FormulaExpressionEvaluator(
                        new FormulaFunctionRegistry())));
            var compiler = new RuntimePackageCompiler(
                policyRepository,
                templateRepository,
                packageRepository,
                buildRepository,
                validationService,
                new PolicyConflictService(),
                new RuntimeRuleProjectionFactory(new PolicyPriorityKeyFactory()),
                new FixedClock(new DateTime(2026, 6, 7, 12, 0, 0)));
            var activationService = new RuntimePackageActivationService(
                packageRepository,
                stateRepository,
                new InMemoryCacheVersionRepository(),
                new InMemoryOutboxRepository(),
                new NoopUnitOfWork(),
                new FixedClock(new DateTime(2026, 6, 7, 12, 0, 0)));
            var publishService = new RuntimePackagePublishService(
                policyRepository,
                new PolicyPublishEligibilityService(new PolicyPublishProfileResolver(), reviewRepository),
                compiler,
                activationService);

            return new PublishFixture
            {
                Service = publishService,
                PolicyRepository = policyRepository,
                StateRepository = stateRepository
            };
        }

        private static void SeedPolicy(InMemoryPolicyRepository policyRepository, InMemoryPolicyReviewRepository reviewRepository, bool direct, bool approvedReview)
        {
            var policyId = direct ? 1L : 2L;
            var versionId = direct ? 101L : 201L;
            var policyCode = direct ? "POL-DIRECT" : "POL-REVIEW";
            var checksum = direct ? "CHK-DIRECT" : "CHK-REVIEW";

            policyRepository.Policies[policyId] = new PolicyAggregate
            {
                PolicyId = policyId,
                PolicyCode = policyCode,
                PolicyName = policyCode,
                PublishProfile = direct ? PolicyPublishProfileCodes.Direct : PolicyPublishProfileCodes.ReviewRequired
            };
            policyRepository.Versions[versionId] = new PolicyVersion
            {
                PolicyVersionId = versionId,
                PolicyId = policyId,
                TemplateVersionId = 1001,
                VersionNo = 1,
                PolicyStatus = direct ? PolicyLifecycleCodes.Validated : PolicyLifecycleCodes.Approved,
                BindingType = "ITEM",
                ScopeLevel = "SCENE",
                PriorityWeight = 10,
                EffectiveFrom = new DateTime(2026, 6, 1),
                EffectiveTo = new DateTime(2026, 12, 31),
                Checksum = checksum
            };
            policyRepository.Bindings[versionId] = new[]
            {
                new PolicyBinding { PolicyBindingId = versionId, PolicyVersionId = versionId, BindingType = "ITEM", ItemCode = direct ? "ITEM001" : "ITEM002" }
            };
            policyRepository.Scopes[versionId] = new[]
            {
                new PolicyScope { PolicyScopeId = versionId, PolicyVersionId = versionId, ScopeDimension = "SCENE", ScopeOperator = "EQ", ScopeValueText = "OUTPATIENT" }
            };
            policyRepository.Parameters[versionId] = new[]
            {
                new PolicyParam { PolicyParamId = versionId, PolicyVersionId = versionId, ParamCode = "RATE", ValueType = "NUMBER", ValueNumber = 0.8m }
            };

            if (approvedReview)
            {
                reviewRepository.Items.Add(new PolicyReview
                {
                    ReviewId = 1,
                    PolicyVersionId = versionId,
                    ReviewStatus = PolicyReviewStatusCodes.Approved,
                    ReviewStage = "NORMAL",
                    SourceChecksum = checksum,
                    ReviewedAt = new DateTime(2026, 6, 7, 11, 0, 0)
                });
            }
        }

        private static void SeedTemplate(InMemoryTemplateRepository templateRepository)
        {
            templateRepository.TemplateVersions[1001] = new TemplateVersion
            {
                TemplateVersionId = 1001,
                TemplateId = 5001,
                VersionNo = 1,
                CapabilityFamily = TemplateCapabilityCodes.FormulaPricing,
                MergeMode = RuntimeMergeModeCodes.SingleWinner
            };
            templateRepository.ParamDefs[1001] = new[]
            {
                new TemplateParamDef { ParamDefId = 1, TemplateVersionId = 1001, ParamCode = "RATE", ValueType = "NUMBER", IsRequired = EnableFlag.Yes, MinValue = 0m, MaxValue = 1m }
            };
            templateRepository.StepDefs[1001] = new[]
            {
                new TemplateStepDef
                {
                    StepDefId = 1,
                    TemplateVersionId = 1001,
                    StepNo = 10,
                    StepKind = "ACTION",
                    CapabilityCode = TemplateCapabilityCodes.FormulaPricing,
                    ActionType = RuleActionTypeCodes.FormulaCalc,
                    ExecutorCode = FormulaExecutorCodes.IncrementPercent,
                    OnError = ActionOnErrorCodes.Stop
                }
            };
            templateRepository.ScopeDefs[1001] = new[]
            {
                new TemplateScopeDef { ScopeDefId = 1, TemplateVersionId = 1001, ScopeDimension = "SCENE", IsRequired = EnableFlag.Yes }
            };
        }
    }

    public sealed class InMemoryPolicyRepository : IPolicyRepository
    {
        public Dictionary<long, PolicyAggregate> Policies { get; } = new();
        public Dictionary<long, PolicyVersion> Versions { get; } = new();
        public Dictionary<long, IReadOnlyList<PolicyBinding>> Bindings { get; } = new();
        public Dictionary<long, IReadOnlyList<PolicyScope>> Scopes { get; } = new();
        public Dictionary<long, IReadOnlyList<PolicyParam>> Parameters { get; } = new();

        public Task<IReadOnlyList<PolicyAggregate>> GetAllAsync() =>
            Task.FromResult((IReadOnlyList<PolicyAggregate>)Policies.Values.OrderBy(item => item.PolicyCode).ToList());

        public Task<PolicyAggregate?> GetByIdAsync(long policyId) =>
            Task.FromResult(Policies.TryGetValue(policyId, out var policy) ? policy : null);

        public Task<PolicyAggregate?> GetByCodeAsync(string policyCode) =>
            Task.FromResult(Policies.Values.FirstOrDefault(policy => policy.PolicyCode == policyCode));

        public Task<IReadOnlyList<PolicyVersion>> GetVersionsByPolicyIdAsync(long policyId) =>
            Task.FromResult((IReadOnlyList<PolicyVersion>)Versions.Values
                .Where(version => version.PolicyId == policyId)
                .OrderByDescending(version => version.VersionNo)
                .ToList());

        public Task<PolicyVersion?> GetVersionAsync(long policyVersionId) =>
            Task.FromResult(Versions.TryGetValue(policyVersionId, out var version) ? version : null);

        public Task<IReadOnlyList<PolicyBinding>> GetBindingsAsync(long policyVersionId) =>
            Task.FromResult(Bindings.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyBinding>)Array.Empty<PolicyBinding>());

        public Task<IReadOnlyList<PolicyScope>> GetScopesAsync(long policyVersionId) =>
            Task.FromResult(Scopes.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyScope>)Array.Empty<PolicyScope>());

        public Task<IReadOnlyList<PolicyParam>> GetParamsAsync(long policyVersionId) =>
            Task.FromResult(Parameters.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyParam>)Array.Empty<PolicyParam>());

        public Task<IReadOnlyList<PolicyVersion>> GetPublishReadyVersionsAsync() =>
            Task.FromResult((IReadOnlyList<PolicyVersion>)Versions.Values
                .Where(version => string.Equals(version.PolicyStatus, PolicyLifecycleCodes.PublishReady, StringComparison.OrdinalIgnoreCase))
                .ToList());

        public Task<long> InsertAsync(PolicyAggregate entity)
        {
            Policies[entity.PolicyId] = entity;
            return Task.FromResult(entity.PolicyId);
        }

        public Task UpdateAsync(PolicyAggregate entity)
        {
            Policies[entity.PolicyId] = entity;
            return Task.CompletedTask;
        }

        public Task UpdateVersionAsync(PolicyVersion entity)
        {
            Versions[entity.PolicyVersionId] = entity;
            return Task.CompletedTask;
        }

        public Task<long> InsertVersionAsync(PolicyVersion entity)
        {
            Versions[entity.PolicyVersionId] = entity;
            return Task.FromResult(entity.PolicyVersionId);
        }

        public Task ReplaceBindingsAsync(long policyVersionId, IReadOnlyList<PolicyBinding> entities)
        {
            Bindings[policyVersionId] = entities;
            return Task.CompletedTask;
        }

        public Task ReplaceScopesAsync(long policyVersionId, IReadOnlyList<PolicyScope> entities)
        {
            Scopes[policyVersionId] = entities;
            return Task.CompletedTask;
        }

        public Task ReplaceParamsAsync(long policyVersionId, IReadOnlyList<PolicyParam> entities)
        {
            Parameters[policyVersionId] = entities;
            return Task.CompletedTask;
        }
    }

    public sealed class InMemoryPolicyReviewRepository : IPolicyReviewRepository
    {
        public List<PolicyReview> Items { get; } = new();

        public Task<PolicyReview?> GetLatestByPolicyVersionIdAsync(long policyVersionId) =>
            Task.FromResult(Items
                .Where(item => item.PolicyVersionId == policyVersionId)
                .OrderByDescending(item => item.ReviewedAt ?? item.SubmittedAt ?? DateTime.MinValue)
                .ThenByDescending(item => item.ReviewId)
                .FirstOrDefault());

        public Task<IReadOnlyList<PolicyReview>> GetByPolicyVersionIdAsync(long policyVersionId) =>
            Task.FromResult((IReadOnlyList<PolicyReview>)Items
                .Where(item => item.PolicyVersionId == policyVersionId)
                .ToList());

        public Task<long> InsertAsync(PolicyReview entity)
        {
            entity.ReviewId = Items.Count + 1;
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

    public sealed class InMemoryTemplateRepository : ITemplateRepository
    {
        public Dictionary<long, TemplateVersion> TemplateVersions { get; } = new();
        public Dictionary<long, IReadOnlyList<TemplateParamDef>> ParamDefs { get; } = new();
        public Dictionary<long, IReadOnlyList<TemplateStepDef>> StepDefs { get; } = new();
        public Dictionary<long, IReadOnlyList<TemplateScopeDef>> ScopeDefs { get; } = new();

        public Task<IReadOnlyList<TemplateAggregate>> GetAllAsync() =>
            Task.FromResult((IReadOnlyList<TemplateAggregate>)Array.Empty<TemplateAggregate>());

        public Task<TemplateAggregate?> GetByIdAsync(long templateId) => Task.FromResult<TemplateAggregate?>(null);
        public Task<TemplateAggregate?> GetByCodeAsync(string templateCode) => Task.FromResult<TemplateAggregate?>(null);
        public Task<IReadOnlyList<TemplateVersion>> GetVersionsByTemplateIdAsync(long templateId) =>
            Task.FromResult((IReadOnlyList<TemplateVersion>)TemplateVersions.Values
                .Where(version => version.TemplateId == templateId)
                .OrderByDescending(version => version.VersionNo)
                .ToList());
        public Task<TemplateVersion?> GetVersionAsync(long templateVersionId) =>
            Task.FromResult(TemplateVersions.TryGetValue(templateVersionId, out var item) ? item : null);
        public Task<IReadOnlyList<TemplateParamDef>> GetParamDefsAsync(long templateVersionId) =>
            Task.FromResult(ParamDefs.TryGetValue(templateVersionId, out var items) ? items : (IReadOnlyList<TemplateParamDef>)Array.Empty<TemplateParamDef>());
        public Task<IReadOnlyList<TemplateStepDef>> GetStepDefsAsync(long templateVersionId) =>
            Task.FromResult(StepDefs.TryGetValue(templateVersionId, out var items) ? items : (IReadOnlyList<TemplateStepDef>)Array.Empty<TemplateStepDef>());
        public Task<IReadOnlyList<TemplateScopeDef>> GetScopeDefsAsync(long templateVersionId) =>
            Task.FromResult(ScopeDefs.TryGetValue(templateVersionId, out var items) ? items : (IReadOnlyList<TemplateScopeDef>)Array.Empty<TemplateScopeDef>());
        public Task<long> InsertAsync(TemplateAggregate entity) => Task.FromResult(0L);
        public Task UpdateAsync(TemplateAggregate entity) => Task.CompletedTask;
        public Task<long> InsertVersionAsync(TemplateVersion entity)
        {
            TemplateVersions[entity.TemplateVersionId] = entity;
            return Task.FromResult(entity.TemplateVersionId);
        }
        public Task UpdateVersionAsync(TemplateVersion entity)
        {
            TemplateVersions[entity.TemplateVersionId] = entity;
            return Task.CompletedTask;
        }
        public Task ReplaceParamDefsAsync(long templateVersionId, IReadOnlyList<TemplateParamDef> entities)
        {
            ParamDefs[templateVersionId] = entities;
            return Task.CompletedTask;
        }
        public Task ReplaceStepDefsAsync(long templateVersionId, IReadOnlyList<TemplateStepDef> entities)
        {
            StepDefs[templateVersionId] = entities;
            return Task.CompletedTask;
        }
        public Task ReplaceScopeDefsAsync(long templateVersionId, IReadOnlyList<TemplateScopeDef> entities)
        {
            ScopeDefs[templateVersionId] = entities;
            return Task.CompletedTask;
        }
    }

    public sealed class InMemoryRuntimePackageRepository : IRuntimePackageRepository
    {
        public Dictionary<long, RuntimePackage> Packages { get; } = new();
        public Dictionary<long, IReadOnlyList<RuntimePackagePolicy>> PackagePolicies { get; } = new();
        private long _nextId = 1000;

        public Task<RuntimePackage?> GetByIdAsync(long packageId) =>
            Task.FromResult(Packages.TryGetValue(packageId, out var item) ? item : null);

        public Task<IReadOnlyList<RuntimePackage>> GetHistoryAsync(int take) =>
            Task.FromResult((IReadOnlyList<RuntimePackage>)Packages.Values
                .OrderByDescending(item => item.PackageVersion)
                .Take(take)
                .ToList());

        public Task<IReadOnlyList<RuntimePackagePolicy>> GetPackagePoliciesAsync(long packageId) =>
            Task.FromResult(PackagePolicies.TryGetValue(packageId, out var items) ? items : (IReadOnlyList<RuntimePackagePolicy>)Array.Empty<RuntimePackagePolicy>());

        public Task<long> InsertAsync(RuntimePackage entity)
        {
            entity.PackageId = _nextId++;
            Packages[entity.PackageId] = entity;
            return Task.FromResult(entity.PackageId);
        }

        public Task UpdateAsync(RuntimePackage entity)
        {
            Packages[entity.PackageId] = entity;
            return Task.CompletedTask;
        }
    }

    public sealed class InMemoryRuntimePackageStateRepository : IRuntimePackageStateRepository
    {
        public RuntimePackageState State { get; private set; }

        public InMemoryRuntimePackageStateRepository(RuntimePackageState state)
        {
            State = state;
        }

        public Task<RuntimePackageState?> GetActiveAsync() => Task.FromResult<RuntimePackageState?>(State);

        public Task<RuntimePackageState?> GetActiveForUpdateAsync() => Task.FromResult<RuntimePackageState?>(State);

        public Task UpsertAsync(RuntimePackageState entity)
        {
            State = entity;
            return Task.CompletedTask;
        }
    }

    public sealed class InMemoryCacheVersionRepository : ICacheVersionRepository
    {
        public List<CacheVersion> Items { get; } = new()
        {
            new CacheVersion { CacheScope = CacheVersionSynchronizer.EffectiveRulesScope, VersionNo = 1, UpdatedAt = DateTime.Now },
            new CacheVersion { CacheScope = CacheVersionSynchronizer.ActionTypeOrderScope, VersionNo = 1, UpdatedAt = DateTime.Now }
        };

        public Task<CacheVersion?> GetByScopeAsync(string cacheScope) =>
            Task.FromResult(Items.SingleOrDefault(item => string.Equals(item.CacheScope, cacheScope, StringComparison.OrdinalIgnoreCase)));

        public Task<long> IncreaseVersionAsync(string cacheScope)
        {
            var item = Items.Single(existing => string.Equals(existing.CacheScope, cacheScope, StringComparison.OrdinalIgnoreCase));
            item.VersionNo++;
            return Task.FromResult(item.VersionNo);
        }
    }

    public sealed class InMemoryOutboxRepository : IRuleCacheInvalidationOutboxRepository
    {
        public List<RuleCacheInvalidationOutbox> Items { get; } = new();
        private long _nextId = 1;

        public Task<long> InsertAsync(RuleCacheInvalidationOutbox entity)
        {
            entity.OutboxId = _nextId++;
            Items.Add(entity);
            return Task.FromResult(entity.OutboxId);
        }

        public Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetPendingAsync(DateTime now, int maxCount) =>
            Task.FromResult((IReadOnlyList<RuleCacheInvalidationOutbox>)Items.ToList());

        public Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetForDashboardAsync(int maxFailedCount) =>
            Task.FromResult((IReadOnlyList<RuleCacheInvalidationOutbox>)Items.ToList());

        public Task<bool> MarkProcessedAsync(long outboxId, DateTime processedAt) => Task.FromResult(true);

        public Task<bool> MarkFailedAsync(long outboxId, string lastError, int retryCount, DateTime nextRetryAt) => Task.FromResult(true);
    }

    public sealed class InMemoryRuntimeRuleBuildRepository : IRuntimeRuleBuildRepository
    {
        private long _nextPackagePolicyId = 2000;
        private long _nextRuleId = 3000;
        private long _nextConditionId = 4000;
        private long _nextActionId = 5000;

        public Task<IReadOnlyList<long>> ReservePackagePolicyIdsAsync(int count) => Task.FromResult(Reserve(ref _nextPackagePolicyId, count));
        public Task<IReadOnlyList<long>> ReserveRuleIdsAsync(int count) => Task.FromResult(Reserve(ref _nextRuleId, count));
        public Task<IReadOnlyList<long>> ReserveConditionIdsAsync(int count) => Task.FromResult(Reserve(ref _nextConditionId, count));
        public Task<IReadOnlyList<long>> ReserveActionIdsAsync(int count) => Task.FromResult(Reserve(ref _nextActionId, count));
        public Task InsertPackagePoliciesAsync(IReadOnlyList<RuntimePackagePolicy> packagePolicies) => Task.CompletedTask;
        public Task InsertRulesAsync(IReadOnlyList<RuntimeRule> rules) => Task.CompletedTask;
        public Task InsertConditionsAsync(IReadOnlyList<RuntimeCondition> conditions) => Task.CompletedTask;
        public Task InsertActionsAsync(IReadOnlyList<RuntimeAction> actions) => Task.CompletedTask;

        private static IReadOnlyList<long> Reserve(ref long seed, int count)
        {
            var result = new List<long>(count);
            for (var i = 0; i < count; i++)
            {
                result.Add(seed++);
            }

            return result;
        }
    }

    public sealed class NoopUnitOfWork : IUnitOfWork
    {
        public Task BeginAsync() => Task.CompletedTask;
        public Task CommitAsync() => Task.CompletedTask;
        public Task RollbackAsync() => Task.CompletedTask;
        public void Dispose() { }
    }
}
