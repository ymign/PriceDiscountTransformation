namespace Pricing.RuleCenter.Core.Options;

public sealed class PricingOptions
{
    public string OracleConnectionString { get; set; } = string.Empty;
    public int ConfirmExpireMinutes { get; set; } = 30;
    public int ExpireCleanupIntervalSeconds { get; set; } = 300;
    public int HttpTimeoutSeconds { get; set; } = 10;
    public int MaxRetryCount { get; set; } = 3;
}
