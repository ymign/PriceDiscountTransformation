using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Pricing.RuleCenter.Api.Startup;

/// <summary>
/// 规则中心 API 启动日志所需的运行元信息。
/// </summary>
public sealed record RuleCenterStartupInfo
{
    private const string DefaultServiceName = "Pricing.RuleCenter.Api";

    /// <summary>服务名称。</summary>
    public string ServiceName { get; init; } = DefaultServiceName;

    /// <summary>当前宿主环境。</summary>
    public string Environment { get; init; } = string.Empty;

    /// <summary>内容根目录。</summary>
    public string ContentRoot { get; init; } = string.Empty;

    /// <summary>当前绑定的监听地址。</summary>
    public string Urls { get; init; } = string.Empty;

    /// <summary>是否启用 Swagger。</summary>
    public bool SwaggerEnabled { get; init; }

    /// <summary>构建提交号。</summary>
    public string? BuildCommit { get; init; }

    /// <summary>构建分支。</summary>
    public string? BuildBranch { get; init; }

    /// <summary>UTC 构建时间。</summary>
    public string? BuildTimeUtc { get; init; }

    /// <summary>
    /// 根据配置和宿主环境创建启动元信息。
    /// </summary>
    public static RuleCenterStartupInfo Create(
        IConfiguration configuration,
        IHostEnvironment environment,
        IEnumerable<string>? urls = null)
    {
        return Create(configuration, environment.EnvironmentName, environment.ContentRootPath, urls);
    }

    /// <summary>
    /// 根据配置、环境名和内容根目录创建启动元信息。
    /// </summary>
    public static RuleCenterStartupInfo Create(
        IConfiguration configuration,
        string environmentName,
        string contentRoot,
        IEnumerable<string>? urls = null)
    {
        return new RuleCenterStartupInfo
        {
            ServiceName = configuration["Service:Name"] ?? DefaultServiceName,
            Environment = environmentName,
            ContentRoot = contentRoot,
            Urls = FormatUrls(configuration, urls),
            SwaggerEnabled = ResolveSwaggerEnabled(configuration, environmentName),
            BuildCommit = configuration["Build:Commit"] ?? System.Environment.GetEnvironmentVariable("BUILD_COMMIT"),
            BuildBranch = configuration["Build:Branch"] ?? System.Environment.GetEnvironmentVariable("BUILD_BRANCH"),
            BuildTimeUtc = configuration["Build:TimeUtc"] ?? System.Environment.GetEnvironmentVariable("BUILD_TIME_UTC")
        };
    }

    /// <summary>
    /// 解析 Swagger 是否启用，保持与现有启动逻辑一致。
    /// </summary>
    public static bool ResolveSwaggerEnabled(IConfiguration configuration, string environmentName)
    {
        var setting = configuration["Swagger:Enabled"];
        return string.IsNullOrWhiteSpace(setting)
            ? string.Equals(environmentName, Environments.Development, StringComparison.OrdinalIgnoreCase)
            : bool.TryParse(setting, out var enabled) && enabled;
    }

    private static string FormatUrls(IConfiguration configuration, IEnumerable<string>? urls)
    {
        var resolvedUrls = urls?
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToArray();
        if (resolvedUrls is { Length: > 0 })
        {
            return string.Join(";", resolvedUrls);
        }

        return configuration["ASPNETCORE_URLS"] ?? "not-bound";
    }
}
