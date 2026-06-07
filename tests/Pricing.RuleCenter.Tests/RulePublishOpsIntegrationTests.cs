using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Aggregates.Catalog;
using Pricing.RuleCenter.Core.Interfaces.Catalog;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RulePublishOpsIntegrationTests
{
    [Fact]
    public async Task CacheOutboxSummaryEndpoint_RequiresRuleAdminAndReturnsSummary()
    {
        await using var factory = new CacheOutboxApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "admin-key");

        var response = await client.GetAsync("/api/pricing/ops/cache-outbox?maxFailedCount=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ApiResult<RuleCacheOutboxSummaryResponse>>();
        Assert.NotNull(payload);
        Assert.Equal(0, payload!.Code);
        Assert.Equal(1, payload.Data!.PendingCount);
        Assert.Equal(1, payload.Data.FailedCount);
    }

    private sealed class CacheOutboxApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Pricing:OracleConnectionString"] = "Data Source=TEST;User Id=test;Password=test;",
                    ["Authentication:ApiKey:Keys:0:Key"] = "admin-key",
                    ["Authentication:ApiKey:Keys:0:Roles:0"] = "pricing.admin"
                });
            });
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IRuleCacheInvalidationOutboxRepository>(
                    new StubRuleCacheInvalidationOutboxRepository(
                        new RuleCacheInvalidationOutbox
                        {
                            OutboxId = 1,
                            CacheScope = "EFFECTIVE_RULES",
                            OperationType = "PUBLISH",
                            RuleId = 10,
                            VersionNo = 1,
                            Status = CacheInvalidationOutboxStatusCodes.Pending,
                            CreatedAt = new DateTime(2026, 6, 7, 10, 0, 0)
                        },
                        new RuleCacheInvalidationOutbox
                        {
                            OutboxId = 2,
                            CacheScope = "ACTION_TYPE_ORDER",
                            OperationType = "ROLLBACK",
                            RuleId = 11,
                            VersionNo = 2,
                            Status = CacheInvalidationOutboxStatusCodes.Failed,
                            RetryCount = 2,
                            LastError = "sync failed",
                            CreatedAt = new DateTime(2026, 6, 7, 10, 1, 0)
                        }));

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

    private sealed class StubRuleCacheInvalidationOutboxRepository : IRuleCacheInvalidationOutboxRepository
    {
        private readonly IReadOnlyList<RuleCacheInvalidationOutbox> _items;

        public StubRuleCacheInvalidationOutboxRepository(params RuleCacheInvalidationOutbox[] items)
        {
            _items = items;
        }

        public Task<long> InsertAsync(RuleCacheInvalidationOutbox entity) => Task.FromResult(0L);
        public Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetPendingAsync(DateTime now, int maxCount) => Task.FromResult((IReadOnlyList<RuleCacheInvalidationOutbox>)Array.Empty<RuleCacheInvalidationOutbox>());
        public Task<IReadOnlyList<RuleCacheInvalidationOutbox>> GetForDashboardAsync(int maxFailedCount) => Task.FromResult(_items);
        public Task<bool> MarkProcessedAsync(long outboxId, DateTime processedAt) => Task.FromResult(true);
        public Task<bool> MarkFailedAsync(long outboxId, string lastError, int retryCount, DateTime nextRetryAt) => Task.FromResult(true);
    }
}
