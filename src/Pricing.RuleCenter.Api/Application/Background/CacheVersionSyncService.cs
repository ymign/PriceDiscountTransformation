using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Api.Application.Background;

/// <summary>
/// 缓存版本后台同步服务。
/// </summary>
/// <remarks>
/// 定时轮询数据库中的共享缓存版本号，发现变化后清理本机缓存。
/// 这样在多实例部署下，即使变更发生在其他节点，本机也能在短时间内收敛到最新缓存状态。
/// </remarks>
public sealed class CacheVersionSyncService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PricingOptions _options;
    private readonly ILogger<CacheVersionSyncService> _logger;

    public CacheVersionSyncService(
        IServiceScopeFactory scopeFactory,
        IOptions<PricingOptions> options,
        ILogger<CacheVersionSyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var synchronizer = scope.ServiceProvider.GetRequiredService<ICacheVersionSynchronizer>();
                await synchronizer.SyncAsync(stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "缓存版本同步任务异常");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(_options.CacheVersionSyncIntervalSeconds),
                stoppingToken);
        }
    }
}
