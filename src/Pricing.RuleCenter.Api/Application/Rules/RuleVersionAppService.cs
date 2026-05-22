using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Api.Application.Rules;

/// <summary>
/// 规则版本应用服务，负责规则版本草稿的创建和查询。
/// </summary>
/// <remarks>
/// <para>
/// 职责边界：规则版本承载条件、动作和发布快照。发布、停用、回滚属于更完整的状态流转，
/// 统一放在 RulePublishAppService 中处理；本服务只负责生成新的 DRAFT 版本和版本查询。
/// </para>
/// <para>
/// 版本号规则：版本号在单条规则内部从 1 开始递增，发布和回滚不会复用旧版本号。
/// 这保证了每个版本号在同一条规则下唯一，便于审计追踪时精确定位当时生效的规则配置。
/// </para>
/// <para>
/// 草稿生命周期：CreateDraftAsync 创建的版本状态为 DRAFT，条件和动作由各自服务
/// 保存到该版本号下。草稿在发布前不参与计价匹配，发布后状态变为 PUBLISHED 并生成快照。
/// </para>
/// </remarks>
public sealed class RuleVersionAppService
{
    /// <summary>
    /// 规则版本仓储，负责 PR_RULE_VERSION 表的按规则查询、插入草稿和更新版本状态。
    /// </summary>
    private readonly IRuleVersionRepository _versionRepository;

    /// <summary>
    /// 规则主档仓储，用于创建版本前确认规则存在，并从主档继承生效时间作为版本的默认生效范围。
    /// </summary>
    private readonly IRuleHeaderRepository _headerRepository;

    /// <summary>
    /// 服务日志，用于记录版本草稿创建结果，同时输出 RuleId、VersionNo 和 VersionId
    /// 以便从接口返回、数据库记录和日志三方互相定位。
    /// </summary>
    private readonly ILogger<RuleVersionAppService> _logger;

    /// <summary>
    /// 初始化规则版本服务。
    /// </summary>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="logger">日志对象。</param>
    public RuleVersionAppService(
        IRuleVersionRepository versionRepository,
        IRuleHeaderRepository headerRepository,
        ILogger<RuleVersionAppService> logger)
    {
        _versionRepository = versionRepository;
        _headerRepository = headerRepository;
        _logger = logger;
    }

    /// <summary>
    /// 读取某条规则的全部版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>该规则下的版本列表。</returns>
    public async Task<IReadOnlyList<RuleVersionResponse>> GetByRuleIdAsync(long ruleId)
    {
        var items = await _versionRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapToResponse).ToList();
    }

    /// <summary>
    /// 按版本主键读取版本信息。
    /// </summary>
    /// <param name="versionId">版本主键。</param>
    /// <returns>找到时返回版本响应；不存在时返回 <c>null</c>。</returns>
    public async Task<RuleVersionResponse?> GetByIdAsync(long versionId)
    {
        var entity = await _versionRepository.GetByIdAsync(versionId);
        return entity is null ? null : MapToResponse(entity);
    }

    /// <summary>
    /// 为指定规则创建下一个版本号的草稿版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>新建草稿版本主键。</returns>
    /// <exception cref="BizException">规则不存在或已存在草稿版本时抛出结构化业务错误。</exception>
    public async Task<long> CreateDraftAsync(long ruleId)
    {
        // ========== 第一阶段：确认主档存在 ==========
        // 版本必须挂在规则主档下，同时默认继承主档生效时间，避免草稿缺少基础适用范围。
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new BizException(
                BizErrorCode.RuleNotFound,
                404,
                $"规则不存在: {ruleId}");

        // ========== 第二阶段：计算下一个版本号 ==========
        // 版本号只在单条规则内部递增，发布和回滚不会复用旧版本号，便于审计追踪。
        var versions = await _versionRepository.GetByRuleIdAsync(ruleId);
        if (versions.Any(v => string.Equals(v.VersionStatus, "DRAFT", StringComparison.OrdinalIgnoreCase)))
        {
            throw new BizException(
                BizErrorCode.DraftVersionAlreadyExists,
                409,
                $"RuleId={ruleId} 已存在草稿版本，不允许重复创建");
        }

        var nextVersionNo = versions.Count > 0
            ? versions.Max(v => v.VersionNo) + 1
            : 1;

        // ========== 第三阶段：插入 DRAFT 草稿 ==========
        // 条件和动作由各自服务保存到该版本号下，草稿发布前不会参与计价匹配。
        var entity = new RuleVersion
        {
            RuleId = ruleId,
            VersionNo = nextVersionNo,
            VersionStatus = "DRAFT",
            EffectiveFrom = header.EffectiveFrom,
            EffectiveTo = header.EffectiveTo
        };

        // ========== 第四阶段：记录创建结果 ==========
        // 日志中同时输出 RuleId、VersionNo 和 VersionId，方便从接口返回、数据库记录和日志三方互相定位。
        var id = await _versionRepository.InsertAsync(entity);
        _logger.LogInformation("新增规则版本 RuleId={RuleId}, VersionNo={VersionNo}, VersionId={VersionId}",
            ruleId, nextVersionNo, id);
        return id;
    }

    /// <summary>
    /// 将规则版本实体映射为接口响应。
    /// </summary>
    /// <remarks>
    /// RuleSnapshot 字段在已发布版本中包含完整的条件和动作快照 JSON，
    /// 用于计价追溯时还原当时的规则配置。草稿版本中该字段为 null。
    /// </remarks>
    /// <param name="entity">规则版本实体，来自 PR_RULE_VERSION 表。</param>
    /// <returns>规则版本响应 DTO，包含版本状态、发布人和发布快照。</returns>
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


