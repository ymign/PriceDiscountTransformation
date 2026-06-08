using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Pricing.RuleCenter.Tests;

/// <summary>
/// API 层集成测试，验证真实 ASP.NET Core 管道中的关键工程化端点。
/// </summary>
public sealed class ApiDocumentationIntegrationTests
{
    /// <summary>
    /// 开发或显式开启配置下，Swagger JSON 应可访问。
    /// </summary>
    [Fact]
    public async Task SwaggerJson_IsAvailableWhenExplicitlyEnabled()
    {
        await using var factory = new PricingRuleCenterWebApplicationFactory(new Dictionary<string, string?>
        {
            ["Swagger:Enabled"] = "true"
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Pricing RuleCenter API", body);
        Assert.Contains("/api/pricing/templates", body);
        Assert.Contains("/api/pricing/policies", body);
        Assert.Contains("/api/pricing/runtime-packages/publish", body);
        Assert.DoesNotContain("/api/pricing/rules/1/publish", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("/api/pricing/rules/{ruleId}/publish", body, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 生产环境默认关闭 Swagger，除非显式配置 Swagger:Enabled=true。
    /// </summary>
    [Fact]
    public async Task SwaggerJson_IsDisabledByDefaultInProduction()
    {
        await using var factory = new PricingRuleCenterWebApplicationFactory(
            new Dictionary<string, string?>(),
            "Production");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// /health 应返回统一 ApiResult 包装的健康检查结果。
    /// </summary>
    [Fact]
    public async Task HealthEndpoint_ReturnsUnifiedApiResultWhenChecksAreHealthy()
    {
        await using var factory = new PricingRuleCenterWebApplicationFactory(new Dictionary<string, string?>(), configureHealthyChecks: true);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;
        Assert.Equal(0, root.GetProperty("code").GetInt32());
        Assert.Equal("healthy", root.GetProperty("message").GetString());
        Assert.Equal("Healthy", root.GetProperty("data").GetProperty("status").GetString());
        Assert.True(root.GetProperty("data").GetProperty("checks").TryGetProperty("self", out _));
    }

    [Fact]
    public async Task HealthVersionEndpoint_ReturnsBuildMetadataWhenConfigured()
    {
        await using var factory = new PricingRuleCenterWebApplicationFactory(new Dictionary<string, string?>
        {
            ["Build:Commit"] = "abc1234",
            ["Build:Branch"] = "main",
            ["Build:TimeUtc"] = "2026-06-07T10:00:00Z"
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/version");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = document.RootElement.GetProperty("data");
        Assert.Equal("abc1234", data.GetProperty("buildCommit").GetString());
        Assert.Equal("main", data.GetProperty("buildBranch").GetString());
        Assert.Equal("2026-06-07T10:00:00Z", data.GetProperty("buildTimeUtc").GetString());
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("serviceVersion").GetString()));
        Assert.Equal("1.0", data.GetProperty("protocolVersion").GetString());
    }

    /// <summary>
    /// 试算接口文档必须与真实规则执行顺序一致：数量限制先于公式计算。
    /// </summary>
    [Fact]
    public async Task PricingSimulateSwaggerDescription_UsesLimitBeforeFormulaOrdering()
    {
        await using var factory = new PricingRuleCenterWebApplicationFactory(new Dictionary<string, string?>
        {
            ["Swagger:Enabled"] = "true"
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var simulatePath = document.RootElement
            .GetProperty("paths")
            .EnumerateObject()
            .FirstOrDefault(item => item.Name.EndsWith("/pricing/calculate/simulate", StringComparison.OrdinalIgnoreCase));

        Assert.NotEqual(default, simulatePath);

        var postOperation = simulatePath.Value.GetProperty("post");
        var description = postOperation.TryGetProperty("description", out var descriptionElement)
            ? descriptionElement.GetString()
            : postOperation.GetProperty("summary").GetString();

        Assert.NotNull(description);
        var limitIndex = description!.IndexOf("日数量限制", StringComparison.Ordinal);
        var formulaIndex = description.IndexOf("公式计算", StringComparison.Ordinal);
        Assert.True(limitIndex >= 0, "Swagger 描述缺少“日数量限制”文本。");
        Assert.True(formulaIndex >= 0, "Swagger 描述缺少“公式计算”文本。");
        Assert.True(limitIndex < formulaIndex, "Swagger 描述顺序错误：应先写数量限制，再写公式计算。");
    }

    private sealed class PricingRuleCenterWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly IReadOnlyDictionary<string, string?> _settings;
        private readonly string _environment;
        private readonly bool _configureHealthyChecks;

        public PricingRuleCenterWebApplicationFactory(
            IReadOnlyDictionary<string, string?> settings,
            string environment = "Development",
            bool configureHealthyChecks = false)
        {
            _settings = settings;
            _environment = environment;
            _configureHealthyChecks = configureHealthyChecks;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(_environment);
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(_settings);
            });
            builder.ConfigureServices(services =>
            {
                var hostedServices = services
                    .Where(descriptor => descriptor.ServiceType == typeof(IHostedService))
                    .ToList();
                foreach (var descriptor in hostedServices)
                {
                    services.Remove(descriptor);
                }

                if (_configureHealthyChecks)
                {
                    services.Configure<HealthCheckServiceOptions>(options =>
                    {
                        options.Registrations.Clear();
                        options.Registrations.Add(new HealthCheckRegistration(
                            "self",
                            _ => new HealthyTestHealthCheck(),
                            HealthStatus.Unhealthy,
                            Array.Empty<string>()));
                    });
                }
            });
        }
    }

    private sealed class HealthyTestHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("test host is healthy"));
        }
    }
}
