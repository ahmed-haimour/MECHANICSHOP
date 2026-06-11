using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Queries.GetCustomerById;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.Customers;
using MediatR;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Customers.Queries.GetCustomerById;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetCustomerByIdQueryHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithExistingCustomer_ShouldSucceed()
    {
        var customer = CustomerFactory.CreateCustomer().Value;

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync(default);

        var result = await _mediator.Send(new GetCustomerByIdQuery(customer.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(customer.Id, result.Value.CustomerId);
    }

    [Fact]
    public async Task Handle_WithMissingCustomer_ShouldFail()
    {
        var result = await _mediator.Send(new GetCustomerByIdQuery(Guid.NewGuid()));

        Assert.True(result.IsError);
    }
}
