using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Api.Services;

public sealed class ExpireCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PricingOptions _options;
    private readonly ILogger<ExpireCleanupService> _logger;

    public ExpireCleanupService(
        IServiceScopeFactory scopeFactory,
        IOptions<PricingOptions> options,
        ILogger<ExpireCleanupService> logger)
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
                await CleanupExpiredAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "过期清理任务异常");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(_options.ExpireCleanupIntervalSeconds),
                stoppingToken);
        }
    }

    private async Task CleanupExpiredAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var requestLogRepo = scope.ServiceProvider.GetRequiredService<IChargeRequestLogRepository>();
        var discountRepo = scope.ServiceProvider.GetRequiredService<IChargeDiscountDetailRepository>();
        var limitRepo = scope.ServiceProvider.GetRequiredService<ILimitOccupyRepository>();

        var expireBefore = DateTime.Now.AddMinutes(-_options.ConfirmExpireMinutes);
        var expired = await requestLogRepo.GetPendingExpiredAsync(expireBefore);

        foreach (var log in expired)
        {
            log.BusinessStatus = "EXPIRED";
            log.ResponseAt = DateTime.Now;
            await requestLogRepo.UpdateAsync(log);

            await discountRepo.UpdateStatusByRequestIdAsync(log.RequestId, "EXPIRED");
            await limitRepo.UpdateStatusByRequestIdAsync(log.RequestId, "EXPIRED");

            _logger.LogInformation("过期清理 RequestId={RequestId}", log.RequestId);
        }

        if (expired.Count > 0)
        {
            _logger.LogInformation("过期清理完成, 处理 {Count} 条记录", expired.Count);
        }
    }
}
