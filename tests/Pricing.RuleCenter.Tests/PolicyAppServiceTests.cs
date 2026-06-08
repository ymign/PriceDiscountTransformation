using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Engine.Formula;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Templates;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PolicyAppServiceTests
{
    [Fact]
    public async Task CreateSaveDraftAndValidate_ShouldPersistPolicyVersion()
    {
        var policyRepository = new InMemoryPolicyRepository();
        var templateRepository = new InMemoryTemplateRepository();
        templateRepository.TemplateVersions[1001] = new TemplateVersion
        {
            TemplateVersionId = 1001,
            TemplateId = 1,
            VersionNo = 1,
            CapabilityFamily = "FORMULA_PRICING",
            MergeMode = "SINGLE_WINNER"
        };
        templateRepository.ParamDefs[1001] = new[]
        {
            new TemplateParamDef { ParamCode = "RATE", ParamName = "比例", ValueType = "NUMBER", IsRequired = "Y", MinValue = 0m, MaxValue = 1m }
        };
        templateRepository.StepDefs[1001] = new[]
        {
            new TemplateStepDef { StepNo = 10, StepKind = "ACTION", CapabilityCode = "FORMULA_PRICING", ActionType = "FORMULA_CALC", ExecutorCode = "INCREMENT_PERCENT", OnError = "STOP" }
        };
        templateRepository.ScopeDefs[1001] = new[]
        {
            new TemplateScopeDef { ScopeDimension = "SCENE", IsRequired = "Y" }
        };

        var clock = new FixedClock(new DateTime(2026, 6, 7, 13, 10, 0));
        var policyAppService = new PolicyAppService(policyRepository, clock);
        var versionAppService = new PolicyVersionAppService(
            policyRepository,
            templateRepository,
            new PolicyExpressionGuard(),
            new PolicyValidationService(new FormulaExpressionValidator(new FormulaExpressionEvaluator(new FormulaFunctionRegistry()))),
            new NoopUnitOfWork(),
            clock);

        var policyId = await policyAppService.CreateAsync(new PolicyCreateRequest
        {
            PolicyCode = "POL001",
            PolicyName = "策略1",
            TemplateId = 1,
            OwnerType = "PRICE_DEPT",
            PublishProfile = "DIRECT"
        });
        var policyVersionId = await versionAppService.SaveDraftAsync(policyId, new PolicyVersionSaveRequest
        {
            TemplateVersionId = 1001,
            BindingType = "ITEM",
            ScopeLevel = "SCENE",
            PriorityWeight = 10,
            Bindings = new[] { new PolicyBindingDto { BindingType = "ITEM", ItemCode = "ITEM001" } },
            Scopes = new[] { new PolicyScopeDto { ScopeDimension = "SCENE", ScopeOperator = "EQ", ScopeValueText = "OUTPATIENT" } },
            Params = new[] { new PolicyParamDto { ParamCode = "RATE", ValueType = "NUMBER", ValueNumber = 0.8m } }
        });
        var validation = await versionAppService.ValidateAsync(policyVersionId);
        var detail = await policyAppService.GetByIdAsync(policyId);

        Assert.Equal("VALIDATED", validation.PolicyStatus);
        Assert.NotNull(detail);
        Assert.Equal(1, detail!.CurrentVersionNo);
        Assert.Single(detail.Versions);
    }

    [Fact]
    public async Task SaveDraftAsync_RejectsPolicyVersionFromAnotherPolicy()
    {
        var policyRepository = new InMemoryPolicyRepository();
        var templateRepository = new InMemoryTemplateRepository();
        var clock = new FixedClock(new DateTime(2026, 6, 8, 9, 0, 0));
        var policyAppService = new PolicyAppService(policyRepository, clock);
        var versionAppService = new PolicyVersionAppService(
            policyRepository,
            templateRepository,
            new PolicyExpressionGuard(),
            new PolicyValidationService(new FormulaExpressionValidator(new FormulaExpressionEvaluator(new FormulaFunctionRegistry()))),
            new NoopUnitOfWork(),
            clock);

        var firstPolicyId = await policyAppService.CreateAsync(new PolicyCreateRequest
        {
            PolicyCode = "POL_A",
            PolicyName = "策略A",
            TemplateId = 1,
            OwnerType = "PRICE_DEPT",
            PublishProfile = "DIRECT"
        });
        var secondPolicyId = await policyAppService.CreateAsync(new PolicyCreateRequest
        {
            PolicyCode = "POL_B",
            PolicyName = "策略B",
            TemplateId = 1,
            OwnerType = "PRICE_DEPT",
            PublishProfile = "DIRECT"
        });
        var firstVersionId = await versionAppService.SaveDraftAsync(firstPolicyId, new PolicyVersionSaveRequest
        {
            TemplateVersionId = 1001,
            BindingType = "ITEM",
            ScopeLevel = "SCENE"
        });

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            versionAppService.SaveDraftAsync(secondPolicyId, new PolicyVersionSaveRequest
            {
                PolicyVersionId = firstVersionId,
                TemplateVersionId = 1001,
                BindingType = "ITEM",
                ScopeLevel = "SCENE"
            }));

        Assert.Equal(BizErrorCode.PolicyNotFound, ex.Code);
    }

    private sealed class InMemoryPolicyRepository : IPolicyRepository
    {
        private long _nextPolicyId = 1;
        private long _nextVersionId = 100;
        private readonly Dictionary<long, PolicyAggregate> _policies = new();
        private readonly Dictionary<long, PolicyVersion> _versions = new();
        private readonly Dictionary<long, IReadOnlyList<PolicyBinding>> _bindings = new();
        private readonly Dictionary<long, IReadOnlyList<PolicyScope>> _scopes = new();
        private readonly Dictionary<long, IReadOnlyList<PolicyParam>> _params = new();

        public Task<IReadOnlyList<PolicyAggregate>> GetAllAsync() =>
            Task.FromResult((IReadOnlyList<PolicyAggregate>)_policies.Values.OrderBy(item => item.PolicyCode).ToList());
        public Task<PolicyAggregate?> GetByIdAsync(long policyId) =>
            Task.FromResult(_policies.TryGetValue(policyId, out var item) ? item : null);
        public Task<PolicyAggregate?> GetByCodeAsync(string policyCode) =>
            Task.FromResult(_policies.Values.FirstOrDefault(item => item.PolicyCode == policyCode));
        public Task<IReadOnlyList<PolicyVersion>> GetVersionsByPolicyIdAsync(long policyId) =>
            Task.FromResult((IReadOnlyList<PolicyVersion>)_versions.Values.Where(item => item.PolicyId == policyId).OrderByDescending(item => item.VersionNo).ToList());
        public Task<PolicyVersion?> GetVersionAsync(long policyVersionId) =>
            Task.FromResult(_versions.TryGetValue(policyVersionId, out var item) ? item : null);
        public Task<IReadOnlyList<PolicyBinding>> GetBindingsAsync(long policyVersionId) =>
            Task.FromResult(_bindings.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyBinding>)Array.Empty<PolicyBinding>());
        public Task<IReadOnlyList<PolicyScope>> GetScopesAsync(long policyVersionId) =>
            Task.FromResult(_scopes.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyScope>)Array.Empty<PolicyScope>());
        public Task<IReadOnlyList<PolicyParam>> GetParamsAsync(long policyVersionId) =>
            Task.FromResult(_params.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyParam>)Array.Empty<PolicyParam>());
        public Task<IReadOnlyList<PolicyVersion>> GetPublishReadyVersionsAsync() =>
            Task.FromResult((IReadOnlyList<PolicyVersion>)_versions.Values.Where(item => item.PolicyStatus == "PUBLISH_READY").ToList());
        public Task<long> InsertAsync(PolicyAggregate entity)
        {
            entity.PolicyId = _nextPolicyId++;
            _policies[entity.PolicyId] = entity;
            return Task.FromResult(entity.PolicyId);
        }
        public Task UpdateAsync(PolicyAggregate entity)
        {
            _policies[entity.PolicyId] = entity;
            return Task.CompletedTask;
        }
        public Task<long> InsertVersionAsync(PolicyVersion entity)
        {
            entity.PolicyVersionId = _nextVersionId++;
            _versions[entity.PolicyVersionId] = entity;
            return Task.FromResult(entity.PolicyVersionId);
        }
        public Task UpdateVersionAsync(PolicyVersion entity)
        {
            _versions[entity.PolicyVersionId] = entity;
            return Task.CompletedTask;
        }
        public Task ReplaceBindingsAsync(long policyVersionId, IReadOnlyList<PolicyBinding> entities)
        {
            _bindings[policyVersionId] = entities;
            return Task.CompletedTask;
        }
        public Task ReplaceScopesAsync(long policyVersionId, IReadOnlyList<PolicyScope> entities)
        {
            _scopes[policyVersionId] = entities;
            return Task.CompletedTask;
        }
        public Task ReplaceParamsAsync(long policyVersionId, IReadOnlyList<PolicyParam> entities)
        {
            _params[policyVersionId] = entities;
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryTemplateRepository : ITemplateRepository
    {
        public Dictionary<long, TemplateVersion> TemplateVersions { get; } = new();
        public Dictionary<long, IReadOnlyList<TemplateParamDef>> ParamDefs { get; } = new();
        public Dictionary<long, IReadOnlyList<TemplateStepDef>> StepDefs { get; } = new();
        public Dictionary<long, IReadOnlyList<TemplateScopeDef>> ScopeDefs { get; } = new();

        public Task<IReadOnlyList<TemplateAggregate>> GetAllAsync() => Task.FromResult((IReadOnlyList<TemplateAggregate>)Array.Empty<TemplateAggregate>());
        public Task<TemplateAggregate?> GetByIdAsync(long templateId) => Task.FromResult<TemplateAggregate?>(null);
        public Task<TemplateAggregate?> GetByCodeAsync(string templateCode) => Task.FromResult<TemplateAggregate?>(null);
        public Task<IReadOnlyList<TemplateVersion>> GetVersionsByTemplateIdAsync(long templateId) => Task.FromResult((IReadOnlyList<TemplateVersion>)TemplateVersions.Values.Where(item => item.TemplateId == templateId).ToList());
        public Task<TemplateVersion?> GetVersionAsync(long templateVersionId) => Task.FromResult(TemplateVersions.TryGetValue(templateVersionId, out var item) ? item : null);
        public Task<IReadOnlyList<TemplateParamDef>> GetParamDefsAsync(long templateVersionId) => Task.FromResult(ParamDefs.TryGetValue(templateVersionId, out var items) ? items : (IReadOnlyList<TemplateParamDef>)Array.Empty<TemplateParamDef>());
        public Task<IReadOnlyList<TemplateStepDef>> GetStepDefsAsync(long templateVersionId) => Task.FromResult(StepDefs.TryGetValue(templateVersionId, out var items) ? items : (IReadOnlyList<TemplateStepDef>)Array.Empty<TemplateStepDef>());
        public Task<IReadOnlyList<TemplateScopeDef>> GetScopeDefsAsync(long templateVersionId) => Task.FromResult(ScopeDefs.TryGetValue(templateVersionId, out var items) ? items : (IReadOnlyList<TemplateScopeDef>)Array.Empty<TemplateScopeDef>());
        public Task<long> InsertAsync(TemplateAggregate entity) => Task.FromResult(0L);
        public Task UpdateAsync(TemplateAggregate entity) => Task.CompletedTask;
        public Task<long> InsertVersionAsync(TemplateVersion entity) => Task.FromResult(0L);
        public Task UpdateVersionAsync(TemplateVersion entity) => Task.CompletedTask;
        public Task ReplaceParamDefsAsync(long templateVersionId, IReadOnlyList<TemplateParamDef> entities) => Task.CompletedTask;
        public Task ReplaceStepDefsAsync(long templateVersionId, IReadOnlyList<TemplateStepDef> entities) => Task.CompletedTask;
        public Task ReplaceScopeDefsAsync(long templateVersionId, IReadOnlyList<TemplateScopeDef> entities) => Task.CompletedTask;
    }

    private sealed class NoopUnitOfWork : IUnitOfWork
    {
        public Task BeginAsync() => Task.CompletedTask;
        public Task CommitAsync() => Task.CompletedTask;
        public Task RollbackAsync() => Task.CompletedTask;
        public void Dispose() { }
    }
}
