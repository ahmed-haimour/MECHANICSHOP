using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.CreateRepairTask;

public class CreateRepairTaskPartCommandTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var command = new CreateRepairTaskPartCommand("Oil Filter", 10m, 2);

        Assert.Equal("Oil Filter", command.Name);
        Assert.Equal(10m, command.Cost);
        Assert.Equal(2, command.Quantity);
    }
}
