using Microsoft.Extensions.Caching.Memory;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Api.Services;

/// <summary>
/// 规则发布应用服务，负责发布、停用和回滚规则版本。
/// </summary>
/// <remarks>
/// <para>
/// 这是规则配置链路中最重要的状态机入口。它负责在规则主档、版本表、发布记录和变更日志之间保持一致：
/// 发布会把草稿版本变成当前生效版本；停用会让规则整体退出匹配；回滚会把当前版本切回最近一个历史版本。
/// </para>
/// <para>
/// 缓存失效：发布、停用、回滚操作完成后，必须立即清除生效规则缓存，
/// 确保计价引擎在下一次请求时读到最新规则集。这是资金安全硬约束。
/// </para>
/// </remarks>
public sealed class RulePublishService
{
    /// <summary>
    /// 规则主档仓储，用于读取和更新当前版本、规则状态及启用标志。
    /// </summary>
    private readonly IRuleHeaderRepository _headerRepository;
    /// <summary>
    /// 规则版本仓储，用于校验草稿、禁用旧版本、发布新版本和回滚历史版本。
    /// </summary>
    private readonly IRuleVersionRepository _versionRepository;
    /// <summary>
    /// 发布记录仓储，用于保存发布、停用和回滚的可审计流水。
    /// </summary>
    private readonly IRulePublishRepository _publishRepository;
    /// <summary>
    /// 变更日志仓储，用于记录面向配置人员的规则变更摘要。
    /// </summary>
    private readonly IRuleChangeLogRepository _changeLogRepository;
    /// <summary>
    /// 规则条件仓储，用于发布前读取目标版本和现有发布版本的条件维度。
    /// </summary>
    private readonly IRuleConditionRepository _conditionRepository;
    /// <summary>
    /// 规则动作仓储，用于发布前识别公式、额度和换算动作冲突。
    /// </summary>
    private readonly IRuleActionRepository _actionRepository;
    /// <summary>
    /// 字典仓储，用于从 PR_DICT 表读取互斥动作类型等可配置数据。
    /// </summary>
    private readonly IDictRepository _dictRepository;
    /// <summary>
    /// 内存缓存，用于在发布/停用/回滚后立即清除生效规则缓存。
    /// </summary>
    private readonly IMemoryCache _cache;
    /// <summary>
    /// 服务日志，用于记录状态机入口的关键操作。
    /// </summary>
    private readonly ILogger<RulePublishService> _logger;

    /// <summary>
    /// 互斥动作类型缓存。从 PR_DICT 表中 DICT_TYPE = "MUTUALLY_EXCLUSIVE_ACTION_TYPE" 的字典项加载。
    /// 在 ValidatePublishConflictsAsync 首次调用时加载，之后使用缓存值。
    /// 规则发布/停用/回滚或字典维护后应清除缓存，确保下次读取最新互斥规则。
    /// </summary>
    private HashSet<string>? _mutuallyExclusiveActionsCache;

    /// <summary>
    /// 缓存加载锁，确保多线程并发时只加载一次互斥动作类型数据。
    /// </summary>
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    /// <summary>
    /// 字典类型编码：发布互斥动作类型。
    /// </summary>
    private const string MutuallyExclusiveActionTypeDictType = "MUTUALLY_EXCLUSIVE_ACTION_TYPE";

    /// <summary>
    /// 初始化规则发布服务。
    /// </summary>
    /// <param name="headerRepository">规则主档仓储。</param>
    /// <param name="versionRepository">规则版本仓储。</param>
    /// <param name="publishRepository">发布记录仓储。</param>
    /// <param name="changeLogRepository">变更日志仓储。</param>
    /// <param name="conditionRepository">规则条件仓储。</param>
    /// <param name="actionRepository">规则动作仓储。</param>
    /// <param name="dictRepository">字典仓储，用于读取互斥动作类型配置。</param>
    /// <param name="cache">内存缓存，用于在状态变更后清除生效规则缓存。</param>
    /// <param name="logger">日志对象。</param>
    public RulePublishService(
        IRuleHeaderRepository headerRepository,
        IRuleVersionRepository versionRepository,
        IRulePublishRepository publishRepository,
        IRuleChangeLogRepository changeLogRepository,
        IRuleConditionRepository conditionRepository,
        IRuleActionRepository actionRepository,
        IDictRepository dictRepository,
        IMemoryCache cache,
        ILogger<RulePublishService> logger)
    {
        _headerRepository = headerRepository;
        _versionRepository = versionRepository;
        _publishRepository = publishRepository;
        _changeLogRepository = changeLogRepository;
        _conditionRepository = conditionRepository;
        _actionRepository = actionRepository;
        _dictRepository = dictRepository;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// 读取规则的发布历史。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>发布、停用、回滚流水列表。</returns>
    public async Task<IReadOnlyList<RulePublishResponse>> GetPublishHistoryAsync(long ruleId)
    {
        var items = await _publishRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapPublishToResponse).ToList();
    }

    /// <summary>
    /// 读取规则变更日志。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>规则变更摘要列表。</returns>
    public async Task<IReadOnlyList<RuleChangeLogResponse>> GetChangeLogsAsync(long ruleId)
    {
        var items = await _changeLogRepository.GetByRuleIdAsync(ruleId);
        return items.Select(MapChangeLogToResponse).ToList();
    }

    /// <summary>
    /// 发布指定草稿版本，让其成为规则当前生效版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">发布请求，包含目标版本号、发布人和备注。</param>
    /// <exception cref="KeyNotFoundException">规则主档或目标版本不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">目标版本不是草稿状态时抛出。</exception>
    public async Task PublishAsync(long ruleId, RulePublishRequest request)
    {
        // ========== 第一阶段：读取主档和目标版本 ==========
        // 发布必须同时拿到主档和版本。主档提供当前版本号，版本表提供目标版本的状态边界。
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");

        var version = await _versionRepository.GetByRuleAndVersionAsync(ruleId, request.VersionNo)
            ?? throw new KeyNotFoundException($"规则版本不存在: RuleId={ruleId}, VersionNo={request.VersionNo}");

        // ========== 第二阶段：校验目标版本必须是草稿 ==========
        // 只允许 DRAFT -> PUBLISHED。已经发布、禁用或回滚过的版本不能再次走普通发布入口，
        // 避免发布历史出现同一个版本被多次发布的歧义。
        if (version.VersionStatus != "DRAFT")
        {
            throw new InvalidOperationException($"只有草稿版本可以发布, 当前状态: {version.VersionStatus}");
        }

        await ValidatePublishConflictsAsync(header, request.VersionNo);

        // ========== 第三阶段：禁用旧生效版本 ==========
        // 系统同一条规则只允许一个当前版本。旧版本保留为 DISABLED，便于回滚时找到最近的历史版本。
        var oldVersion = header.CurrentVersion;

        if (oldVersion > 0)
        {
            await _versionRepository.UpdateStatusAsync(
                (await _versionRepository.GetByRuleAndVersionAsync(ruleId, oldVersion))!.VersionId,
                "DISABLED");
        }

        // ========== 第四阶段：发布目标版本并推进主档 ==========
        // 版本状态和主档 CurrentVersion 必须一起推进，否则计价匹配会看到主档和版本表不一致。
        await _versionRepository.UpdateStatusAsync(version.VersionId, "PUBLISHED");

        header.CurrentVersion = request.VersionNo;
        header.Status = "PUBLISHED";
        header.UpdatedAt = DateTime.Now;
        await _headerRepository.UpdateAsync(header);

        // ========== 第五阶段：写发布流水 ==========
        // PublishNo 带规则、版本和时间戳，便于和日志、人工发布单进行交叉核对。
        var publishNo = $"PUB-{ruleId}-{request.VersionNo}-{DateTime.Now:yyyyMMddHHmmss}";
        await _publishRepository.InsertAsync(new RulePublish
        {
            PublishNo = publishNo,
            RuleId = ruleId,
            FromVersion = oldVersion > 0 ? oldVersion : null,
            ToVersion = request.VersionNo,
            ActionType = "PUBLISH",
            PublishedBy = request.PublishedBy,
            PublishedAt = DateTime.Now,
            Remark = request.Remark
        });

        // ========== 第六阶段：写变更摘要 ==========
        // 变更日志面向配置人员，语义比发布流水更轻，用于页面展示"谁做了什么"。
        await _changeLogRepository.InsertAsync(new RuleChangeLog
        {
            RuleId = ruleId,
            VersionNo = request.VersionNo,
            ChangeType = "PUBLISH",
            ChangeSummary = $"发布版本 V{request.VersionNo}",
            ChangedBy = request.PublishedBy,
            ChangedAt = DateTime.Now
        });

        // ========== 第七阶段：清除生效规则缓存 ==========
        // 发布改变了当前生效版本，必须立即清除缓存，确保计价引擎读到最新规则集。
        ClearEffectiveCache();

        _logger.LogInformation("发布规则 RuleId={RuleId}, VersionNo={VersionNo}", ruleId, request.VersionNo);
    }

    /// <summary>
    /// 停用已发布规则。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">停用请求，包含操作人和备注。</param>
    /// <exception cref="KeyNotFoundException">规则不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">规则不是已发布状态时抛出。</exception>
    public async Task DisableAsync(long ruleId, RuleDisableRequest request)
    {
        // ========== 第一阶段：读取并校验主档状态 ==========
        // 只有已发布规则才有"停用"的业务含义，草稿规则不应通过停用入口改变可编辑状态。
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");

        if (header.Status != "PUBLISHED")
        {
            throw new InvalidOperationException($"只有已发布的规则可以停用, 当前状态: {header.Status}");
        }

        // ========== 第二阶段：禁用当前版本 ==========
        // 版本表同步置为 DISABLED，确保后续回滚/审计能够知道它曾经生效但当前已停用。
        if (header.CurrentVersion > 0)
        {
            var currentVersion = await _versionRepository.GetByRuleAndVersionAsync(ruleId, header.CurrentVersion);
            if (currentVersion is not null)
            {
                await _versionRepository.UpdateStatusAsync(currentVersion.VersionId, "DISABLED");
            }
        }

        // ========== 第三阶段：更新主档启用状态 ==========
        // 计价匹配通常先看主档是否启用，因此主档 IsEnabled 必须明确置 N。
        header.Status = "DISABLED";
        header.IsEnabled = "N";
        header.UpdatedAt = DateTime.Now;
        await _headerRepository.UpdateAsync(header);

        // ========== 第四阶段：写停用流水和变更日志 ==========
        // 停用不改变 CurrentVersion，只表示该版本不再参与匹配；流水中 From/To 都记录当前版本。
        await _publishRepository.InsertAsync(new RulePublish
        {
            PublishNo = $"DIS-{ruleId}-{DateTime.Now:yyyyMMddHHmmss}",
            RuleId = ruleId,
            FromVersion = header.CurrentVersion,
            ToVersion = header.CurrentVersion,
            ActionType = "DISABLE",
            PublishedBy = request.PublishedBy,
            PublishedAt = DateTime.Now,
            Remark = request.Remark
        });

        await _changeLogRepository.InsertAsync(new RuleChangeLog
        {
            RuleId = ruleId,
            VersionNo = header.CurrentVersion,
            ChangeType = "DISABLE",
            ChangeSummary = $"停用规则, 当前版本 V{header.CurrentVersion}",
            ChangedBy = request.PublishedBy,
            ChangedAt = DateTime.Now
        });

        // ========== 第五阶段：清除生效规则缓存 ==========
        // 停用后规则不再参与匹配，必须立即清除缓存。
        ClearEffectiveCache();

        _logger.LogInformation("停用规则 RuleId={RuleId}", ruleId);
    }

    /// <summary>
    /// 将已发布规则回滚到最近一个历史发布版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">回滚请求，包含操作人和备注。</param>
    /// <exception cref="KeyNotFoundException">规则不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">规则未发布或没有可回滚版本时抛出。</exception>
    public async Task RollbackAsync(long ruleId, RuleRollbackRequest request)
    {
        // ========== 第一阶段：读取并校验主档 ==========
        // 回滚只对已发布规则有效；停用或草稿状态下回滚会让规则是否参与匹配变得不清晰。
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");

        if (header.Status != "PUBLISHED")
        {
            throw new InvalidOperationException($"只有已发布的规则可以回滚, 当前状态: {header.Status}");
        }

        // ========== 第二阶段：定位最近的历史版本 ==========
        // 这里选择小于当前版本号且状态为 DISABLED 的最大版本号，表示"最近一次被替换下来的发布版本"。
        var versions = await _versionRepository.GetByRuleIdAsync(ruleId);
        var previousPublished = versions
            .Where(v => v.VersionNo < header.CurrentVersion && v.VersionStatus == "DISABLED")
            .OrderByDescending(v => v.VersionNo)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("没有可回滚的历史版本");

        // ========== 第三阶段：把当前版本标记为已回滚 ==========
        // ROLLED_BACK 与 DISABLED 区分开来，方便审计判断该版本是普通被新版本替换，还是被主动撤回。
        var currentVersion = await _versionRepository.GetByRuleAndVersionAsync(ruleId, header.CurrentVersion);
        if (currentVersion is not null)
        {
            await _versionRepository.UpdateStatusAsync(currentVersion.VersionId, "ROLLED_BACK");
        }

        // ========== 第四阶段：恢复历史版本为发布状态 ==========
        // 主档 CurrentVersion 随后会指向该版本，使计价匹配重新使用历史规则内容。
        await _versionRepository.UpdateStatusAsync(previousPublished.VersionId, "PUBLISHED");

        var oldVersionNo = header.CurrentVersion;
        header.CurrentVersion = previousPublished.VersionNo;
        header.UpdatedAt = DateTime.Now;
        await _headerRepository.UpdateAsync(header);

        // ========== 第五阶段：记录回滚流水和变更摘要 ==========
        // 回滚会改变当前生效版本，必须同时写发布流水和变更日志，便于追查某次计价为什么回到旧规则。
        await _publishRepository.InsertAsync(new RulePublish
        {
            PublishNo = $"RB-{ruleId}-{previousPublished.VersionNo}-{DateTime.Now:yyyyMMddHHmmss}",
            RuleId = ruleId,
            FromVersion = oldVersionNo,
            ToVersion = previousPublished.VersionNo,
            ActionType = "ROLLBACK",
            PublishedBy = request.PublishedBy,
            PublishedAt = DateTime.Now,
            Remark = request.Remark
        });

        await _changeLogRepository.InsertAsync(new RuleChangeLog
        {
            RuleId = ruleId,
            VersionNo = previousPublished.VersionNo,
            ChangeType = "ROLLBACK",
            ChangeSummary = $"从 V{oldVersionNo} 回滚到 V{previousPublished.VersionNo}",
            ChangedBy = request.PublishedBy,
            ChangedAt = DateTime.Now
        });

        // ========== 第六阶段：清除生效规则缓存 ==========
        // 回滚改变了当前生效版本，必须立即清除缓存。
        ClearEffectiveCache();

        _logger.LogInformation("回滚规则 RuleId={RuleId}, 从 V{FromVersion} 到 V{ToVersion}",
            ruleId, oldVersionNo, previousPublished.VersionNo);
    }

    private async Task ValidatePublishConflictsAsync(RuleHeader targetHeader, int targetVersionNo)
    {
        if (string.IsNullOrWhiteSpace(targetHeader.ItemCode))
        {
            return;
        }

        var targetProfile = await BuildRuleProfileAsync(targetHeader, targetVersionNo);
        var sameItemRules = await _headerRepository.GetByItemCodeAsync(targetHeader.ItemCode);
        var publishedRules = sameItemRules
            .Where(r => r.RuleId != targetHeader.RuleId)
            .Where(r => r.Status == "PUBLISHED" && r.IsEnabled == "Y")
            .Where(r => IsEffectiveRangeOverlap(targetHeader, r))
            .ToList();

        foreach (var existingRule in publishedRules)
        {
            var existingProfile = await BuildRuleProfileAsync(existingRule, existingRule.CurrentVersion);
            if (!HasSceneOverlap(targetProfile.ConditionScopes, existingProfile.ConditionScopes))
            {
                continue;
            }

            var (hasConflict, conflictActionType) = await HasForbiddenActionConflictAsync(targetProfile.Actions, existingProfile.Actions);
            if (hasConflict)
            {
                throw new InvalidOperationException(
                    $"RULE_CONFLICT: 项目 {targetHeader.ItemCode} 在相同场景和生效期内已存在 {conflictActionType} 规则，" +
                    $"RuleId={existingRule.RuleId}");
            }

            if (targetProfile.Actions.Contains("CONVERT_QTY") &&
                existingProfile.Actions.Contains("CONVERT_QTY") &&
                HasSceneAndBodyPartOverlap(targetProfile.ConditionScopes, existingProfile.ConditionScopes))
            {
                throw new InvalidOperationException(
                    $"RULE_CONFLICT: 项目 {targetHeader.ItemCode} 的换算规则部位范围重叠，" +
                    $"RuleId={existingRule.RuleId}");
            }
        }
    }

    private async Task<RuleConflictProfile> BuildRuleProfileAsync(RuleHeader header, int versionNo)
    {
        var conditions = await _conditionRepository.GetByRuleAndVersionAsync(header.RuleId, versionNo);
        var actions = await _actionRepository.GetByRuleAndVersionAsync(header.RuleId, versionNo);

        return new RuleConflictProfile(
            BuildConditionScopes(conditions),
            actions
                .Where(a => a.IsEnabled == "Y")
                .Select(a => a.ActionType)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToHashSet(StringComparer.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<RuleConditionScope> BuildConditionScopes(
        IReadOnlyList<RuleCondition> conditions)
    {
        var enabled = conditions
            .Where(c => c.IsEnabled == "Y")
            .ToList();
        if (enabled.Count == 0)
        {
            return new[] { RuleConditionScope.Wildcard };
        }

        return enabled
            .GroupBy(c => string.IsNullOrWhiteSpace(c.ConditionGroup)
                ? "DEFAULT"
                : c.ConditionGroup.Trim())
            .Select(group => new RuleConditionScope(
                GetConditionValues(group, "CHARGE_SCENE", "CHARGE_SCENE_MATCH"),
                GetConditionValues(group, "BODY_PART", "BODY_PART_MATCH")))
            .ToList();
    }

    private static HashSet<string> GetConditionValues(
        IEnumerable<RuleCondition> conditions,
        params string[] conditionTypes)
    {
        var conditionTypeSet = conditionTypes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return conditions
            .Where(c => conditionTypeSet.Contains(c.ConditionType))
            .Select(c => c.RightValue?.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsEffectiveRangeOverlap(RuleHeader left, RuleHeader right)
    {
        var leftFrom = left.EffectiveFrom ?? DateTime.MinValue;
        var leftTo = left.EffectiveTo ?? DateTime.MaxValue;
        var rightFrom = right.EffectiveFrom ?? DateTime.MinValue;
        var rightTo = right.EffectiveTo ?? DateTime.MaxValue;
        return leftFrom <= rightTo && rightFrom <= leftTo;
    }

    private static bool HasSceneOverlap(
        IReadOnlyList<RuleConditionScope> left,
        IReadOnlyList<RuleConditionScope> right)
    {
        return left.Any(l => right.Any(r => IsDimensionOverlap(l.ChargeScenes, r.ChargeScenes)));
    }

    private static bool HasSceneAndBodyPartOverlap(
        IReadOnlyList<RuleConditionScope> left,
        IReadOnlyList<RuleConditionScope> right)
    {
        return left.Any(l => right.Any(r =>
            IsDimensionOverlap(l.ChargeScenes, r.ChargeScenes) &&
            IsDimensionOverlap(l.BodyParts, r.BodyParts)));
    }

    private static bool IsDimensionOverlap(HashSet<string> left, HashSet<string> right)
    {
        return left.Count == 0 || right.Count == 0 || left.Overlaps(right);
    }

    /// <summary>
    /// 检查两组动作类型之间是否存在互斥冲突。
    ///
    /// 互斥动作类型从 PR_DICT 字典表读取（DICT_TYPE = "MUTUALLY_EXCLUSIVE_ACTION_TYPE"），
    /// 首次调用时加载并缓存，后续使用缓存值。
    ///
    /// 互斥含义：同一项目在相同场景和生效期内，不允许同时存在某些动作类型的规则。
    /// 例如：不允许同一项目同时存在两条折价公式规则（FORMULA_CALC 互斥）。
    /// </summary>
    /// <param name="left">待发布规则的动作类型集合。</param>
    /// <param name="right">已发布规则的动作类型集合。</param>
    /// <param name="actionType">输出参数，返回第一个冲突的动作类型编码。</param>
    /// <returns>存在互斥冲突返回 <c>true</c>，无冲突返回 <c>false</c>。</returns>
    /// <remarks>
    /// 设计变更说明：
    ///   原实现硬编码 6 个互斥动作类型字符串，新增动作类型必须修改代码。
    ///   新实现从 PR_DICT 表的 MUTUALLY_EXCLUSIVE_ACTION_TYPE 类型中读取互斥列表，
    ///   配置人员可通过字典维护界面管理互斥规则，无需修改代码。
    ///
    /// 向后兼容：
    ///   字典数据未加载或为空时，使用默认互斥列表（DefaultMutuallyExclusiveActions），
    ///   确保系统在种子数据未初始化时仍能正常进行互斥校验。
    /// </remarks>
    private async Task<(bool HasConflict, string ActionType)> HasForbiddenActionConflictAsync(
        HashSet<string> left,
        HashSet<string> right)
    {
        var forbiddenActions = await GetMutuallyExclusiveActionsAsync();

        var actionType = forbiddenActions.FirstOrDefault(a => left.Contains(a) && right.Contains(a)) ?? string.Empty;
        return (!string.IsNullOrEmpty(actionType), actionType);
    }

    /// <summary>
    /// 获取互斥动作类型集合。
    ///
    /// 从 PR_DICT 字典表读取 DICT_TYPE = "MUTUALLY_EXCLUSIVE_ACTION_TYPE" 的所有启用字典项，
    /// 返回其 DICT_CODE 集合。首次调用时加载并缓存，后续使用缓存值。
    /// </summary>
    /// <returns>互斥动作类型编码集合（不区分大小写）。</returns>
    /// <remarks>
    /// <para>
    /// 默认互斥列表（字典未加载时使用）：
    /// <list type="bullet">
    ///   <item><description>FORMULA_CALC — 折价公式：同一项目不能有多套折价公式</description></item>
    ///   <item><description>APPLY_MIN_AMOUNT — 金额下限：同一项目不能有多套下限规则</description></item>
    ///   <item><description>APPLY_MAX_AMOUNT — 金额上限：同一项目不能有多套上限规则</description></item>
    ///   <item><description>APPLY_DAY_LIMIT_QTY — 日数量限制：同一项目不能有多套日限规则</description></item>
    ///   <item><description>APPLY_ONCE_LIMIT_QTY — 单次数量限制：同一项目不能有多套单次限规则</description></item>
    ///   <item><description>APPLY_TIME_WINDOW_LIMIT — 时间窗限制：同一项目不能有多套时间窗规则</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// 注意：CONVERT_QTY（换算规则）允许按不同部位维护不同规则，因此不在互斥列表中。
    /// </para>
    /// </remarks>
    private async Task<HashSet<string>> GetMutuallyExclusiveActionsAsync()
    {
        // 快速路径：缓存已加载，直接返回。
        if (_mutuallyExclusiveActionsCache is not null)
        {
            return _mutuallyExclusiveActionsCache;
        }

        await _cacheLock.WaitAsync();
        try
        {
            // 双重检查：进入锁之后再次判断，避免并发场景下重复加载。
            if (_mutuallyExclusiveActionsCache is not null)
            {
                return _mutuallyExclusiveActionsCache;
            }

            return await LoadMutuallyExclusiveActionsAsync();
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    /// <summary>
    /// 从 PR_DICT 字典表加载互斥动作类型到缓存。
    ///
    /// 读取 DICT_TYPE = "MUTUALLY_EXCLUSIVE_ACTION_TYPE" 的所有启用字典项，
    /// 以 DICT_CODE 为动作类型编码构建 HashSet。
    ///
    /// 加载失败时使用默认互斥列表，记录错误日志但不抛出异常，
    /// 确保字典表不可用时发布校验仍能正常工作。
    /// </summary>
    private async Task<HashSet<string>> LoadMutuallyExclusiveActionsAsync()
    {
        try
        {
            var dictItems = await _dictRepository.GetByTypeAsync(MutuallyExclusiveActionTypeDictType);

            if (dictItems.Count == 0)
            {
                // 字典表中没有互斥动作类型数据，使用默认列表。
                _logger.LogWarning(
                    "PR_DICT 中未找到 DICT_TYPE = {DictType} 的字典项，使用默认互斥动作类型列表",
                    MutuallyExclusiveActionTypeDictType);
                _mutuallyExclusiveActionsCache = new HashSet<string>(
                    DefaultMutuallyExclusiveActions,
                    StringComparer.OrdinalIgnoreCase);
                return _mutuallyExclusiveActionsCache;
            }

            var result = new HashSet<string>(
                DefaultMutuallyExclusiveActions,
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in dictItems.Where(d => d.IsEnabled == "Y"))
            {
                result.Add(item.DictCode);
            }

            _mutuallyExclusiveActionsCache = result;

            _logger.LogInformation(
                "已从 PR_DICT 加载互斥动作类型，共 {Count} 个",
                result.Count);

            return result;
        }
        catch (Exception ex)
        {
            // 加载失败时使用默认互斥列表，记录错误日志但不抛出异常。
            _logger.LogError(ex,
                "从 PR_DICT 加载互斥动作类型失败，使用默认互斥列表");
            _mutuallyExclusiveActionsCache = new HashSet<string>(
                DefaultMutuallyExclusiveActions,
                StringComparer.OrdinalIgnoreCase);
            return _mutuallyExclusiveActionsCache;
        }
    }

    /// <summary>
    /// 默认互斥动作类型列表。在字典数据尚未加载时使用，确保向后兼容。
    /// </summary>
    /// <remarks>
    /// 与原硬编码 HasForbiddenActionConflict 方法中的 forbiddenActions 数组保持一致。
    /// CONVER_TQTY（换算规则）允许按不同部位维护不同规则，因此不在互斥列表中。
    /// </remarks>
    private static readonly HashSet<string> DefaultMutuallyExclusiveActions = new(StringComparer.OrdinalIgnoreCase)
    {
        "FORMULA_CALC",
        "APPLY_MIN_AMOUNT",
        "APPLY_MAX_AMOUNT",
        "APPLY_DAY_LIMIT_QTY",
        "APPLY_ONCE_LIMIT_QTY",
        "APPLY_TIME_WINDOW_LIMIT"
    };

    private sealed record RuleConflictProfile(
        IReadOnlyList<RuleConditionScope> ConditionScopes,
        HashSet<string> Actions);

    private sealed record RuleConditionScope(
        HashSet<string> ChargeScenes,
        HashSet<string> BodyParts)
    {
        public static RuleConditionScope Wildcard { get; } = new(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            new HashSet<string>(StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 清除生效规则缓存和互斥动作类型缓存。
    /// </summary>
    /// <remarks>
    /// 发布、停用、回滚操作后必须调用，确保计价引擎在下一次请求时读到最新规则集。
    /// IMemoryCache 不提供按前缀批量删除，因此移除已知 key。
    ///
    /// 同时清除互斥动作类型缓存，因为字典数据可能在发布周期内被修改。
    /// </remarks>
    private void ClearEffectiveCache()
    {
        var removed = EffectiveRuleCacheKeys.Clear(_cache);
        _mutuallyExclusiveActionsCache = null;
        _logger.LogDebug("已清除生效规则缓存和互斥动作类型缓存，共清理 {Count} 个生效规则缓存键", removed);
    }

    /// <summary>
    /// 将发布流水实体映射为接口响应。
    /// </summary>
    /// <param name="entity">发布流水实体。</param>
    /// <returns>发布流水响应 DTO。</returns>
    private static RulePublishResponse MapPublishToResponse(RulePublish entity)
    {
        return new RulePublishResponse
        {
            PublishId = entity.PublishId,
            PublishNo = entity.PublishNo,
            RuleId = entity.RuleId,
            FromVersion = entity.FromVersion,
            ToVersion = entity.ToVersion,
            ActionType = entity.ActionType,
            PublishedBy = entity.PublishedBy,
            PublishedAt = entity.PublishedAt,
            Remark = entity.Remark
        };
    }

    /// <summary>
    /// 将规则变更日志实体映射为接口响应。
    /// </summary>
    /// <param name="entity">规则变更日志实体。</param>
    /// <returns>规则变更日志响应 DTO。</returns>
    private static RuleChangeLogResponse MapChangeLogToResponse(RuleChangeLog entity)
    {
        return new RuleChangeLogResponse
        {
            ChangeId = entity.ChangeId,
            RuleId = entity.RuleId,
            VersionNo = entity.VersionNo,
            ChangeType = entity.ChangeType,
            ChangeSummary = entity.ChangeSummary,
            ChangedBy = entity.ChangedBy,
            ChangedAt = entity.ChangedAt,
            SourceSystem = entity.SourceSystem
        };
    }
}
