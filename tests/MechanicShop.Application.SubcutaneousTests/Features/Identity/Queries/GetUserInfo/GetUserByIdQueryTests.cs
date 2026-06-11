using MechanicShop.Application.Features.Identity.Queries.GetUserInfo;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Identity.Queries.GetUserInfo;

public class GetUserByIdQueryTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var query = new GetUserByIdQuery("user-id");

        Assert.Equal("user-id", query.UserId);
    }
}
