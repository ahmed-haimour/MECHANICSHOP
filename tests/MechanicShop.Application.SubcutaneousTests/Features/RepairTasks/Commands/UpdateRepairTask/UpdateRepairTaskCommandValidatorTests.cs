using MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
using MechanicShop.Domain.RepairTasks.Enums;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.UpdateRepairTask;

public class UpdateRepairTaskCommandValidatorTests
{
    private readonly UpdateRepairTaskCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_RepairTaskId_Is_Empty()
    {
        var command = ValidCommand() with { RepairTaskId = Guid.Empty };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "RepairTaskId");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = ValidCommand() with { Name = "" };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Have_Error_When_LaborCost_Is_OutOfRange()
    {
        var command = ValidCommand() with { LaborCost = 0 };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LaborCost");
    }

    [Fact]
    public void Should_Have_Error_When_Parts_Is_Empty()
    {
        var command = ValidCommand() with { Parts = [] };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parts");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var result = _validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    private static UpdateRepairTaskCommand ValidCommand()
    {
        return new UpdateRepairTaskCommand(
            Guid.NewGuid(),
            "Oil Change",
            50m,
            RepairDurationInMinutes.Min60,
            [new UpdateRepairTaskPartCommand(Guid.NewGuid(), "Oil Filter", 10m, 1)]);
    }
}
