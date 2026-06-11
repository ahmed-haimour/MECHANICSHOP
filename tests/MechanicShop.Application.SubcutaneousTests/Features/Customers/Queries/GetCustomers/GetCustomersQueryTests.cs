using MechanicShop.Application.Features.Customers.Queries.GetCustomers;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryTests
{
    [Fact]
    public void Constructor_ShouldSetCacheProperties()
    {
        var query = new GetCustomersQuery();

        Assert.Equal("customers", query.CacheKey);
        Assert.Equal(["customer"], query.Tags);
    }
}
