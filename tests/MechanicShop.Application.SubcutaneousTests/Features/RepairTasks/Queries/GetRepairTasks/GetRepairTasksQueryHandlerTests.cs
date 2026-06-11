using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.RepaireTasks;
using MediatR;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Queries.GetRepairTasks;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetRepairTasksQueryHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_ShouldReturnRepairTasks()
    {
        var repairTask = RepairTaskFactory.CreateRepairTask().Value;

        await _context.RepairTasks.AddAsync(repairTask);
        await _context.SaveChangesAsync(default);

        var result = await _mediator.Send(new GetRepairTasksQuery());

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value, x => x.RepairTaskId == repairTask.Id);
    }
}
