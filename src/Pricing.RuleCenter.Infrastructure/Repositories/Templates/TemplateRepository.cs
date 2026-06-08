using Pricing.RuleCenter.Core.Aggregates.Templates;
using Pricing.RuleCenter.Core.Interfaces.Templates;
using SqlSugar;

namespace Pricing.RuleCenter.Infrastructure.Repositories.Templates;

public sealed class TemplateRepository : ITemplateRepository
{
    private readonly ISqlSugarClient _db;

    public TemplateRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TemplateAggregate>> GetAllAsync()
    {
        return await _db.Queryable<TemplateAggregate>()
            .OrderBy(item => item.TemplateCode)
            .ToListAsync();
    }

    public async Task<TemplateAggregate?> GetByIdAsync(long templateId)
    {
        return await _db.Queryable<TemplateAggregate>()
            .InSingleAsync(templateId);
    }

    public async Task<TemplateAggregate?> GetByCodeAsync(string templateCode)
    {
        return await _db.Queryable<TemplateAggregate>()
            .FirstAsync(item => item.TemplateCode == templateCode);
    }

    public async Task<IReadOnlyList<TemplateVersion>> GetVersionsByTemplateIdAsync(long templateId)
    {
        return await _db.Queryable<TemplateVersion>()
            .Where(item => item.TemplateId == templateId)
            .OrderByDescending(item => item.VersionNo)
            .ToListAsync();
    }

    public async Task<TemplateVersion?> GetVersionAsync(long templateVersionId)
    {
        return await _db.Queryable<TemplateVersion>()
            .InSingleAsync(templateVersionId);
    }

    public async Task<IReadOnlyList<TemplateParamDef>> GetParamDefsAsync(long templateVersionId)
    {
        return await _db.Queryable<TemplateParamDef>()
            .Where(item => item.TemplateVersionId == templateVersionId)
            .OrderBy(item => item.SortNo)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TemplateStepDef>> GetStepDefsAsync(long templateVersionId)
    {
        return await _db.Queryable<TemplateStepDef>()
            .Where(item => item.TemplateVersionId == templateVersionId)
            .OrderBy(item => item.StepNo)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TemplateScopeDef>> GetScopeDefsAsync(long templateVersionId)
    {
        return await _db.Queryable<TemplateScopeDef>()
            .Where(item => item.TemplateVersionId == templateVersionId)
            .OrderBy(item => item.SortNo)
            .ToListAsync();
    }

    public async Task<long> InsertAsync(TemplateAggregate entity)
    {
        var templateId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_TEMPLATE.NEXTVAL FROM DUAL");
        entity.TemplateId = templateId;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return templateId;
    }

    public async Task UpdateAsync(TemplateAggregate entity)
    {
        await _db.Updateable(entity)
            .Where(item => item.TemplateId == entity.TemplateId)
            .ExecuteCommandAsync();
    }

    public async Task<long> InsertVersionAsync(TemplateVersion entity)
    {
        var templateVersionId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_TEMPLATE_VERSION.NEXTVAL FROM DUAL");
        entity.TemplateVersionId = templateVersionId;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return templateVersionId;
    }

    public async Task UpdateVersionAsync(TemplateVersion entity)
    {
        await _db.Updateable(entity)
            .Where(item => item.TemplateVersionId == entity.TemplateVersionId)
            .ExecuteCommandAsync();
    }

    public async Task ReplaceParamDefsAsync(long templateVersionId, IReadOnlyList<TemplateParamDef> entities)
    {
        await _db.Deleteable<TemplateParamDef>()
            .Where(item => item.TemplateVersionId == templateVersionId)
            .ExecuteCommandAsync();

        if (entities.Count == 0)
        {
            return;
        }

        foreach (var entity in entities)
        {
            entity.ParamDefId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_TEMPLATE_PARAM_DEF.NEXTVAL FROM DUAL");
            entity.TemplateVersionId = templateVersionId;
        }

        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }

    public async Task ReplaceStepDefsAsync(long templateVersionId, IReadOnlyList<TemplateStepDef> entities)
    {
        await _db.Deleteable<TemplateStepDef>()
            .Where(item => item.TemplateVersionId == templateVersionId)
            .ExecuteCommandAsync();

        if (entities.Count == 0)
        {
            return;
        }

        foreach (var entity in entities)
        {
            entity.StepDefId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_TEMPLATE_STEP_DEF.NEXTVAL FROM DUAL");
            entity.TemplateVersionId = templateVersionId;
        }

        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }

    public async Task ReplaceScopeDefsAsync(long templateVersionId, IReadOnlyList<TemplateScopeDef> entities)
    {
        await _db.Deleteable<TemplateScopeDef>()
            .Where(item => item.TemplateVersionId == templateVersionId)
            .ExecuteCommandAsync();

        if (entities.Count == 0)
        {
            return;
        }

        foreach (var entity in entities)
        {
            entity.ScopeDefId = await _db.Ado.GetLongAsync("SELECT SEQ_PR_TEMPLATE_SCOPE_DEF.NEXTVAL FROM DUAL");
            entity.TemplateVersionId = templateVersionId;
        }

        await _db.Insertable(entities.ToList()).ExecuteCommandAsync();
    }
}
