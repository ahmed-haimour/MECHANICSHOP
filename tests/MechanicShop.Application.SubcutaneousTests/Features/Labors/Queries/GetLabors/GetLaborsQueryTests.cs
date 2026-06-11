using MechanicShop.Application.Features.Labors.Queries.GetLabors;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Labors.Queries.GetLabors;

public class GetLaborsQueryTests
{
    [Fact]
    public void Constructor_ShouldSetCacheProperties()
    {
        var query = new GetLaborsQuery();

        Assert.Equal("labors", query.CacheKey);
        Assert.Equal(["labors"], query.Tags);
    }
}
