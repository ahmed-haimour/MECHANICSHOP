using MechanicShop.Application.Features.Identity.Queries.GenerateTokens;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Identity.Queries.GenerateTokens;

public class GenerateTokenQueryTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var query = new GenerateTokenQuery("user@example.com", "password");

        Assert.Equal("user@example.com", query.Email);
        Assert.Equal("password", query.Password);
    }
}
