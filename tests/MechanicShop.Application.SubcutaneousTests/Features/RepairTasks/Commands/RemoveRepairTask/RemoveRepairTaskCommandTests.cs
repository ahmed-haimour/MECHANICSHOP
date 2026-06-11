using MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.RemoveRepairTask;

public class RemoveRepairTaskCommandTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var repairTaskId = Guid.NewGuid();

        var command = new RemoveRepairTaskCommand(repairTaskId);

        Assert.Equal(repairTaskId, command.RepairTaskId);
    }
}
