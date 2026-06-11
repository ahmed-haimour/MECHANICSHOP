using MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.UpdateRepairTask;

public class UpdateRepairTaskPartCommandTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var partId = Guid.NewGuid();

        var command = new UpdateRepairTaskPartCommand(partId, "Oil Filter", 10m, 2);

        Assert.Equal(partId, command.PartId);
        Assert.Equal("Oil Filter", command.Name);
        Assert.Equal(10m, command.Cost);
        Assert.Equal(2, command.Quantity);
    }
}
