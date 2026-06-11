using MechanicShop.Application.Features.Customers.Commands.CreateCustomer;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = ValidCommand() with { Name = "" };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = ValidCommand() with { Email = "invalid-email" };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Invalid()
    {
        var command = ValidCommand() with { PhoneNumber = "abc" };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PhoneNumber");
    }

    [Fact]
    public void Should_Have_Error_When_Vehicles_Is_Empty()
    {
        var command = ValidCommand() with { Vehicles = [] };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Vehicles");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var result = _validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    private static CreateCustomerCommand ValidCommand()
    {
        return new CreateCustomerCommand(
            "Mohammed",
            "5555555555",
            "mohammed@example.com",
            [new CreateVehicleCommand("Honda", "Accord", 2024, "ABC123")]);
    }
}
