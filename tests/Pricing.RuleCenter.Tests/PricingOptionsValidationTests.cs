using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Infrastructure;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class PricingOptionsValidationTests
{
    [Fact]
    public void AddInfrastructure_RejectsMissingOracleConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Pricing:OracleConnectionString"] = "",
                ["Pricing:ConfirmExpireMinutes"] = "30",
                ["Pricing:ExpireCleanupIntervalSeconds"] = "300",
                ["Pricing:CacheVersionSyncIntervalSeconds"] = "3"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<PricingOptions>>();

        var ex = Assert.Throws<OptionsValidationException>(() => _ = options.Value);
        Assert.Contains("OracleConnectionString", ex.Message);
    }
}
