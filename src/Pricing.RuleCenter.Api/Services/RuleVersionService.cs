using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

public sealed class RuleVersionService
{
    private readonly IRuleVersionRepository _versionRepository;
    private readonly IRuleHeaderRepository _headerRepository;
    private readonly ILogger<RuleVersionService> _logger;

    public RuleVersionService(
        IRuleVersionRepository versionRepository,
        IRuleHeaderRepository headerRepository,
        ILogger<RuleVersionService> logger)
    {
        _versionRepository = versionRepository;
        _headerRepository = headerRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RuleVersionResponse>> GetByRuleIdAsync(long ruleId)
    {
        var items = await _versionRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapToResponse).ToList();
    }

    public async Task<RuleVersionResponse?> GetByIdAsync(long versionId)
    {
        var entity = await _versionRepository.GetByIdAsync(versionId);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<long> CreateDraftAsync(long ruleId)
    {
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");

        var versions = await _versionRepository.GetByRuleIdAsync(ruleId);
        var nextVersionNo = versions.Count > 0
            ? versions.Max(v => v.VersionNo) + 1
            : 1;

        var entity = new RuleVersion
        {
            RuleId = ruleId,
            VersionNo = nextVersionNo,
            VersionStatus = "DRAFT",
            EffectiveFrom = header.EffectiveFrom,
            EffectiveTo = header.EffectiveTo
        };

        var id = await _versionRepository.InsertAsync(entity);
        _logger.LogInformation("新增规则版本 RuleId={RuleId}, VersionNo={VersionNo}, VersionId={VersionId}",
            ruleId, nextVersionNo, id);
        return id;
    }

    private static RuleVersionResponse MapToResponse(RuleVersion entity)
    {
        return new RuleVersionResponse
        {
            VersionId = entity.VersionId,
            RuleId = entity.RuleId,
            VersionNo = entity.VersionNo,
            VersionStatus = entity.VersionStatus,
            EffectiveFrom = entity.EffectiveFrom,
            EffectiveTo = entity.EffectiveTo,
            RuleSnapshot = entity.RuleSnapshot,
            PublishedBy = entity.PublishedBy,
            PublishedAt = entity.PublishedAt,
            PublishRemark = entity.PublishRemark
        };
    }
}
