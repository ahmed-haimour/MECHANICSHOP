using MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Commands.UpdateCustomer;

public class UpdateVehicleCommandTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var vehicleId = Guid.NewGuid();

        var command = new UpdateVehicleCommand(vehicleId, "Honda", "Accord", 2024, "ABC123");

        Assert.Equal(vehicleId, command.VehicleId);
        Assert.Equal("Honda", command.Make);
        Assert.Equal("Accord", command.Model);
        Assert.Equal(2024, command.Year);
        Assert.Equal("ABC123", command.LicensePlate);
    }
}
