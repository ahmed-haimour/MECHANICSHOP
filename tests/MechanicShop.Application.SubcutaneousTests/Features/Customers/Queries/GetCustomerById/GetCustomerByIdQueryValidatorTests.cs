using MechanicShop.Application.Features.Customers.Queries.GetCustomerById;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryValidatorTests
{
    private readonly GetCustomerByIdQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_CustomerId_Is_Empty()
    {
        var result = _validator.Validate(new GetCustomerByIdQuery(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CustomerId");
    }

    [Fact]
    public void Should_Pass_When_CustomerId_Is_Valid()
    {
        var result = _validator.Validate(new GetCustomerByIdQuery(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
