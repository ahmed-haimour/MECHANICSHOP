using MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
using MechanicShop.Domain.RepairTasks.Enums;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.UpdateRepairTask;

public class UpdateRepairTaskCommandTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var repairTaskId = Guid.NewGuid();
        var parts = new List<UpdateRepairTaskPartCommand>
        {
            new(Guid.NewGuid(), "Oil Filter", 10m, 1)
        };

        var command = new UpdateRepairTaskCommand(repairTaskId, "Oil Change", 50m, RepairDurationInMinutes.Min60, parts);

        Assert.Equal(repairTaskId, command.RepairTaskId);
        Assert.Equal("Oil Change", command.Name);
        Assert.Equal(50m, command.LaborCost);
        Assert.Equal(RepairDurationInMinutes.Min60, command.EstimatedDurationInMins);
        Assert.Same(parts, command.Parts);
    }
}
