using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
using MechanicShop.Domain.RepairTasks.Enums;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.CreateRepairTask;

public class CreateRepairTaskCommandTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var parts = new List<CreateRepairTaskPartCommand>
        {
            new("Oil Filter", 10m, 1)
        };

        var command = new CreateRepairTaskCommand("Oil Change", 50m, RepairDurationInMinutes.Min60, parts);

        Assert.Equal("Oil Change", command.Name);
        Assert.Equal(50m, command.LaborCost);
        Assert.Equal(RepairDurationInMinutes.Min60, command.EstimatedDurationInMins);
        Assert.Same(parts, command.Parts);
    }
}
