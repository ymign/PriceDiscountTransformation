using Pricing.RuleCenter.Core.Aggregates.Templates;

namespace Pricing.RuleCenter.Core.Interfaces.Templates;

public interface ITemplateRepository
{
    Task<TemplateAggregate?> GetByIdAsync(long templateId);

    Task<TemplateAggregate?> GetByCodeAsync(string templateCode);

    Task<TemplateVersion?> GetVersionAsync(long templateVersionId);

    Task<IReadOnlyList<TemplateParamDef>> GetParamDefsAsync(long templateVersionId);

    Task<IReadOnlyList<TemplateStepDef>> GetStepDefsAsync(long templateVersionId);

    Task<IReadOnlyList<TemplateScopeDef>> GetScopeDefsAsync(long templateVersionId);

    Task<long> InsertAsync(TemplateAggregate entity);

    Task UpdateAsync(TemplateAggregate entity);
}
