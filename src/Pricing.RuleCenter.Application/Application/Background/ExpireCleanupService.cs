using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Application.Pricing;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Interfaces.Charging;
using Pricing.RuleCenter.Core.Interfaces.Quota;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Application.Background;

/// <summary>
/// confirm 过期清理后台服务，定时扫描并过期超时的 CONFIRM_PENDING 记录。
/// </summary>
/// <remarks>
/// <para>
/// 职责边界：该后台服务负责清理长时间停留在 CONFIRM_PENDING 的确认结果。
/// 它的职责不是"批量改状态"这么简单，而是把请求日志、折价明细和限额占用三张资金表
/// 一起推进到 EXPIRED，释放 confirm 阶段的保护占用。
/// </para>
/// <para>
/// 资金安全意义：如果 HIS 或渠道异常退出（如网络中断、进程崩溃），
/// confirm 占用的额度会永久压住患者后续收费额度。过期清理是兜底释放机制，
/// 确保 PENDING 状态不会无限期占用额度。
/// </para>
/// <para>
/// 并发安全：每条记录在过期前重新读取当前状态，确认仍然是 CONFIRM_PENDING 才推进。
/// 这避免了与 HIS commit/cancel 操作的竞态——如果记录已被 commit 或 cancel，
/// 清理任务会跳过该记录，不会把已推进的状态覆盖成 EXPIRED。
/// </para>
/// <para>
/// 事务策略：每条记录使用独立事务，单条失败不影响其他记录。
/// 三张资金表在同一事务中同步更新，保证状态一致性。
/// </para>
/// </remarks>
public sealed class ExpireCleanupAppService : BackgroundService
{
    /// <summary>
    /// 服务作用域工厂，用于在后台单例服务中创建 scoped 仓储和数据库客户端。
    /// BackgroundService 本身是 Singleton 生命周期，而仓储和 SqlSugarClient 是 Scoped，
    /// 因此每轮清理必须通过 ScopeFactory 创建独立的作用域。
    /// </summary>
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// 计价中心配置对象，集中承载超时时间（ConfirmExpireMinutes）、
    /// 清理间隔（ExpireCleanupIntervalSeconds）等运行参数。
    /// </summary>
    private readonly PricingOptions _options;

    /// <summary>
    /// 清理任务日志，用于记录过期处理结果和异常。
    /// 异常日志不能吞掉，因为过期清理是资金安全的兜底机制，异常需要人工介入。
    /// </summary>
    private readonly ILogger<ExpireCleanupAppService> _logger;

    /// <summary>
    /// 初始化 confirm 过期清理后台服务。
    /// </summary>
    /// <param name="scopeFactory">服务作用域工厂，用于创建 scoped 的仓储和数据库上下文。</param>
    /// <param name="options">计价中心配置，包含 ConfirmExpireMinutes 和 ExpireCleanupIntervalSeconds。</param>
    /// <param name="logger">日志对象，用于记录过期处理结果和异常。</param>
    public ExpireCleanupAppService(
        IServiceScopeFactory scopeFactory,
        IOptions<PricingOptions> options,
        ILogger<ExpireCleanupAppService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// 后台循环入口，按配置间隔反复执行过期清理，直到应用停止。
    /// </summary>
    /// <remarks>
    /// 该方法由 ASP.NET Core 的 BackgroundService 框架在应用启动后自动调用。
    /// 循环必须响应 stoppingToken，否则应用关闭时会阻塞进程退出。
    /// 单次清理异常不会终止循环，而是记录日志后等待下一轮继续尝试。
    /// </remarks>
    /// <param name="stoppingToken">应用停止信号，由 IHostedService 生命周期管理。</param>
    /// <returns>异步任务，仅在应用停止时完成。</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ========== 第一阶段：持续运行直到应用停止 ==========
        // BackgroundService 由 ASP.NET Core 托管，循环必须响应 stoppingToken。
        // 如果忽略 stoppingToken，应用关闭时会因为后台线程未退出而延迟关闭。
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // ========== 第二阶段：执行一次清理 ==========
                // 单次清理失败不能让后台服务退出。过期清理是兜底机制，
                // 临时的数据库抖动或某条脏数据不应影响服务的持续运行。
                await CleanupExpiredAsync();
            }
            catch (Exception ex)
            {
                // 清理任务异常通常来自：数据库连接超时、锁等待超时、或某条数据状态异常。
                // 记录异常但不吞掉 stoppingToken 信号，下一轮会继续尝试。
                _logger.LogError(ex, "过期清理任务异常");
            }

            // ========== 第三阶段：按配置间隔等待 ==========
            // 间隔由 PricingOptions.ExpireCleanupIntervalSeconds 控制。
            // 间隔过短会增加数据库扫描压力（每轮都要查询 PENDING 记录）；
            // 过长会导致患者额度释放不及时，影响后续收费。
            // 传入 stoppingToken 使得应用停止时 Task.Delay 能立即抛出 OperationCanceledException。
            await Task.Delay(
                TimeSpan.FromSeconds(_options.ExpireCleanupIntervalSeconds),
                stoppingToken);
        }
    }

    /// <summary>
    /// 扫描并过期超时的 confirm 保护记录。
    /// </summary>
    /// <remarks>
    /// 该方法执行一次完整的清理扫描，包含以下步骤：
    /// 1. 创建独立的数据库作用域（与 HTTP 请求隔离）
    /// 2. 筛选出超时的 CONFIRM_PENDING 记录
    /// 3. 逐条在独立事务中推进状态，三张资金表同步更新
    /// </remarks>
    /// <returns>异步任务。</returns>
    private async Task CleanupExpiredAsync()
    {
        // ========== 第一阶段：创建独立作用域 ==========
        // 后台服务本身是 Singleton，仓储和 SqlSugarClient 是 Scoped。
        // 每轮清理必须创建独立 scope，确保数据库上下文生命周期和普通 HTTP 请求隔离。
        // 如果不创建 scope，会因为 ServiceProvider 已释放而抛出 ObjectDisposedException。
        using var scope = _scopeFactory.CreateScope();
        var requestLogRepo = scope.ServiceProvider.GetRequiredService<IChargeRequestLogRepository>();
        var discountRepo = scope.ServiceProvider.GetRequiredService<IChargeDiscountDetailRepository>();
        var limitRepo = scope.ServiceProvider.GetRequiredService<ILimitOccupyRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();

        // ========== 第二阶段：筛选超时的 CONFIRM_PENDING ==========
        // 过期时间 = 当前时间 - ConfirmExpireMinutes，与 confirm 接口返回给调用方的有效期口径一致。
        // GetPendingExpiredAsync 在仓储层实现筛选逻辑，只返回 BusinessStatus=BusinessStatusCodes.ConfirmPending
        // 且 RequestAt 早于 expireBefore 的记录。
        var expireBefore = clock.Now.AddMinutes(-_options.ConfirmExpireMinutes);
        var expired = await requestLogRepo.GetPendingExpiredAsync(expireBefore);

        // ========== 第三阶段：逐笔事务处理 ==========
        // 不使用一条 UPDATE 批量改状态，原因是：
        // 1. HIS commit/cancel 可能与清理任务并发执行，批量更新会覆盖已推进的状态
        // 2. 逐条处理允许在事务内重新读取当前状态，确认仍是 PENDING 才过期
        // 3. 单条失败不影响其他记录的清理
        foreach (var log in expired)
        {
            // 每条记录使用独立事务，保证三张资金表的原子性，同时隔离单条失败的影响。
            await unitOfWork.BeginAsync();
            try
            {
                // 重新读取当前状态，而不是依赖扫描时的快照。
                // 在扫描到处理之间的时间窗口内，HIS 可能已经 commit 或 cancel 了该记录。
                await limitRepo.EnsureAndLockAsync(new[] { PricingLockKeyBuilder.BuildRequestLockKey(log.RequestId) });
                var current = await requestLogRepo.GetByIdAsync(log.RequestId);
                if (current is null || current.BusinessStatus != BusinessStatusCodes.ConfirmPending)
                {
                    // 记录可能已经被 HIS commit/cancel，或者被上一轮清理任务处理过。
                    // 提交空事务并跳过，不要把已推进的状态覆盖成 EXPIRED。
                    // 这是并发安全的关键——只对"仍然是 PENDING"的记录执行过期。
                    await unitOfWork.CommitAsync();
                    continue;
                }

                // ========== 第四阶段：三张资金表同步过期 ==========
                // 请求日志、折价明细和限额占用必须在同一事务中一起变成 EXPIRED。
                // 如果只更新请求日志而不释放限额占用，会导致患者额度被永久占用；
                // 如果只释放限额而不更新请求日志，会导致报表和追溯数据不一致。
                current.MarkExpired(clock.Now);
                await requestLogRepo.UpdateAsync(current);

                await discountRepo.UpdateStatusByRequestIdAsync(log.RequestId, BusinessStatusCodes.Expired);
                await limitRepo.UpdateStatusByRequestIdAsync(log.RequestId, BusinessStatusCodes.Expired);

                await unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                // 回滚本条记录的变更，但不向上抛出，确保后续记录继续处理。
                // 如果 throw 会导致本轮剩余记录全部跳过，影响额度及时释放。
                await unitOfWork.RollbackAsync();
                _logger.LogError(ex, "过期清理单条失败 请求ID={RequestId}，跳过继续", log.RequestId);
                continue;
            }

            _logger.LogInformation("过期清理 请求ID={RequestId}", log.RequestId);
        }

        // 仅在有处理记录时输出汇总日志，避免无操作时产生大量空日志。
        if (expired.Count > 0)
        {
            _logger.LogInformation("过期清理完成，已处理 {Count} 条记录", expired.Count);
        }
    }
}
