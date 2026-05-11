using HIS.Pricing.Client;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class HisClientPricingSdkOptionsTests
{
    [Fact]
    public void NormalizeBaseUrl_TrimsWhitespaceAndTrailingSlash()
    {
        string? normalized = PricingSdkOptions.NormalizeBaseUrl("  http://pricing-rule-center/  ");

        Assert.Equal("http://pricing-rule-center", normalized);
    }

    [Fact]
    public void ValidateForHttpClient_RejectsMissingBaseUrl()
    {
        PricingSdkOptions options = new();

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
            () => options.ValidateForHttpClient());

        Assert.Contains("BaseUrl", ex.Message);
    }

    [Fact]
    public void Constructor_SetsProductDefaults()
    {
        PricingSdkOptions options = new();

        Assert.Equal(10000, options.TimeoutMs);
        Assert.Equal(3, options.MaxRetry);
        Assert.Equal(2000, options.RetryDelayMs);
        Assert.Equal("HIS", options.SourceSystem);
        Assert.Equal("OUTPATIENT", options.DefaultChargeScene);
    }
}
