using MechanicShop.Application.Features.Customers.Queries.GetCustomerById;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var customerId = Guid.NewGuid();

        var query = new GetCustomerByIdQuery(customerId);

        Assert.Equal(customerId, query.CustomerId);
        Assert.Equal($"customer_{customerId}", query.CacheKey);
        Assert.Equal(["customer"], query.Tags);
    }
}
