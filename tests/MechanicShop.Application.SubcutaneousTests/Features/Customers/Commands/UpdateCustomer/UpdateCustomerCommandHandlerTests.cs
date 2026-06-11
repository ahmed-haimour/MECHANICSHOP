using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Commands.UpdateCustomer;

[Collection(WebAppFactoryCollection.CollectionName)]
public class UpdateCustomerCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync(default);

        var command = new UpdateCustomerCommand(
            customer.Id,
            "Updated Customer",
            "5555555555",
            "updated@example.com",
            [new UpdateVehicleCommand(vehicle.Id, "Toyota", "Camry", 2024, "XYZ123")]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsSuccess);

        var updatedCustomer = await _context.Customers.AsNoTracking().FirstAsync(x => x.Id == customer.Id);
        Assert.Equal("Updated Customer", updatedCustomer.Name);
        Assert.Equal("updated@example.com", updatedCustomer.Email);
    }

    [Fact]
    public async Task Handle_WithMissingCustomer_ShouldFail()
    {
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            "Updated Customer",
            "5555555555",
            "updated@example.com",
            [new UpdateVehicleCommand(null, "Toyota", "Camry", 2024, "XYZ123")]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
    }
}
