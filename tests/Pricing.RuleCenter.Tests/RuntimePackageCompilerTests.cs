using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Runtime;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Engine.Formula;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Runtime;
using Pricing.RuleCenter.Core.Interfaces.Templates;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuntimePackageCompilerTests
{
    [Fact]
    public async Task CompileAsync_BuildsCandidatePackageFromPublishReadyPolicies()
    {
        var policyRepository = new InMemoryPolicyRepository();
        var templateRepository = new InMemoryTemplateRepository();
        var packageRepository = new InMemoryRuntimePackageRepository();
        var buildRepository = new InMemoryRuntimeRuleBuildRepository();
        SeedSinglePublishReadyPolicy(policyRepository, templateRepository);

        var compiler = CreateCompiler(policyRepository, templateRepository, packageRepository, buildRepository);
        var result = await compiler.CompileAsync(new RuntimePackageBuildContext
        {
            BuiltBy = "tester",
            BuildAt = new DateTime(2026, 6, 7, 12, 0, 0, DateTimeKind.Utc)
        });

        Assert.Equal(RuntimePackageStatusCodes.Built, result.Package.PackageStatus);
        Assert.Equal(result.Package.PackageId, Assert.Single(result.Rules).PackageId);
        Assert.Equal("ITEM001", Assert.Single(result.Rules).TargetItemCode);
        Assert.Equal(RuleConditionTypeCodes.ChargeScene, Assert.Single(result.Conditions).ConditionType);
        Assert.Equal(RuleActionTypeCodes.FormulaCalc, Assert.Single(result.Actions).ActionType);
        Assert.Single(result.PackagePolicies);
        Assert.Single(buildRepository.InsertedRules);
        Assert.Single(buildRepository.InsertedConditions);
        Assert.Single(buildRepository.InsertedActions);
    }

    [Fact]
    public async Task CompileAsync_RejectsWhenSingleWinnerPoliciesConflict()
    {
        var policyRepository = new InMemoryPolicyRepository();
        var templateRepository = new InMemoryTemplateRepository();
        var packageRepository = new InMemoryRuntimePackageRepository();
        var buildRepository = new InMemoryRuntimeRuleBuildRepository();
        SeedConflictingPolicies(policyRepository, templateRepository);

        var compiler = CreateCompiler(policyRepository, templateRepository, packageRepository, buildRepository);

        var ex = await Assert.ThrowsAsync<BizException>(() => compiler.CompileAsync(new RuntimePackageBuildContext
        {
            BuiltBy = "tester",
            BuildAt = new DateTime(2026, 6, 7, 12, 0, 0, DateTimeKind.Utc)
        }));

        Assert.Equal(BizErrorCode.RuntimePackageBuildConflict, ex.Code);
    }

    private static RuntimePackageCompiler CreateCompiler(
        InMemoryPolicyRepository policyRepository,
        InMemoryTemplateRepository templateRepository,
        InMemoryRuntimePackageRepository packageRepository,
        InMemoryRuntimeRuleBuildRepository buildRepository)
    {
        var validationService = new PolicyValidationService(
            new FormulaExpressionValidator(
                new FormulaExpressionEvaluator(
                    new FormulaFunctionRegistry())));
        var priorityFactory = new PolicyPriorityKeyFactory();
        var conflictService = new PolicyConflictService();
        var projectionFactory = new RuntimeRuleProjectionFactory(priorityFactory);

        return new RuntimePackageCompiler(
            policyRepository,
            templateRepository,
            packageRepository,
            buildRepository,
            validationService,
            conflictService,
            projectionFactory,
            new FixedClock(new DateTime(2026, 6, 7, 12, 0, 0)));
    }

    private static void SeedSinglePublishReadyPolicy(
        InMemoryPolicyRepository policyRepository,
        InMemoryTemplateRepository templateRepository)
    {
        policyRepository.Policies.Add(new PolicyAggregate
        {
            PolicyId = 1,
            PolicyCode = "POL001",
            PolicyName = "门诊比例折价"
        });
        policyRepository.PublishReadyVersions.Add(new PolicyVersion
        {
            PolicyVersionId = 101,
            PolicyId = 1,
            TemplateVersionId = 1001,
            VersionNo = 1,
            PolicyStatus = PolicyLifecycleCodes.PublishReady,
            BindingType = "ITEM",
            ScopeLevel = "SCENE",
            PriorityWeight = 10,
            EffectiveFrom = new DateTime(2026, 6, 1),
            EffectiveTo = new DateTime(2026, 12, 31),
            Checksum = "POL001-V1"
        });
        policyRepository.Bindings[101] = new[]
        {
            new PolicyBinding { PolicyBindingId = 1, PolicyVersionId = 101, BindingType = "ITEM", ItemCode = "ITEM001" }
        };
        policyRepository.Scopes[101] = new[]
        {
            new PolicyScope { PolicyScopeId = 1, PolicyVersionId = 101, ScopeDimension = "SCENE", ScopeOperator = "EQ", ScopeValueText = "OUTPATIENT" }
        };
        policyRepository.Parameters[101] = new[]
        {
            new PolicyParam { PolicyParamId = 1, PolicyVersionId = 101, ParamCode = "RATE", ValueType = "NUMBER", ValueNumber = 0.8m }
        };

        templateRepository.TemplateVersions[1001] = new TemplateVersion
        {
            TemplateVersionId = 1001,
            TemplateId = 5001,
            VersionNo = 1,
            CapabilityFamily = TemplateCapabilityCodes.FormulaPricing,
            MergeMode = RuntimeMergeModeCodes.SingleWinner,
            Checksum = "TPL1001-V1"
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

    private static void SeedConflictingPolicies(
        InMemoryPolicyRepository policyRepository,
        InMemoryTemplateRepository templateRepository)
    {
        SeedSinglePublishReadyPolicy(policyRepository, templateRepository);

        policyRepository.Policies.Add(new PolicyAggregate
        {
            PolicyId = 2,
            PolicyCode = "POL002",
            PolicyName = "门诊比例折价2"
        });
        policyRepository.PublishReadyVersions.Add(new PolicyVersion
        {
            PolicyVersionId = 102,
            PolicyId = 2,
            TemplateVersionId = 1001,
            VersionNo = 1,
            PolicyStatus = PolicyLifecycleCodes.PublishReady,
            BindingType = "ITEM",
            ScopeLevel = "SCENE",
            PriorityWeight = 10,
            EffectiveFrom = new DateTime(2026, 6, 1),
            EffectiveTo = new DateTime(2026, 12, 31),
            Checksum = "POL002-V1"
        });
        policyRepository.Bindings[102] = new[]
        {
            new PolicyBinding { PolicyBindingId = 2, PolicyVersionId = 102, BindingType = "ITEM", ItemCode = "ITEM001" }
        };
        policyRepository.Scopes[102] = new[]
        {
            new PolicyScope { PolicyScopeId = 2, PolicyVersionId = 102, ScopeDimension = "SCENE", ScopeOperator = "EQ", ScopeValueText = "OUTPATIENT" }
        };
        policyRepository.Parameters[102] = new[]
        {
            new PolicyParam { PolicyParamId = 2, PolicyVersionId = 102, ParamCode = "RATE", ValueType = "NUMBER", ValueNumber = 0.7m }
        };
    }

    private sealed class InMemoryPolicyRepository : IPolicyRepository
    {
        public List<PolicyAggregate> Policies { get; } = new();

        public List<PolicyVersion> PublishReadyVersions { get; } = new();

        public Dictionary<long, IReadOnlyList<PolicyBinding>> Bindings { get; } = new();

        public Dictionary<long, IReadOnlyList<PolicyScope>> Scopes { get; } = new();

        public Dictionary<long, IReadOnlyList<PolicyParam>> Parameters { get; } = new();

        public Task<IReadOnlyList<PolicyAggregate>> GetAllAsync() =>
            Task.FromResult((IReadOnlyList<PolicyAggregate>)Policies.ToList());

        public Task<PolicyAggregate?> GetByIdAsync(long policyId) =>
            Task.FromResult(Policies.FirstOrDefault(policy => policy.PolicyId == policyId));

        public Task<PolicyAggregate?> GetByCodeAsync(string policyCode) =>
            Task.FromResult(Policies.FirstOrDefault(policy => policy.PolicyCode == policyCode));

        public Task<IReadOnlyList<PolicyVersion>> GetVersionsByPolicyIdAsync(long policyId) =>
            Task.FromResult((IReadOnlyList<PolicyVersion>)PublishReadyVersions
                .Where(version => version.PolicyId == policyId)
                .OrderByDescending(version => version.VersionNo)
                .ToList());

        public Task<PolicyVersion?> GetVersionAsync(long policyVersionId) =>
            Task.FromResult(PublishReadyVersions.FirstOrDefault(version => version.PolicyVersionId == policyVersionId));

        public Task<IReadOnlyList<PolicyBinding>> GetBindingsAsync(long policyVersionId) =>
            Task.FromResult(Bindings.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyBinding>)Array.Empty<PolicyBinding>());

        public Task<IReadOnlyList<PolicyScope>> GetScopesAsync(long policyVersionId) =>
            Task.FromResult(Scopes.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyScope>)Array.Empty<PolicyScope>());

        public Task<IReadOnlyList<PolicyParam>> GetParamsAsync(long policyVersionId) =>
            Task.FromResult(Parameters.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyParam>)Array.Empty<PolicyParam>());

        public Task<IReadOnlyList<PolicyVersion>> GetPublishReadyVersionsAsync() =>
            Task.FromResult((IReadOnlyList<PolicyVersion>)PublishReadyVersions.ToList());

        public Task<long> InsertAsync(PolicyAggregate entity) => Task.FromResult(0L);

        public Task UpdateAsync(PolicyAggregate entity) => Task.CompletedTask;

        public Task<long> InsertVersionAsync(PolicyVersion entity)
        {
            PublishReadyVersions.Add(entity);
            return Task.FromResult(entity.PolicyVersionId);
        }

        public Task UpdateVersionAsync(PolicyVersion entity)
        {
            var existing = PublishReadyVersions.FirstOrDefault(item => item.PolicyVersionId == entity.PolicyVersionId);
            if (existing is not null)
            {
                PublishReadyVersions.Remove(existing);
            }

            PublishReadyVersions.Add(entity);
            return Task.CompletedTask;
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

    private sealed class InMemoryTemplateRepository : ITemplateRepository
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

    private sealed class InMemoryRuntimePackageRepository : IRuntimePackageRepository
    {
        public List<RuntimePackage> Items { get; } = new();

        private long _nextId = 1000;

        public Task<RuntimePackage?> GetByIdAsync(long packageId) =>
            Task.FromResult(Items.FirstOrDefault(item => item.PackageId == packageId));

        public Task<IReadOnlyList<RuntimePackage>> GetHistoryAsync(int take) =>
            Task.FromResult((IReadOnlyList<RuntimePackage>)Items
                .OrderByDescending(item => item.PackageVersion)
                .Take(take)
                .ToList());

        public Task<IReadOnlyList<RuntimePackagePolicy>> GetPackagePoliciesAsync(long packageId) =>
            Task.FromResult((IReadOnlyList<RuntimePackagePolicy>)Array.Empty<RuntimePackagePolicy>());

        public Task<long> InsertAsync(RuntimePackage entity)
        {
            entity.PackageId = _nextId++;
            Items.Add(entity);
            return Task.FromResult(entity.PackageId);
        }

        public Task UpdateAsync(RuntimePackage entity)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryRuntimeRuleBuildRepository : IRuntimeRuleBuildRepository
    {
        public List<RuntimePackagePolicy> InsertedPackagePolicies { get; } = new();
        public List<RuntimeRule> InsertedRules { get; } = new();
        public List<RuntimeCondition> InsertedConditions { get; } = new();
        public List<RuntimeAction> InsertedActions { get; } = new();

        private long _nextPackagePolicyId = 2000;
        private long _nextRuleId = 3000;
        private long _nextConditionId = 4000;
        private long _nextActionId = 5000;

        public Task<IReadOnlyList<long>> ReservePackagePolicyIdsAsync(int count) =>
            Task.FromResult(Reserve(ref _nextPackagePolicyId, count));

        public Task<IReadOnlyList<long>> ReserveRuleIdsAsync(int count) =>
            Task.FromResult(Reserve(ref _nextRuleId, count));

        public Task<IReadOnlyList<long>> ReserveConditionIdsAsync(int count) =>
            Task.FromResult(Reserve(ref _nextConditionId, count));

        public Task<IReadOnlyList<long>> ReserveActionIdsAsync(int count) =>
            Task.FromResult(Reserve(ref _nextActionId, count));

        public Task InsertPackagePoliciesAsync(IReadOnlyList<RuntimePackagePolicy> packagePolicies)
        {
            InsertedPackagePolicies.AddRange(packagePolicies);
            return Task.CompletedTask;
        }

        public Task InsertRulesAsync(IReadOnlyList<RuntimeRule> rules)
        {
            InsertedRules.AddRange(rules);
            return Task.CompletedTask;
        }

        public Task InsertConditionsAsync(IReadOnlyList<RuntimeCondition> conditions)
        {
            InsertedConditions.AddRange(conditions);
            return Task.CompletedTask;
        }

        public Task InsertActionsAsync(IReadOnlyList<RuntimeAction> actions)
        {
            InsertedActions.AddRange(actions);
            return Task.CompletedTask;
        }

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
}
