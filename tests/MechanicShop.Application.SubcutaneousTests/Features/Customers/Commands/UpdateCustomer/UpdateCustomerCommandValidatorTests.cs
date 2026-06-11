using MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidatorTests
{
    private readonly UpdateCustomerCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var result = _validator.Validate(ValidCommand() with { Name = "" });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var result = _validator.Validate(ValidCommand() with { Email = "invalid-email" });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Should_Have_Error_When_Vehicles_Is_Empty()
    {
        var result = _validator.Validate(ValidCommand() with { Vehicles = [] });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Vehicles");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var result = _validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    private static UpdateCustomerCommand ValidCommand()
    {
        return new UpdateCustomerCommand(
            Guid.NewGuid(),
            "Mohammed",
            "5555555555",
            "mohammed@example.com",
            [new UpdateVehicleCommand(Guid.NewGuid(), "Honda", "Accord", 2024, "ABC123")]);
    }
}
