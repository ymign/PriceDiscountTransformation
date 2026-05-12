using HIS.Pricing.Client;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class HisClientPricingProductizationTests
{
    [Fact]
    public void ConfigLoader_LoadsProductRuntimeOptions()
    {
        var root = CreateTempDirectory();
        try
        {
            var configPath = Path.Combine(root, "pricing-agent.config");
            File.WriteAllLines(configPath, new[]
            {
                "BaseUrl=http://pricing-center:8080/",
                "SourceSystem=HIS_WY",
                "DefaultChargeScene=INPATIENT",
                "TimeoutMs=15000",
                "MaxRetry=4",
                "RetryDelayMs=500",
                "EnableLocalLog=true",
                "LogDirectory=" + Path.Combine(root, "logs"),
                "EnableCompensationQueue=true",
                "CompensationDirectory=" + Path.Combine(root, "pending"),
                "ExpectedProtocolVersion=1.0"
            });

            var options = PricingSdkConfigLoader.LoadFromFile(configPath);

            Assert.Equal("http://pricing-center:8080", options.GetNormalizedBaseUrl());
            Assert.Equal("HIS_WY", options.SourceSystem);
            Assert.Equal("INPATIENT", options.DefaultChargeScene);
            Assert.Equal(15000, options.TimeoutMs);
            Assert.Equal(4, options.MaxRetry);
            Assert.True(options.EnableLocalLog);
            Assert.True(options.EnableCompensationQueue);
            Assert.Equal("1.0", options.ExpectedProtocolVersion);
        }
        finally
        {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void Logger_WritesOperationLogWithoutThrowing()
    {
        var root = CreateTempDirectory();
        try
        {
            var logger = new PricingAgentLogger(root);

            logger.Info("commit", "1001", "CHG001", "REQ001", "TRACE001", 0, 12, "ok");

            var files = Directory.GetFiles(root, "pricing-agent-*.log");
            Assert.Single(files);
            Assert.Contains("commit", File.ReadAllText(files[0]));
        }
        finally
        {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void CompensationStore_SavesPendingOperationRecord()
    {
        var root = CreateTempDirectory();
        try
        {
            var store = new PricingCompensationStore(root);
            var request = new PricingCancelRequest { RequestId = 1001 };

            var path = store.SavePending("cancel", "1001", request, 500, "failed", "TRACE001", null);

            Assert.True(File.Exists(path));
            var files = PricingCompensationStore.GetPendingFiles(root);
            Assert.Single(files);
            Assert.Contains("cancel", File.ReadAllText(path));
        }
        finally
        {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void AgentVersion_RejectsMismatchedProtocolVersion()
    {
        var health = new PricingServiceHealthResponse
        {
            Status = "healthy",
            ProtocolVersion = "2.0"
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            PricingAgentVersion.EnsureCompatibleService(health));

        Assert.Contains("协议版本不兼容", ex.Message);
    }

    [Fact]
    public void ServerHealthResult_ExposesProtocolVersionForSdkCompatibility()
    {
        var health = new Pricing.RuleCenter.Api.Controllers.HealthResult();

        Assert.Equal(PricingAgentVersion.ProtocolVersion, health.ProtocolVersion);
        Assert.False(string.IsNullOrWhiteSpace(health.ServiceVersion));
    }

    private static string CreateTempDirectory()
    {
        string path = Path.Combine(Path.GetTempPath(), "pricing-agent-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
}
