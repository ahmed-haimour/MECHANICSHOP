using MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.UpdateRepairTask;

public class UpdateRepairTaskPartCommandValidatorTests
{
    private readonly UpdateRepairTaskPartCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new UpdateRepairTaskPartCommand(Guid.NewGuid(), "", 10m, 1);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Have_Error_When_Cost_Is_OutOfRange()
    {
        var command = new UpdateRepairTaskPartCommand(Guid.NewGuid(), "Oil Filter", 0, 1);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Cost");
    }

    [Fact]
    public void Should_Have_Error_When_Quantity_Is_OutOfRange()
    {
        var command = new UpdateRepairTaskPartCommand(Guid.NewGuid(), "Oil Filter", 10m, 0);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Quantity");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var command = new UpdateRepairTaskPartCommand(Guid.NewGuid(), "Oil Filter", 10m, 1);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
