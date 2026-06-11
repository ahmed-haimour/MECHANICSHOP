using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
using MechanicShop.Domain.RepairTasks.Enums;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Commands.CreateRepairTask;

public class CreateRepairTaskCommandValidatorTests
{
    private readonly CreateRepairTaskCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = ValidCommand() with { Name = "" };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Have_Error_When_LaborCost_Is_Not_Positive()
    {
        var command = ValidCommand() with { LaborCost = 0 };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LaborCost");
    }

    [Fact]
    public void Should_Have_Error_When_EstimatedDuration_Is_Null()
    {
        var command = ValidCommand() with { EstimatedDurationInMins = null };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EstimatedDurationInMins");
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

    private static CreateRepairTaskCommand ValidCommand()
    {
        return new CreateRepairTaskCommand(
            "Oil Change",
            50m,
            RepairDurationInMinutes.Min60,
            [new CreateRepairTaskPartCommand("Oil Filter", 10m, 1)]);
    }
}
