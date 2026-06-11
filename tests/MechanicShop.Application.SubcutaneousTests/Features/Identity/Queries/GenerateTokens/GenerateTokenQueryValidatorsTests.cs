using MechanicShop.Application.Features.Identity.Queries.GenerateTokens;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Identity.Queries.GenerateTokens;

public class GenerateTokenQueryValidatorsTests
{
    private readonly GenerateTokenQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var result = _validator.Validate(new GenerateTokenQuery("", "password"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var result = _validator.Validate(new GenerateTokenQuery("user@example.com", ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var result = _validator.Validate(new GenerateTokenQuery("user@example.com", "password"));

        Assert.True(result.IsValid);
    }
}
