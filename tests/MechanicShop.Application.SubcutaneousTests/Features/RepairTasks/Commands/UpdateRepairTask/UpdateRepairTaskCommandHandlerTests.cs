using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Tests.Common.RepaireTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.UpdateRepairTask;

[Collection(WebAppFactoryCollection.CollectionName)]
public class UpdateRepairTaskCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var repairTask = RepairTaskFactory.CreateRepairTask(name: "Old Service").Value;

        await _context.RepairTasks.AddAsync(repairTask);
        await _context.SaveChangesAsync(default);

        var command = new UpdateRepairTaskCommand(
            repairTask.Id,
            "Updated Service",
            150m,
            RepairDurationInMinutes.Min60,
            [new UpdateRepairTaskPartCommand(null, "Filter", 15m, 1)]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsSuccess);

        var updatedRepairTask = await _context.RepairTasks
            .AsNoTracking()
            .Include(x => x.Parts)
            .FirstAsync(x => x.Id == repairTask.Id);

        Assert.Equal("Updated Service", updatedRepairTask.Name);
        Assert.Single(updatedRepairTask.Parts);
    }

    [Fact]
    public async Task Handle_WithMissingRepairTask_ShouldFail()
    {
        var command = new UpdateRepairTaskCommand(
            Guid.NewGuid(),
            "Updated Service",
            150m,
            RepairDurationInMinutes.Min60,
            [new UpdateRepairTaskPartCommand(null, "Filter", 15m, 1)]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
    }
}
