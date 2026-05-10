using HIS.Pricing.Client;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class HisClientPricingApiUrlBuilderTests
{
    [Fact]
    public void BuildRulesQuery_EncodesOptionalFiltersAndPagination()
    {
        string query = PricingApiUrlBuilder.BuildRulesQuery("CT 001/头", "PUBLISHED", "折价", 2, 50);

        Assert.Equal("/api/pricing/rules?itemCode=CT%20001%2F%E5%A4%B4&status=PUBLISHED&category=%E6%8A%98%E4%BB%B7&pageIndex=2&pageSize=50", query);
    }
}
