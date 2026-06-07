using Pricing.RuleCenter.Core.Aggregates.Templates;

namespace Pricing.RuleCenter.Core.Interfaces.Templates;

public interface ITemplateRepository
{
    Task<IReadOnlyList<TemplateAggregate>> GetAllAsync();

    Task<TemplateAggregate?> GetByIdAsync(long templateId);

    Task<TemplateAggregate?> GetByCodeAsync(string templateCode);

    Task<IReadOnlyList<TemplateVersion>> GetVersionsByTemplateIdAsync(long templateId);

    Task<TemplateVersion?> GetVersionAsync(long templateVersionId);

    Task<IReadOnlyList<TemplateParamDef>> GetParamDefsAsync(long templateVersionId);

    Task<IReadOnlyList<TemplateStepDef>> GetStepDefsAsync(long templateVersionId);

    Task<IReadOnlyList<TemplateScopeDef>> GetScopeDefsAsync(long templateVersionId);

    Task<long> InsertAsync(TemplateAggregate entity);

    Task UpdateAsync(TemplateAggregate entity);

    Task<long> InsertVersionAsync(TemplateVersion entity);

    Task UpdateVersionAsync(TemplateVersion entity);

    Task ReplaceParamDefsAsync(long templateVersionId, IReadOnlyList<TemplateParamDef> entities);

    Task ReplaceStepDefsAsync(long templateVersionId, IReadOnlyList<TemplateStepDef> entities);

    Task ReplaceScopeDefsAsync(long templateVersionId, IReadOnlyList<TemplateScopeDef> entities);
}
