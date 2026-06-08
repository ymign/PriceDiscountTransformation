using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Application.Pricing.AuthorityPrice;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class AuthorityPriceCheckerTests
{
    [Fact]
    public async Task CheckAsync_ShouldUseChildPrice_WhenPatientAgeUnderSix()
    {
        var repository = new FixedPriceMasterRepository(new PriceMasterItem
        {
            ItemCode = "ITEM_CHILD",
            UnitPrice = 100m,
            ChildPrice = 130m,
            PerinatalPrice = 150m
        });
        var request = CreateRequest(
            patientAge: 5,
            new PricingCalculateItemRequest { ItemCode = "ITEM_CHILD", UnitPrice = 130m, InputQty = 1m });

        await CreateChecker(repository).CheckAsync(request, request.Items);

        Assert.Equal(1, repository.PriceItemsBatchCallCount);
    }

    [Fact]
    public async Task CheckAsync_ShouldUseNormalPrice_WhenPatientAgeIsSix()
    {
        var repository = new FixedPriceMasterRepository(new PriceMasterItem
        {
            ItemCode = "ITEM_NORMAL",
            UnitPrice = 100m,
            ChildPrice = 130m,
            PerinatalPrice = 150m
        });
        var request = CreateRequest(
            patientAge: 6,
            new PricingCalculateItemRequest { ItemCode = "ITEM_NORMAL", UnitPrice = 100m, InputQty = 1m });

        await CreateChecker(repository).CheckAsync(request, request.Items);
    }

    [Fact]
    public async Task CheckAsync_ShouldUsePerinatalPrice_WhenPerinatalFlagProvided()
    {
        var repository = new FixedPriceMasterRepository(new PriceMasterItem
        {
            ItemCode = "ITEM_PERINATAL",
            UnitPrice = 100m,
            ChildPrice = 130m,
            PerinatalPrice = 150m
        });
        var request = CreateRequest(
            patientAge: 30,
            new PricingCalculateItemRequest { ItemCode = "ITEM_PERINATAL", UnitPrice = 150m, InputQty = 1m },
            new Dictionary<string, object?> { ["is_perinatal"] = true });

        await CreateChecker(repository).CheckAsync(request, request.Items);
    }

    [Fact]
    public async Task CheckAsync_ShouldOnlyLogWarning_WhenRequestPriceDiffersFromResolvedPrice()
    {
        var repository = new FixedPriceMasterRepository(new PriceMasterItem
        {
            ItemCode = "ITEM_CHILD",
            UnitPrice = 100m,
            ChildPrice = 130m
        });
        var request = CreateRequest(
            patientAge: 5,
            new PricingCalculateItemRequest { ItemCode = "ITEM_CHILD", UnitPrice = 100m, InputQty = 1m });
        var logger = new CapturingLogger<AuthorityPriceChecker>();

        await CreateChecker(repository, logger).CheckAsync(request, request.Items);

        Assert.Contains(logger.Entries, entry =>
            entry.Level == LogLevel.Warning &&
            entry.Message.Contains("单价不一致") &&
            entry.Message.Contains("儿童价"));
    }

    [Fact]
    public async Task CheckAsync_ShouldOnlyLogWarning_WhenAuthorityPriceMissing()
    {
        var repository = new FixedPriceMasterRepository();
        var request = CreateRequest(
            patientAge: 30,
            new PricingCalculateItemRequest { ItemCode = "ITEM_MISSING", UnitPrice = 100m, InputQty = 1m });
        var logger = new CapturingLogger<AuthorityPriceChecker>();

        await CreateChecker(repository, logger).CheckAsync(request, request.Items);

        Assert.Contains(logger.Entries, entry =>
            entry.Level == LogLevel.Warning &&
            entry.Message.Contains("未找到项目权威单价"));
    }

    private static AuthorityPriceChecker CreateChecker(
        IPriceMasterRepository repository,
        ILogger<AuthorityPriceChecker>? logger = null) =>
        new(
            repository,
            Options.Create(new PricingOptions { EnableAuthorityPriceCheck = true }),
            logger ?? NullLogger<AuthorityPriceChecker>.Instance);

    private static PricingCalculateRequest CreateRequest(
        int? patientAge,
        PricingCalculateItemRequest item,
        Dictionary<string, object?>? extraParams = null) =>
        new()
        {
            PatientId = "P001",
            SourceSystem = "HIS",
            BusinessChargeTime = new DateTime(2026, 6, 8, 10, 0, 0),
            PatientAge = patientAge,
            ExtraParams = extraParams,
            Items = new[] { item }
        };

    private sealed class FixedPriceMasterRepository : IPriceMasterRepository
    {
        private readonly Dictionary<string, PriceMasterItem> _items;

        public FixedPriceMasterRepository(params PriceMasterItem[] items)
        {
            _items = items.ToDictionary(item => item.ItemCode, StringComparer.OrdinalIgnoreCase);
        }

        public int PriceItemsBatchCallCount { get; private set; }

        public Task<decimal?> GetUnitPriceAsync(string itemCode) =>
            Task.FromResult<decimal?>(_items.TryGetValue(itemCode, out var item) ? item.UnitPrice : null);

        public Task<IReadOnlyDictionary<string, PriceMasterItem?>> GetPriceItemsAsync(IReadOnlyCollection<string> itemCodes)
        {
            PriceItemsBatchCallCount++;
            var result = new Dictionary<string, PriceMasterItem?>(StringComparer.OrdinalIgnoreCase);
            foreach (var itemCode in itemCodes)
            {
                result[itemCode] = _items.GetValueOrDefault(itemCode);
            }

            return Task.FromResult((IReadOnlyDictionary<string, PriceMasterItem?>)result);
        }
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
