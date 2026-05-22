using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Api.Application.Background;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Aggregates.Rules;
using Newtonsoft.Json.Linq;

namespace Pricing.RuleCenter.Api.Application.Rules;

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
public sealed class RulePublishAppService
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
    /// 规则测试用例仓储，用于发布前校验是否存在启用用例。
    /// </summary>
    private readonly IRuleTestCaseRepository _testCaseRepository;
    /// <summary>
    /// 规则测试运行仓储，用于发布前校验启用用例的最新运行是否全部通过。
    /// </summary>
    private readonly IRuleTestRunRepository _testRunRepository;
    /// <summary>
    /// 内存缓存，用于在发布/停用/回滚后立即清除生效规则缓存。
    /// </summary>
    private readonly IMemoryCache _cache;
    /// <summary>
    /// 服务日志，用于记录状态机入口的关键操作。
    /// </summary>
    private readonly ILogger<RulePublishAppService> _logger;
    /// <summary>
    /// 工作单元，用于在发布操作中开启事务保证多表状态变更的原子性。
    /// 发布操作会同时更新版本状态、主档状态、发布流水和变更日志，必须使用事务包裹。
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;
    /// <summary>
    /// 规则运行期缓存失效器，用于清除计价引擎侧跨请求共享缓存。
    /// </summary>
    private readonly IRuleRuntimeCacheInvalidator _runtimeCacheInvalidator;
    /// <summary>
    /// 跨实例缓存版本同步器，用于在发布状态变更后递增共享缓存版本。
    /// </summary>
    private readonly ICacheVersionSynchronizer _cacheVersionSynchronizer;

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
    /// <param name="lifecycleRepositories">规则发布生命周期仓储集合。</param>
    /// <param name="definitionRepositories">规则定义仓储集合，用于发布前冲突校验。</param>
    /// <param name="cache">内存缓存，用于在状态变更后清除生效规则缓存。</param>
    /// <param name="unitOfWork">工作单元，用于事务支持。</param>
    /// <param name="runtimeCacheInvalidator">运行期缓存失效器，用于清除规则匹配服务的动作顺序缓存。</param>
    /// <param name="logger">日志对象。</param>
    public RulePublishAppService(
        RulePublishLifecycleRepositories lifecycleRepositories,
        RulePublishDefinitionRepositories definitionRepositories,
        IMemoryCache cache,
        IUnitOfWork unitOfWork,
        ICacheVersionSynchronizer cacheVersionSynchronizer,
        IRuleRuntimeCacheInvalidator runtimeCacheInvalidator,
        ILogger<RulePublishAppService> logger)
    {
        _headerRepository = lifecycleRepositories.HeaderRepository;
        _versionRepository = lifecycleRepositories.VersionRepository;
        _publishRepository = lifecycleRepositories.PublishRepository;
        _changeLogRepository = lifecycleRepositories.ChangeLogRepository;
        _conditionRepository = definitionRepositories.ConditionRepository;
        _actionRepository = definitionRepositories.ActionRepository;
        _dictRepository = definitionRepositories.DictRepository;
        _testCaseRepository = definitionRepositories.TestCaseRepository;
        _testRunRepository = definitionRepositories.TestRunRepository;
        _cache = cache;
        _unitOfWork = unitOfWork;
        _cacheVersionSynchronizer = cacheVersionSynchronizer;
        _runtimeCacheInvalidator = runtimeCacheInvalidator;
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
    /// <remarks>
    /// 【事务保护】发布操作涉及多表状态变更（版本状态、主档状态、发布流水、变更日志），
    /// 必须在同一事务中执行，确保要么全部成功要么全部回滚。否则可能出现：
    /// - 版本状态已更新但主档未更新 → 计价引擎匹配到不一致的规则
    /// - 发布流水缺失 → 无法追溯发布历史
    /// </remarks>
    public async Task PublishAsync(long ruleId, RulePublishRequest request)
    {
        // ========== 第一阶段：读取主档和目标版本（事务外） ==========
        // 发布必须同时拿到主档和版本。主档提供当前版本号，版本表提供目标版本的状态边界。
        var header = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");

        var version = await _versionRepository.GetByRuleAndVersionAsync(ruleId, request.VersionNo)
            ?? throw new BizException(
                BizErrorCode.RuleVersionNotFound,
                404,
                $"规则版本不存在: RuleId={ruleId}, VersionNo={request.VersionNo}");

        // ========== 第二阶段：校验目标版本必须是草稿（事务外） ==========
        // 只允许 DRAFT -> PUBLISHED。已经发布、禁用或回滚过的版本不能再次走普通发布入口，
        // 避免发布历史出现同一个版本被多次发布的歧义。
        if (version.VersionStatus != VersionStatusCodes.Draft)
        {
            throw new BizException(
                BizErrorCode.VersionStatusNotAllowed,
                409,
                $"只有草稿版本可以发布, 当前状态: {version.VersionStatus}");
        }

        await ValidatePublishConflictsAsync(header, request.VersionNo);

        // ========== 第三阶段：事务内执行状态变更和流水写入 ==========
        // 版本状态、主档状态、发布流水、变更日志必须在同一事务中更新。
        // 如果任何一步失败，整体回滚，避免数据不一致。
        await ExecuteInTransactionAsync(async () =>
        {
            var currentHeader = await _headerRepository.GetByIdForUpdateAsync(ruleId)
                ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");
            var currentVersion = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, request.VersionNo)
                ?? throw new BizException(
                    BizErrorCode.RuleVersionNotFound,
                    404,
                    $"规则版本不存在: RuleId={ruleId}, VersionNo={request.VersionNo}");
            if (currentVersion.VersionStatus != VersionStatusCodes.Draft)
            {
                throw new BizException(
                    BizErrorCode.VersionStatusNotAllowed,
                    409,
                    $"只有草稿版本可以发布, 当前状态: {currentVersion.VersionStatus}");
            }

            var oldVersion = currentHeader.CurrentVersion;
            var previousHeaderStatus = currentHeader.Status;

            // 禁用旧生效版本
            if (oldVersion > 0)
            {
                var oldVersionEntity = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, oldVersion);
                if (oldVersionEntity is null)
                {
                    _logger.LogWarning("发布规则时未找到旧版本记录 RuleId={RuleId}, OldVersion={OldVersion}", ruleId, oldVersion);
                }
                else
                {
                    if (string.Equals(oldVersionEntity.VersionStatus, VersionStatusCodes.Published, StringComparison.OrdinalIgnoreCase))
                    {
                        var disableOldVersionUpdated = await _versionRepository.UpdateStatusAsync(
                            oldVersionEntity.VersionId,
                            VersionStatusCodes.Disabled,
                            VersionStatusCodes.Published);
                        if (!disableOldVersionUpdated)
                        {
                            throw new BizException(
                                BizErrorCode.RuleVersionConcurrencyConflict,
                                409,
                                $"RuleId={ruleId}, OldVersionNo={oldVersion} 状态已变化，请刷新后重试");
                        }
                    }
                }
            }

            // 发布目标版本并推进主档
            var publishVersionUpdated = await _versionRepository.UpdateStatusAsync(
                currentVersion.VersionId,
                VersionStatusCodes.Published,
                VersionStatusCodes.Draft);
            if (!publishVersionUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleVersionConcurrencyConflict,
                    409,
                    $"RuleId={ruleId}, VersionNo={request.VersionNo} 状态已变化，请刷新后重试");
            }

            currentHeader.CurrentVersion = request.VersionNo;
            currentHeader.Status = RuleStatusCodes.Published;
            // 规则从停用态重新发布时，必须同步恢复启用标志；
            // 否则 GetEffective/GetSpecialFlag 仍会把它排除，形成“已发布但不生效”的假发布。
            currentHeader.IsEnabled = EnableFlag.Yes;
            currentHeader.UpdatedAt = DateTime.Now;
            var publishHeaderUpdated = await _headerRepository.UpdateAsync(
                currentHeader,
                previousHeaderStatus);
            if (!publishHeaderUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleHeaderConcurrencyConflict,
                    409,
                    $"RuleId={ruleId} 主档状态已变化，请刷新后重试");
            }

            // 写发布流水
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

            // 写变更摘要
            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = ruleId,
                VersionNo = request.VersionNo,
                ChangeType = "PUBLISH",
                ChangeSummary = $"发布版本 V{request.VersionNo}",
                ChangedBy = request.PublishedBy,
                ChangedAt = DateTime.Now
            });
        });

        // ========== 第四阶段：清除生效规则缓存（事务外） ==========
        // 发布改变了当前生效版本，必须立即清除缓存，确保计价引擎读到最新规则集。
        ClearEffectiveCache();
        await _cacheVersionSynchronizer.IncreaseVersionAsync(CacheVersionSynchronizer.EffectiveRulesScope);
        await _cacheVersionSynchronizer.IncreaseVersionAsync(CacheVersionSynchronizer.ActionTypeOrderScope);

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
            ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");

        if (header.Status != RuleStatusCodes.Published)
        {
            throw new BizException(
                BizErrorCode.RuleAlreadyDisabled,
                409,
                $"只有已发布的规则可以停用, 当前状态: {header.Status}");
        }

        // ========== 第二阶段：事务内执行状态变更和流水写入 ==========
        await ExecuteInTransactionAsync(async () =>
        {
            var currentHeader = await _headerRepository.GetByIdForUpdateAsync(ruleId)
                ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");
            if (currentHeader.Status != RuleStatusCodes.Published)
            {
                throw new BizException(
                    BizErrorCode.RuleAlreadyDisabled,
                    409,
                    $"只有已发布的规则可以停用, 当前状态: {currentHeader.Status}");
            }

            // 禁用当前版本
            if (currentHeader.CurrentVersion > 0)
            {
                var currentVersion = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, currentHeader.CurrentVersion);
                if (currentVersion is not null)
                {
                    var disableCurrentVersionUpdated = await _versionRepository.UpdateStatusAsync(
                        currentVersion.VersionId,
                        VersionStatusCodes.Disabled,
                        VersionStatusCodes.Published);
                    if (!disableCurrentVersionUpdated)
                    {
                        throw new BizException(
                            BizErrorCode.RuleVersionConcurrencyConflict,
                            409,
                            $"RuleId={ruleId}, CurrentVersionNo={currentHeader.CurrentVersion} 状态已变化，请刷新后重试");
                    }
                }
            }

            // 更新主档启用状态
            currentHeader.Status = RuleStatusCodes.Disabled;
            currentHeader.IsEnabled = EnableFlag.No;
            currentHeader.UpdatedAt = DateTime.Now;
            var disableHeaderUpdated = await _headerRepository.UpdateAsync(
                currentHeader,
                RuleStatusCodes.Published);
            if (!disableHeaderUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleHeaderConcurrencyConflict,
                    409,
                    $"RuleId={ruleId} 主档状态已变化，请刷新后重试");
            }

            // 写停用流水和变更日志
            await _publishRepository.InsertAsync(new RulePublish
            {
                PublishNo = $"DIS-{ruleId}-{DateTime.Now:yyyyMMddHHmmss}",
                RuleId = ruleId,
                FromVersion = currentHeader.CurrentVersion,
                ToVersion = currentHeader.CurrentVersion,
                ActionType = "DISABLE",
                PublishedBy = request.PublishedBy,
                PublishedAt = DateTime.Now,
                Remark = request.Remark
            });

            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = ruleId,
                VersionNo = currentHeader.CurrentVersion,
                ChangeType = "DISABLE",
                ChangeSummary = $"停用规则, 当前版本 V{currentHeader.CurrentVersion}",
                ChangedBy = request.PublishedBy,
                ChangedAt = DateTime.Now
            });
        });

        // ========== 第三阶段：清除生效规则缓存 ==========
        ClearEffectiveCache();
        await _cacheVersionSynchronizer.IncreaseVersionAsync(CacheVersionSynchronizer.EffectiveRulesScope);
        await _cacheVersionSynchronizer.IncreaseVersionAsync(CacheVersionSynchronizer.ActionTypeOrderScope);

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
            ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");

        if (header.Status != RuleStatusCodes.Published)
        {
            throw new BizException(
                BizErrorCode.RollbackTargetNotAvailable,
                409,
                $"只有已发布的规则可以回滚, 当前状态: {header.Status}");
        }

        // ========== 第三阶段：事务内执行状态变更和流水写入 ==========
        await ExecuteInTransactionAsync(async () =>
        {
            var currentHeader = await _headerRepository.GetByIdForUpdateAsync(ruleId)
                ?? throw new BizException(BizErrorCode.RuleNotFound, 404, $"规则不存在: {ruleId}");
            if (currentHeader.Status != RuleStatusCodes.Published)
            {
                throw new BizException(
                    BizErrorCode.RollbackTargetNotAvailable,
                    409,
                    $"只有已发布的规则可以回滚, 当前状态: {currentHeader.Status}");
            }

            var oldVersionNo = currentHeader.CurrentVersion;
            var rollbackVersionNo = await ResolveRollbackVersionNoAsync(ruleId, oldVersionNo);

            // 把当前版本标记为已回滚
            var currentVersion = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, oldVersionNo);
            if (currentVersion is not null)
            {
                var rollbackCurrentVersionUpdated = await _versionRepository.UpdateStatusAsync(
                    currentVersion.VersionId,
                    VersionStatusCodes.RolledBack,
                    VersionStatusCodes.Published);
                if (!rollbackCurrentVersionUpdated)
                {
                    throw new BizException(
                        BizErrorCode.RuleVersionConcurrencyConflict,
                        409,
                        $"RuleId={ruleId}, CurrentVersionNo={oldVersionNo} 状态已变化，请刷新后重试");
                }
            }

            // 恢复历史版本为发布状态
            var rollbackVersion = await _versionRepository.GetByRuleAndVersionForUpdateAsync(ruleId, rollbackVersionNo)
                ?? throw new BizException(
                    BizErrorCode.RollbackTargetNotAvailable,
                    409,
                    $"回滚目标版本不存在: RuleId={ruleId}, VersionNo={rollbackVersionNo}");
            var rollbackVersionUpdated = await _versionRepository.UpdateStatusAsync(
                rollbackVersion.VersionId,
                VersionStatusCodes.Published,
                VersionStatusCodes.Disabled);
            if (!rollbackVersionUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleVersionConcurrencyConflict,
                    409,
                    $"RuleId={ruleId}, RollbackVersionNo={rollbackVersionNo} 状态已变化，请刷新后重试");
            }

            currentHeader.CurrentVersion = rollbackVersionNo;
            currentHeader.Status = RuleStatusCodes.Published;
            currentHeader.IsEnabled = EnableFlag.Yes;
            currentHeader.UpdatedAt = DateTime.Now;
            var rollbackHeaderUpdated = await _headerRepository.UpdateAsync(
                currentHeader,
                RuleStatusCodes.Published);
            if (!rollbackHeaderUpdated)
            {
                throw new BizException(
                    BizErrorCode.RuleHeaderConcurrencyConflict,
                    409,
                    $"RuleId={ruleId} 主档状态已变化，请刷新后重试");
            }

            // 记录回滚流水和变更摘要
            await _publishRepository.InsertAsync(new RulePublish
            {
                PublishNo = $"RB-{ruleId}-{rollbackVersionNo}-{DateTime.Now:yyyyMMddHHmmss}",
                RuleId = ruleId,
                FromVersion = oldVersionNo,
                ToVersion = rollbackVersionNo,
                ActionType = "ROLLBACK",
                PublishedBy = request.PublishedBy,
                PublishedAt = DateTime.Now,
                Remark = request.Remark
            });

            await _changeLogRepository.InsertAsync(new RuleChangeLog
            {
                RuleId = ruleId,
                VersionNo = rollbackVersionNo,
                ChangeType = "ROLLBACK",
                ChangeSummary = $"从 V{oldVersionNo} 回滚到 V{rollbackVersionNo}",
                ChangedBy = request.PublishedBy,
                ChangedAt = DateTime.Now
            });
        });

        // ========== 第四阶段：清除生效规则缓存 ==========
        ClearEffectiveCache();
        await _cacheVersionSynchronizer.IncreaseVersionAsync(CacheVersionSynchronizer.EffectiveRulesScope);
        await _cacheVersionSynchronizer.IncreaseVersionAsync(CacheVersionSynchronizer.ActionTypeOrderScope);

        var currentHeaderAfterRollback = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");
        _logger.LogInformation("回滚规则 RuleId={RuleId}, 从 V{FromVersion} 到 V{ToVersion}",
            ruleId, header.CurrentVersion, currentHeaderAfterRollback.CurrentVersion);
    }

    /// <summary>
    /// 解析回滚目标版本号。
    /// </summary>
    /// <remarks>
    /// 优先按发布流水追溯“当前版本之前最近一次激活的版本”，避免把一个从未发布过、但碰巧处于 DISABLED 的草稿版本误当成回滚目标。
    /// 当历史流水缺失时，再退回旧的版本状态兜底策略，兼容历史数据。
    /// </remarks>
    private async Task<int> ResolveRollbackVersionNoAsync(long ruleId, int currentVersionNo)
    {
        var publishHistory = await _publishRepository.GetByRuleIdAsync(ruleId);
        var activationHistory = publishHistory
            .Where(p => string.Equals(p.ActionType, "PUBLISH", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(p.ActionType, "ROLLBACK", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(p => p.PublishedAt)
            .ThenByDescending(p => p.PublishId)
            .ToList();

        if (activationHistory.Count > 0)
        {
            var currentActivationIndex = activationHistory.FindIndex(p => p.ToVersion == currentVersionNo);
            if (currentActivationIndex >= 0)
            {
                var previousActivation = activationHistory
                    .Skip(currentActivationIndex + 1)
                    .FirstOrDefault(p => p.ToVersion != currentVersionNo);
                if (previousActivation is not null)
                {
                    return previousActivation.ToVersion;
                }
            }
        }

        var versions = await _versionRepository.GetByRuleIdAsync(ruleId);
        var previousPublished = versions
            .Where(v => v.VersionNo < currentVersionNo && v.VersionStatus == VersionStatusCodes.Disabled)
            .OrderByDescending(v => v.VersionNo)
            .FirstOrDefault()
            ?? throw new BizException(
                BizErrorCode.RollbackTargetNotAvailable,
                409,
                "没有可回滚的历史版本");
        return previousPublished.VersionNo;
    }

    private async Task ValidatePublishConflictsAsync(RuleAggregate targetHeader, int targetVersionNo)
    {
        if (string.IsNullOrWhiteSpace(targetHeader.ItemCode))
        {
            return;
        }

        await ValidateActionParametersAsync(targetHeader.RuleId, targetVersionNo);
        await ValidateEnabledTestCasesAsync(targetHeader.RuleId, targetVersionNo);

        var targetProfile = await BuildRuleProfileAsync(targetHeader, targetVersionNo);
        var sameItemRules = await _headerRepository.GetByItemCodeAsync(targetHeader.ItemCode);
        var publishedRules = sameItemRules
            .Where(r => r.RuleId != targetHeader.RuleId)
            .Where(r => r.Status == RuleStatusCodes.Published && r.IsEnabled == EnableFlag.Yes)
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
                throw new BizException(
                    BizErrorCode.RuleOverlapConflict,
                    409,
                    $"项目 {targetHeader.ItemCode} 在相同场景和生效期内已存在 {conflictActionType} 规则，RuleId={existingRule.RuleId}");
            }

            if (targetProfile.Actions.Contains("CONVERT_QTY") &&
                existingProfile.Actions.Contains("CONVERT_QTY") &&
                HasSceneAndBodyPartOverlap(targetProfile.ConditionScopes, existingProfile.ConditionScopes))
            {
                throw new BizException(
                    BizErrorCode.RuleOverlapConflict,
                    409,
                    $"项目 {targetHeader.ItemCode} 的换算规则部位范围重叠，RuleId={existingRule.RuleId}");
            }
        }
    }

    private async Task ValidateActionParametersAsync(long ruleId, int versionNo)
    {
        var actions = await _actionRepository.GetByRuleAndVersionAsync(ruleId, versionNo);
        ValidateCriticalActionOnError(actions);
        ValidateAddChildItemActions(actions);

        foreach (var action in actions.Where(a => a.IsEnabled == EnableFlag.Yes))
        {
            ValidateActionParameters(action);
        }
    }

    private async Task ValidateEnabledTestCasesAsync(long ruleId, int versionNo)
    {
        var enabledCases = (await _testCaseRepository.GetByRuleAndVersionAsync(ruleId, versionNo))
            .Where(c => string.Equals(c.IsEnabled, EnableFlag.Yes, StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (enabledCases.Count == 0)
        {
            throw new BizException(
                BizErrorCode.MissingTestCase,
                409,
                $"规则 RuleId={ruleId}, VersionNo={versionNo} 缺少启用测试用例");
        }

        foreach (var testCase in enabledCases)
        {
            if (string.IsNullOrWhiteSpace(testCase.InputJson) ||
                string.IsNullOrWhiteSpace(testCase.ExpectedJson))
            {
                throw new BizException(
                    BizErrorCode.TestCaseIncomplete,
                    409,
                    $"TestCaseId={testCase.TestCaseId} 缺少 InputJson 或 ExpectedJson");
            }

            var latestRun = (await _testRunRepository.GetByTestCaseIdAsync(testCase.TestCaseId))
                .OrderByDescending(r => r.RunAt)
                .ThenByDescending(r => r.TestRunId)
                .FirstOrDefault();
            if (latestRun is null)
            {
                throw new BizException(
                    BizErrorCode.TestRunMissing,
                    409,
                    $"TestCaseId={testCase.TestCaseId} 尚未执行测试");
            }

            if (!string.Equals(latestRun.IsPass, EnableFlag.Yes, StringComparison.OrdinalIgnoreCase))
            {
                throw new BizException(
                    BizErrorCode.TestRunFailed,
                    409,
                    $"TestCaseId={testCase.TestCaseId} 最新测试未通过");
            }
        }
    }

    private static void ValidateCriticalActionOnError(IReadOnlyList<RuleAction> actions)
    {
        foreach (var action in actions.Where(a => a.IsEnabled == EnableFlag.Yes))
        {
            var actionType = NormalizeActionType(action.ActionType);
            if (!CriticalActionTypes.Contains(actionType))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(action.OnError) ||
                string.Equals(action.OnError, "STOP", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            throw new BizException(
                BizErrorCode.ActionOnErrorInvalid,
                409,
                $"ActionType={action.ActionType} 的 OnError 必须为 STOP");
        }
    }

    private static void ValidateAddChildItemActions(IReadOnlyList<RuleAction> actions)
    {
        var normalizedChildCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var action in actions.Where(a =>
                     a.IsEnabled == EnableFlag.Yes &&
                     string.Equals(NormalizeActionType(a.ActionType), "ADD_CHILD_ITEM", StringComparison.OrdinalIgnoreCase)))
        {
            var config = ParseAddChildItemConfig(action.ParamsJson);
            if (config?.ChildItems is null || config.ChildItems.Count == 0)
            {
                continue;
            }

            foreach (var child in config.ChildItems)
            {
                var normalizedItemCode = NormalizeChildItemCode(child.ItemCode);
                if (normalizedItemCode is null)
                {
                    throw new BizException(
                        BizErrorCode.ChildItemInvalid,
                        409,
                        "ADD_CHILD_ITEM 的 childItems[].itemCode 不能为空");
                }

                if (!normalizedChildCodes.Add(normalizedItemCode))
                {
                    throw new BizException(
                        BizErrorCode.ChildItemDuplicate,
                        409,
                        $"ADD_CHILD_ITEM 重复引用子项目 {normalizedItemCode}");
                }
            }
        }
    }

    private static void ValidateActionParameters(RuleAction action)
    {
        var json = ParseParams(action.ParamsJson);
        switch (action.ActionType?.Trim().ToUpperInvariant())
        {
            case "APPLY_TIME_WINDOW_LIMIT":
                if (!HasPositiveNumber(json, "limitQty", "maxQty") ||
                    !HasPositiveNumber(json, "windowMinutes", "windowHours"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 limitQty/maxQty 或 windowMinutes/windowHours");
                }
                break;

            case "APPLY_DAY_LIMIT_QTY":
                if (!HasPositiveNumber(json, "maxDailyQty"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 maxDailyQty");
                }
                break;

            case "APPLY_ONCE_LIMIT_QTY":
                if (!HasNonNegativeNumber(json, "maxOnceQty", "maxQty"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 maxOnceQty/maxQty");
                }
                break;

            case "APPLY_MAX_AMOUNT":
                if (!HasNonNegativeNumber(json, "maxAmount", "ceilingAmount"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 maxAmount/ceilingAmount");
                }
                break;

            case "APPLY_MIN_AMOUNT":
                if (!HasNonNegativeNumber(json, "minAmount", "floorAmount"))
                {
                    throw new BizException(
                        BizErrorCode.ActionParamMissing,
                        409,
                        $"ActionType={action.ActionType} 缺少有效的 minAmount/floorAmount");
                }
                break;

        }
    }

    private static JObject? ParseParams(string? paramsJson)
    {
        if (string.IsNullOrWhiteSpace(paramsJson))
        {
            return null;
        }

        try
        {
            return JObject.Parse(paramsJson);
        }
        catch
        {
            throw new BizException(
                BizErrorCode.ActionParamInvalid,
                409,
                "动作参数不是合法 JSON");
        }
    }

    private static AddChildItemConfig? ParseAddChildItemConfig(string? paramsJson)
    {
        if (string.IsNullOrWhiteSpace(paramsJson))
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<AddChildItemConfig>(paramsJson);
        }
        catch
        {
            throw new BizException(
                BizErrorCode.ActionParamInvalid,
                409,
                "ADD_CHILD_ITEM 动作参数不是合法 JSON");
        }
    }

    private static string NormalizeActionType(string? actionType) =>
        actionType?.Trim().ToUpperInvariant() ?? string.Empty;

    private static string? NormalizeChildItemCode(string? itemCode)
    {
        var normalized = itemCode?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized.ToUpperInvariant();
    }

    private static bool HasPositiveNumber(JObject? json, params string[] keys)
    {
        return TryGetNumber(json, keys, out var value) && value > 0m;
    }

    private static bool HasNonNegativeNumber(JObject? json, params string[] keys)
    {
        return TryGetNumber(json, keys, out var value) && value >= 0m;
    }

    private static bool TryGetNumber(JObject? json, string[] keys, out decimal value)
    {
        value = 0m;
        if (json is null)
        {
            return false;
        }

        foreach (var key in keys)
        {
            if (json.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var token) &&
                token.Type != JTokenType.Null &&
                decimal.TryParse(token.ToString(), out value))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<RuleConflictProfile> BuildRuleProfileAsync(RuleAggregate header, int versionNo)
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

    private static bool IsEffectiveRangeOverlap(RuleAggregate left, RuleAggregate right)
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

    private sealed class AddChildItemConfig
    {
        public List<AddChildItemChildConfig>? ChildItems { get; set; }
    }

    private sealed class AddChildItemChildConfig
    {
        public string? ItemCode { get; set; }
    }

    private sealed record RuleConditionScope(
        HashSet<string> ChargeScenes,
        HashSet<string> BodyParts)
    {
        /// <summary>
        /// 表示未配置收费场景或部位约束的通配作用域。
        /// </summary>
        /// <remarks>
        /// 冲突校验中，空集合不是"无效规则"，而是"适用于所有场景/部位"。因此该实例用于表达旧规则或兜底规则
        /// 覆盖全部收费入口，避免发布一条看似更细但实际会与通配规则重叠的新规则。
        /// </remarks>
        public static RuleConditionScope Wildcard { get; } = new(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            new HashSet<string>(StringComparer.OrdinalIgnoreCase));
    }

    private static readonly HashSet<string> CriticalActionTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "CONVERT_QTY",
        "FORMULA_CALC",
        "APPLY_DAY_LIMIT_QTY",
        "APPLY_TIME_WINDOW_LIMIT",
        "APPLY_ONCE_LIMIT_QTY",
        "SAME_GROUP_MUTEX",
        "APPLY_MIN_AMOUNT",
        "APPLY_MAX_AMOUNT",
        "SAME_OPERATION_CEILING",
        "ADD_CHILD_ITEM",
        "DISCOUNT_EXCEED_TO_ZERO"
    };

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
        _runtimeCacheInvalidator.ClearRuntimeCache();
        _logger.LogDebug("已清除生效规则缓存、互斥动作类型缓存和规则运行期缓存，共清理 {Count} 个生效规则缓存键", removed);
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

    /// <summary>
    /// 在数据库事务中执行操作，确保多表状态变更的原子性。
    /// </summary>
    /// <param name="action">要执行的操作。</param>
    /// <remarks>
    /// 【事务边界】发布、停用、回滚操作会同时更新多张表（版本、主档、发布流水、变更日志），
    /// 必须在同一事务中执行，确保要么全部成功要么全部回滚。
    /// 【异常处理】任何异常都会触发回滚并向上抛出，由调用方决定如何处理。
    /// </remarks>
    private async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        try
        {
            await _unitOfWork.BeginAsync();
            await action();
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "规则发布事务执行异常，已回滚");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}


