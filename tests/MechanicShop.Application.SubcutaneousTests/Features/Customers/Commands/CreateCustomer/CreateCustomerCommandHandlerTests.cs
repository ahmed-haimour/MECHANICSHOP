using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Commands.CreateCustomer;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Commands.CreateCustomer;

[Collection(WebAppFactoryCollection.CollectionName)]
public class CreateCustomerCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var command = new CreateCustomerCommand(
            "Mohammed",
            "5555555555",
            "mohammed.unique@example.com",
            [new CreateVehicleCommand("Honda", "Accord", 2024, "ABC123")]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsSuccess);
        Assert.Equal("mohammed.unique@example.com", result.Value.Email);
        Assert.Single(result.Value.Vehicles);

        var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == result.Value.CustomerId);
        Assert.NotNull(customer);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldFail()
    {
        var customer = CustomerFactory.CreateCustomer(email: "duplicate@example.com").Value;

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync(default);

        var command = new CreateCustomerCommand(
            "Mohammed",
            "5555555555",
            "duplicate@example.com",
            [new CreateVehicleCommand("Honda", "Accord", 2024, "ABC123")]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
    }
}
