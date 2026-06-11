using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.RepaireTasks;
using MediatR;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Queries.GetRepairTaskById;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetRepairTaskByIdQueryHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithExistingRepairTask_ShouldSucceed()
    {
        var repairTask = RepairTaskFactory.CreateRepairTask().Value;

        await _context.RepairTasks.AddAsync(repairTask);
        await _context.SaveChangesAsync(default);

        var query = new GetRepairTaskByIdQuery(repairTask.Id);

        var result = await _mediator.Send(query);

        Assert.True(result.IsSuccess);
        Assert.Equal(repairTask.Id, result.Value.RepairTaskId);
    }

    [Fact]
    public async Task Handle_WithMissingRepairTask_ShouldFail()
    {
        var query = new GetRepairTaskByIdQuery(Guid.NewGuid());

        var result = await _mediator.Send(query);

        Assert.True(result.IsError);
    }
}
