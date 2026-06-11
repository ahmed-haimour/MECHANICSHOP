using MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Commands.UpdateCustomer;

public class UpdateVehicleCommandValidatorTests
{
    private readonly UpdateVehicleCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Make_Is_Empty()
    {
        var result = _validator.Validate(new UpdateVehicleCommand(Guid.NewGuid(), "", "Accord", 2024, "ABC123"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Make");
    }

    [Fact]
    public void Should_Have_Error_When_Model_Is_Empty()
    {
        var result = _validator.Validate(new UpdateVehicleCommand(Guid.NewGuid(), "Honda", "", 2024, "ABC123"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Model");
    }

    [Fact]
    public void Should_Have_Error_When_LicensePlate_Is_Empty()
    {
        var result = _validator.Validate(new UpdateVehicleCommand(Guid.NewGuid(), "Honda", "Accord", 2024, ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LicensePlate");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var result = _validator.Validate(new UpdateVehicleCommand(Guid.NewGuid(), "Honda", "Accord", 2024, "ABC123"));

        Assert.True(result.IsValid);
    }
}
