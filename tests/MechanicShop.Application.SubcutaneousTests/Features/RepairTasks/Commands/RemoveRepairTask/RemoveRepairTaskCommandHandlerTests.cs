using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.RepaireTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.RemoveRepairTask;

[Collection(WebAppFactoryCollection.CollectionName)]
public class RemoveRepairTaskCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var repairTask = RepairTaskFactory.CreateRepairTask().Value;

        await _context.RepairTasks.AddAsync(repairTask);
        await _context.SaveChangesAsync(default);

        var command = new RemoveRepairTaskCommand(repairTask.Id);

        var result = await _mediator.Send(command);

        Assert.True(result.IsSuccess);

        var deletedRepairTask = await _context.RepairTasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == repairTask.Id);
        Assert.Null(deletedRepairTask);
    }

    [Fact]
    public async Task Handle_WithMissingRepairTask_ShouldFail()
    {
        var command = new RemoveRepairTaskCommand(Guid.NewGuid());

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
    }
}
