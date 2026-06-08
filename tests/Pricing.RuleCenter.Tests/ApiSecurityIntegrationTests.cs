using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ApiSecurityIntegrationTests
{
    [Fact]
    public async Task PricingConfirm_WithoutApiKey_ReturnsUnauthorized()
    {
        await using var factory = new SecureApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsJsonAsync("/api/pricing/calculate/confirm", new { });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RulePublish_WithServiceApiKey_ReturnsForbidden()
    {
        await using var factory = new SecureApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        client.DefaultRequestHeaders.Add("X-Api-Key", "service-key");

        var response = await client.PostAsJsonAsync("/api/pricing/rules/1/publish", new
        {
            version_no = 1,
            published_by = "tester"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RulePublish_WithAdminApiKey_ReturnsGoneWhenLegacyAuthoringDisabled()
    {
        await using var factory = new SecureApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        client.DefaultRequestHeaders.Add("X-Api-Key", "admin-key");

        var response = await client.PostAsJsonAsync("/api/pricing/rules/1/publish", new
        {
            version_no = 1,
            published_by = "tester"
        });

        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(document.RootElement.TryGetProperty("trace_id", out _));
        Assert.False(document.RootElement.TryGetProperty("traceId", out _));
    }

    private sealed class SecureApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Pricing:OracleConnectionString"] = "Data Source=TEST;User Id=test;Password=test;",
                    ["Authentication:ApiKey:Keys:0:Key"] = "service-key",
                    ["Authentication:ApiKey:Keys:0:Roles:0"] = "pricing.service",
                    ["Authentication:ApiKey:Keys:1:Key"] = "admin-key",
                    ["Authentication:ApiKey:Keys:1:Roles:0"] = "pricing.admin",
                    ["Authentication:ApiKey:Keys:1:Roles:1"] = "pricing.service"
                });
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
            });
        }
    }
}
