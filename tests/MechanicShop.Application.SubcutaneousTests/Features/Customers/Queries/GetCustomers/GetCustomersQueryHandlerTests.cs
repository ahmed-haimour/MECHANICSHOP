using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Queries.GetCustomers;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.Customers;
using MediatR;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Queries.GetCustomers;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetCustomersQueryHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_ShouldReturnCustomers()
    {
        var customer = CustomerFactory.CreateCustomer().Value;

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync(default);

        var result = await _mediator.Send(new GetCustomersQuery());

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value, x => x.CustomerId == customer.Id);
    }
}
