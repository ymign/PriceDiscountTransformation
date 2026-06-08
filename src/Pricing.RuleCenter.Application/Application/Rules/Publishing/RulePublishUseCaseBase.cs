using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Rules;
using Pricing.RuleCenter.Application.Rules.Guards;
using Pricing.RuleCenter.Application.Background;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Rules;
using Pricing.RuleCenter.Core.Aggregates.Rules;

namespace Pricing.RuleCenter.Application.Rules.Publishing;

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
public abstract class RulePublishUseCaseBase
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
    /// 服务日志，用于记录状态机入口的关键操作。
    /// </summary>
    private readonly ILogger _logger;
    /// <summary>
    /// 规则发布生命周期事务执行器。
    /// </summary>
    private readonly RulePublishTransactionWriter _transactionWriter;
    private readonly IClock _clock;
    private readonly RuleApprovalGate _approvalGate;
    private readonly RulePublishGuard _publishGuard;
    private readonly RulePublishCacheCoordinator _cacheCoordinator;
    private readonly RuleRollbackTargetResolver _rollbackTargetResolver;
    private readonly RulePublishExecutionWorkflow _publishWorkflow;
    private readonly RuleDisableExecutionWorkflow _disableWorkflow;
    private readonly RuleRollbackExecutionWorkflow _rollbackWorkflow;

    /// <summary>
    /// 初始化规则发布服务。
    /// </summary>
    /// <param name="lifecycleRepositories">规则发布生命周期仓储集合。</param>
    /// <param name="definitionRepositories">规则定义仓储集合，用于发布前冲突校验。</param>
    /// <param name="transactionWriter">规则发布生命周期事务执行器。</param>
    /// <param name="cacheInvalidator">规则发布缓存失效器。</param>
    /// <param name="cacheInvalidationOutboxRepository">规则缓存失效 outbox 仓储。</param>
    /// <param name="cacheInvalidationOutboxProcessor">规则缓存失效 outbox 处理器。</param>
    /// <param name="clock">系统技术时间提供者。</param>
    /// <param name="approvalGate">生命周期操作审批门禁。</param>
    /// <param name="publishGuard">规则发布前聚合门禁。</param>
    /// <param name="logger">日志对象。</param>
    protected RulePublishUseCaseBase(
        RulePublishLifecycleRepositories lifecycleRepositories,
        RulePublishDefinitionRepositories definitionRepositories,
        RulePublishTransactionWriter transactionWriter,
        RulePublishCacheInvalidator cacheInvalidator,
        IRuleCacheInvalidationOutboxRepository cacheInvalidationOutboxRepository,
        RuleCacheInvalidationOutboxProcessor cacheInvalidationOutboxProcessor,
        IClock clock,
        RuleApprovalGate approvalGate,
        RulePublishGuard publishGuard,
        ILogger logger)
    {
        _headerRepository = lifecycleRepositories.HeaderRepository;
        _versionRepository = lifecycleRepositories.VersionRepository;
        _publishRepository = lifecycleRepositories.PublishRepository;
        _changeLogRepository = lifecycleRepositories.ChangeLogRepository;
        _transactionWriter = transactionWriter;
        _clock = clock;
        _approvalGate = approvalGate;
        _publishGuard = publishGuard;
        _logger = logger;
        _cacheCoordinator = new RulePublishCacheCoordinator(
            publishGuard,
            cacheInvalidator,
            cacheInvalidationOutboxRepository,
            cacheInvalidationOutboxProcessor);
        _rollbackTargetResolver = new RuleRollbackTargetResolver(
            lifecycleRepositories.PublishRepository,
            lifecycleRepositories.VersionRepository);
        _publishWorkflow = new RulePublishExecutionWorkflow(
            lifecycleRepositories.HeaderRepository,
            lifecycleRepositories.VersionRepository,
            lifecycleRepositories.PublishRepository,
            lifecycleRepositories.ChangeLogRepository,
            transactionWriter,
            _cacheCoordinator,
            clock,
            logger);
        _disableWorkflow = new RuleDisableExecutionWorkflow(
            lifecycleRepositories.HeaderRepository,
            lifecycleRepositories.VersionRepository,
            lifecycleRepositories.PublishRepository,
            lifecycleRepositories.ChangeLogRepository,
            transactionWriter,
            _cacheCoordinator,
            clock);
        _rollbackWorkflow = new RuleRollbackExecutionWorkflow(
            lifecycleRepositories.HeaderRepository,
            lifecycleRepositories.VersionRepository,
            lifecycleRepositories.PublishRepository,
            lifecycleRepositories.ChangeLogRepository,
            transactionWriter,
            _cacheCoordinator,
            _rollbackTargetResolver,
            clock);
    }

    /// <summary>
    /// 读取规则的发布历史。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>发布、停用、回滚流水列表。</returns>
    public async Task<IReadOnlyList<RulePublishResponse>> GetPublishHistoryAsync(long ruleId)
    {
        var items = await _publishRepository.GetByRuleIdAsync(ruleId);
        return items.Select(RulePublishResponseMapper.ToResponse).ToList();
    }

    /// <summary>
    /// 读取规则变更日志。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <returns>规则变更摘要列表。</returns>
    public async Task<IReadOnlyList<RuleChangeLogResponse>> GetChangeLogsAsync(long ruleId)
    {
        var items = await _changeLogRepository.GetByRuleIdAsync(ruleId);
        return items.Select(RulePublishResponseMapper.ToResponse).ToList();
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
    protected async Task ExecutePublishAsync(long ruleId, RulePublishRequest request)
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

        await _approvalGate.EnsurePassedAsync(ruleId, request.VersionNo, ApprovalActionCodes.Publish);
        await ValidatePublishConflictsAsync(header, request.VersionNo);

        await _publishWorkflow.ExecuteAsync(ruleId, request);

        // ========== 第四阶段：清除生效规则缓存（事务外） ==========
        // 发布改变了当前生效版本，必须立即清除缓存，确保计价引擎读到最新规则集。
        await _cacheCoordinator.InvalidateAfterCommitAsync();

        _logger.LogInformation("发布规则 RuleId={RuleId}, VersionNo={VersionNo}", ruleId, request.VersionNo);
    }

    /// <summary>
    /// 停用已发布规则。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">停用请求，包含操作人和备注。</param>
    /// <exception cref="KeyNotFoundException">规则不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">规则不是已发布状态时抛出。</exception>
    protected async Task ExecuteDisableAsync(long ruleId, RuleDisableRequest request)
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

        await _approvalGate.EnsurePassedAsync(ruleId, header.CurrentVersion, ApprovalActionCodes.Disable);

        await _disableWorkflow.ExecuteAsync(ruleId, request);

        // ========== 第三阶段：清除生效规则缓存 ==========
        await _cacheCoordinator.InvalidateAfterCommitAsync();

        _logger.LogInformation("停用规则 RuleId={RuleId}", ruleId);
    }

    /// <summary>
    /// 将已发布规则回滚到最近一个历史发布版本。
    /// </summary>
    /// <param name="ruleId">规则主键。</param>
    /// <param name="request">回滚请求，包含操作人和备注。</param>
    /// <exception cref="KeyNotFoundException">规则不存在时抛出。</exception>
    /// <exception cref="InvalidOperationException">规则未发布或没有可回滚版本时抛出。</exception>
    protected async Task ExecuteRollbackAsync(long ruleId, RuleRollbackRequest request)
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

        await _approvalGate.EnsurePassedAsync(ruleId, header.CurrentVersion, ApprovalActionCodes.Rollback);

        await _rollbackWorkflow.ExecuteAsync(ruleId, request);

        // ========== 第四阶段：清除生效规则缓存 ==========
        await _cacheCoordinator.InvalidateAfterCommitAsync();

        var currentHeaderAfterRollback = await _headerRepository.GetByIdAsync(ruleId)
            ?? throw new KeyNotFoundException($"规则不存在: {ruleId}");
        _logger.LogInformation("回滚规则 RuleId={RuleId}, 从 V{FromVersion} 到 V{ToVersion}",
            ruleId, header.CurrentVersion, currentHeaderAfterRollback.CurrentVersion);
    }

    private async Task ValidatePublishConflictsAsync(RuleAggregate targetHeader, int targetVersionNo)
    {
        if (string.IsNullOrWhiteSpace(targetHeader.ItemCode))
        {
            return;
        }

        await _publishGuard.EnsureCanPublishAsync(targetHeader, targetVersionNo);
    }
}
