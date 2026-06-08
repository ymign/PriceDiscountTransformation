using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Pricing.RuleCenter.Api.HealthChecks;

/// <summary>
/// 关键第三方服务健康检查。
/// </summary>
/// <remarks>
/// 配置格式：
/// <code>
/// "HealthChecks": {
///   "ThirdPartyServices": [
///     { "Name": "price-master", "Url": "https://host/health" }
///   ]
/// }
/// </code>
/// 未配置第三方服务时返回 Healthy，表示当前部署没有启用外部探针。
/// </remarks>
public sealed class ThirdPartyServicesHealthCheck : IHealthCheck
{
    private const string ConfigurationSectionName = "HealthChecks:ThirdPartyServices";

    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ThirdPartyServicesHealthCheck> _logger;

    /// <summary>
    /// 初始化第三方服务健康检查。
    /// </summary>
    /// <param name="configuration">应用配置。</param>
    /// <param name="httpClientFactory">HTTP 客户端工厂。</param>
    /// <param name="logger">健康检查日志。</param>
    public ThirdPartyServicesHealthCheck(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<ThirdPartyServicesHealthCheck> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var targets = _configuration
            .GetSection(ConfigurationSectionName)
            .Get<IReadOnlyList<ThirdPartyHealthCheckTarget>>() ?? Array.Empty<ThirdPartyHealthCheckTarget>();

        if (targets.Count == 0)
        {
            return HealthCheckResult.Healthy("No third-party health check targets configured.");
        }

        var client = _httpClientFactory.CreateClient("health-checks");
        var failures = new List<string>();
        var checkedTargets = new Dictionary<string, object>();

        foreach (var target in targets.Where(t => !string.IsNullOrWhiteSpace(t.Url)))
        {
            var name = string.IsNullOrWhiteSpace(target.Name) ? target.Url : target.Name;
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, target.Url);
                using var response = await client.SendAsync(request, cancellationToken);
                checkedTargets[name] = (int)response.StatusCode;
                if (!response.IsSuccessStatusCode)
                {
                    failures.Add($"{name}:{(int)response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "第三方服务健康检查失败 Target={Target}", name);
                checkedTargets[name] = ex.GetType().Name;
                failures.Add($"{name}:{ex.GetType().Name}");
            }
        }

        if (failures.Count == 0)
        {
            return HealthCheckResult.Healthy("All configured third-party services are reachable.", checkedTargets);
        }

        return HealthCheckResult.Unhealthy(
            $"Third-party service health check failed: {string.Join(", ", failures)}",
            data: checkedTargets);
    }

    private sealed class ThirdPartyHealthCheckTarget
    {
        /// <summary>
        /// 第三方服务名称。
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// 第三方服务健康检查 URL。
        /// </summary>
        public string Url { get; init; } = string.Empty;
    }
}
