using MechanicShop.Application.Features.Identity.Queries.RefreshTokens;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Identity.Queries.RefreshTokens;

public class RefreshTokenQueryTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var query = new RefreshTokenQuery("refresh-token", "expired-access-token");

        Assert.Equal("refresh-token", query.RefreshToken);
        Assert.Equal("expired-access-token", query.ExpiredAccessToken);
    }
}
