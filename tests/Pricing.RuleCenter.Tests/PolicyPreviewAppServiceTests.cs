using Pricing.RuleCenter.Application.Policies;
using Pricing.RuleCenter.Core.Aggregates.Policies;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Engine.Formula;
using Pricing.RuleCenter.Core.Interfaces.Policies;
using Pricing.RuleCenter.Core.Interfaces.Templates;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PolicyPreviewAppServiceTests
{
    [Fact]
    public async Task PreviewAsync_ReturnsReadableBindingScopeAndActionChain()
    {
        var policyRepository = new InMemoryPolicyRepository();
        var templateRepository = new InMemoryTemplateRepository();
        var service = new PolicyPreviewAppService(
            policyRepository,
            templateRepository,
            new PolicyValidationService(new FormulaExpressionValidator(new FormulaExpressionEvaluator(new FormulaFunctionRegistry()))));

        var preview = await service.PreviewAsync(101);

        Assert.Equal("POL001", preview.PolicyCode);
        Assert.Contains("ITEM:ITEM001", preview.BindingSummary);
        Assert.Contains("SCENE EQ OUTPATIENT", preview.ScopeSummary);
        Assert.Contains("10:FORMULA_CALC/INCREMENT_PERCENT", preview.ActionChain);
    }

    private sealed class InMemoryPolicyRepository : IPolicyRepository
    {
        public Task<IReadOnlyList<PolicyAggregate>> GetAllAsync() => Task.FromResult((IReadOnlyList<PolicyAggregate>)Array.Empty<PolicyAggregate>());
        public Task<PolicyAggregate?> GetByIdAsync(long policyId) => Task.FromResult<PolicyAggregate?>(new PolicyAggregate { PolicyId = 1, PolicyCode = "POL001" });
        public Task<PolicyAggregate?> GetByCodeAsync(string policyCode) => Task.FromResult<PolicyAggregate?>(null);
        public Task<IReadOnlyList<PolicyVersion>> GetVersionsByPolicyIdAsync(long policyId) => Task.FromResult((IReadOnlyList<PolicyVersion>)Array.Empty<PolicyVersion>());
        public Task<PolicyVersion?> GetVersionAsync(long policyVersionId) => Task.FromResult<PolicyVersion?>(new PolicyVersion { PolicyVersionId = 101, PolicyId = 1, TemplateVersionId = 1001, VersionNo = 1, PolicyStatus = "DRAFT", BindingType = "ITEM", ScopeLevel = "SCENE", PriorityWeight = 10 });
        public Task<IReadOnlyList<PolicyBinding>> GetBindingsAsync(long policyVersionId) => Task.FromResult((IReadOnlyList<PolicyBinding>)new[] { new PolicyBinding { BindingType = "ITEM", ItemCode = "ITEM001" } });
        public Task<IReadOnlyList<PolicyScope>> GetScopesAsync(long policyVersionId) => Task.FromResult((IReadOnlyList<PolicyScope>)new[] { new PolicyScope { ScopeDimension = "SCENE", ScopeOperator = "EQ", ScopeValueText = "OUTPATIENT" } });
        public Task<IReadOnlyList<PolicyParam>> GetParamsAsync(long policyVersionId) => Task.FromResult((IReadOnlyList<PolicyParam>)new[] { new PolicyParam { ParamCode = "RATE", ValueType = "NUMBER", ValueNumber = 0.8m } });
        public Task<IReadOnlyList<PolicyVersion>> GetPublishReadyVersionsAsync() => Task.FromResult((IReadOnlyList<PolicyVersion>)Array.Empty<PolicyVersion>());
        public Task<long> InsertAsync(PolicyAggregate entity) => Task.FromResult(0L);
        public Task UpdateAsync(PolicyAggregate entity) => Task.CompletedTask;
        public Task<long> InsertVersionAsync(PolicyVersion entity) => Task.FromResult(0L);
        public Task UpdateVersionAsync(PolicyVersion entity) => Task.CompletedTask;
        public Task ReplaceBindingsAsync(long policyVersionId, IReadOnlyList<PolicyBinding> entities) => Task.CompletedTask;
        public Task ReplaceScopesAsync(long policyVersionId, IReadOnlyList<PolicyScope> entities) => Task.CompletedTask;
        public Task ReplaceParamsAsync(long policyVersionId, IReadOnlyList<PolicyParam> entities) => Task.CompletedTask;
    }

    private sealed class InMemoryTemplateRepository : ITemplateRepository
    {
        public Task<IReadOnlyList<TemplateAggregate>> GetAllAsync() => Task.FromResult((IReadOnlyList<TemplateAggregate>)Array.Empty<TemplateAggregate>());
        public Task<TemplateAggregate?> GetByIdAsync(long templateId) => Task.FromResult<TemplateAggregate?>(null);
        public Task<TemplateAggregate?> GetByCodeAsync(string templateCode) => Task.FromResult<TemplateAggregate?>(null);
        public Task<IReadOnlyList<TemplateVersion>> GetVersionsByTemplateIdAsync(long templateId) => Task.FromResult((IReadOnlyList<TemplateVersion>)Array.Empty<TemplateVersion>());
        public Task<TemplateVersion?> GetVersionAsync(long templateVersionId) => Task.FromResult<TemplateVersion?>(new TemplateVersion { TemplateVersionId = 1001, TemplateId = 1, VersionNo = 1, CapabilityFamily = "FORMULA_PRICING", MergeMode = "SINGLE_WINNER" });
        public Task<IReadOnlyList<TemplateParamDef>> GetParamDefsAsync(long templateVersionId) => Task.FromResult((IReadOnlyList<TemplateParamDef>)new[] { new TemplateParamDef { ParamCode = "RATE", ParamName = "比例", ValueType = "NUMBER", IsRequired = "Y", MinValue = 0m, MaxValue = 1m } });
        public Task<IReadOnlyList<TemplateStepDef>> GetStepDefsAsync(long templateVersionId) => Task.FromResult((IReadOnlyList<TemplateStepDef>)new[] { new TemplateStepDef { StepNo = 10, StepKind = "ACTION", CapabilityCode = "FORMULA_PRICING", ActionType = "FORMULA_CALC", ExecutorCode = "INCREMENT_PERCENT", OnError = "STOP" } });
        public Task<IReadOnlyList<TemplateScopeDef>> GetScopeDefsAsync(long templateVersionId) => Task.FromResult((IReadOnlyList<TemplateScopeDef>)new[] { new TemplateScopeDef { ScopeDimension = "SCENE", IsRequired = "Y" } });
        public Task<long> InsertAsync(TemplateAggregate entity) => Task.FromResult(0L);
        public Task UpdateAsync(TemplateAggregate entity) => Task.CompletedTask;
        public Task<long> InsertVersionAsync(TemplateVersion entity) => Task.FromResult(0L);
        public Task UpdateVersionAsync(TemplateVersion entity) => Task.CompletedTask;
        public Task ReplaceParamDefsAsync(long templateVersionId, IReadOnlyList<TemplateParamDef> entities) => Task.CompletedTask;
        public Task ReplaceStepDefsAsync(long templateVersionId, IReadOnlyList<TemplateStepDef> entities) => Task.CompletedTask;
        public Task ReplaceScopeDefsAsync(long templateVersionId, IReadOnlyList<TemplateScopeDef> entities) => Task.CompletedTask;
    }
}
