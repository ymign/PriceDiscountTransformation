using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Templates;
using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Templates;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class TemplateAppServiceTests
{
    [Fact]
    public async Task CreateAndSaveVersion_ShouldPersistTemplateAndDefinitions()
    {
        var repository = new InMemoryTemplateRepository();
        var clock = new FixedClock(new DateTime(2026, 6, 7, 13, 0, 0));
        var templateAppService = new TemplateAppService(repository, clock);
        var versionAppService = new TemplateVersionAppService(repository, new NoopUnitOfWork(), clock);

        var templateId = await templateAppService.CreateAsync(new TemplateCreateRequest
        {
            TemplateCode = "TPL001",
            TemplateName = "模板1",
            Category = "FORMULA",
            RiskLevel = "LOW",
            ExpressionMode = "WEAK",
            CreatedBy = "tester"
        });
        var templateVersionId = await versionAppService.SaveAsync(templateId, new TemplateVersionSaveRequest
        {
            CapabilityFamily = "FORMULA_PRICING",
            MergeMode = "SINGLE_WINNER",
            ParamDefs = new[]
            {
                new TemplateParamDefDto { ParamCode = "RATE", ParamName = "比例", ValueType = "NUMBER", IsRequired = true, SortNo = 1 }
            },
            StepDefs = new[]
            {
                new TemplateStepDefDto { StepNo = 10, StepKind = "ACTION", CapabilityCode = "FORMULA_PRICING", ActionType = "FORMULA_CALC", ExecutorCode = "INCREMENT_PERCENT" }
            },
            ScopeDefs = new[]
            {
                new TemplateScopeDefDto { ScopeDimension = "SCENE", IsRequired = true, SortNo = 1 }
            }
        });

        var detail = await templateAppService.GetByIdAsync(templateId);
        var version = await versionAppService.GetByIdAsync(templateVersionId);

        Assert.NotNull(detail);
        Assert.Equal("TPL001", detail!.TemplateCode);
        Assert.Equal(1, detail.CurrentVersionNo);
        Assert.NotNull(version);
        Assert.Single(version!.ParamDefs);
        Assert.Single(version.StepDefs);
        Assert.Single(version.ScopeDefs);
    }

    [Fact]
    public async Task SaveAsync_RejectsTemplateVersionFromAnotherTemplate()
    {
        var repository = new InMemoryTemplateRepository();
        var clock = new FixedClock(new DateTime(2026, 6, 8, 9, 10, 0));
        var templateAppService = new TemplateAppService(repository, clock);
        var versionAppService = new TemplateVersionAppService(repository, new NoopUnitOfWork(), clock);

        var firstTemplateId = await templateAppService.CreateAsync(new TemplateCreateRequest
        {
            TemplateCode = "TPL_A",
            TemplateName = "模板A",
            Category = "FORMULA",
            RiskLevel = "LOW",
            ExpressionMode = "WEAK"
        });
        var secondTemplateId = await templateAppService.CreateAsync(new TemplateCreateRequest
        {
            TemplateCode = "TPL_B",
            TemplateName = "模板B",
            Category = "FORMULA",
            RiskLevel = "LOW",
            ExpressionMode = "WEAK"
        });
        var firstVersionId = await versionAppService.SaveAsync(firstTemplateId, new TemplateVersionSaveRequest
        {
            CapabilityFamily = "FORMULA_PRICING",
            MergeMode = "SINGLE_WINNER"
        });

        var ex = await Assert.ThrowsAsync<BizException>(() =>
            versionAppService.SaveAsync(secondTemplateId, new TemplateVersionSaveRequest
            {
                TemplateVersionId = firstVersionId,
                CapabilityFamily = "FORMULA_PRICING",
                MergeMode = "SINGLE_WINNER"
            }));

        Assert.Equal(BizErrorCode.TemplateVersionNotFound, ex.Code);
    }

    private sealed class InMemoryTemplateRepository : ITemplateRepository
    {
        private long _nextTemplateId = 1;
        private long _nextTemplateVersionId = 100;
        private readonly Dictionary<long, TemplateAggregate> _templates = new();
        private readonly Dictionary<long, TemplateVersion> _versions = new();
        private readonly Dictionary<long, IReadOnlyList<TemplateParamDef>> _paramDefs = new();
        private readonly Dictionary<long, IReadOnlyList<TemplateStepDef>> _stepDefs = new();
        private readonly Dictionary<long, IReadOnlyList<TemplateScopeDef>> _scopeDefs = new();

        public Task<IReadOnlyList<TemplateAggregate>> GetAllAsync() =>
            Task.FromResult((IReadOnlyList<TemplateAggregate>)_templates.Values.OrderBy(item => item.TemplateCode).ToList());
        public Task<TemplateAggregate?> GetByIdAsync(long templateId) =>
            Task.FromResult(_templates.TryGetValue(templateId, out var item) ? item : null);
        public Task<TemplateAggregate?> GetByCodeAsync(string templateCode) =>
            Task.FromResult(_templates.Values.FirstOrDefault(item => item.TemplateCode == templateCode));
        public Task<IReadOnlyList<TemplateVersion>> GetVersionsByTemplateIdAsync(long templateId) =>
            Task.FromResult((IReadOnlyList<TemplateVersion>)_versions.Values.Where(item => item.TemplateId == templateId).OrderByDescending(item => item.VersionNo).ToList());
        public Task<TemplateVersion?> GetVersionAsync(long templateVersionId) =>
            Task.FromResult(_versions.TryGetValue(templateVersionId, out var item) ? item : null);
        public Task<IReadOnlyList<TemplateParamDef>> GetParamDefsAsync(long templateVersionId) =>
            Task.FromResult(_paramDefs.TryGetValue(templateVersionId, out var items) ? items : (IReadOnlyList<TemplateParamDef>)Array.Empty<TemplateParamDef>());
        public Task<IReadOnlyList<TemplateStepDef>> GetStepDefsAsync(long templateVersionId) =>
            Task.FromResult(_stepDefs.TryGetValue(templateVersionId, out var items) ? items : (IReadOnlyList<TemplateStepDef>)Array.Empty<TemplateStepDef>());
        public Task<IReadOnlyList<TemplateScopeDef>> GetScopeDefsAsync(long templateVersionId) =>
            Task.FromResult(_scopeDefs.TryGetValue(templateVersionId, out var items) ? items : (IReadOnlyList<TemplateScopeDef>)Array.Empty<TemplateScopeDef>());
        public Task<long> InsertAsync(TemplateAggregate entity)
        {
            entity.TemplateId = _nextTemplateId++;
            _templates[entity.TemplateId] = entity;
            return Task.FromResult(entity.TemplateId);
        }
        public Task UpdateAsync(TemplateAggregate entity)
        {
            _templates[entity.TemplateId] = entity;
            return Task.CompletedTask;
        }
        public Task<long> InsertVersionAsync(TemplateVersion entity)
        {
            entity.TemplateVersionId = _nextTemplateVersionId++;
            _versions[entity.TemplateVersionId] = entity;
            return Task.FromResult(entity.TemplateVersionId);
        }
        public Task UpdateVersionAsync(TemplateVersion entity)
        {
            _versions[entity.TemplateVersionId] = entity;
            return Task.CompletedTask;
        }
        public Task ReplaceParamDefsAsync(long templateVersionId, IReadOnlyList<TemplateParamDef> entities)
        {
            _paramDefs[templateVersionId] = entities;
            return Task.CompletedTask;
        }
        public Task ReplaceStepDefsAsync(long templateVersionId, IReadOnlyList<TemplateStepDef> entities)
        {
            _stepDefs[templateVersionId] = entities;
            return Task.CompletedTask;
        }
        public Task ReplaceScopeDefsAsync(long templateVersionId, IReadOnlyList<TemplateScopeDef> entities)
        {
            _scopeDefs[templateVersionId] = entities;
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
