using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Queries.GetRepairTaskById;

public class GetRepairTaskByIdQueryValidatorTests
{
    private readonly GetRepairTaskByIdQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_RepairTaskId_Is_Empty()
    {
        var query = new GetRepairTaskByIdQuery(Guid.Empty);

        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "RepairTaskId");
    }

    [Fact]
    public void Should_Pass_When_RepairTaskId_Is_Valid()
    {
        var query = new GetRepairTaskByIdQuery(Guid.NewGuid());

        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }
}
