using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Tests.Common.RepaireTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.CreateRepairTask;

[Collection(WebAppFactoryCollection.CollectionName)]
public class CreateRepairTaskCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var command = new CreateRepairTaskCommand(
            "Transmission Service",
            120m,
            RepairDurationInMinutes.Min60,
            [new CreateRepairTaskPartCommand("Fluid", 25m, 2)]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsSuccess);
        Assert.Equal("Transmission Service", result.Value.Name);
        Assert.Single(result.Value.Parts);

        var repairTask = await _context.RepairTasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == result.Value.RepairTaskId);
        Assert.NotNull(repairTask);
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldFail()
    {
        var repairTask = RepairTaskFactory.CreateRepairTask(name: "Duplicate Service").Value;

        await _context.RepairTasks.AddAsync(repairTask);
        await _context.SaveChangesAsync(default);

        var command = new CreateRepairTaskCommand(
            "Duplicate Service",
            120m,
            RepairDurationInMinutes.Min60,
            [new CreateRepairTaskPartCommand("Fluid", 25m, 2)]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
    }
}
