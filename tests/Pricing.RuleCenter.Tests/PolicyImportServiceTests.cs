using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Application.RuntimePackages;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Templates;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PolicyImportServiceTests
{
    [Fact]
    public async Task ImportAsync_ImportsSupportedLegacyActionAsDraftPolicy()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var policyRepository = new InMemoryPolicyRepository();
        var templateRepository = new InMemoryTemplateRepository();

        headerRepository.Headers[1] = new RuleHeader
        {
            RuleId = 1,
            RuleCode = "RULE_OLD_001",
            RuleName = "旧规则1",
            ItemCode = "ITEM001",
            ItemName = "项目1",
            CurrentVersion = 2,
            Priority = 10
        };
        conditionRepository.Conditions[(1, 2)] = new[]
        {
            new RuleCondition
            {
                RuleId = 1,
                VersionNo = 2,
                ConditionType = "CHARGE_SCENE",
                OperatorType = "EQ",
                RightValue = "OUTPATIENT",
                IsEnabled = "Y",
                SortNo = 1
            }
        };
        actionRepository.Actions[(1, 2)] = new[]
        {
            new RuleAction
            {
                RuleId = 1,
                VersionNo = 2,
                ActionType = "FORMULA_CALC",
                ExecutorCode = "INCREMENT_PERCENT",
                ParamsJson = "{\"rate\":0.8}",
                IsEnabled = "Y"
            }
        };
        templateRepository.Templates["TPL_INCREMENT_PERCENT"] = new TemplateAggregate
        {
            TemplateId = 10,
            TemplateCode = "TPL_INCREMENT_PERCENT",
            TemplateName = "比例递增"
        };
        templateRepository.TemplateVersions[10] = new[]
        {
            new TemplateVersion
            {
                TemplateVersionId = 1001,
                TemplateId = 10,
                VersionNo = 1,
                CapabilityFamily = "FORMULA_PRICING",
                MergeMode = "SINGLE_WINNER"
            }
        };

        var service = new PolicyImportService(
            headerRepository,
            conditionRepository,
            actionRepository,
            policyRepository,
            templateRepository,
            new NoopUnitOfWork(),
            new FixedClock(new DateTime(2026, 6, 8, 10, 0, 0)));

        var imported = await service.ImportAsync(new[] { 1L }, "importer");

        var versionId = Assert.Single(imported);
        var policy = Assert.Single(policyRepository.Policies.Values);
        var version = policyRepository.Versions[versionId];
        Assert.Equal("IMP_RULE_OLD_001_INCREMENT_PERCENT", policy.PolicyCode);
        Assert.Equal("ITEM", version.BindingType);
        Assert.Single(policyRepository.Bindings[versionId]);
        Assert.Single(policyRepository.Scopes[versionId]);
        var parameter = Assert.Single(policyRepository.Params[versionId]);
        Assert.Equal(RuntimeRuleProjectionFactory.LegacyActionParamsJsonParamCode, parameter.ParamCode);
        Assert.Equal("{\"rate\":0.8}", parameter.ValueText);
    }

    [Fact]
    public async Task ImportAsync_RejectsSupportedAction_WhenLegacyConditionIsUnsupported()
    {
        var headerRepository = new InMemoryRuleHeaderRepository();
        var conditionRepository = new InMemoryRuleConditionRepository();
        var actionRepository = new InMemoryRuleActionRepository();
        var policyRepository = new InMemoryPolicyRepository();
        var templateRepository = new InMemoryTemplateRepository();

        headerRepository.Headers[1] = new RuleHeader
        {
            RuleId = 1,
            RuleCode = "RULE_OLD_UNSUPPORTED",
            RuleName = "旧规则不支持条件",
            ItemCode = "ITEM001",
            CurrentVersion = 1,
            Priority = 10
        };
        conditionRepository.Conditions[(1, 1)] = new[]
        {
            new RuleCondition
            {
                RuleId = 1,
                VersionNo = 1,
                ConditionType = "PATIENT_AGE_RANGE",
                OperatorType = "BETWEEN",
                RightValue = "0,6",
                IsEnabled = "Y",
                SortNo = 1
            }
        };
        actionRepository.Actions[(1, 1)] = new[]
        {
            new RuleAction
            {
                RuleId = 1,
                VersionNo = 1,
                ActionType = "FORMULA_CALC",
                ExecutorCode = "INCREMENT_PERCENT",
                ParamsJson = "{\"rate\":0.8}",
                IsEnabled = "Y"
            }
        };
        templateRepository.Templates["TPL_INCREMENT_PERCENT"] = new TemplateAggregate
        {
            TemplateId = 10,
            TemplateCode = "TPL_INCREMENT_PERCENT",
            TemplateName = "比例递增"
        };
        templateRepository.TemplateVersions[10] = new[]
        {
            new TemplateVersion
            {
                TemplateVersionId = 1001,
                TemplateId = 10,
                VersionNo = 1,
                CapabilityFamily = "FORMULA_PRICING",
                MergeMode = "SINGLE_WINNER"
            }
        };

        var service = new PolicyImportService(
            headerRepository,
            conditionRepository,
            actionRepository,
            policyRepository,
            templateRepository,
            new NoopUnitOfWork(),
            new FixedClock(new DateTime(2026, 6, 8, 10, 0, 0)));

        var ex = await Assert.ThrowsAsync<BizException>(() => service.ImportAsync(new[] { 1L }, "importer"));

        Assert.Equal(BizErrorCode.PolicyScopeUnsupported, ex.Code);
        Assert.Empty(policyRepository.Policies);
    }

    private sealed class InMemoryRuleHeaderRepository : IRuleHeaderRepository
    {
        public Dictionary<long, RuleHeader> Headers { get; } = new();
        public Task<RuleHeader?> GetByIdAsync(long ruleId) => Task.FromResult(Headers.TryGetValue(ruleId, out var item) ? item : null);
        public Task<RuleHeader?> GetByIdForUpdateAsync(long ruleId) => Task.FromResult<RuleHeader?>(null);
        public Task<RuleHeader?> GetByCodeAsync(string ruleCode) => Task.FromResult<RuleHeader?>(Headers.Values.FirstOrDefault(item => item.RuleCode == ruleCode));
        public Task<IReadOnlyList<RuleHeader>> GetByItemCodeAsync(string itemCode) => Task.FromResult((IReadOnlyList<RuleHeader>)Headers.Values.Where(item => item.ItemCode == itemCode).ToList());
        public Task<(IReadOnlyList<RuleHeader> Items, int Total)> GetPagedAsync(string? itemCode, string? status, string? category, int pageIndex, int pageSize) => Task.FromResult(((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>(), 0));
        public Task<IReadOnlyList<RuleHeader>> GetEffectiveAsync(DateTime businessTime) => Task.FromResult((IReadOnlyList<RuleHeader>)Array.Empty<RuleHeader>());
        public Task<long> InsertAsync(RuleHeader entity) => Task.FromResult(0L);
        public Task<bool> UpdateAsync(RuleHeader entity, string? expectedCurrentStatus = null) => Task.FromResult(false);
        public Task<bool> ExistsAsync(string ruleCode) => Task.FromResult(false);
    }

    private sealed class InMemoryRuleConditionRepository : IRuleConditionRepository
    {
        public Dictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleCondition>> Conditions { get; } = new();
        public Task<IReadOnlyList<RuleCondition>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult(Conditions.TryGetValue((ruleId, versionNo), out var items) ? items : (IReadOnlyList<RuleCondition>)Array.Empty<RuleCondition>());
        public Task InsertBatchAsync(IReadOnlyList<RuleCondition> entities) => Task.CompletedTask;
        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class InMemoryRuleActionRepository : IRuleActionRepository
    {
        public Dictionary<(long RuleId, int VersionNo), IReadOnlyList<RuleAction>> Actions { get; } = new();
        public Task<IReadOnlyList<RuleAction>> GetByRuleAndVersionAsync(long ruleId, int versionNo) =>
            Task.FromResult(Actions.TryGetValue((ruleId, versionNo), out var items) ? items : (IReadOnlyList<RuleAction>)Array.Empty<RuleAction>());
        public Task InsertBatchAsync(IReadOnlyList<RuleAction> entities) => Task.CompletedTask;
        public Task DeleteByRuleAndVersionAsync(long ruleId, int versionNo) => Task.CompletedTask;
    }

    private sealed class InMemoryPolicyRepository : IPolicyRepository
    {
        private long _nextPolicyId = 1;
        private long _nextVersionId = 100;
        public Dictionary<long, PolicyAggregate> Policies { get; } = new();
        public Dictionary<long, PolicyVersion> Versions { get; } = new();
        public Dictionary<long, IReadOnlyList<PolicyBinding>> Bindings { get; } = new();
        public Dictionary<long, IReadOnlyList<PolicyScope>> Scopes { get; } = new();
        public Dictionary<long, IReadOnlyList<PolicyParam>> Params { get; } = new();

        public Task<IReadOnlyList<PolicyAggregate>> GetAllAsync() => Task.FromResult((IReadOnlyList<PolicyAggregate>)Policies.Values.ToList());
        public Task<PolicyAggregate?> GetByIdAsync(long policyId) => Task.FromResult(Policies.TryGetValue(policyId, out var item) ? item : null);
        public Task<PolicyAggregate?> GetByCodeAsync(string policyCode) => Task.FromResult(Policies.Values.FirstOrDefault(item => item.PolicyCode == policyCode));
        public Task<IReadOnlyList<PolicyVersion>> GetVersionsByPolicyIdAsync(long policyId) => Task.FromResult((IReadOnlyList<PolicyVersion>)Versions.Values.Where(item => item.PolicyId == policyId).ToList());
        public Task<PolicyVersion?> GetVersionAsync(long policyVersionId) => Task.FromResult(Versions.TryGetValue(policyVersionId, out var item) ? item : null);
        public Task<IReadOnlyList<PolicyBinding>> GetBindingsAsync(long policyVersionId) => Task.FromResult(Bindings.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyBinding>)Array.Empty<PolicyBinding>());
        public Task<IReadOnlyList<PolicyScope>> GetScopesAsync(long policyVersionId) => Task.FromResult(Scopes.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyScope>)Array.Empty<PolicyScope>());
        public Task<IReadOnlyList<PolicyParam>> GetParamsAsync(long policyVersionId) => Task.FromResult(Params.TryGetValue(policyVersionId, out var items) ? items : (IReadOnlyList<PolicyParam>)Array.Empty<PolicyParam>());
        public Task<IReadOnlyList<PolicyVersion>> GetPublishReadyVersionsAsync() => Task.FromResult((IReadOnlyList<PolicyVersion>)Array.Empty<PolicyVersion>());
        public Task<long> InsertAsync(PolicyAggregate entity)
        {
            entity.PolicyId = _nextPolicyId++;
            Policies[entity.PolicyId] = entity;
            return Task.FromResult(entity.PolicyId);
        }
        public Task UpdateAsync(PolicyAggregate entity)
        {
            Policies[entity.PolicyId] = entity;
            return Task.CompletedTask;
        }
        public Task<long> InsertVersionAsync(PolicyVersion entity)
        {
            entity.PolicyVersionId = _nextVersionId++;
            Versions[entity.PolicyVersionId] = entity;
            return Task.FromResult(entity.PolicyVersionId);
        }
        public Task UpdateVersionAsync(PolicyVersion entity)
        {
            Versions[entity.PolicyVersionId] = entity;
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
            Params[policyVersionId] = entities;
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryTemplateRepository : ITemplateRepository
    {
        public Dictionary<string, TemplateAggregate> Templates { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<long, IReadOnlyList<TemplateVersion>> TemplateVersions { get; } = new();
        public Task<IReadOnlyList<TemplateAggregate>> GetAllAsync() => Task.FromResult((IReadOnlyList<TemplateAggregate>)Templates.Values.ToList());
        public Task<TemplateAggregate?> GetByIdAsync(long templateId) => Task.FromResult(Templates.Values.FirstOrDefault(item => item.TemplateId == templateId));
        public Task<TemplateAggregate?> GetByCodeAsync(string templateCode) => Task.FromResult(Templates.TryGetValue(templateCode, out var item) ? item : null);
        public Task<IReadOnlyList<TemplateVersion>> GetVersionsByTemplateIdAsync(long templateId) => Task.FromResult(TemplateVersions.TryGetValue(templateId, out var items) ? items : (IReadOnlyList<TemplateVersion>)Array.Empty<TemplateVersion>());
        public Task<TemplateVersion?> GetVersionAsync(long templateVersionId) => Task.FromResult(TemplateVersions.Values.SelectMany(items => items).FirstOrDefault(item => item.TemplateVersionId == templateVersionId));
        public Task<IReadOnlyList<TemplateParamDef>> GetParamDefsAsync(long templateVersionId) => Task.FromResult((IReadOnlyList<TemplateParamDef>)Array.Empty<TemplateParamDef>());
        public Task<IReadOnlyList<TemplateStepDef>> GetStepDefsAsync(long templateVersionId) => Task.FromResult((IReadOnlyList<TemplateStepDef>)Array.Empty<TemplateStepDef>());
        public Task<IReadOnlyList<TemplateScopeDef>> GetScopeDefsAsync(long templateVersionId) => Task.FromResult((IReadOnlyList<TemplateScopeDef>)Array.Empty<TemplateScopeDef>());
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
