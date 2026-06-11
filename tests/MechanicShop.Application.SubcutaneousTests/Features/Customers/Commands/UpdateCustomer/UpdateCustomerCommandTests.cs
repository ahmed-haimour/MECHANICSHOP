using MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var customerId = Guid.NewGuid();
        var vehicles = new List<UpdateVehicleCommand>
        {
            new(Guid.NewGuid(), "Honda", "Accord", 2024, "ABC123")
        };

        var command = new UpdateCustomerCommand(customerId, "Mohammed", "5555555555", "mohammed@example.com", vehicles);

        Assert.Equal(customerId, command.CustomerId);
        Assert.Equal("Mohammed", command.Name);
        Assert.Equal("5555555555", command.PhoneNumber);
        Assert.Equal("mohammed@example.com", command.Email);
        Assert.Same(vehicles, command.Vehicles);
    }
}
