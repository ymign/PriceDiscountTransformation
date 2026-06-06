using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Infrastructure;

/// <summary>
/// 启动期配置校验服务。
/// </summary>
public sealed class OptionsValidationStartupService : IHostedService
{
    private readonly IOptions<PricingOptions> _pricingOptions;

    /// <summary>初始化启动期配置校验服务。</summary>
    public OptionsValidationStartupService(IOptions<PricingOptions> pricingOptions)
    {
        _pricingOptions = pricingOptions;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = _pricingOptions.Value;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
