using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;
using SqlSugar;

namespace Pricing.RuleCenter.Api.Services;

/// <summary>
/// confirm 过期清理后台服务。
/// </summary>
/// <remarks>
/// 该后台服务负责清理长时间停留在 CONFIRM_PENDING 的确认结果。它的职责不是“批量改状态”这么简单，
/// 而是把请求日志、折价明细和限额占用一起推进到 EXPIRED，释放 confirm 阶段的保护占用。
/// 这能避免 HIS 或渠道异常退出后，PENDING 占额永久压住患者后续收费额度。
/// </remarks>
public sealed class ExpireCleanupService : BackgroundService
{
    /// <summary>
    /// 服务作用域工厂，用于在后台单例服务中创建 scoped 仓储和数据库客户端。
    /// </summary>
    private readonly IServiceScopeFactory _scopeFactory;
    /// <summary>
    /// _options 配置对象，集中承载超时、清理间隔、单价校验等运行参数。
    /// </summary>
    private readonly PricingOptions _options;
    /// <summary>
    /// 清理任务日志，用于记录过期处理结果和异常。
    /// </summary>
    private readonly ILogger<ExpireCleanupService> _logger;

    /// <summary>
    /// 初始化 confirm 过期清理后台服务。
    /// </summary>
    /// <param name="scopeFactory">服务作用域工厂。</param>
    /// <param name="options">计价中心配置。</param>
    /// <param name="logger">日志对象。</param>
    public ExpireCleanupService(
        IServiceScopeFactory scopeFactory,
        IOptions<PricingOptions> options,
        ILogger<ExpireCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// 后台循环入口。
    /// </summary>
    /// <param name="stoppingToken">应用停止信号。</param>
    /// <returns>异步任务。</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ========== 第一阶段：持续运行直到应用停止 ==========
        // BackgroundService 由 ASP.NET Core 托管，循环必须响应 stoppingToken，避免应用关闭时阻塞进程退出。
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // ========== 第二阶段：执行一次清理 ==========
                // 单次清理失败不能让后台服务退出。异常会被记录，然后等待下一轮继续尝试。
                await CleanupExpiredAsync();
            }
            catch (Exception ex)
            {
                // 清理任务异常通常来自数据库连接、锁等待或某条脏数据。这里记录异常但不吞掉停止信号。
                _logger.LogError(ex, "过期清理任务异常");
            }

            // ========== 第三阶段：按配置间隔等待 ==========
            // 间隔过短会增加数据库扫描压力；过长会导致待确认额度释放不及时。
            await Task.Delay(
                TimeSpan.FromSeconds(_options.ExpireCleanupIntervalSeconds),
                stoppingToken);
        }
    }

    /// <summary>
    /// 扫描并过期超时的 confirm 保护记录。
    /// </summary>
    /// <returns>异步任务。</returns>
    private async Task CleanupExpiredAsync()
    {
        // ========== 第一阶段：创建独立作用域 ==========
        // 后台服务本身是单例，仓储和 SqlSugarClient 是 Scoped。每轮清理必须创建 scope，
        // 确保数据库上下文生命周期和普通请求隔离。
        using var scope = _scopeFactory.CreateScope();
        var requestLogRepo = scope.ServiceProvider.GetRequiredService<IChargeRequestLogRepository>();
        var discountRepo = scope.ServiceProvider.GetRequiredService<IChargeDiscountDetailRepository>();
        var limitRepo = scope.ServiceProvider.GetRequiredService<ILimitOccupyRepository>();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        // ========== 第二阶段：筛选超时的 CONFIRM_PENDING ==========
        // 这里用 RequestAt + ConfirmExpireMinutes 判断过期时间，与 confirm 返回有效期口径一致。
        var expireBefore = DateTime.Now.AddMinutes(-_options.ConfirmExpireMinutes);
        var expired = await requestLogRepo.GetPendingExpiredAsync(expireBefore);

        foreach (var log in expired)
        {
            // ========== 第三阶段：逐笔事务处理 ==========
            // 不使用一条 UPDATE 批量改状态，是为了避免 commit 与 expire 并发竞态。
            // 每条记录重新读取当前状态，确认仍然是 CONFIRM_PENDING 后才过期。
            await db.Ado.BeginTranAsync();
            try
            {
                var current = await requestLogRepo.GetByIdAsync(log.RequestId);
                if (current is null || current.BusinessStatus != "CONFIRM_PENDING")
                {
                    // 记录可能已经被 HIS commit/cancel，或者被上一轮任务处理过。
                    // 此时提交空事务并跳过，不要把已经推进的状态覆盖成 EXPIRED。
                    await db.Ado.CommitTranAsync();
                    continue;
                }

                // ========== 第四阶段：三张资金表同步过期 ==========
                // 请求日志、折价明细和限额占用必须一起变成 EXPIRED，避免报表和累计口径不一致。
                current.BusinessStatus = "EXPIRED";
                current.ResponseAt = DateTime.Now;
                await requestLogRepo.UpdateAsync(current);

                await discountRepo.UpdateStatusByRequestIdAsync(log.RequestId, "EXPIRED");
                await limitRepo.UpdateStatusByRequestIdAsync(log.RequestId, "EXPIRED");

                await db.Ado.CommitTranAsync();
            }
            catch
            {
                // 任意一步失败都回滚本条记录，下一轮清理会再次尝试。
                // 这比留下部分 EXPIRED 更安全。
                await db.Ado.RollbackTranAsync();
                throw;
            }

            _logger.LogInformation("过期清理 RequestId={RequestId}", log.RequestId);
        }

        if (expired.Count > 0)
        {
            _logger.LogInformation("过期清理完成, 处理 {Count} 条记录", expired.Count);
        }
    }
}
