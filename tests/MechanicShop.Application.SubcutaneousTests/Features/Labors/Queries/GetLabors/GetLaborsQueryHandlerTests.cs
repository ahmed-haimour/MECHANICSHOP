using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Queries.GetLabors;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.Employees;
using MediatR;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Labors.Queries.GetLabors;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetLaborsQueryHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_ShouldReturnLaborsOnly()
    {
        var labor = EmployeeFactory.CreateLabor().Value;
        var manager = EmployeeFactory.CreateManager().Value;

        await _context.Employees.AddRangeAsync(labor, manager);
        await _context.SaveChangesAsync(default);

        var result = await _mediator.Send(new GetLaborsQuery());

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value, x => x.LaborId == labor.Id);
        Assert.DoesNotContain(result.Value, x => x.LaborId == manager.Id);
    }
}
