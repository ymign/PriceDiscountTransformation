using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pricing.RuleCenter.Api.Controllers;
using Pricing.RuleCenter.Api.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
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
        Assert.True(root.GetProperty("data").TryGetProperty("total_duration_ms", out _));
        Assert.False(root.GetProperty("data").TryGetProperty("totalDurationMs", out _));
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
        Assert.Equal("abc1234", data.GetProperty("build_commit").GetString());
        Assert.Equal("main", data.GetProperty("build_branch").GetString());
        Assert.Equal("2026-06-07T10:00:00Z", data.GetProperty("build_time_utc").GetString());
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("service_version").GetString()));
        Assert.Equal("1.0", data.GetProperty("protocol_version").GetString());
        Assert.False(data.TryGetProperty("buildCommit", out _));
    }

    [Fact]
    public void ApiJsonOptions_CanDeserializeSnakeCasePricingRequest()
    {
        using var factory = new PricingRuleCenterWebApplicationFactory(new Dictionary<string, string?>());
        var jsonOptions = factory.Services
            .GetRequiredService<IOptions<JsonOptions>>()
            .Value
            .JsonSerializerOptions;

        var request = JsonSerializer.Deserialize<PricingCalculateRequest>(
            """
            {
              "source_system": "HIS",
              "patient_id": "P001",
              "business_request_no": "BR202606080001",
              "business_charge_time": "2026-06-08T10:00:00",
              "items": [
                {
                  "item_code": "FW001",
                  "input_qty": 2,
                  "unit_price": 12.34,
                  "pricing_parts": [
                    {
                      "part_seq": 1,
                      "body_part_code": "HEAD",
                      "measure_value": 1.5
                    }
                  ]
                }
              ]
            }
            """,
            jsonOptions);

        Assert.NotNull(request);
        Assert.Equal("HIS", request!.SourceSystem);
        Assert.Equal("BR202606080001", request.BusinessRequestNo);
        Assert.Equal("FW001", request.Items[0].ItemCode);
        Assert.Equal(12.34m, request.Items[0].UnitPrice);
        Assert.Equal("HEAD", request.Items[0].PricingParts![0].BodyPartCode);
    }

    [Fact]
    public void PublicApiDtos_DeclareExplicitSnakeCaseJsonNames()
    {
        AssertJsonName<PricingCalculateRequest>(nameof(PricingCalculateRequest.BusinessRequestNo), "business_request_no");
        AssertJsonName<PricingCalculateItemRequest>(nameof(PricingCalculateItemRequest.ItemCode), "item_code");
        AssertJsonName<PricingPartItemRequest>(nameof(PricingPartItemRequest.BodyPartCode), "body_part_code");
        AssertJsonName<RuleCacheOutboxSummaryResponse>(nameof(RuleCacheOutboxSummaryResponse.PendingCount), "pending_count");
        AssertJsonName<HealthVersionResult>(nameof(HealthVersionResult.BuildCommit), "build_commit");
        AssertJsonName<HealthCheckSummary>(nameof(HealthCheckSummary.TotalDurationMs), "total_duration_ms");
    }

    [Fact]
    public async Task SwaggerJson_UsesSnakeCaseSchemaPropertyNames()
    {
        await using var factory = new PricingRuleCenterWebApplicationFactory(new Dictionary<string, string?>
        {
            ["Swagger:Enabled"] = "true"
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var schemas = document.RootElement.GetProperty("components").GetProperty("schemas");

        var requestProperties = schemas
            .GetProperty(nameof(PricingCalculateRequest))
            .GetProperty("properties");
        Assert.True(requestProperties.TryGetProperty("source_system", out _));
        Assert.True(requestProperties.TryGetProperty("business_request_no", out _));
        Assert.False(requestProperties.TryGetProperty("sourceSystem", out _));

        var itemProperties = schemas
            .GetProperty(nameof(PricingCalculateItemRequest))
            .GetProperty("properties");
        Assert.True(itemProperties.TryGetProperty("item_code", out _));
        Assert.True(itemProperties.TryGetProperty("input_qty", out _));
        Assert.False(itemProperties.TryGetProperty("itemCode", out _));

        var specialFlagPath = document.RootElement
            .GetProperty("paths")
            .EnumerateObject()
            .FirstOrDefault(item => item.Name.EndsWith("/special-flag", StringComparison.OrdinalIgnoreCase));
        Assert.NotEqual(default, specialFlagPath);

        var parameterNames = specialFlagPath.Value
            .GetProperty("get")
            .GetProperty("parameters")
            .EnumerateArray()
            .Where(parameter => parameter.GetProperty("in").GetString() == "query")
            .Select(parameter => parameter.GetProperty("name").GetString())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        Assert.Contains("charge_scene", parameterNames);
        Assert.Contains("business_charge_time", parameterNames);
        Assert.DoesNotContain("chargeScene", parameterNames);
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

    private static void AssertJsonName<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        Assert.NotNull(property);

        var attribute = property!.GetCustomAttributes(typeof(JsonPropertyNameAttribute), inherit: false)
            .Cast<JsonPropertyNameAttribute>()
            .SingleOrDefault();
        Assert.NotNull(attribute);
        Assert.Equal(expectedJsonName, attribute!.Name);
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
