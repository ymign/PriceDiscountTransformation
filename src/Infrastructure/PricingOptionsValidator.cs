using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Core.Options;

namespace Pricing.RuleCenter.Infrastructure;

/// <summary>
/// PricingOptions 字段级配置校验器。
/// </summary>
public sealed class PricingOptionsValidator : IValidateOptions<PricingOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, PricingOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.OracleConnectionString))
        {
            failures.Add("Pricing:OracleConnectionString 不能为空");
        }

        if (options.ConfirmExpireMinutes <= 0)
        {
            failures.Add("Pricing:ConfirmExpireMinutes 必须大于 0");
        }

        if (options.ExpireCleanupIntervalSeconds <= 0)
        {
            failures.Add("Pricing:ExpireCleanupIntervalSeconds 必须大于 0");
        }

        if (options.CacheVersionSyncIntervalSeconds <= 0)
        {
            failures.Add("Pricing:CacheVersionSyncIntervalSeconds 必须大于 0");
        }

        if (options.HttpTimeoutSeconds <= 0)
        {
            failures.Add("Pricing:HttpTimeoutSeconds 必须大于 0");
        }

        if (options.MaxRetryCount < 0)
        {
            failures.Add("Pricing:MaxRetryCount 不能小于 0");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
