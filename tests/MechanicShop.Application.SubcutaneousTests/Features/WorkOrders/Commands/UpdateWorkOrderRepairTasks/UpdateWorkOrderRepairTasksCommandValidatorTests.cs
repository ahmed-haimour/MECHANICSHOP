using MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks;

public class UpdateWorkOrderRepairTasksCommandValidatorTests
{
    private readonly UpdateWorkOrderRepairTasksCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_WorkOrderId_Is_Empty()
    {
        var command = new UpdateWorkOrderRepairTasksCommand(Guid.Empty, [Guid.NewGuid()]);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WorkOrderId");
    }

    [Fact]
    public void Should_Have_Error_When_RepairTaskIds_Is_Empty()
    {
        var command = new UpdateWorkOrderRepairTasksCommand(Guid.NewGuid(), []);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "RepairTaskIds");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var command = new UpdateWorkOrderRepairTasksCommand(Guid.NewGuid(), [Guid.NewGuid()]);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
